<script setup>
import { ref, computed } from 'vue';
import { useStore } from 'vuex';
import dayjs from 'dayjs';
import 'dayjs/locale/ru';
dayjs.locale('ru');

import userActivityService from '@/services/userActivityService';

const props = defineProps({
  comment: { type: Object, required: true },
});

const emit = defineEmits(['update']);

const store = useStore();
const user = computed(() => store.getters['auth/user']);
const userId = computed(() => user.value?.idUser || null);

const editedText = ref(props.comment.contentComment);

const formatDate = (dateString) => {
  return dayjs(dateString).format('DD.MM.YYYY');
};

const handleSave = async () => {
  try {
    await userActivityService.updateUserComment(
      userId.value,
      props.comment.idComment,
      editedText.value
    );
    emit('update');
  } catch (error) {
    console.error('Ошибка при редактировании комментариев:', error);
  }
};

const handleDelete = async () => {
  try {
    await userActivityService.deleteUserComment(
      userId.value,
      props.comment.idComment
    );
    emit('update');
  } catch (error) {
    console.error('Ошибка при удалении комментария:', error);
  }
};
</script>

<template>
  <div class="comment-card">
    <div class="card-header">
      <div>
        Комментарий к:
        <RouterLink
          v-if="comment.typeEntity === 'Рецензия'"
          :to="`/reviews/${comment.idEntity}`"
          >{{ comment.entityName }}</RouterLink
        >
        <RouterLink
          v-if="comment.typeEntity === 'Подборка'"
          :to="`/collections/${comment.idEntity}`"
          >{{ comment.entityName }}</RouterLink
        >
      </div>
    </div>
    <div class="card-content">
      <img
        v-if="user?.profileImageUrl"
        :src="`https://localhost:7157${user?.profileImageUrl}`"
      />
      <img v-else src="@/assets/user_photo.png" />
      <div class="user">
        <div>{{ user?.nameUser }}</div>
        <div class="date">{{ formatDate(comment.dateComment) }}</div>
      </div>
    </div>
    <textarea v-model="editedText"></textarea>
    <div class="card-footer">
      <button class="delete" @click="handleDelete">Удалить</button>
      <button class="save" @click="handleSave">Сохранить изменения</button>
    </div>
  </div>
</template>

<style scoped>
.comment-card {
  display: flex;
  flex-direction: column;
  gap: 5px;
  padding: 5px;
  background-color: white;
  border-radius: 5px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  border-bottom: 1px solid forestgreen;
}

.card-header a:hover {
  color: forestgreen;
  font-weight: bold;
}

.card-content {
  display: flex;
  gap: 3px;
}

.card-content img {
  height: 80px;
}

.user {
  display: flex;
  flex-direction: column;
}

.date {
  color: grey;
}

.card-footer button {
  font-size: 14px;
  border: none;
  background: none;
}

.card-footer button.save:hover {
  color: darkgreen;
}

.card-footer button.delete:hover {
  color: darkred;
}
</style>
