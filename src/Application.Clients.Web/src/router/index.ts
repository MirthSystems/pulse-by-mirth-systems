import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import { requireAuth } from '../guards/auth'
import analyticsService from '../services/analytics'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'Home',
      component: HomeView,
    },
    {
      path: '/search',
      name: 'Search',
      component: () => import('../views/SearchView.vue'),
    },
    {
      path: '/backoffice',
      name: 'Backoffice',
      component: () => import('../views/BackofficeView.vue'),
      beforeEnter: requireAuth,
      meta: {
        requiresAuth: true,
        title: 'Backoffice'
      }
    },
    {
      path: '/backoffice/venues',
      name: 'VenueManagement',
      component: () => import('../views/BackofficeView.vue'),
      beforeEnter: requireAuth,
      meta: {
        requiresAuth: true,
        title: 'Venue Management'
      }
    },
    {
      path: '/backoffice/venues/:id',
      name: 'BackofficeVenueDetail',
      component: () => import('../views/BackofficeVenueDetailView.vue'),
      beforeEnter: requireAuth,
      props: true,
      meta: {
        requiresAuth: true,
        title: 'Venue Detail'
      }
    },
    {
      path: '/backoffice/venues/:venueId/permissions',
      name: 'VenuePermissions',
      component: () => import('../views/VenuePermissionsView.vue'),
      beforeEnter: requireAuth,
      props: true,
      meta: {
        requiresAuth: true,
        title: 'Venue Permissions'
      }
    },
    {
      path: '/backoffice/venues/:venueId/specials/new',
      name: 'CreateSpecial',
      component: () => import('../views/BackofficeSpecialCreateView.vue'),
      beforeEnter: requireAuth,
      props: true,
      meta: {
        requiresAuth: true,
        title: 'Create Special'
      }
    },
    {
      path: '/backoffice/venues/:venueId/specials/:specialId',
      name: 'BackofficeSpecialDetail',
      component: () => import('../views/BackofficeSpecialEditView.vue'),
      beforeEnter: requireAuth,
      props: true,
      meta: {
        requiresAuth: true,
        title: 'Special Detail'
      }
    },
    {
      path: '/profile',
      name: 'Profile',
      component: () => import('../views/ProfileView.vue'),
      beforeEnter: requireAuth,
      meta: {
        requiresAuth: true,
        title: 'Profile Settings'
      }
    },
    {
      path: '/confirm',
      name: 'Confirm',
      component: () => import('../views/ConfirmView.vue'),
      beforeEnter: requireAuth,
      meta: {
        requiresAuth: true,
        title: 'Confirm Action'
      }
    },
    {
      path: '/callback',
      name: 'Callback',
      component: () => import('../views/CallbackView.vue'),
    },
  ],
})

// Global navigation tracking
router.beforeEach((to, from, next) => {
  // Track page view timing
  const startTime = performance.now()
  
  // Store start time for performance tracking
  to.meta.navigationStartTime = startTime
  
  next()
})

router.afterEach((to, from) => {
  // Track navigation performance
  if (to.meta.navigationStartTime) {
    const navigationTime = performance.now() - (to.meta.navigationStartTime as number)
    analyticsService.trackPerformance('navigation_time', navigationTime)
  }
  
  // Track page view
  setTimeout(() => {
    analyticsService.page({
      title: document.title,
      path: to.path,
      url: window.location.href,
      referrer: from.path || document.referrer,
      route_name: to.name as string,
      requires_auth: to.meta.requiresAuth || false
    })
  }, 100) // Small delay to ensure DOM is updated
})

export default router
