<script setup>
import ReviewCard from '../cards/ReviewCard.vue';

const props = defineProps({
  countReviews: { type: Number, required: true },
  reviews: { type: Array, required: true },
});
</script>

<template>
  <h3 v-if="reviews.length === 0">Здесь пока нет рецензий.</h3>
  <h3 v-else>
    Всего: <span>{{ countReviews }}</span>
  </h3>
  <div class="reviews-container">
    <ReviewCard
      v-for="(review, index) in reviews"
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
</template>

<style scoped>
h3 {
  text-align: end;
}

.reviews-container {
  display: flex;
  justify-content: center;
  flex-wrap: wrap;
}
</style>
