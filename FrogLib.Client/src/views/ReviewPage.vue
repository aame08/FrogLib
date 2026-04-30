<script setup>
import { ref } from 'vue';
import { useRoute } from 'vue-router';
import reviewsService from '@/services/reviewsService';

import TheHeader from '@/components/TheHeader.vue';
import ReviewHeader from '@/components/reviewComponents/ReviewHeader.vue';
import ReviewBook from '@/components/reviewComponents/ReviewBook.vue';
import ReviewContent from '@/components/reviewComponents/ReviewContent.vue';
import CommentsContainer from '@/components/entityComponents/CommentsContainer.vue';
import BookReview from '@/components/BookReview.vue';
import TheFooter from '@/components/TheFooter.vue';

const route = useRoute();

const review = ref(null);
const showEditReview = ref(false);
const currentReview = ref(null);

const getReviewData = async () => {
  try {
    const response = await reviewsService.getReviewData(route.params.id);
    review.value = response;
  } catch (error) {
    console.log('Ошибка при загрузке данных рецензии:', error);
  }
};
getReviewData();

const openEditReview = (review) => {
  currentReview.value = review;
  showEditReview.value = true;
};

const closeReviewForm = () => {
  showEditReview.value = false;
};
</script>

<template>
  <TheHeader @refresh-data="getReviewData" />
  <main style="background-color: whitesmoke">
    <div v-if="review && !showEditReview" class="review-container">
      <ReviewHeader
        :title="review.title"
        :userId="review.userId"
        :userURL="review.userURL"
        :userName="review.userName"
        :createdDate="review.createdDate"
        @edit-review="openEditReview"
      />
      <ReviewBook :book="review.book" />
      <ReviewContent
        :bookRating="review.bookRating"
        :title="review.title"
        :content="review.content"
        :countView="review.countView"
        :rating="review.rating"
        :likes="review.likes"
        :dislikes="review.dislikes"
        @refresh-review-data="getReviewData"
      />
    </div>
    <CommentsContainer
      v-if="review && !showEditReview"
      :comments="review.comments"
      @refresh-data="getReviewData"
    />
    <BookReview
      v-if="showEditReview && review"
      :id="review.book.id"
      :title="review.book.title"
      :imageURL="review.book.imageURL"
      :authors="review.book.authors"
      :averageRating="review.book.averageRating"
      :reviewId="review.id"
      :initialData="{
        titleReview: review.title,
        textReview: review.content,
        rating: review.bookRating,
      }"
      :closeReviewForm="closeReviewForm"
    />
  </main>
  <TheFooter />
</template>

<style scoped>
main {
  margin-top: 70px;
  max-width: 1400px;
  margin-left: auto;
  margin-right: auto;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

.review-container {
  display: flex;
  flex-direction: column;
  border-radius: 5px;
  border: 1px solid darkgreen;
  gap: 10px;
  background-color: white;
}
</style>
