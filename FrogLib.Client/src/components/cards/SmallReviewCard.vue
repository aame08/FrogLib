<script setup>
import { computed } from 'vue';

const props = defineProps({
  id: { type: Number, required: true },
  rating: { type: Number, required: true },
  countView: { type: Number, required: true },
  imageURL: { type: String, required: true },
  title: { type: String, required: true },
  content: { type: String, required: true },
});

const truncatedContent = computed(() => {
  const maxLength = 100;
  if (!props.content || props.content.trim() === '') {
    return 'Нет описания.';
  }
  return props.content.length > maxLength
    ? props.content.slice(0, maxLength) + '...'
    : props.content;
});
</script>

<template>
  <RouterLink :to="`/reviews/${id}`" class="review-card">
    <div class="card-header">
      <div>♡ {{ rating.toFixed(0) }} %</div>
      <div>👁 {{ countView }}</div>
    </div>
    <img :src="imageURL" :alt="title" />
    <div class="title">{{ title }}</div>
    <p v-html="truncatedContent"></p>
    <div class="link">Читать полностью</div>
  </RouterLink>
</template>

<style scoped>
.review-card {
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  min-width: 25%;
  max-width: 25%;
  height: auto;
  padding: 5px;
  background-color: white;
  border: none;
  border-radius: 8px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}
.review-card:hover {
  border: 1px solid forestgreen;
}

.card-header {
  display: flex;
  justify-content: space-between;
  padding: 5px;
  color: white;
  background-color: forestgreen;
}

.review-card img {
  height: 200px;
}

.title {
  font-size: 18px;
  font-weight: bold;
  max-width: 300px;
}

.review-card p {
  color: grey;
  max-width: 95%;
}

.link:hover {
  font-weight: bold;
}
</style>
