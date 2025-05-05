<script setup>
import { ref, computed } from 'vue';

import BookCard from '../cards/BookCard.vue';

const props = defineProps({
  title: { type: String, required: true },
  books: { type: Array, required: true },
  isRecommendation: { type: Boolean, default: false },
});

const currentIndex = ref(0);
const showInfo = ref(false);

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

const toogleInfo = () => {
  showInfo.value = !showInfo.value;
};
</script>

<template>
  <section>
    <div class="section-title">
      <h2>{{ title }}</h2>
      <button
        v-if="isRecommendation"
        class="info-button"
        @mouseenter="toogleInfo"
        @mouseleave="toogleInfo"
        @focus="toogleInfo"
        @blur="toogleInfo"
      >
        🛈
      </button>
      <div v-if="showInfo && isRecommendation" class="info-tooltip">
        <h3>Персональные рекомендации</h3>
        <p>
          Эту подборку сделал для вас искусственный интеллект по имени Frog-Ai.
          Его создали и обучили наши программисты и специалисты по работе с
          данными.
        </p>
        <h4>Как это работает</h4>
        <p>В основе работы Frog-Ai лежит гибридная система рекомендаций:</p>
        <ul>
          <li>
            <strong>Контентная фильтрация:</strong> Анализирует содержание книг,
            которые вам нравятся, и ищет похожие
          </li>
          <li>
            <strong>Коллаборативная фильтрация:</strong> Сравнивает ваши
            предпочтения с другими читателями
          </li>
        </ul>
        <h4>Как улучшить рекомендации</h4>
        <ul>
          <li>Оценивайте прочитанные книги</li>
          <li>Добавляйте книги в свои коллекции</li>
          <li>Отмечайте статусы чтения ("Читаю", "Прочитано" и другие)</li>
        </ul>
        <p>
          Система обновляет рекомендации ежедневно, поэтому загладывайте сюда
          почаще!
        </p>
      </div>
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
  margin-bottom: 10px;
  position: relative;
}

.section-title {
  display: flex;
  align-items: center;
  gap: 10px;
  position: relative;
}

.info-button {
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: help;
  background: none;
  border: none;
  font-size: 20px;
}

.info-button:hover {
  color: darkgreen;
}

.info-tooltip {
  position: absolute;
  top: 100%;
  left: 0;
  z-index: 100;
  width: 600px;
  padding: 15px;
  border: 1px solid lightgrey;
  border-radius: 8px;
  background: white;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
  font-size: 14px;
  line-height: 1.5;
}

.info-tooltip h3 {
  margin-top: 0;
  color: darkgreen;
}

.info-tooltip h4 {
  margin-bottom: 8px;
  color: darkgreen;
}

.info-tooltip ul {
  padding-left: 20px;
}

.info-tooltip li {
  margin-bottom: 5px;
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
.nav-buttons:disabled {
  color: lightgrey;
  cursor: not-allowed;
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
