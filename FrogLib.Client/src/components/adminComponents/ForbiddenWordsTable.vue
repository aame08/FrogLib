<script setup>
import { ref } from 'vue';
import adminService from '@/services/adminService';

const props = defineProps({
  words: { type: Array, required: true },
});

const emit = defineEmits(['refresh']);

const editingId = ref(null);
const editedWord = ref('');

const startEdit = (index, word) => {
  editingId.value = index;
  editedWord.value = word;
};

const saveEdit = async (index) => {
  try {
    await adminService.updateForbiddenWord(index, editedWord.value);
    emit('refresh');
    editingId.value = null;
  } catch (error) {
    console.error('Ошибка при обновлении слова:', error);
  }
};

const cancelEdit = () => {
  editingId.value = null;
};

const deleteWord = async (index) => {
  try {
    await adminService.deleteForbiddenWord(index);
    emit('refresh');
  } catch (error) {
    console.error('Ошибка при удалении слова:', error);
  }
};
</script>

<template>
  <table>
    <thead>
      <tr>
        <th>ID</th>
        <th>Значение</th>
        <th>Действия</th>
      </tr>
    </thead>
    <tbody>
      <tr v-for="(word, index) in words" :key="index">
        <td>{{ index + 1 }}</td>
        <td>
          <span v-if="editingId !== index">{{ word }}</span>
          <input v-else v-model="editedWord" />
        </td>
        <td>
          <div class="action-buttons">
            <button
              v-if="editingId !== index"
              @click="startEdit(index, word)"
              class="action-button"
            >
              Редактировать
            </button>
            <button v-else class="action-button" @click="saveEdit(index)">
              Сохранить
            </button>
            <button
              v-if="editingId === index"
              class="action-button delete"
              @click="cancelEdit"
            >
              Отменить
            </button>
            <button class="action-button delete" @click="deleteWord(index)">
              Удалить
            </button>
          </div>
        </td>
      </tr>
    </tbody>
  </table>
</template>

<style scoped>
.action-buttons {
  display: flex;
  justify-content: center;
  gap: 10px;
}

td select {
  height: auto;
  width: 90%;
  padding: 10px;
  font-size: 14px;
  border: 1px solid lightgrey;
  border-radius: 5px;
}

input {
  height: 30px;
  width: 80%;
  border-color: forestgreen;
  border-radius: 5px;
}

input:focus {
  outline: none;
  border-color: darkgreen;
}
</style>
