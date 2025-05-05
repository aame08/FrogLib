<script setup>
import { computed } from 'vue';
import { useStore } from 'vuex';

const emit = defineEmits(['create-collection']);

const store = useStore();
const isAuthenticated = computed(() => store.getters['auth/isAuthenticated']);

const handleCreateCollection = () => {
  emit('create-collection');
};
</script>

<template>
  <div class="create-banner">
    <div class="banner-text">
      <h1>Твоя коллекция книг - твои правила</h1>
      <p>
        Составь уникальную подборку из любимых произведений и открывай новое
        каждый день.
      </p>
    </div>
    <button
      class="transparent-button"
      v-if="isAuthenticated"
      @click="handleCreateCollection"
    >
      + Создать подборку
    </button>
    <button v-else :disabled="!isAuthenticated" class="transparent-button">
      + Создать подборку
    </button>
  </div>
</template>

<style scoped>
.create-banner {
  display: flex;
  background-color: white;
  margin-top: 70px;
  border: 1px solid forestgreen;
  border-radius: 5px;
  padding: 5px;
  max-width: 850px;
  margin-left: auto;
  margin-right: auto;
  align-items: center;
  gap: 10px;
}
</style>
