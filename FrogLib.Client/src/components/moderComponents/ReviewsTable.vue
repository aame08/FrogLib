<script setup>
const props = defineProps({
  reviews: { type: Array, required: true },
});

const emit = defineEmits(['review-view']);

const handleView = (review) => {
  emit('review-view', review);
};
</script>

<template>
  <table>
    <thead>
      <tr>
        <th>ID</th>
        <th>Имя пользователя</th>
        <th>Название книги</th>
        <th>Заголовок рецензии</th>
        <th>Действие</th>
      </tr>
    </thead>
    <tbody>
      <tr v-for="review in reviews" :key="review.idReview">
        <td>{{ review.idReview }}</td>
        <td>{{ review.author.name }}</td>
        <td>{{ review.book.title }}</td>
        <td>{{ review.titleReview }}</td>
        <td>
          <button class="action-button" @click="handleView(review)">
            Подробнее
          </button>
        </td>
      </tr>
    </tbody>
  </table>
</template>

<style scoped></style>
