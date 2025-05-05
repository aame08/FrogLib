<script setup>
const props = defineProps({
  books: { type: Array, required: true },
});

const emit = defineEmits(['edit-book']);

const formatAuthors = (authors) => {
  return authors
    .map((a) => {
      let name = `${a.surnameAuthor} ${a.nameAuthor}`;
      if (a.patronymicAuthor) name += ` ${a.patronymicAuthor}`;
      return name;
    })
    .join(', ');
};

const handleEdit = (book) => {
  emit('edit-book', book);
};
</script>

<template>
  <table>
    <thead>
      <tr>
        <th>ISBN-10</th>
        <th>ISBN-13</th>
        <th>Название</th>
        <th>Автор(ы)</th>
        <th>Категория</th>
        <th>Статус</th>
        <th>Действие</th>
      </tr>
    </thead>
    <tbody>
      <tr v-for="book in books" :key="book.id">
        <td>{{ book.isbn10 }}</td>
        <td>{{ book.isbn13 }}</td>
        <td>{{ book.titleBook }}</td>
        <td>{{ formatAuthors(book.authors) }}</td>
        <td>{{ book.categoryName }}</td>
        <td>{{ book.statusBook }}</td>
        <td style="text-align: center">
          <button class="action-button" @click="handleEdit(book)">
            Редактировать
          </button>
        </td>
      </tr>
    </tbody>
  </table>
</template>

<style scoped></style>
