<script setup>
import NewComment from './NewComment.vue';
import CommentCard from '../cards/CommentCard.vue';

const props = defineProps({
  comments: { type: Array, required: true },
});
</script>

<template>
  <div class="comments-container">
    <h2>Комментарии</h2>
    <NewComment @refresh-data="$emit('refresh-data')" />
    <CommentCard
      v-for="(comment, index) in comments"
      :key="index"
      :id="comment.id"
      :author="comment.author"
      :authorURL="comment.authorURL"
      :date="comment.date"
      :content="comment.content"
      :isReply="comment.isReply"
      :replies="comment.replies"
      @refresh-data="$emit('refresh-data')"
    />
  </div>
</template>

<style scoped>
.comments-container {
  display: flex;
  flex-direction: column;
  gap: 5px;
  padding: 5px;
}
</style>
