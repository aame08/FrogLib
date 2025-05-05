<script setup>
import { ref } from 'vue';
import moderService from '@/services/moderService';

const props = defineProps({
  selectedCollection: { type: Object, required: true },
  closeForm: { type: Function },
});

const emit = defineEmits(['refresh-data']);

const categories = [
  'Спам',
  'Оскорбления',
  'Мошенничество',
  'Призывы к насилию',
  'Другое',
];

const localCollection = ref({ ...props.selectedCollection });
const showViolationsForm = ref(false);
const categoryViolation = ref('Спам');
const textViolation = ref('');

const updateCollectionStatus = async (status) => {
  try {
    await moderService.updateCollectionStatus(
      localCollection.value.idCollection,
      status
    );
    console.log('Статус изменен.');
    props.closeForm();
    emit('refresh-data');
  } catch (error) {
    console.error('Ошибка при изменении статуса подборки:', error);
  }
};

const submitViolation = async () => {
  try {
    const violationData = {
      idUser: localCollection.value.author.id,
      categoryViolation: categoryViolation.value,
      descriptionViolation: textViolation.value,
    };

    await moderService.updateCollectionStatus(
      localCollection.value.idCollection,
      'Отказано'
    );
    await moderService.addViolation(violationData);
    console.log('Нарушение отправлено.');
    props.closeForm();
    emit('refresh-data');
  } catch (error) {
    console.error('Ошибка при отправке нарушения:', error);
  }
};
</script>

<template>
  <main>
    <h1>Просмотр подборки</h1>
    <div class="collection-container">
      <div class="inner-container">
        <p>Автор:</p>
        <span>{{ localCollection.author.name }}</span>
      </div>
      <div class="inner-container">
        <p>Заголовок:</p>
        <span>{{ localCollection.titleCollection }}</span>
      </div>
      <div class="inner-container">
        <p>Описание:</p>
        <span v-html="localCollection.textCollection"></span>
      </div>
      <div class="inner-container">
        <p>Книги:</p>
        <div class="book-container">
          <div
            class="book-card"
            v-for="book in localCollection.books"
            :key="book.idBook"
          >
            <img :src="book.imageURL" />
            <span>{{ book.title }}</span>
          </div>
        </div>
      </div>
      <div class="buttons-form" v-if="!showViolationsForm">
        <button class="button red" @click="closeForm">Отмена</button>
        <button class="button red" @click="showViolationsForm = true">
          Отклонить с нарушением
        </button>
        <button class="button red" @click="updateCollectionStatus('Отказано')">
          Отклонить
        </button>
        <button class="button" @click="updateCollectionStatus('Одобрено')">
          Принять
        </button>
      </div>
      <div class="violations-container" v-else>
        <div class="violations-info">
          <label>
            Выберите категорию:
            <select v-model="categoryViolation">
              <option
                v-for="category in categories"
                :key="category"
                :value="category"
              >
                {{ category }}
              </option>
            </select></label
          >
          <label
            >Описание нарушения: <textarea v-model="textViolation"></textarea>
          </label>
        </div>
        <div class="violations-buttons">
          <button class="button red" @click="showViolationsForm = false">
            Отмена
          </button>
          <button class="button" @click="submitViolation">Сохранить</button>
        </div>
      </div>
    </div>
  </main>
</template>

<style scoped>
main {
  max-width: 1200px;
  margin-left: auto;
  margin-right: auto;
  padding: 20px;
  border-radius: 5px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

h1 {
  text-align: center;
  text-decoration: underline;
  text-decoration-color: forestgreen;
  font-size: 28px;
  margin-bottom: 20px;
}

.collection-container {
  display: flex;
  flex-direction: column;
  padding: 20px;
  background-color: white;
  border: 1px solid forestgreen;
  border-radius: 5px;
}

.inner-container {
  display: flex;
  align-items: center;
  gap: 10px;
}

p {
  font-weight: bold;
  width: 150px;
  flex-shrink: 0;
}

span {
  word-break: break-word;
}

.book-container {
  display: flex;
  gap: 15px;
  align-items: center;
}

.book-card {
  display: flex;
  flex-direction: column;
}

.book-card img {
  height: 200px;
}

.book-card span {
  max-width: 150px;
}

.buttons-form {
  margin-top: 25px;
  display: flex;
  gap: 15px;
  justify-content: center;
}

.button {
  padding: 10px 20px;
  background-color: forestgreen;
  color: white;
  border: none;
  border-radius: 5px;
}

.button:hover {
  background-color: darkgreen;
}

.button.red {
  background-color: crimson;
}

.button.red:hover {
  background-color: darkred;
}

.violations-container {
  align-self: center;
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.violations-info {
  display: flex;
  flex-direction: column;
}

.violations-container label {
  font-weight: bold;
}

.violations-buttons {
  display: flex;
  justify-content: center;
  gap: 5px;
}
</style>
