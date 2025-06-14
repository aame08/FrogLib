<script setup>
import { computed, watch } from 'vue';
import moderService from '@/services/moderService';
import useModeration from '@/composables/useModeration';

const props = defineProps({
  selectedReview: { type: Object, required: true },
  closeForm: { type: Function },
});

const emit = defineEmits(['refresh-data']);

const updateStatusFn = async (idReview, status) => {
  try {
    await moderService.updateReviewStatus(idReview, status);
    console.log('Статус изменен.');
    props.closeForm();
    emit('refresh-data');
  } catch (error) {
    console.error('Ошибка при изменении статуса рецензии:', error);
  }
};

const {
  localData,
  showViolationsForm,
  categoryViolation,
  textViolation,
  forbiddenWords,
  message,
  categories,
  getForbiddenWords,
  highlightForbiddenWords,
  submitViolation,
} = useModeration(props.selectedReview, updateStatusFn, 'Рецензия', 'idReview');

const highlightedText = computed(() => {
  return highlightForbiddenWords(localData.value.textReview);
});

const highlightedTitle = computed(() => {
  return highlightForbiddenWords(localData.value.titleReview);
});

getForbiddenWords();

const idReview = computed(() => localData.value?.idReview);

watch(showViolationsForm, (newVal) => {
  if (!newVal) {
    message.value = '';
    textViolation.value = '';
    categoryViolation.value = 'Спам';
  }
});
</script>

<template>
  <main>
    <h1>Просмотр рецензии</h1>
    <div class="review-container">
      <div class="inner-container">
        <p>Рецензия на книгу:</p>
        <img :src="localData.book.imageURL" />
        <span>{{ localData.book.title }}</span>
      </div>
      <div class="inner-container">
        <p>Автор:</p>
        <span>{{ localData.author.name }}</span>
      </div>
      <div class="inner-container">
        <p>Заголовок:</p>
        <div v-html="highlightedTitle"></div>
      </div>
      <div class="inner-container">
        <p>Текст:</p>
        <div v-html="highlightedText"></div>
      </div>
      <div class="inner-container">
        <p>Оценка пользователя:</p>
        <span>{{ localData.userRating }}</span>
      </div>
      <div class="buttons-form" v-if="!showViolationsForm">
        <button class="button red" @click="props.closeForm">Отмена</button>
        <button class="button red" @click="showViolationsForm = true">
          Отклонить с нарушением
        </button>
        <button
          class="button red"
          @click="updateStatusFn(idReview, 'Отказано')"
        >
          Отклонить
        </button>
        <button class="button" @click="updateStatusFn(idReview, 'Одобрено')">
          Принять
        </button>
      </div>
      <div class="violations-container" v-else>
        <div class="violations-info">
          <label>
            Выберите категорию:
            <select v-model="categoryViolation">
              <option
                v-for="category in categories"
                :key="category"
                :value="category"
              >
                {{ category }}
              </option>
            </select></label
          >
          <label
            >Описание нарушения:
            <div v-if="message" class="message">{{ message }}</div>
            <textarea v-model="textViolation"></textarea>
          </label>
        </div>
        <div class="violations-buttons">
          <button class="button red" @click="showViolationsForm = false">
            Отмена
          </button>
          <button class="button" @click="submitViolation">Сохранить</button>
        </div>
      </div>
    </div>
  </main>
</template>

<style scoped>
main {
  max-width: 1200px;
  margin-left: auto;
  margin-right: auto;
  padding: 20px;
  border-radius: 5px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

h1 {
  text-align: center;
  text-decoration: underline;
  text-decoration-color: forestgreen;
  font-size: 28px;
  margin-bottom: 20px;
}

.review-container {
  display: flex;
  flex-direction: column;
  padding: 20px;
  background-color: white;
  border: 1px solid forestgreen;
  border-radius: 5px;
}

.inner-container {
  display: flex;
  align-items: center;
  gap: 10px;
}

.inner-container img {
  height: 150px;
  border-radius: 3px;
}

p {
  font-weight: bold;
  width: 150px;
  flex-shrink: 0;
}

span {
  word-break: break-word;
}

.buttons-form {
  margin-top: 25px;
  display: flex;
  gap: 15px;
  justify-content: center;
}

.button {
  padding: 10px 20px;
  background-color: forestgreen;
  color: white;
  border: none;
  border-radius: 5px;
}

.button:hover {
  background-color: darkgreen;
}

.button.red {
  background-color: crimson;
}

.button.red:hover {
  background-color: darkred;
}

.violations-container {
  align-self: center;
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.violations-info {
  display: flex;
  flex-direction: column;
}

.violations-container label {
  font-weight: bold;
}

.violations-buttons {
  display: flex;
  justify-content: center;
  gap: 5px;
}

.message {
  color: grey;
  text-align: center;
}
</style>
