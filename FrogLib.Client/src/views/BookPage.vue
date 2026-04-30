<script setup>
import { ref } from 'vue'
import { useRoute } from 'vue-router';
import booksService from '@/services/booksService';

import TheHeader from '@/components/TheHeader.vue';
import BookHeader from '@/components/bookComponents/BookHeader.vue';
import BookCover from '@/components/bookComponents/BookCover.vue';
import BookDetails from '@/components/bookComponents/BookDetails.vue';
import BookStatistics from '@/components/bookComponents/BookStatistics.vue';
import BookReviews from '@/components/bookComponents/BookReviews.vue';
import BookCollections from '@/components/bookComponents/BookCollections.vue';
import BookReview from '@/components/BookReview.vue';
import BookCollection from '@/components/BookCollection.vue';
import TheFooter from '@/components/TheFooter.vue';

const book = ref(null);
const activeSection = ref('info');
const showReviewForm = ref(false);
const showCollectionForm = ref(false);

const route = useRoute();

const getBookData = async () => {
  try {
    const response = await booksService.getBookData(route.params.id);
    book.value = response;
  } catch (error) {
    console.error('Ошибка при загрузке данных о книге:', error);
  }
};
getBookData();

const changeSection = (section) => {
  activeSection.value = section;
};

const openReviewForm = () => {
  showReviewForm.value = true;
};

const closeReviewForm = () => {
  showReviewForm.value = false;
};

const openCollectionForm = () => {
  showCollectionForm.value = true;
};

const closeCollectionForm = () => {
  showCollectionForm.value = false;
};
</script>

<template>
  <TheHeader @refresh-data="getBookData" />
  <main style="background-color: whitesmoke">
    <div v-if="!showReviewForm && !showCollectionForm" class="book-container">
      <BookHeader
        :title="book.title"
        :authors="book.authors"
        @change-section="changeSection"
      />
    </div>
    <div v-if="!showReviewForm && !showCollectionForm" class="book-info">
      <BookCover
        :id="book.id"
        :title="book.title"
        :imageURL="book.imageURL"
        @refresh-book-data="getBookData"
        :openReviewForm="openReviewForm"
        :openCollectionForm="openCollectionForm"
        :bookData="book"
      />
      <div class="info-section" v-if="activeSection === 'info'">
        <BookDetails
          :id="book.id"
          :isbN13="book.isbN13"
          :description="book.description"
          :yearPublication="book.yearPublication"
          :pageCount="book.pageCount"
          :languageBook="book.languageBook"
          :category="book.category"
          :publisher="book.publisher"
          :averageRating="book.averageRating"
          :countView="book.countView"
          @refresh-book-data="getBookData"
        />
        <BookStatistics
          :averageRating="book.averageRating"
          :totalRatings="book.totalRatings"
          :ratingStatistics="book.ratingStatistics"
          :totalUserBookmarks="book.totalUserBookmarks"
          :userBookmarks="book.userBookmarks"
        />
      </div>
      <div v-else-if="activeSection === 'reviews'" class="reviews-section">
        <BookReviews
          :countReviews="book.countReviews"
          :reviews="book.reviews"
        />
      </div>
      <div
        v-else-if="activeSection === 'collections'"
        class="collections-section"
      >
        <BookCollections
          :countCollections="book.countCollections"
          :collections="book.collections"
        />
      </div>
    </div>
    <BookReview
      v-else-if="showReviewForm && !showCollectionForm"
      :id="book.id"
      :title="book.title"
      :imageURL="book.imageURL"
      :authors="book.authors"
      :averageRating="book.averageRating"
      :closeReviewForm="closeReviewForm"
      @refresh-data="getBookData"
    />
    <BookCollection
      v-else-if="showCollectionForm && !showReviewForm"
      :closeCollectionForm="closeCollectionForm"
      :initialBook="book"
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

.book-container {
  display: flex;
  flex-direction: column;
  border-radius: 5px;
}

.book-info {
  margin-top: 10px;
  display: flex;
  gap: 15px;
}

.info-section {
  margin-right: 10px;
  flex-grow: 1;
  display: flex;
  flex-direction: column;
  gap: 10px;
  background-color: white;
  border-radius: 5px;
  border: 2px solid rgba(34, 139, 34, 0.5);
  padding: 3px;
}

.reviews-section {
  margin-right: 10px;
  flex-grow: 1;
  display: flex;
  flex-direction: column;
  gap: 15px;
  background-color: white;
  border-radius: 5px;
  border: 2px solid rgba(34, 139, 34, 0.5);
  padding: 3px;
}

.collections-section {
  margin-right: 10px;
  flex-grow: 1;
  display: flex;
  flex-direction: column;
  gap: 10px;
  background-color: white;
  border-radius: 5px;
  border: 2px solid rgba(34, 139, 34, 0.5);
  padding: 3px;
}
</style>
