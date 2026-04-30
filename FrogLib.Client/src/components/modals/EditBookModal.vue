<script setup>
import { ref, computed } from 'vue';
import { useStore } from 'vuex';
import booksService from '@/services/booksService';
import userActivityService from '@/services/userActivityService';

const props = defineProps({
  book: {
    type: Object,
    default: () => ({ rating: null }),
  },
  isVisible: Boolean,
});

const emit = defineEmits(['close', 'update']);

const selectedRating = ref(props.book.rating || 0);
const selectedList = ref(props.book.idListType);
const listTypes = ref([]);

const store = useStore();
const user = computed(() => store.getters['auth/user']);
const userId = computed(() => user.value?.idUser || null);

const getListTypes = async () => {
  try {
    const response = await booksService.getListTypes();
    listTypes.value = response;
  } catch (error) {
    console.error('Ошибка при загрузке списков:', error);
  }
};
getListTypes();

const setRating = (rating) => {
  selectedRating.value = rating;
};

const handleSave = async () => {
  try {
    if (selectedRating.value > 0) {
      await userActivityService.updateBookRating(
        userId.value,
        props.book.idBook,
        selectedRating.value
      );
    }
    await userActivityService.updateUserList(
      userId.value,
      props.book.idBook,
      selectedList.value
    );
    emit('update');
    emit('close');
  } catch (error) {
    console.error('Ошибка при обновлении:', error);
  }
};

const handleDelete = async () => {
  try {
    if (selectedRating.value > 0) {
      await userActivityService.deleteBookRating(
        userId.value,
        props.book.idBook
      );
    }
    await userActivityService.deleteUserList(userId.value, props.book.idBook);
    emit('update');
    emit('close');
  } catch (error) {
    console.error('Ошибка при удалении:', error);
  }
};
</script>

<template>
  <div class="modal-overlay" v-if="isVisible">
    <div class="modal-content">
      <button class="button-close" @click="$emit('close')">✕</button>
      <div class="modal-container">
        <div class="title">Редактирование книги</div>
        <div class="evaluation-container">
          <div class="user-evaluation">
            <div>
              Моя оценка:
              <span style="font-weight: bold" v-if="selectedRating > 0">
                {{ book.rating }} из 5
              </span>
              <span v-else>Отсутствует</span>
            </div>
          </div>
          <div class="evaluations">
            <button
              v-for="star in 5"
              :key="star"
              class="button-star"
              :class="{ selected: selectedRating >= star }"
              @click="setRating(star)"
            >
              {{ selectedRating >= star ? '★' : '☆' }}
            </button>
          </div>
        </div>
        <div class="user-list">
          <div>Список</div>
          <select name="list-books" v-model="selectedList">
            <option
              v-for="list in listTypes"
              :key="list.idListType"
              :value="list.idListType"
            >
              {{ list.nameList }}
            </option>
          </select>
        </div>
        <div class="buttons">
          <button class="button delete" @click="handleDelete">Удалить</button>
          <button class="button save" @click="handleSave">Сохранить</button>
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
  display: flex;
  flex-direction: column;
  height: 300px;
  width: 550px;
  border-radius: 25px;
  background-color: white;
  overflow: auto;
  padding: 0 25px;
  box-sizing: border-box;
}

.button-close {
  align-self: flex-end;
  width: 20px;
  height: 20px;
  margin-right: 15px;
  background: none;
  border: none;
  color: black;
  font-size: 18px;
}

.button-close:hover {
  color: darkgreen;
}

.modal-container {
  display: flex;
  flex-direction: column;
  align-self: center;
  margin-top: 30px;
  gap: 15px;
  width: 100%;
}

.title {
  text-align: center;
  font-size: 20px;
  font-weight: bold;
  border-bottom: 2px solid forestgreen;
}

.evaluation-container {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.user-evaluation {
  display: flex;
  justify-content: space-between;
  gap: 10px;
}

.evaluations {
  display: flex;
  justify-content: center;
  gap: 10px;
}

.button-star {
  background: none;
  border: none;
  font-size: 24px;
  cursor: pointer;
  color: gray;
}

.button-star.selected {
  color: darkgreen;
}

select {
  width: 100%;
  font-size: 16px;
}

.buttons {
  display: flex;
  align-self: flex-end;
  margin-top: 10px;
  gap: 5px;
}

.button {
  display: flex;
  align-items: center;
  background: none;
  border: none;
  color: black;
  font-size: 16px;
}

.button.save:hover {
  border: 2px solid darkgreen;
  border-radius: 5px;
}

.button.delete:hover {
  border: 2px solid darkred;
  border-radius: 5px;
}
</style>
