import axios from 'axios';
import store from '@/store';

const apiClient = axios.create({
  baseURL: 'https://localhost:7157/api/Books',
  headers: {
    'Content-Type': 'application/json',
  },
});

export default {
  getListTypes() {
    return apiClient.get('/list-types').then((response) => response.data);
  },
  getCategories() {
    return apiClient.get('/categories').then((response) => response.data);
  },
  getBookData(id) {
    return apiClient.get(`books/${id}`).then((response) => response.data);
  },
  getAllBooks() {
    return apiClient.get('/all-books').then((response) => response.data);
  },
  getNewBooks() {
    return apiClient.get('/new-books').then((response) => response.data);
  },
  getBestSelling() {
    return apiClient.get('/best-selling').then((response) => response.data);
  },
  getPopularBooks() {
    return apiClient.get('/popular-books').then((response) => response.data);
  },
  getRecommendations(idUser) {
    const authApiClient = axios.create({
      baseURL: 'https://localhost:7157/api/Books',
    });

    authApiClient.interceptors.request.use((config) => {
      const accessToken = store.getters['auth/accessToken'];
      if (accessToken) {
        config.headers.Authorization = `Bearer ${accessToken}`;
      }
      return config;
    });

    return authApiClient
      .get(`/recommendations/${idUser}`)
      .then((response) => response.data);
  },
};
