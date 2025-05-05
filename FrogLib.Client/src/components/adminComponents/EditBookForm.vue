<script setup>
import { ref } from 'vue';
import adminService from '@/services/adminService';

const props = defineProps({
  selectedBook: { type: Object, required: true },
  closeForm: { type: Function },
});

const emit = defineEmits(['refresh-data']);

const localBook = ref({ ...props.selectedBook });
const authors = ref(
  props.selectedBook.authors.map((a) => ({
    surnameAuthor: a.surnameAuthor || '',
    nameAuthor: a.nameAuthor || '',
    patronymicAuthor: a.patronymicAuthor || '',
  }))
);

const addAuthor = () => {
  authors.value.push({
    surnameAuthor: '',
    nameAuthor: '',
    patronymicAuthor: '',
  });
};

const removeAuthor = (index) => {
  if (authors.value.length > 1) {
    authors.value.splice(index, 1);
  }
};

const submitEdit = async () => {
  if (!localBook.value) {
    return;
  }

  try {
    const bookData = {
      id: localBook.value.id,
      isbn10: localBook.value.isbn10 || '',
      isbn13: localBook.value.isbn13 || '',
      publisherName: localBook.value.publisherName || '',
      categoryName: localBook.value.categoryName || '',
      imageUrl: localBook.value.imageUrl || '',
      titleBook: localBook.value.titleBook || '',
      descriptionBook: localBook.value.descriptionBook || '',
      yearPublication:
        localBook.value.yearPublication || new Date().getFullYear(),
      pageCount: localBook.value.pageCount || 0,
      languageBook: localBook.value.languageBook || 'ru',
      statusBook: localBook.value.statusBook || '',
      authors: authors.value.map((a) => ({
        surnameAuthor: a.surnameAuthor || '',
        nameAuthor: a.nameAuthor || '',
        patronymicAuthor: a.patronymicAuthor || '',
      })),
    };

    await adminService.adminEditBook(localBook.value.id, bookData);
    console.log('Информация о книге обновлена.');
    emit('refresh-data');
    props.closeForm();
  } catch (error) {
    console.error('Ошибка при редактировании книги:', error);
  }
};
</script>

<template>
  <main>
    <h1>Редактирование книги</h1>
    <div class="section">
      <div class="form-section">
        <label>Обложка:</label>
        <img :src="localBook.imageUrl" :alt="localBook.title" />
        <label>ISBN-10:</label>
        <input type="number" v-model="localBook.isbn10" />
        <label>ISBN-13:</label>
        <input type="number" v-model="localBook.isbn13" />
        <label>Издатель:</label>
        <input type="text" v-model="localBook.publisherName" />
        <label>Категория:</label>
        <input type="text" :value="localBook.categoryName" />
        <label>Название книги:</label>
        <input type="text" v-model="localBook.titleBook" />
        <label>Описание:</label>
        <textarea v-model="localBook.descriptionBook"></textarea>
        <label>Автор(ы):</label>
        <div
          class="author-list"
          v-for="(author, index) in authors"
          :key="index"
        >
          <div class="author">
            <div>
              <label>Фамилия:</label>
              <input type="text" v-model="author.surnameAuthor" />
            </div>
            <div>
              <label>Имя:</label>
              <input type="text" v-model="author.nameAuthor" />
            </div>
            <div>
              <label>Отчество:</label>
              <input type="text" v-model="author.patronymicAuthor" />
            </div>
            <button
              type="button"
              class="button remove-author"
              @click="removeAuthor(index)"
              v-if="authors.length > 1"
            >
              Удалить
            </button>
          </div>
          <button type="button" class="button add-author" @click="addAuthor">
            Добавить автора
          </button>
        </div>
        <label>Год издания:</label>
        <input type="number" v-model="localBook.yearPublication" />
        <label>Количество страниц:</label>
        <input type="number" v-model="localBook.pageCount" />
        <label>Язык книги:</label>
        <input type="text" v-model="localBook.languageBook" />
        <label>Статус:</label>
        <select v-model="localBook.statusBook">
          <option value="">Без статуса</option>
          <option value="Новинка">Новинка</option>
          <option value="Бестселлер">Бестселлер</option>
        </select>
        <div class="form-buttons">
          <button class="button cancel" @click="closeForm">Отмена</button>
          <button class="button" @click="submitEdit">Сохранить</button>
        </div>
      </div>
    </div>
  </main>
</template>

<style scoped>
main {
  padding: 20px;
  border-radius: 5px;
  max-width: 1200px;
  margin-left: auto;
  margin-right: auto;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  display: flex;
  flex-direction: column;
  gap: 20px;
}

h1 {
  font-size: 28px;
  text-align: center;
  text-decoration: underline;
  text-decoration-color: forestgreen;
  margin-bottom: 20px;
}

.section {
  display: flex;
  gap: 20px;
}

.form-section {
  flex: 1;
  height: fit-content;
  padding: 20px;
  background-color: white;
  border-radius: 5px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

label {
  display: block;
  font-weight: bold;
  margin-bottom: 5px;
}

input,
textarea,
select {
  width: calc(100% - 40px);
  padding: 10px;
  border: 1px solid lightgrey;
  border-radius: 5px;
  margin-bottom: 10px;
  height: auto;
}

input:focus,
textarea:focus,
select:focus {
  outline: none;
  border-color: darkgreen;
}

.author-list {
  margin-top: 10px;
}

.author {
  display: flex;
}

.author label {
  font-weight: normal;
}

.button {
  padding: 10px 20px;
  color: white;
  background-color: forestgreen;
  border: none;
  border-radius: 5px;
}

.add-author {
  margin-top: 5px;
  padding: 5px 10px;
}

img {
  width: 120px;
  border-radius: 5px;
}

.button:hover {
  background-color: darkgreen;
}
.form-buttons {
  margin-top: 15px;
  display: flex;
  gap: 15px;
  justify-content: center;
}
.button.cancel,
.remove-author {
  height: 40px;
  background-color: crimson;
}

.button.cancel:hover,
.remove-author:hover {
  background-color: darkred;
}
</style>
