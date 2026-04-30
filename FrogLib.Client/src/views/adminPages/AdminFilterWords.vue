<script setup>
import { ref, computed } from 'vue';
import adminService from '@/services/adminService';

import ForbiddenWordsTable from '@/components/adminComponents/ForbiddenWordsTable.vue';
import AddForbiddenWord from '@/components/modals/AddForbiddenWord.vue';

const forbiddenWords = ref([]);
const searchQuery = ref('');
const showModal = ref(false);

const getForbiddenWords = async () => {
  try {
    const response = await adminService.getForbiddenWords();
    forbiddenWords.value = Array.from(response);
  } catch (error) {
    console.error('Ошибка при загрузке запрещенных слов:', error);
  }
};
getForbiddenWords();

const filteredWords = computed(() => {
  let result = forbiddenWords.value;

  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase();
    result = result.filter((word) => word.toLowerCase().includes(query));
  }

  return result;
});

const openModal = () => (showModal.value = true);

const closeModal = () => {
  showModal.value = false;
};
</script>

<template>
  <AddForbiddenWord
    :isVisible="showModal"
    @close="closeModal"
    @refresh="getForbiddenWords"
  />
  <h1>Управление запрещёнными словами</h1>
  <div class="add-button">
    <button @click="openModal">Добавить слово</button>
  </div>
  <div class="search-filter">
    <input type="text" placeholder="Поиск слова..." v-model="searchQuery" />
  </div>
  <ForbiddenWordsTable :words="filteredWords" @refresh="getForbiddenWords" />
</template>

<style scoped>
h1 {
  text-align: center;
  font-size: 24px;
}

.search-filter {
  display: flex;
  justify-content: space-between;
  gap: 15px;
  margin-bottom: 20px;
}

.search-filter input {
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
  margin-bottom: 10px;
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
