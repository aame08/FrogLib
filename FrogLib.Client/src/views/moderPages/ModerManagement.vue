<script setup>
import { ref } from 'vue';
import moderService from '@/services/moderService';

import ReviewsTable from '@/components/moderComponents/ReviewsTable.vue';
import ReviewView from '@/components/moderComponents/ReviewView.vue';
import CollectionsTable from '@/components/moderComponents/CollectionsTable.vue';
import CollectionView from '@/components/moderComponents/CollectionView.vue';

const activeTab = ref('reviews');
const reviews = ref([]);
const selectedReview = ref(null);
const showReviewView = ref(false);
const collections = ref([]);
const selectedCollection = ref(null);
const showCollectionView = ref(false);

const loadData = async () => {
  try {
    if (activeTab.value === 'reviews') {
      const response = await moderService.getModerReviews();
      reviews.value = response;
    } else {
      const response = await moderService.getModerCollections();
      collections.value = response;
    }
  } catch (error) {
    console.error('Ошибка при загрузке данных:', error);
  }
};
loadData();

const switchTab = (tab) => {
  activeTab.value = tab;
  loadData();
};

const openReviewView = (review) => {
  selectedReview.value = review;
  showReviewView.value = true;
};

const closeReviewView = () => {
  showReviewView.value = false;
};

const openCollectionView = (collection) => {
  selectedCollection.value = collection;
  showCollectionView.value = true;
};

const closeCollectionView = () => {
  showCollectionView.value = false;
};
</script>

<template>
  <div v-if="!showReviewView && !showCollectionView">
    <h1>Модерация подборок и рецензий</h1>
    <div class="container">
      <fieldset class="aside-left">
        <legend>Выбор</legend>
        <div class="menu">
          <label class="menu-item">
            <input
              type="radio"
              name="choice-filter"
              value="reviews"
              v-model="activeTab"
              @change="switchTab('reviews')"
              :checked="activeTab === 'reviews'"
            />Рецензии
          </label>
          <label class="menu-item">
            <input
              type="radio"
              name="choice-filter"
              value="collections"
              v-model="activeTab"
              @change="switchTab('collections')"
              :checked="activeTab === 'collections'"
            />Подборки
          </label>
        </div>
      </fieldset>
      <ReviewsTable
        v-if="activeTab === 'reviews'"
        :reviews="reviews"
        @review-view="openReviewView"
      />
      <CollectionsTable
        v-if="activeTab === 'collections'"
        :collections="collections"
        @collection-view="openCollectionView"
      />
    </div>
  </div>
  <ReviewView
    v-if="showReviewView"
    :selectedReview="selectedReview"
    :closeForm="closeReviewView"
    @refresh-data="loadData"
  />
  <CollectionView
    v-if="showCollectionView"
    :selectedCollection="selectedCollection"
    :closeForm="closeCollectionView"
    @refresh-data="loadData"
  />
</template>

<style scoped>
h1 {
  text-align: center;
  font-size: 24px;
}

.container {
  display: flex;
  gap: 10px;
}

.aside-left {
  width: 200px;
  height: fit-content;
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
</style>
