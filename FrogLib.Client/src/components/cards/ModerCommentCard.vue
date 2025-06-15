<script setup>
import { computed, watch } from 'vue';
import moderService from '@/services/moderService';
import useModeration from '@/composables/useModeration';
import { formattedDate } from '@/utils/dateUtils';

const props = defineProps({
  comment: { type: Object, required: true },
});

const emit = defineEmits(['refresh-data']);

const updateStatusFn = async (idComment, status) => {
  try {
    await moderService.updateStatusComment(idComment, status);
    console.log('Статус комментария изменён.');
    emit('refresh-data');
  } catch (error) {
    console.error('Ошибка при изменении статуса комментария:', error);
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
} = useModeration(props.comment, updateStatusFn, 'Комментарий', 'id');

getForbiddenWords();

const idComment = computed(() => localData.value?.id);

const highlightedText = computed(() => {
  if (!localData.value || !localData.value.content) return '';
  return highlightForbiddenWords(localData.value.content);
});

watch(showViolationsForm, (newVal) => {
  if (!newVal) {
    message.value = '';
    textViolation.value = '';
    categoryViolation.value = 'Спам';
  }
});

const handleSubmitViolation = async () => {
  await submitViolation(() => {
    emit('refresh-data');
  });
};
</script>

<template>
  <div
    :class="[
      'comment',
      {
        'reply-comment': localData.isReply,
        'new-comment': localData.status === 'Новое',
        'violation-comment': localData.status === 'Обнаружено нарушение',
      },
    ]"
  >
    <div class="comment-header">
      <strong
        >Автор:
        <span class="comment-author">{{ localData.author }}</span></strong
      >
      <span class="comment-date">{{ formattedDate(localData.date) }}</span>
    </div>
    <div v-html="highlightedText" class="comment-text"></div>
    <div class="comment-actions" v-if="!showViolationsForm">
      <button class="button-reject" @click="showViolationsForm = true">
        Удалить с нарушением
      </button>
      <button
        class="button"
        v-if="
          localData.status === 'Новое' ||
          localData.status === 'Обнаружено нарушение'
        "
        @click="updateStatusFn(idComment, 'Просмотрено')"
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
        <button class="button" @click="handleSubmitViolation">Сохранить</button>
      </div>
    </div>
    <div
      class="replies"
      v-if="localData.replies && localData.replies.length > 0"
    >
      <ModerCommentCard
        v-for="reply in localData.replies"
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

.forbidden-word {
  color: crimson;
}
</style>
