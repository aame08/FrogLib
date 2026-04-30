<script setup>
const props = defineProps({
  categories: { type: Array, required: true },
});

const emit = defineEmits(['edit-category']);

const handleEdit = (category) => {
  emit('edit-category', category);
};
</script>

<template>
  <table>
    <thead>
      <tr>
        <th>ID</th>
        <th>Название</th>
        <th>Количество книг</th>
        <th>Действие</th>
      </tr>
    </thead>
    <tbody>
      <tr v-for="category in categories" :key="category.idCategory">
        <td>{{ category.idCategory }}</td>
        <td>{{ category.nameCategory }}</td>
        <td>{{ category.countBooks }}</td>
        <td>
          <button class="action-button" @click="handleEdit(category)">
            Подробнее
          </button>
        </td>
      </tr>
    </tbody>
  </table>
</template>

<style scoped></style>
