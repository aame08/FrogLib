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
const message = ref('');
const errors = ref({});

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
  errors.value = {};

  if (!localBook.value) {
    return;
  }

  try {
    const bookData = {
      id: localBook.value.id,
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
    if (error.response && error.response.status === 400) {
      if (error.response.data && error.response.data.errors) {
        const apiErrors = error.response.data.errors;

        errors.value = {
          Publisher: apiErrors.Publisher?.[0],
          Category: apiErrors.Category?.[0],
          Authors: apiErrors.Authors?.[0],
          TitleBook: apiErrors.TitleBook?.[0],
          Description: apiErrors.Description?.[0],
          YearPublication: apiErrors.YearPublication?.[0],
          PageCount: apiErrors.PageCount?.[0],
          LanguageBook: apiErrors.LanguageBook?.[0],
        };
      } else {
        message.value = 'Ошибка при редактировании книги';
      }
    } else {
      message.value = 'Ошибка при редактировании книги.';
    }
  }
};
</script>

<template>
  <main>
    <h1>Редактирование книги</h1>
    <div class="section">
      <div class="form-section">
        <div v-if="message" class="message">{{ message }}</div>
        <label>Обложка:</label>
        <img :src="localBook.imageUrl" :alt="localBook.title" />
        <label>ISBN-13:</label>
        <input type="number" v-model="localBook.isbn13" readonly />
        <label>Издатель:</label>
        <input
          type="text"
          v-model="localBook.publisherName"
          :class="{ 'input-error': errors.Publisher }"
        />
        <div v-if="errors.Publisher" class="error-message">
          {{ errors.Publisher }}
        </div>
        <label>Категория:</label>
        <input
          type="text"
          :value="localBook.categoryName"
          :class="{ 'input-error': errors.Category }"
        />
        <div v-if="errors.Category" class="error-message">
          {{ errors.Category }}
        </div>
        <label>Название книги:</label>
        <input
          type="text"
          v-model="localBook.titleBook"
          :class="{ 'input-error': errors.TitleBook }"
        />
        <div v-if="errors.TitleBook" class="error-message">
          {{ errors.TitleBook }}
        </div>
        <label>Описание:</label>
        <textarea
          v-model="localBook.descriptionBook"
          :class="{ 'input-error': errors.Description }"
        ></textarea>
        <div v-if="errors.Description" class="error-message">
          {{ errors.Description }}
        </div>
        <label>Автор(ы):</label>
        <div
          class="author-list"
          v-for="(author, index) in authors"
          :key="index"
          :class="{ 'input-error': errors.Authors }"
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
        <div v-if="errors.Authors" class="error-message">
          {{ errors.Authors }}
        </div>
        <label>Год издания:</label>
        <input
          type="number"
          v-model="localBook.yearPublication"
          :class="{ 'input-error': errors.YearPublication }"
        />
        <div v-if="errors.YearPublication" class="error-message">
          {{ errors.YearPublication }}
        </div>
        <label>Количество страниц:</label>
        <input
          type="number"
          v-model="localBook.pageCount"
          :class="{ 'input-error': errors.PageCount }"
        />
        <div v-if="errors.PageCount" class="error-message">
          {{ errors.PageCount }}
        </div>
        <label>Язык книги:</label>
        <input
          type="text"
          v-model="localBook.languageBook"
          :class="{ 'input-error': errors.LanguageBook }"
        />
        <div v-if="errors.LanguageBook" class="error-message">
          {{ errors.LanguageBook }}
        </div>
        <label>Статус:</label>
        <select v-model="localBook.statusBook">
          <option value="" :selected="localBook.statusBook == ''">
            Без статуса
          </option>
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

.error-message {
  color: crimson;
  font-size: 14px;
}

.input-error {
  border: 2px solid darkred;
}

.message {
  color: grey;
  text-align: center;
}
</style>
