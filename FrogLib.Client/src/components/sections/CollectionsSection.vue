<script setup>
import { ref, computed } from 'vue';

import SmallCollectionCard from '../cards/SmallCollectionCard.vue';

const props = defineProps({
  collections: { type: Array, required: true },
});

const currentIndex = ref(0);

const visibleCollections = computed(() => {
  return props.collections.slice(currentIndex.value, currentIndex.value + 2);
});

const isPrevDisabled = computed(() => {
  return currentIndex.value === 0;
});

const isNextDisabled = computed(() => {
  return currentIndex.value + 2 >= props.collections.length;
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
      <h2>Популярные подборки</h2>
      <button class="transparent-button">
        <RouterLink to="/collections" style="color: white">ВСЕ</RouterLink>
      </button>
    </div>
    <div class="collections-container">
      <button
        class="nav-buttons prev-button"
        @click="prevSlide"
        :disabled="isPrevDisabled"
      >
        ←
      </button>
      <div class="collections-slider">
        <SmallCollectionCard
          v-for="(collection, index) in visibleCollections"
          :key="index"
          :id="collection.id"
          :title="collection.title"
          :description="collection.description"
          :rating="collection.rating"
          :countBooks="collection.countBooks"
          :books="collection.books"
          :countView="collection.countView"
          :countComments="collection.countComments"
          :countLiked="collection.countLiked"
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

.collections-container {
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

.collections-slider {
  display: flex;
  justify-content: center;
  width: 100%;
  overflow: hidden;
  padding: 5px;
  gap: 15px;
}
</style>
