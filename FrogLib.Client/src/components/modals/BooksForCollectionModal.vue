<script setup>
import { ref, computed } from 'vue';
import booksService from '@/services/booksService';

const props = defineProps({
  isVisible: Boolean,
  initialSelectedBooks: {
    type: Array,
    default: () => [],
  },
});

const emit = defineEmits(['close', 'save', 'cancel']);

const allBooks = ref([]);
const selectedBooks = ref([]);
const searchQuery = ref('');

const getAllBooks = async () => {
  try {
    const response = await booksService.getAllBooks();
    allBooks.value = response;
    selectedBooks.value = [...props.initialSelectedBooks];
  } catch (error) {
    console.error('Ошибка при загрузке книг:', error);
  }
};
getAllBooks();

const filteredBooks = computed(() => {
  let books = allBooks.value;

  if (searchQuery.value) {
    books = books.filter((book) =>
      book.title.toLowerCase().includes(searchQuery.value.toLowerCase())
    );
  }

  return books;
});

const toggleBookSelection = (book) => {
  const index = selectedBooks.value.findIndex((b) => b.id === book.id);
  if (index === -1) {
    selectedBooks.value.push(book);
  } else {
    selectedBooks.value.splice(index, 1);
  }
};

const isBookSelected = (book) => {
  return selectedBooks.value.some((b) => b.id === book.id);
};

const saveSelection = () => {
  emit('save', selectedBooks.value);
};

const cancelSelection = () => {
  selectedBooks.value = [];
  emit('cancel');
};
</script>

<template>
  <div class="modal-overlay" v-if="isVisible">
    <div class="modal-content">
      <button class="button-close" @click="$emit('close')">✕</button>
      <div class="modal-header">Выберите книги</div>
      <div class="modal-container">
        <div class="search-container">
          <input
            type="text"
            placeholder="Поиск книг"
            v-model="searchQuery"
            @input="filteredBooks"
          />
          <div>⌕</div>
        </div>
        <ul id="bookSelectionList">
          <div v-for="book in filteredBooks" :key="book.id" class="book-card">
            <img :src="book.imageURL" alt="" />
            <label>
              <input
                type="checkbox"
                :checked="isBookSelected(book)"
                @change="toggleBookSelection(book)"
              />
              <span>{{ book.title }}</span>
            </label>
          </div>
        </ul>
      </div>
      <div class="buttons">
        <button class="transparent-button" @click="cancelSelection">
          Отмена
        </button>
        <button class="transparent-button" @click="saveSelection">
          Сохранить
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  right: 0;
  bottom: 0;
  left: 0;
  z-index: 2000;
  display: flex;
  justify-content: center;
  align-items: center;
  background: rgba(0, 0, 0, 0.5);
}

.modal-content {
  display: flex;
  flex-direction: column;
  width: 650px;
  height: 550px;
  border-radius: 25px;
  background-color: white;
  overflow: auto;
}

.button-close {
  align-self: flex-end;
  margin-right: 15px;
  width: 20px;
  height: 20px;
  font-size: 18px;
  background: none;
  border: none;
  color: black;
}

.button-close:hover {
  color: darkgreen;
}

.modal-header {
  text-align: center;
  font-size: 20px;
  font-weight: bold;
  border-bottom: 2px solid darkgreen;
}

.modal-container {
  display: flex;
  flex-direction: column;
  align-self: center;
  width: 100%;
  gap: 5px;
}

.search-container {
  display: flex;
  padding: 5px;
}

.search-container input {
  width: 90%;
  height: 30px;
  padding-left: 10px;
  border-radius: 5px 0 0 5px;
}

.search-container div {
  width: 10%;
  padding: 5px;
  font-size: 16px;
  text-align: center;
  color: white;
  background-color: forestgreen;
  border: none;
  border-radius: 0 5px 5px 0;
}

.book-card {
  display: flex;
  align-items: center;
  gap: 10px;
  margin: 10px 0;
}

.book-card img {
  width: 50px;
  height: 75px;
}

.buttons {
  display: flex;
  align-self: flex-end;
  gap: 5px;
  margin-bottom: 10px;
}

.transparent-button {
  display: flex;
  align-items: center;
  font-size: 16px;
  background: none;
  border: none;
  color: black;
}
</style>
