<script setup>
const props = defineProps({
  book: { type: Object, required: true },
});
</script>

<template>
  <div class="book-cover">
    <div
      class="blurred-background"
      :style="{
        backgroundImage: `url(${props.book.imageURL})`,
      }"
    ></div>
    <div class="book-container">
      <div class="content-wrapper">
        <img
          :src="props.book.imageURL"
          :alt="props.book.title"
          class="book-image"
        />
        <div class="text-content">
          <h1 class="book-title">
            <RouterLink :to="`/books/${props.book.id}`">{{
              props.book.title
            }}</RouterLink>
          </h1>
          <div class="book-author">{{ props.book.authors }}</div>
          <div class="rating">☆ {{ props.book.averageRating.toFixed(1) }}</div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.book-cover {
  position: relative;
  display: flex;
  align-self: center;
  width: 90%;
  padding: 10px;
  border-radius: 5px;
}

.blurred-background {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-size: cover;
  background-position: center;
  filter: blur(10px);
  z-index: 1;
}

.book-container {
  position: relative;
  z-index: 2;
  width: 100%;
  height: 100%;
  display: flex;
  justify-content: center;
  align-items: center;
}

.content-wrapper {
  display: flex;
  align-items: center;
  gap: 30px;
  padding: 20px;
  background-color: rgba(255, 255, 255, 0.3);
  backdrop-filter: blur(5px);
  border-radius: 8px;
}

.book-image {
  height: 350px;
}

.text-content {
  color: white;
  text-shadow: 0 2px 4px rgba(0, 0, 0, 0.5);
}

.book-title {
  margin: 0;
  max-width: 600px;
}

.book-title a {
  color: inherit;
}

.book-title a:hover {
  color: white;
  text-decoration: underline;
}

.book-author {
  font-size: 20px;
  margin-bottom: 20px;
}

.rating {
  font-size: 20px;
  font-weight: bold;
}
</style>
