<script setup>
import { defineProps, ref } from 'vue';
import { useStore } from 'vuex';
import { useRoute } from 'vue-router';
import { useRouter } from 'vue-router';
import authService from '@/services/authService';
import userActivityService from '@/services/userActivityService';

const props = defineProps({
  isVisible: Boolean,
});

const emit = defineEmits([
  'close',
  'openPasswordRecovery',
  'openRegister',
  'refresh-data',
]);

const email = ref('');
const password = ref('');
const message = ref('');
const errors = ref({});

const store = useStore();
const route = useRoute();
const router = useRouter();

const openPasswordRecovery = () => {
  emit('close');
  emit('openPasswordRecovery');
};

const openRegister = () => {
  emit('close');
  emit('openRegister');
};

const login = async () => {
  errors.value = {};

  let isValid = true;

  if (email.value === '') {
    errors.value.email = 'Электронная почта не может быть пустой.';
    isValid = false;
  }
  if (password.value === '') {
    errors.value.password = 'Пароль не может быть пустым.';
    isValid = false;
  }

  if (!isValid) {
    return;
  }

  try {
    const response = await authService.login(email.value, password.value);

    store.commit('auth/setTokens', {
      accessToken: response.data.accessToken,
      refreshToken: response.data.refreshToken,
    });

    store.commit('auth/setUser', response.data.user);

    const userRole = response.data.user.nameRole;

    message.value = 'С возвращением.';
    // emit('close');

    email.value = '';
    password.value = '';
    message.value = '';

    if (userRole === 'Пользователь') {
      const idUser = response.data.user.idUser;
      const idEntity = route.params.id ? parseInt(route.params.id) : null;
      const typeEntity = route.meta.entityType || null;

      if (idEntity && typeEntity) {
        try {
          await userActivityService.addView(idUser, idEntity, typeEntity);
          console.log('Просмотр добавлен.');
          emit('refresh-data');
        } catch (error) {
          console.error('Ошибка при добавлении просмотра:', error);
        }
      }
    } else if (userRole === 'Администратор') {
      router.push('/admin/management');
      return;
    } else if (userRole === 'Модератор') {
      router.push('/moder/management');
      return;
    }
  } catch (error) {
    if (
      error.response &&
      error.response.status === 400 &&
      error.response.data.errors
    ) {
      const serverErrors = error.response.data.errors;
      if (serverErrors.Unauthorized) {
        errors.value.email = serverErrors.Unauthorized[0];
      }
      if (serverErrors.InvalidPassword) {
        errors.value.password = serverErrors.InvalidPassword[0];
      }
      if (serverErrors.BannedUser) {
        errors.value.ban = serverErrors.BannedUser[0];
      }
    } else {
      message.value = 'Ошибка авторизации.';
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
          <div>С возвращением!</div>
          <div>Войдите в систему, чтобы получить все функции форума.</div>
        </div>
        <div v-if="message" class="message">{{ message }}</div>
        <form class="input-container">
          <div class="inner-container">
            <div>Электронная почта</div>
            <input
              type="email"
              placeholder="username@example.com"
              required
              v-model="email"
              :class="{ 'input-error': errors.login }"
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
          <div v-if="errors.ban" class="error-message">{{ errors.ban }}</div>
        </form>
        <button class="transparent-button" @click="openPasswordRecovery">
          Забыли пароль?
        </button>
        <button class="login-button" @click="login">Войти</button>
        <div class="new-user-container">
          <div>Новый пользователь?</div>
          <button class="transparent-button" @click="openRegister">
            Зарегистрироваться
          </button>
        </div>
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
  height: 550px;
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

.transparent-button {
  align-self: flex-end;
  display: flex;
  align-items: center;
  font-size: 14px;
  background: none;
  border: none;
  color: black;
}
.transparent-button:hover {
  color: darkgreen;
}

.login-button {
  height: 35px;
  width: 200px;
  border: none;
  border-radius: 5px;
  align-self: center;
  font-size: 16px;
  background-color: forestgreen;
  color: white;
}

.new-user-container {
  margin-top: 10px;
  display: flex;
  gap: 5px;
  align-items: center;
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
