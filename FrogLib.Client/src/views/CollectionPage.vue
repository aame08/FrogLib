<script setup>
import { ref } from 'vue';
import { useRoute } from 'vue-router';
import collectionsService from '@/services/collectionsService';

import TheHeader from '@/components/TheHeader.vue';
import CollectionHeader from '@/components/collectionComponents/CollectionHeader.vue';
import CollectionBooks from '@/components/collectionComponents/CollectionBooks.vue';
import CollectionFooter from '@/components/collectionComponents/CollectionFooter.vue';
import CommentsContainer from '@/components/entityComponents/CommentsContainer.vue';
import BookCollection from '@/components/BookCollection.vue';
import TheFooter from '@/components/TheFooter.vue';

const collection = ref(null);

const route = useRoute();

const showEditCollection = ref(false);
const currentCollection = ref(null);

const getCollectionData = async () => {
  try {
    const response = await collectionsService.getCollectionData(
      route.params.id
    );
    collection.value = response;
  } catch (error) {
    console.error('Ошибка при загрузке данных подборки:', error);
  }
};
getCollectionData();

const openEditCollection = (collection) => {
  currentCollection.value = collection;
  showEditCollection.value = true;
};

const closeCollectionForm = () => {
  showEditCollection.value = false;
};
</script>

<template>
  <TheHeader @refresh-data="getCollectionData" />
  <main style="background-color: whitesmoke">
    <div v-if="collection && !showEditCollection" class="collection-container">
      <CollectionHeader
        :title="collection.title"
        :countBooks="collection.countBooks"
        :userURL="collection.userURL"
        :userName="collection.userName"
        :userId="collection.userId"
        :createdDate="collection.createdDate"
        :description="collection.description"
        @edit-collection="openEditCollection"
      />
      <CollectionBooks :books="collection.books" />
      <CollectionFooter
        :userId="collection.userId"
        :countView="collection.countView"
        :rating="collection.rating"
        :likes="collection.likes"
        :dislikes="collection.dislikes"
        :countLiked="collection.countLiked"
        @refresh-collection-data="getCollectionData"
      />
    </div>
    <CommentsContainer
      v-if="collection && !showEditCollection"
      :comments="collection.comments"
      @refresh-data="getCollectionData"
    />
    <BookCollection
      v-if="collection && showEditCollection"
      :collectionId="collection.id"
      :initialData="{
        titleCollection: collection.title,
        textCollection: collection.description,
        initialBooks: collection.books,
      }"
      :closeCollectionForm="closeCollectionForm"
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
.collection-container {
  display: flex;
  flex-direction: column;
  border-radius: 5px;
  border: 1px solid darkgreen;
  gap: 10px;
}
</style>
