import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'Home',
      component: HomeView,
    },
    {
      path: '/venues',
      name: 'Venues',
      component: () => import('../views/VenuesView.vue'),
    },
    {
      path: '/venues/:id',
      name: 'VenueDetail',
      component: () => import('../views/VenueDetailView.vue'),
      props: true,
    },
    {
      path: '/specials',
      name: 'Specials',
      component: () => import('../views/SpecialsView.vue'),
    },
    {
      path: '/specials/:id',
      name: 'SpecialDetail',
      component: () => import('../views/SpecialDetailView.vue'),
      props: true,
    },
    {
      path: '/search',
      name: 'Search',
      component: () => import('../views/SearchView.vue'),
    },
    {
      path: '/about',
      name: 'About',
      component: () => import('../views/AboutView.vue'),
    },
  ],
})

export default router
