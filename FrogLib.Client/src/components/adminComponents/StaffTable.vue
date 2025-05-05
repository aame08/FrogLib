<script setup>
import adminService from '@/services/adminService';

const props = defineProps({
  staff: { type: Array, required: true },
});

const emit = defineEmits(['refresh-data']);

const roles = ['Администратор', 'Модератор'];

const saveRole = async (employee) => {
  try {
    await adminService.adminChangeRole(employee.idUser, employee.nameRole);
    console.log('Роль пользователя изменена.');
  } catch (error) {
    console.error('Ошибка при изменении роли:', error);
  }
};

const deleteEmployee = async (employee) => {
  try {
    await adminService.adminDeleteEmployee(employee.idUser);
    console.log('Пользователь отстранен.');
    emit('refresh-data');
  } catch (error) {
    console.error('Ошибка при отстранении пользователя:', error);
  }
};
</script>

<template>
  <table>
    <thead>
      <tr>
        <th>ID</th>
        <th>Имя</th>
        <th>Электронная почта</th>
        <th>Роль</th>
        <th>Действия</th>
      </tr>
    </thead>
    <tbody>
      <tr v-for="employee in staff" :key="employee.idUser">
        <td>{{ employee.idUser }}</td>
        <td>{{ employee.nameUser }}</td>
        <td>{{ employee.loginUser }}</td>
        <td>
          <select v-model="employee.nameRole">
            <option v-for="role in roles" :key="role" :value="role">
              {{ role }}
            </option>
          </select>
        </td>
        <td>
          <div class="action-buttons">
            <button class="action-button" @click="saveRole(employee)">
              Сохранить
            </button>
            <button
              class="action-button delete"
              @click="deleteEmployee(employee)"
            >
              Отстранить
            </button>
          </div>
        </td>
      </tr>
    </tbody>
  </table>
</template>

<style scoped>
.action-buttons {
  display: flex;
  justify-content: center;
  gap: 10px;
}

td select {
  height: auto;
  width: 90%;
  padding: 10px;
  font-size: 14px;
  border: 1px solid lightgrey;
  border-radius: 5px;
}
</style>
