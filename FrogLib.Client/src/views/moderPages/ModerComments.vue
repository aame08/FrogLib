<script setup>
import { ref, computed } from 'vue';
import moderService from '@/services/moderService';

import EntityCard from '@/components/cards/EntityCard.vue';
import ModerCommentCard from '@/components/cards/ModerCommentCard.vue';

const entities = ref([]);
const searchQuery = ref('');
const selectedOption = ref('all');
const selectedEntity = ref(null);
const comments = ref([]);

const getEntities = async () => {
  try {
    const response = await moderService.getComments();
    entities.value = response;
  } catch (error) {
    console.error('Ошибка при загрузке:', error);
  }
};
getEntities();

const filteredEntities = computed(() => {
  let result = entities.value;

  if (searchQuery.value) {
    result = result.filter((entity) =>
      entity.entityName.toLowerCase().includes(searchQuery.value.toLowerCase())
    );
  }

  if (selectedOption.value !== 'all') {
    result = result.filter(
      (entity) => entity.typeEntity === selectedOption.value
    );
  }

  return result;
});

const loadComments = async (entity) => {
  if (!entity) return;

  comments.value = [];

  try {
    const response = await moderService.getCommentsForEntity(
      entity.typeEntity,
      entity.idEntity
    );
    comments.value = response;
  } catch (error) {
    console.error('Ошибка при загрузке комментариев:', error);
  }
};

const handleEntitySelect = (entity) => {
  selectedEntity.value = entity;
  loadComments(entity);
};

const refreshData = async () => {
  if (selectedEntity.value) {
    await loadComments(selectedEntity.value);
  }

  await getEntities();
};
</script>

<template>
  <h1>Модерация комментариев</h1>
  <div class="container">
    <fieldset class="aside-left">
      <legend>Список подборок/рецензий</legend>
      <div class="menu">
        <div class="search-container">
          <div style="display: flex; width: 50%">
            <input
              type="text"
              placeholder="Поиск по названию..."
              v-model="searchQuery"
            />
            <div class="icon">⌕</div>
          </div>
          <select v-model="selectedOption">
            <option value="all" selected>Все</option>
            <option value="Рецензия">Рецензии</option>
            <option value="Подборка">Подборки</option>
          </select>
        </div>
      </div>
      <EntityCard
        v-for="entity in filteredEntities"
        :key="entity.idEntity"
        :entity="entity"
        :isSelected="
          selectedEntity?.idEntity === entity.idEntity &&
          selectedEntity?.typeEntity === entity.typeEntity
        "
        @selected="handleEntitySelect"
      />
    </fieldset>
    <div class="comments-section">
      <h2>Комментарии</h2>
      <ModerCommentCard
        v-for="comment in comments"
        :key="comment.id"
        :comment="comment"
        @refresh-data="refreshData"
      />
    </div>
  </div>
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
  width: 450px;
  padding: 5px;
  height: fit-content;
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

.search-container {
  display: flex;
  align-items: center;
  gap: 5px;
  margin-bottom: 15px;
}

.search-container input {
  width: 100%;
  height: 30px;
  padding-left: 10px;
  border-radius: 5px 0 0 5px;
}

.search-container select {
  width: 50%;
  height: 30px;
}

.icon {
  width: 25px;
  padding: 5px;
  font-size: 16px;
  text-align: center;
  color: white;
  background-color: forestgreen;
  border: none;
  border-radius: 0 5px 5px 0;
}

.comments-section {
  flex: 1;
  height: fit-content;
  padding: 20px;
  background-color: white;
  border-radius: 5px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

.comments-section h2 {
  margin-top: 0;
}
</style>
