<script setup>
import { computed } from 'vue';
import { useStore } from 'vuex';
import { formattedDate } from '@/utils/dateUtils';

const props = defineProps({
  id: { type: Number, required: true },
  author: { type: String, required: true },
  authorURL: { type: String, required: true },
  date: { type: String, required: true },
  content: { type: String, required: true },
  replies: { type: Array, required: true },
});

const store = useStore();
const isAuthenticated = computed(() => store.getters['auth/isAuthenticated']);
</script>

<template>
  <div class="comment reply-comment">
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
          <div class="comment-date">{{ formattedDate(props.date) }}</div>
        </div>
        <div class="comment-content">{{ content }}</div>
      </div>
    </div>
    <div class="comment-footer">
      <button class="button-reply" :disabled="!isAuthenticated">
        Ответить
      </button>
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

.reply-comment {
  margin-top: 10px;
}
</style>
