<script setup>
import { ref, computed } from 'vue';
import { useStore } from 'vuex';
import userActivityService from '@/services/userActivityService';

import UserCommentCard from '../cards/UserCommentCard.vue';

const store = useStore();
const user = computed(() => store.getters['auth/user']);
const userId = computed(() => user.value?.idUser || null);

const comments = ref([]);
const selectedOption = ref('all');

const getUserComments = async () => {
  try {
    const response = await userActivityService.getUserComments(userId.value);
    comments.value = response;
  } catch (error) {
    console.error('Ошибка при загрузке комментариев пользователя:', error);
  }
};
getUserComments();

const filteredComments = computed(() => {
  let result = comments.value;

  if (selectedOption.value !== 'all') {
    result = result.filter(
      (comment) => comment.typeEntity === selectedOption.value
    );
  }

  return result;
});
</script>

<template>
  <div class="view-container">
    <div>
      <fieldset class="aside-left">
        <legend>Фильтр</legend>
        <div class="menu">
          <label class="menu-item"
            ><input
              type="radio"
              name="list-type"
              value="all"
              v-model="selectedOption"
              checked
            />Все</label
          >
          <label class="menu-item"
            ><input
              type="radio"
              name="list-type"
              value="Рецензия"
              v-model="selectedOption"
            />К рецензии</label
          >
          <label class="menu-item"
            ><input
              type="radio"
              name="list-type"
              value="Подборка"
              v-model="selectedOption"
            />К подборке</label
          >
        </div>
      </fieldset>
    </div>
    <div class="view-comment">
      <div class="comments-container">
        <UserCommentCard
          v-for="comment in filteredComments"
          :key="comment.idComment"
          :comment="comment"
          @update="getUserComments"
        />
        <div
          v-if="filteredComments.length === 0"
          style="text-align: center; font-size: 20px"
        >
          Нет комментариев.
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
  padding: 5px;
  width: 230px;
  max-height: 350px;
  background-color: white;
  border: 2px solid forestgreen;
  border-radius: 8px;
  margin-top: 10px;
}

legend {
  font-weight: bold;
}

.menu {
  margin-top: 5px;
  display: flex;
  flex-direction: column;
  gap: 3px;
}

.menu-item {
  font-size: 17px;
}

.view-comment {
  display: flex;
  flex-direction: column;
  margin-top: 10px;
  margin-left: 15px;
  width: 100%;
  background-color: white;
  border-radius: 5px;
  padding: 5px;
}

.comments-container {
  display: flex;
  flex-direction: column;
  padding: 10px;
  gap: 5px;
}
</style>
