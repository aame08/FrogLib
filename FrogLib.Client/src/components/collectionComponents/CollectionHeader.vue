<script setup>
import { computed } from 'vue';
import { useStore } from 'vuex';
import dayjs from 'dayjs';
import 'dayjs/locale/ru';
dayjs.locale('ru');

import userPhotoPlaceholder from '@/assets/user_photo.png';

const props = defineProps({
  title: { type: String, required: true },
  countBooks: { type: Number, required: true },
  userURL: { type: String, required: true },
  userName: { type: String, required: true },
  userId: { type: Number, required: true },
  createdDate: { type: String, required: true },
  description: { type: String, required: true },
});

const emit = defineEmits(['edit-collection']);

const store = useStore();
const isAuthenticated = computed(() => store.getters['auth/isAuthenticated']);
const user = computed(() => store.getters['auth/user']);
const idUser = computed(() => user.value?.idUser || null);

const handleEditClick = () => {
  emit('edit-collection');
};

const formattedDate = () => {
  return dayjs(props.createdDate).isValid()
    ? dayjs(props.createdDate).format('DD MMMM YYYY')
    : 'Неверный формат даты';
};

const profileImageSrc = computed(() =>
  props.userURL
    ? `https://localhost:7157${props.userURL}`
    : userPhotoPlaceholder
);

console.log('author id', props.userId);
console.log('user id', idUser.value);
</script>

<template>
  <div class="collection-header">
    <div
      class="upper-container"
      v-if="isAuthenticated && props.userId === idUser"
    >
      <button @click="handleEditClick" title="Редактировать">🖋</button>
    </div>
    <h1 class="collection-title">{{ title }}</h1>
    <div class="count-books">
      Количество книг: <span>{{ countBooks }}</span>
    </div>
    <div class="lower-container">
      <img class="photo-user" :src="profileImageSrc" :alt="userName" />
      <div class="inner-container">
        <div class="collection-author">
          Автор: <span>{{ userName }}</span>
        </div>
        <div>
          Дата создания: <span>{{ formattedDate() }}</span>
        </div>
      </div>
    </div>
  </div>
  <div class="collection-info">
    Описание подборки: <span v-html="description"></span>
  </div>
</template>

<style scoped>
.collection-header {
  background-color: forestgreen;
  border-radius: 5px;
  padding: 15px;
  display: flex;
  flex-direction: column;
  color: white;
}

.upper-container {
  display: flex;
  justify-content: flex-end;
}

.upper-container button {
  background-color: forestgreen;
  border: none;
  font-size: 18px;
}

.collection-title {
  margin-top: 0;
  font-size: 36px;
}

.count-books {
  margin-top: -25px;
  font-size: 20px;
}

.lower-container {
  margin-top: 10px;
  display: flex;
  align-items: center;
  gap: 5px;
}

.photo-user {
  height: 100px;
}

.inner-container {
  display: flex;
  flex-direction: column;
}

.collection-info {
  font-weight: bold;
  margin-top: 10px;
  margin-left: 5px;
}

.collection-info span {
  font-weight: normal;
  white-space: pre-wrap;
}
</style>
