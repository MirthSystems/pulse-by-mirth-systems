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
              <h1 class="text-xl sm:text-2xl font-bold text-gray-900 truncate">Special Details</h1>
              <p class="mt-1 text-sm text-gray-500 truncate">
                <span v-if="venue">{{ venue.name }}</span>
                <span v-if="special"> - {{ special.title }}</span>
              </p>
            </div>
          </div>
          <div class="flex flex-wrap items-center justify-end gap-2 lg:justify-start lg:space-x-3 lg:flex-nowrap">
            <button
              v-if="!isEditing"
              @click="startEditing"
              class="inline-flex items-center px-3 py-2 lg:px-4 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500"
            >
              <PencilIcon class="-ml-1 mr-2 h-4 w-4" />
              <span class="hidden lg:inline">Edit Special</span>
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
              v-if="isEditing"
              @click="saveSpecial"
              :disabled="saving"
              class="inline-flex items-center px-3 py-2 lg:px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-green-600 hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500 disabled:opacity-50"
            >
              <span v-if="!saving">Save Changes</span>
              <span v-else class="flex items-center">
                <svg class="animate-spin -ml-1 mr-3 h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                Saving...
              </span>
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div class="bg-white shadow rounded-lg p-6">
        <div class="animate-pulse">
          <div class="h-4 bg-gray-200 rounded w-1/4 mb-4"></div>
          <div class="h-4 bg-gray-200 rounded w-1/2 mb-2"></div>
          <div class="h-4 bg-gray-200 rounded w-3/4"></div>
        </div>
      </div>
    </div>

    <!-- Content -->
    <div v-else class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
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
                <p class="mt-1 text-sm text-gray-500">View and edit the details for this special offer.</p>
              </div>

              <div class="grid grid-cols-1 gap-6">
                <div>
                  <label for="title" class="block text-sm font-medium text-gray-700">Title *</label>
                  <input
                    v-model="form.title"
                    type="text"
                    id="title"
                    :readonly="!isEditing"
                    required
                    class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-green-500 focus:border-green-500 sm:text-sm"
                    :class="{ 'bg-gray-50': !isEditing, 'border-red-300': errors.title }"
                  />
                  <p v-if="errors.title" class="mt-1 text-sm text-red-600">{{ errors.title }}</p>
                </div>

                <div>
                  <label for="specialCategoryId" class="block text-sm font-medium text-gray-700">Category *</label>
                  <select
                    v-model="form.specialCategoryId"
                    id="specialCategoryId"
                    :disabled="!isEditing"
                    required
                    class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-green-500 focus:border-green-500 sm:text-sm"
                    :class="{ 'bg-gray-50': !isEditing, 'border-red-300': errors.specialCategoryId }"
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
                    :readonly="!isEditing"
                    required
                    class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-green-500 focus:border-green-500 sm:text-sm"
                    :class="{ 'bg-gray-50': !isEditing, 'border-red-300': errors.description }"
                  ></textarea>
                  <p v-if="errors.description" class="mt-1 text-sm text-red-600">{{ errors.description }}</p>
                </div>

                <div class="flex items-center">
                  <input
                    v-model="form.isActive"
                    id="isActive"
                    type="checkbox"
                    :disabled="!isEditing"
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
                <p class="mt-1 text-sm text-gray-500">When this special is active.</p>
              </div>
              
              <div class="space-y-6">
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4 md:gap-6">
                  <div>
                    <label for="startDate" class="block text-sm font-medium text-gray-700">Start Date *</label>
                    <input
                      v-model="form.startDate"
                      type="date"
                      id="startDate"
                      :readonly="!isEditing"
                      required
                      class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-green-500 focus:border-green-500 text-sm px-3 py-2"
                      :class="{ 'bg-gray-50': !isEditing, 'border-red-300': errors.startDate }"
                    />
                    <p v-if="errors.startDate" class="mt-1 text-sm text-red-600">{{ errors.startDate }}</p>
                  </div>

                  <div>
                    <label for="endDate" class="block text-sm font-medium text-gray-700">End Date</label>
                    <input
                      v-model="form.endDate"
                      type="date"
                      id="endDate"
                      :readonly="!isEditing"
                      class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-green-500 focus:border-green-500 text-sm px-3 py-2"
                      :class="{ 'bg-gray-50': !isEditing, 'border-red-300': errors.endDate }"
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
                      :disabled="!isEditing"
                      class="h-4 w-4 text-green-600 focus:ring-green-500 border-gray-300 rounded"
                      :class="{ 'opacity-50 cursor-not-allowed': !isEditing }"
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
                      :readonly="!isEditing"
                      :required="!isAllDay"
                      :disabled="isAllDay || !isEditing"
                      class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-green-500 focus:border-green-500 text-sm px-3 py-2"
                      :class="{ 
                        'bg-gray-50': !isEditing || isAllDay, 
                        'border-red-300': errors.startTime,
                        'cursor-not-allowed': isAllDay || !isEditing
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
                      :readonly="!isEditing"
                      :disabled="isAllDay || !isEditing"
                      class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-green-500 focus:border-green-500 text-sm px-3 py-2"
                      :class="{ 
                        'bg-gray-50': !isEditing || isAllDay, 
                        'border-red-300': errors.endTime,
                        'cursor-not-allowed': isAllDay || !isEditing
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
                    :disabled="!isEditing"
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
                      :disabled="!isEditing"
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
import { ref, computed, watch, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { ChevronLeftIcon, ExclamationTriangleIcon, PencilIcon } from '@heroicons/vue/24/outline'
import CronScheduler from '@/components/common/CronScheduler.vue'
import { apiService } from '@/services/api'
import { useAnalytics } from '@/composables/useAnalytics'
import type { VenueSummary, SpecialCategory, SpecialSummary, UpdateSpecialRequest } from '@/types/api'

interface Props {
  venueId?: string
  specialId?: string
}

const props = defineProps<Props>()
const router = useRouter()
const route = useRoute()

// Analytics
const { trackForm, trackEvent, trackContentView, trackError } = useAnalytics()

// State
const venue = ref<VenueSummary | null>(null)
const special = ref<SpecialSummary | null>(null)
const categories = ref<SpecialCategory[]>([])
const loading = ref(true)
const saving = ref(false)
const isEditing = ref(false)
const submitError = ref('')

// Form state
const form = ref<UpdateSpecialRequest>({
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

const specialId = computed(() => {
  const id = props.specialId || route.params.specialId as string
  return parseInt(id)
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

const loadSpecial = async () => {
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
      
      // Check if this is an all-day special (00:00 to 23:59)
      isAllDay.value = special.value.startTime === '00:00' && special.value.endTime === '23:59'
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

const goBack = () => {
  router.back()
}

const startEditing = () => {
  isEditing.value = true
  
  // Track special editing start
  trackForm('start', 'special_edit_form', {
    special_id: specialId.value,
    special_title: special.value?.title,
    venue_id: venueId.value
  })
}

const cancelEditing = () => {
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
    
    // Reset all day flag
    isAllDay.value = special.value.startTime === '00:00' && special.value.endTime === '23:59'
  }
  errors.value = {}
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

const saveSpecial = async () => {
  if (!validateForm()) {
    // Track form validation failure
    trackForm('submit', 'special_edit_form', {
      success: false,
      errorMessage: 'Form validation failed',
      special_id: specialId.value,
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
    if (!specialData.endTime) specialData.endTime = undefined
    if (!specialData.endDate) specialData.endDate = undefined
    if (!specialData.isRecurring) specialData.cronSchedule = undefined

    const response = await apiService.updateSpecial(specialId.value, specialData)
    if (!response.success) {
      throw new Error(response.message || 'Failed to update special')
    }
    
    // Track successful special update
    trackForm('submit', 'special_edit_form', {
      success: true,
      special_id: specialId.value,
      special_title: specialData.title,
      venue_id: venueId.value,
      special_category: specialData.specialCategoryId,
      is_recurring: specialData.isRecurring,
      is_active: specialData.isActive
    })
    
    // Track special management action
    trackEvent('special_management', {
      action: 'edit',
      special_id: specialId.value,
      special_title: specialData.title,
      venue_id: venueId.value,
      special_category: specialData.specialCategoryId,
      is_recurring: specialData.isRecurring,
      is_active: specialData.isActive
    })
    
    // Refresh special data and exit edit mode
    await loadSpecial()
    isEditing.value = false
  } catch (error) {
    console.error('Error saving special:', error)
    const errorMessage = error instanceof Error ? error.message : 'An unexpected error occurred'
    submitError.value = errorMessage
    
    // Track form submission error
    trackForm('submit', 'special_edit_form', {
      success: false,
      errorMessage: errorMessage,
      special_id: specialId.value,
      venue_id: venueId.value
    })
    
    // Track error
    trackError('special_save_error', errorMessage, {
      special_id: specialId.value,
      venue_id: venueId.value,
      form_data: form.value
    })
  } finally {
    saving.value = false
  }
}

// Watch for route changes
watch(() => [route.params.venueId, route.params.specialId], () => {
  loadSpecial()
}, { immediate: true })

onMounted(async () => {
  // Track special detail page view
  trackContentView('special_detail', specialId.value.toString(), special.value?.title || 'Special Edit')
  
  await Promise.all([
    loadVenue(),
    loadCategories()
    // loadSpecial() is handled by the watch function
  ])
})
</script>
