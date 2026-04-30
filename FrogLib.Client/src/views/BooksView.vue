<script setup>
import { ref, computed } from 'vue';
import booksService from '@/services/booksService';

import TheHeader from '@/components/TheHeader.vue';
import BooksFilterPanel from '@/components/filteringPanels/BooksFilterPanel.vue';
import SmallBookCard from '@/components/cards/SmallBookCard.vue';
import TheFooter from '@/components/TheFooter.vue';

const allBooks = ref([]);
const searchQuery = ref('');
const selectedSort = ref('popular');
const selectedCategories = ref([]);

const getAllBooks = async () => {
  try {
    const response = await booksService.getAllBooks();
    allBooks.value = response;
  } catch (error) {
    console.error('Ошибка при загрузке книг:', error);
  }
};
getAllBooks();

const filteredBooks = computed(() => {
  let books = allBooks.value;

  if (searchQuery.value) {
    books = books.filter((book) =>
      book.title.toLowerCase().includes(searchQuery.value.toLowerCase())
    );
  }

  if (selectedCategories.value.length > 0) {
    books = books.filter((book) =>
      selectedCategories.value.includes(book.idCategory)
    );
  }

  if (selectedSort.value === 'popular') {
    books = books.sort((a, b) => b.averageRating - a.averageRating);
  } else if (selectedSort.value === 'news') {
    books = books.sort((a, b) => b.yearPublication - a.yearPublication);
  }

  return books;
});
</script>

<template>
  <main style="background-color: whitesmoke">
    <TheHeader />
    <div class="content-container">
      <BooksFilterPanel
        @search="searchQuery = $event"
        @sort="selectedSort = $event"
        @filter="selectedCategories = $event"
      />
      <section class="books-section">
        <div class="books-container">
          <SmallBookCard
            v-for="(book, index) in filteredBooks"
            :key="index"
            :id="book.id || index"
            :title="book.title"
            :averageRating="book.averageRating"
            :imageURL="book.imageURL"
          />
        </div>
      </section>
    </div>
    <TheFooter />
  </main>
</template>

<style scoped>
.content-container {
  display: flex;
  align-items: flex-start;
  gap: 20px;
  margin: 20px;
  margin-top: 70px;
}

.books-section {
  flex: 1;
  padding: 20px;
  background-color: white;
  border: 1px solid lightgrey;
  border-radius: 8px;
}

.books-container {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
}
</style>
