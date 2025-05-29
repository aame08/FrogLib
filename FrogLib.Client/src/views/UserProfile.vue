<script setup>
import { ref, computed } from 'vue';
import { useStore } from 'vuex';
import dayjs from 'dayjs';
import 'dayjs/locale/ru';
dayjs.locale('ru');
import collectionsService from '@/services/collectionsService';
import reviewsService from '@/services/reviewsService';

import TheHeader from '@/components/TheHeader.vue';
import UserBooks from '@/components/userComponents/UserBooks.vue';
import UserComments from '@/components/userComponents/UserComments.vue';
import UserCollections from '@/components/userComponents/UserCollections.vue';
import BookCollection from '@/components/BookCollection.vue';
import UserReviews from '@/components/userComponents/UserReviews.vue';
import BookReview from '@/components/BookReview.vue';
import UserCalendar from '@/components/userComponents/UserCalendar.vue';

const store = useStore();
const user = computed(() => store.getters['auth/user']);

const formattedDate = computed(() => {
  const registrationDate = user.value?.registrationDate;
  return dayjs(registrationDate).isValid()
    ? dayjs(registrationDate).format('DD.MM.YYYY')
    : 'Неверный формат даты';
});

const activeSection = ref('books');
const selectedCollectionId = ref(null);
const selectedCollection = ref(null);
const showCollection = ref(false);
const selectedReviewId = ref(null);
const selectedReview = ref(null);
const showReview = ref(false);

const openCollection = async (collectionId) => {
  try {
    selectedCollectionId.value = collectionId;
    const response = await collectionsService.getCollectionData(collectionId);
    selectedCollection.value = response;
    showCollection.value = true;
  } catch (error) {
    console.error('Ошибка при загрузке данных подборки:', error);
  }
};

const closeCollection = () => {
  showCollection.value = false;
  activeSection.value = 'collections';
};

const openReview = async (reviewId) => {
  try {
    selectedReviewId.value = reviewId;
    const response = await reviewsService.getReviewData(reviewId);
    selectedReview.value = response;
    showReview.value = true;
  } catch (error) {
    console.error('Ошибка при загрузке данных рецензии:', error);
  }
};

const closeReview = () => {
  showReview.value = false;
  activeSection.value = 'reviews';
};
</script>

<template>
  <TheHeader />
  <div
    style="
      margin-top: 70px;
      max-width: 1000px;
      margin-left: auto;
      margin-right: auto;
    "
    v-if="!showCollection && showReview"
  >
    <BookReview
      :id="selectedReview.book.id"
      :title="selectedReview.book.title"
      :imageURL="selectedReview.book.imageURL"
      :authors="selectedReview.book.authors"
      :averageRating="selectedReview.book.averageRating"
      :reviewId="selectedReviewId"
      :initialData="{
        titleReview: selectedReview.title,
        textReview: selectedReview.content,
        rating: selectedReview.bookRating,
      }"
      :closeReviewForm="closeReview"
    />
  </div>
  <div
    style="
      margin-top: 70px;
      max-width: 1000px;
      margin-left: auto;
      margin-right: auto;
    "
    v-if="showCollection && !showReview"
  >
    <BookCollection
      :collectionId="selectedCollectionId"
      :initialData="{
        titleCollection: selectedCollection.title,
        textCollection: selectedCollection.description,
        initialBooks: selectedCollection.books,
      }"
      :closeCollectionForm="closeCollection"
    />
  </div>
  <main v-if="!showCollection && !showReview">
    <div class="account-header">
      <div class="upper-container">
        <img
          v-if="user?.profileImageUrl"
          :src="`https://localhost:7157${user?.profileImageUrl}`"
          alt="User Image"
        />
        <img v-else src="@/assets/user_photo.png" alt="user image" />
        <div class="inner-container">
          <div>{{ user?.nameUser }}</div>
          <div>
            Дата регистрации: <span>{{ formattedDate }}</span>
          </div>
        </div>
      </div>
      <div class="lower-container">
        <button
          :class="{ active: activeSection === 'books' }"
          @click="activeSection = 'books'"
        >
          Книги
        </button>
        <button
          :class="{ active: activeSection === 'comments' }"
          @click="activeSection = 'comments'"
        >
          Комментарии
        </button>
        <button
          :class="{ active: activeSection === 'collections' }"
          @click="activeSection = 'collections'"
        >
          Подборки
        </button>
        <button
          :class="{ active: activeSection === 'reviews' }"
          @click="activeSection = 'reviews'"
        >
          Рецензии
        </button>
        <button
          :class="{ active: activeSection === 'calendar' }"
          @click="activeSection = 'calendar'"
        >
          Календарь
        </button>
      </div>
      <UserBooks v-if="activeSection === 'books'" />
      <UserComments v-if="activeSection === 'comments'" />
      <UserCollections
        v-if="activeSection === 'collections'"
        @open-collection="openCollection"
      />
      <UserReviews
        v-if="activeSection === 'reviews'"
        @open-review="openReview"
      />
      <UserCalendar v-if="activeSection === 'calendar'" />
    </div>
  </main>
</template>

<style scoped>
main {
  margin: 0 25px;
  background-color: whitesmoke;
}

.account-header {
  margin-top: 70px;
  padding: 5px;
  gap: 5px;
  display: flex;
  flex-direction: column;
  background-color: white;
  border-radius: 5px 0;
}

.upper-container {
  display: flex;
  align-items: center;
  gap: 15px;
}

.upper-container img {
  height: 100px;
}

.lower-container {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 5px;
  background-color: white;
  border-top: 2px solid forestgreen;
}

.lower-container button {
  background: none;
  border: none;
}

.lower-container button.active {
  border-bottom: 2px solid forestgreen;
}

.lower-container button:hover:not(.active) {
  font-weight: bold;
}
</style>
