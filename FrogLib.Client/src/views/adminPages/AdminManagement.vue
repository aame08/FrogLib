<script setup>
import { ref, computed } from 'vue';
import adminService from '@/services/adminService';
import booksService from '@/services/booksService';

import BooksTable from '@/components/adminComponents/BooksTable.vue';
import AddBookForm from '@/components/adminComponents/AddBookForm.vue';
import EditBookForm from '@/components/adminComponents/EditBookForm.vue';
import AuthorsTable from '@/components/adminComponents/AuthorsTable.vue';
import EditAuthorForm from '@/components/adminComponents/EditAuthorForm.vue';
import CategoriesTable from '@/components/adminComponents/CategoriesTable.vue';
import EditCategoryForm from '@/components/adminComponents/EditCategoryForm.vue';

const activeTab = ref('books');
const books = ref([]);
const categories = ref([]);
const searchQueryBook = ref('');
const selectedCategory = ref('all');
const selectedBook = ref(null);
const showAddBookForm = ref(false);
const showEditBookForm = ref(false);
const authors = ref([]);
const searchQueryAuthor = ref('');
const selectedAuthor = ref(null);
const showEditAuthorForm = ref(false);
const categoriesTable = ref([]);
const searchQueryCategory = ref('');
const selectedCategoryTable = ref(null);
const showEditCategoryForm = ref(false);

const getBooks = async () => {
  try {
    const response = await adminService.getAdminBooks();
    books.value = response;
  } catch (error) {
    console.error('Ошибка при загрузке книг:', error);
  }
};
getBooks();

const getCategories = async () => {
  try {
    const response = await booksService.getCategories();
    categories.value = response;
  } catch (error) {
    console.error('Ошибка при загрузке категорий:', error);
  }
};
getCategories();

const filteredBooks = computed(() => {
  let result = books.value;

  if (searchQueryBook.value) {
    const query = searchQueryBook.value.toLowerCase();
    result = result.filter((book) =>
      book.titleBook.toLowerCase().includes(query)
    );
  }

  if (selectedCategory.value !== 'all') {
    result = result.filter(
      (book) => book.categoryName == selectedCategory.value
    );
  }

  return result;
});

const openAddBookForm = () => {
  showAddBookForm.value = true;
};

const closeAddBookForm = () => {
  showAddBookForm.value = false;
};

const openEditBookForm = (book) => {
  selectedBook.value = book;
  showEditBookForm.value = true;
};

const closeEditBookForm = () => {
  showEditBookForm.value = false;
};

const getAuthors = async () => {
  try {
    const response = await adminService.getAdminAuthors();
    authors.value = response;
  } catch (error) {
    console.log('Ошибка при загрузке авторов:', error);
  }
};
getAuthors();

const filteredAuthors = computed(() => {
  let result = authors.value;

  if (searchQueryAuthor.value) {
    const query = searchQueryAuthor.value.toLowerCase();
    result = result.filter((author) =>
      author.surnameAuthor.toLowerCase().includes(query)
    );
  }

  return result;
});

const openEditAuthorForm = (author) => {
  selectedAuthor.value = author;
  showEditAuthorForm.value = true;
};

const closeEditAuthorForm = () => {
  showEditAuthorForm.value = false;
};

const getCategoriesTable = async () => {
  try {
    const response = await adminService.getAdminCategories();
    categoriesTable.value = response;
  } catch (error) {
    console.error('Ошибка при загрузке категорий:', error);
  }
};
getCategoriesTable();

const filteredCategories = computed(() => {
  let result = categoriesTable.value;

  if (searchQueryCategory.value) {
    const query = searchQueryCategory.value.toLowerCase();
    result = result.filter((category) =>
      category.nameCategory.toLowerCase().includes(query)
    );
  }

  return result;
});

const openEditCategoryForm = (category) => {
  selectedCategoryTable.value = category;
  showEditCategoryForm.value = true;
};

const closeEditCategoryForm = () => {
  showEditCategoryForm.value = false;
};
</script>

<template>
  <div
    v-if="
      !showAddBookForm &&
      !showEditBookForm &&
      !showEditAuthorForm &&
      !showEditCategoryForm
    "
  >
    <h1>Управление контентом</h1>
    <div class="tabs">
      <button
        :class="{ active: activeTab === 'books' }"
        @click="activeTab = 'books'"
      >
        Книги
      </button>
      <button
        :class="{ active: activeTab === 'authors' }"
        @click="activeTab = 'authors'"
      >
        Авторы
      </button>
      <button
        :class="{ active: activeTab === 'categories' }"
        @click="activeTab = 'categories'"
      >
        Категории
      </button>
    </div>
    <div v-if="activeTab === 'books'">
      <div class="search-filter">
        <input
          type="text"
          placeholder="Поиск книги по названию..."
          v-model="searchQueryBook"
        />
        <select v-model="selectedCategory">
          <option value="all">Все категории</option>
          <option
            v-for="category in categories"
            :key="category.idCategory"
            :value="category.nameCategory"
          >
            {{ category.nameCategory }}
          </option>
        </select>
      </div>
      <div class="add-button">
        <button @click="openAddBookForm">Добавить книгу</button>
      </div>
      <BooksTable :books="filteredBooks" @edit-book="openEditBookForm" />
    </div>
    <div v-if="activeTab === 'authors'">
      <div class="search-filter">
        <input
          type="text"
          placeholder="Поиск автора по фамилии..."
          v-model="searchQueryAuthor"
        />
      </div>
      <AuthorsTable
        :authors="filteredAuthors"
        @edit-author="openEditAuthorForm"
      />
    </div>
    <div v-if="activeTab === 'categories'">
      <div class="search-filter">
        <input
          type="text"
          placeholder="Поиск категории по названию..."
          v-model="searchQueryCategory"
        />
      </div>
      <CategoriesTable
        :categories="filteredCategories"
        @edit-category="openEditCategoryForm"
      />
    </div>
  </div>
  <AddBookForm
    v-if="showAddBookForm"
    :closeForm="closeAddBookForm"
    @refresh-data="getBooks"
  />
  <EditBookForm
    v-if="showEditBookForm"
    :closeForm="closeEditBookForm"
    :selectedBook="selectedBook"
    @refresh-data="getBooks"
  />
  <EditAuthorForm
    v-if="showEditAuthorForm"
    :closeForm="closeEditAuthorForm"
    :selectedAuthor="selectedAuthor"
    @refresh-data="getAuthors"
  />
  <EditCategoryForm
    v-if="showEditCategoryForm"
    :closeForm="closeEditCategoryForm"
    :selectedCategory="selectedCategoryTable"
    @refresh-data="getCategoriesTable"
  />
</template>

<style scoped>
h1 {
  text-align: center;
  font-size: 24px;
}

.tabs {
  display: flex;
  gap: 10px;
  margin-bottom: 20px;
  padding-bottom: 10px;
  border-bottom: 1px solid grey;
}

.tabs button {
  padding: 8px 16px;
  font-size: 16px;
  background: none;
  border: none;
  border-radius: 5px;
}

.tabs button.active {
  background-color: darkgreen;
  color: white;
}

.tabs button:hover:not(.active) {
  background-color: lightgrey;
}

.search-filter {
  display: flex;
  justify-content: space-between;
  gap: 15px;
  margin-bottom: 20px;
}

.search-filter input,
.search-filter select {
  height: auto;
  width: 100%;
  padding: 10px;
  font-size: 14px;
  border: 1px solid lightgrey;
  border-radius: 5px;
}

.add-button {
  display: flex;
  justify-content: flex-start;
  margin-top: 10px;
}

.add-button button {
  padding: 10px 20px;
  font-size: 14px;
  color: white;
  background-color: forestgreen;
  border: none;
  border-radius: 5px;
}

.add-button button:hover {
  background-color: darkgreen;
}
</style>
