<script setup>
import { ref } from 'vue';
import adminService from '@/services/adminService';

const props = defineProps({
  selectedCategory: { type: Object, required: true },
  closeForm: { type: Function },
});

const emit = defineEmits(['refresh-data']);

const localCategory = ref({ ...props.selectedCategory });

const submitEdit = async () => {
  if (!localCategory.value) {
    return;
  }

  try {
    await adminService.adminEditCategory(
      localCategory.value.idCategory,
      localCategory.value.nameCategory
    );
    console.log('Информация о категории обновлена.');
    emit('refresh-data');
    props.closeForm();
  } catch (error) {
    console.error('Ошибка при редактировании категории:', error);
  }
};

const submitDelete = async () => {
  if (!localCategory.value) {
    return;
  }

  try {
    await adminService.adminDeleteCategory(localCategory.value.idCategory);
    console.log('Категория удалена.');
    emit('refresh-data');
    props.closeForm();
  } catch (error) {
    console.error('Ошибка при удалении категории:', error);
  }
};
</script>

<template>
  <main>
    <h1>Редактирование категории</h1>
    <div class="section">
      <div class="form-section">
        <label>Название:</label>
        <input type="text" v-model="localCategory.nameCategory" />
        <label>Количество книг:</label>
        <input type="text" v-model="localCategory.countBooks" readonly />
        <div v-if="localCategory.countBooks > 0">
          <label>Книги:</label>
          <ul class="book-list">
            <li
              v-for="book in localCategory.books"
              :key="book.idBook"
              class="book-item"
            >
              <img
                :src="book.imageURL"
                :alt="book.titleBook"
                class="book-cover"
              />
              <span>{{ book.titleBook }}</span>
            </li>
          </ul>
        </div>
        <div class="form-buttons">
          <button class="button cancel" @click="closeForm">Отмена</button>
          <button
            v-if="localCategory.countBooks === 0"
            class="button cancel"
            @click="submitDelete"
          >
            Удалить
          </button>
          <button class="button" @click="submitEdit">Сохранить</button>
        </div>
      </div>
    </div>
  </main>
</template>

<style scoped>
main {
  max-width: 1200px;
  margin: 0 auto;
  padding: 20px;
  border-radius: 5px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  display: flex;
  flex-direction: column;
  gap: 20px;
}

h1 {
  margin-bottom: 20px;
  font-size: 28px;
  text-align: center;
  text-decoration: underline;
  text-decoration-color: forestgreen;
}

.section {
  display: flex;
  gap: 20px;
}

.form-section {
  flex: 1;
  padding: 20px;
  height: fit-content;
  background-color: white;
  border-radius: 5px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

label {
  display: block;
  margin-bottom: 5px;
  font-weight: bold;
}

input {
  width: calc(100% - 40px);
  height: auto;
  padding: 10px;
  margin-bottom: 10px;
  border: 1px solid lightgrey;
  border-radius: 5px;
}

input:focus {
  outline: none;
  border-color: darkgreen;
}

.book-list {
  padding-left: 0;
  list-style-type: none;
}

.book-item {
  display: flex;
  align-items: center;
  gap: 15px;
  position: relative;
  padding-left: 25px;
  margin-bottom: 10px;
}

.book-item::before {
  content: '•';
  position: absolute;
  left: 0;
  top: 50%;
  transform: translateY(-50%);
  font-size: 24px;
  color: forestgreen;
}

.book-cover {
  height: 100px;
  border-radius: 5px;
}

.button {
  padding: 10px 20px;
  color: white;
  background-color: forestgreen;
  border: none;
  border-radius: 5px;
}

.button:hover {
  background-color: darkgreen;
}

.form-buttons {
  margin-top: 15px;
  display: flex;
  justify-content: center;
  gap: 15px;
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
