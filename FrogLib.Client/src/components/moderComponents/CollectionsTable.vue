<script setup>
const props = defineProps({
  collections: { type: Array, required: true },
});

const emit = defineEmits(['collection-view']);

const handleView = (collection) => {
  emit('collection-view', collection);
};
</script>

<template>
  <table>
    <thead>
      <tr>
        <th>ID</th>
        <th>Имя пользователя</th>
        <th>Заголовок подборки</th>
        <th>Количество книг</th>
        <th>Действие</th>
      </tr>
    </thead>
    <tbody>
      <tr
        v-for="collection in collections"
        :key="collection.idCollection"
        :class="{
          'violation-row':
            collection.statusCollection === 'Обнаружено нарушение',
        }"
      >
        <td>{{ collection.idCollection }}</td>
        <td>{{ collection.author.name }}</td>
        <td>{{ collection.titleCollection }}</td>
        <td>{{ collection.countBooks }}</td>
        <td>
          <button class="action-button" @click="handleView(collection)">
            Подробнее
          </button>
        </td>
      </tr>
    </tbody>
  </table>
</template>

<style scoped></style>
