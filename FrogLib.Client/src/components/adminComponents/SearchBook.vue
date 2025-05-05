<script setup>
import { ref } from 'vue';
import adminService from '@/services/adminService';

import SearchResultCard from '../cards/SearchResultCard.vue';

const emit = defineEmits(['select-book']);

const searchQuery = ref('');
const searchResults = ref([]);
const handleSearch = async () => {
  if (!searchQuery.value.trim()) {
    return;
  }
  try {
    searchResults.value = [];
    const response = await adminService.adminSearchBook(searchQuery.value);
    searchResults.value = response;
  } catch (error) {
    console.error('Ошибка при поиске книг:', error);
  }
};

const selectBook = (book) => {
  emit('select-book', book);
};
</script>

<template>
  <div class="search-section">
    <h2>Поиск книг</h2>
    <label>Введите название книги:</label>
    <div class="search-container">
      <input
        v-model="searchQuery"
        type="text"
        placeholder="Название книги"
        @keyup.enter="handleSearch"
      />
      <button class="search-button" @click="handleSearch">Поиск</button>
    </div>
    <div class="search-results">
      <SearchResultCard
        v-for="(book, index) in searchResults"
        :key="index"
        :imageURL="book.imageUrl"
        :title="book.title"
        :authors="book.authors.join(', ')"
        :yearPublication="book.yearPublication"
        @select="selectBook(book)"
      />
    </div>
  </div>
</template>

<style scoped>
.search-section {
  flex: 1;
  height: fit-content;
  padding: 20px;
  background-color: white;
  border-radius: 5px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

h2 {
  font-size: 20px;
}

label {
  display: block;
  margin-bottom: 5px;
  font-weight: bold;
}

.search-container {
  display: flex;
}

.search-container input {
  width: calc(100% - 40px);
  height: auto;
  font-size: 14px;
  border: 1px solid lightgrey;
  border-radius: 5px 0 0 5px;
}

.search-container button {
  height: 40px;
  border-radius: 0 5px 5px 0;
}

.search-button {
  padding: 10px 20px;
  color: white;
  background-color: forestgreen;
  border: none;
  border-radius: 5px;
}

.search-button:hover {
  background-color: darkgreen;
}

.search-results {
  margin-top: 10px;
  max-height: 300px;
  overflow-y: auto;
}
</style>
