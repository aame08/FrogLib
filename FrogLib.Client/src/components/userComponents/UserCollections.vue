<script setup>
import { ref, computed } from 'vue';
import { useStore } from 'vuex';
import userActivityService from '@/services/userActivityService';

import UserCollectionCard from '../cards/UserCollectionCard.vue';

const emit = defineEmits(['open-collection']);

const store = useStore();
const user = computed(() => store.getters['auth/user']);
const userId = computed(() => user.value?.idUser || null);

const activeTab = ref('my');
const collections = ref([]);
const likedCollections = ref([]);

const loadData = async () => {
  try {
    if (activeTab.value === 'my') {
      const response = await userActivityService.getUserCollections(
        userId.value
      );
      collections.value = response;
    } else {
      const response = await userActivityService.getUserLikedCollections(
        userId.value
      );
      likedCollections.value = response;
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

const displayedCollections = computed(() => {
  return activeTab.value === 'my' ? collections.value : likedCollections.value;
});

const handleOpenCollection = (collectionId) => {
  emit('open-collection', collectionId);
};
</script>

<template>
  <div class="view-container">
    <div>
      <fieldset class="aside-left">
        <legend>Фильтр</legend>
        <div class="menu">
          <label class="menu-item">
            <input
              type="radio"
              name="collection-filter"
              value="my"
              v-model="activeTab"
              @change="switchTab('my')"
              :checked="activeTab === 'my'"
            />
            Мои подборки
          </label>
          <label class="menu-item">
            <input
              type="radio"
              name="collection-filter"
              value="liked"
              v-model="activeTab"
              @change="switchTab('liked')"
              :checked="activeTab === 'liked'"
            />
            Избранное
          </label>
        </div>
      </fieldset>
    </div>
    <div class="view-collection">
      <div class="collections-container">
        <UserCollectionCard
          v-for="collection in displayedCollections"
          :key="collection.idCollection"
          :collection="collection"
          @open-collection="handleOpenCollection"
        />
        <div
          v-if="displayedCollections.length === 0"
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

.view-collection {
  width: 100%;
  margin-top: 10px;
  margin-left: 15px;
  padding: 5px;
  background-color: whitesmoke;
  border-radius: 5px;
}

.collections-container {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
  padding: 10px;
  justify-content: center;
}
</style>
