<script setup>
import { ref, computed } from 'vue';
import moderService from '@/services/moderService';

import UsersTable from '@/components/moderComponents/UsersTable.vue';
import ViolationsView from '@/components/moderComponents/ViolationsView.vue';

const allUsers = ref([]);
const selectedUser = ref(null);
const violations = ref(null);
const showViolationsForm = ref(false);
const sortDirection = ref('desc');

const getUsers = async () => {
  try {
    const response = await moderService.getModerUsers();
    allUsers.value = response;
  } catch (error) {
    console.error('Ошибка при загрузке пользователей:', error);
  }
};
getUsers();

const openViolationsForm = async (idUser) => {
  try {
    const response = await moderService.getUserViolations(idUser);
    selectedUser.value = response.user;
    violations.value = response.violations;
    showViolationsForm.value = true;
  } catch (error) {
    console.error('Ошибка при загрузке нарушений:', error);
  }
};

const filteredUser = computed(() => {
  return [...allUsers.value].sort((a, b) => {
    const countA = a.countViolations || 0;
    const countB = b.countViolations || 0;
    return sortDirection.value === 'asc' ? countA - countB : countB - countA;
  });
});

const toggleSort = () => {
  sortDirection.value = sortDirection.value === 'asc' ? 'desc' : 'asc';
};

const closeViolationsForm = () => {
  selectedUser.value = null;
  violations.value = [];
  showViolationsForm.value = false;
};
</script>

<template>
  <div v-if="!showViolationsForm">
    <h1>Модерация пользователей</h1>
    <UsersTable
      :users="filteredUser"
      :sort-direction="sortDirection"
      @show-violations="openViolationsForm"
      @toggle-sort="toggleSort"
    />
  </div>
  <ViolationsView
    v-if="showViolationsForm"
    :user="selectedUser"
    :violations="violations"
    :closeForm="closeViolationsForm"
    @refresh-data="getUsers"
  />
</template>

<style scoped>
h1 {
  text-align: center;
  font-size: 24px;
}
</style>
