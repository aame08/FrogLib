<script setup>
import { ref, computed, watch } from 'vue';
import { useStore } from 'vuex';
import booksService from '@/services/booksService';
import userActivityService from '@/services/userActivityService';

const props = defineProps({
  id: { type: Number, required: true },
  title: { type: String, required: true },
  imageURL: { type: String, required: true },
  openReviewForm: { type: Function, required: true },
  openCollectionForm: { type: Function, required: true },
  bookData: { type: Object, required: true },
});

const emit = defineEmits(['refresh-book-data', 'create-collection']);

const store = useStore();
const isAuthenticated = computed(() => store.getters['auth/isAuthenticated']);
const user = computed(() => store.getters['auth/user']);
const idUser = computed(() => user.value?.idUser || null);

const listTypes = ref([]);
const userList = ref(null);
const selectedList = ref('');

const isListChanged = computed(() => {
  return selectedList.value !== userList.value;
});

watch(
  userList,
  (newVal) => {
    selectedList.value = newVal || '';
  },
  { immediate: true }
);

const handleCreateCollection = () => {
  if (props.openCollectionForm) {
    props.openCollectionForm(props.bookData);
  } else {
    emit('create-collection', props.bookData);
  }
};

const getListTypes = async () => {
  try {
    const response = await booksService.getListTypes();

    listTypes.value = response;
  } catch (error) {
    console.error('Ошибка при загрузке списков:', error);
  }
};
getListTypes();

const loadBookList = async () => {
  if (isAuthenticated.value && idUser.value) {
    try {
      const listUser = await userActivityService.getUserList(
        idUser.value,
        props.id
      );
      userList.value = listUser;
      selectedList.value = userList.value || '';
    } catch (error) {
      console.error('Ошибка при загрузке:', error);
    }
  }
};
loadBookList();

const updateBookList = async (newList) => {
  if (isListChanged.value) {
    newList = parseInt(newList);
    try {
      await userActivityService.updateUserList(idUser.value, props.id, newList);
      userList.value = newList;
      emit('refresh-book-data');
    } catch (error) {
      console.error('Ошибка:', error);
    }
  }
};

const handleSelectChange = async () => {
  if (selectedList.value === 'remove') {
    try {
      await userActivityService.deleteUserList(idUser.value, props.id);
      userList.value = null;
      selectedList.value = '';
      console.log('Книга удалена из списка');
      emit('refresh-book-data');
    } catch (error) {
      console.error('Ошибка при удалении:', error);
    }
  } else {
    await updateBookList(selectedList.value);
  }
};
</script>

<template>
  <div class="book-cover">
    <img :src="imageURL" :alt="title" />
    <select
      name="list-books"
      id="list-books"
      v-model="selectedList"
      @change="handleSelectChange"
    >
      <option v-if="!userList" value="" disabled>+ Добавить</option>
      <option
        v-for="list in listTypes"
        v-if="isAuthenticated"
        :key="list.idListType"
        :value="list.idListType"
      >
        {{ list.nameList }}
      </option>
      <option v-if="userList" value="remove">Удалить из списка</option>
    </select>
    <button v-if="isAuthenticated" @click="openReviewForm">
      📃 написать рецензию
    </button>
    <button v-else :disabled="!isAuthenticated">📃 написать рецензию</button>
    <button v-if="isAuthenticated" @click="handleCreateCollection">
      + создать подборку
    </button>
    <button v-else :disabled="!isAuthenticated">+ создать подборку</button>
  </div>
</template>

<style scoped>
.book-cover {
  flex: 0 0 300px;
  background-color: whitesmoke;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 10px;
}

.book-cover img {
  height: 350px;
  margin-top: -160px;
  z-index: 100;
}

.book-cover select {
  font-size: 16px;
  width: 90%;
  text-align: center;
}

.book-cover button {
  font-size: 15px;
  background: none;
  border: 2px solid forestgreen;
  border-radius: 5px;
}
</style>
