<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Header -->
    <div class="bg-white shadow">
      <div class="px-4 sm:px-6 lg:px-8 py-6">
        <div class="flex flex-col space-y-4 lg:flex-row lg:items-center lg:justify-between lg:space-y-0">
          <div class="flex items-center">
            <button
              @click="goBack"
              class="mr-4 p-2 text-gray-400 hover:text-gray-600"
            >
              <ChevronLeftIcon class="h-5 w-5" />
            </button>
            <div class="min-w-0 flex-1">
              <h1 class="text-xl sm:text-2xl font-bold text-gray-900 truncate">Create New Special</h1>
              <p class="mt-1 text-sm text-gray-500 truncate">
                <span v-if="venue">{{ venue.name }}</span>
              </p>
            </div>
          </div>
          <div class="flex flex-wrap items-center justify-end gap-2 lg:justify-start lg:space-x-3 lg:flex-nowrap">
            <button
              @click="goBack"
              class="inline-flex items-center px-3 py-2 lg:px-4 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-gray-500"
            >
              Cancel
            </button>
            <button
              @click="createSpecial"
              :disabled="saving"
              class="inline-flex items-center px-3 py-2 lg:px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-green-600 hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500 disabled:opacity-50"
            >
              <span v-if="!saving">Create Special</span>
              <span v-else class="flex items-center">
                <svg class="animate-spin -ml-1 mr-3 h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                Creating...
              </span>
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Content -->
    <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Error Message -->
      <div v-if="submitError" class="mb-6 bg-red-50 border border-red-200 rounded-md p-4">
        <div class="flex">
          <ExclamationTriangleIcon class="h-5 w-5 text-red-400" />
          <div class="ml-3">
            <p class="text-sm text-red-600">{{ submitError }}</p>
          </div>
        </div>
      </div>

      <!-- Form -->
      <div class="bg-white shadow rounded-lg">
        <div class="px-6 py-6">
          <div class="space-y-8">
            <!-- Basic Information -->
            <div class="space-y-6">
              <div>
                <h3 class="text-lg font-medium text-gray-900">Basic Information</h3>
                <p class="mt-1 text-sm text-gray-500">Enter the details for this special offer.</p>
              </div>

              <div class="grid grid-cols-1 gap-6">
                <div>
                  <label for="title" class="block text-sm font-medium text-gray-700">Title *</label>
                  <input
                    v-model="form.title"
                    type="text"
                    id="title"
                    required
                    class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-green-500 focus:border-green-500 sm:text-sm"
                    :class="{ 'border-red-300': errors.title }"
                  />
                  <p v-if="errors.title" class="mt-1 text-sm text-red-600">{{ errors.title }}</p>
                </div>

                <div>
                  <label for="specialCategoryId" class="block text-sm font-medium text-gray-700">Category *</label>
                  <select
                    v-model="form.specialCategoryId"
                    id="specialCategoryId"
                    required
                    class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-green-500 focus:border-green-500 sm:text-sm"
                    :class="{ 'border-red-300': errors.specialCategoryId }"
                  >
                    <option value="">Select a category</option>
                    <option v-for="category in categories" :key="category.id" :value="category.id">
                      {{ category.name }}
                    </option>
                  </select>
                  <p v-if="errors.specialCategoryId" class="mt-1 text-sm text-red-600">{{ errors.specialCategoryId }}</p>
                </div>

                <div>
                  <label for="description" class="block text-sm font-medium text-gray-700">Description *</label>
                  <textarea
                    v-model="form.description"
                    id="description"
                    rows="3"
                    required
                    class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-green-500 focus:border-green-500 sm:text-sm"
                    :class="{ 'border-red-300': errors.description }"
                  ></textarea>
                  <p v-if="errors.description" class="mt-1 text-sm text-red-600">{{ errors.description }}</p>
                </div>

                <div class="flex items-center">
                  <input
                    v-model="form.isActive"
                    id="isActive"
                    type="checkbox"
                    class="h-4 w-4 text-green-600 focus:ring-green-500 border-gray-300 rounded"
                  />
                  <label for="isActive" class="ml-2 block text-sm text-gray-900">
                    Active special
                  </label>
                </div>
              </div>
            </div>

            <!-- Schedule Information -->
            <div class="space-y-6">
              <div>
                <h3 class="text-lg font-medium text-gray-900">Schedule</h3>
                <p class="mt-1 text-sm text-gray-500">Set when this special will be active.</p>
              </div>
              
              <div class="space-y-6">
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4 md:gap-6">
                  <div>
                    <label for="startDate" class="block text-sm font-medium text-gray-700">Start Date *</label>
                    <input
                      v-model="form.startDate"
                      type="date"
                      id="startDate"
                      required
                      class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-green-500 focus:border-green-500 text-sm px-3 py-2"
                      :class="{ 'border-red-300': errors.startDate }"
                    />
                    <p v-if="errors.startDate" class="mt-1 text-sm text-red-600">{{ errors.startDate }}</p>
                  </div>

                  <div>
                    <label for="endDate" class="block text-sm font-medium text-gray-700">End Date</label>
                    <input
                      v-model="form.endDate"
                      type="date"
                      id="endDate"
                      class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-green-500 focus:border-green-500 text-sm px-3 py-2"
                      :class="{ 'border-red-300': errors.endDate }"
                    />
                    <p v-if="errors.endDate" class="mt-1 text-sm text-red-600">{{ errors.endDate }}</p>
                  </div>
                </div>

                <!-- All Day Special Checkbox -->
                <div>
                  <div class="flex items-center">
                    <input
                      v-model="isAllDay"
                      id="isAllDay"
                      type="checkbox"
                      class="h-4 w-4 text-green-600 focus:ring-green-500 border-gray-300 rounded"
                      @change="handleAllDayChange"
                    />
                    <label for="isAllDay" class="ml-2 block text-sm text-gray-900">
                      All Day Special
                    </label>
                  </div>
                  <p class="mt-1 text-sm text-gray-500">Check this if the special runs all day (no specific start/end times)</p>
                </div>

                <div class="grid grid-cols-1 md:grid-cols-2 gap-4 md:gap-6">
                  <div>
                    <label for="startTime" class="block text-sm font-medium text-gray-700">
                      Start Time {{ !isAllDay ? '*' : '' }}
                    </label>
                    <input
                      v-model="form.startTime"
                      type="time"
                      id="startTime"
                      :required="!isAllDay"
                      :disabled="isAllDay"
                      class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-green-500 focus:border-green-500 text-sm px-3 py-2"
                      :class="{ 
                        'border-red-300': errors.startTime, 
                        'bg-gray-100 cursor-not-allowed': isAllDay 
                      }"
                    />
                    <p v-if="errors.startTime" class="mt-1 text-sm text-red-600">{{ errors.startTime }}</p>
                  </div>

                  <div>
                    <label for="endTime" class="block text-sm font-medium text-gray-700">End Time</label>
                    <input
                      v-model="form.endTime"
                      type="time"
                      id="endTime"
                      :disabled="isAllDay"
                      class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-green-500 focus:border-green-500 text-sm px-3 py-2"
                      :class="{ 
                        'border-red-300': errors.endTime,
                        'bg-gray-100 cursor-not-allowed': isAllDay 
                      }"
                    />
                    <p v-if="errors.endTime" class="mt-1 text-sm text-red-600">{{ errors.endTime }}</p>
                  </div>
                </div>
              </div>

              <!-- Recurring Schedule -->
              <div class="space-y-4">
                <div class="flex items-center">
                  <input
                    v-model="form.isRecurring"
                    id="isRecurring"
                    type="checkbox"
                    class="h-4 w-4 text-green-600 focus:ring-green-500 border-gray-300 rounded"
                  />
                  <label for="isRecurring" class="ml-2 block text-sm text-gray-900">
                    Recurring special
                  </label>
                </div>

                <div v-if="form.isRecurring" class="pl-6 border-l-2 border-green-200">
                  <div class="space-y-3">
                    <label class="block text-sm font-medium text-gray-700">
                      Cron Schedule
                    </label>
                    <CronScheduler
                      v-model="form.cronSchedule"
                      :start-time="form.startTime"
                      :selected-date="form.startDate"
                      :show-presets="true"
                      :show-builder="true"
                      @valid="onCronValid"
                    />
                    <p v-if="errors.cronSchedule" class="mt-1 text-sm text-red-600">{{ errors.cronSchedule }}</p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { ChevronLeftIcon, ExclamationTriangleIcon } from '@heroicons/vue/24/outline'
import CronScheduler from '@/components/common/CronScheduler.vue'
import { apiService } from '@/services/api'
import { useAnalytics } from '@/composables/useAnalytics'
import type { VenueSummary, SpecialCategory, CreateSpecialRequest } from '@/types/api'

interface Props {
  venueId?: string
}

const props = defineProps<Props>()
const router = useRouter()
const route = useRoute()

// Analytics
const { trackForm, trackEvent, trackContentView, trackError } = useAnalytics()

// State
const venue = ref<VenueSummary | null>(null)
const categories = ref<SpecialCategory[]>([])
const saving = ref(false)
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

const isAllDay = ref(false)

const errors = ref<Record<string, string>>({})

// Computed
const venueId = computed(() => {
  return parseInt(props.venueId || route.params.venueId as string)
})

// Methods
const handleAllDayChange = () => {
  if (isAllDay.value) {
    // Set all day times (00:00 to 23:59)
    form.value.startTime = '00:00'
    form.value.endTime = '23:59'
    // Clear any time-related errors
    errors.value.startTime = ''
    errors.value.endTime = ''
  } else {
    // Reset to default business hours when unchecked
    form.value.startTime = '17:00'
    form.value.endTime = ''
  }
}

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

const goBack = () => {
  router.back()
}

const onCronValid = (valid: boolean) => {
  if (valid && errors.value.cronSchedule) {
    delete errors.value.cronSchedule
  }
}

const validateForm = (): boolean => {
  errors.value = {}

  if (!form.value.title?.trim()) {
    errors.value.title = 'Title is required'
  }

  if (!form.value.specialCategoryId) {
    errors.value.specialCategoryId = 'Category is required'
  }

  if (!form.value.description?.trim()) {
    errors.value.description = 'Description is required'
  }

  if (!form.value.startDate) {
    errors.value.startDate = 'Start date is required'
  }

  if (!form.value.startTime && !isAllDay.value) {
    errors.value.startTime = 'Start time is required'
  }

  if (form.value.endDate && form.value.startDate && form.value.endDate < form.value.startDate) {
    errors.value.endDate = 'End date must be after start date'
  }

  if (form.value.endTime && form.value.startTime && form.value.endTime <= form.value.startTime && !isAllDay.value) {
    errors.value.endTime = 'End time must be after start time'
  }

  if (form.value.isRecurring && !form.value.cronSchedule?.trim()) {
    errors.value.cronSchedule = 'Cron schedule is required for recurring specials'
  }

  return Object.keys(errors.value).length === 0
}

const createSpecial = async () => {
  if (!validateForm()) {
    // Track form validation failure
    trackForm('submit', 'special_create_form', {
      success: false,
      errorMessage: 'Form validation failed',
      venue_id: venueId.value,
      error_fields: Object.keys(errors.value)
    })
    return
  }

  saving.value = true
  submitError.value = ''

  try {
    // Clean up empty optional fields
    const specialData = { ...form.value }
    specialData.venueId = venueId.value
    if (!specialData.endTime) specialData.endTime = undefined
    if (!specialData.endDate) specialData.endDate = undefined
    if (!specialData.isRecurring) specialData.cronSchedule = undefined

    const response = await apiService.createSpecial(venueId.value, specialData)
    if (!response.success) {
      throw new Error(response.message || 'Failed to create special')
    }
    
    // Track successful special creation
    trackForm('submit', 'special_create_form', {
      success: true,
      special_id: response.data?.id,
      special_title: specialData.title,
      venue_id: venueId.value,
      special_category: specialData.specialCategoryId,
      is_recurring: specialData.isRecurring,
      is_active: specialData.isActive
    })
    
    // Track special management action
    trackEvent('special_management', {
      action: 'create',
      special_id: response.data?.id,
      special_title: specialData.title,
      venue_id: venueId.value,
      special_category: specialData.specialCategoryId,
      is_recurring: specialData.isRecurring,
      is_active: specialData.isActive
    })
    
    // Navigate back to the venue detail page instead of the new special's detail page
    router.replace(`/backoffice/venues/${venueId.value}`)
  } catch (error) {
    console.error('Error creating special:', error)
    const errorMessage = error instanceof Error ? error.message : 'An unexpected error occurred'
    submitError.value = errorMessage
    
    // Track form submission error
    trackForm('submit', 'special_create_form', {
      success: false,
      errorMessage: errorMessage,
      venue_id: venueId.value
    })
    
    // Track error
    trackError('special_create_error', errorMessage, {
      venue_id: venueId.value,
      form_data: form.value
    })
  } finally {
    saving.value = false
  }
}

onMounted(async () => {
  // Track special creation page view
  trackContentView('special_creation', venueId.value.toString(), 'New Special Creation')
  
  // Track form start
  trackForm('start', 'special_create_form', {
    venue_id: venueId.value
  })
  
  // Set venue ID and default values
  form.value.venueId = venueId.value
  form.value.startDate = new Date().toISOString().split('T')[0]
  form.value.startTime = '17:00'

  await Promise.all([
    loadVenue(),
    loadCategories()
  ])
})
</script>
