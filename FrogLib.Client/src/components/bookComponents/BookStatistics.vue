<script setup>
const props = defineProps({
  averageRating: { type: Number, required: true },
  totalRatings: { type: Number, required: true },
  ratingStatistics: { type: Array, required: true },
  totalUserBookmarks: { type: Number, required: true },
  userBookmarks: { type: Array, required: true },
});

const emit = defineEmits(['refresh-book-data']);

const pluralizeUsers = (count) => {
  if (count === 1) {
    return ' пользователя';
  } else if ([2, 3, 4].includes(count % 10)) {
    return ' пользователей';
  } else if (count === 0) {
    return ' пользователей';
  }
};
</script>

<template>
  <div class="statistics">
    <div class="rating">
      <div class="statistics-header">
        <h4>Оценки пользователей</h4>
        <span class="average-rating">
          ☆ {{ averageRating.toFixed(2) }}
          <span class="total-votes">{{ totalRatings.toFixed(1) }}</span>
        </span>
      </div>
      <div class="statistics-bars">
        <div
          class="statistics-bar"
          v-for="rating in ratingStatistics"
          :key="rating.ratingValue"
        >
          <div class="statistics-value">{{ rating.ratingValue }}</div>
          <div class="bar-container">
            <div class="bar" :style="{ width: rating.percentage + '%' }"></div>
          </div>
          <span class="percentage">{{ rating.percentage.toFixed(0) }}%</span>
          <span class="votes">{{ rating.count }}</span>
        </div>
      </div>
    </div>
    <div class="bookmarks">
      <div class="bookmarks-header">
        <h4>
          В списках у <span>{{ totalUserBookmarks }}</span>
          <span>{{ pluralizeUsers(totalUserBookmarks) }}</span>
        </h4>
      </div>
      <div class="statistics-bars">
        <div
          class="statistics-bar"
          v-for="bookmark in userBookmarks"
          :key="bookmark.listType"
        >
          <div class="bookmarks-value">{{ bookmark.listType }}</div>
          <div class="bar-container">
            <div
              class="bar"
              :style="{ width: bookmark.percentage + '% ' }"
            ></div>
          </div>
          <span class="percentage">{{ bookmark.percentage.toFixed(0) }}%</span>
          <span class="votes">{{ bookmark.count }}</span>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.statistics {
  margin-top: 10px;
  display: flex;
  gap: 25px;
  flex-wrap: wrap;
}

.rating {
  flex-grow: 1;
  display: flex;
  flex-direction: column;
  border-radius: 8px;
  padding: 10px;
  max-width: 300px;
}

.bookmarks {
  flex-grow: 1;
  display: flex;
  flex-direction: column;
  border-radius: 8px;
  padding: 10px;
  max-width: 400px;
}

.statistics-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: -15px;
}

.bookmarks-header {
  display: flex;
  align-items: center;
  margin-top: -15px;
}

.average-rating {
  display: flex;
  align-items: center;
}

.total-votes {
  margin-left: 10px;
  font-size: 14px;
  color: grey;
}

.statistics-bars {
  display: flex;
  flex-direction: column;
  gap: 5px;
}

.statistics-bar {
  display: flex;
  align-items: center;
}

.statistics-value {
  margin-right: 10px;
  font-size: 16px;
}

.bookmarks-value {
  margin-right: 10px;
  font-size: 16px;
  width: 80px;
}

.bar-container {
  width: 200px;
  height: 10px;
  background-color: whitesmoke;
  border-radius: 5px;
  overflow: hidden;
  margin-right: 10px;
}

.bar {
  height: 100%;
  background-color: forestgreen;
  border-radius: 5px;
}

.percentage {
  width: 35px;
  margin-right: 10px;
  font-size: 14px;
}

.votes {
  font-size: 14px;
  color: grey;
}
</style>
