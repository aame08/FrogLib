<script setup>
import TheHeader from '@/components/TheHeader.vue';
</script>

<template>
  <TheHeader />
  <main>
    <RouterView></RouterView>
  </main>
</template>

<style scoped>
main {
  margin-top: 70px;
  padding: 20px;
  background-color: whitesmoke;
  border-radius: 5px;
}
</style>
