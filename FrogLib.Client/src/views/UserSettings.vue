<script setup>
import { ref, computed } from 'vue';
import { useStore } from 'vuex';
import authService from '@/services/authService';

import TheHeader from '@/components/TheHeader.vue';

const store = useStore();
const user = computed(() => store.getters['auth/user']);

const form = ref({
  nameUser: user.value?.nameUser || '',
  loginUser: user.value?.loginUser || '',
  oldPassword: '',
  newPassword: '',
  confirmPassword: '',
  profileImage: null,
});
const deleteCurrentImage = ref(false);
const message = ref('');
const errors = ref({});

const handleDeleteImage = () => {
  deleteCurrentImage.value = true;
};

const handleFileChange = (e) => {
  form.value.profileImage = e.target.files[0];
  deleteCurrentImage.value = false;
};

const saveChanges = async () => {
  errors.value = {};
  message.value = '';

  try {
    const formData = new FormData();

    if (form.value.nameUser !== user.value?.nameUser) {
      formData.append('NameUser', form.value.nameUser);
    }

    if (form.value.loginUser !== user.value?.loginUser) {
      formData.append('LoginUser', form.value.loginUser);
    }

    if (form.value.newPassword) {
      if (!form.value.oldPassword) {
        errors.value.oldPassword = 'Введите текущий пароль';
        return;
      }
      if (form.value.newPassword !== form.value.confirmPassword) {
        errors.value.confirmPassword = 'Пароли не совпадают';
        return;
      }
      formData.append('OldPassword', form.value.oldPassword);
      formData.append('NewPassword', form.value.newPassword);
      formData.append('ConfirmPassword', form.value.confirmPassword);
    }

    if (deleteCurrentImage.value) {
      formData.append('DeleteImage', 'true');
    } else if (form.value.profileImage) {
      formData.append('ProfileImageUrl', form.value.profileImage);
    }

    const response = await authService.updateProfile(
      user.value.idUser,
      formData
    );

    console.log('Ответ сервера:', response.data);

    store.commit('auth/setUser', {
      ...user.value,
      nameUser: form.value.nameUser,
      loginUser: form.value.loginUser,
      profileImageUrl: response.data.profileImageUrl,
    });

    message.value = 'Профиль успешно обновлен.';

    console.log('image', user.value?.profileImageUrl);
  } catch (error) {
    if (
      error.response &&
      error.response.status === 400 &&
      error.response.data.errors
    ) {
      const serverErrors = error.response.data.errors;
      if (serverErrors.OldPassword) {
        errors.value.password = serverErrors.OldPassword[0];
      }
      if (serverErrors.ConfirmPassword) {
        errors.value.confirmPassword = serverErrors.ConfirmPassword[0];
      }
      if (serverErrors.LoginUser) {
        errors.value.email = serverErrors.LoginUser[0];
      }
      if (serverErrors.ProfileImageUrl) {
        errors.value.image = serverErrors.ProfileImageUrl[0];
      }
      if (serverErrors.ProfileImageUrl) {
        errors.value.image = serverErrors.ProfileImageUrl[0];
      }
    } else {
      message.value = 'Ошибка обновления профиля.';
    }
  }
};
</script>

<template>
  <TheHeader />
  <main style="background-color: whitesmoke">
    <div class="info-container">
      <div v-if="message" class="message">{{ message }}</div>
      <div class="title-container">Настройки пользователя</div>
      <div class="photo-user">
        <div class="heading">Изображения пользователя</div>
        <div style="display: flex; gap: 20px">
          <div
            class="current-photo"
            v-if="user?.profileImageUrl && !deleteCurrentImage"
          >
            <button class="button-delete" @click="handleDeleteImage">🗑</button>
            <img :src="`https://localhost:7157${user.profileImageUrl}`" />
          </div>
          <div class="new-photo">
            <div>Новое изображение</div>
            <input
              type="file"
              accept="image/*"
              class="input-image"
              @change="handleFileChange"
            />
          </div>
        </div>
      </div>
      <div v-if="errors.image" class="error-message">
        {{ errors.image }}
      </div>
      <div class="name-user">
        <div class="heading">Имя пользователя</div>
        <input type="text" v-model="form.nameUser" />
      </div>
      <div class="email-user">
        <div class="heading">Электронная почта</div>
        <input
          type="email"
          v-model="form.loginUser"
          :class="{ 'input-error': errors.email }"
        />
        <div v-if="errors.email" class="error-message">
          {{ errors.email }}
        </div>
      </div>
      <div class="password-user">
        <div class="heading">Смена пароля</div>
        <div class="current-password">
          <div>Текущий пароль</div>
          <input
            type="password"
            v-model="form.oldPassword"
            placeholder="*****"
            :class="{ 'input-error': errors.password }"
          />
          <div v-if="errors.password" class="error-message">
            {{ errors.password }}
          </div>
        </div>
        <div class="new-password">
          <div>Новый пароль</div>
          <input
            type="password"
            v-model="form.newPassword"
            placeholder="*****"
          />
        </div>
        <div class="new-password">
          <div>Подтверждение нового пароля</div>
          <input
            type="password"
            v-model="form.confirmPassword"
            placeholder="*****"
            :class="{ 'input-error': errors.confirmPassword }"
          />
          <div v-if="errors.confirmPassword" class="error-message">
            {{ errors.confirmPassword }}
          </div>
        </div>
      </div>
      <div class="buttons-container">
        <button @click="saveChanges">Сохранить изменения</button>
      </div>
    </div>
  </main>
</template>

<style scoped>
.info-container {
  margin: 70px auto 0 auto;
  width: 700px;
  display: flex;
  flex-direction: column;
  gap: 15px;
  padding: 15px;
  background-color: white;
  border-radius: 5px;
}

.title-container {
  text-align: center;
  font-size: 24px;
  font-weight: bold;
  text-decoration: underline;
  text-decoration-color: forestgreen;
}

.heading {
  font-size: 18px;
  font-weight: bold;
}

.photo-user {
  display: flex;
  flex-direction: column;
}

.current-photo {
  display: flex;
}

.current-photo img {
  height: 200px;
}

.button-delete {
  width: 30px;
  height: 30px;
  border: none;
  font-size: 16px;
  border-radius: 10px;
  background-color: white;
}

.button-delete:hover {
  background-color: darkred;
}

.input-image::file-selector-button {
  font-size: 14px;
  border: 1px solid forestgreen;
  border-radius: 5px;
  background: none;
}

input {
  width: 100%;
  height: 30px;
  border-radius: 5px;
  border-color: whitesmoke;
}

input:focus {
  outline: none;
  border-color: darkgreen;
}

.password-user {
  display: flex;
  flex-direction: column;
  gap: 5px;
}

.buttons-container {
  display: flex;
  gap: 5px;
  align-self: center;
}

.buttons-container button {
  font-size: 14px;
  border: 1px solid forestgreen;
  border-radius: 5px;
  background: none;
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
  font-size: 18px;
}
</style>
