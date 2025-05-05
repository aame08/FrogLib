<script setup>
const props = defineProps({
  title: { type: String, required: true },
  authors: { type: String, required: true },
});
</script>

<template>
  <div class="book-header">
    <h1 class="book-title">{{ title }}</h1>
    <div class="book-author">{{ authors }}</div>
    <div class="buttons-group">
      <button @click="$emit('change-section', 'info')">Информация</button>
      <button @click="$emit('change-section', 'reviews')">Рецензии</button>
      <button @click="$emit('change-section', 'collections')">Подборки</button>
    </div>
  </div>
</template>

<style scoped>
.book-header {
  background-color: forestgreen;
  border-radius: 5px;
  padding: 15px;
  display: flex;
  flex-direction: column;
  color: white;
}

.book-title {
  font-size: 36px;
  max-width: 75%;
  align-self: flex-end;
}

.book-author {
  max-width: 50%;
  align-self: flex-end;
}

.buttons-group {
  align-self: flex-end;
  display: flex;
  gap: 10px;
}

.buttons-group button {
  font-size: 16px;
  border-color: forestgreen;
  border-radius: 5px;
  background-color: white;
}

.buttons-group button:hover {
  font-weight: bold;
}
</style>
