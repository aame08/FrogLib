<script setup>
import { computed } from 'vue';

const props = defineProps({
  id: { type: Number, required: true },
  title: { type: String, required: true },
  description: { type: String, required: true },
  rating: { type: Number, required: true },
  countBooks: { type: Number, required: true },
  books: { type: Array, required: true },
  countView: { type: Number, required: true },
  countComments: { type: Number, required: true },
  countLiked: { type: Number, required: true },
});

const truncatedDescription = computed(() => {
  const maxLength = 100;
  if (!props.description || props.description.trim() === '') {
    return 'Нет описания.';
  }
  return props.description.length > maxLength
    ? props.description.slice(0, maxLength) + '...'
    : props.description;
});
</script>

<template>
  <RouterLink :to="`/collections/${id}`" class="collection-card">
    <div class="card-header">
      <div>♡ {{ rating.toFixed(0) }} %</div>
      <div>🕮 {{ countBooks }}</div>
    </div>
    <div class="images-container">
      <img
        v-for="(book, index) in this.books"
        :key="book.id || index"
        :src="book.imageURL"
        :alt="book.title"
      />
    </div>
    <div class="collection-title">{{ title }}</div>
    <p v-html="truncatedDescription"></p>
    <div class="card-footer">
      <div>👁 {{ countView }}</div>
      <div>💬 {{ countComments }}</div>
      <div>⛉ {{ countLiked }}</div>
    </div>
  </RouterLink>
</template>

<style scoped>
.collection-card {
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  min-width: 40%;
  max-width: 40%;
  width: 400px;
  padding: 5px;
  background-color: white;
  border: none;
  border-radius: 8px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}
.collection-card:hover {
  border: 1px solid forestgreen;
}

.card-header {
  display: flex;
  justify-content: space-between;
  padding: 5px;
  color: white;
  background-color: forestgreen;
}

.images-container {
  display: flex;
  justify-content: center;
  align-items: center;
  padding: 5px;
}

.images-container img {
  min-height: 100px;
  max-height: 200px;
  min-width: 50px;
  max-width: 150px;
}

.collection-title {
  font-size: 24px;
  font-weight: bold;
}
.collection-title:hover {
  color: forestgreen;
}

.collection-card p {
  color: grey;
  max-width: 95%;
}

.card-footer {
  display: flex;
  align-items: center;
  gap: 15px;
  padding: 5px;
  border-top: 2px solid forestgreen;
}
</style>
