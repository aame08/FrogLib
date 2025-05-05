import { createStore } from 'vuex';
import auth from './auth';

const store = createStore({
  modules: {
    auth, // Регистрируем модуль auth
  },
});

export default store;