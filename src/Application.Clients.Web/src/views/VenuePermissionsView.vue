<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Header -->
    <div class="bg-white shadow">
      <div class="px-4 sm:px-6 lg:px-8 py-6">
        <div class="flex items-center justify-between">
          <div>
            <nav class="flex mb-4" aria-label="Breadcrumb">
              <ol class="flex items-center space-x-2">
                <li>
                  <router-link to="/backoffice" class="text-gray-400 hover:text-gray-500">
                    Backoffice
                  </router-link>
                </li>
                <li class="flex items-center">
                  <ChevronRightIcon class="h-4 w-4 text-gray-400 mx-2" />
                  <router-link 
                    :to="`/backoffice/venues/${venueId}`" 
                    class="text-gray-400 hover:text-gray-500"
                  >
                    {{ venue?.name || 'Venue' }}
                  </router-link>
                </li>
                <li class="flex items-center">
                  <ChevronRightIcon class="h-4 w-4 text-gray-400 mx-2" />
                  <span class="text-gray-900">Permissions</span>
                </li>
              </ol>
            </nav>
            <h1 class="text-2xl font-bold text-gray-900">Venue Permissions</h1>
            <p class="mt-1 text-sm text-gray-500">
              Manage who can access and modify this venue
            </p>
          </div>
          <button
            @click="showInviteModal = true"
            class="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700"
          >
            <UserPlusIcon class="-ml-1 mr-2 h-4 w-4" />
            Invite User
          </button>
        </div>
      </div>
    </div>

    <!-- Content -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Active Permissions -->
      <div class="bg-white shadow rounded-lg mb-6">
        <div class="px-6 py-4 border-b border-gray-200">
          <h3 class="text-lg font-medium text-gray-900">Active Permissions</h3>
          <p class="mt-1 text-sm text-gray-500">Users who currently have access to this venue</p>
        </div>
        
        <div v-if="loading" class="px-6 py-8">
          <div class="animate-pulse space-y-4">
            <div v-for="i in 3" :key="i" class="flex items-center space-x-4">
              <div class="h-10 w-10 bg-gray-300 rounded-full"></div>
              <div class="flex-1 space-y-2">
                <div class="h-4 bg-gray-300 rounded w-1/4"></div>
                <div class="h-3 bg-gray-300 rounded w-1/6"></div>
              </div>
            </div>
          </div>
        </div>
        
        <div v-else-if="permissions.length === 0" class="px-6 py-8 text-center">
          <UserGroupIcon class="mx-auto h-12 w-12 text-gray-400" />
          <h3 class="mt-2 text-sm font-medium text-gray-900">No permissions</h3>
          <p class="mt-1 text-sm text-gray-500">
            No users have been granted access to this venue yet.
          </p>
        </div>
        
        <div v-else class="divide-y divide-gray-200">
          <div 
            v-for="permission in permissions" 
            :key="permission.id"
            class="px-6 py-4 flex items-center justify-between"
          >
            <div class="flex items-center space-x-4">
              <div class="h-10 w-10 bg-blue-100 rounded-full flex items-center justify-center">
                <UserIcon class="h-5 w-5 text-blue-600" />
              </div>
              <div>
                <p class="text-sm font-medium text-gray-900">{{ getUserEmail(permission) }}</p>
                <p class="text-sm font-semibold text-blue-600">
                  {{ getPermissionLabel(permission.name) }}
                </p>
                <p class="text-xs text-gray-500">
                  Granted {{ formatDate(permission.grantedAt) }}
                </p>
              </div>
            </div>
            <div class="flex items-center space-x-2">
              <span 
                :class="[
                  'inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium',
                  permission.isActive ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-800'
                ]"
              >
                {{ permission.isActive ? 'Active' : 'Inactive' }}
              </span>
              <div class="flex space-x-1">
                <button
                  @click="editPermission(permission)"
                  class="p-1 text-gray-400 hover:text-gray-500"
                >
                  <PencilIcon class="h-4 w-4" />
                </button>
                <button
                  @click="confirmRevokePermission(permission)"
                  class="p-1 text-red-400 hover:text-red-500"
                >
                  <TrashIcon class="h-4 w-4" />
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Pending Invitations -->
      <div class="bg-white shadow rounded-lg">
        <div class="px-6 py-4 border-b border-gray-200">
          <h3 class="text-lg font-medium text-gray-900">Pending Invitations</h3>
          <p class="mt-1 text-sm text-gray-500">Invitations that have been sent but not yet accepted</p>
        </div>
        
        <div v-if="invitations.length === 0" class="px-6 py-8 text-center">
          <EnvelopeIcon class="mx-auto h-12 w-12 text-gray-400" />
          <h3 class="mt-2 text-sm font-medium text-gray-900">No pending invitations</h3>
          <p class="mt-1 text-sm text-gray-500">
            All invitations have been accepted or have expired.
          </p>
        </div>
        
        <div v-else class="divide-y divide-gray-200">
          <div 
            v-for="invitation in invitations" 
            :key="invitation.id"
            class="px-6 py-4 flex items-center justify-between"
          >
            <div class="flex items-center space-x-4">
              <div class="h-10 w-10 bg-yellow-100 rounded-full flex items-center justify-center">
                <EnvelopeIcon class="h-5 w-5 text-yellow-600" />
              </div>
              <div>
                <p class="text-sm font-medium text-gray-900">{{ invitation.email }}</p>
                <p class="text-sm text-gray-500">
                  {{ getPermissionLabel(invitation.name) }} • 
                  Invited {{ formatDate(invitation.invitedAt) }}
                </p>
                <p class="text-xs text-gray-400">
                  Expires {{ formatDate(invitation.expiresAt) }}
                </p>
              </div>
            </div>
            <button
              @click="cancelInvitation(invitation)"
              class="text-red-400 hover:text-red-500"
            >
              <XMarkIcon class="h-5 w-5" />
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Invite User Modal -->
    <div v-if="showInviteModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
      <div class="relative top-20 mx-auto p-5 border w-96 shadow-lg rounded-md bg-white">
        <div class="mt-3">
          <h3 class="text-lg font-medium text-gray-900 mb-4">Invite User</h3>
          
          <form @submit.prevent="sendInvitation">
            <div class="mb-4">
              <label class="block text-sm font-medium text-gray-700 mb-2">
                Email Address
              </label>
              <input
                v-model="inviteForm.email"
                type="email"
                required
                class="block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
                placeholder="user@example.com"
              />
            </div>
            
            <div class="mb-4">
              <label class="block text-sm font-medium text-gray-700 mb-2">
                Permission Level
              </label>
              <select
                v-model="inviteForm.permission"
                required
                class="block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
              >
                <option value="venue:owner">Owner</option>
                <option value="venue:manager">Manager</option>
                <option value="venue:staff">Staff</option>
              </select>
            </div>
            
            <div class="mb-4">
              <label class="block text-sm font-medium text-gray-700 mb-2">
                Notes (Optional)
              </label>
              <textarea
                v-model="inviteForm.notes"
                rows="3"
                class="block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
                placeholder="Additional notes or instructions..."
              ></textarea>
            </div>
            
            <div class="flex justify-end space-x-3">
              <button
                type="button"
                @click="showInviteModal = false"
                class="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-md hover:bg-gray-50"
              >
                Cancel
              </button>
              <button
                type="submit"
                :disabled="sendingInvite"
                class="px-4 py-2 text-sm font-medium text-white bg-blue-600 border border-transparent rounded-md hover:bg-blue-700 disabled:opacity-50"
              >
                {{ sendingInvite ? 'Sending...' : 'Send Invitation' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <!-- Edit Permission Modal -->
    <div v-if="showEditModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
      <div class="relative top-20 mx-auto p-5 border w-96 shadow-lg rounded-md bg-white">
        <div class="flex justify-between items-center mb-4">
          <h3 class="text-lg font-medium">Edit Permission</h3>
          <button
            @click="showEditModal = false"
            class="text-gray-400 hover:text-gray-500"
          >
            <XMarkIcon class="h-5 w-5" />
          </button>
        </div>
        
        <form @submit.prevent="updatePermission">
          <div class="mb-4">
            <label class="block text-sm font-medium text-gray-700 mb-2">
              User Email
            </label>
            <div class="text-sm text-gray-900 bg-gray-50 px-3 py-2 border border-gray-300 rounded-md">
              {{ editForm.userEmail }}
            </div>
          </div>
          
          <div class="mb-4">
            <label class="block text-sm font-medium text-gray-700 mb-2">
              Permission Level *
            </label>
            <select
              v-model="editForm.permission"
              required
              class="block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
            >
              <option value="venue:owner">Owner</option>
              <option value="venue:manager">Manager</option>
              <option value="venue:staff">Staff</option>
            </select>
          </div>
          
          <div class="mb-4">
            <label class="block text-sm font-medium text-gray-700 mb-2">
              Notes (Optional)
            </label>
            <textarea
              v-model="editForm.notes"
              rows="3"
              class="block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
              placeholder="Additional notes or instructions..."
            ></textarea>
          </div>
          
          <div class="flex justify-end space-x-3">
            <button
              type="button"
              @click="showEditModal = false"
              class="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-md hover:bg-gray-50"
            >
              Cancel
            </button>
            <button
              type="submit"
              :disabled="updatingPermission"
              class="px-4 py-2 text-sm font-medium text-white bg-blue-600 border border-transparent rounded-md hover:bg-blue-700 disabled:opacity-50"
            >
              {{ updatingPermission ? 'Updating...' : 'Update Permission' }}
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- Toast Notification -->
    <div v-if="showToast" class="fixed bottom-4 right-4 z-50">
      <div 
        v-if="toastType === 'success'"
        class="bg-green-500 text-white text-sm font-medium rounded-lg px-4 py-2 shadow-md"
      >
        {{ toastMessage }}
      </div>
      <div 
        v-else-if="toastType === 'error'"
        class="bg-red-500 text-white text-sm font-medium rounded-lg px-4 py-2 shadow-md"
      >
        {{ toastMessage }}
      </div>
      <div 
        v-else-if="toastType === 'warning'"
        class="bg-yellow-500 text-white text-sm font-medium rounded-lg px-4 py-2 shadow-md"
      >
        {{ toastMessage }}
      </div>
      <div 
        v-else-if="toastType === 'info'"
        class="bg-blue-500 text-white text-sm font-medium rounded-lg px-4 py-2 shadow-md"
      >
        {{ toastMessage }}
      </div>
    </div>
  </div>

  <!-- Toast Notifications -->
  <Toast
    v-if="showToast"
    :type="toastType"
    :title="toastMessage"
    @close="showToast = false"
  />
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuth0 } from '@auth0/auth0-vue'
import {
  ChevronRightIcon,
  UserIcon,
  UserGroupIcon,
  UserPlusIcon,
  EnvelopeIcon,
  PencilIcon,
  TrashIcon,
  XMarkIcon
} from '@heroicons/vue/24/outline'
import apiService from '@/services/api'
import Toast from '@/components/Toast.vue'
import type { VenueSummary, UserVenuePermission, VenueInvitation, CreateInvitationRequest } from '@/types/api'

interface Props {
  venueId?: string
}

const props = defineProps<Props>()
const route = useRoute()
const router = useRouter()
const { user } = useAuth0()

// State
const venue = ref<VenueSummary | null>(null)
const permissions = ref<UserVenuePermission[]>([])
const invitations = ref<VenueInvitation[]>([])
const loading = ref(true)
const showInviteModal = ref(false)
const showEditModal = ref(false)
const sendingInvite = ref(false)
const updatingPermission = ref(false)

// Toast notifications
const showToast = ref(false)
const toastMessage = ref('')
const toastType = ref<'success' | 'error' | 'warning' | 'info'>('info')

// Form state
const inviteForm = ref<CreateInvitationRequest>({
  email: '',
  venueId: 0,
  permission: 'venue:staff',
  notes: '',
  senderEmail: '' // Will be set from Auth0 user
})

const editForm = ref({
  permissionId: 0,
  permission: '',
  notes: '',
  userEmail: ''
})

// Computed
const venueId = computed(() => {
  return parseInt(props.venueId || route.params.venueId as string)
})

// Methods
const loadVenue = async () => {
  try {
    const response = await apiService.getVenue(venueId.value)
    if (response.success) {
      venue.value = response.data
    }
  } catch (error) {
    console.error('Error loading venue:', error)
  }
}

const loadPermissions = async () => {
  try {
    const response = await apiService.getVenuePermissions(venueId.value)
    if (response.success) {
      permissions.value = response.data
    }
  } catch (error) {
    console.error('Error loading permissions:', error)
  }
}

const loadInvitations = async () => {
  try {
    const response = await apiService.getVenueInvitations(venueId.value)
    if (response.success) {
      invitations.value = response.data.filter(inv => inv.isActive && !inv.acceptedAt)
    }
  } catch (error) {
    console.error('Error loading invitations:', error)
  }
}

const loadData = async () => {
  loading.value = true
  try {
    await Promise.all([
      loadVenue(),
      loadPermissions(),
      loadInvitations()
    ])
  } finally {
    loading.value = false
  }
}

const sendInvitation = async () => {
  console.debug('Sending invitation:', inviteForm.value)
  sendingInvite.value = true
  try {
    // Set the venue ID and sender email
    inviteForm.value.venueId = venueId.value
    inviteForm.value.senderEmail = user.value?.email || ''
    
    if (!inviteForm.value.senderEmail) {
      console.error('Cannot send invitation: No sender email available')
      return
    }
    
    console.debug('Sending invitation with venueId and senderEmail:', inviteForm.value)
    const response = await apiService.sendInvitation(inviteForm.value)
    console.debug('Invitation response:', response)
    if (response.success) {
      console.debug('Invitation sent successfully')
      // Reset form and close modal
      inviteForm.value = {
        email: '',
        venueId: 0,
        permission: 'venue:staff',
        notes: '',
        senderEmail: ''
      }
      showInviteModal.value = false
      // Reload invitations
      await loadInvitations()
      // Show success toast
      toastMessage.value = 'Invitation sent successfully.'
      toastType.value = 'success'
      showToast.value = true
    } else {
      console.error('Invitation failed:', response)
      // Show error toast
      toastMessage.value = 'Failed to send invitation. Please try again.'
      toastType.value = 'error'
      showToast.value = true
    }
  } catch (error) {
    console.error('Error sending invitation:', error)
    // Show error toast
    toastMessage.value = 'Error sending invitation. Please try again.'
    toastType.value = 'error'
    showToast.value = true
  } finally {
    sendingInvite.value = false
  }
}

const cancelInvitation = (invitation: VenueInvitation) => {
  router.push({
    path: '/confirm',
    query: {
      type: 'cancel-invitation',
      id: invitation.id.toString(),
      name: invitation.email,
      returnTo: `/backoffice/venues/${venueId.value}/permissions`
    }
  })
}

const editPermission = (permission: UserVenuePermission) => {
  editForm.value = {
    permissionId: permission.id,
    permission: permission.name,
    notes: permission.notes || '',
    userEmail: permission.userEmail || ''
  }
  showEditModal.value = true
}

const confirmRevokePermission = (permission: UserVenuePermission) => {
  router.push({
    path: '/confirm',
    query: {
      type: 'revoke-permission',
      id: permission.id.toString(),
      name: permission.userEmail,
      returnTo: `/backoffice/venues/${venueId.value}/permissions`
    }
  })
}

const updatePermission = async () => {
  updatingPermission.value = true
  try {
    const response = await apiService.updateUserPermission(editForm.value.permissionId, {
      permission: editForm.value.permission,
      notes: editForm.value.notes
    })
    
    if (response.success) {
      showEditModal.value = false
      await loadPermissions()
      // Show success toast
      toastMessage.value = 'Permission updated successfully.'
      toastType.value = 'success'
      showToast.value = true
    }
  } catch (error) {
    console.error('Error updating permission:', error)
    // Show error toast
    toastMessage.value = 'Error updating permission. Please try again.'
    toastType.value = 'error'
    showToast.value = true
  } finally {
    updatingPermission.value = false
  }
}

const getUserEmail = (permission: UserVenuePermission) => {
  return permission.userEmail || 'Unknown User'
}

const getPermissionLabel = (permission: string) => {
  switch (permission) {
    case 'venue:owner': return 'Owner'
    case 'venue:manager': return 'Manager' 
    case 'venue:staff': return 'Staff'
    default: return permission
  }
}

const formatDate = (dateString: string) => {
  try {
    const date = new Date(dateString)
    return date.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    })
  } catch (error) {
    return 'Invalid date'
  }
}

const showToastMessage = (message: string, type: 'success' | 'error' | 'warning' | 'info') => {
  toastMessage.value = message
  toastType.value = type
  showToast.value = true
}

const handleQueryParameters = () => {
  // Check for success/error messages from the confirm view
  if (route.query.success === 'true' && route.query.message) {
    showToastMessage(route.query.message as string, 'success')
    // Reload data after successful action
    loadData()
  } else if (route.query.error === 'true' && route.query.message) {
    showToastMessage(route.query.message as string, 'error')
  }
  
  // Clean up query parameters
  if (route.query.success || route.query.error || route.query.message) {
    router.replace({ path: route.path })
  }
}

onMounted(() => {
  loadData()
  handleQueryParameters()
})
</script>
