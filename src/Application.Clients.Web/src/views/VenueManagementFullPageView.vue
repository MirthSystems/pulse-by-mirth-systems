<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Header -->
    <div class="bg-white shadow">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between items-center py-6">
          <div>
            <h1 class="text-3xl font-bold text-gray-900">Venue Management</h1>
            <p class="mt-1 text-sm text-gray-500">
              Manage your venues and their specials
            </p>
          </div>
          <button
            @click="showCreateForm = true"
            class="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
          >
            <PlusIcon class="-ml-1 mr-2 h-5 w-5" />
            Add Venue
          </button>
        </div>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div class="text-center">
        <div class="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
        <p class="mt-2 text-gray-600">Loading venues...</p>
      </div>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div class="bg-red-50 border border-red-200 rounded-md p-4">
        <div class="flex">
          <ExclamationTriangleIcon class="h-5 w-5 text-red-400" />
          <div class="ml-3">
            <h3 class="text-sm font-medium text-red-800">Error</h3>
            <p class="mt-1 text-sm text-red-700">{{ error }}</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div v-else class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Create/Edit Form -->
      <div v-if="showCreateForm || editingVenue" class="bg-white shadow rounded-lg mb-8">
        <div class="px-6 py-4 border-b border-gray-200">
          <h3 class="text-lg font-medium text-gray-900">
            {{ editingVenue ? 'Edit Venue' : 'Create New Venue' }}
          </h3>
        </div>
        <div class="p-6">
          <form @submit.prevent="handleSubmit" class="space-y-6">
            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <label for="name" class="block text-sm font-medium text-gray-700">
                  Venue Name *
                </label>
                <input
                  v-model="form.name"
                  type="text"
                  id="name"
                  required
                  class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                  :class="{ 'border-red-300': formErrors.name }"
                />
                <p v-if="formErrors.name" class="mt-1 text-sm text-red-600">{{ formErrors.name }}</p>
              </div>

              <div>
                <label for="category" class="block text-sm font-medium text-gray-700">
                  Category *
                </label>
                <select
                  v-model="form.categoryId"
                  id="category"
                  required
                  class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                  :class="{ 'border-red-300': formErrors.categoryId }"
                >
                  <option value="">Select a category</option>
                  <option v-for="category in categories" :key="category.id" :value="category.id">
                    {{ category.name }}
                  </option>
                </select>
                <p v-if="formErrors.categoryId" class="mt-1 text-sm text-red-600">{{ formErrors.categoryId }}</p>
              </div>

              <div class="md:col-span-2">
                <label for="description" class="block text-sm font-medium text-gray-700">
                  Description
                </label>
                <textarea
                  v-model="form.description"
                  id="description"
                  rows="3"
                  class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                />
              </div>

              <div>
                <label for="phoneNumber" class="block text-sm font-medium text-gray-700">
                  Phone Number
                </label>
                <input
                  v-model="form.phoneNumber"
                  type="tel"
                  id="phoneNumber"
                  class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                />
              </div>

              <div>
                <label for="website" class="block text-sm font-medium text-gray-700">
                  Website
                </label>
                <input
                  v-model="form.website"
                  type="url"
                  id="website"
                  class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                />
              </div>

              <div>
                <label for="email" class="block text-sm font-medium text-gray-700">
                  Email
                </label>
                <input
                  v-model="form.email"
                  type="email"
                  id="email"
                  class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                />
              </div>

              <div>
                <label for="streetAddress" class="block text-sm font-medium text-gray-700">
                  Street Address *
                </label>
                <input
                  v-model="form.streetAddress"
                  type="text"
                  id="streetAddress"
                  required
                  class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                  :class="{ 'border-red-300': formErrors.streetAddress }"
                />
                <p v-if="formErrors.streetAddress" class="mt-1 text-sm text-red-600">{{ formErrors.streetAddress }}</p>
              </div>

              <div>
                <label for="locality" class="block text-sm font-medium text-gray-700">
                  City *
                </label>
                <input
                  v-model="form.locality"
                  type="text"
                  id="locality"
                  required
                  class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                  :class="{ 'border-red-300': formErrors.locality }"
                />
                <p v-if="formErrors.locality" class="mt-1 text-sm text-red-600">{{ formErrors.locality }}</p>
              </div>

              <div>
                <label for="region" class="block text-sm font-medium text-gray-700">
                  State/Region *
                </label>
                <input
                  v-model="form.region"
                  type="text"
                  id="region"
                  required
                  class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                  :class="{ 'border-red-300': formErrors.region }"
                />
                <p v-if="formErrors.region" class="mt-1 text-sm text-red-600">{{ formErrors.region }}</p>
              </div>

              <div>
                <label for="postalCode" class="block text-sm font-medium text-gray-700">
                  Postal Code *
                </label>
                <input
                  v-model="form.postalCode"
                  type="text"
                  id="postalCode"
                  required
                  class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                  :class="{ 'border-red-300': formErrors.postalCode }"
                />
                <p v-if="formErrors.postalCode" class="mt-1 text-sm text-red-600">{{ formErrors.postalCode }}</p>
              </div>

              <div class="md:col-span-2 flex items-center">
                <input
                  v-model="form.isActive"
                  id="isActive"
                  type="checkbox"
                  class="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
                />
                <label for="isActive" class="ml-2 block text-sm text-gray-900">
                  Active venue
                </label>
              </div>
            </div>

            <!-- Form Actions -->
            <div class="flex justify-end space-x-3 pt-6 border-t border-gray-200">
              <button
                @click="cancelEdit"
                type="button"
                class="inline-flex items-center px-4 py-2 border border-gray-300 shadow-sm text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
              >
                Cancel
              </button>
              <button
                type="submit"
                :disabled="submitting"
                class="inline-flex items-center px-4 py-2 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-50"
              >
                <span v-if="!submitting">{{ editingVenue ? 'Update Venue' : 'Create Venue' }}</span>
                <span v-else class="flex items-center">
                  <svg class="animate-spin -ml-1 mr-3 h-4 w-4 text-white" fill="none" viewBox="0 0 24 24">
                    <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                    <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                  </svg>
                  {{ editingVenue ? 'Updating...' : 'Creating...' }}
                </span>
              </button>
            </div>
          </form>
        </div>
      </div>

      <!-- Venues List -->
      <div class="bg-white shadow rounded-lg">
        <div class="px-6 py-4 border-b border-gray-200">
          <h3 class="text-lg font-medium text-gray-900">Venues</h3>
        </div>
        <div class="overflow-x-auto">
          <table class="min-w-full divide-y divide-gray-200">
            <thead class="bg-gray-50">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Venue
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Category
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Location
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Contact
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Status
                </th>
                <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Actions
                </th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-gray-200">
              <tr v-for="venue in venues" :key="venue.id">
                <td class="px-6 py-4 whitespace-nowrap">
                  <div>
                    <div class="text-sm font-medium text-gray-900">{{ venue.name }}</div>
                    <div class="text-sm text-gray-500">{{ venue.description }}</div>
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="text-sm text-gray-900">{{ venue.categoryName }}</div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="text-sm text-gray-900">{{ venue.locality }}, {{ venue.region }}</div>
                  <div class="text-sm text-gray-500">{{ venue.streetAddress }}</div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="text-sm text-gray-900">{{ venue.phoneNumber }}</div>
                  <div class="text-sm text-gray-500">{{ venue.email }}</div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span :class="[
                    'inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium',
                    venue.isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
                  ]">
                    {{ venue.isActive ? 'Active' : 'Inactive' }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                  <div class="flex justify-end space-x-2">
                    <button
                      @click="editVenue(venue)"
                      class="text-blue-600 hover:text-blue-900"
                    >
                      <PencilIcon class="h-4 w-4" />
                    </button>
                    <button
                      @click="deleteVenue(venue)"
                      class="text-red-600 hover:text-red-900"
                    >
                      <TrashIcon class="h-4 w-4" />
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { 
  PlusIcon, 
  PencilIcon, 
  TrashIcon,
  ExclamationTriangleIcon
} from '@heroicons/vue/24/outline'
import apiService from '@/services/api'
import { useApiAuth } from '@/composables/useApiAuth'
import type { VenueSummary, VenueCategory, CreateVenueRequest, UpdateVenueRequest } from '@/types/api'

// State
const venues = ref<VenueSummary[]>([])
const categories = ref<VenueCategory[]>([])
const loading = ref(true)
const error = ref<string | null>(null)

// Form state
const showCreateForm = ref(false)
const editingVenue = ref<VenueSummary | null>(null)
const submitting = ref(false)
const formErrors = ref<Record<string, string>>({})

const form = ref<CreateVenueRequest & { id?: number }>({
  categoryId: 0,
  name: '',
  description: '',
  phoneNumber: '',
  website: '',
  email: '',
  streetAddress: '',
  secondaryAddress: '',
  locality: '',
  region: '',
  postalCode: '',
  country: 'US',
  latitude: 0,
  longitude: 0,
  isActive: true
})

// Initialize API auth
const { updateApiToken } = useApiAuth()

// Load data
const loadData = async () => {
  try {
    loading.value = true
    error.value = null
    
    await updateApiToken()
    
    const [venuesResponse, categoriesResponse] = await Promise.all([
      apiService.getVenues(),
      apiService.getVenueCategories()
    ])
    
    if (venuesResponse.success) {
      venues.value = venuesResponse.data
    } else {
      throw new Error(venuesResponse.message || 'Failed to load venues')
    }
    
    if (categoriesResponse.success) {
      categories.value = categoriesResponse.data
    } else {
      throw new Error(categoriesResponse.message || 'Failed to load categories')
    }
  } catch (err) {
    console.error('Error loading data:', err)
    error.value = err instanceof Error ? err.message : 'Failed to load data'
  } finally {
    loading.value = false
  }
}

// Form methods
const resetForm = () => {
  form.value = {
    categoryId: 0,
    name: '',
    description: '',
    phoneNumber: '',
    website: '',
    email: '',
    streetAddress: '',
    secondaryAddress: '',
    locality: '',
    region: '',
    postalCode: '',
    country: 'US',
    latitude: 0,
    longitude: 0,
    isActive: true
  }
  formErrors.value = {}
}

const validateForm = (): boolean => {
  formErrors.value = {}
  
  if (!form.value.name.trim()) {
    formErrors.value.name = 'Venue name is required'
  }
  
  if (!form.value.categoryId) {
    formErrors.value.categoryId = 'Category is required'
  }
  
  if (!form.value.streetAddress.trim()) {
    formErrors.value.streetAddress = 'Street address is required'
  }
  
  if (!form.value.locality.trim()) {
    formErrors.value.locality = 'City is required'
  }
  
  if (!form.value.region.trim()) {
    formErrors.value.region = 'State/Region is required'
  }
  
  if (!form.value.postalCode.trim()) {
    formErrors.value.postalCode = 'Postal code is required'
  }
  
  return Object.keys(formErrors.value).length === 0
}

const handleSubmit = async () => {
  if (!validateForm()) {
    return
  }
  
  submitting.value = true
  
  try {
    await updateApiToken()
    
    const { id, ...venueData } = form.value
    
    let response
    if (editingVenue.value) {
      response = await apiService.updateVenue(editingVenue.value.id, venueData as UpdateVenueRequest)
    } else {
      response = await apiService.createVenue(venueData as CreateVenueRequest)
    }
    
    if (response.success) {
      await loadData()
      cancelEdit()
    } else {
      throw new Error(response.message || 'Failed to save venue')
    }
  } catch (err) {
    console.error('Error saving venue:', err)
    error.value = err instanceof Error ? err.message : 'Failed to save venue'
  } finally {
    submitting.value = false
  }
}

const editVenue = (venue: VenueSummary) => {
  editingVenue.value = venue
  form.value = {
    id: venue.id,
    categoryId: venue.categoryId,
    name: venue.name,
    description: venue.description,
    phoneNumber: venue.phoneNumber,
    website: venue.website,
    email: venue.email,
    streetAddress: venue.streetAddress,
    secondaryAddress: venue.secondaryAddress || '',
    locality: venue.locality,
    region: venue.region,
    postalCode: venue.postalCode,
    country: venue.country,
    latitude: venue.latitude,
    longitude: venue.longitude,
    isActive: venue.isActive
  }
  showCreateForm.value = false
}

const cancelEdit = () => {
  editingVenue.value = null
  showCreateForm.value = false
  resetForm()
}

const deleteVenue = async (venue: VenueSummary) => {
  if (!confirm(`Are you sure you want to delete "${venue.name}"?`)) {
    return
  }
  
  try {
    await updateApiToken()
    const response = await apiService.deleteVenue(venue.id)
    
    if (response.success) {
      await loadData()
    } else {
      throw new Error(response.message || 'Failed to delete venue')
    }
  } catch (err) {
    console.error('Error deleting venue:', err)
    error.value = err instanceof Error ? err.message : 'Failed to delete venue'
  }
}

onMounted(() => {
  loadData()
})
</script>
