<script setup>
import { ref, computed, watch } from 'vue';
import { useStore } from 'vuex';
import { useRoute } from 'vue-router';

import userActivityService from '@/services/userActivityService';

const props = defineProps({
  userId: { type: Number, required: true },
  countView: { type: Number, required: true },
  rating: { type: Number, required: true },
  likes: { type: Number, required: true },
  dislikes: { type: Number, required: true },
  countLiked: { type: Number, required: true },
});

const emit = defineEmits(['refresh-collection-data']);

const store = useStore();
const route = useRoute();
const isAuthenticated = computed(() => store.getters['auth/isAuthenticated']);
const user = computed(() => store.getters['auth/user']);
const idUser = computed(() => user.value?.idUser || null);

const typeEntity = route.meta.entityType;
const userRating = ref(null);
const isLiked = ref(false);

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

const loadLikedCollection = async () => {
  if (isAuthenticated.value && idUser.value) {
    try {
      const liked = await userActivityService.getLikedCollection(
        idUser.value,
        route.params.id
      );
      isLiked.value = liked;
    } catch (error) {
      console.error('Ошибка при получении сохранения:', error);
    }
  }
};
loadLikedCollection();

const toggleLike = async () => {
  if (!isAuthenticated.value || !idUser.value) {
    console.warn('Пользователь не авторизован.');
    return;
  }
  try {
    if (isLiked.value) {
      await userActivityService.unlikeCollection(idUser.value, route.params.id);
      isLiked.value = false;
      emit('refresh-collection-data');
      console.log('Коллекция удалена из избранных.');
    } else {
      await userActivityService.likeCollection(idUser.value, route.params.id);
      isLiked.value = true;
      emit('refresh-collection-data');
      console.log('Коллекция добавлена в избранные.');
    }
  } catch (error) {
    console.error('Ошибка при изменении:', error);
  }
};

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
    emit('refresh-collection-data');
    console.log('Рейтинг обновлен:', newRating);
  } catch (error) {
    console.error('Ошибка при изменении рейтинга:', error);
  }
};

watch(isAuthenticated, (newValue) => {
  if (newValue) {
    loadUserRating();
    loadLikedCollection();
  }
});
</script>

<template>
  <div class="footer-container">
    <div class="container views">
      👁 <span>{{ countView }}</span>
    </div>
    <div class="container reaction">
      <button
        class="reaction-button like"
        :class="{ active: userRating === 1 }"
        @click="updateRating(1)"
        title="Подборка понравилась"
      >
        <div class="rating-text">{{ likes.toFixed(0) }}</div>
        🖒
      </button>
      {{ rating.toFixed(0) }}%
      <button
        class="reaction-button dislike"
        :class="{ active: userRating === 0 }"
        @click="updateRating(0)"
        title="Подборка не понравилась"
      >
        🖓
        <div class="rating-text">{{ dislikes.toFixed(0) }}</div>
      </button>
    </div>
    <div class="container liked">
      <button
        class="liked-button"
        :class="{ active: isLiked }"
        @click="toggleLike"
        :disabled="isAuthenticated && props.userId === idUser.value"
        title="Добавить подборку в Избранное"
      >
        ⛉
      </button>
      {{ countLiked.toFixed(0) }}
    </div>
  </div>
</template>

<style scoped>
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
}

.liked-button,
.views {
  font-size: 20px;
  color: black;
}

.reaction-button,
.liked-button {
  border: none;
  background: none;
}

.like:hover,
.liked-button:hover {
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
