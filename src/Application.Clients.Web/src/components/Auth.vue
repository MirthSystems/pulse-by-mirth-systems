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
    <UserProfileDropdown v-else />
  </div>
</template>

<script setup lang="ts">
import { onMounted, watch } from 'vue'
import { storeToRefs } from 'pinia'
import { useAuth0 } from '@auth0/auth0-vue'
import { UserIcon } from '@heroicons/vue/24/outline'
import { useAuthStore } from '../stores/auth'
import UserProfileDropdown from './UserProfileDropdown.vue'

const { 
  loginWithRedirect, 
  user: auth0User, 
  isAuthenticated: auth0IsAuthenticated, 
  isLoading: auth0IsLoading,
  error: auth0Error
} = useAuth0()

const authStore = useAuthStore()
const { isAuthenticated, isLoading } = storeToRefs(authStore)

const handleLogin = () => {
  loginWithRedirect()
}

// Initialize auth state when component mounts
onMounted(() => {
  authStore.setLoading(auth0IsLoading.value)
})

// Watch for auth state changes
watch(auth0IsLoading, (loading) => {
  authStore.setLoading(loading)
})

watch(auth0IsAuthenticated, (authenticated) => {
  if (authenticated && auth0User.value) {
    authStore.setUser(auth0User.value)
  } else {
    authStore.setUser(null)
  }
})

watch(auth0User, (user) => {
  if (user && auth0IsAuthenticated.value) {
    authStore.setUser(user)
  }
}, { deep: true })

watch(auth0Error, (error) => {
  if (error) {
    authStore.setError(error.message)
    console.error('Auth0 error:', error)
  }
})
</script>
