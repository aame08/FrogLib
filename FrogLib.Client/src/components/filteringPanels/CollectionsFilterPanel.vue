<script setup>
import { ref } from 'vue';

const emit = defineEmits(['search', 'sort']);

const searchQuery = ref('');
const selectedSort = ref('date');

const emitSearch = (event) => {
  searchQuery.value = event.target.value;
  emit('search', searchQuery.value);
};

const emitSort = (event) => {
  selectedSort.value = event.target.value;
  emit('sort', selectedSort.value);
};
</script>

<template>
  <div class="sidebar-container">
    <div class="filter-container">
      <div class="search-container">
        <input
          type="text"
          placeholder="Поиск подборок"
          v-model="searchQuery"
          @input="emitSearch"
        />
        <div>⌕</div>
      </div>
      <div class="sort-container">
        <select name="sort" id="sort" v-model="selectedSort" @change="emitSort">
          <option value="date" selected>По дате добавления</option>
          <option value="popular">Самые популярные</option>
          <option value="comments">Самые обсуждаемые</option>
        </select>
      </div>
    </div>
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
</style>
