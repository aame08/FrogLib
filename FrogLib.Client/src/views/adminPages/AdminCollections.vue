<script setup>
import { ref, computed } from 'vue';
import { useStore } from 'vuex';
import userActivityService from '@/services/userActivityService';
import collectionsService from '@/services/collectionsService';

import CollectionsTable from '@/components/adminComponents/CollectionsTable.vue';
import BookCollection from '@/components/BookCollection.vue';

const store = useStore();
const user = computed(() => store.getters['auth/user']);
const idUser = computed(() => user.value?.idUser || null);

const collections = ref([]);
const searchQuery = ref('');
const selectedCollection = ref(null);
const selectedCollectionId = ref(null);
const showEditForm = ref(false);
const showAddForm = ref(false);

const getCollections = async () => {
  try {
    const response = await userActivityService.getUserCollections(idUser.value);
    collections.value = response;
  } catch (error) {
    console.error('Ошибка при загрузке подборок:', error);
  }
};
getCollections();

const filteredCollections = computed(() => {
  let result = collections.value;

  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase();
    result = result.filter((collection) =>
      collection.titleCollection.toLowerCase().includes(query)
    );
  }

  return result;
});

const getCollectionData = async (id) => {
  try {
    const response = await collectionsService.getCollectionData(id);
    selectedCollection.value = response;
    showEditForm.value = true;
  } catch (error) {
    console.error('Ошибка при загрузке данных подборки:', error);
  }
};

const handleEditCollection = (collectionId) => {
  selectedCollectionId.value = collectionId;
  getCollectionData(collectionId);
};

const closeEditCollection = () => {
  showEditForm.value = false;
};

const openAddForm = () => {
  showAddForm.value = true;
};

const closeAddForm = () => {
  showAddForm.value = false;
};
</script>

<template>
  <div v-if="!showEditForm && !showAddForm">
    <h1>Управление подборками</h1>
    <div class="search-filter">
      <input
        type="text"
        placeholder="Поиск подборки по названию..."
        v-model="searchQuery"
      />
    </div>
    <button class="add-button" @click="openAddForm">Добавить подборку</button>
    <CollectionsTable
      :collections="filteredCollections"
      @edit-collection="handleEditCollection"
    />
  </div>
  <BookCollection
    v-if="showEditForm && !showAddForm"
    :collectionId="selectedCollectionId"
    :initialData="{
      titleCollection: selectedCollection.title,
      textCollection: selectedCollection.description,
      initialBooks: selectedCollection.books,
    }"
    :closeCollectionForm="closeEditCollection"
    :isAdminContext="true"
    @refresh-data="getCollections"
  />
  <BookCollection
    v-else-if="showAddForm && !showEditForm"
    :close-collection-form="closeAddForm"
    :isAdminContext="true"
    @refresh-data="getCollections"
  />
</template>

<style scoped>
h1 {
  text-align: center;
  font-size: 24px;
}

.search-filter {
  display: flex;
  justify-content: space-between;
  gap: 15px;
  margin-bottom: 20px;
}

.search-filter input {
  height: auto;
  width: 100%;
  padding: 10px;
  font-size: 14px;
  border: 1px solid lightgrey;
  border-radius: 5px;
}

.add-button {
  padding: 10px 20px;
  font-size: 14px;
  color: white;
  background-color: forestgreen;
  border: none;
  border-radius: 5px;
}

.add-button:hover {
  background-color: darkgreen;
}
</style>
