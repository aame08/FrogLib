<script setup>
import { ref, computed, watch } from 'vue';
import { useStore } from 'vuex';
import { useRoute } from 'vue-router';
import userActivityService from '@/services/userActivityService';

const props = defineProps({
  bookRating: { type: Number, required: true },
  title: { type: String, required: true },
  content: { type: String, required: true },
  countView: { type: Number, required: true },
  rating: { type: Number, required: true },
  likes: { type: Number, required: true },
  dislikes: { type: Number, required: true },
});

const emit = defineEmits(['refresh-review-data']);

const store = useStore();
const route = useRoute();
const isAuthenticated = computed(() => store.getters['auth/isAuthenticated']);
const user = computed(() => store.getters['auth/user']);
const idUser = computed(() => user.value?.idUser || null);

const typeEntity = route.meta.entityType;
const userRating = ref(null);

const loadUserRating = async () => {
  if (isAuthenticated.value && idUser.value) {
    try {
      const rating = await userActivityService.getEntityRating(
        idUser.value,
        route.params.id,
        typeEntity
      );
      userRating.value = rating;
    } catch (error) {
      console.error('Ошибка при получении рейтинга:', error);
    }
  }
};
loadUserRating();

const updateRating = async (newRating) => {
  if (!isAuthenticated.value || !idUser.value) {
    console.warn('Пользователь не авторизован.');
    return;
  }
  try {
    await userActivityService.updateEntityRating(
      idUser.value,
      route.params.id,
      typeEntity,
      newRating
    );
    userRating.value = newRating;
    emit('refresh-review-data');
    console.log('Рейтинг обновлен:', newRating);
  } catch (error) {
    console.error('Ошибка при изменении рейтинга:', error);
  }
};

watch(isAuthenticated, (newValue) => {
  if (newValue) {
    loadUserRating();
  }
});
</script>

<template>
  <div class="review-content">
    <div class="content-header">
      <div class="review-evaluation">★ {{ bookRating.toFixed(0) }}</div>
      <div class="review-name">{{ title }}</div>
    </div>
    <div class="review-text" v-html="content"></div>
  </div>
  <div class="footer-container">
    <div class="container views">
      👁 <span>{{ countView }}</span>
    </div>
    <div class="container reaction">
      <button
        class="reaction-button like"
        :class="{ active: userRating === 1 }"
        @click="updateRating(1)"
        title="Рецензия понравилась"
      >
        <div class="rating-text">{{ likes.toFixed(0) }}</div>
        🖒
      </button>
      {{ rating.toFixed(0) }}%
      <button
        class="reaction-button dislike"
        :class="{ active: userRating === 0 }"
        @click="updateRating(0)"
        title="Рецензия не понравилась"
      >
        🖓
        <div class="rating-text">{{ dislikes.toFixed(0) }}</div>
      </button>
    </div>
  </div>
</template>

<style scoped>
.review-content {
  margin-top: 20px;
  display: flex;
  flex-direction: column;
  gap: 25px;
  padding: 5px;
}

.content-header {
  display: flex;
  gap: 10px;
}

.review-evaluation {
  font-size: 20px;
}

.review-name {
  font-weight: bold;
  font-size: 20px;
}

.review-text {
  white-space: pre-wrap;
}

.footer-container {
  display: flex;
  align-items: center;
  align-self: flex-start;
  margin-left: 5px;
  gap: 10px;
  padding: 5px;
}

.container {
  display: flex;
  align-items: center;
}

.reaction-button {
  display: flex;
  align-items: center;
  gap: 1px;
  font-size: 26px;
  color: black;
  border: none;
  background: none;
}

.liked-button,
.views {
  font-size: 20px;
  color: black;
}

.like:hover {
  color: darkgreen;
}

.dislike:hover {
  color: darkred;
}

.like.active,
.liked-button.active {
  color: darkgreen;
}

.dislike.active {
  color: darkred;
}

.rating-text {
  font-size: 16px;
}
</style>
