<script setup>
import { ref, computed, watch } from 'vue';
import { useStore } from 'vuex';
import userActivityService from '@/services/userActivityService';

import BookRatingModal from '../modals/BookRatingModal.vue';

const props = defineProps({
  id: { type: Number, required: true },
  isbN13: { type: Number, required: true },
  description: { type: String, required: true },
  yearPublication: { type: Number, required: true },
  pageCount: { type: Number, required: true },
  languageBook: { type: String, required: true },
  category: { type: String, required: true },
  publisher: { type: String, required: true },
  averageRating: { type: Number, required: true },
  countView: { type: Number, required: true },
});

const emit = defineEmits(['refresh-book-data']);

const store = useStore();
const isAuthenticated = computed(() => store.getters['auth/isAuthenticated']);
const user = computed(() => store.getters['auth/user']);
const idUser = computed(() => user.value?.idUser || null);

const showBookRatingModal = ref(false);
const userRating = ref(0);

const openBookRatingModal = () => (showBookRatingModal.value = true);
const closeModal = () => (showBookRatingModal.value = false);

const loadUserRating = async () => {
  if (isAuthenticated.value && idUser.value) {
    try {
      const rating = await userActivityService.getBookRating(
        idUser.value,
        props.id
      );
      userRating.value = rating;
    } catch (error) {
      console.error('Ошибка при загрузке рейтинга:', error);
    }
  } else {
    userRating.value = 0;
  }
};
loadUserRating();

const handleRatingSubmit = async (newRating) => {
  try {
    await userActivityService.updateBookRating(
      idUser.value,
      props.id,
      newRating
    );
    userRating.value = newRating;
    console.log('Рейтинг обновлен:', newRating);
    emit('refresh-book-data');
  } catch (error) {
    console.error('Ошибка при обновлении рейтинга:', error);
  }
};

const handleRatingDelete = async () => {
  try {
    await userActivityService.deleteBookRating(idUser.value, props.id);
    userRating.value = 0;
    console.log('Рейтинг удален.');
    emit('refresh-book-data');
  } catch (error) {
    console.error('Ошибка при удалении рейтинга:', error);
  }
};

watch(isAuthenticated, (newValue) => {
  if (newValue) {
    loadUserRating();
  }
});
</script>

<template>
  <div style="padding: 10px">
    <BookRatingModal
      :isVisible="showBookRatingModal"
      :initialRating="userRating"
      @close="closeModal"
      @submit="handleRatingSubmit"
      @delete="handleRatingDelete"
    />
    <div class="views-rating">
      <div class="book-rating">
        <div>
          ☆ <span>{{ averageRating.toFixed(2) }}</span>
        </div>
        <button
          v-if="isAuthenticated && userRating > 0"
          @click="openBookRatingModal"
        >
          Ваша оценка: {{ userRating }}
        </button>
        <button
          v-else
          @click="openBookRatingModal"
          :disabled="!isAuthenticated"
        >
          ☆ Оценить
        </button>
      </div>
      <div>
        👁 <span>{{ countView }}</span>
      </div>
    </div>

    <div class="description-container">
      <div class="header">Описание книги</div>
      <div class="book-description">{{ description }}</div>
    </div>

    <div class="book-details">
      <div class="header">Информация об издании</div>
      <div>
        ISBN-13: <span>{{ isbN13 }}</span>
      </div>
      <div>
        Издатель: <span>{{ publisher }}</span>
      </div>
      <div>
        Год издания: <span>{{ yearPublication }}</span>
      </div>
      <div>
        Категория: <span>{{ category }}</span>
      </div>
      <div>
        Количество страниц: <span>{{ pageCount }}</span>
      </div>
      <div>
        Язык книги: <span>{{ languageBook }}</span>
      </div>
    </div>
  </div>
</template>

<style scoped>
button {
  font-size: 16px;
  border-color: forestgreen;
  border-radius: 5px;
  background-color: white;
}

button:hover {
  border-color: darkgreen;
  font-weight: bold;
}

.views-rating {
  display: flex;
  justify-content: space-between;
  font-size: 20px;
}

.book-rating {
  display: flex;
  flex-direction: row;
  gap: 10px;
}

.book-details {
  display: flex;
  flex-direction: column;
  gap: 5px;
}

.description-container {
  display: flex;
  flex-direction: column;
}

.header {
  font-size: 22px;
  font-weight: bold;
}
</style>
