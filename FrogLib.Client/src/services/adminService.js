import axios from 'axios';
import store from '@/store';

const apiClient = axios.create({
  baseURL: 'https://localhost:7157/api/Admin/admin',
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
  getAdminBooks() {
    return apiClient.get('/all-books').then((response) => response.data);
  },
  adminSearchBook(title) {
    return apiClient
      .get(`/search?title=${title}`)
      .then((response) => response.data);
  },
  adminAddBook(bookDTO) {
    return apiClient.post('/add-book', bookDTO);
  },
  adminEditBook(idBook, bookDTO) {
    return apiClient.put(`/edit-book/${idBook}`, bookDTO);
  },
  getAdminAuthors() {
    return apiClient.get('/authors').then((response) => response.data);
  },
  adminEditAuthor(idAuthor, authorDTO) {
    return apiClient.put(`/edit-author/${idAuthor}`, authorDTO);
  },
  adminDeleteAuthor(idAuthor) {
    return apiClient.delete(`/delete-author/${idAuthor}`);
  },
  getAdminCategories() {
    return apiClient.get('/categories').then((response) => response.data);
  },
  adminEditCategory(idCategory, newNameCategory) {
    return apiClient.put(`/edit-category/${idCategory}/${newNameCategory}`);
  },
  adminDeleteCategory(idCategory) {
    return apiClient.delete(`/delete-category/${idCategory}`);
  },
  getStaff(idUser) {
    return apiClient.get(`/staff/${idUser}`).then((response) => response.data);
  },
  addEmployee(employeeDTO) {
    return apiClient.post(`/add-employee`, employeeDTO);
  },
  adminChangeRole(idUser, nameRole) {
    return apiClient.put(`/change-role/${idUser}/${nameRole}`);
  },
  adminDeleteEmployee(idUser) {
    return apiClient.put(`/delete-employee/${idUser}`);
  },
};
