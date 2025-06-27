<template>
  <div class="search-specials">
    <div class="max-w-3xl mx-auto">
      <form @submit.prevent="handleSearch" class="space-y-4">
        <!-- Location and Radius Row -->
        <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
          <!-- Location Input -->
          <div class="relative">
            <div class="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
              <MapPinIcon class="h-5 w-5 text-gray-400" />
            </div>
            <input
              v-model="locationName"
              type="text"
              placeholder="Enter location..."
              required
              class="block w-full pl-12 pr-4 py-3 border border-gray-300 rounded-lg leading-5 bg-white placeholder-gray-500 text-gray-900 focus:outline-none focus:ring-2 focus:ring-blue-600 focus:border-blue-600 shadow-sm transition-colors"
            />
          </div>

          <!-- Radius Dropdown -->
          <select
            v-model="radius"
            class="block w-full py-3 px-4 border border-gray-300 rounded-lg leading-5 bg-white text-gray-900 focus:outline-none focus:ring-2 focus:ring-blue-600 focus:border-blue-600 shadow-sm transition-colors"
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
            :disabled="isSearching || !locationName.trim()"
            class="inline-flex items-center justify-center px-6 py-3 border border-blue-600 text-base font-semibold rounded-lg text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed transition-all shadow-sm hover:shadow-md"
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
          </div>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useSpecialStore } from '../stores/special'
import { 
  MagnifyingGlassIcon, 
  MapPinIcon, 
  AdjustmentsHorizontalIcon,
  ChevronDownIcon 
} from '@heroicons/vue/24/outline'

const router = useRouter()
const specialStore = useSpecialStore()

// Search parameters
const searchTerm = ref('')
const locationName = ref('')
const radius = ref(5000)
const date = ref('')
const time = ref('')
const categoryId = ref<number | undefined>(undefined)
const sortBy = ref('distance')
const sortOrder = ref('asc')
const showFilters = ref(false)
const isSearching = ref(false)

// Categories from store
const categories = ref<any[]>([])

// Load categories on mount
onMounted(async () => {
  await specialStore.fetchSpecialCategories()
  categories.value = specialStore.categories
})

const handleSearch = async () => {
  // Validate required fields
  if (!locationName.value.trim()) {
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
        location: locationName.value.trim(),
        radius: radius.value.toString(),
        date: date.value || undefined,
        time: time.value || undefined,
        category: categoryId.value?.toString() || undefined,
        sortBy: sortBy.value,
        sortOrder: sortOrder.value
      }
    })
  } finally {
    isSearching.value = false
  }
}
</script>
