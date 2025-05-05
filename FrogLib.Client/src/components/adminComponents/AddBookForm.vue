<script setup>
import { ref } from 'vue';
import adminService from '@/services/adminService';

import SearchBook from './SearchBook.vue';

const props = defineProps({
  closeForm: { type: Function },
});

const emit = defineEmits(['refresh-data']);

const selectedBook = ref(null);
const authors = ref([{ lastName: '', firstName: '', middleName: '' }]);
const statusBook = ref('');

const handleBookSelect = (book) => {
  selectedBook.value = book;

  authors.value = book.authors.map((author) => {
    const parts = author.split(' ');
    return {
      lastName: parts[0] || '',
      firstName: parts.length > 1 ? parts[1] : '',
      middleName: parts.length > 2 ? parts[2] : '',
    };
  });

  if (authors.value.length === 0) {
    authors.value = [{ lastName: '', firstName: '', middleName: '' }];
  }
};

const addAuthor = () => {
  authors.value.push({ lastName: '', firstName: '', middleName: '' });
};

const removeAuthor = (index) => {
  if (authors.value.length > 1) {
    authors.value.splice(index, 1);
  }
};

const submitBook = async () => {
  if (!selectedBook.value) {
    return;
  }

  try {
    const bookData = {
      isbn10: selectedBook.value.isbn10 || '',
      isbn13: selectedBook.value.isbn13 || '',
      publisherName: selectedBook.value.publisher || '',
      categoryName: selectedBook.value.categories?.[0] || '',
      imageUrl: selectedBook.value.imageUrl || '',
      titleBook: selectedBook.value.title || '',
      descriptionBook: selectedBook.value.description || '',
      yearPublication:
        selectedBook.value.yearPublication || new Date().getFullYear(),
      pageCount: selectedBook.value.pageCount || 0,
      languageBook: selectedBook.value.language || 'ru',
      statusBook: statusBook.value || '',
      authors: authors.value.map((a) => ({
        surnameAuthor: a.lastName || '',
        nameAuthor: a.firstName || '',
        patronymicAuthor: a.middleName || '',
      })),
    };

    await adminService.adminAddBook(bookData);
    console.log('Книга добавлена.');
    emit('refresh-data');
    props.closeForm();
  } catch (error) {
    console.log('Ошибка при добавлении книги:', error);
  }
};
</script>

<template>
  <main>
    <h1>Добавление книги</h1>
    <div class="section">
      <SearchBook @select-book="handleBookSelect" />
      <div class="form-section">
        <h2>Детали книги</h2>
        <template v-if="selectedBook">
          <label>Обложка:</label>
          <img :src="selectedBook.imageUrl" :alt="selectedBook.title" />
          <label>ISBN-10:</label>
          <input type="number" v-model="selectedBook.isbn10" />
          <label>ISBN-13:</label>
          <input type="number" v-model="selectedBook.isbn13" />
          <label>Издатель:</label>
          <input type="text" v-model="selectedBook.publisher" />
          <label>Категория:</label>
          <input type="text" :value="selectedBook.categories.join(', ')" />
          <label>Название книги:</label>
          <input type="text" v-model="selectedBook.title" />
          <label>Описание:</label>
          <textarea v-model="selectedBook.description"></textarea>
          <label>Автор(ы):</label>
          <div
            class="author-list"
            v-for="(author, index) in authors"
            :key="index"
          >
            <div class="author">
              <div>
                <label>Фамилия:</label>
                <input type="text" v-model="author.lastName" />
              </div>
              <div>
                <label>Имя:</label>
                <input type="text" v-model="author.firstName" />
              </div>
              <div>
                <label>Отчество:</label>
                <input type="text" v-model="author.middleName" />
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
          <input type="number" v-model="selectedBook.yearPublication" />
          <label>Количество страниц:</label>
          <input type="number" v-model="selectedBook.pageCount" />
          <label>Язык книги:</label>
          <input type="text" v-model="selectedBook.language" />
          <label>Статус:</label>
          <select v-model="selectedBook.statusBook">
            <option value="">Без статуса</option>
            <option value="Новинка">Новинка</option>
            <option value="Бестселлер">Бестселлер</option>
          </select>
        </template>
      </div>
    </div>
    <div class="form-buttons">
      <button class="button cancel" @click="closeForm">Отмена</button>
      <button v-if="selectedBook" class="button" @click="submitBook">
        Сохранить
      </button>
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
  margin-top: 30px;
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
