<script setup>
import { ref, computed } from 'vue';
import reviewsService from '@/services/reviewsService';

import TheHeader from '@/components/TheHeader.vue';
import ReviewsFilterPanel from '@/components/filteringPanels/ReviewsFilterPanel.vue';
import ReviewCard from '@/components/cards/ReviewCard.vue';
import TheFooter from '@/components/TheFooter.vue';

const allReviews = ref([]);
const searchQuery = ref('');
const selectedSort = ref('date');

const getAllReviews = async () => {
  try {
    const response = await reviewsService.getAllReviews();
    allReviews.value = response;
  } catch (error) {
    console.error('Ошибка при загрузке рецензий:', error);
  }
};
getAllReviews();

const filteredReviews = computed(() => {
  let reviews = allReviews.value;

  if (searchQuery.value) {
    reviews = reviews.filter((review) =>
      review.title.toLowerCase().includes(searchQuery.value.toLowerCase())
    );
  }

  if (selectedSort.value === 'date') {
    reviews = reviews.sort(
      (a, b) =>
        new Date(b.createdDate).getTime() - new Date(a.createdDate).getTime()
    );
  } else if (selectedSort.value === 'popular') {
    reviews = reviews.sort((a, b) => b.rating - a.rating);
  }

  return reviews;
});
</script>

<template>
  <main style="background-color: whitesmoke">
    <TheHeader />
    <div class="main-banner">
      <div class="banner-text">
        <p>
          Чтение - это не только процесс, но и
          <span
            style="
              text-decoration: underline;
              text-decoration-color: forestgreen;
            "
            >глубокий анализ</span
          >. На этой страницы собраны все рецензии на книги, прочитанные нашими
          пользователями. Здесь читатели делятся своими уникальными мнениями,
          конструктивными суждениями, логичными выводами и компетентными
          оценками прочитанных произведений. Каждая рецензия основана на личном
          осмыслени текста и авторских идей, что делает её особенно ценной и
          интересной для других читателей.
        </p>
      </div>
      <div class="banner-img">
        <img src="@/assets/book2.png" alt="image banner" />
      </div>
    </div>
    <div class="content-container">
      <ReviewsFilterPanel
        @search="searchQuery = $event"
        @sort="selectedSort = $event"
      />
      <section class="reviews-section">
        <div class="reviews-container">
          <ReviewCard
            v-for="(review, index) in filteredReviews"
            :key="index"
            :id="review.id"
            :title="review.title"
            :content="review.content"
            :imageURL="review.imageURL"
            :rating="review.rating"
            :countView="review.countView"
            :createdDate="review.createdDate"
            :userName="review.userName"
            :userURL="review.userURL"
          />
        </div>
      </section>
    </div>
    <TheFooter />
  </main>
</template>

<style scoped>
.main-banner {
  display: flex;
  margin-top: 70px;
  margin-left: auto;
  margin-right: auto;
  max-width: 800px;
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

.banner-img {
  display: flex;
  flex-grow: 1;
  align-items: center;
  justify-content: center;
}

.banner-img img {
  height: 250px;
  width: 250px;
}

.content-container {
  display: flex;
  align-items: flex-start;
  gap: 20px;
  margin: 20px;
}

.reviews-section {
  flex: 1;
  padding: 20px;
  background-color: white;
  border: 1px solid lightgrey;
  border-radius: 8px;
}

.reviews-container {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
}
</style>
