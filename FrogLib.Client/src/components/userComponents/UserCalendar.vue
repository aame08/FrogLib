<script setup>
import { ref, computed, watch } from 'vue';
import { useStore } from 'vuex';
import userActivityService from '@/services/userActivityService';
import booksService from '@/services/booksService';

const currentDate = ref(new Date());
const today = new Date();
const selectedDate = ref(null);
const booksByDate = ref({});
const listTypes = ref([]);
const activeFilter = ref(null);
const availableFilters = ref([]);

const store = useStore();
const user = computed(() => store.getters['auth/user']);
const userId = computed(() => user.value?.idUser || null);

const loadListTypes = async () => {
  try {
    const response = await booksService.getListTypes();
    listTypes.value = response;
  } catch (error) {
    console.error('Ошибка при загрузке списков:', error);
  }
};
loadListTypes();

watch(listTypes, (types) => {
  availableFilters.value = [
    { id: null, name: 'Все книги' },
    ...types.map((type) => ({
      id: type.idListType,
      name: type.nameList,
    })),
  ];
});

const filteredBooks = computed(() => {
  if (!selectedDate.value) return [];

  let books = booksByDate.value[selectedDate.value] || [];

  if (activeFilter.value) {
    books = books.filter((book) => book.listType.id === activeFilter.value);
  }

  return books;
});

const loadCalendarData = async () => {
  try {
    const response = await userActivityService.getUserCalendar(userId.value);
    processCalendarData(response);
  } catch (error) {
    console.error('Ошибка при загрузке календаря:', error);
  }
};
loadCalendarData();

const processCalendarData = (books) => {
  const formattedData = {};
  books.forEach((book) => {
    const date = book.addedDate;
    if (!formattedData[date]) {
      formattedData[date] = [];
    }
    formattedData[date].push({
      id: book.idBook,
      title: book.title,
      imageURL: book.imageURL,
      listType: book.listType,
    });
  });
  booksByDate.value = formattedData;
};

const currentMonthDisplay = computed(() => {
  return currentDate.value.toLocaleString('ru', {
    month: 'long',
    year: 'numeric',
  });
});

const selectedDateBooks = computed(() => {
  if (!selectedDate.value) return [];
  return booksByDate.value[selectedDate.value] || [];
});

const calendarDays = computed(() => {
  const days = [];
  const year = currentDate.value.getFullYear();
  const month = currentDate.value.getMonth();

  const daysInMonth = new Date(year, month + 1, 0).getDate();
  const firstDayIndex = new Date(year, month, 1).getDay();
  const lastDayIndex = new Date(year, month, daysInMonth).getDay();

  for (let i = 0; i < (firstDayIndex || 7) - 1; i++) {
    days.push({ empty: true });
  }

  for (let day = 1; day <= daysInMonth; day++) {
    const formattedDate = `${year}-${String(month + 1).padStart(
      2,
      '0'
    )}-${String(day).padStart(2, '0')}`;
    let booksForDay = booksByDate.value[formattedDate] || [];

    if (activeFilter.value) {
      booksForDay = booksForDay.filter(
        (book) => book.listType.id === activeFilter.value
      );
    }

    const isBooked = booksForDay.length > 0;
    const isToday = formattedDate === today.toISOString().split('T')[0];
    const isSelected = selectedDate.value === formattedDate;

    days.push({
      dayNumber: day,
      date: formattedDate,
      booked: isBooked,
      today: isToday,
      empty: false,
      selected: isSelected,
    });
  }

  for (let i = lastDayIndex; i < 6; i++) {
    days.push({ empty: true });
  }

  return days;
});

function prevMonth() {
  const newDate = new Date(currentDate.value);
  newDate.setMonth(newDate.getMonth() - 1);
  currentDate.value = newDate;
}

function nextMonth() {
  const newDate = new Date(currentDate.value);
  newDate.setMonth(newDate.getMonth() + 1);
  currentDate.value = newDate;
}

function goToToday() {
  currentDate.value = new Date(today);
  const todayDate = today.toISOString().split('T')[0];
  showBooksForDate(todayDate);
}

function showBooksForDate(date) {
  selectedDate.value = date;
}

const formatDisplayDate = (dateStr) => {
  const [year, month, day] = dateStr.split('-');
  return `${day}.${month}.${year}`;
};
</script>

<template>
  <div class="view-container">
    <fieldset class="calendar-container">
      <legend>Календарь книг</legend>
      <div class="month-navigation">
        <button @click="prevMonth">&lt; Предыдущий</button>
        <span id="current-month">{{ currentMonthDisplay }}</span>
        <button @click="nextMonth">Следующий &gt;</button>
      </div>
      <button id="go-to-today" @click="goToToday">Сегодня</button>
      <div class="calendar">
        <div
          v-for="(day, index) in calendarDays"
          :key="index"
          class="day"
          :class="{
            empty: day.empty,
            booked: day.booked,
            today: day.today,
            selected: day.selected,
          }"
          @click="!day.empty && showBooksForDate(day.date)"
        >
          {{ day.dayNumber }}
        </div>
      </div>
    </fieldset>
    <fieldset class="book-list-container">
      <legend>
        {{
          selectedDate
            ? `Книги за ${formatDisplayDate(selectedDate)}`
            : 'Прочитанные книги'
        }}
      </legend>
      <div class="filter-controls">
        <label>Фильтр:</label>
        <select v-model="activeFilter">
          <option
            v-for="filter in availableFilters"
            :key="filter.id"
            :value="filter.id"
          >
            {{ filter.name }}
          </option>
        </select>
      </div>
      <ul id="book-list">
        <li v-if="!selectedDate" class="book-item">
          Выберите дату, чтобы увидеть книги
        </li>
        <li v-else-if="filteredBooks.length === 0" class="book-item">
          В этот день не было добавлено книг
        </li>
        <li v-for="book in filteredBooks" :key="book.id" class="book-item">
          <span class="book-marker">·</span>
          <img
            :src="book.imageURL"
            :alt="book.title"
            class="book-image"
            @error="book.imageURL = '@/assets/default-book.png'"
          />
          <div class="book-info">
            <span class="book-title">{{ book.title }}</span>
            <span class="book-list" :class="'list-' + book.listType.id">{{
              book.listType.name
            }}</span>
          </div>
        </li>
      </ul>
    </fieldset>
  </div>
</template>

<style scoped>
.view-container {
  display: flex;
  margin-top: 20px;
}

.calendar-container {
  flex: 2;
  padding: 5px;
  margin-right: 20px;
  background-color: white;
  border: 2px solid forestgreen;
  border-radius: 8px;
}

legend {
  font-weight: bold;
}

.month-navigation {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 15px;
}

.month-navigation button {
  border: none;
  background: none;
  font-size: 16px;
}

.month-navigation button:hover {
  border-bottom: 1px solid darkgreen;
}

#go-to-today {
  display: block;
  margin: 10px auto;
  padding: 8px 12px;
  background: none;
  border: none;
  cursor: pointer;
  font-size: 14px;
}

#go-to-today:hover {
  border-bottom: 1px solid darkgreen;
}

.calendar {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  gap: 5px;
}

.calendar .day {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 50px;
  font-size: 14px;
  border: 1px solid #ccc;
  border-radius: 5px;
  background-color: white;
  cursor: pointer;
}

.calendar .day.empty {
  border: none;
  background-color: transparent;
  cursor: default;
}

.calendar .day.booked {
  background-color: forestgreen;
  color: white;
}

.calendar .day.selected {
  background-color: darkgreen;
  color: white;
  font-weight: bold;
}

.calendar .day:hover:not(.empty) {
  background-color: lightgray;
}

.book-list-container {
  flex: 1;
  padding: 5px;
  margin-right: 20px;
  background-color: white;
  border: 2px solid forestgreen;
  border-radius: 8px;
}

.filter-controls {
  display: flex;
  gap: 10px;
}

.filter-controls select {
  width: 300px;
}

#book-list {
  margin: 0;
  padding: 0;
}

.book-item {
  display: flex;
  align-items: center;
  gap: 10px;
  margin: 10px 0;
  padding: 8px;
  background-color: white;
  border-radius: 4px;
}

.book-marker {
  color: forestgreen;
  font-weight: bold;
}

.book-image {
  height: 100px;
  border-radius: 3px;
}

.book-info {
  display: flex;
  flex-direction: column;
}

.book-title {
  flex: 1;
}

.list-1 {
  color: #3498db;
}
.list-2 {
  color: #f39c12;
}
.list-3 {
  color: #e74c3c;
}
.list-4 {
  color: #2ecc71;
}
.list-5 {
  color: #9b59b6;
}
</style>
