<script setup>
const props = defineProps({
  id: { type: Number, required: true },
  title: { type: String, required: true },
  imageURL: { type: String, required: true },
  averageRating: { type: Number, required: true },
});
</script>

<template>
  <router-link :to="`/books/${id}`" class="book-card">
    <div class="rating">☆ {{ averageRating.toFixed(1) }}</div>
    <img :src="imageURL" :alt="title" class="book-image" />
    <!-- <div class="book-title">{{ title }}</div> -->
  </router-link>
</template>

<style scoped>
.book-card {
  display: flex;
  flex-direction: column;
  width: 23%;
  padding: 5px;
  background-color: white;
  border-radius: 8px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

.book-card:hover {
  border: 1px solid forestgreen;
}

.book-card img {
  height: 350px;
}

.rating {
  width: 80px;
  height: 25px;
  text-align: center;
  background-color: forestgreen;
  color: white;
}

.button-name {
  display: flex;
  margin-top: 5px;
  font-size: 16px;
  text-wrap: wrap;
  background: none;
  border: none;
}
.button-name:hover {
  font-weight: bold;
}
</style>
