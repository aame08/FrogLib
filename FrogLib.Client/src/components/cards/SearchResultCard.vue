<script setup>
const props = defineProps({
  imageURL: { type: String, required: true },
  title: { type: String, required: true },
  authors: { type: String, required: true },
  yearPublication: { type: String, required: true },
});

const emit = defineEmits(['select']);

const handleSelect = () => {
  emit('select');
};
</script>

<template>
  <div class="card">
    <img :src="imageURL" :alt="title" />
    <div class="book-info">
      <div>
        Название: <span>{{ title }}</span>
      </div>
      <div>
        Автор(ы): <span>{{ authors }}</span>
      </div>
      <div>
        Год издания: <span>{{ yearPublication }}</span>
      </div>
    </div>
    <button class="button" type="button" @click="handleSelect">Выбрать</button>
  </div>
</template>

<style scoped>
.card {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 10px;
  margin-bottom: 10px;
  border: 1px solid whitesmoke;
  border-radius: 5px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

.card img {
  height: 125px;
}

.book-info {
  display: flex;
  flex-direction: column;
  gap: 5px;
}

div {
  font-weight: bold;
}

div span {
  font-weight: normal;
}

button {
  padding: 5px 10px;
}

.button {
  padding: 10px 20px;
  color: white;
  background-color: forestgreen;
  border: none;
  border-radius: 5px;
}
</style>
