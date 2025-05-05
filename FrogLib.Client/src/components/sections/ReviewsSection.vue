<script setup>
import { ref, computed } from 'vue';

import SmallReviewCard from '../cards/SmallReviewCard.vue';

const props = defineProps({
  reviews: { type: Array, required: true },
});

const currentIndex = ref(0);

const visibleReviews = computed(() => {
  return props.reviews.slice(currentIndex.value, currentIndex.value + 3);
});

const isPrevDisabled = computed(() => {
  return currentIndex.value === 0;
});

const isNextDisabled = computed(() => {
  return currentIndex.value + 3 >= props.reviews.length;
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
      <h2>Популярные рецензии</h2>
      <button class="transparent-button">
        <RouterLink to="/reviews" style="color: white">ВСЕ</RouterLink>
      </button>
    </div>
    <div class="reviews-container">
      <button
        class="nav-buttons prev-button"
        @click="prevSlide"
        :disabled="isPrevDisabled"
      >
        ←
      </button>
      <div class="reviews-slider">
        <SmallReviewCard
          v-for="(review, index) in visibleReviews"
          :key="index"
          :id="review.id || index"
          :rating="review.rating"
          :countView="review.countView"
          :imageURL="review.imageURL"
          :title="review.title"
          :content="review.content"
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

.transparent-button {
  font-size: 12px;
  border: none;
  border-radius: 5px;
  background-color: forestgreen;
}
.transparent-button:hover {
  background-color: forestgreen;
  opacity: 0.5;
}

.reviews-container {
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

.reviews-slider {
  display: flex;
  justify-content: center;
  width: 100%;
  overflow: hidden;
  padding: 5px;
  gap: 15px;
}
</style>
