<script setup>
import { ref, computed, watch, onMounted, onUnmounted } from 'vue';
import { useStore } from 'vuex';
import booksService from '@/services/booksService';
import collectionsService from '@/services/collectionsService';
import reviewsService from '@/services/reviewsService';

import TheHeader from '@/components/TheHeader.vue';
import RecommendationSection from '@/components/sections/RecommendationSection.vue';
import BooksSection from '@/components/sections/BooksSection.vue';
import PopularBooksSection from '@/components/sections/PopularBooksSection.vue';
import ReviewsSection from '@/components/sections/ReviewsSection.vue';
import CollectionsSection from '@/components/sections/CollectionsSection.vue';
import TheFooter from '@/components/TheFooter.vue';

const recommendBooks = ref([]);
const newBooks = ref([]);
const bestsellers = ref([]);
const popularBooks = ref([]);
const popularReviews = ref([]);
const popularCollections = ref([]);
const lastUpdateTime = ref(false);
const updateInterval = 30 * 60 * 1000;

const store = useStore();
const user = computed(() => store.getters['auth/user']);
const userId = computed(() => user.value?.idUser || null);

const getRecommendations = async (force = false) => {
  if (!userId.value) return;

  try {
    const response = await booksService.getRecommendations(userId.value);
    recommendBooks.value = response;
    lastUpdateTime.value = new Date();
  } catch (error) {
    console.error('Ошибка при получении рекомендаций:', error);
  }
};

const getNewBooks = async () => {
  try {
    const response = await booksService.getNewBooks();
    newBooks.value = response;
  } catch (error) {
    console.error('Ошибка при загрузке новых книг:', error);
  }
};
getNewBooks();

const getBestSelling = async () => {
  try {
    const response = await booksService.getBestSelling();
    bestsellers.value = response;
  } catch (error) {
    console.error('Ошибка при загрузке бестселлеров:', error);
  }
};
getBestSelling();

const getPopularBooks = async () => {
  try {
    const response = await booksService.getPopularBooks();
    popularBooks.value = response;
  } catch (error) {
    console.error('Ошибка при загрузке популярных книг:', error);
  }
};
getPopularBooks();

const getPopularReviews = async () => {
  try {
    const response = await reviewsService.getPopularReviews();
    popularReviews.value = response;
  } catch (error) {
    console.error('Ошибка при загрузке популярных рецензий:', error);
  }
};
getPopularReviews();

const getPopularCollections = async () => {
  try {
    const response = await collectionsService.getPopularCollections();
    popularCollections.value = response;
  } catch (error) {
    console.error('Ошибка при загрузке популярных подборок:', error);
  }
};
getPopularCollections();

let updateTimer = null;

onMounted(() => {
  getRecommendations();
  updateTimer = setInterval(getRecommendations, updateInterval);
});

onUnmounted(() => {
  if (updateTimer) clearInterval(updateTimer);
});

watch(
  userId,
  (newId) => {
    if (newId) {
      getRecommendations();
      if (updateTimer) clearInterval(updateTimer);
      updateTimer = setInterval(getRecommendations, updateInterval);
    } else {
      recommendBooks.value = [];
      if (updateTimer) clearInterval(updateTimer);
    }
  },
  { immediate: true }
);
</script>

<template>
  <main style="background-color: whitesmoke">
    <TheHeader />
    <div class="main-banner">
      <div class="banner-text">
        <h1>Книги - наши тихие собеседники в час одиночества.</h1>
        <p>
          Они напоминают, что даже в самых темных страницах жизни можно найти
          светлые слова и мысли. Они дают нам укрытие, когда реальность
          становится невыносимой.
        </p>
      </div>
      <div class="banner-img">
        <img src="@/assets/book.png" alt="image banner" />
      </div>
    </div>
    <RecommendationSection
      v-if="recommendBooks.length > 0"
      title="Персональные рекомендации"
      :books="recommendBooks"
      :isRecommendation="true"
    />
    <BooksSection title="Книжные новинки" :books="newBooks" />
    <BooksSection title="Бестселлеры" :books="bestsellers" />
    <section class="features-container">
      <h1>Почему именно мы?</h1>
      <div class="inner-container">
        <div class="features-card">
          <img src="@/assets/search.png" alt="Поиск книг" />
          <h2>Поиск книг</h2>
          <p>Наш удобный поиск поможет вам найти именно то, что вы ищете.</p>
        </div>
        <div class="features-card">
          <img src="@/assets/collection.png" alt="Создание подборок" />
          <h2>Создание подборок</h2>
          <p>
            Создавайте свои собственные подборки книг и просматривайте другие.
          </p>
        </div>
        <div class="features-card">
          <img src="@/assets/recommend.png" alt="Получение рекомендаций" />
          <h2>Получение рекомендаций</h2>
          <p>
            Получайте персонализированные рекомендации книг на основе ваших
            предпочтений.
          </p>
        </div>
      </div>
    </section>
    <PopularBooksSection :books="popularBooks" />
    <ReviewsSection :reviews="popularReviews" />
    <CollectionsSection :collections="popularCollections" />
    <TheFooter />
  </main>
</template>

<style scoped>
.main-banner {
  display: flex;
  margin-top: 70px;
  margin-left: auto;
  margin-right: auto;
  max-width: 1200px;
  padding: 5px;
  background-color: white;
  border: 1px solid forestgreen;
  border-radius: 5px;
}

.banner-text {
  display: flex;
  flex-grow: 2;
  flex-direction: column;
  justify-content: center;
  margin-left: 20px;
  max-width: 550px;
}

.banner-text p {
  border-top: 2px solid darkgreen;
}

.banner-img {
  display: flex;
  flex-grow: 1;
  align-items: center;
  justify-content: center;
}

.banner-img img {
  height: 450px;
  width: 450px;
}

.features-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  width: 100%;
}

.features-container h1 {
  text-decoration: underline;
  text-decoration-color: forestgreen;
}

.inner-container {
  display: flex;
  justify-content: space-evenly;
  gap: 10px;
}

.features-card {
  width: 200px;
  padding: 20px;
  text-align: center;
  background-color: white;
  border: 2px solid darkgreen;
  border-radius: 10px;
}

.features-card img {
  height: 100px;
  width: 100px;
}

.features-card h2 {
  font-size: 18px;
  margin: 10px 0;
}

.features-card p {
  font-size: 14px;
  color: grey;
}
</style>
