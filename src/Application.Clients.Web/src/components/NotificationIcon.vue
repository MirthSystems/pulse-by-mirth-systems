<template>
  <div class="relative">
    <button
      @click="toggleNotifications"
      class="p-2 text-gray-400 hover:text-gray-600 transition-colors relative"
      :class="{ 'text-blue-600': hasNotifications }"
    >
      <BellIcon class="h-6 w-6" />
      
      <!-- Notification Badge -->
      <span
        v-if="notificationCount > 0"
        class="absolute top-0 right-0 inline-flex items-center justify-center px-2 py-1 text-xs font-bold leading-none text-white bg-red-600 rounded-full transform translate-x-1/2 -translate-y-1/2"
      >
        {{ notificationCount > 99 ? '99+' : notificationCount }}
      </span>
    </button>

    <!-- Notifications Dropdown -->
    <Transition
      enter-active-class="transition ease-out duration-100"
      enter-from-class="transform opacity-0 scale-95"
      enter-to-class="transform opacity-100 scale-100"
      leave-active-class="transition ease-in duration-75"
      leave-from-class="transform opacity-100 scale-100"
      leave-to-class="transform opacity-0 scale-95"
    >
      <div
        v-if="isOpen"
        class="absolute right-0 mt-2 w-80 bg-white rounded-md shadow-lg ring-1 ring-black ring-opacity-5 focus:outline-none z-50"
      >
        <div class="py-1">
          <!-- Header -->
          <div class="px-4 py-3 border-b border-gray-200">
            <h3 class="text-sm font-medium text-gray-900">Notifications</h3>
          </div>
          
          <!-- Loading State -->
          <div v-if="loading" class="px-4 py-8 text-center">
            <div class="animate-spin rounded-full h-6 w-6 border-b-2 border-blue-600 mx-auto"></div>
            <p class="text-sm text-gray-500 mt-2">Loading notifications...</p>
          </div>
          
          <!-- No Notifications -->
          <div v-else-if="invitations.length === 0" class="px-4 py-8 text-center">
            <BellSlashIcon class="h-8 w-8 text-gray-400 mx-auto mb-2" />
            <p class="text-sm text-gray-500">No notifications</p>
          </div>
          
          <!-- Invitations List -->
          <div v-else class="max-h-96 overflow-y-auto">
            <div
              v-for="invitation in invitations"
              :key="invitation.id"
              class="px-4 py-3 border-b border-gray-100 last:border-b-0"
            >
              <div class="flex items-start space-x-3">
                <div class="flex-shrink-0">
                  <BuildingStorefrontIcon class="h-6 w-6 text-blue-500" />
                </div>
                <div class="flex-1 min-w-0">
                  <p class="text-sm font-medium text-gray-900">
                    Venue Invitation
                  </p>
                  <p class="text-sm text-gray-600">
                    You've been invited to join <strong>{{ invitation.venueName }}</strong> 
                    as {{ getPermissionDisplayName(invitation.name) }}
                  </p>
                  <p class="text-xs text-gray-500 mt-1">
                    From {{ invitation.invitedByUserEmail }} • {{ formatDate(invitation.invitedAt) }}
                  </p>
                  
                  <!-- Action Buttons -->
                  <div class="flex space-x-2 mt-3">
                    <button
                      @click="acceptInvitation(invitation.id)"
                      :disabled="processingInvitation === invitation.id"
                      class="inline-flex items-center px-3 py-1 border border-transparent text-xs font-medium rounded text-white bg-green-600 hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500 disabled:opacity-50"
                    >
                      <CheckIcon class="h-3 w-3 mr-1" />
                      Accept
                    </button>
                    <button
                      @click="declineInvitation(invitation.id)"
                      :disabled="processingInvitation === invitation.id"
                      class="inline-flex items-center px-3 py-1 border border-gray-300 text-xs font-medium rounded text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-gray-500 disabled:opacity-50"
                    >
                      <XMarkIcon class="h-3 w-3 mr-1" />
                      Decline
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </Transition>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted, watch } from 'vue'
import { 
  BellIcon, 
  BellSlashIcon, 
  BuildingStorefrontIcon,
  CheckIcon,
  XMarkIcon
} from '@heroicons/vue/24/outline'
import { storeToRefs } from 'pinia'
import { useAuthStore } from '@/stores/auth'
import apiService from '@/services/api'
import type { VenueInvitationResponse } from '@/types/api'

const authStore = useAuthStore()
const { isAuthenticated, accessToken, user } = storeToRefs(authStore)

const isOpen = ref(false)
const loading = ref(false)
const invitations = ref<VenueInvitationResponse[]>([])
const processingInvitation = ref<number | null>(null)
let loadInvitationsTimeout: number | null = null

const notificationCount = computed(() => invitations.value.length)
const hasNotifications = computed(() => notificationCount.value > 0)

const toggleNotifications = () => {
  isOpen.value = !isOpen.value
  if (isOpen.value && invitations.value.length === 0) {
    loadInvitations()
  }
}

const loadInvitations = async () => {
  if (!isAuthenticated.value || !accessToken.value) {
    invitations.value = []
    return
  }

  loading.value = true
  try {
    console.log('Loading invitations with access token available:', !!accessToken.value)
    
    // Ensure the API service has the latest token
    apiService.setAccessToken(accessToken.value)
    
    // Get user email from auth store
    const userEmail = user.value?.email
    console.log('User email for invitations:', userEmail)
    
    const response = await apiService.getMyInvitations(userEmail)
    if (response.success) {
      invitations.value = response.data
      console.log('Loaded invitations:', invitations.value.length)
    } else {
      console.error('Failed to load invitations - API response not successful:', response)
    }
  } catch (error) {
    console.error('Failed to load invitations:', error)
    // If it's a 401, clear invitations and don't retry immediately
    if (error instanceof Error && error.message?.includes('401')) {
      invitations.value = []
    }
  } finally {
    loading.value = false
  }
}

const loadInvitationsDebounced = () => {
  // Clear any existing timeout
  if (loadInvitationsTimeout) {
    clearTimeout(loadInvitationsTimeout)
  }
  
  // Set a new timeout to debounce rapid calls
  loadInvitationsTimeout = setTimeout(loadInvitations, 100)
}

const acceptInvitation = async (invitationId: number) => {
  processingInvitation.value = invitationId
  try {
    const response = await apiService.acceptInvitation(invitationId)
    if (response.success) {
      // Remove the invitation from the list
      invitations.value = invitations.value.filter((inv: VenueInvitationResponse) => inv.id !== invitationId)
      // Reload permissions to update UI
      window.dispatchEvent(new CustomEvent('permissions-updated'))
    }
  } catch (error) {
    console.error('Failed to accept invitation:', error)
  } finally {
    processingInvitation.value = null
  }
}

const declineInvitation = async (invitationId: number) => {
  processingInvitation.value = invitationId
  try {
    const response = await apiService.declineInvitation(invitationId)
    if (response.success) {
      // Remove the invitation from the list
      invitations.value = invitations.value.filter((inv: VenueInvitationResponse) => inv.id !== invitationId)
    }
  } catch (error) {
    console.error('Failed to decline invitation:', error)
  } finally {
    processingInvitation.value = null
  }
}

const getPermissionDisplayName = (permission: string): string => {
  const permissionMap: Record<string, string> = {
    'venue:owner': 'Owner',
    'venue:manager': 'Manager',
    'venue:staff': 'Staff'
  }
  return permissionMap[permission] || permission
}

const formatDate = (dateString: string): string => {
  const date = new Date(dateString)
  const now = new Date()
  const diffMs = now.getTime() - date.getTime()
  const diffDays = Math.floor(diffMs / (1000 * 60 * 60 * 24))
  
  if (diffDays === 0) {
    return 'Today'
  } else if (diffDays === 1) {
    return 'Yesterday'
  } else if (diffDays < 7) {
    return `${diffDays} days ago`
  } else {
    return date.toLocaleDateString()
  }
}

// Close dropdown when clicking outside
const handleClickOutside = (event: MouseEvent) => {
  const target = event.target as HTMLElement
  if (!target.closest('.relative')) {
    isOpen.value = false
  }
}

// Watch for authentication and access token changes
watch([isAuthenticated, accessToken, user], ([auth, token, userInfo]) => {
  console.log('Auth state changed:', { auth, hasToken: !!token, hasUser: !!userInfo, userEmail: userInfo?.email })
  if (auth && token && userInfo?.email) {
    loadInvitationsDebounced()
  } else {
    invitations.value = []
    isOpen.value = false
  }
}, { immediate: true })

onMounted(() => {
  document.addEventListener('click', handleClickOutside)
  
  // Load invitations on mount if authenticated, token is available, and we have user email
  if (isAuthenticated.value && accessToken.value && user.value?.email) {
    loadInvitations()
  }
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
  
  // Clear any pending timeouts
  if (loadInvitationsTimeout) {
    clearTimeout(loadInvitationsTimeout)
  }
})
</script>
