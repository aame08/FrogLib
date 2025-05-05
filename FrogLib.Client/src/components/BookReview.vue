<script setup>
import { ref, computed, watchEffect } from 'vue';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';
import userActivityService from '@/services/userActivityService';

const props = defineProps({
  id: { type: Number, required: true },
  title: { type: String, required: true },
  imageURL: { type: String, required: true },
  authors: { type: String, required: true },
  averageRating: { type: Number, required: true },
  closeReviewForm: { type: Function, required: true },
  reviewId: { type: Number, default: null },
  initialData: { type: Object, default: () => ({}) },
});

const emit = defineEmits(['refresh-data']);

const store = useStore();
const router = useRouter();
const isAuthenticated = computed(() => store.getters['auth/isAuthenticated']);
const user = computed(() => store.getters['auth/user']);
const idUser = computed(() => user.value?.idUser || null);

const titleReview = ref('');
const textReview = ref('');
const isBold = ref(false);
const isItalic = ref(false);
const isUnderline = ref(false);
const selectedRating = ref(0);
const errors = ref({});

const loadUserRating = async () => {
  if (isAuthenticated.value && idUser.value) {
    try {
      const response = await userActivityService.getBookRating(
        idUser.value,
        props.id
      );
      selectedRating.value = response;
    } catch (error) {
      console.error('Ошибка при загрузке рейтинга:', error);
    }
  } else {
    selectedRating.value = 0;
  }
};

const setRating = (rating) => {
  selectedRating.value = rating;
};

const toggleFormatting = (type) => {
  const textarea = document.querySelector('textarea');
  const start = textarea.selectionStart;

  let newText = textReview.value;

  switch (type) {
    case 'bold':
      newText = isBold.value
        ? `${textReview.value.slice(0, start)}</b>${textReview.value.slice(
            start
          )}`
        : `${textReview.value.slice(0, start)}<b>${textReview.value.slice(
            start
          )}`;
      isBold.value = !isBold.value;
      break;
    case 'italic':
      newText = isItalic.value
        ? `${textReview.value.slice(0, start)}</i>${textReview.value.slice(
            start
          )}`
        : `${textReview.value.slice(0, start)}<i>${textReview.value.slice(
            start
          )}`;
      isItalic.value = !isItalic.value;
      break;
    case 'underline':
      newText = isUnderline.value
        ? `${textReview.value.slice(0, start)}</u>${textReview.value.slice(
            start
          )}`
        : `${textReview.value.slice(0, start)}<u>${textReview.value.slice(
            start
          )}`;
      isUnderline.value = !isUnderline.value;
      break;
  }

  textReview.value = newText;
  textarea.focus();
  textarea.setSelectionRange(start + 3, start + 3);
};

const saveReview = async () => {
  errors.value = {};
  let isValid = true;

  if (!titleReview.value.trim()) {
    errors.value.titleReview = 'Заголовок рецензии не может быть пустым.';
    isValid = false;
  }
  if (!textReview.value.trim()) {
    errors.value.textReview = 'Текст рецензии не может быть пустым.';
    isValid = false;
  }
  if (selectedRating.value === 0) {
    errors.value.selectedRating = 'Оценка не должна быть пустой.';
    isValid = false;
  }
  if (!isValid) return;

  try {
    const formattedText = textReview.value.replace(/\n/g, '<br>');

    if (props.reviewId) {
      await userActivityService.editReview(
        props.reviewId,
        idUser.value,
        props.id,
        titleReview.value,
        formattedText,
        selectedRating.value
      );

      props.closeReviewForm();

      if (router.currentRoute.value.path !== '/profile') {
        await router.push('/reviews');
      }
    } else {
      await userActivityService.addReview(
        idUser.value,
        props.id,
        titleReview.value,
        formattedText,
        selectedRating.value
      );

      props.closeReviewForm();
      emit('refresh-data');
    }
  } catch (error) {
    console.error('Ошибка при отправке рецензии:', error);
  }
};

watchEffect(() => {
  if (props.reviewId && props.initialData) {
    titleReview.value = props.initialData.titleReview || '';
    textReview.value = props.initialData.textReview || '';
    selectedRating.value = props.initialData.rating || 0;
  } else {
    loadUserRating();
  }
});
</script>

<template>
  <main>
    <div class="review-container">
      <h1 class="title-page">Рецензия на книгу</h1>

      <div class="book-cover">
        <div
          class="blurred-background"
          :style="{ backgroundImage: `url(${imageURL})` }"
        ></div>
        <div class="book-container">
          <div class="content-wrapper">
            <img :src="imageURL" :alt="title" class="book-image" />
            <div class="text-content">
              <h1 class="book-title">{{ title }}</h1>
              <div class="book-author">{{ authors }}</div>
              <div class="rating">☆ {{ averageRating.toFixed(2) }}</div>
            </div>
          </div>
        </div>
      </div>

      <div class="review-name">
        <h4>Заголовок рецензии:</h4>
        <input
          type="text"
          placeholder="Введите заголовок рецензии..."
          v-model="titleReview"
          :class="{ error: errors.titleReview }"
        />
      </div>
      <div v-if="errors.titleReview" class="error-message">
        {{ errors.titleReview }}
      </div>

      <div class="review-evaluation">
        <h4>Оценка книги:</h4>
        <div class="evaluations">
          <button
            v-for="star in 5"
            :key="star"
            class="star-button"
            :class="{
              selected: selectedRating >= star,
              error: errors.selectedRating,
            }"
            @click="setRating(star)"
          >
            {{ selectedRating >= star ? '★' : '☆' }}
          </button>
        </div>
      </div>
      <div v-if="errors.selectedRating" class="error-message">
        {{ errors.selectedRating }}
      </div>

      <div class="review-description">
        <h4>Текст рецензии:</h4>
        <div class="editor">
          <textarea
            v-model="textReview"
            :class="{ error: errors.textReview }"
            class="text-editor"
          ></textarea>
          <div class="toolbar">
            <button
              :class="{ active: isBold }"
              @click="toggleFormatting('bold')"
              title="Жирный текст"
            >
              <b>B</b>
            </button>
            <button
              :class="{ active: isItalic }"
              @click="toggleFormatting('italic')"
              title="Курсив"
            >
              <i>I</i>
            </button>
            <button
              :class="{ active: isUnderline }"
              @click="toggleFormatting('underline')"
              title="Подчеркнутый текст"
            >
              <u>U</u>
            </button>
          </div>
        </div>
      </div>
      <div v-if="errors.textReview" class="error-message">
        {{ errors.textReview }}
      </div>

      <div class="buttons-group">
        <button @click="closeReviewForm">Отмена</button>
        <button @click="saveReview">Сохранить</button>
      </div>
    </div>
  </main>
</template>

<style scoped>
main {
  margin-top: 70px;
  max-width: 1000px;
  margin-left: auto;
  margin-right: auto;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
}

.review-container {
  display: flex;
  flex-direction: column;
  background-color: white;
  border-radius: 5px;
  padding: 5px;
}

.title-page {
  text-align: center;
  text-decoration: underline;
  text-decoration-color: forestgreen;
}

.book-cover {
  display: flex;
  position: relative;
  align-self: center;
  width: 90%;
  padding: 10px;
  border-radius: 5px;
}

.blurred-background {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-size: cover;
  background-position: center;
  filter: blur(10px);
  z-index: 1;
}

.book-container {
  position: relative;
  z-index: 2;
  width: 100%;
  height: 100%;
  display: flex;
  justify-content: center;
  align-items: center;
}

.content-wrapper {
  display: flex;
  align-items: center;
  gap: 30px;
  padding: 20px;
  background-color: rgba(255, 255, 255, 0.3);
  backdrop-filter: blur(5px);
  border-radius: 8px;
}

.book-image {
  height: 250px;
}

.text-content {
  color: white;
  text-shadow: 0 2px 4px rgba(0, 0, 0, 0.5);
}

.book-title {
  margin: 0;
  max-width: 600px;
}

.book-title a {
  color: inherit;
}

.book-title a:hover {
  color: white;
  text-decoration: underline;
}

.book-author {
  font-size: 20px;
  margin-bottom: 20px;
}

.rating {
  font-size: 20px;
  font-weight: bold;
}

.review-name,
.review-description,
.review-evaluation {
  display: flex;
  align-items: center;
  gap: 10px;
}

h4 {
  width: 200px;
}

.review-name input {
  width: 80%;
  height: 35px;
  border-radius: 5px;
}

.star-button {
  background: none;
  border: none;
  font-size: 18px;
}

.star-button.selected {
  color: darkgreen;
}

.editor {
  position: relative;
  width: 80%;
}

.text-editor {
  height: 100px;
  font-size: 16px;
  border-radius: 5px;
}

.toolbar {
  margin-top: 10px;
}

.toolbar button {
  width: 35px;
  height: 35px;
  padding: 5px 10px;
  margin-right: 5px;
  font-size: 16px;
  background: none;
  border: none;
}

.toolbar button:hover {
  border: 1px solid forestgreen;
  border-radius: 5px;
}

.toolbar button.active {
  border: 1px solid forestgreen;
  border-radius: 5px;
}

.buttons-group {
  display: flex;
  gap: 15px;
  align-self: center;
  margin-top: 25px;
}

.buttons-group button {
  height: 40px;
  font-size: 16px;
  border-color: forestgreen;
  border-radius: 5px;
  background-color: white;
}

.buttons-group button:hover {
  font-weight: bold;
}

.error {
  border-color: darkred;
}

.error-message {
  color: crimson;
  font-size: 16px;
  text-align: center;
}
</style>