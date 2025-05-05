<script setup>
const props = defineProps({
  authors: { type: Array, required: true },
});

const emit = defineEmits(['edit-author']);

const handleEdit = (author) => {
  emit('edit-author', author);
};
</script>

<template>
  <table>
    <thead>
      <tr>
        <th>ID</th>
        <th>Фамилия</th>
        <th>Имя</th>
        <th>Отчество</th>
        <th>Количество книг</th>
        <th>Действие</th>
      </tr>
    </thead>
    <tbody>
      <tr v-for="author in authors" :key="author.idAuthor">
        <td>{{ author.idAuthor }}</td>
        <td>{{ author.surnameAuthor }}</td>
        <td>{{ author.nameAuthor }}</td>
        <td>{{ author.patronymicAuthor }}</td>
        <td>{{ author.countBooks }}</td>
        <td>
          <button class="action-button" @click="handleEdit(author)">
            Подробнее
          </button>
        </td>
      </tr>
    </tbody>
  </table>
</template>

<style scoped></style>
