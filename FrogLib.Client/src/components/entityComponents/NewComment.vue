<script setup>
import { ref, computed } from 'vue';
import { useStore } from 'vuex';
import { useRoute } from 'vue-router';

import userActivityService from '@/services/userActivityService';

const emit = defineEmits(['refresh-data']);

const store = useStore();
const route = useRoute();
const isAuthenticated = computed(() => store.getters['auth/isAuthenticated']);
const user = computed(() => store.getters['auth/user']);
const idUser = computed(() => user.value?.idUser || null);

const typeEntity = route.meta.entityType;
const idEntity = route.params.id;
const textComment = ref('');

const addNewComment = async () => {
  if (isAuthenticated.value && idUser.value) {
    if (textComment !== '') {
      await userActivityService.addNewComment(
        idUser.value,
        idEntity,
        typeEntity,
        textComment.value
      );
      textComment.value = '';
      console.log('Комментарий добавлен.');
      emit('refresh-data');
    } else {
      console.error('Комментарий не может быть пустым.');
    }
  }
};
</script>

<template>
  <div class="new-comment">
    <textarea
      placeholder="Ваш комментарий...."
      v-model="textComment"
    ></textarea>
    <button :disabled="!isAuthenticated" @click="addNewComment">
      Отправить
    </button>
  </div>
</template>

<style scoped>
.new-comment {
  background-color: white;
  border-radius: 5px;
  border-bottom: 1px solid forestgreen;
  padding: 5px;
  display: flex;
  gap: 5px;
  align-items: center;
  margin-bottom: 10px;
}

.new-comment button {
  height: 30px;
  border-radius: 5px;
  border: none;
  background-color: forestgreen;
  color: white;
  font-size: 16px;
}
</style>
