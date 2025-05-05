<script setup>
const props = defineProps({
  id: { type: Number, required: true },
  title: { type: String, required: true },
  averageRating: { type: Number, required: true },
  imageURL: { type: String, required: true },
});
</script>

<template>
  <RouterLink :to="`/books/${id}`" class="book-card">
    <div class="rating">☆ {{ averageRating.toFixed(1) }}</div>
    <img :src="imageURL" :alt="title" class="book-image" />
    <div class="book-title">{{ title }}</div>
  </RouterLink>
</template>

<style scoped>
.book-card {
  position: relative;
  display: flex;
  flex-direction: column;
  width: 23%;
  min-height: 300px;
  padding: 5px;
  background-color: white;
  border-radius: 8px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

.book-card:hover {
  border: 1px solid forestgreen;
}

.book-card img {
  height: 250px;
  margin-bottom: 10px;
}

.book-title {
  margin-top: auto;
  padding: 8px 4px;
  text-align: center;
  min-height: 60px;
  word-break: break-word;
}

.rating {
  width: 80px;
  height: 25px;
  text-align: center;
  background-color: forestgreen;
  color: white;
  border-radius: 5px;
}

.button-name {
  margin-top: 5px;
  font-size: 16px;
  background: none;
  border: none;
  text-wrap: wrap;
}

.button-name:hover {
  font-weight: bold;
}
</style>
