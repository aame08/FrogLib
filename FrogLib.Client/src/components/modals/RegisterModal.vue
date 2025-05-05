<script setup>
import { defineProps, ref } from 'vue';

import authService from '@/services/authService';

const props = defineProps({
  isVisible: Boolean,
});

const emit = defineEmits(['close']);

const username = ref('');
const email = ref('');
const password = ref('');
const confirmPassword = ref('');
const profilePicture = ref(null);
const message = ref('');
const errors = ref({});

const handleFileChange = (event) => {
  const file = event.target.files[0];
  if (file) {
    profilePicture.value = file;
  }
};

const registerUser = async () => {
  errors.value = {};

  let isValid = true;

  if (password.value != confirmPassword.value) {
    errors.value.confirmPassword = 'Пароли не совпадают.';
    isValid = false;
  }
  if (username.value === '') {
    errors.value.username = 'Имя пользователя не может быть пустым.';
    isValid = false;
  }
  if (email.value === '') {
    errors.value.email = 'Электронная почта не может быть пустой.';
    isValid = false;
  }
  if (password.value === '') {
    errors.value.password = 'Пароль не может быть пустым.';
    isValid = false;
  }

  if (password.value.length <= 8) {
    errors.value.password = 'Пароль должен состоять от 8 символов.';
    isValid = false;
  }

  if (!isValid) {
    return;
  }

  const formData = new FormData();
  formData.append('nameUser', username.value);
  formData.append('loginUser', email.value);
  formData.append('passwordHash', password.value);
  formData.append('confirmPassword', confirmPassword.value);
  if (profilePicture.value) {
    formData.append('profileImageUrl', profilePicture.value);
  }

  try {
    const response = await authService.register(formData);

    if (response.status === 201) {
      message.value = 'Регистрация прошла успешно.';

      // emit('close');

      username.value = '';
      email.value = '';
      password.value = '';
      confirmPassword.value = '';
      profilePicture.value = null;
    }
  } catch (error) {
    if (
      error.response &&
      error.response.status === 400 &&
      error.response.data.errors
    ) {
      const serverErrors = error.response.data.errors;
      if (serverErrors.NameUser) {
        errors.value.username = serverErrors.NameUser[0];
      }
      if (serverErrors.LoginUser) {
        errors.value.email = serverErrors.LoginUser[0];
      }
      if (serverErrors.ConfirmPassword) {
        errors.value.confirmPassword = serverErrors.ConfirmPassword[0];
      }
    } else {
      message.value = 'Ошибка регистрации.';
    }
  }
};
</script>

<template>
  <div class="modal-overlay" v-if="isVisible">
    <div class="modal-content">
      <button class="button-close" @click="$emit('close')">✕</button>
      <div class="modal-header">
        <img src="/frog.ico" alt="FrogLib" style="height: 80px" />
        <div>FrogLib</div>
      </div>
      <div class="modal-container">
        <div class="text-container">
          <div>Добро пожаловать!</div>
          <div>Зарегистрируйтесь, чтобы получить все функции форума.</div>
        </div>
        <div v-if="message" class="message">{{ message }}</div>
        <form class="input-container">
          <div class="inner-container">
            <div>Имя пользователя</div>
            <input
              type="text"
              placeholder="username"
              required
              v-model="username"
              :class="{ 'input-error': errors.username }"
            />
            <div v-if="errors.username" class="error-message">
              {{ errors.username }}
            </div>
          </div>
          <div class="inner-container">
            <div>Электронная почта</div>
            <input
              type="email"
              placeholder="username@example.com"
              required
              v-model="email"
              :class="{ 'input-error': errors.email }"
            />
            <div v-if="errors.email" class="error-message">
              {{ errors.email }}
            </div>
          </div>
          <div class="inner-container">
            <div>Пароль</div>
            <input
              type="password"
              placeholder="*****"
              required
              v-model="password"
              :class="{ 'input-error': errors.password }"
            />
            <div v-if="errors.password" class="error-message">
              {{ errors.password }}
            </div>
          </div>
          <div class="inner-container">
            <div>Подтверждение пароля</div>
            <input
              type="password"
              placeholder="*****"
              required
              v-model="confirmPassword"
              :class="{ 'input-error': errors.confirmPassword }"
            />
            <div v-if="errors.confirmPassword" class="error-message">
              {{ errors.confirmPassword }}
            </div>
          </div>
          <div class="inner-container">
            <div>Изображение пользователя</div>
            <input
              type="file"
              accept="image/*"
              class="input-image"
              @change="handleFileChange"
            />
            <div v-if="errors.profilePicture" class="error-message">
              {{ errors.profilePicture }}
            </div>
          </div>
        </form>
        <button class="register-button" @click="registerUser">
          Зарегистрироваться
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
  height: 650px;
  width: 550px;
  display: flex;
  flex-direction: column;
  border-radius: 25px;
  background-color: white;
  overflow: auto;
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

.input-image {
  font-size: 14px;
}
.input-image::file-selector-button {
  background: none;
  border: 1px solid forestgreen;
  border-radius: 5px;
  font-size: 14px;
}

.register-button {
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
