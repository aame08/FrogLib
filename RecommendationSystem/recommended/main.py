# uvicorn main:app --reload

# Импорт библиотек
from fastapi import FastAPI
import pandas as pd
import numpy as np
from sklearn.metrics.pairwise import cosine_similarity
from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.decomposition import TruncatedSVD
from scipy.sparse import hstack
import pymysql
import nltk
from nltk.corpus import stopwords
import asyncio
from datetime import datetime

# Загрузка русских стоп-слов
nltk.download('stopwords')
russian_stopwords = stopwords.words('russian')

app = FastAPI()
last_update_time = None
update_interval = 3600  # Обновление данных каждый час

# Параметры подключения к базе данных
db_config = {
    'host': 'localhost',
    'user': 'root',
    'password': 'ame0372',
    'database': 'froglib'
}

# Глобальные переменные для хранения данных и моделей
books = None
user_interactions = None
content_similarity = None
predicted_ratings = None
user_ids = None
book_ids = None


async def update_data():
    global books, user_interactions, content_similarity, predicted_ratings, user_ids, book_ids, last_update_time
    while True:
        print(f'[DEBUG] Обновление данных в {datetime.now()}')
        books, user_interactions = load_data()
        content_similarity = content_based_recommendation(books)
        predicted_ratings, user_ids, book_ids = collaborative_filtering(user_interactions, books)
        last_update_time = datetime.now()
        await asyncio.sleep(update_interval)


@app.on_event("startup")
async def startup():
    asyncio.create_task(update_data())


def load_data():
    connection = pymysql.connect(**db_config)

    books_query = """
    SELECT b.id_book, b.title_book, b.description_book, b.id_category, c.name_category 
    FROM Books b JOIN Categories c ON b.id_category = c.id_category
    """
    books = pd.read_sql(books_query, connection)
    books['clean_description'] = books['description_book'].fillna('').str.lower()

    user_books_query = "SELECT id_user, id_book, id_list_type FROM UserBooks"
    user_books = pd.read_sql(user_books_query, connection)

    ratings_query = "SELECT id_user, id_book, rating FROM BookRatings"
    ratings = pd.read_sql(ratings_query, connection)

    # Веса для списков (1 = Прочитано, 2 = Хочу прочитать, 3 = Отложено, 4 = Любимые, 5 = Не понравилось)
    list_weights = {1: 3, 2: 2, 3: -1, 4: 4, 5: 1}  # Уменьшил негативный вес для "Не понравилось"
    user_books['weight'] = user_books['id_list_type'].map(list_weights)

    # Объединение оценок и списков
    user_interactions = pd.concat([
        ratings.assign(weight=ratings['rating'] * 2),  # Увеличение веса явных оценок
        user_books[['id_user', 'id_book', 'weight']]
    ]).drop_duplicates(['id_user', 'id_book'], keep='first')

    connection.close()

    print(f"[DEBUG] Загружено {len(books)} книг и {len(user_interactions)} взаимодействий")
    return books, user_interactions


def content_based_recommendation(books):
    tfidf = TfidfVectorizer(stop_words=russian_stopwords, max_features=5000)
    tfidf_matrix = tfidf.fit_transform(books['clean_description'])

    # Добавление категории как дополнительные признаки
    category_dummies = pd.get_dummies(books['name_category'], sparse=True)
    content_features = hstack([tfidf_matrix, category_dummies])

    return cosine_similarity(content_features)


def collaborative_filtering(user_interactions, books):
    try:
        # Создание матрицы пользователь-книга
        user_book_matrix = user_interactions.pivot_table(
            index='id_user',
            columns='id_book',
            values='weight',
            fill_value=0
        )

        # Если матрица пустая (нет данных)
        if user_book_matrix.empty:
            print("[WARNING] user_book_matrix пустая! Возвращаю заглушки.")
            dummy_books = books['id_book'].unique()
            return np.zeros((1, len(dummy_books))), np.array([0]), dummy_books

        # Нормализация и SVD
        user_means = user_book_matrix.mean(axis=1)
        user_book_matrix = user_book_matrix.sub(user_means, axis=0)

        n_components = min(50, user_book_matrix.shape[1])
        svd = TruncatedSVD(n_components=n_components, random_state=42)
        user_factors = svd.fit_transform(user_book_matrix)
        book_factors = svd.components_.T

        predicted_ratings = np.dot(user_factors, book_factors.T)
        return predicted_ratings, user_book_matrix.index, user_book_matrix.columns

    except Exception as e:
        print(f"[ERROR] Ошибка в collaborative_filtering(): {e}")
        dummy_books = books['id_book'].unique()
        return np.zeros((1, len(dummy_books))), np.array([0]), dummy_books


@app.get("/recommend/{user_id}")
async def recommend(user_id: int):
    try:
        # Проверка, что данные загружены
        if book_ids is None or user_ids is None or predicted_ratings is None:
            print("[ERROR] Данные не загружены! Возвращаю популярные книги.")
            popular_books = user_interactions['id_book'].value_counts().head(5).index
            result = books[books['id_book'].isin(popular_books)][['id_book', 'title_book', 'name_category']]
            return [{"book_id": row.id_book, "title": row.title_book, "category": row.name_category} for row in
                    result.itertuples()]

        # Получение индекса пользователя
        user_idx = np.where(user_ids == user_id)[0]
        if len(user_idx) == 0:
            print(f"[ERROR] Пользователь {user_id} не найден!")
            popular_books = user_interactions['id_book'].value_counts().head(5).index
            result = books[books['id_book'].isin(popular_books)][['id_book', 'title_book', 'name_category']]
            return [{"book_id": row.id_book, "title": row.title_book, "category": row.name_category} for row in
                    result.itertuples()]

        user_idx = user_idx[0]
        collab_scores = predicted_ratings[user_idx]

        # Получение книг, с которыми пользователь взаимодействовал
        interacted_books = user_interactions[user_interactions['id_user'] == user_id]['id_book'].unique()
        print(f"[DEBUG] Пользователь {user_id} взаимодействовал с книгами: {interacted_books}")

        # Контентные рекомендации на основе любимых книг
        favorite_books = user_interactions[
            (user_interactions['id_user'] == user_id) &
            (user_interactions['weight'] >= 4)
            ]['id_book'].values

        if len(favorite_books) > 0:
            favorite_indices = books[books['id_book'].isin(favorite_books)].index
            content_scores = content_similarity[favorite_indices].mean(axis=0)
        else:
            interacted_indices = books[books['id_book'].isin(interacted_books)].index
            content_scores = content_similarity[interacted_indices].mean(axis=0) if len(
                interacted_indices) > 0 else np.zeros(len(books))

        # Нормализация оценок
        content_scores = (content_scores - content_scores.min()) / (content_scores.max() - content_scores.min() + 1e-10)
        collab_scores = (collab_scores - collab_scores.min()) / (collab_scores.max() - collab_scores.min() + 1e-10)

        # Выравнивание длины массивов
        min_length = min(len(book_ids), len(content_scores), len(collab_scores))
        book_ids_aligned = book_ids[:min_length]
        content_scores_aligned = content_scores[:min_length]
        collab_scores_aligned = collab_scores[:min_length]

        # Создание DataFrame
        recommendations = pd.DataFrame({
            'book_id': book_ids_aligned,
            'content_score': content_scores_aligned,
            'collab_score': collab_scores_aligned,
            'score': 0.6 * content_scores_aligned + 0.4 * collab_scores_aligned  # Добавляем score сразу
        })

        # Фильтрация уже просмотренные книги
        recommendations = recommendations[~recommendations['book_id'].isin(interacted_books)]
        print(f"[DEBUG] Рекомендаций после фильтрации: {len(recommendations)}")

        # Если ничего не осталось - возврат популярных
        if len(recommendations) == 0:
            print("[DEBUG] Все рекомендации отфильтрованы, возвращаем популярные книги")
            popular_books = user_interactions['id_book'].value_counts().head(5).index
            result = books[books['id_book'].isin(popular_books)][['id_book', 'title_book', 'name_category']]
            return [{"book_id": row.id_book, "title": row.title_book, "category": row.name_category} for row in
                    result.itertuples()]

        # Топ-5 рекомендаций
        top_books = recommendations.sort_values('score', ascending=False).head(5)

        # Добавление информации о книгах
        result = pd.merge(
            top_books,
            books[['id_book', 'title_book', 'name_category']],
            left_on='book_id',
            right_on='id_book',
            how='left'
        )

        return [{
            "book_id": row.book_id,
            "title": row.title_book,
            "category": row.name_category,
            "score": round(row.score, 2)
        } for row in result.itertuples()]

    except Exception as e:
        print(f"[ERROR] Ошибка в рекомендациях: {str(e)}")
        # Возвращение популярных книги в случае ошибки
        popular_books = user_interactions['id_book'].value_counts().head(5).index
        result = books[books['id_book'].isin(popular_books)][['id_book', 'title_book', 'name_category']]
        return [{"book_id": row.id_book, "title": row.title_book, "category": row.name_category} for row in
                result.itertuples()]