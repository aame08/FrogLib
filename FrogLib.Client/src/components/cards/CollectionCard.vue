<script setup>
import { computed } from 'vue';
import { formattedDate } from '@/utils/dateUtils';
import { truncateText } from '@/utils/truncateText';

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
  createdDate: { type: String, required: true },
  userName: { type: String, required: true },
  userURL: { type: String, required: true },
});

const truncatedDescription = computed(() => {
  return truncateText(props.description, 100);
});
</script>

<template>
  <RouterLink :to="`/collections/${id}`" class="collection-card">
    <div class="card-header">
      <div class="user-info">
        <img
          v-if="userURL"
          :src="`https://localhost:7157${userURL}`"
          alt="user-image"
        />
        <img v-else src="@/assets/user_photo.png" />
        <div>{{ userName }}</div>
      </div>
      <div class="collection-date">{{ formattedDate(props.createdDate) }}</div>
    </div>
    <div class="card-info">
      <div>♡ {{ rating.toFixed(0) }} %</div>
      <div>🕮 {{ countBooks }}</div>
    </div>
    <div class="images-container">
      <img
        v-for="(book, index) in books"
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
  width: 520px;
  padding: 5px;
  background-color: white;
  border: none;
  border-radius: 8px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}
.collection-card:hover {
  border: 1px solid forestgreen;
  text-decoration: none;
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

.user-info img {
  height: 20px;
  border-radius: 50%;
  overflow: hidden;
}

.collection-date {
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
