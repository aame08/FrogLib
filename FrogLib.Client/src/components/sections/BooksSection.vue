<script setup>
import { ref, computed } from 'vue';

import BookCard from '../cards/BookCard.vue';

const props = defineProps({
  title: { type: String, required: true },
  books: { type: Array, required: true },
});

const currentIndex = ref(0);

const visibleBooks = computed(() => {
  return props.books.slice(currentIndex.value, currentIndex.value + 4);
});

const isPrevDisabled = computed(() => {
  return currentIndex.value === 0;
});

const isNextDisabled = computed(() => {
  return currentIndex.value + 4 >= props.books.length;
});

const prevSlide = () => {
  if (!isPrevDisabled.value) {
    currentIndex.value -= 1;
  }
};

const nextSlide = () => {
  if (!isNextDisabled.value) {
    currentIndex.value += 1;
  }
};
</script>

<template>
  <section>
    <div class="section-title">
      <h2>{{ title }}</h2>
    </div>
    <div class="books-container">
      <button
        class="nav-buttons prev-button"
        @click="prevSlide"
        :disabled="isPrevDisabled"
      >
        ←
      </button>
      <div class="books-slider">
        <BookCard
          v-for="(book, index) in visibleBooks"
          :key="index"
          :id="book.id || index"
          :title="book.title"
          :imageURL="book.imageURL"
          :averageRating="book.averageRating"
        />
      </div>
      <button
        class="nav-buttons next-button"
        @click="nextSlide"
        :disabled="isNextDisabled"
      >
        →
      </button>
    </div>
  </section>
</template>

<style scoped>
section {
  display: flex;
  flex-direction: column;
}

.section-title {
  display: flex;
  align-items: center;
  gap: 10px;
}

.books-container {
  display: flex;
  align-items: center;
  gap: 5px;
}

.nav-buttons {
  width: 30px;
  height: 30px;
  background: none;
  border: none;
}

.nav-buttons:hover:not(:disabled) {
  color: forestgreen;
}

.prev-button {
  left: 10px;
}

.next-button {
  right: 10px;
}

.books-slider {
  display: flex;
  justify-content: center;
  width: 100%;
  overflow: hidden;
  padding: 5px;
  gap: 10px;
}
</style>
