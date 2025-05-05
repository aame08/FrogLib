<script setup>
import { ref, computed } from 'vue';
import { useStore } from 'vuex';
import { useRoute } from 'vue-router';
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

const formattedDate = () => {
  return dayjs(props.comment.date).isValid()
    ? dayjs(props.comment.date).format('DD MMMM YYYY')
    : 'Неверный формат даты';
};

const submitViolation = async () => {
  try {
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
</script>

<template>
  <div :class="['comment', { 'reply-comment': props.comment.isReply }]">
    <div class="comment-header">
      <strong
        >Автор:
        <span class="comment-author">{{ props.comment.author }}</span></strong
      >
      <span class="comment-date">{{ formattedDate() }}</span>
    </div>
    <div class="comment-text">{{ props.comment.content }}</div>
    <div class="comment-actions" v-if="!showViolationsForm">
      <button class="button-reject" @click="showViolationsForm = true">
        Удалить с нарушением
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
          >Описание нарушения: <textarea v-model="textViolation"></textarea>
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
</style>
