# uvicorn main:app --reload

# импорт библиотек
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


# загрузка русских стоп-слов
nltk.download('stopwords')
russian_stopwords = stopwords.words('russian')


app = FastAPI()
last_update_time = None
update_interval = 3600

# параметры подключения к базе данных
db_config = {
    'host': 'localhost',
    'user': 'root',
    'password': 'ame0372',
    'database': 'froglib'
}

# глобальные переменные для хранения данных и моделей
books = None
user_interactions = None
content_similarity = None
predicted_ratings = None
user_ids = None
book_ids = None


async def update_data():
    global books, user_interactions, content_similarity, predicted_ratings, user_ids, book_ids, last_update_time
    while True:
        print(f'Обновление данных в {datetime.now()}')
        books, user_interactions = load_data()
        content_similarity = content_based_recommendation(books)
        predicted_ratings, user_ids, book_ids = collaborative_filtering(user_interactions, books)
        last_update_time = datetime.now()
        await asyncio.sleep(update_interval)


@app.on_event("startup")
async def startup():
    asyncio.create_task(update_data())


# загрузка данных из БД
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

    # создание весов для списков
    list_weights = {1: 3, 2: 2, 3: -1, 4: 4, 5: 5}
    # добавление веса к спискам пользователей
    user_books['weight'] = user_books['id_list_type'].map(list_weights)

    # объединение оценки и списка
    user_interactions = pd.concat([
        ratings.assign(weight=ratings['rating'] * 2),
        user_books[['id_user', 'id_book', 'weight']]
    ]).drop_duplicates(['id_user', 'id_book'], keep='first')

    connection.close()
    return books, user_interactions


# контентная фильтрация
def content_based_recommendation(books):
    tfidf = TfidfVectorizer(stop_words=russian_stopwords, max_features=5000)
    tfidf_matrix = tfidf.fit_transform(books['clean_description'])

    # добавление категорий как доп признаки
    category_dummies = pd.get_dummies(books['name_category'], sparse=True)
    # объединение признаков
    content_features = hstack([tfidf_matrix, category_dummies])

    # косинусное сходство
    return cosine_similarity(content_features)


# коллаборативная фильтрация
def collaborative_filtering(user_interactions, books):
    # матрица пользователь-книга
    user_book_matrix = user_interactions.pivot_table(
        index='id_user',
        columns='id_book',
        values='weight',
        fill_value=0
    )

    # нормализация оценок
    user_means = user_book_matrix.mean(axis=1)
    user_book_matrix = user_book_matrix.sub(user_means, axis=0)

    n_components = min(50, user_book_matrix.shape[1]) # ограничение числа компонентов

    # SVD для уменьшения размерности
    svd = TruncatedSVD(n_components=n_components, random_state=42)
    user_factors = svd.fit_transform(user_book_matrix)
    book_factors = svd.components_.T

    # вычисление предсказанных оценок
    predicted_ratings = np.dot(user_factors, book_factors.T)

    return predicted_ratings, user_book_matrix.index, user_book_matrix.columns


# получение рекомендация для пользователя
@app.get("/recommend/{user_id}")
async def recommend(user_id: int):
    try:
        # получение индекса пользователя для коллаборативной фильтрации
        user_idx = np.where(user_ids == user_id)[0][0]
        # получение предсказанных оценок от коллаборативной фильтрации
        collab_scores = predicted_ratings[user_idx]

        # получение книг, с которыми пользователь уже взаимодействовал
        interacted_books = user_interactions[user_interactions['id_user'] == user_id]['id_book'].values

        # усреднение сходства по любимым книгам для контентной фильтрации
        favorite_books = user_interactions[
            (user_interactions['id_user'] == user_id) &
            (user_interactions['weight'] >= 4) # Любимые и прочитанные
            ]['id_book'].values

        if len(favorite_books) > 0:
            # нахождение индексов любимых книг
            favorite_indices = books[books['id_book'].isin(favorite_books)].index
            # усреднение сходства с любимыми книгами
            content_scores = content_similarity[favorite_indices].mean(axis=0)
        else:
            # если нет любимых, используются все взаимодействия
            interacted_indices = books[books['id_book'].isin(interacted_books)].index
            if len(interacted_indices) > 0:
                content_scores = content_similarity[interacted_indices].mean(axis=0)
            else:
                # если нет взаимодействий, возвращение популярных
                popular_books = user_interactions['id_book'].value_counts().head(5).index
                result = books[books['id_book'].isin(popular_books)][['id_book', 'title_book', 'name_category']]
                return [{"book_id": row.id_book, "title": row.title_book, "category": row.name_category}
                        for row in result.itertuples()]

        # нормализация оценок
        content_scores = (content_scores - content_scores.min()) / (content_scores.max() - content_scores.min())
        collab_scores = (collab_scores - collab_scores.min()) / (collab_scores.max() - collab_scores.min())

        # формирование рекомендаций
        recommendations = pd.DataFrame({
            'book_id': book_ids,
            'content_score': content_scores,
            'collab_score': collab_scores
        })

        # расчёт оценки
        recommendations['score'] = 0.6 * recommendations['content_score'] + 0.4 * recommendations['collab_score']
        # исключение уже взаимодействовавших книги
        recommendations = recommendations[~recommendations['book_id'].isin(interacted_books)]
        # сортировка топ-5
        top_books = recommendations.sort_values('score', ascending=False).head(5)

        # добавление информации о книгах
        result = pd.merge(
            top_books,
            books[['id_book', 'title_book', 'name_category']],
            left_on='book_id',
            right_on='id_book',
            how='left'
        )

        return [{"book_id": row.book_id, "title": row.title_book, "category": row.name_category, "score": row.score}
                for row in result.itertuples()]

    except Exception as e:
        # для новых пользователей популярные книги
        popular_books = user_interactions['id_book'].value_counts().head(5).index
        result = books[books['id_book'].isin(popular_books)][['id_book', 'title_book', 'name_category']]
        return [{"book_id": row.id_book, "title": row.title_book, "category": row.name_category}
                for row in result.itertuples()]
