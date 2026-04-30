import { createRouter, createWebHistory } from 'vue-router';
import store from '@/store';
import userActivityService from '@/services/userActivityService';

const routes = [
  { path: '/', component: () => import('@/views/HomeView.vue') },
  { path: '/books', component: () => import('@/views/BooksView.vue') },
  {
    path: '/books/:id',
    component: () => import('@/views/BookPage.vue'),
    props: true,
    meta: {
      entityType: 'Книга',
      trackView: true,
    },
  },
  {
    path: '/collections',
    component: () => import('@/views/CollectionsView.vue'),
  },
  {
    path: '/collections/:id',
    component: () => import('@/views/CollectionPage.vue'),
    props: true,
    meta: {
      entityType: 'Подборка',
      trackView: true,
    },
  },
  { path: '/reviews', component: () => import('@/views/ReviewsView.vue') },
  {
    path: '/reviews/:id',
    component: () => import('@/views/ReviewPage.vue'),
    props: true,
    meta: {
      entityType: 'Рецензия',
      trackView: true,
    },
  },
  {
    path: '/admin',
    component: () => import('@/views/adminPages/AdminPage.vue'),
    meta: { requiresAuth: true, requiresAdmin: true },
    children: [
      {
        path: '',
        redirect: '/admin/management',
      },
      {
        path: 'management',
        component: () => import('@/views/adminPages/AdminManagement.vue'),
      },
      {
        path: 'collections',
        component: () => import('@/views/adminPages/AdminCollections.vue'),
      },
      {
        path: 'users',
        component: () => import('@/views/adminPages/AdminUsers.vue'),
      },
      {
        path: 'filter-words',
        component: () => import('@/views/adminPages/AdminFilterWords.vue'),
      },
    ],
  },
  {
    path: '/moder',
    component: () => import('@/views/moderPages/ModerPage.vue'),
    meta: { requiresAuth: true, requiresModer: true },
    children: [
      {
        path: '',
        redirect: '/moder/management',
      },
      {
        path: 'management',
        component: () => import('@/views/moderPages/ModerManagement.vue'),
      },
      {
        path: 'comments',
        component: () => import('@/views/moderPages/ModerComments.vue'),
      },
      {
        path: 'users',
        component: () => import('@/views/moderPages/ModerUsers.vue'),
      },
    ],
  },
  {
    path: '/profile',
    component: () => import('@/views/UserProfile.vue'),
    meta: { requiresAuth: true },
  },
  {
    path: '/settings',
    component: () => import('@/views/UserSettings.vue'),
    meta: { requiresAuth: true },
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

router.beforeEach((to, from, next) => {
  const isAuthenticated = store.getters['auth/isAuthenticated'];
  const user = store.getters['auth/user'];
  const userRole = user?.nameRole;

  if (to.matched.some((record) => record.meta.requiresAuth)) {
    if (!isAuthenticated) {
      console.warn('Пользователь не авторизован.');
      router.push('/');
      return;
    }

    if (
      to.matched.some((record) => record.meta.requiresAdmin) &&
      userRole !== 'Администратор'
    ) {
      console.warn('Пользователь не имеет достаточных прав.');
      router.push('/');
      return;
    }

    if (
      to.matched.some((record) => record.meta.requiresModer) &&
      userRole !== 'Модератор'
    ) {
      console.warn('Пользователь не имеет достаточных прав.');
      router.push('/');
      return;
    }
  }

  next();
});

router.beforeEach(async (to, from) => {
  if (!to.meta.trackView) {
    return;
  }

  const isAuthenticated = store.getters['auth/isAuthenticated'];
  const user = store.getters['auth/user'];

  if (!isAuthenticated || !user?.idUser) {
    console.warn('Пользователь не авторизован или данные отсутствуют.');
    return;
  }

  const idUser = user.idUser;
  const idEntity = to.params.id ? parseInt(to.params.id) : null;
  const typeEntity = to.meta.entityType || null;

  if (!idEntity || !typeEntity) {
    console.log('Нет данных для добавления просмотра.');
    return;
  }

  try {
    await userActivityService.addView(idUser, idEntity, typeEntity);
    console.log('Просмотр добавлен успешно.');
  } catch (error) {
    console.error('Ошибка при добавлении просмотра:', error);
  }
});

export default router;
