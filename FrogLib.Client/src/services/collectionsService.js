import axios from 'axios';

const apiClient = axios.create({
  baseURL: 'https://localhost:7157/api/Collections',
  headers: {
    'Content-Type': 'application/json',
  },
});

export default {
  getCollectionData(id) {
    return apiClient
      .get(`/collections/${id}`)
      .then((response) => response.data);
  },
  getAllCollections() {
    return apiClient.get('/all-collections').then((response) => response.data);
  },
  getPopularCollections() {
    return apiClient
      .get('/popular-collections')
      .then((response) => response.data);
  },
};
