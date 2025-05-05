<script setup>
import CollectionCard from '../cards/CollectionCard.vue';

const props = defineProps({
  countCollections: { type: Number, required: true },
  collections: { type: Array, required: true },
});
</script>

<template>
  <h3 v-if="collections.length === 0">Здесь пока нет подборок.</h3>
  <h3 v-else>
    Всего: <span>{{ countCollections }}</span>
  </h3>
  <div class="collections-container">
    <CollectionCard
      v-for="(collection, index) in collections"
      :key="index"
      :id="collection.id"
      :title="collection.title"
      :description="collection.description"
      :rating="collection.rating"
      :countBooks="collection.countBooks"
      :books="collection.books"
      :countView="collection.countView"
      :countComments="collection.countComments"
      :countLiked="collection.countLiked"
      :createdDate="collection.createdDate"
      :userName="collection.userName"
      :userURL="collection.userURL"
    />
  </div>
</template>

<style scoped>
h3 {
  text-align: end;
}

.collections-container {
  display: flex;
  justify-content: center;
  flex-wrap: wrap;
}
</style>
