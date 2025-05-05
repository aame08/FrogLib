<script setup>
import { ref, computed } from 'vue';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';

import LoginModal from './modals/LoginModal.vue';
import PasswordRecovery from './modals/PasswordRecoveryModal.vue';
import RegisterModal from './modals/RegisterModal.vue';

const store = useStore();
const router = useRouter();
const isAuthenticated = computed(() => store.getters['auth/isAuthenticated']);
const user = computed(() => store.getters['auth/user']);
const userRole = computed(() => user.value?.nameRole || null);

const showLoginModal = ref(false);
const showRegisterModal = ref(false);
const showPasswordModal = ref(false);
const showAccountMenu = ref(false);

const openLoginModal = () => (showLoginModal.value = true);

const openPasswordRecoveryModal = () => (showPasswordModal.value = true);

const openRegisterModal = () => (showRegisterModal.value = true);

const closeModal = () => {
  showLoginModal.value = false;
  showPasswordModal.value = false;
  showRegisterModal.value = false;
};

const toggleAccountMenu = () => {
  showAccountMenu.value = !showAccountMenu.value;
};

const closeAccountMenu = () => {
  showAccountMenu.value = false;
};

const logout = () => {
  store.dispatch('auth/logout');
  router.push('/');
  console.log('Аутентификация пользователя:', isAuthenticated.value);
};

const goToProfile = () => {
  router.push('/profile');
  closeAccountMenu();
};
const goToSettings = () => {
  router.push('/settings');
  closeAccountMenu();
};
</script>

<template>
  <LoginModal
    :isVisible="showLoginModal"
    @close="closeModal"
    @openRegister="openRegisterModal"
    @openPasswordRecovery="openPasswordRecoveryModal"
  />
  <PasswordRecovery :isVisible="showPasswordModal" @close="closeModal" />
  <RegisterModal :isVisible="showRegisterModal" @close="closeModal" />

  <div class="header">
    <div class="name-site">
      <img src="@/assets/frog.png" style="height: 50px" alt="logo" />
      <p>FrogLib</p>
    </div>
    <div class="nav-buttons">
      <template v-if="!isAuthenticated || userRole === 'Пользователь'">
        <RouterLink to="/" class="nav-link">Главная</RouterLink>
        <RouterLink to="/books" class="nav-link">Книги</RouterLink>
        <RouterLink to="/collections" class="nav-link">Подборки</RouterLink>
        <RouterLink to="/reviews" class="nav-link">Рецензии</RouterLink>
      </template>
      <template v-else-if="userRole === 'Администратор'">
        <RouterLink to="/admin/management" class="nav-link"
          >Управление контентом</RouterLink
        >
        <RouterLink to="/admin/collections" class="nav-link"
          >Подборки</RouterLink
        >
        <RouterLink to="/admin/users" class="nav-link">Пользователи</RouterLink>
      </template>
      <template v-else-if="userRole === 'Модератор'">
        <RouterLink to="/moder/management" class="nav-link"
          >Подборки и рецензии</RouterLink
        >
        <RouterLink to="/moder/comments" class="nav-link"
          >Комментарии</RouterLink
        >
        <RouterLink to="/moder/users" class="nav-link">Пользователи</RouterLink>
      </template>
    </div>
    <div class="nav-buttons">
      <template v-if="!isAuthenticated">
        <button
          class="nav-button"
          @click="openLoginModal"
          @refresh-data="$emit('refresh-data')"
        >
          Вход
        </button>
        <button class="nav-button" @click="openRegisterModal">
          Регистрация
        </button>
      </template>
      <template v-else>
        <div class="account-menu-container">
          <div>{{ user?.nameUser }}</div>
          <button class="button-account" @click="toggleAccountMenu">
            <img
              v-if="user?.profileImageUrl"
              :src="`https://localhost:7157${user?.profileImageUrl}`"
              style="height: 50px; width: 50px; border-radius: 50%"
              alt="User Image"
            />
            <img
              v-else
              src="@/assets/user_photo.png"
              style="height: 50px; width: 50px; border-radius: 50%"
              alt="user image"
            />
          </button>
          <div v-if="showAccountMenu" class="account-menu">
            <button v-if="userRole === 'Пользователь'" @click="goToProfile">
              Аккаунт
            </button>
            <button @click="goToSettings">Настройки</button>
            <button @click="logout">Выход</button>
          </div>
        </div>
      </template>
    </div>
  </div>
</template>

<style scoped>
.header {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  z-index: 1000;
  display: flex;
  justify-content: space-between;
  align-items: center;
  background-color: white;
  border-bottom: 2px solid forestgreen;
}

.name-site {
  display: flex;
  align-items: center;
  gap: 5px;
}

.name-site p {
  color: forestgreen;
  font-size: 18px;
  font-weight: bold;
  letter-spacing: 3px;
}

.nav-buttons {
  display: flex;
  gap: 5px;
  justify-content: center;
  height: 45px;
  align-items: center;
}

.nav-button, .nav-link {
  border: none;
  background: none;
  font-size: 16px;
  color: black;
  padding: 10px 10px;
}

.nav-button:hover, .nav-link:hover {
  border-bottom: 2px solid darkgreen;
  font-weight: bold;
  box-sizing: border-box;
}

.nav-link.router-link-exact-active {
  border-bottom: 2px solid darkgreen;
  font-weight: bold;
}

.button-account {
  background: none;
  border: none;
}

.account-menu-container {
  position: relative;
  display: flex;
  align-items: center;
}

.account-menu {
  position: absolute;
  top: 100%;
  right: 0;
  z-index: 1001;
  min-width: 150px;
  background-color: white;
  border: 1px solid lightgrey;
  border-radius: 5px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

.account-menu button {
  display: block;
  width: 100%;
  padding: 8px 16px;
  background: none;
  border: none;
  text-align: left;
}

.account-menu button:hover {
  background-color: lightgrey;
  font-weight: bold;
}
</style>
