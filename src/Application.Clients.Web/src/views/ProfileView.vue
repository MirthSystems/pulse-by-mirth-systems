<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Header -->
    <div class="bg-white shadow">
      <div class="px-4 sm:px-6 lg:px-8 py-4 sm:py-6">
        <div class="flex flex-col space-y-4 sm:flex-row sm:items-center sm:justify-between sm:space-y-0">
          <div class="flex items-center">
            <button
              @click="$router.go(-1)"
              class="mr-3 sm:mr-4 p-2 text-gray-400 hover:text-gray-600 flex-shrink-0"
            >
              <ChevronLeftIcon class="h-5 w-5" />
            </button>
            <div class="min-w-0">
              <h1 class="text-xl sm:text-2xl font-bold text-gray-900 break-words">Profile Settings</h1>
              <p class="mt-1 text-sm text-gray-500 break-words">
                View and manage your profile information
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Content -->
    <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-6 sm:py-8">
      <div v-if="loading" class="flex items-center justify-center py-12">
        <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
        <span class="ml-2 text-gray-600">Loading profile...</span>
      </div>

      <div v-else-if="error" class="bg-red-50 border border-red-200 rounded-md p-4">
        <div class="flex">
          <ExclamationTriangleIcon class="h-5 w-5 text-red-400" />
          <div class="ml-3">
            <h3 class="text-sm font-medium text-red-800">Error loading profile</h3>
            <p class="mt-1 text-sm text-red-600">{{ error }}</p>
          </div>
        </div>
      </div>

      <div v-else class="space-y-6">
        <!-- Profile Information -->
        <div class="bg-white shadow rounded-lg overflow-hidden">
          <div class="px-4 sm:px-6 py-4 border-b border-gray-200">
            <h3 class="text-base sm:text-lg leading-6 font-medium text-gray-900 break-words">Profile Information</h3>
            <p class="mt-1 text-sm text-gray-500 break-words">Your account details from the authentication token</p>
          </div>
          
          <div class="p-4 sm:p-6">
            <div class="grid grid-cols-1 lg:grid-cols-2 gap-4 sm:gap-6">
              <!-- Profile Picture -->
              <div class="lg:col-span-2 flex flex-col space-y-4 sm:flex-row sm:items-center sm:space-y-0 sm:space-x-6">
                <div class="flex-shrink-0 self-center sm:self-start">
                  <img 
                    v-if="userProfile?.picture" 
                    :src="userProfile.picture" 
                    :alt="userProfile.name || 'Profile picture'"
                    class="h-16 w-16 sm:h-20 sm:w-20 rounded-full object-cover"
                  />
                  <div 
                    v-else 
                    class="h-16 w-16 sm:h-20 sm:w-20 rounded-full bg-gray-300 flex items-center justify-center"
                  >
                    <UserIcon class="h-8 w-8 sm:h-10 sm:w-10 text-gray-500" />
                  </div>
                </div>
                <div class="text-center sm:text-left min-w-0 flex-1">
                  <h3 class="text-base sm:text-lg font-medium text-gray-900 break-words">{{ userProfile?.name || 'No name available' }}</h3>
                  <p class="text-sm text-gray-500 break-words">{{ userProfile?.email || 'No email available' }}</p>
                </div>
              </div>

              <!-- Name -->
              <div class="lg:col-span-1">
                <label class="block text-sm font-medium text-gray-700">Full Name</label>
                <div class="mt-1 text-sm text-gray-900 bg-gray-50 border border-gray-300 rounded-md px-3 py-2 break-words min-h-[2.5rem] flex items-center">
                  {{ userProfile?.name || 'Not available' }}
                </div>
              </div>

              <!-- Email -->
              <div class="lg:col-span-1">
                <label class="block text-sm font-medium text-gray-700">Email Address</label>
                <div class="mt-1 text-sm text-gray-900 bg-gray-50 border border-gray-300 rounded-md px-3 py-2 break-words min-h-[2.5rem] flex items-center">
                  {{ userProfile?.email || 'Not available' }}
                </div>
              </div>

              <!-- Nickname -->
              <div class="lg:col-span-1">
                <label class="block text-sm font-medium text-gray-700">Nickname</label>
                <div class="mt-1 text-sm text-gray-900 bg-gray-50 border border-gray-300 rounded-md px-3 py-2 break-words min-h-[2.5rem] flex items-center">
                  {{ userProfile?.nickname || 'Not available' }}
                </div>
              </div>

              <!-- Email Verified -->
              <div class="lg:col-span-1">
                <label class="block text-sm font-medium text-gray-700">Email Verified</label>
                <div class="mt-1 flex items-center">
                  <span 
                    :class="[
                      'inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium',
                      userProfile?.email_verified ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
                    ]"
                  >
                    {{ userProfile?.email_verified ? 'Verified' : 'Not Verified' }}
                  </span>
                </div>
              </div>

              <!-- Last Updated -->
              <div class="lg:col-span-1">
                <label class="block text-sm font-medium text-gray-700">Last Updated</label>
                <div class="mt-1 text-sm text-gray-900 bg-gray-50 border border-gray-300 rounded-md px-3 py-2 break-words min-h-[2.5rem] flex items-center">
                  {{ formatDate(userProfile?.updated_at) || 'Not available' }}
                </div>
              </div>

              <!-- User ID -->
              <div class="lg:col-span-2">
                <label class="block text-sm font-medium text-gray-700">User ID</label>
                <div class="mt-1 text-xs sm:text-sm text-gray-900 bg-gray-50 border border-gray-300 rounded-md px-3 py-2 font-mono break-all min-h-[2.5rem] flex items-center">
                  {{ userProfile?.sub || 'Not available' }}
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Token Information -->
        <div class="bg-white shadow rounded-lg overflow-hidden">
          <div class="px-4 sm:px-6 py-4 border-b border-gray-200">
            <h3 class="text-base sm:text-lg leading-6 font-medium text-gray-900 break-words">Authentication Details</h3>
            <p class="mt-1 text-sm text-gray-500 break-words">Information about your current session</p>
          </div>
          
          <div class="p-4 sm:p-6">
            <div class="grid grid-cols-1 lg:grid-cols-2 gap-4 sm:gap-6">
              <!-- Token Issued -->
              <div class="lg:col-span-1">
                <label class="block text-sm font-medium text-gray-700">Token Issued</label>
                <div class="mt-1 text-sm text-gray-900 bg-gray-50 border border-gray-300 rounded-md px-3 py-2 break-words min-h-[2.5rem] flex items-center">
                  {{ formatDate(tokenInfo?.iat) || 'Not available' }}
                </div>
              </div>

              <!-- Token Expires -->
              <div class="lg:col-span-1">
                <label class="block text-sm font-medium text-gray-700">Token Expires</label>
                <div class="mt-1 text-sm text-gray-900 bg-gray-50 border border-gray-300 rounded-md px-3 py-2 break-words min-h-[2.5rem] flex items-center">
                  {{ formatDate(tokenInfo?.exp) || 'Not available' }}
                </div>
              </div>

              <!-- Issuer -->
              <div class="lg:col-span-2">
                <label class="block text-sm font-medium text-gray-700">Issuer</label>
                <div class="mt-1 text-sm text-gray-900 bg-gray-50 border border-gray-300 rounded-md px-3 py-2 break-words min-h-[2.5rem] flex items-center">
                  {{ tokenInfo?.iss || 'Not available' }}
                </div>
              </div>

              <!-- Audience -->
              <div class="lg:col-span-2">
                <label class="block text-sm font-medium text-gray-700">Audience</label>
                <div class="mt-1 text-sm text-gray-900 bg-gray-50 border border-gray-300 rounded-md px-3 py-2 break-words min-h-[2.5rem] flex items-center">
                  {{ Array.isArray(tokenInfo?.aud) ? tokenInfo.aud.join(', ') : tokenInfo?.aud || 'Not available' }}
                </div>
              </div>

              <!-- Permissions -->
              <div class="lg:col-span-2">
                <label class="block text-sm font-medium text-gray-700">Permissions</label>
                <div class="mt-1">
                  <div v-if="tokenInfo?.permissions && tokenInfo.permissions.length > 0" class="flex flex-wrap gap-2">
                    <span 
                      v-for="permission in tokenInfo.permissions" 
                      :key="permission"
                      class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-blue-100 text-blue-800 break-words"
                    >
                      {{ permission }}
                    </span>
                  </div>
                  <div v-else class="text-sm text-gray-500 bg-gray-50 border border-gray-300 rounded-md px-3 py-2 min-h-[2.5rem] flex items-center">
                    No specific permissions assigned
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Actions -->
        <div class="bg-white shadow rounded-lg overflow-hidden">
          <div class="px-4 sm:px-6 py-4 border-b border-gray-200">
            <h3 class="text-base sm:text-lg leading-6 font-medium text-gray-900 break-words">Account Actions</h3>
            <p class="mt-1 text-sm text-gray-500 break-words">Manage your account and session</p>
          </div>
          
          <div class="p-4 sm:p-6">
            <div class="flex flex-col space-y-3 sm:flex-row sm:space-y-0 sm:space-x-4">
              <button
                @click="refreshToken"
                :disabled="refreshing"
                class="inline-flex items-center justify-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-50 w-full sm:w-auto"
              >
                <ArrowPathIcon class="-ml-1 mr-2 h-4 w-4 flex-shrink-0" />
                <span class="break-words">{{ refreshing ? 'Refreshing...' : 'Refresh Token' }}</span>
              </button>
              
              <button
                @click="signOut"
                class="inline-flex items-center justify-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-red-600 hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500 w-full sm:w-auto"
              >
                <ArrowRightOnRectangleIcon class="-ml-1 mr-2 h-4 w-4 flex-shrink-0" />
                <span class="break-words">Sign Out</span>
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { 
  ChevronLeftIcon, 
  UserIcon, 
  ExclamationTriangleIcon,
  ArrowPathIcon,
  ArrowRightOnRectangleIcon
} from '@heroicons/vue/24/outline'
import { useAuth0 } from '@auth0/auth0-vue'

const router = useRouter()
const { user, getAccessTokenSilently, logout } = useAuth0()

const loading = ref(true)
const error = ref<string | null>(null)
const refreshing = ref(false)
const userProfile = ref<any>(null)
const tokenInfo = ref<any>(null)

const formatDate = (timestamp: number | string | undefined) => {
  if (!timestamp) return null
  
  let date: Date
  if (typeof timestamp === 'number') {
    // Unix timestamp (seconds)
    date = new Date(timestamp * 1000)
  } else {
    date = new Date(timestamp)
  }
  
  if (isNaN(date.getTime())) return null
  
  return date.toLocaleString()
}

const loadProfile = async () => {
  try {
    loading.value = true
    error.value = null

    // Get user profile from Auth0
    if (user.value) {
      userProfile.value = user.value
    }

    // Get token claims from access token if available
    try {
      const accessToken = await getAccessTokenSilently()
      if (accessToken) {
        // Parse JWT token to get claims (basic parsing without verification)
        const tokenParts = accessToken.split('.')
        if (tokenParts.length === 3) {
          const payload = JSON.parse(atob(tokenParts[1]))
          tokenInfo.value = payload
        }
      }
    } catch (tokenError) {
      console.warn('Could not get access token:', tokenError)
    }

  } catch (err) {
    console.error('Error loading profile:', err)
    error.value = err instanceof Error ? err.message : 'Failed to load profile'
  } finally {
    loading.value = false
  }
}

const refreshToken = async () => {
  try {
    refreshing.value = true
    await getAccessTokenSilently({ cacheMode: 'off' })
    await loadProfile()
  } catch (err) {
    console.error('Error refreshing token:', err)
    error.value = 'Failed to refresh token'
  } finally {
    refreshing.value = false
  }
}

const signOut = () => {
  logout({
    logoutParams: {
      returnTo: window.location.origin
    }
  })
}

onMounted(() => {
  loadProfile()
})
</script>
