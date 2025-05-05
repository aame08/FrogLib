import axios from 'axios';
import store from '@/store';

const apiClient = axios.create({
  baseURL: 'https://localhost:7157/api/UserActivity',
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
  addView(idUser, idEntity, typeEntity) {
    return apiClient.post(`/add-view/${idUser}/${idEntity}/${typeEntity}`);
  },
  getBookRating(idUser, idBook) {
    return apiClient
      .get(`/get-book-rating/${idUser}/${idBook}`)
      .then((response) => response.data);
  },
  updateBookRating(idUser, idBook, newRating) {
    return apiClient.post(
      `/update-book-rating/${idUser}/${idBook}/${newRating}`
    );
  },
  deleteBookRating(idUser, idBook) {
    return apiClient.delete(`/delete-book-rating/${idUser}/${idBook}`);
  },
  getUserList(idUser, idBook) {
    return apiClient
      .get(`/get-user-list/${idUser}/${idBook}`)
      .then((response) => response.data);
  },
  updateUserList(idUser, idBook, idList) {
    return apiClient.post(`/update-user-list/${idUser}/${idBook}/${idList}`);
  },
  deleteUserList(idUser, idBook) {
    return apiClient.delete(`/delete-user-list/${idUser}/${idBook}`);
  },
  getLikedCollection(idUser, idCollection) {
    return apiClient
      .get(`/get-liked-collection/${idUser}/${idCollection}`)
      .then((response) => response.data);
  },
  likeCollection(idUser, idCollection) {
    return apiClient.post(`/like-collection/${idUser}/${idCollection}`);
  },
  unlikeCollection(idUser, idCollection) {
    return apiClient.delete(`/unlike-collection/${idUser}/${idCollection}`);
  },
  getEntityRating(idUser, idEntity, typeEntity) {
    return apiClient
      .get(`/get-entity-rating/${idUser}/${idEntity}/${typeEntity}`)
      .then((response) => response.data);
  },
  updateEntityRating(idUser, idEntity, typeEntity, newRating) {
    return apiClient.post(
      `/update-entity-rating/${idUser}/${idEntity}/${typeEntity}/${newRating}`
    );
  },
  addNewComment(idUser, idEntity, typeEntity, textComment) {
    return apiClient.post(
      `/add-new-comment/${idUser}/${idEntity}/${typeEntity}/${textComment}`
    );
  },
  addReplyComment(idUser, idEntity, typeEntity, textComment, parentId) {
    return apiClient.post(
      `/add-reply-comment/${idUser}/${idEntity}/${typeEntity}/${textComment}/${parentId}`
    );
  },
  addReview(idUser, idBook, titleReview, textReview, newRating) {
    return apiClient.post(`/add-review`, {
      idUser,
      idBook,
      titleReview,
      textReview,
      newRating,
    });
  },
  editReview(idReview, idUser, idBook, titleReview, textReview, newRating) {
    return apiClient.put(`/edit-review?idReview=${idReview}`, {
      idUser,
      idBook,
      titleReview,
      textReview,
      newRating,
    });
  },
  addCollection(collectionRequest) {
    return apiClient.post('/add-collection', collectionRequest);
  },
  editCollection(idCollection, collectionRequest) {
    return apiClient.put(`/edit-collection/${idCollection}`, collectionRequest);
  },
  getUserCollections(idUser) {
    return apiClient
      .get(`/user-collections/${idUser}`)
      .then((response) => response.data);
  },
  getUserLikedCollections(idUser) {
    return apiClient
      .get(`/user-liked-collections/${idUser}`)
      .then((response) => response.data);
  },
  getUserBooks(idUser) {
    return apiClient
      .get(`/user-books/${idUser}`)
      .then((response) => response.data);
  },
  getUserComments(idUser) {
    return apiClient
      .get(`/user-comments/${idUser}`)
      .then((response) => response.data);
  },
  updateUserComment(idUser, idComment, textComment) {
    return apiClient.put(
      `/update-comment/${idUser}/${idComment}/${textComment}`
    );
  },
  deleteUserComment(idUser, idComment) {
    return apiClient.delete(`/delete-comment/${idUser}/${idComment}`);
  },
  getUserReviews(idUser) {
    return apiClient
      .get(`/user-reviews/${idUser}`)
      .then((response) => response.data);
  },
  getUserCalendar(idUser) {
    return apiClient
      .get(`/user-calendar/${idUser}`)
      .then((response) => response.data);
  },
};
