<script setup>
import { defineProps, ref } from 'vue';
import authService from '@/services/authService';

const props = defineProps({
  isVisible: Boolean,
});

const emit = defineEmits(['close']);

const email = ref('');
const message = ref('');
const errors = ref({});

const clearFields = () => {
  email.value = '';
  message.value = '';
  errors.value = {};
};

const handleClose = () => {
  clearFields();
  emit('close');
};

const handleResetPassword = async () => {
  errors.value = {};

  if (email.value === '') {
    errors.value.email = 'Введите электронную почту.';
    return;
  }

  try {
    const response = await authService.resetPassword(email.value);
    message.value =
      response.data.message || 'Новый пароль отправлен на электронную почту.';
    // emit('close');
  } catch (error) {
    if (error.response) {
      if (error.response.data?.errors) {
        errors.value = error.response.data.errors;
      } else {
        message.value =
          error.response.data?.message || 'Произошла ошибка при сбросе пароля';
      }
    } else {
      message.value = 'Не удалось подключиться к серверу';
    }
    console.error('Ошибка сброса пароля:', error);
  }
};
</script>

<template>
  <div
    class="modal-overlay"
    v-if="isVisible"
    @keyup.esc="handleClose"
    tabindex="0"
  >
    <div class="modal-content">
      <button class="button-close" @click="handleClose">✕</button>
      <div class="modal-header">
        <img src="/frog.ico" alt="FrogLib" style="height: 80px" />
        <div>FrogLib</div>
      </div>
      <div class="modal-container">
        <div class="text-container">
          <div>Восстановление пароля</div>
          <div>Введите электронную почту для восстановления пароля.</div>
        </div>
        <div v-if="message" class="message">{{ message }}</div>
        <form class="input-container">
          <div class="inner-container">
            <div>Электронная почта</div>
            <input
              type="email"
              placeholder="username@example.com"
              v-model="email"
              :class="{ 'input-error': errors.email }"
            />
            <div v-if="errors.email" class="error-message">
              {{ errors.email }}
            </div>
          </div>
        </form>
        <button class="send-button" @click="handleResetPassword">
          Отправить
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  right: 0;
  bottom: 0;
  left: 0;
  z-index: 2000;
  display: flex;
  justify-content: center;
  align-items: center;
  background: rgba(0, 0, 0, 0.5);
}

.modal-content {
  height: 400px;
  width: 550px;
  display: flex;
  flex-direction: column;
  border-radius: 25px;
  background-color: white;
}

.button-close {
  align-self: flex-end;
  height: 20px;
  width: 20px;
  margin-right: 15px;
  font-size: 18px;
  background: none;
  border: none;
  color: black;
}
.button-close:hover {
  color: darkgreen;
}

.modal-header {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 3px;
}

.modal-header div {
  font-size: 20px;
  font-weight: bold;
  letter-spacing: 3px;
}

.modal-container {
  display: flex;
  flex-direction: column;
  align-self: center;
  margin-top: 30px;
  gap: 15px;
}

.text-container {
  text-align: center;
  border-bottom: 2px solid darkgreen;
}

.input-container {
  align-self: flex-start;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 10px;
}

.inner-container div {
  font-weight: bold;
  font-size: 18px;
}

.inner-container input {
  border-radius: 5px;
  height: 30px;
  width: 400px;
}

.send-button {
  height: 35px;
  width: 200px;
  border: none;
  border-radius: 5px;
  align-self: center;
  font-size: 16px;
  background-color: forestgreen;
  color: white;
}

.error-message {
  color: crimson;
  font-size: 10px;
}

.input-error {
  border: 2px solid darkred;
}

.message {
  color: grey;
  text-align: center;
}
</style>
