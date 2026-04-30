<script setup>
import { truncateText } from '@/utils/truncateText';

const props = defineProps({
  collections: { type: Array, required: true },
});

const emit = defineEmits(['edit-collection']);

const handleEditClick = (collectionId) => {
  emit('edit-collection', collectionId);
};

const truncatedDescription = (text) => {
  return truncateText(text, 50);
};
</script>

<template>
  <table>
    <thead>
      <tr>
        <th>ID</th>
        <th>Название</th>
        <th>Описание</th>
        <th>Количество книг</th>
        <th>Действие</th>
      </tr>
    </thead>
    <tbody>
      <tr v-for="collection in collections" :key="collection.idCollection">
        <td>{{ collection.idCollection }}</td>
        <td>{{ collection.titleCollection }}</td>
        <td
          v-html="truncatedDescription(collection.descriptionCollection)"
        ></td>
        <td>{{ collection.countBooks }}</td>
        <td>
          <button
            class="action-button"
            @click="handleEditClick(collection.idCollection)"
          >
            Редактировать
          </button>
        </td>
      </tr>
    </tbody>
  </table>
</template>

<style scoped></style>
