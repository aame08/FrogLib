<script setup>
import { ref, computed, watch } from 'vue';

const props = defineProps({
  isVisible: Boolean,
  initialRating: Number,
});

const emit = defineEmits(['close', 'submit', 'delete', 'refresh-book-data']);

const selectedRating = ref(props.initialRating || 0);
const hasRating = computed(() => props.initialRating > 0);

const isRatingChanged = computed(
  () => selectedRating.value !== props.initialRating
);

const setRating = (rating) => {
  selectedRating.value = rating;
};

const resetRating = () => {
  selectedRating.value = 0;
};

const submitRating = () => {
  if (isRatingChanged.value) {
    emit('submit', selectedRating.value);
    emit('refresh-book-data');
  }
  emit('close');
};

const deleteRating = () => {
  emit('delete');
  emit('refresh-book-data');
  emit('close');
};

watch(() => props.initialRating, (newVal) => {
  selectedRating.value = newVal || 0;
});
</script>

<template>
  <div class="modal-overlay" v-if="isVisible">
    <div class="modal-content">
      <button class="button-close" @click="$emit('close')">✕</button>
      <div class="modal-container">
        <div class="title">Оценка книги</div>
        <div class="sub-title">Поставьте оценку.</div>
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
        <div class="buttons">
          <button class="transparent-button" @click="resetRating">
            Отмена
          </button>
          <button
            v-if="hasRating && !isRatingChanged"
            class="transparent-button"
            @click="deleteRating"
          >
            Удалить оценку
          </button>
          <button
            v-else-if="hasRating && isRatingChanged"
            class="transparent-button"
            @click="submitRating"
          >
            Изменить оценку
          </button>
          <button v-else class="transparent-button" @click="submitRating">
            Оценить
          </button>
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
  height: 240px;
  width: 450px;
  display: flex;
  flex-direction: column;
  border-radius: 25px;
  background-color: white;
  overflow: auto;
}

.button-close {
  align-self: flex-end;
  height: 20px;
  width: 20px;
  margin-right: 15px;
  font-size: 18px;
  background: none;
  border: none;
  color: black;
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
  border-bottom: 2px solid forestgreen;
  font-size: 20px;
  font-weight: bold;
}

.sub-title {
  width: 100%;
  font-size: 18px;
  text-align: center;
}

.evaluations {
  display: flex;
  gap: 10px;
  justify-content: center;
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

.buttons {
  align-self: flex-end;
  margin-top: 10px;
  display: flex;
  gap: 5px;
  margin-right: 10px;
}
</style>
