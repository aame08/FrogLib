<script setup>
import { computed } from 'vue';
import { useStore } from 'vuex';
import { formattedDate } from '@/utils/dateUtils';

import userPhotoPlaceholder from '@/assets/user_photo.png';

const props = defineProps({
  title: { type: String, required: true },
  userId: { type: Number, required: true },
  userURL: { type: String, required: true },
  userName: { type: String, required: true },
  createdDate: { type: String, required: true },
});

const emit = defineEmits(['edit-review']);

const store = useStore();
const isAuthenticated = computed(() => store.getters['auth/isAuthenticated']);
const user = computed(() => store.getters['auth/user']);
const idUser = computed(() => user.value?.idUser || null);

const handleEditClick = () => {
  emit('edit-review');
};

const profileImageSrc = computed(() =>
  props.userURL
    ? `https://localhost:7157${props.userURL}`
    : userPhotoPlaceholder
);
</script>

<template>
  <div class="review-header">
    <div
      class="upper-container"
      v-if="isAuthenticated && props.userId === idUser"
    >
      <button
        @click="handleEditClick"
        title="Редактировать"
        style="color: white"
      >
        🖋
      </button>
    </div>
    <h1 class="review-title">{{ title }}</h1>
    <div class="lower-container">
      <img class="photo-user" :src="profileImageSrc" :alt="userName" />
      <div class="inner-container">
        <div class="review-author">
          Автор: <span>{{ userName }}</span>
        </div>
        <div>
          Дата создания: <span>{{ formattedDate(props.createdDate) }}</span>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.review-header {
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

.review-title {
  margin-top: 0;
  font-size: 36px;
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
</style>
