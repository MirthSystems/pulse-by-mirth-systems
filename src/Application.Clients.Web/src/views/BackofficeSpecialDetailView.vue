<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Header -->
    <div class="bg-white shadow">
      <div class="px-4 sm:px-6 lg:px-8 py-6">
        <div class="flex items-center justify-between">
          <div class="flex items-center">
            <button
              @click="goBack"
              class="mr-4 p-2 text-gray-400 hover:text-gray-600"
            >
              <ChevronLeftIcon class="h-5 w-5" />
            </button>
            <div>
              <h1 class="text-2xl font-bold text-gray-900">
                {{ isNewSpecial ? 'Create New Special' : 'Special Details' }}
              </h1>
              <p class="mt-1 text-sm text-gray-500">
                <span v-if="venue">{{ venue.name }}</span>
                <span v-if="!isNewSpecial && special"> - {{ special.title }}</span>
              </p>
            </div>
          </div>
          <div class="flex items-center space-x-3">
            <button
              v-if="!isNewSpecial && !isEditing"
              @click="startEditing"
              class="inline-flex items-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500"
            >
              <PencilIcon class="-ml-1 mr-2 h-4 w-4" />
              Edit Special
            </button>
            <button
              v-if="isEditing"
              @click="cancelEditing"
              class="inline-flex items-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-gray-500"
            >
              Cancel
            </button>
            <button
              v-if="isEditing || isNewSpecial"
              @click="saveSpecial"
              :disabled="saving"
              class="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-green-600 hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500 disabled:opacity-50"
            >
              <span v-if="!saving">{{ isNewSpecial ? 'Create Special' : 'Save Changes' }}</span>
              <span v-else class="flex items-center">
                <svg class="animate-spin -ml-1 mr-3 h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                {{ isNewSpecial ? 'Creating...' : 'Saving...' }}
              </span>
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Content -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div v-if="loading" class="flex items-center justify-center py-12">
        <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-green-600"></div>
        <span class="ml-2 text-gray-600">Loading special...</span>
      </div>

      <div v-else class="space-y-8">
        <!-- Special Information Form -->
        <div class="bg-white shadow rounded-lg">
          <div class="px-6 py-4 border-b border-gray-200">
            <h3 class="text-lg leading-6 font-medium text-gray-900">Special Information</h3>
          </div>
          
          <form @submit.prevent="saveSpecial" class="p-6">
            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <!-- Basic Information -->
              <div class="space-y-4">
                <h4 class="text-md font-medium text-gray-900">Basic Information</h4>
                
                <div>
                  <label for="title" class="block text-sm font-medium text-gray-700">Title *</label>
                  <input
                    v-model="form.title"
                    type="text"
                    id="title"
                    :readonly="!isEditing && !isNewSpecial"
                    required
                    class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-green-500 focus:border-green-500 sm:text-sm"
                    :class="{ 'bg-gray-50': !isEditing && !isNewSpecial, 'border-red-300': errors.title }"
                  />
                  <p v-if="errors.title" class="mt-1 text-sm text-red-600">{{ errors.title }}</p>
                </div>

                <div>
                  <label for="category" class="block text-sm font-medium text-gray-700">Category *</label>
                  <select
                    v-model="form.specialCategoryId"
                    id="category"
                    :disabled="!isEditing && !isNewSpecial"
                    required
                    class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-green-500 focus:border-green-500 sm:text-sm"
                    :class="{ 'bg-gray-50': !isEditing && !isNewSpecial, 'border-red-300': errors.specialCategoryId }"
                  >
                    <option value="">Select a category</option>
                    <option v-for="category in categories" :key="category.id" :value="category.id">
                      {{ category.icon }} {{ category.name }}
                    </option>
                  </select>
                  <p v-if="errors.specialCategoryId" class="mt-1 text-sm text-red-600">{{ errors.specialCategoryId }}</p>
                </div>

                <div>
                  <label for="description" class="block text-sm font-medium text-gray-700">Description *</label>
                  <textarea
                    v-model="form.description"
                    id="description"
                    rows="4"
                    :readonly="!isEditing && !isNewSpecial"
                    required
                    class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-green-500 focus:border-green-500 sm:text-sm"
                    :class="{ 'bg-gray-50': !isEditing && !isNewSpecial, 'border-red-300': errors.description }"
                  ></textarea>
                  <p v-if="errors.description" class="mt-1 text-sm text-red-600">{{ errors.description }}</p>
                </div>

                <div class="flex items-center">
                  <input
                    v-model="form.isActive"
                    id="isActive"
                    type="checkbox"
                    :disabled="!isEditing && !isNewSpecial"
                    class="h-4 w-4 text-green-600 focus:ring-green-500 border-gray-300 rounded"
                  />
                  <label for="isActive" class="ml-2 block text-sm text-gray-900">
                    Active special
                  </label>
                </div>
              </div>

              <!-- Schedule Information -->
              <div class="space-y-4">
                <h4 class="text-md font-medium text-gray-900">Schedule</h4>
                
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label for="startDate" class="block text-sm font-medium text-gray-700">Start Date *</label>
                    <input
                      v-model="form.startDate"
                      type="date"
                      id="startDate"
                      :readonly="!isEditing && !isNewSpecial"
                      required
                      class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-green-500 focus:border-green-500 sm:text-sm"
                      :class="{ 'bg-gray-50': !isEditing && !isNewSpecial, 'border-red-300': errors.startDate }"
                    />
                    <p v-if="errors.startDate" class="mt-1 text-sm text-red-600">{{ errors.startDate }}</p>
                  </div>

                  <div>
                    <label for="endDate" class="block text-sm font-medium text-gray-700">End Date</label>
                    <input
                      v-model="form.endDate"
                      type="date"
                      id="endDate"
                      :readonly="!isEditing && !isNewSpecial"
                      class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-green-500 focus:border-green-500 sm:text-sm"
                      :class="{ 'bg-gray-50': !isEditing && !isNewSpecial, 'border-red-300': errors.endDate }"
                    />
                    <p v-if="errors.endDate" class="mt-1 text-sm text-red-600">{{ errors.endDate }}</p>
                  </div>

                  <div>
                    <label for="startTime" class="block text-sm font-medium text-gray-700">Start Time *</label>
                    <input
                      v-model="form.startTime"
                      type="time"
                      id="startTime"
                      :readonly="!isEditing && !isNewSpecial"
                      required
                      class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-green-500 focus:border-green-500 sm:text-sm"
                      :class="{ 'bg-gray-50': !isEditing && !isNewSpecial, 'border-red-300': errors.startTime }"
                    />
                    <p v-if="errors.startTime" class="mt-1 text-sm text-red-600">{{ errors.startTime }}</p>
                  </div>

                  <div>
                    <label for="endTime" class="block text-sm font-medium text-gray-700">End Time</label>
                    <input
                      v-model="form.endTime"
                      type="time"
                      id="endTime"
                      :readonly="!isEditing && !isNewSpecial"
                      class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-green-500 focus:border-green-500 sm:text-sm"
                      :class="{ 'bg-gray-50': !isEditing && !isNewSpecial, 'border-red-300': errors.endTime }"
                    />
                    <p v-if="errors.endTime" class="mt-1 text-sm text-red-600">{{ errors.endTime }}</p>
                  </div>
                </div>

                <div class="space-y-3">
                  <div class="flex items-center">
                    <input
                      v-model="form.isRecurring"
                      id="isRecurring"
                      type="checkbox"
                      :disabled="!isEditing && !isNewSpecial"
                      class="h-4 w-4 text-green-600 focus:ring-green-500 border-gray-300 rounded"
                    />
                    <label for="isRecurring" class="ml-2 block text-sm text-gray-900">
                      Recurring special
                    </label>
                  </div>

                  <div v-if="form.isRecurring">
                    <label class="block text-sm font-medium text-gray-700 mb-2">
                      Cron Schedule
                    </label>
                    <CronScheduler
                      v-model="form.cronSchedule"
                      :start-time="form.startTime"
                      :disabled="!isEditing && !isNewSpecial"
                      :show-presets="true"
                      :show-builder="true"
                      @valid="onCronValid"
                    />
                    <p v-if="errors.cronSchedule" class="mt-1 text-sm text-red-600">{{ errors.cronSchedule }}</p>
                  </div>
                </div>
              </div>
            </div>

            <!-- Error Summary -->
            <div v-if="submitError" class="mt-6 p-4 bg-red-50 border border-red-200 rounded-md">
              <div class="flex">
                <ExclamationTriangleIcon class="h-5 w-5 text-red-400" />
                <div class="ml-3">
                  <h3 class="text-sm font-medium text-red-800">
                    Error saving special
                  </h3>
                  <div class="mt-2 text-sm text-red-700">
                    <p>{{ submitError }}</p>
                  </div>
                </div>
              </div>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import apiService from '@/services/api'
import type { 
  VenueSummary, 
  SpecialSummary, 
  SpecialCategory,
  CreateSpecialRequest, 
  UpdateSpecialRequest 
} from '@/types/api'
import {
  ChevronLeftIcon,
  PencilIcon,
  ExclamationTriangleIcon
} from '@heroicons/vue/24/outline'
import CronScheduler from '@/components/common/CronScheduler.vue'

const route = useRoute()
const router = useRouter()

// Props
interface Props {
  venueId?: string
  specialId?: string
}

const props = defineProps<Props>()

// State
const venue = ref<VenueSummary | null>(null)
const special = ref<SpecialSummary | null>(null)
const categories = ref<SpecialCategory[]>([])
const loading = ref(true)
const saving = ref(false)
const isEditing = ref(false)
const submitError = ref('')

// Form state
const form = ref<CreateSpecialRequest>({
  venueId: 0,
  specialCategoryId: 0,
  title: '',
  description: '',
  startDate: '',
  startTime: '',
  endTime: '',
  endDate: '',
  isRecurring: false,
  cronSchedule: '',
  isActive: true
})

const errors = ref<Record<string, string>>({})

// Computed
const venueId = computed(() => {
  return parseInt(props.venueId || route.params.venueId as string)
})

const specialId = computed(() => {
  const id = props.specialId || route.params.specialId as string
  return id === 'new' ? null : parseInt(id)
})

const isNewSpecial = computed(() => specialId.value === null)

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

const loadSpecial = async () => {
  if (isNewSpecial.value) {
    loading.value = false
    isEditing.value = true
    // Set venue ID for new special
    form.value.venueId = venueId.value
    // Set default values
    form.value.startDate = new Date().toISOString().split('T')[0]
    form.value.startTime = '17:00'
    return
  }

  // Only load if we have a valid special ID
  if (!specialId.value || isNaN(specialId.value)) {
    loading.value = false
    return
  }

  loading.value = true
  try {
    const response = await apiService.getSpecial(specialId.value)
    if (response.success) {
      special.value = response.data
      // Populate form with special data
      form.value = {
        venueId: special.value.venueId,
        specialCategoryId: special.value.specialCategoryId,
        title: special.value.title,
        description: special.value.description,
        startDate: special.value.startDate,
        startTime: special.value.startTime,
        endTime: special.value.endTime || '',
        endDate: special.value.endDate || '',
        isRecurring: special.value.isRecurring,
        cronSchedule: special.value.cronSchedule || '',
        isActive: special.value.isActive
      }
    }
  } catch (error) {
    console.error('Error loading special:', error)
  } finally {
    loading.value = false
  }
}

const loadCategories = async () => {
  try {
    const response = await apiService.getSpecialCategories()
    if (response.success) {
      categories.value = response.data
    }
  } catch (error) {
    console.error('Error loading categories:', error)
  }
}

const startEditing = () => {
  isEditing.value = true
}

const cancelEditing = () => {
  if (isNewSpecial.value) {
    goBack()
    return
  }
  
  isEditing.value = false
  // Reset form to original special data
  if (special.value) {
    form.value = {
      venueId: special.value.venueId,
      specialCategoryId: special.value.specialCategoryId,
      title: special.value.title,
      description: special.value.description,
      startDate: special.value.startDate,
      startTime: special.value.startTime,
      endTime: special.value.endTime || '',
      endDate: special.value.endDate || '',
      isRecurring: special.value.isRecurring,
      cronSchedule: special.value.cronSchedule || '',
      isActive: special.value.isActive
    }
  }
  errors.value = {}
  submitError.value = ''
}

const goBack = () => {
  router.push(`/backoffice/venues/${venueId.value}`)
}

// Cron validation handler
const onCronValid = (isValid: boolean) => {
  if (form.value.isRecurring) {
    if (isValid) {
      delete errors.value.cronSchedule
    } else {
      errors.value.cronSchedule = 'Invalid cron expression'
    }
  }
}

const validateForm = (): boolean => {
  errors.value = {}

  if (!form.value.title.trim()) {
    errors.value.title = 'Title is required'
  }

  if (!form.value.specialCategoryId) {
    errors.value.specialCategoryId = 'Category is required'
  }

  if (!form.value.description.trim()) {
    errors.value.description = 'Description is required'
  }

  if (!form.value.startDate) {
    errors.value.startDate = 'Start date is required'
  }

  if (!form.value.startTime) {
    errors.value.startTime = 'Start time is required'
  }

  if (form.value.endDate && form.value.startDate && form.value.endDate < form.value.startDate) {
    errors.value.endDate = 'End date must be after start date'
  }

  if (form.value.endTime && form.value.startTime && form.value.endTime <= form.value.startTime) {
    errors.value.endTime = 'End time must be after start time'
  }

  if (form.value.isRecurring && !form.value.cronSchedule?.trim()) {
    errors.value.cronSchedule = 'Cron schedule is required for recurring specials'
  }

  return Object.keys(errors.value).length === 0
}

const saveSpecial = async () => {
  if (!validateForm()) {
    return
  }

  saving.value = true
  submitError.value = ''

  try {
    // Clean up empty optional fields
    const specialData = { ...form.value }
    if (!specialData.endTime) specialData.endTime = undefined
    if (!specialData.endDate) specialData.endDate = undefined
    if (!specialData.isRecurring) specialData.cronSchedule = undefined

    if (isNewSpecial.value) {
      const response = await apiService.createSpecial(specialData as CreateSpecialRequest)
      if (!response.success) {
        throw new Error(response.message || 'Failed to create special')
      }
      // Navigate to the new special's detail page
      router.replace(`/backoffice/venues/${venueId.value}/specials/${response.data.id}`)
    } else {
      const response = await apiService.updateSpecial(specialId.value!, specialData as UpdateSpecialRequest)
      if (!response.success) {
        throw new Error(response.message || 'Failed to update special')
      }
      // Refresh special data and exit edit mode
      await loadSpecial()
      isEditing.value = false
    }
  } catch (error) {
    console.error('Error saving special:', error)
    submitError.value = error instanceof Error ? error.message : 'An unexpected error occurred'
  } finally {
    saving.value = false
  }
}

// Watch for route changes
watch(() => [route.params.venueId, route.params.specialId], async () => {
  // Reset state when route changes
  special.value = null
  errors.value = {}
  submitError.value = ''
  isEditing.value = false
  
  await loadSpecial()
}, { immediate: true })

onMounted(async () => {
  await Promise.all([
    loadVenue(),
    loadCategories()
    // loadSpecial() is handled by the watch function
  ])
})
</script>
