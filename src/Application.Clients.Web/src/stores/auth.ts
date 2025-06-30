import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export interface User {
  email?: string
  email_verified?: boolean
  family_name?: string
  given_name?: string
  locale?: string
  name?: string
  nickname?: string
  picture?: string
  sub?: string
  updated_at?: string
}

export const useAuthStore = defineStore('auth', () => {
  const user = ref<User | null>(null)
  const isAuthenticated = ref(false)
  const isLoading = ref(true)
  const error = ref<string | null>(null)
  const accessToken = ref<string | null>(null)

  const displayName = computed(() => {
    if (!user.value) return ''
    return user.value.name || user.value.nickname || user.value.email || 'User'
  })

  const userInitials = computed(() => {
    if (!user.value) return 'U'
    const name = displayName.value
    const parts = name.split(' ')
    if (parts.length >= 2) {
      return (parts[0][0] + parts[1][0]).toUpperCase()
    }
    return name[0]?.toUpperCase() || 'U'
  })

  function setUser(userData: User | null) {
    user.value = userData
    isAuthenticated.value = !!userData
  }

  function setLoading(loading: boolean) {
    isLoading.value = loading
  }

  function setError(errorMessage: string | null) {
    error.value = errorMessage
  }

  function setAccessToken(token: string | null) {
    accessToken.value = token
  }

  function logout() {
    user.value = null
    isAuthenticated.value = false
    error.value = null
    accessToken.value = null
  }

  return {
    user,
    isAuthenticated,
    isLoading,
    error,
    accessToken,
    displayName,
    userInitials,
    setUser,
    setLoading,
    setError,
    setAccessToken,
    logout
  }
})
