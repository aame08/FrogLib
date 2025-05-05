import './assets/style.css';
import { createApp } from 'vue';
import App from './App.vue';
import router from './router';
import store from './store';

const app = createApp(App);

// Подключаем хранилище и роутер до инициализации
app.use(store);
app.use(router);

const initializeApp = async () => {
  if (store.state.auth.refreshToken) {
    try {
      await store.dispatch('auth/refreshToken');
    } catch (error) {
      console.error('Ошибка при обновлении токена:', error);
      store.dispatch('auth/logout'); // Теперь это сработает
    }
  }

  app.mount('#app');
};

initializeApp();