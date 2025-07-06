<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Header -->
    <div class="bg-white shadow">
      <div class="px-4 sm:px-6 lg:px-8 py-6">
        <div class="flex flex-col space-y-4 lg:flex-row lg:items-center lg:justify-between lg:space-y-0">
          <div class="flex items-center">
            <button
              @click="$router.push('/backoffice')"
              class="mr-4 p-2 text-gray-400 hover:text-gray-600"
            >
              <ChevronLeftIcon class="h-5 w-5" />
            </button>
            <div class="min-w-0 flex-1">
              <h1 class="text-xl sm:text-2xl font-bold text-gray-900 truncate">
                {{ isNewVenue ? 'Create New Venue' : 'Venue Details' }}
              </h1>
              <p class="mt-1 text-sm text-gray-500 truncate">
                {{ isNewVenue ? 'Enter venue information below' : `Manage ${venue?.name || 'venue'} details and specials` }}
              </p>
            </div>
          </div>
          <div class="flex flex-wrap items-center justify-end gap-2 lg:justify-start lg:space-x-3 lg:flex-nowrap">
            <router-link
              v-if="!isNewVenue && venuePermissions.canManageUsers"
              :to="`/backoffice/venues/${venueId}/permissions`"
              class="inline-flex items-center px-3 py-2 lg:px-4 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
            >
              <UserGroupIcon class="-ml-1 mr-2 h-4 w-4" />
              <span class="hidden lg:inline">Manage Users</span>
              <span class="lg:hidden">Users</span>
            </router-link>
            <button
              v-if="!isNewVenue && !isEditing && venuePermissions.canEdit"
              @click="startEditing"
              class="inline-flex items-center px-3 py-2 lg:px-4 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
            >
              <PencilIcon class="-ml-1 mr-2 h-4 w-4" />
              <span class="hidden lg:inline">Edit Venue</span>
              <span class="lg:hidden">Edit</span>
            </button>
            <button
              v-if="isEditing"
              @click="cancelEditing"
              class="inline-flex items-center px-3 py-2 lg:px-4 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-gray-500"
            >
              Cancel
            </button>
            <button
              v-if="isEditing || isNewVenue"
              @click="saveVenue"
              :disabled="saving"
              class="inline-flex items-center px-3 py-2 lg:px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-50"
            >
              <span v-if="!saving">{{ isNewVenue ? 'Create Venue' : 'Save Changes' }}</span>
              <span v-else class="flex items-center">
                <svg class="animate-spin -ml-1 mr-3 h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                {{ isNewVenue ? 'Creating...' : 'Saving...' }}
              </span>
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Content -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div v-if="loading" class="flex items-center justify-center py-12">
        <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
        <span class="ml-2 text-gray-600">Loading venue...</span>
      </div>

      <div v-else class="space-y-8">
        <!-- Venue Information Form -->
        <div class="bg-white shadow rounded-lg">
          <div class="px-6 py-4 border-b border-gray-200">
            <h3 class="text-lg leading-6 font-medium text-gray-900">Venue Information</h3>
          </div>
          
          <form @submit.prevent="saveVenue" class="p-6">
            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <!-- Basic Information -->
              <div class="space-y-4">
                <h4 class="text-md font-medium text-gray-900">Basic Details</h4>
                
                <div>
                  <label for="name" class="block text-sm font-medium text-gray-700">Venue Name *</label>
                  <input
                    v-model="form.name"
                    type="text"
                    id="name"
                    :readonly="!isEditing && !isNewVenue"
                    required
                    class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                    :class="{ 'bg-gray-50': !isEditing && !isNewVenue, 'border-red-300': errors.name }"
                  />
                  <p v-if="errors.name" class="mt-1 text-sm text-red-600">{{ errors.name }}</p>
                </div>

                <div>
                  <label for="category" class="block text-sm font-medium text-gray-700">Category *</label>
                  <select
                    v-model="form.categoryId"
                    id="category"
                    :disabled="!isEditing && !isNewVenue"
                    required
                    class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                    :class="{ 'bg-gray-50': !isEditing && !isNewVenue, 'border-red-300': errors.categoryId }"
                  >
                    <option value="">Select a category</option>
                    <option v-for="category in categories" :key="category.id" :value="category.id">
                      {{ category.icon }} {{ category.name }}
                    </option>
                  </select>
                  <p v-if="errors.categoryId" class="mt-1 text-sm text-red-600">{{ errors.categoryId }}</p>
                </div>

                <div>
                  <label for="description" class="block text-sm font-medium text-gray-700">Description</label>
                  <textarea
                    v-model="form.description"
                    id="description"
                    rows="4"
                    :readonly="!isEditing && !isNewVenue"
                    class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                    :class="{ 'bg-gray-50': !isEditing && !isNewVenue }"
                  ></textarea>
                </div>

                <div class="flex items-center">
                  <input
                    v-model="form.isActive"
                    id="isActive"
                    type="checkbox"
                    :disabled="!isEditing && !isNewVenue"
                    class="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
                  />
                  <label for="isActive" class="ml-2 block text-sm text-gray-900">
                    Active venue
                  </label>
                </div>
              </div>

              <!-- Contact & Location -->
              <div class="space-y-4">
                <h4 class="text-md font-medium text-gray-900">Contact & Location</h4>
                
                <div>
                  <label for="phoneNumber" class="block text-sm font-medium text-gray-700">Phone Number</label>
                  <input
                    v-model="form.phoneNumber"
                    type="tel"
                    id="phoneNumber"
                    :readonly="!isEditing && !isNewVenue"
                    class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                    :class="{ 'bg-gray-50': !isEditing && !isNewVenue }"
                  />
                </div>

                <div>
                  <label for="email" class="block text-sm font-medium text-gray-700">Email</label>
                  <input
                    v-model="form.email"
                    type="email"
                    id="email"
                    :readonly="!isEditing && !isNewVenue"
                    class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                    :class="{ 'bg-gray-50': !isEditing && !isNewVenue }"
                  />
                </div>

                <div>
                  <label for="websiteUrl" class="block text-sm font-medium text-gray-700">Website URL</label>
                  <input
                    v-model="form.website"
                    type="url"
                    id="websiteUrl"
                    :readonly="!isEditing && !isNewVenue"
                    class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                    :class="{ 'bg-gray-50': !isEditing && !isNewVenue }"
                  />
                </div>

                <div>
                  <label for="streetAddress" class="block text-sm font-medium text-gray-700 mb-4">Address Information</label>
                  <VenueAddressForm
                    v-model="addressForm"
                    :readonly="!isEditing && !isNewVenue"
                    :errors="errors"
                    @address-geocoded="onAddressGeocoded"
                  />
                </div>
              </div>
            </div>

            <!-- Business Hours -->
            <div class="space-y-6">
              <BusinessHoursEditor
                v-model="form.businessHours"
                :disabled="!isEditing && !isNewVenue"
              />
            </div>

            <!-- Error Summary -->
            <div v-if="submitError" class="mt-6 p-4 bg-red-50 border border-red-200 rounded-md">
              <div class="flex">
                <ExclamationTriangleIcon class="h-5 w-5 text-red-400" />
                <div class="ml-3">
                  <h3 class="text-sm font-medium text-red-800">
                    Error saving venue
                  </h3>
                  <div class="mt-2 text-sm text-red-700">
                    <p>{{ submitError }}</p>
                  </div>
                </div>
              </div>
            </div>
          </form>
        </div>

        <!-- Specials Management (only for existing venues) -->
        <div v-if="!isNewVenue" class="bg-white shadow rounded-lg">
          <div class="px-6 py-4 border-b border-gray-200 flex items-center justify-between">
            <div>
              <h3 class="text-lg leading-6 font-medium text-gray-900">Specials</h3>
              <p class="text-sm text-gray-500">Manage specials for this venue</p>
            </div>
            <router-link
              :to="`/backoffice/venues/${venueId}/specials/new`"
              class="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-green-600 hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500"
            >
              <StarIcon class="-ml-1 mr-2 h-4 w-4" />
              New Special
            </router-link>
          </div>
          
          <div v-if="loadingSpecials" class="px-6 py-12">
            <div class="flex items-center justify-center">
              <div class="animate-spin rounded-full h-6 w-6 border-b-2 border-green-600"></div>
              <span class="ml-2 text-gray-600">Loading specials...</span>
            </div>
          </div>
          
          <div v-else-if="specials.length === 0" class="px-6 py-12 text-center">
            <StarIcon class="mx-auto h-12 w-12 text-gray-400" />
            <h3 class="mt-2 text-sm font-medium text-gray-900">No specials yet</h3>
            <p class="mt-1 text-sm text-gray-500">Get started by creating your first special.</p>
          </div>
          
          <div v-else class="p-6">
            <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
              <BackofficeSpecialCard 
                v-for="special in specials" 
                :key="special.id"
                :special="special"
                @edit="editSpecial"
                @delete="confirmDeleteSpecial"
                class="hover:shadow-lg transform hover:-translate-y-1 transition-all duration-200"
              />
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import apiService from '@/services/api'
import { usePermissions } from '@/composables/usePermissions'
import { useAnalytics } from '@/composables/useAnalytics'
import type { 
  Venue,
  VenueCategory, 
  SpecialSummary,
  CreateVenueRequest, 
  UpdateVenueRequest,
  BusinessHoursRequest,
  GeocodeResult
} from '@/types/api'
import {
  ChevronLeftIcon,
  PencilIcon,
  TrashIcon,
  StarIcon,
  UserGroupIcon,
  ExclamationTriangleIcon
} from '@heroicons/vue/24/outline'
import BusinessHoursEditor from '../components/common/BusinessHoursEditor.vue'
import VenueAddressForm from '../components/VenueAddressForm.vue'
import BackofficeSpecialCard from '../components/BackofficeSpecialCard.vue'

const route = useRoute()
const router = useRouter()

// Permissions
const { getVenuePermissions } = usePermissions()

// Analytics
const { trackForm, trackEvent, trackContentView, trackError } = useAnalytics()

// Props
interface Props {
  id?: string
}

const props = defineProps<Props>()

// State
const venue = ref<Venue | null>(null)
const categories = ref<VenueCategory[]>([])
const specials = ref<SpecialSummary[]>([])
const loading = ref(true)
const loadingSpecials = ref(false)
const saving = ref(false)
const isEditing = ref(false)
const submitError = ref('')

// Form state
const form = ref<CreateVenueRequest>({
  categoryId: 0,
  name: '',
  description: '',
  streetAddress: '',
  locality: '',
  region: '',
  postalCode: '',
  country: 'United States',
  latitude: undefined,
  longitude: undefined,
  phoneNumber: '',
  email: '',
  website: '',
  isActive: true,
  businessHours: []
})

const errors = ref<Record<string, string>>({})

// Computed
const venueId = computed(() => {
  const id = props.id || route.params.id as string
  return id === 'new' ? null : parseInt(id)
})

const isNewVenue = computed(() => venueId.value === null)

// Venue permissions
const venuePermissions = computed(() => {
  if (!venueId.value) return { canEdit: true, canManageUsers: true } // New venue creation
  return getVenuePermissions(venueId.value)
})

// Address form computed property for the VenueAddressForm component
const addressForm = computed({
  get: () => ({
    streetAddress: form.value.streetAddress,
    locality: form.value.locality,
    region: form.value.region,
    postalCode: form.value.postalCode,
    country: form.value.country
  }),
  set: (value) => {
    form.value.streetAddress = value.streetAddress
    form.value.locality = value.locality
    form.value.region = value.region
    form.value.postalCode = value.postalCode
    form.value.country = value.country
  }
})

// Methods
const loadVenue = async () => {
  if (isNewVenue.value) {
    loading.value = false
    isEditing.value = true
    return
  }

  loading.value = true
  try {
    const response = await apiService.getVenue(venueId.value!)
    if (response.success) {
      venue.value = response.data
      // Populate form with venue data
      form.value = {
        categoryId: venue.value.categoryId,
        name: venue.value.name,
        description: venue.value.description || '',
        streetAddress: venue.value.streetAddress,
        locality: venue.value.locality,
        region: venue.value.region,
        postalCode: venue.value.postalCode,
        country: venue.value.country,
        latitude: venue.value.latitude,
        longitude: venue.value.longitude,
        phoneNumber: venue.value.phoneNumber || '',
        email: venue.value.email || '',
        website: venue.value.website || '',
        isActive: venue.value.isActive,
        businessHours: venue.value.businessHours?.map(bh => ({
          dayOfWeekId: bh.dayOfWeekId,
          openTime: bh.isClosed ? undefined : bh.openTime,
          closeTime: bh.isClosed ? undefined : bh.closeTime,
          isClosed: bh.isClosed
        })) || []
      }
    }
  } catch (error) {
    console.error('Error loading venue:', error)
  } finally {
    loading.value = false
  }
}

const loadCategories = async () => {
  try {
    const response = await apiService.getVenueCategories()
    if (response.success) {
      categories.value = response.data
    }
  } catch (error) {
    console.error('Error loading categories:', error)
  }
}

const loadSpecials = async () => {
  if (isNewVenue.value) return

  loadingSpecials.value = true
  try {
    const response = await apiService.getSpecialsByVenue(venueId.value!)
    if (response.success) {
      specials.value = response.data
    }
  } catch (error) {
    console.error('Error loading specials:', error)
  } finally {
    loadingSpecials.value = false
  }
}

const loadSpecialCategories = async () => {
  // No longer needed - categories are loaded in special detail view
}

const onAddressGeocoded = (result: GeocodeResult) => {
  // Clear address validation errors when address is successfully geocoded
  errors.value.streetAddress = ''
  errors.value.locality = ''
  errors.value.region = ''
  errors.value.postalCode = ''
  errors.value.country = ''
  
  // SAVE THE COORDINATES TO THE FORM!
  form.value.latitude = result.latitude
  form.value.longitude = result.longitude
  
  console.log('Address geocoded successfully:', result)
  console.log('Saved coordinates to form:', { latitude: form.value.latitude, longitude: form.value.longitude })
}

const startEditing = () => {
  isEditing.value = true
  
  // Track editing start
  trackForm('start', 'venue_edit_form', {
    venue_id: venueId.value,
    venue_name: venue.value?.name,
    is_new_venue: isNewVenue.value
  })
}

const cancelEditing = () => {
  if (isNewVenue.value) {
    router.go(-1)
    return
  }
  
  isEditing.value = false
  // Reset form to original venue data
  if (venue.value) {
    form.value = {
      categoryId: venue.value.categoryId,
      name: venue.value.name,
      description: venue.value.description || '',
      streetAddress: venue.value.streetAddress,
      locality: venue.value.locality,
      region: venue.value.region,
      postalCode: venue.value.postalCode,
      country: venue.value.country,
      phoneNumber: venue.value.phoneNumber || '',
      email: venue.value.email || '',
      website: venue.value.website || '',
      isActive: venue.value.isActive,
      businessHours: venue.value.businessHours?.map(bh => ({
        dayOfWeekId: bh.dayOfWeekId,
        openTime: bh.isClosed ? undefined : bh.openTime,
        closeTime: bh.isClosed ? undefined : bh.closeTime,
        isClosed: bh.isClosed
      })) || []
    }
  }
  errors.value = {}
  submitError.value = ''
}

const validateForm = (): boolean => {
  errors.value = {}

  if (!form.value.name.trim()) {
    errors.value.name = 'Venue name is required'
  }

  if (!form.value.categoryId) {
    errors.value.categoryId = 'Category is required'
  }

  if (!form.value.streetAddress.trim()) {
    errors.value.streetAddress = 'Street address is required'
  }

  if (!form.value.locality.trim()) {
    errors.value.locality = 'City is required'
  }

  if (!form.value.region.trim()) {
    errors.value.region = 'State/Region is required'
  }

  if (!form.value.postalCode.trim()) {
    errors.value.postalCode = 'Postal code is required'
  }

  return Object.keys(errors.value).length === 0
}

const saveVenue = async () => {
  if (!validateForm()) {
    // Track form validation failure
    trackForm('submit', 'venue_form', {
      success: false,
      errorMessage: 'Form validation failed',
      venue_id: venueId.value,
      is_new_venue: isNewVenue.value,
      error_fields: Object.keys(errors.value)
    })
    return
  }

  saving.value = true
  submitError.value = ''

  try {
    // Clean up empty optional fields
    const venueData = { ...form.value }
    if (!venueData.description) venueData.description = undefined
    if (!venueData.phoneNumber) venueData.phoneNumber = undefined
    if (!venueData.email) venueData.email = undefined
    if (!venueData.website) venueData.website = undefined

    if (isNewVenue.value) {
      const response = await apiService.createVenue(venueData as CreateVenueRequest)
      if (!response.success) {
        throw new Error(response.message || 'Failed to create venue')
      }
      
      // Track successful venue creation
      trackForm('submit', 'venue_create_form', {
        success: true,
        venue_id: response.data?.id,
        venue_name: venueData.name,
        venue_category: venueData.categoryId
      })
      
      // Track venue management action
      trackEvent('venue_management', {
        action: 'create',
        venue_id: response.data?.id,
        venue_name: venueData.name,
        venue_category: venueData.categoryId
      })
      
      // Navigate back to the backoffice main page instead of the new venue's detail page
      router.replace('/backoffice')
    } else {
      const response = await apiService.updateVenue(venueId.value!, venueData as UpdateVenueRequest)
      if (!response.success) {
        throw new Error(response.message || 'Failed to update venue')
      }
      
      // Track successful venue update
      trackForm('submit', 'venue_edit_form', {
        success: true,
        venue_id: venueId.value,
        venue_name: venueData.name,
        venue_category: venueData.categoryId
      })
      
      // Track venue management action
      trackEvent('venue_management', {
        action: 'edit',
        venue_id: venueId.value,
        venue_name: venueData.name,
        venue_category: venueData.categoryId
      })
      
      // Refresh venue data and exit edit mode
      await loadVenue()
      isEditing.value = false
    }
  } catch (error) {
    console.error('Error saving venue:', error)
    const errorMessage = error instanceof Error ? error.message : 'An unexpected error occurred'
    submitError.value = errorMessage
    
    // Track form submission error
    trackForm('submit', isNewVenue.value ? 'venue_create_form' : 'venue_edit_form', {
      success: false,
      errorMessage: errorMessage,
      venue_id: venueId.value,
      is_new_venue: isNewVenue.value
    })
    
    // Track error
    trackError('venue_save_error', errorMessage, {
      venue_id: venueId.value,
      is_new_venue: isNewVenue.value,
      form_data: form.value
    })
  } finally {
    saving.value = false
  }
}

// Special management methods
const editSpecial = (special: SpecialSummary) => {
  // Track special edit navigation
  trackEvent('special_management_navigation', {
    action: 'edit',
    special_id: special.id,
    special_title: special.title,
    venue_id: venueId.value
  })
  
  router.push(`/backoffice/venues/${venueId.value}/specials/${special.id}`)
}

const confirmDeleteSpecial = (special: SpecialSummary) => {
  // Track special delete intent
  trackEvent('special_management_navigation', {
    action: 'delete_confirm',
    special_id: special.id,
    special_title: special.title,
    venue_id: venueId.value
  })
  
  router.push({
    path: '/confirm',
    query: {
      type: 'special',
      id: special.id.toString(),
      name: special.title,
      returnTo: route.path
    }
  })
}

const deleteSpecial = async () => {
  // This function is now handled by ConfirmView
  // Reload specials if we're returning from a successful delete
  if (route.query.success) {
    await loadSpecials()
  }
}

// Watch for route changes
watch(() => route.params.id, async (newId, oldId) => {
  // Only react if the ID actually changed and avoid initial trigger
  if (newId !== oldId) {
    await loadVenue()
    if (!isNewVenue.value) {
      await loadSpecials()
    }
  }
})

onMounted(async () => {
  // Track page view for venue management
  if (isNewVenue.value) {
    trackContentView('venue_creation', 'new', 'New Venue Creation')
  } else {
    trackContentView('venue_detail', venueId.value?.toString() || 'unknown', venue.value?.name || 'Unknown Venue')
  }
  
  await Promise.all([
    loadCategories(),
    loadVenue()
  ])
  
  if (!isNewVenue.value) {
    await loadSpecials()
  }
})
</script>
