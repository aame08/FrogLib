<script setup>
import { ref, computed, watchEffect } from 'vue';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';
import userActivityService from '@/services/userActivityService';

import BooksForCollection from './modals/BooksForCollectionModal.vue';

const props = defineProps({
  closeCollectionForm: { type: Function, required: true },
  initialBook: { type: Object, default: null },
  collectionId: { type: Number, default: null },
  initialData: { type: Object, default: () => ({}) },
  isAdminContext: { type: Boolean, default: false },
});

const emit = defineEmits(['refresh-data']);

const store = useStore();
const router = useRouter();
const user = computed(() => store.getters['auth/user']);
const idUser = computed(() => user.value?.idUser || null);

const showBooksForCollectionModal = ref(false);
const errors = ref({});
const titleCollection = ref('');
const textCollection = ref('');
const selectedBooks = ref([]);
const isBold = ref(false);
const isItalic = ref(false);
const isUnderline = ref(false);

const openModal = () => (showBooksForCollectionModal.value = true);
const closeModal = () => {
  showBooksForCollectionModal.value = false;
};

if (props.initialBook) {
  selectedBooks.value = [
    {
      id: props.initialBook.id,
      title: props.initialBook.title,
      imageURL: props.initialBook.imageURL,
    },
  ];
}

const toggleFormatting = (type) => {
  const textarea = document.querySelector('textarea');
  const start = textarea.selectionStart;

  let newText = textCollection.value;

  switch (type) {
    case 'bold':
      newText = isBold.value
        ? `${textCollection.value.slice(
            0,
            start
          )}</b>${textCollection.value.slice(start)}`
        : `${textCollection.value.slice(
            0,
            start
          )}<b>${textCollection.value.slice(start)}`;
      isBold.value = !isBold.value;
      break;
    case 'italic':
      newText = isItalic.value
        ? `${textCollection.value.slice(
            0,
            start
          )}</i>${textCollection.value.slice(start)}`
        : `${textCollection.value.slice(
            0,
            start
          )}<i>${textCollection.value.slice(start)}`;
      isItalic.value = !isItalic.value;
      break;
    case 'underline':
      newText = isUnderline.value
        ? `${textCollection.value.slice(
            0,
            start
          )}</u>${textCollection.value.slice(start)}`
        : `${textCollection.value.slice(
            0,
            start
          )}<u>${textCollection.value.slice(start)}`;
      isUnderline.value = !isUnderline.value;
      break;
  }

  textCollection.value = newText;
  textarea.focus();
  textarea.setSelectionRange(start + 3, start + 3);
};

const handleSaveBooks = (books) => {
  selectedBooks.value = books;
  closeModal();
};

const handleCancelBooks = () => {
  if (route.name === 'BookPage') {
    selectedBooks.value = selectedBooks.value.filter(
      (book) => book.id === route.params.id
    );
  } else {
    selectedBooks.value = [];
  }
  closeModal();
};

const saveCollection = async () => {
  errors.value = {};

  let isValid = true;

  if (titleCollection.value.trim() === '') {
    errors.value.titleCollection = 'Заголовок подборки не может быть пустым.';
    isValid = false;
  }
  if (selectedBooks.value.length < 5) {
    errors.value.selectedBooks = 'В подборке должно быть от 5 книг.';
    isValid = false;
  }

  if (!isValid) {
    return;
  }

  try {
    const formattedText = textCollection.value.replace(/\n/g, '<br>');

    const collectionRequest = {
      idUser: idUser.value,
      titleCollection: titleCollection.value,
      descriptionCollection: formattedText,
      idBooks: selectedBooks.value.map((book) => book.id),
    };

    if (props.collectionId) {
      await userActivityService.editCollection(
        props.collectionId,
        collectionRequest
      );
    } else {
      await userActivityService.addCollection(collectionRequest);
    }

    props.closeCollectionForm();
    emit('refresh-data');

    if (!props.isAdminContext) {
      if (router.currentRoute.value.path !== '/profile') {
        router.push('/collections');
      }
    }
  } catch (error) {
    console.error('Ошибка при отправке подборки:', error);
  }
};

watchEffect(() => {
  if (props.initialData && props.collectionId) {
    titleCollection.value = props.initialData.titleCollection || '';
    textCollection.value = props.initialData.textCollection || '';
    selectedBooks.value = props.initialData.initialBooks || [];
  }
});
</script>

<template>
  <main>
    <BooksForCollection
      :isVisible="showBooksForCollectionModal"
      :initialSelectedBooks="selectedBooks"
      @save="handleSaveBooks"
      @cancel="handleCancelBooks"
      @close="closeModal"
    />
    <div class="collection-container">
      <h1 class="title-page">Подборка книг</h1>
      <div class="collection-name">
        <h4>Название подборки:</h4>
        <input
          type="text"
          placeholder="Введите название подборки..."
          v-model="titleCollection"
          :class="{ error: errors.titleCollection }"
        />
      </div>
      <div v-if="errors.titleCollection" class="error-message">
        {{ errors.titleCollection }}
      </div>
      <div class="collection-description">
        <h4>Описание подборки:</h4>
        <div class="collection-text">
          <textarea v-model="textCollection"></textarea>
          <div class="toolbar">
            <button
              :class="{ active: isBold }"
              @click="toggleFormatting('bold')"
              title="Жирный текст"
            >
              <b>B</b>
            </button>
            <button
              :class="{ active: isItalic }"
              @click="toggleFormatting('italic')"
              title="Курсив"
            >
              <i>I</i>
            </button>
            <button
              :class="{ active: isUnderline }"
              @click="toggleFormatting('underline')"
              title="Подчеркнутый текст"
            >
              <u>U</u>
            </button>
          </div>
        </div>
      </div>
      <div class="collection-books">
        <div class="inner-container">
          <h4>Добавить книги:</h4>
          <button class="button-add" @click="openModal">Добавить</button>
        </div>
        <ul>
          <li v-for="book in selectedBooks" :key="book.id">{{ book.title }}</li>
        </ul>
      </div>
      <div v-if="errors.selectedBooks" class="error-message">
        {{ errors.selectedBooks }}
      </div>
      <div class="buttons-group">
        <button @click="closeCollectionForm">Отмена</button>
        <button @click="saveCollection">Сохранить</button>
      </div>
    </div>
  </main>
</template>

<style scoped>
main {
  margin-top: 70px;
  max-width: 1000px;
  margin-left: auto;
  margin-right: auto;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
}

.collection-container {
  display: flex;
  flex-direction: column;
  background-color: white;
  border-radius: 5px;
  padding: 5px;
}

.title-page {
  text-align: center;
  text-decoration: underline;
  text-decoration-color: forestgreen;
}

.collection-name,
.collection-description {
  display: flex;
  align-items: center;
  gap: 10px;
}

h4 {
  width: 200px;
}

.collection-text {
  position: relative;
  width: 80%;
}

.collection-text textarea {
  height: 100px;
  font-size: 16px;
  border-radius: 5px;
}

.toolbar {
  margin-top: 10px;
}

.toolbar button {
  width: 35px;
  height: 35px;
  padding: 5px 10px;
  margin-right: 5px;
  font-size: 16px;
  background: none;
  border: none;
}

.toolbar button:hover {
  border: 1px solid forestgreen;
  border-radius: 5px;
}

.toolbar button.active {
  border: 1px solid forestgreen;
  border-radius: 5px;
}

input {
  width: 80%;
  height: 35px;
  border-radius: 5px;
}

.inner-container {
  display: flex;
  align-items: center;
  gap: 10px;
}

.button-add {
  height: 40px;
  border-color: forestgreen;
  border-radius: 5px;
  background: white;
}

.button-add:hover {
  font-weight: bold;
}

.buttons-group {
  display: flex;
  align-self: center;
  gap: 15px;
  margin-top: 25px;
}

.buttons-group button {
  height: 40px;
  font-size: 16px;
  border-color: forestgreen;
  border-radius: 5px;
  background: white;
}

.buttons-group button:hover {
  font-weight: bold;
}

.error {
  border-color: darkred;
}

.error-message {
  color: crimson;
  font-size: 16px;
  text-align: center;
}
</style>
