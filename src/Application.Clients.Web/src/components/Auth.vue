<template>
  <div class="auth-component">
    <!-- Loading state -->
    <div v-if="isLoading" class="flex items-center space-x-2">
      <div class="animate-spin rounded-full h-4 w-4 border-b-2 border-blue-600"></div>
      <span class="text-sm text-gray-600">Loading...</span>
    </div>
    
    <!-- Login button when not authenticated -->
    <button
      v-else-if="!isAuthenticated"
      @click="handleLogin"
      class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 transition-colors"
    >
      <UserIcon class="mr-2 h-4 w-4" />
      Sign In
    </button>
    
    <!-- User profile dropdown when authenticated -->
    <div v-else class="flex items-center space-x-2">
      <NotificationIcon />
      <UserProfileDropdown />
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, watch } from 'vue'
import { storeToRefs } from 'pinia'
import { useAuth0 } from '@auth0/auth0-vue'
import { UserIcon } from '@heroicons/vue/24/outline'
import { useAuthStore } from '../stores/auth'
import UserProfileDropdown from '../components/UserProfileDropdown.vue'
import NotificationIcon from '../components/NotificationIcon.vue'
import apiService from '../services/api'

const { 
  loginWithRedirect, 
  user: auth0User, 
  isAuthenticated: auth0IsAuthenticated, 
  isLoading: auth0IsLoading,
  error: auth0Error,
  getAccessTokenSilently
} = useAuth0()

const authStore = useAuthStore()
const { isAuthenticated, isLoading } = storeToRefs(authStore)

const handleLogin = () => {
  loginWithRedirect()
}

// Function to sync user with backend
const syncUserWithBackend = async () => {
  if (auth0User.value && apiService.hasAccessToken()) {
    try {
      const userInfo = {
        sub: auth0User.value.sub,
        email: auth0User.value.email,
        name: auth0User.value.name,
        nickname: auth0User.value.nickname,
        picture: auth0User.value.picture,
        emailVerified: auth0User.value.email_verified || false
      }
      
      console.debug('Syncing user with backend:', userInfo)
      const response = await apiService.syncUser(userInfo)
      
      if (response.success) {
        console.debug('User synced successfully:', response.data)
      } else {
        console.error('Failed to sync user:', response)
      }
    } catch (error) {
      console.error('Error syncing user with backend:', error)
    }
  }
}

// Function to get and set access token
const updateAccessToken = async () => {
  if (auth0IsAuthenticated.value) {
    try {
      const token = await getAccessTokenSilently()
      apiService.setAccessToken(token)
      authStore.setAccessToken(token)
      console.debug('Access token updated, hasToken:', !!token, 'tokenStart:', token?.substring(0, 20) + '...')
      
      // Sync user with backend after setting token
      await syncUserWithBackend()
    } catch (error) {
      console.error('Failed to get access token:', error)
      apiService.setAccessToken(null)
      authStore.setAccessToken(null)
    }
  } else {
    apiService.setAccessToken(null)
    authStore.setAccessToken(null)
  }
}

// Initialize auth state when component mounts
onMounted(async () => {
  authStore.setLoading(auth0IsLoading.value)
  
  // If already authenticated on mount, get the access token
  if (auth0IsAuthenticated.value) {
    await updateAccessToken()
  }
})

// Watch for auth state changes
watch(auth0IsLoading, (loading) => {
  authStore.setLoading(loading)
})

watch(auth0IsAuthenticated, async (authenticated) => {
  if (authenticated && auth0User.value) {
    authStore.setUser(auth0User.value)
    await updateAccessToken()
  } else {
    authStore.setUser(null)
    apiService.setAccessToken(null)
    authStore.setAccessToken(null)
  }
})

watch(auth0User, async (user) => {
  if (user && auth0IsAuthenticated.value) {
    authStore.setUser(user)
    await updateAccessToken()
  }
}, { deep: true })

watch(auth0Error, (error) => {
  if (error) {
    authStore.setError(error.message)
    console.error('Auth0 error:', error)
  }
})
</script>
