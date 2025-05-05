<script setup>
const props = defineProps({
  users: { type: Array, required: true },
  sortDirection: { type: String, required: true },
});

const emit = defineEmits(['show-violations', 'toggle-sort']);

const handleShowViolations = (idUser) => {
  emit('show-violations', idUser);
};

const handleSortClick = () => {
  emit('toggle-sort');
};
</script>

<template>
  <table>
    <thead>
      <th>ID</th>
      <th>Имя пользователя</th>
      <th>Эл. почта</th>
      <th>Статус</th>
      <th @click="handleSortClick" style="cursor: pointer">
        Количество нарушений
        <span v-if="sortDirection === 'asc'">↑</span>
        <span v-else>↓</span>
      </th>
      <th>Действие</th>
    </thead>
    <tbody>
      <tr v-for="user in users" :key="user.idUser">
        <td>{{ user.idUser }}</td>
        <td>{{ user.nameUser }}</td>
        <td>{{ user.loginUser }}</td>
        <td>{{ user.statusUser }}</td>
        <td>{{ user.countViolations }}</td>
        <td>
          <button
            class="action-button"
            @click="handleShowViolations(user.idUser)"
          >
            Подробнее
          </button>
        </td>
      </tr>
    </tbody>
  </table>
</template>

<style scoped></style>
