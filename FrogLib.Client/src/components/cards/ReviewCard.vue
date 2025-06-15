<script setup>
import { computed } from 'vue';
import { formattedDate } from '@/utils/dateUtils';
import { truncateText } from '@/utils/truncateText';

const props = defineProps({
  id: { type: Number, required: true },
  title: { type: String, required: true },
  content: { type: String, required: true },
  imageURL: { type: String, required: true },
  rating: { type: Number, required: true },
  countView: { type: Number, required: true },
  createdDate: { type: String, required: true },
  userName: { type: String, required: true },
  userURL: { type: String, required: true },
});

const truncatedDescription = computed(() => {
  return truncateText(props.content, 150);
});
</script>

<template>
  <RouterLink :to="`/reviews/${id}`" class="review-card">
    <div class="card-header">
      <div class="user-info">
        <img
          class="user-image"
          v-if="userURL"
          :src="`https://localhost:7157${userURL}`"
          alt="user-image"
        />
        <img class="user-image" v-else src="@/assets/user_photo.png" />
        <div>{{ userName }}</div>
      </div>
      <div class="review-date">{{ formattedDate(props.createdDate) }}</div>
    </div>
    <div class="card-info">
      <div>♡ {{ rating.toFixed(0) }} %</div>
      <div>👁 {{ countView }}</div>
    </div>
    <img class="review-image" :src="imageURL" :alt="title" />
    <div class="review-title">{{ title }}</div>
    <p v-html="truncatedDescription"></p>
  </RouterLink>
</template>

<style scoped>
.review-card {
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  width: 48%;
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
}

.user-info {
  display: flex;
  justify-items: center;
  gap: 5px;
  font-size: 14px;
}

.user-image {
  height: 20px;
  border-radius: 50%;
  overflow: hidden;
}

.review-date {
  font-size: 12px;
  color: grey;
}

.card-info {
  display: flex;
  justify-content: space-between;
  padding: 5px;
  color: white;
  background-color: forestgreen;
}

.review-image {
  height: 200px;
}

.review-title {
  font-size: 18px;
  font-weight: bold;
  max-width: 300px;
}

.review-card p {
  color: grey;
  max-width: 95%;
}
</style>
