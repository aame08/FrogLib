<script setup>
import { computed, ref, watch } from 'vue';
import moderService from '@/services/moderService';

const props = defineProps({
  selectedReview: { type: Object, required: true },
  closeForm: { type: Function },
});

const emit = defineEmits(['refresh-data']);

const categories = [
  'Спам',
  'Оскорбления',
  'Мошенничество',
  'Призывы к насилию',
  'Другое',
];

const localReview = ref({ ...props.selectedReview });
const showViolationsForm = ref(false);
const categoryViolation = ref('Спам');
const textViolation = ref('');
const forbiddenWords = ref([]);
const message = ref('');

const getForbiddenWords = async () => {
  try {
    const response = await moderService.getForbiddenWords();
    forbiddenWords.value = Array.from(response);
  } catch (error) {
    console.error('Ошибка при загрузке запрещенных слов:', error);
  }
};
getForbiddenWords();

const highlightForbiddenWords = (text) => {
  if (!text || !forbiddenWords.value.length) return text;

  let result = text;

  forbiddenWords.value.forEach((word) => {
    const escapedWord = word.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
    const regex = new RegExp(
      `(^|\\s|[.,!?;:()\\[\\]{}"'])(${escapedWord})(?=$|\\s|[.,!?;:()\\[\\]{}"'])`,
      'gi'
    );
    result = result.replace(
      regex,
      (match, p1, p2) => `${p1}<span class="forbidden-word">${p2}</span>`
    );
  });

  console.log('Подсвеченный текст:', result);
  return result;
};

const highlightedText = computed(() => {
  return highlightForbiddenWords(localReview.value.textReview);
});

const highlightedTitle = computed(() => {
  return highlightForbiddenWords(localReview.value.titleReview);
});

const updateReviewStatus = async (status) => {
  try {
    await moderService.updateReviewStatus(localReview.value.idReview, status);
    console.log('Статус изменен.');
    props.closeForm();
    emit('refresh-data');
  } catch (error) {
    console.error('Ошибка при изменении статуса рецензии:', error);
  }
};

const submitViolation = async () => {
  try {
    message.value = '';

    if (textViolation.value === '') {
      message.value = 'Описание нарушения не может быть пустым.';
      return;
    }

    const violationData = {
      idUser: localReview.value.author.id,
      categoryViolation: categoryViolation.value,
      descriptionViolation: textViolation.value,
    };

    await moderService.updateReviewStatus(
      localReview.value.idReview,
      'Отказано'
    );
    await moderService.addViolation(violationData);
    console.log('Нарушение отправлено.');
    props.closeForm();
    emit('refresh-data');
  } catch (error) {
    console.error('Ошибка при отправке нарушения:', error);
  }
};

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
        <img :src="localReview.book.imageURL" />
        <span>{{ localReview.book.title }}</span>
      </div>
      <div class="inner-container">
        <p>Автор:</p>
        <span>{{ localReview.author.name }}</span>
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
        <span>{{ localReview.userRating }}</span>
      </div>
      <div class="buttons-form" v-if="!showViolationsForm">
        <button class="button red" @click="closeForm">Отмена</button>
        <button class="button red" @click="showViolationsForm = true">
          Отклонить с нарушением
        </button>
        <button class="button red" @click="updateReviewStatus('Отказано')">
          Отклонить
        </button>
        <button class="button" @click="updateReviewStatus('Одобрено')">
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
