<script setup>
import { ref, computed } from 'vue';
import { useStore } from 'vuex';
import booksService from '@/services/booksService';
import userActivityService from '@/services/userActivityService';

import UserBookCard from '../cards/UserBookCard.vue';
import EditBookModal from '../modals/EditBookModal.vue';

const listTypes = ref([]);
const books = ref([]);
const selectedBook = ref(null);
const showEditModal = ref(false);
const searchQuery = ref('');
const selectedListType = ref('all');
const sortOption = ref('title-asc');
const activeSort = ref('title-asc');

const store = useStore();
const user = computed(() => store.getters['auth/user']);
const userId = computed(() => user.value?.idUser || null);

const getListTypes = async () => {
  try {
    const response = await booksService.getListTypes();
    listTypes.value = response;
  } catch (error) {
    console.error('Ошибка при загрузке списков:', error);
  }
};
getListTypes();

const getUserBooks = async () => {
  try {
    const response = await userActivityService.getUserBooks(userId.value);
    books.value = response;
  } catch (error) {
    console.error('Ошибка при загрузке книг пользователя:', error);
  }
};
getUserBooks();

const openEditModal = (book) => {
  selectedBook.value = book || { rating: null };
  showEditModal.value = true;
};

const closeEditModal = () => {
  showEditModal.value = false;
  selectedBook.value = null;
};

const filteredBooks = computed(() => {
  let result = books.value;

  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase();
    result = result.filter((book) =>
      book.titleBook.toLowerCase().includes(query)
    );
  }

  if (selectedListType.value !== 'all') {
    result = result.filter(
      (book) => book.idListType === selectedListType.value
    );
  }

  switch (sortOption.value) {
    case 'title-asc':
      result = [...result].sort((a, b) =>
        a.titleBook.localeCompare(b.titleBook)
      );
      break;
    case 'title-desc':
      result = [...result].sort((a, b) =>
        b.titleBook.localeCompare(a.titleBook)
      );
      break;
    case 'date-added':
      result = [...result].sort(
        (a, b) => new Date(b.addedDate) - new Date(a.addedDate)
      );
      break;
  }

  return result;
});

const setSortOption = (option) => {
  sortOption.value = option;
  activeSort.value = option;
};
</script>

<template>
  <div class="view-container">
    <div>
      <fieldset class="aside-left">
        <legend>Списки</legend>
        <div class="menu">
          <label class="menu-item"
            ><input
              type="radio"
              name="list-type"
              value="all"
              v-model="selectedListType"
              checked
            />Все</label
          >
          <label
            class="menu-item"
            v-for="type in listTypes"
            :key="type.idListType"
          >
            <input
              type="radio"
              name="list-type"
              :value="type.idListType"
              v-model="selectedListType"
            />
            {{ type.nameList }}
          </label>
        </div>
      </fieldset>
      <fieldset class="aside-left">
        <legend>Сортировка</legend>
        <div class="menu">
          <label class="menu-item"
            ><input
              type="radio"
              name="sort"
              value="title-asc"
              v-model="sortOption"
              @change="setSortOption('title-asc')"
            />По названию (А-Я)</label
          >
          <label class="menu-item"
            ><input
              type="radio"
              name="sort"
              value="title-desc"
              v-model="sortOption"
              @change="setSortOption('title-desc')"
            />По названию (Я-А)</label
          >
          <label class="menu-item"
            ><input
              type="radio"
              name="sort"
              value="date-added"
              v-model="sortOption"
              @change="setSortOption('date-added')"
            />По дате добавления</label
          >
        </div>
      </fieldset>
    </div>
    <div class="view-list">
      <div class="search-container">
        <input
          type="text"
          placeholder="Поиск книги по названию..."
          v-model="searchQuery"
        />
        <div>⌕</div>
      </div>
      <div class="books-container">
        <UserBookCard
          v-for="book in filteredBooks"
          :key="book.idBook"
          :book="book"
          @edit="openEditModal"
        />
        <div
          v-if="filteredBooks.length === 0"
          style="text-align: center; font-size: 20px"
        >
          В этом списке пока нет книг.
        </div>
      </div>
    </div>
  </div>
  <EditBookModal
    v-if="showEditModal"
    :book="selectedBook || {}"
    :isVisible="showEditModal"
    @close="closeEditModal"
    @update="getUserBooks"
  />
</template>

<style scoped>
.view-container {
  display: flex;
}
.aside-left {
  padding: 5px;
  width: 230px;
  max-height: 350px;
  background-color: white;
  border: 2px solid forestgreen;
  border-radius: 8px;
  margin-top: 10px;
}

legend {
  font-weight: bold;
}

.menu {
  margin-top: 5px;
  display: flex;
  flex-direction: column;
  gap: 3px;
}
.menu-item {
  font-size: 17px;
}
.view-list {
  display: flex;
  flex-direction: column;
  margin-top: 10px;
  margin-left: 15px;
  width: 100%;
  background-color: white;
  border-radius: 5px;
  padding: 5px;
}
.search-container {
  display: flex;
  align-self: center;
}
.search-container input {
  height: 25px;
  border-radius: 0 0 0 5px;
  width: 350px;
}

.search-container div {
  width: 25px;
  background-color: forestgreen;
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 0 5px 0 0;
}

.books-container {
  display: flex;
  flex-direction: column;
  padding: 10px;
  gap: 5px;
}
</style>
