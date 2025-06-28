<template>
  <div class="search-specials">
    <div class="max-w-3xl mx-auto">
      <form @submit.prevent="handleSearch" class="space-y-4">
        <!-- Location and Radius Row -->
        <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
          <!-- Address Autocomplete Input -->
          <AddressAutocomplete
            v-model="locationAddress"
            placeholder="Enter location..."
            required
            @location-selected="handleLocationSelected"
            @coordinates-updated="handleCoordinatesUpdated"
            class="input-control"
          />

          <!-- Radius Dropdown -->
          <select
            v-model="radius"
            class="input-control block w-full py-3 px-4 border border-gray-300 rounded-lg bg-white text-gray-900 focus:outline-none focus:ring-2 focus:ring-blue-600 focus:border-blue-600 shadow-sm transition-colors"
          >
            <option :value="1000">1 km</option>
            <option :value="2000">2 km</option>
            <option :value="5000">5 km</option>
            <option :value="10000">10 km</option>
            <option :value="25000">25 km</option>
          </select>

          <!-- Search Button -->
          <button
            type="submit"
            :disabled="isSearching || !locationAddress.trim()"
            class="btn-primary inline-flex items-center justify-center px-6 py-3 border border-blue-600 text-base font-semibold rounded-lg text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed transition-all shadow-sm hover:shadow-md"
          >
            <MagnifyingGlassIcon v-if="!isSearching" class="mr-2 h-5 w-5" />
            <div v-else class="animate-spin rounded-full h-5 w-5 border-b-2 border-white mr-2"></div>
            {{ isSearching ? 'Searching...' : 'Search' }}
          </button>
        </div>

        <!-- Optional Filter Expansion -->
        <div class="text-center">
          <button
            type="button"
            @click="showFilters = !showFilters"
            class="inline-flex items-center px-4 py-2 text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 rounded-lg transition-all shadow-sm border border-gray-300"
          >
            <AdjustmentsHorizontalIcon class="mr-2 h-4 w-4" />
            {{ showFilters ? 'Hide' : 'Show' }} Filters
            <ChevronDownIcon 
              class="ml-2 h-4 w-4 transition-transform"
              :class="{ 'rotate-180': showFilters }"
            />
          </button>
        </div>

        <!-- Expanded Filters -->
        <div v-if="showFilters" class="bg-gray-50 rounded-lg p-6 space-y-4 shadow-sm border border-gray-300">
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            <!-- Search Term Filter -->
            <div>
              <label class="block text-sm font-semibold text-gray-900 mb-2">Search Term</label>
              <input
                v-model="searchTerm"
                type="text"
                placeholder="Keywords, venue, cuisine..."
                class="block w-full py-3 px-4 border border-gray-300 rounded-lg bg-white text-gray-900 placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-blue-600 focus:border-blue-600 transition-colors"
              />
              <p class="mt-1 text-xs text-gray-700">Optional - leave blank to see all</p>
            </div>

            <!-- Date Filter -->
            <div>
              <label class="block text-sm font-semibold text-gray-900 mb-2">Date</label>
              <input
                v-model="date"
                type="date"
                class="block w-full py-3 px-4 border border-gray-300 rounded-lg bg-white text-gray-900 focus:outline-none focus:ring-2 focus:ring-blue-600 focus:border-blue-600 transition-colors"
              />
              <p class="mt-1 text-xs text-gray-700">Optional - defaults to today</p>
            </div>

            <!-- Time Filter -->
            <div>
              <label class="block text-sm font-semibold text-gray-900 mb-2">Time</label>
              <input
                v-model="time"
                type="time"
                class="block w-full py-3 px-4 border border-gray-300 rounded-lg bg-white text-gray-900 focus:outline-none focus:ring-2 focus:ring-blue-600 focus:border-blue-600 transition-colors"
              />
              <p class="mt-1 text-xs text-gray-700">Optional - defaults to current time</p>
            </div>

            <!-- Category Filter -->
            <div>
              <label class="block text-sm font-semibold text-gray-900 mb-2">Category</label>
              <select
                v-model="categoryId"
                class="block w-full py-3 px-4 border border-gray-300 rounded-lg bg-white text-gray-900 focus:outline-none focus:ring-2 focus:ring-blue-600 focus:border-blue-600 transition-colors"
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

            <!-- Sort By -->
            <div>
              <label class="block text-sm font-semibold text-gray-900 mb-2">Sort By</label>
              <select
                v-model="sortBy"
                class="block w-full py-3 px-4 border border-gray-300 rounded-lg bg-white text-gray-900 focus:outline-none focus:ring-2 focus:ring-blue-600 focus:border-blue-600 transition-colors"
              >
                <option value="distance">Distance</option>
                <option value="name">Name</option>
                <option value="special_count">Special Count</option>
              </select>
            </div>

            <!-- Sort Order -->
            <div>
              <label class="block text-sm font-semibold text-gray-900 mb-2">Sort Order</label>
              <select
                v-model="sortOrder"
                class="block w-full py-3 px-4 border border-gray-300 rounded-lg bg-white text-gray-900 focus:outline-none focus:ring-2 focus:ring-blue-600 focus:border-blue-600 transition-colors"
              >
                <option value="asc">Ascending</option>
                <option value="desc">Descending</option>
              </select>
            </div>

            <!-- Currently Running Toggle -->
            <div>
              <label class="block text-sm font-semibold text-gray-900 mb-2">Special Timing</label>
              <div class="flex items-center space-x-3">
                <label class="relative inline-flex items-center cursor-pointer">
                  <input
                    type="checkbox"
                    v-model="currentlyRunning"
                    class="sr-only peer"
                  />
                  <div class="w-11 h-6 bg-gray-200 peer-focus:outline-none peer-focus:ring-4 peer-focus:ring-blue-300 rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-blue-600"></div>
                  <span class="ml-3 text-sm font-medium text-gray-900">
                    {{ currentlyRunning ? 'Currently Running' : 'All Active' }}
                  </span>
                </label>
              </div>
              <p class="mt-1 text-xs text-gray-700">
                {{ currentlyRunning ? 'Show only specials running right now' : 'Show all active specials' }}
              </p>
            </div>
          </div>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useSpecialStore } from '../stores/special'
import AddressAutocomplete from './AddressAutocomplete.vue'
import type { GeocodeResult } from '@/types/api'
import { 
  MagnifyingGlassIcon, 
  AdjustmentsHorizontalIcon,
  ChevronDownIcon 
} from '@heroicons/vue/24/outline'

const router = useRouter()
const route = useRoute()
const specialStore = useSpecialStore()

// Search parameters
const searchTerm = ref('')
const locationAddress = ref('')
const selectedLocation = ref<GeocodeResult | null>(null)
const currentLatitude = ref<number | undefined>(undefined)
const currentLongitude = ref<number | undefined>(undefined)
const radius = ref(5000)
const date = ref('')
const time = ref('')
const categoryId = ref<number | undefined>(undefined)
const currentlyRunning = ref(true)
const sortBy = ref('distance')
const sortOrder = ref('asc')
const showFilters = ref(false)
const isSearching = ref(false)

// Categories from store
const categories = ref<any[]>([])

// Location handling
const handleLocationSelected = (location: GeocodeResult) => {
  selectedLocation.value = location
  currentLatitude.value = location.latitude
  currentLongitude.value = location.longitude
}

const handleCoordinatesUpdated = (latitude: number, longitude: number) => {
  currentLatitude.value = latitude
  currentLongitude.value = longitude
}

// Load categories on mount and initialize from URL params
onMounted(async () => {
  await specialStore.fetchSpecialCategories()
  categories.value = specialStore.categories
  
  // Initialize form from URL parameters
  const query = route.query
  if (query.searchTerm) searchTerm.value = query.searchTerm as string
  if (query.address) locationAddress.value = query.address as string
  if (query.latitude) currentLatitude.value = parseFloat(query.latitude as string)
  if (query.longitude) currentLongitude.value = parseFloat(query.longitude as string)
  if (query.radius) radius.value = parseInt(query.radius as string)
  if (query.date) date.value = query.date as string
  if (query.time) time.value = query.time as string
  if (query.category) categoryId.value = parseInt(query.category as string)
  if (query.currentlyRunning) currentlyRunning.value = query.currentlyRunning === 'true'
  if (query.sortBy) sortBy.value = query.sortBy as string
  if (query.sortOrder) sortOrder.value = query.sortOrder as string
})

const handleSearch = async () => {
  // Validate required fields
  if (!locationAddress.value.trim()) {
    // Could add a toast notification here
    return
  }

  isSearching.value = true
  
  try {
    // Navigate to search page with parameters that match the enhanced API
    router.push({
      name: 'Search',
      query: {
        searchTerm: searchTerm.value.trim() || undefined,
        address: locationAddress.value.trim(),
        latitude: currentLatitude.value?.toString(),
        longitude: currentLongitude.value?.toString(),
        radius: radius.value.toString(),
        date: date.value || undefined,
        time: time.value || undefined,
        category: categoryId.value?.toString() || undefined,
        currentlyRunning: currentlyRunning.value.toString(),
        sortBy: sortBy.value,
        sortOrder: sortOrder.value
      }
    })
  } finally {
    isSearching.value = false
  }
}
</script>

<style scoped>
.input-control {
  height: 3.25rem; /* 52px - consistent height for all form controls */
  min-height: 3.25rem;
}

.btn-primary {
  height: 3.25rem; /* 52px - same height as input controls */
  min-height: 3.25rem;
}

/* Improve contrast for better accessibility */
.btn-primary:hover:not(:disabled) {
  background-color: #1d4ed8; /* blue-700 */
  border-color: #1d4ed8;
}

.btn-primary:focus:not(:disabled) {
  box-shadow: 0 0 0 2px rgba(59, 130, 246, 0.5);
}

/* Better disabled state contrast */
.btn-primary:disabled {
  background-color: #9ca3af; /* gray-400 */
  border-color: #9ca3af;
  color: #ffffff;
  opacity: 0.7;
}

/* Improve filter toggle button accessibility */
.search-specials button[type="button"]:not(.btn-primary) {
  border: 1px solid #d1d5db; /* gray-300 */
  background-color: #ffffff;
  color: #374151; /* gray-700 */
}

.search-specials button[type="button"]:not(.btn-primary):hover {
  background-color: #f9fafb; /* gray-50 */
  border-color: #9ca3af; /* gray-400 */
}

.search-specials button[type="button"]:not(.btn-primary):focus {
  outline: none;
  box-shadow: 0 0 0 2px rgba(59, 130, 246, 0.5); /* blue-500 with opacity */
  border-color: #2563eb; /* blue-600 */
}
</style>