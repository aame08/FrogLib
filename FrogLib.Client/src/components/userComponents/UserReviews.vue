<script setup>
import { ref, computed } from 'vue';
import { useStore } from 'vuex';
import userActivityService from '@/services/userActivityService';

import UserReviewCard from '../cards/UserReviewCard.vue';

const emit = defineEmits(['open-review']);

const store = useStore();
const user = computed(() => store.getters['auth/user']);
const userId = computed(() => user.value?.idUser || null);

const reviews = ref([]);
const sortOption = ref('date-desc');
const activeSort = ref('date-desc');

const getUserReviews = async () => {
  try {
    const response = await userActivityService.getUserReviews(userId.value);
    reviews.value = response;
  } catch (error) {
    console.error('Ошибка при загрузке рецензий:', error);
  }
};
getUserReviews();


const handleOpenReview = (reviewId) => {
  emit('open-review', reviewId);
};

const filteredReviews = computed(() => {
  let result = reviews.value;

  switch (sortOption.value) {
    case 'date-desc':
      result = [...result].sort((a, b) => new Date(b.createdDate) - new Date(a.createdDate));
      break;
    case 'date-asc':
      result = [...result].sort((a, b) => new Date(a.createdDate) - new Date(b.createdDate));
      break;
    case 'title-asc':
      result = [...result].sort((a, b) => a.titleBook.localeCompare(b.titleBook));
      break;
    case 'title-desc':
      result = [...result].sort((a, b) => b.titleBook.localeCompare(a.titleBook));
      break;
    case 'rating-desc':
      result = [...result].sort((a, b) => b.rating - a.rating);
      break;
    case 'rating-asc':
      result = [...result].sort((a, b) => a.rating - b.rating);
      break;
  }

  return result;
});
</script>

<template>
  <div class="view-container">
    <div>
      <fieldset class="aside-left">
        <legend>Сортировка</legend>
        <div class="menu">
          <label class="menu-item"
            ><input
              type="radio"
              name="sort"
              value="date-desc"
              v-model="sortOption"
            />
            От новых к старым</label
          >
          <label class="menu-item"
            ><input
              type="radio"
              name="sort"
              value="date-asc"
              v-model="sortOption"
            />
            От старых к новым</label
          >
        </div>
      </fieldset>
    </div>
    <div class="view-review">
      <div class="reviews-container">
        <UserReviewCard
          v-for="review in filteredReviews"
          :key="review.idReview"
          :review="review"
          @open-review="handleOpenReview"
        />
        <div
          v-if="filteredReviews.length === 0"
          style="text-align: center; font-size: 20px"
        >
          Ничего не найдено.
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.view-container {
  display: flex;
}

.aside-left {
  width: 230px;
  max-height: 350px;
  margin-top: 10px;
  padding: 5px;
  background-color: white;
  border: 2px solid forestgreen;
  border-radius: 8px;
}

legend {
  font-weight: bold;
}

.menu {
  display: flex;
  flex-direction: column;
  gap: 3px;
  margin-top: 5px;
}

.menu-item {
  font-size: 17px;
}

.view-review {
  width: 100%;
  margin-top: 10px;
  margin-left: 15px;
  padding: 5px;
  background-color: whitesmoke;
  border-radius: 5px;
}

.reviews-container {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
  padding: 10px;
  justify-content: center;
}
</style>
