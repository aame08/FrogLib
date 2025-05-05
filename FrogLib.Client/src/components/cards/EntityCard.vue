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
    <span class="entity-info">
      <strong>{{ props.entity.typeEntity }}: </strong>
      <span>{{ props.entity.entityName }}</span></span
    >
    <span>Количество комментариев: {{ props.entity.commentCount }}</span>
  </button>
</template>

<style scoped>
.list-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
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

.entity-info {
  text-align: start;
}

.list-item.selected {
  border-color: darkgreen;
}
</style>
