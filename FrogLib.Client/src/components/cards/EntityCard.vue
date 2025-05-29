<script setup>
const props = defineProps({
  entity: { type: Object, required: true },
  isSelected: { type: Boolean, default: false },
});

const emit = defineEmits(['selected']);

const handleSelect = () => {
  emit('selected', props.entity);
};
</script>

<template>
  <button
    class="list-item"
    :class="{ selected: isSelected }"
    @click="handleSelect"
  >
    <span>
      <strong>{{ props.entity.typeEntity }}: </strong>
      <span>{{ props.entity.entityName }}</span></span
    >
    <span>Всего комментариев: {{ props.entity.totalCommentCount }}</span>
    <span>Непроверенных комментариев: {{ props.entity.newCommentCount }}</span>
  </button>
</template>

<style scoped>
.list-item {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  width: 100%;
  padding: 10px;
  margin-bottom: 10px;
  border: 1px solid lightgrey;
  border-radius: 5px;
  background: none;
}

.list-item:hover {
  background-color: whitesmoke;
}

.list-item.selected {
  border-color: darkgreen;
}
</style>
