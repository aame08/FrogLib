<script setup>
import moderService from '@/services/moderService';

const props = defineProps({
  user: { type: Object, required: true },
  violations: { type: Array, required: true },
  closeForm: { type: Function },
});

const emit = defineEmits(['refresh-data']);

const changeStatusUser = async (status) => {
  try {
    await moderService.changeStatusUser(props.user.idUser, status);
    console.log('Статус изменен.');
    props.closeForm();
    emit('refresh-data');
  } catch (error) {
    console.error('Ошибка при изменении статуса пользователя:', error);
  }
};
</script>

<template>
  <main>
    <h1>Просмотр нарушений пользователя</h1>
    <div class="container">
      <div class="user-info">
        <h2>
          Нарушения пользователя:
          <span style="font-weight: normal">{{ user.nameUser }}</span>
        </h2>
        <div class="user-details">
          <p><strong>Email: </strong>{{ user.loginUser }}</p>
          <p><strong>Статус: </strong>{{ user.statusUser }}</p>
          <p><strong>Всего нарушений: </strong>{{ user.countViolations }}</p>
        </div>
      </div>
      <div class="violation-list">
        <div
          v-for="violation in violations"
          :key="violation.idViolation"
          class="violation-item"
        >
          <div class="violation-header">
            <span class="violation-category">{{
              violation.categoryViolation
            }}</span>
            <span class="violation-date">{{
              new Date(violation.dateViolation).toLocaleDateString()
            }}</span>
          </div>
          <div class="violation-description">
            {{ violation.descriptionViolation }}
          </div>
        </div>
      </div>
      <div class="buttons-container">
        <button class="button red" @click="closeForm">Отмена</button>
        <button
          v-if="user.statusUser === 'Активен'"
          class="button red"
          @click="changeStatusUser('Заблокирован')"
        >
          Заблокировать
        </button>
        <button
          v-if="user.statusUser === 'Заблокирован'"
          class="button"
          @click="changeStatusUser('Активен')"
        >
          Разблокировать
        </button>
      </div>
    </div>
  </main>
</template>

<style scoped>
main {
  max-width: 1200px;
  margin-left: auto;
  margin-right: auto;
  padding: 20px;
  border-radius: 5px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

h1 {
  text-align: center;
  font-size: 28px;
  margin-bottom: 50px;
  text-decoration: underline;
  text-decoration-color: forestgreen;
}

.container {
  display: flex;
  flex-direction: column;
  padding: 20px;
  margin-top: -30px;
  background-color: white;
  border: 1px solid forestgreen;
  border-radius: 5px;
}

.user-details {
  display: flex;
  gap: 10px;
  margin-top: -30px;
}

.user-details p {
  padding-right: 5px;
  border-right: 1px solid grey;
}

.violation-list {
  margin-top: 15px;
}

.violation-item {
  padding: 15px;
  margin-bottom: 15px;
  border: 1px solid lightgrey;
  border-radius: 5px;
}

.violation-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 10px;
  margin-bottom: 10px;
}

.violation-category {
  padding: 4px 8px;
  font-size: 14px;
  border-radius: 5px;
  color: crimson;
  background-color: whitesmoke;
}

.violation-date {
  font-size: 14px;
  color: grey;
}

.violation-description {
  padding: 10px;
  border-radius: 5px;
}

.buttons-container {
  display: flex;
  justify-content: center;
  gap: 15px;
}

.button {
  padding: 10px 20px;
  color: white;
  border: none;
  border-radius: 5px;
  background-color: forestgreen;
}

.button:hover {
  background-color: darkgreen;
}

.button.red {
  background-color: crimson;
}

.button.red:hover {
  background-color: darkred;
}
</style>
