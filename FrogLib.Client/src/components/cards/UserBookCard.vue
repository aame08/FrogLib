<script setup>
import dayjs from 'dayjs';
import 'dayjs/locale/ru';
dayjs.locale('ru');

const props = defineProps({
  book: {
    type: Object,
    required: true,
  },
});

const emit = defineEmits(['edit']);

const formatDate = (dateString) => {
  return dayjs(dateString).format('DD.MM.YYYY');
};
</script>

<template>
  <div class="book-card">
    <div class="img-name">
      <img :src="book.imageURL" :alt="book.titleBook" />
      <RouterLink :to="`/books/${book.idBook}`">{{
        book.titleBook
      }}</RouterLink>
    </div>
    <div class="date-adding">
      <div>Добавлено:</div>
      <div>{{ formatDate(book.addedDate) }}</div>
    </div>
    <button title="Редактировать" @click="emit('edit', book)">···</button>
  </div>
</template>

<style scoped>
.book-card {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 5px;
  background-color: white;
  border-radius: 5px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  border-bottom: 1px solid forestgreen;
}

.book-card img {
  height: 150px;
  width: 100px;
}

.img-name {
  display: flex;
  align-items: center;
  gap: 15px;
  margin-left: 15px;
}

.img-name a {
  font-size: 18px;
  width: 250px;
  max-width: 250px;
}

.img-name a:hover {
  font-weight: bold;
}

.book-card button {
  background: none;
  border: none;
}
</style>
