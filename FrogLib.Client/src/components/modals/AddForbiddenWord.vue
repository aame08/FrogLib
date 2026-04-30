<script setup>
import { defineProps, ref } from 'vue';
import adminService from '@/services/adminService';

const props = defineProps({
  isVisible: Boolean,
});

const emit = defineEmits(['close', 'refresh']);

const newWord = ref('');
const message = ref('');

const addWord = async () => {
  message.value = '';

  if (newWord.value === '') {
    message.value = 'Слово не может быть пустым.';
    return;
  }

  try {
    await adminService.addForbiddenWord(newWord.value);
    emit('refresh');
    emit('close');

    message.value = '';
  } catch (error) {
    console.error('Ошибка при добавлении слова:', error);
  }
};
</script>

<template>
  <div class="modal-overlay" v-if="isVisible">
    <div class="modal-content">
      <button class="button-close" @click="$emit('close')">✕</button>
      <div class="modal-container">
        <div class="title">Добавление нового запрещённого слова</div>
        <div v-if="message" class="message">{{ message }}</div>
        <div class="container">
          <input type="text" v-model="newWord" />
          <div class="buttons">
            <button class="transparent-button cancel">Отмена</button>
            <button class="transparent-button" @click="addWord">
              Добавить
            </button>
          </div>
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
  height: 225px;
  width: 450px;
  display: flex;
  flex-direction: column;
  border-radius: 25px;
  background-color: white;
  overflow: auto;
  position: relative;
}

.button-close {
  position: absolute;
  top: 15px;
  right: 15px;
  height: 20px;
  width: 20px;
  font-size: 18px;
  background: none;
  border: none;
  color: black;
  cursor: pointer;
}

.button-close:hover {
  color: darkgreen;
}

.modal-container {
  display: flex;
  flex-direction: column;
  height: 100%;
  padding: 40px 20px 20px;
  gap: 15px;
}

.title {
  text-align: center;
  border-bottom: 2px solid forestgreen;
  font-size: 20px;
  font-weight: bold;
  padding-bottom: 10px;
}

.container {
  display: flex;
  flex-direction: column;
  flex-grow: 1;
  justify-content: center;
  gap: 15px;
}

input {
  width: 80%;
  border-radius: 5px;
  border: 1px solid forestgreen;
  padding: 8px;
  margin: 0 auto;
}

input:focus {
  border-color: darkgreen;
}

.buttons {
  display: flex;
  justify-content: flex-end;
  gap: 5px;
  margin-top: auto;
}

.cancel:hover {
  text-decoration-color: darkred;
}

.message {
  color: grey;
  text-align: center;
}
</style>
