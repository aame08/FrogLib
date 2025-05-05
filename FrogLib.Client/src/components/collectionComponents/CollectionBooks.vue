<script setup>
import BookCard from '../cards/BookCard.vue';

const props = defineProps({
  books: { type: Array, requered: true },
});
</script>

<template>
  <div class="books-section">
    <BookCard
      v-for="(book, index) in books"
      :key="index"
      :id="book.id || index"
      :title="book.title"
      :imageURL="book.imageURL"
      :averageRating="book.averageRating"
    />
  </div>
</template>

<style scoped>
.books-section {
  margin-top: 10px;
  display: flex;
  flex-wrap: wrap;
  gap: 15px;
  padding: 10px;
}
</style>
