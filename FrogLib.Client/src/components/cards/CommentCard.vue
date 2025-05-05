<script setup>
import { ref, computed } from 'vue';
import { useStore } from 'vuex';
import { useRoute } from 'vue-router';
import dayjs from 'dayjs';
import 'dayjs/locale/ru';
dayjs.locale('ru');

import userActivityService from '@/services/userActivityService';

const props = defineProps({
  id: { type: Number, required: true },
  author: { type: String, required: true },
  authorURL: { type: String, required: true },
  date: { type: String, required: true },
  content: { type: String, required: true },
  isReply: { type: Boolean, required: true },
  replies: { type: Array, required: true },
});

const emit = defineEmits(['reply-added', 'refresh-data']);

const store = useStore();
const route = useRoute();
const isAuthenticated = computed(() => store.getters['auth/isAuthenticated']);
const user = computed(() => store.getters['auth/user']);
const idUser = computed(() => user.value?.idUser || null);

const typeEntity = route.meta.entityType;
const idEntity = route.params.id;
const showReplyInput = ref(false);
const replyText = ref('');

const formattedDate = () => {
  return dayjs(props.date).isValid()
    ? dayjs(props.date).format('DD MMMM YYYY')
    : 'Неверный формат даты';
};

const toggleReplyInput = () => {
  showReplyInput.value = !showReplyInput.value;
};

const submitReply = async () => {
  if (isAuthenticated.value && idUser.value) {
    try {
      if (replyText !== '') {
        await userActivityService.addReplyComment(
          idUser.value,
          idEntity,
          typeEntity,
          replyText.value,
          props.id
        );
        emit('refresh-data');
        replyText.value = '';
        showReplyInput.value = false;
        console.log('Ответ добавлен.');
      }
    } catch (error) {
      console.error('Ошибка при отправке ответов:', error);
    }
  }
};
</script>

<template>
  <div :class="['comment', { 'reply-comment': isReply }]">
    <div style="display: flex; gap: 15px">
      <img
        v-if="authorURL"
        :src="`https://localhost:7157${authorURL}`"
        :alt="author"
      />
      <img v-else src="@/assets/user_photo.png" :alt="author" />
      <div class="comment-container">
        <div class="comment-header">
          <div class="comment-author">{{ author }}</div>
          <div class="comment-date">{{ formattedDate() }}</div>
        </div>
        <div class="comment-content">
          {{ content }}
        </div>
      </div>
    </div>
    <div class="comment-footer">
      <button
        class="button-reply"
        :disabled="!isAuthenticated"
        @click="toggleReplyInput"
        v-if="!showReplyInput"
      >
        Ответить
      </button>
      <button
        class="button-reply"
        :disabled="!isAuthenticated"
        v-else="showReplyInput"
        @click="toggleReplyInput"
      >
        Отменить
      </button>
    </div>
    <div class="reply-input" v-if="showReplyInput">
      <textarea v-model="replyText" placeholder="Ваш ответ.."></textarea>
      <button class="button-reply" @click="submitReply">Отправить</button>
    </div>
    <div class="replies" v-if="replies && replies.length > 0">
      <CommentCard
        v-for="(reply, index) in replies"
        :key="index"
        :id="reply.id"
        :author="reply.author"
        :authorURL="reply.authorURL"
        :date="reply.date"
        :content="reply.content"
        :isReply="true"
        :replies="reply.replies"
        @refresh-data="$emit('refresh-data')"
      />
    </div>
  </div>
</template>

<style scoped>
.comment {
  background-color: white;
  border-radius: 5px;
  border: 1px solid lightgrey;
  padding: 10px;
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.reply-comment {
  margin-top: 10px;
}

.comment img {
  height: 50px;
}

.comment-container {
  display: flex;
  flex-direction: column;
  width: 100%;
  gap: 10px;
}

.comment-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 14px;
}

.comment-author {
  font-weight: bold;
  font-size: 16px;
}

.comment-date {
  font-style: italic;
  color: grey;
}

.comment-content {
  font-size: 14px;
}

.comment-footer {
  margin-top: 5px;
}

.button-reply {
  background: none;
  border: none;
  color: forestgreen;
  font-size: 14px;
}

.button-reply:hover {
  text-decoration: underline;
  text-decoration-color: darkgreen;
}

.reply-input {
  display: flex;
  padding: 5px;
  border-radius: 5px;
  border: 1px solid lightgrey;
}

.replies {
  margin-left: 20px;
  border-left: 2px solid #ddd;
  padding-left: 10px;
  margin-top: 10px;
}
</style>
