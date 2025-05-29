<script setup>
const props = defineProps({
  collection: { type: Object, required: true },
});

const emit = defineEmits(['open-collection']);

const handleClick = () => {
  emit('open-collection', props.collection.idCollection);
};
</script>

<template>
  <div class="collection-card">
    <div
      v-if="collection.statusCollection"
      class="collection-status"
      :class="{
        approved: collection.statusCollection === 'Одобрено',
        rejected: collection.statusCollection === 'Отказано',
        pending: collection.statusCollection === 'На рассмотрении',
        violation: collection.statusCollection === 'Обнаружено нарушение',
      }"
    >
      <span v-if="collection.statusCollection === 'Одобрено'">✓</span>
      <span v-else-if="collection.statusCollection === 'Отказано'">×</span>
      <span v-else-if="collection.statusCollection === 'На рассмотрении'"
        >🕐</span
      >
      <span v-else-if="collection.statusCollection === 'Обнаружено нарушение'"
        >⚠</span
      >
      {{ collection.statusCollection }}
    </div>
    <button v-if="collection.statusCollection" @click="handleClick">
      {{ collection.titleCollection }}
    </button>
    <RouterLink v-else :to="`/collections/${collection.idCollection}`">{{
      collection.titleCollection
    }}</RouterLink>
    <div class="images-container">
      <img
        v-for="(book, index) in collection.books"
        :key="book.id || index"
        :src="book.imageURL"
        :alt="book.title"
      />
    </div>
  </div>
</template>

<style scoped>
.collection-card {
  width: 500px;
  box-sizing: border-box;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 10px;
  padding: 5px;
  background-color: white;
  border-radius: 5px;
  border-bottom: 2px solid forestgreen;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

.collection-status {
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

.violation {
  background-color: gold;
}

.images-container {
  display: flex;
  justify-content: center;
  align-items: center;
  padding: 5px;
}

.images-container img {
  min-width: 50px;
  max-width: 150px;
  min-height: 100px;
  max-height: 200px;
}

.collection-card button {
  background: none;
  border: none;
}

.collection-card button:hover {
  color: darkgreen;
}

.collection-card a:hover {
  color: darkgreen;
}
</style>
