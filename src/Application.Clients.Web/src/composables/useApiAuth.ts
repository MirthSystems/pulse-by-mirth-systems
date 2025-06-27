import { watch, onMounted, nextTick, getCurrentInstance } from 'vue'
import { useAuth0 } from '@auth0/auth0-vue'
import apiService from '@/services/api'

export function useApiAuth() {
  // Only call useAuth0 if we're inside a component
  const instance = getCurrentInstance()
  if (!instance) {
    console.warn('useApiAuth must be called inside a Vue component')
    return {
      updateApiToken: async () => {
        console.warn('Auth0 not available outside component context')
      }
    }
  }

  const auth0 = useAuth0()

  const updateApiToken = async () => {
    const { getAccessTokenSilently, isAuthenticated, isLoading } = auth0

    if (isAuthenticated.value && !isLoading.value) {
      try {
        const token = await getAccessTokenSilently()
        apiService.setAccessToken(token)
        console.log('API token updated successfully')
      } catch (error) {
        console.warn('Failed to get access token:', error)
        apiService.setAccessToken(null)
      }
    } else {
      apiService.setAccessToken(null)
    }
  }

  const setupWatchers = () => {
    const { isAuthenticated, isLoading } = auth0

    // Watch for authentication changes
    watch([isAuthenticated, isLoading], () => {
      if (!isLoading.value) {
        updateApiToken()
      }
    })

    // Initialize on mount
    if (!isLoading.value) {
      updateApiToken()
    }
  }

  // Setup watchers with next tick to ensure Auth0 is initialized
  onMounted(() => {
    nextTick(() => {
      setupWatchers()
    })
  })

  return {
    updateApiToken
  }
}
