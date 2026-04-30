<script setup>
import { ref } from 'vue';
import adminService from '@/services/adminService';

const props = defineProps({
  closeForm: { type: Function },
});

const emit = defineEmits(['refresh-data']);

const formData = ref({
  name: '',
  email: '',
  password: '',
  confirmPassword: '',
  role: 'Модератор',
});
const errors = ref({});

const handleSubmit = async () => {
  errors.value = {};

  let isValid = true;

  if (formData.value.password !== formData.value.confirmPassword) {
    errors.value.confirmPassword = 'Пароли не совпадают.';
    isValid = false;
  }
  if (formData.value.name === '') {
    errors.value.name = 'Имя не может быть пустым.';
    isValid = false;
  }
  if (formData.value.email === '') {
    errors.value.email = 'Электронная почта не может быть пустой.';
    isValid = false;
  }
  if (formData.value.password === '') {
    errors.value.password = 'Пароль не может быть пустым.';
    isValid = false;
  }
  if (formData.value.password.length <= 8) {
    errors.value.password = 'Пароль должен состоять от 8 символов.';
    isValid = false;
  }

  if (!isValid) {
    return;
  }

  try {
    const employeeData = {
      nameEmployee: formData.value.name,
      loginEmployee: formData.value.email,
      passwordEmployee: formData.value.password,
      confirmPassword: formData.value.confirmPassword,
      roleEmployee: formData.value.role,
    };
    await adminService.addEmployee(employeeData);
    console.log('Пользователь добавлен.');
    emit('refresh-data');
    props.closeForm();
  } catch (error) {
    if (error.response && error.response.data.errors) {
      const serverErrors = error.response.data.errors;
      if (serverErrors.NameUser) {
        errors.value.name = serverErrors.NameUser[0];
      }
      if (serverErrors.ConfirmPassword) {
        errors.value.confirmPassword = serverErrors.ConfirmPassword[0];
      }
      if (serverErrors.ShortPassword) {
        errors.value.password = serverErrors.ShortPassword[0];
      }
      if (serverErrors.LoginUser) {
        errors.value.email = serverErrors.LoginUser[0];
      }
      if (serverErrors.RoleUser) {
        errors.value.role = serverErrors.RoleUser[0];
      }
    }
  }
};
</script>

<template>
  <main>
    <h1>Добавление модератора</h1>
    <div class="form">
      <label>Имя:</label>
      <input
        type="text"
        placeholder="Введите имя пользователя"
        v-model="formData.name"
        :class="{ 'input-error': errors.name }"
      />
      <div v-if="errors.name" class="error-message">{{ errors.name }}</div>
      <label>Электронная почта:</label>
      <input
        type="text"
        placeholder="Введите эл. почту пользователя"
        v-model="formData.email"
        :class="{ 'input-error': errors.email }"
      />
      <div v-if="errors.email" class="error-message">{{ errors.email }}</div>
      <label>Пароль:</label>
      <input
        type="password"
        placeholder="*****"
        v-model="formData.password"
        :class="{ 'input-error': errors.password }"
      />
      <div v-if="errors.password" class="error-message">
        {{ errors.password }}
      </div>
      <label>Подтверждение пароля:</label>
      <input
        type="password"
        placeholder="*****"
        v-model="formData.confirmPassword"
        :class="{ 'input-error': errors.confirmPassword }"
      />
      <div v-if="errors.confirmPassword" class="error-message">
        {{ errors.confirmPassword }}
      </div>
      <label>Роль:</label>
      <div>Модератор</div>
      <!-- <select v-model="formData.role">
        <option v-for="role in roles" :key="role" :value="role">
          {{ role }}
        </option>
      </select> -->
      <div v-if="errors.role" class="error-message">{{ errors.role }}</div>
      <div class="form-buttons">
        <button class="cancel" @click="closeForm">Отмена</button>
        <button @click="handleSubmit">Сохранить</button>
      </div>
    </div>
  </main>
</template>

<style scoped>
main {
  padding: 20px;
  background-color: white;
  border-radius: 5px;
  max-width: 600px;
  margin-left: auto;
  margin-right: auto;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

h1 {
  text-align: center;
  text-decoration: underline;
  text-decoration-color: forestgreen;
  font-size: 28px;
  margin-bottom: 20px;
}

.form {
  display: flex;
  flex-direction: column;
  gap: 15px;
}

label {
  font-weight: bold;
}

input,
select {
  width: 100%;
  height: auto;
  padding: 10px;
  border: 1px solid lightgrey;
  border-radius: 5px;
  box-sizing: border-box;
  font-size: 14px;
}

input:focus,
select:focus {
  outline: none;
  border-color: darkgreen;
}

.form-buttons {
  display: flex;
  justify-content: center;
  gap: 15px;
}

.form-buttons button {
  padding: 10px 20px;
  color: white;
  background-color: forestgreen;
  border: none;
  border-radius: 5px;
}

.form-buttons button:hover {
  background-color: darkgreen;
}

.form-buttons button.cancel {
  background-color: crimson;
}

.form-buttons button.cancel:hover {
  background-color: darkred;
}

.error-message {
  color: crimson;
  font-size: 10px;
}

.input-error {
  border: 2px solid darkred;
}
</style>
