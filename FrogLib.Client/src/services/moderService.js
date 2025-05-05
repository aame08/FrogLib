import axios from 'axios';
import store from '@/store';

const apiClient = axios.create({
  baseURL: 'https://localhost:7157/api/Moder/moder',
  headers: {
    'Content-Type': 'application/json',
  },
});

apiClient.interceptors.request.use(
  (config) => {
    const accessToken = store.getters['auth/accessToken'];
    if (accessToken) {
      config.headers.Authorization = `Bearer ${accessToken}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

export default {
  getModerReviews() {
    return apiClient.get('/reviews').then((response) => response.data);
  },
  getModerCollections() {
    return apiClient.get('/collections').then((response) => response.data);
  },
  updateReviewStatus(idReview, status) {
    return apiClient.put(`/update-review-status/${idReview}/${status}`);
  },
  updateCollectionStatus(idCollection, status) {
    return apiClient.put(`/update-collection-status/${idCollection}/${status}`);
  },
  addViolation(violationDTO) {
    return apiClient.post('/add-violation', violationDTO);
  },
  getComments() {
    return apiClient.get('/comments').then((response) => response.data);
  },
  getCommentsForEntity(typeEntity, idEntity) {
    return apiClient
      .get(`/comments/${typeEntity}/${idEntity}`)
      .then((response) => response.data);
  },
  deleteComment(idComment, violationDTO) {
    return apiClient.post(`/delete-comment/${idComment}`, violationDTO);
  },
  getModerUsers() {
    return apiClient.get('/users').then((response) => response.data);
  },
  getUserViolations(idUser) {
    return apiClient
      .get(`/user-violations/${idUser}`)
      .then((response) => response.data);
  },
  changeStatusUser(idUser, status) {
    return apiClient.put(`/change-status-user/${idUser}/${status}`);
  },
};
