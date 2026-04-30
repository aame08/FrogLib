<script setup>
import { ref } from 'vue';

import booksService from '@/services/booksService';

const emit = defineEmits(['search', 'sort', 'filter']);

const categories = ref([]);
const searchQuery = ref('');
const selectedSort = ref('popular');
const selectedCategories = ref([]);

const getCategories = async () => {
  try {
    const response = await booksService.getCategories();
    categories.value = response;
  } catch (error) {
    console.error('Ошибка при загрузке категорий:', error);
  }
};
getCategories();

const emitSearch = (event) => {
  searchQuery.value = event.target.value;
  emit('search', searchQuery.value);
};

const emitSort = (event) => {
  selectedSort.value = event.target.value;
  emit('sort', selectedSort.value);
};

const emitFilter = (event, categoryId) => {
  if (event.target.checked) {
    selectedCategories.value.push(categoryId);
  } else {
    selectedCategories.value = selectedCategories.value.filter(
      (id) => id !== categoryId
    );
  }
  emit('filter', selectedCategories.value);
};
</script>

<template>
  <div class="sidebar-container">
    <div class="filter-container">
      <div class="search-container">
        <input
          type="text"
          placeholder="Поиск книг"
          v-model="searchQuery"
          @input="emitSearch"
        />
        <div>⌕</div>
      </div>
      <div class="sort-container">
        <select name="sort" id="sort" v-model="selectedSort" @change="emitSort">
          <option value="popular" selected>По популярности</option>
          <option value="news">По новизне</option>
        </select>
      </div>
    </div>

    <fieldset>
      <legend>Категории книг</legend>
      <div class="categories">
        <label v-for="category in categories" :key="category.idCategory">
          <input
            type="checkbox"
            :id="`category-${category.idCategory}`"
            @change="emitFilter($event, category.idCategory)"
          />
          {{ category.nameCategory }}
        </label>
      </div>
    </fieldset>
  </div>
</template>

<style scoped>
.sidebar-container {
  position: sticky;
  top: 65px;
  display: flex;
  flex-direction: column;
  width: 300px;
  height: fit-content;
  padding: 10px;
  gap: 20px;
  background-color: white;
  border: 2px solid lightgrey;
  border-radius: 8px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

.filter-container {
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.search-container {
  display: flex;
}

.search-container input {
  height: 30px;
  width: 90%;
  padding-left: 10px;
  border-radius: 5px 0 0 5px;
}

.search-container div {
  width: 25px;
  padding: 5px;
  background-color: forestgreen;
  color: white;
  border: none;
  border-radius: 0 5px 5px 0;
  font-size: 16px;
  text-align: center;
}

.sort-container select {
  width: 100%;
  padding-left: 10px;
  background-color: white;
}

fieldset {
  padding: 10px;
  max-height: 300px;
  background-color: white;
  border: 2px solid forestgreen;
  border-radius: 8px;
  overflow-y: auto;
}

.categories {
  display: flex;
  flex-direction: column;
  gap: 10px;
}
</style>
