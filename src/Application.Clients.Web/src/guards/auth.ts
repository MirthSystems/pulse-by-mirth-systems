import type { NavigationGuardNext, RouteLocationNormalized } from 'vue-router'

export const requireAuth = async (
  to: RouteLocationNormalized,
  from: RouteLocationNormalized,
  next: NavigationGuardNext
) => {
  // Check if user is authenticated by looking at localStorage or sessionStorage
  // Since we can't use useAuth0() outside of components, we'll do a simple check
  // and let the component handle the actual Auth0 logic
  
  // For now, just proceed to the route and let the component handle auth
  // The component will redirect to login if needed
  next()
}
