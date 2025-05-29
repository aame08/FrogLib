<script setup>
import { ref, computed } from 'vue';
import { useStore } from 'vuex';
import adminService from '@/services/adminService';

import StaffTable from '@/components/adminComponents/StaffTable.vue';
import AddUser from '@/components/adminComponents/AddUser.vue';

const store = useStore();
const user = computed(() => store.getters['auth/user']);

const staff = ref([]);
const showAddForm = ref(false);

const getStaff = async () => {
  try {
    const response = await adminService.getStaff(user.value?.idUser);
    staff.value = response;
  } catch (error) {
    console.error('Ошибка при загрузке сотрудников:', error);
  }
};
getStaff();

const openAddForm = () => {
  showAddForm.value = true;
};

const closeAddForm = () => {
  showAddForm.value = false;
};
</script>

<template>
  <div v-if="!showAddForm">
    <h1>Управление пользователями</h1>
    <div class="add-button">
      <button @click="openAddForm">Добавить сотрудника</button>
    </div>
    <StaffTable :staff="staff" @refresh-data="getStaff" />
  </div>
  <AddUser
    v-if="showAddForm"
    @refresh-data="getStaff"
    :closeForm="closeAddForm"
  />
</template>

<style scoped>
h1 {
  text-align: center;
  font-size: 24px;
}

.add-button {
  display: flex;
  justify-content: flex-start;
  margin-top: 10px;
}

.add-button button {
  padding: 10px 20px;
  font-size: 14px;
  color: white;
  background-color: forestgreen;
  border: none;
  border-radius: 5px;
}

.add-button button:hover {
  background-color: darkgreen;
}
</style>
