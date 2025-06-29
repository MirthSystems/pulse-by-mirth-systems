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
                <p class="text-sm text-gray-500">
                  {{ getPermissionLabel(permission.name) }} • 
                  Granted {{ formatDate(permission.grantedAt) }}
                </p>
                <p v-if="permission.notes" class="text-xs text-gray-400 mt-1">
                  {{ permission.notes }}
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
                  {{ getPermissionLabel(invitation.permission) }} • 
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
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
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
import type { VenueSummary, UserVenuePermission, VenueInvitation, CreateInvitationRequest } from '@/types/api'

interface Props {
  venueId?: string
}

const props = defineProps<Props>()
const route = useRoute()
const router = useRouter()

// State
const venue = ref<VenueSummary | null>(null)
const permissions = ref<UserVenuePermission[]>([])
const invitations = ref<VenueInvitation[]>([])
const loading = ref(true)
const showInviteModal = ref(false)
const sendingInvite = ref(false)

// Form state
const inviteForm = ref<CreateInvitationRequest>({
  email: '',
  venueId: 0,
  permission: 'venue:staff',
  notes: ''
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
  sendingInvite.value = true
  try {
    inviteForm.value.venueId = venueId.value
    const response = await apiService.sendInvitation(inviteForm.value)
    if (response.success) {
      // Reset form and close modal
      inviteForm.value = {
        email: '',
        venueId: 0,
        permission: 'venue:staff',
        notes: ''
      }
      showInviteModal.value = false
      // Reload invitations
      await loadInvitations()
    }
  } catch (error) {
    console.error('Error sending invitation:', error)
  } finally {
    sendingInvite.value = false
  }
}

const cancelInvitation = async (invitation: VenueInvitation) => {
  if (confirm('Are you sure you want to cancel this invitation?')) {
    try {
      const response = await apiService.cancelInvitation(invitation.id)
      if (response.success) {
        await loadInvitations()
      }
    } catch (error) {
      console.error('Error canceling invitation:', error)
    }
  }
}

const editPermission = (permission: UserVenuePermission) => {
  // TODO: Implement permission editing modal
  console.log('Edit permission:', permission)
}

const confirmRevokePermission = async (permission: UserVenuePermission) => {
  if (confirm('Are you sure you want to revoke this permission?')) {
    try {
      const response = await apiService.revokeUserPermission(permission.id)
      if (response.success) {
        await loadPermissions()
      }
    } catch (error) {
      console.error('Error revoking permission:', error)
    }
  }
}

const getUserEmail = (permission: UserVenuePermission) => {
  // In a real implementation, you might want to fetch user details
  // For now, we can use a placeholder or the permission data
  return permission.grantedByUserEmail || 'Unknown User'
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

onMounted(() => {
  loadData()
})
</script>
