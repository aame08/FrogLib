<script setup>
import dayjs from 'dayjs';
import 'dayjs/locale/ru';
dayjs.locale('ru');

const props = defineProps({
  review: { type: Object, required: true },
});

const emit = defineEmits(['open-review']);

const formatDate = (dateString) => {
  return dayjs(dateString).format('DD.MM.YYYY');
};

const truncatedDescription = (textReview) => {
  const maxLength = 100;
  return textReview.length > maxLength
    ? textReview.slice(0, maxLength) + '...'
    : textReview;
};

const handleClick = () => {
  emit('open-review', props.review.idReview);
};
</script>

<template>
  <div class="review-card">
    <div
      v-if="review.statusReview"
      class="review-status"
      :class="{
        approved: review.statusReview === 'Одобрено',
        rejected: review.statusReview === 'Отказано',
        pending: review.statusReview === 'На рассмотрении',
      }"
    >
      <span v-if="review.statusReview === 'Одобрено'">✓</span>
      <span v-else-if="review.statusReview === 'Отказано'">×</span>
      <span v-else-if="review.statusReview === 'На рассмотрении'">🕐</span>
      {{ review.statusReview }}
    </div>
    <img :src="review.imageURL" />
    <button @click="handleClick">{{ review.titleReview }}</button>
    <p v-html="truncatedDescription(review.textReview)"></p>
    <div class="date">
      Дата: <span>{{ formatDate(review.createdDate) }}</span>
    </div>
  </div>
</template>

<style scoped>
.review-card {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 10px;
  width: 500px;
  padding: 5px;
  box-sizing: border-box;
  background-color: white;
  border-radius: 5px;
  border-bottom: 2px solid forestgreen;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

.review-status {
  display: inline-block;
  padding: 4px 8px;
  font-size: 12px;
  font-weight: 500;
  color: white;
  border-radius: 0 0 5px 5px;
}

.approved {
  background-color: forestgreen;
}

.rejected {
  background-color: crimson;
}

.pending {
  background-color: grey;
}

.review-card img {
  height: 200px;
}

.review-card p {
  width: 100%;
  color: grey;
}

.review-card button {
  border: none;
  background: none;
  font-size: 18px;
}

.review-card button:hover {
  font-weight: bold;
}

.date {
  font-size: 14px;
}
</style>
