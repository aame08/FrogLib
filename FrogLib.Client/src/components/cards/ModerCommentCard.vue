<script setup>
import { ref, computed, watch } from 'vue';
import dayjs from 'dayjs';
import 'dayjs/locale/ru';
import moderService from '@/services/moderService';
dayjs.locale('ru');

const props = defineProps({
  comment: { type: Object, required: true },
});

const emit = defineEmits(['refresh-data']);

const categories = [
  'Спам',
  'Оскорбления',
  'Мошенничество',
  'Призывы к насилию',
  'Другое',
];

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

  return result;
};

const highlightedText = computed(() => {
  return highlightForbiddenWords(props.comment.content);
});

const formattedDate = () => {
  return dayjs(props.comment.date).isValid()
    ? dayjs(props.comment.date).format('DD MMMM YYYY')
    : 'Неверный формат даты';
};

const updateStatusComment = async () => {
  try {
    await moderService.updateStatusComment(props.comment.id);
    console.log('Комментарий проверен.');
    emit('refresh-data');
  } catch (error) {
    console.error('Ошибка при изменении статуса комментария:', error);
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
      idUser: props.comment.authorId,
      categoryViolation: categoryViolation.value,
      descriptionViolation: textViolation.value,
    };

    await moderService.deleteComment(props.comment.id, violationData);
    console.log('Нарушение отправлено.');
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
  <div
    :class="[
      'comment',
      {
        'reply-comment': props.comment.isReply,
        'new-comment': props.comment.status === 'Новое',
        'violation-comment': props.comment.status === 'Обнаружено нарушение',
      },
    ]"
  >
    <div class="comment-header">
      <strong
        >Автор:
        <span class="comment-author">{{ props.comment.author }}</span></strong
      >
      <span class="comment-date">{{ formattedDate() }}</span>
    </div>
    <div class="comment-text" v-html="highlightedText"></div>
    <div class="comment-actions" v-if="!showViolationsForm">
      <button class="button-reject" @click="showViolationsForm = true">
        Удалить с нарушением
      </button>
      <button
        class="button"
        v-if="
          props.comment.status === 'Новое' ||
          props.comment.status === 'Обнаружено нарушение'
        "
        @click="updateStatusComment"
      >
        Просмотрено
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
    <div
      class="replies"
      v-if="props.comment.replies && props.comment.replies.length > 0"
    >
      <ModerCommentCard
        v-for="reply in props.comment.replies"
        :key="reply.id"
        :comment="reply"
        @refresh-data="$emit('refresh-data')"
      />
    </div>
  </div>
</template>

<style scoped>
.comment {
  padding: 10px;
  margin-bottom: 10px;
  border: 1px solid lightgrey;
  border-radius: 5px;
}

.reply-comment {
  margin-top: 10px;
}

.new-comment {
  border: 1px solid forestgreen;
  border-radius: 5px;
}

.violation-comment {
  border: 1px solid crimson;
  border-radius: 5px;
}

.comment-header {
  display: flex;
  justify-content: space-between;
  margin-bottom: 5px;
  font-size: 14px;
}

.comment-author {
  font-weight: normal;
}

.comment-date {
  font-style: italic;
  color: grey;
}

.replies {
  margin-left: 20px;
  margin-top: 10px;
  padding-left: 10px;
  border-left: 2px solid #ddd;
}

.comment-actions {
  display: flex;
  gap: 10px;
  margin-top: 10px;
}

.button-reject {
  padding: 5px 10px;
  color: white;
  border: none;
  border-radius: 5px;
  background-color: crimson;
}

.button-reject:hover {
  background-color: darkred;
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
  border: 1px solid darkred;
  border-radius: 5px;
  margin: 5px;
  padding: 5px;
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
