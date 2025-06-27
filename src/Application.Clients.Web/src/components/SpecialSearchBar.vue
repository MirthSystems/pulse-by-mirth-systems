<template>
  <div class="bg-white shadow-sm border-b">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
      <!-- Main Search Bar -->
      <div class="max-w-2xl mx-auto">
        <div class="relative">
          <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <MagnifyingGlassIcon class="h-5 w-5 text-gray-400" />
          </div>
          <input
            v-model="searchTerm"
            @keyup.enter="performSearch"
            type="text"
            placeholder="Search for specials..."
            class="block w-full pl-10 pr-12 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent text-lg"
          />
          <div class="absolute inset-y-0 right-0 flex items-center">
            <button
              @click="performSearch"
              :disabled="isSearching"
              class="mr-2 px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 disabled:opacity-50"
            >
              <span v-if="isSearching" class="flex items-center">
                <svg class="animate-spin -ml-1 mr-2 h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                Searching...
              </span>
              <span v-else>Search</span>
            </button>
          </div>
        </div>
      </div>

      <!-- Location and Radius Controls -->
      <div class="max-w-4xl mx-auto mt-6">
        <div class="flex flex-wrap items-center justify-center gap-4">
          <!-- Location Input -->
          <div class="flex items-center space-x-2">
            <MapIcon class="h-4 w-4 text-gray-500" />
            <input
              v-model="locationInput"
              @keyup.enter="performSearch"
              type="text"
              placeholder="Enter location or use current"
              class="px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent text-sm"
            />
            <button
              @click="getCurrentLocation"
              :disabled="gettingLocation"
              class="px-3 py-2 bg-gray-100 text-gray-700 border border-gray-300 rounded-lg hover:bg-gray-200 text-sm"
            >
              {{ gettingLocation ? 'Getting...' : 'Current' }}
            </button>
          </div>

          <!-- Radius Dropdown -->
          <div class="flex items-center space-x-2">
            <label class="text-sm text-gray-700">Within:</label>
            <select 
              v-model="radius"
              class="px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent text-sm"
            >
              <option :value="1000">1 km</option>
              <option :value="2000">2 km</option>
              <option :value="5000">5 km</option>
              <option :value="10000">10 km</option>
              <option :value="25000">25 km</option>
            </select>
          </div>

          <!-- Filters Toggle -->
          <button
            @click="showFilters = !showFilters"
            class="inline-flex items-center px-4 py-2 bg-white text-gray-700 border border-gray-300 rounded-lg hover:bg-gray-50 text-sm font-medium"
          >
            <FunnelIcon class="h-4 w-4 mr-2" />
            Filters
            <span v-if="hasActiveFilters" class="ml-2 bg-blue-500 text-white text-xs rounded-full px-2 py-1">
              {{ activeFiltersCount }}
            </span>
          </button>

          <!-- Clear All -->
          <button
            v-if="hasAnyFilters"
            @click="clearAll"
            class="inline-flex items-center px-4 py-2 bg-red-50 text-red-700 border border-red-200 rounded-lg hover:bg-red-100 text-sm font-medium"
          >
            Clear All
          </button>
        </div>
      </div>

      <!-- Expandable Filters -->
      <div v-if="showFilters" class="max-w-4xl mx-auto mt-6 bg-gray-50 rounded-lg p-4">
        <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Category</label>
            <select 
              v-model="selectedCategory"
              class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            >
              <option :value="undefined">All Categories</option>
              <option 
                v-for="category in categories" 
                :key="category.id" 
                :value="category.id"
              >
                {{ category.name }}
              </option>
            </select>
          </div>
          
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Date</label>
            <input
              v-model="selectedDate"
              type="date"
              class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
          </div>
          
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Time</label>
            <input
              v-model="selectedTime"
              type="time"
              class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useSpecialStore } from '../stores/special'
import { 
  MagnifyingGlassIcon,
  MapIcon,
  FunnelIcon
} from '@heroicons/vue/24/outline'

interface SearchParams {
  searchTerm?: string
  latitude?: number
  longitude?: number
  radiusInMeters?: number
  categoryId?: number
  date?: string
  time?: string
}

const emit = defineEmits<{
  search: [params: SearchParams]
}>()

const specialStore = useSpecialStore()

// Search state
const searchTerm = ref('')
const locationInput = ref('')
const latitude = ref<number>()
const longitude = ref<number>()
const radius = ref(5000)
const selectedCategory = ref<number>()
const selectedDate = ref('')
const selectedTime = ref('')
const showFilters = ref(false)
const isSearching = ref(false)
const gettingLocation = ref(false)

// Computed
const categories = computed(() => specialStore.categories)

const hasActiveFilters = computed(() => 
  selectedCategory.value !== undefined || 
  selectedDate.value !== '' || 
  selectedTime.value !== ''
)

const activeFiltersCount = computed(() => {
  let count = 0
  if (selectedCategory.value !== undefined) count++
  if (selectedDate.value !== '') count++
  if (selectedTime.value !== '') count++
  return count
})

const hasAnyFilters = computed(() => 
  searchTerm.value !== '' || 
  locationInput.value !== '' || 
  latitude.value !== undefined || 
  hasActiveFilters.value ||
  radius.value !== 5000
)

// Methods
const performSearch = async () => {
  isSearching.value = true
  
  const params: SearchParams = {
    searchTerm: searchTerm.value.trim() || undefined,
    latitude: latitude.value,
    longitude: longitude.value,
    radiusInMeters: radius.value,
    categoryId: selectedCategory.value,
    date: selectedDate.value || undefined,
    time: selectedTime.value || undefined
  }
  
  emit('search', params)
  
  setTimeout(() => {
    isSearching.value = false
  }, 500)
}

const getCurrentLocation = async () => {
  if (!navigator.geolocation) {
    alert('Geolocation is not supported by this browser.')
    return
  }

  gettingLocation.value = true
  
  try {
    const position = await new Promise<GeolocationPosition>((resolve, reject) => {
      navigator.geolocation.getCurrentPosition(resolve, reject, {
        enableHighAccuracy: true,
        timeout: 10000,
        maximumAge: 300000 // 5 minutes
      })
    })
    
    latitude.value = position.coords.latitude
    longitude.value = position.coords.longitude
    locationInput.value = 'Current Location'
    
    // Auto-search if we have a search term
    if (searchTerm.value) {
      await performSearch()
    }
  } catch (error) {
    console.error('Error getting location:', error)
    alert('Unable to access your location. Please enable location services or enter a location manually.')
  } finally {
    gettingLocation.value = false
  }
}

const clearAll = () => {
  searchTerm.value = ''
  locationInput.value = ''
  latitude.value = undefined
  longitude.value = undefined
  radius.value = 5000
  selectedCategory.value = undefined
  selectedDate.value = ''
  selectedTime.value = ''
  showFilters.value = false
  
  // Emit empty search to clear results
  emit('search', {})
}

// Initialize categories on mount
onMounted(async () => {
  await specialStore.fetchSpecialCategories()
})

// Expose methods for parent component
defineExpose({
  performSearch,
  clearAll
})
</script>
