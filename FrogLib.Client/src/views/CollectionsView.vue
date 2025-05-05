<script setup>
import { ref, computed } from 'vue';
import collectionsService from '@/services/collectionsService';

import TheHeader from '@/components/TheHeader.vue';
import CreateCollectionBanner from '@/components/CreateCollectionBanner.vue';
import CollectionsFilterPanel from '@/components/filteringPanels/CollectionsFilterPanel.vue';
import CollectionCard from '@/components/cards/CollectionCard.vue';
import BookCollection from '@/components/BookCollection.vue';
import TheFooter from '@/components/TheFooter.vue';

const allCollections = ref([]);
const searchQuery = ref('');
const selectedSort = ref('date');
const showCollectionForm = ref(false);

const getAllCollections = async () => {
  try {
    const response = await collectionsService.getAllCollections();
    allCollections.value = response;
  } catch (error) {
    console.error('Ошибка при загрузке подборок:', error);
  }
};
getAllCollections();

const filteredCollections = computed(() => {
  let collections = allCollections.value;

  if (searchQuery.value) {
    collections = collections.filter((collection) =>
      collection.title.toLowerCase().includes(searchQuery.value.toLowerCase())
    );
  }

  if (selectedSort.value === 'date') {
    collections = collections.sort(
      (a, b) =>
        new Date(b.createdDate).getTime() - new Date(a.createdDate).getTime()
    );
  } else if (selectedSort.value === 'popular') {
    collections = collections.sort((a, b) => b.rating - a.rating);
  } else if (selectedSort.value === 'comments') {
    collections = collections.sort((a, b) => b.countComments - a.countComments);
  }

  return collections;
});

const openCollectionForm = () => {
  showCollectionForm.value = true;
};

const closeCollectionForm = () => {
  showCollectionForm.value = false;
};
</script>

<template>
  <main style="background-color: whitesmoke">
    <TheHeader />
    <CreateCollectionBanner
      v-if="!showCollectionForm"
      @create-collection="openCollectionForm"
    />
    <div class="content-container" v-if="!showCollectionForm">
      <CollectionsFilterPanel
        @search="searchQuery = $event"
        @sort="selectedSort = $event"
      />
      <section class="collections-section">
        <div class="collections-container">
          <CollectionCard
            v-for="(collection, index) in filteredCollections"
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
            :createdDate="collection.createdDate"
            :userName="collection.userName"
            :userURL="collection.userURL"
          />
        </div>
      </section>
    </div>
    <BookCollection v-else :closeCollectionForm="closeCollectionForm" />
    <TheFooter />
  </main>
</template>

<style scoped>
.content-container {
  display: flex;
  align-items: flex-start;
  gap: 20px;
  margin: 20px;
}

.collections-section {
  flex: 1;
  padding: 20px;
  background-color: white;
  border: 1px solid lightgrey;
  border-radius: 8px;
}

.collections-container {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
}
</style>
