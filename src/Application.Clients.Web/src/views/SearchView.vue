<script setup lang="ts">
import { onMounted, ref, computed, watch } from 'vue'
import { useRoute } from 'vue-router'
import { useVenueStore } from '../stores/venue'
import { useSpecialStore } from '../stores/special'
import VenueCard from '../components/VenueCard.vue'
import SpecialCard from '../components/SpecialCard.vue'
import SearchBar from '../components/SearchBar.vue'
import { 
  MagnifyingGlassIcon,
  BuildingStorefrontIcon,
  StarIcon,
  MapIcon,
  ClockIcon,
  AdjustmentsHorizontalIcon,
  FunnelIcon
} from '@heroicons/vue/24/outline'

const route = useRoute()
const venueStore = useVenueStore()
const specialStore = useSpecialStore()

const searchResults = ref<'all' | 'venues' | 'specials'>('all')
const sortBy = ref<'relevance' | 'distance' | 'name' | 'rating'>('relevance')
const showAdvancedSearch = ref(false)
const isSearching = ref(false)

// Get search params from URL
const searchTerm = ref(route.query.q as string || '')
const latitude = ref(route.query.lat ? parseFloat(route.query.lat as string) : undefined)
const longitude = ref(route.query.lng ? parseFloat(route.query.lng as string) : undefined)
const radius = ref(route.query.radius ? parseInt(route.query.radius as string) : 5000)
const selectedCategory = ref(route.query.category ? parseInt(route.query.category as string) : undefined)
const selectedDate = ref(route.query.date as string || '')

const hasResults = computed(() => 
  venueStore.searchResults.length > 0 || specialStore.searchResults.length > 0
)

const venueResults = computed(() => venueStore.searchResults)
const specialResults = computed(() => specialStore.searchResults)

const filteredResults = computed(() => {
  switch (searchResults.value) {
    case 'venues':
      return { venues: venueResults.value, specials: [] }
    case 'specials':
      return { venues: [], specials: specialResults.value }
    default:
      return { venues: venueResults.value, specials: specialResults.value }
  }
})

const totalResults = computed(() => 
  filteredResults.value.venues.length + filteredResults.value.specials.length
)

const handleSearch = async (params: { 
  searchTerm: string; 
  categoryId?: number; 
  latitude?: number; 
  longitude?: number; 
  radius?: number; 
  date?: string 
}) => {
  isSearching.value = true
  
  try {
    // Update search parameters
    searchTerm.value = params.searchTerm
    latitude.value = params.latitude
    longitude.value = params.longitude
    radius.value = params.radius || 5000
    selectedCategory.value = params.categoryId
    selectedDate.value = params.date || ''
    
    // Perform searches
    const searchPromises = []
    
    if (params.searchTerm) {
      searchPromises.push(venueStore.searchVenues(params.searchTerm, params.categoryId))
      searchPromises.push(specialStore.searchSpecials(params.searchTerm))
    }
    
    if (params.latitude && params.longitude) {
      searchPromises.push(venueStore.fetchNearbyVenues(params.latitude, params.longitude, params.radius || 5000))
    }
    
    await Promise.all(searchPromises)
  } finally {
    isSearching.value = false
  }
}

const clearSearch = () => {
  searchTerm.value = ''
  latitude.value = undefined
  longitude.value = undefined
  selectedCategory.value = undefined
  selectedDate.value = ''
  venueStore.searchResults = []
  specialStore.searchResults = []
}

const getLocationString = () => {
  if (latitude.value && longitude.value) {
    return `within ${radius.value / 1000}km of your location`
  }
  return ''
}

// Perform initial search if query params exist
onMounted(async () => {
  await Promise.all([
    venueStore.fetchVenueCategories(),
    specialStore.fetchSpecialCategories()
  ])
  
  if (searchTerm.value || latitude.value) {
    await handleSearch({
      searchTerm: searchTerm.value,
      categoryId: selectedCategory.value,
      latitude: latitude.value,
      longitude: longitude.value,
      radius: radius.value,
      date: selectedDate.value
    })
  }
})

// Watch for route query changes
watch(() => route.query, async (newQuery) => {
  const newSearchTerm = newQuery.q as string || ''
  const newLat = newQuery.lat ? parseFloat(newQuery.lat as string) : undefined
  const newLng = newQuery.lng ? parseFloat(newQuery.lng as string) : undefined
  
  if (newSearchTerm !== searchTerm.value || newLat !== latitude.value || newLng !== longitude.value) {
    await handleSearch({
      searchTerm: newSearchTerm,
      categoryId: newQuery.category ? parseInt(newQuery.category as string) : undefined,
      latitude: newLat,
      longitude: newLng,
      radius: newQuery.radius ? parseInt(newQuery.radius as string) : 5000,
      date: newQuery.date as string || ''
    })
  }
})
</script>

<template>
  <div>
    <!-- Search Header -->
    <div class="bg-gradient-to-r from-blue-600 via-purple-600 to-blue-800 text-white">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
        <div class="text-center mb-8">
          <div class="flex items-center justify-center mb-4">
            <MagnifyingGlassIcon class="h-8 w-8 text-yellow-300 mr-3" />
            <h1 class="text-4xl font-bold">Search</h1>
          </div>
          <p class="text-xl text-blue-100">
            Find venues, specials, and deals near you
          </p>
        </div>
        
        <!-- Main Search Bar -->
        <div class="max-w-4xl mx-auto">
          <SearchBar 
            @search="handleSearch"
            :model-value="searchTerm"
            :categories="[...venueStore.categories, ...specialStore.categories]"
            placeholder="Search for venues, specials, cuisine types, or locations..."
            :show-category-filter="true"
            :show-location-filter="true"
            :show-date-filter="true"
            :loading="isSearching"
            class="shadow-2xl"
          />
        </div>
        
        <!-- Quick Actions -->
        <div class="flex justify-center mt-6 space-x-4">
          <button
            @click="showAdvancedSearch = !showAdvancedSearch"
            class="inline-flex items-center px-4 py-2 bg-white/10 text-white rounded-lg hover:bg-white/20 transition-colors"
          >
            <AdjustmentsHorizontalIcon class="h-4 w-4 mr-2" />
            Advanced Search
          </button>
          
          <button
            v-if="searchTerm || latitude"
            @click="clearSearch"
            class="inline-flex items-center px-4 py-2 bg-white/10 text-white rounded-lg hover:bg-white/20 transition-colors"
          >
            Clear Search
          </button>
        </div>
      </div>
    </div>

    <!-- Advanced Search (Expandable) -->
    <div v-if="showAdvancedSearch" class="bg-white border-b border-gray-200 py-6">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <h3 class="text-lg font-medium text-gray-900 mb-4">Advanced Search Options</h3>
        <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Search Radius</label>
            <select 
              v-model="radius"
              class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            >
              <option :value="1000">1 km</option>
              <option :value="2000">2 km</option>
              <option :value="5000">5 km</option>
              <option :value="10000">10 km</option>
              <option :value="25000">25 km</option>
            </select>
          </div>
          
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Sort By</label>
            <select 
              v-model="sortBy"
              class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            >
              <option value="relevance">Relevance</option>
              <option value="distance">Distance</option>
              <option value="name">Name</option>
              <option value="rating">Rating</option>
            </select>
          </div>
          
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Show Results</label>
            <select 
              v-model="searchResults"
              class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            >
              <option value="all">All Results</option>
              <option value="venues">Venues Only</option>
              <option value="specials">Specials Only</option>
            </select>
          </div>
        </div>
      </div>
    </div>

    <!-- Search Results -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Results Header -->
      <div v-if="searchTerm || latitude" class="mb-8">
        <div class="flex items-center justify-between mb-4">
          <div>
            <h2 class="text-2xl font-bold text-gray-900">
              Search Results
              <span v-if="!isSearching" class="text-lg font-normal text-gray-600">
                ({{ totalResults }} results)
              </span>
            </h2>
            <p class="text-gray-600 mt-1">
              <span v-if="searchTerm">Results for "{{ searchTerm }}"</span>
              <span v-if="searchTerm && getLocationString()"> </span>
              <span v-if="getLocationString()">{{ getLocationString() }}</span>
            </p>
          </div>
          
          <div class="flex items-center space-x-4">
            <div class="flex bg-gray-100 rounded-lg p-1">
              <button
                @click="searchResults = 'all'"
                :class="[
                  'px-3 py-2 rounded-md text-sm font-medium transition-colors',
                  searchResults === 'all' 
                    ? 'bg-white text-gray-900 shadow-sm' 
                    : 'text-gray-600 hover:text-gray-900'
                ]"
              >
                All ({{ totalResults }})
              </button>
              <button
                @click="searchResults = 'venues'"
                :class="[
                  'px-3 py-2 rounded-md text-sm font-medium transition-colors',
                  searchResults === 'venues' 
                    ? 'bg-white text-gray-900 shadow-sm' 
                    : 'text-gray-600 hover:text-gray-900'
                ]"
              >
                Venues ({{ venueResults.length }})
              </button>
              <button
                @click="searchResults = 'specials'"
                :class="[
                  'px-3 py-2 rounded-md text-sm font-medium transition-colors',
                  searchResults === 'specials' 
                    ? 'bg-white text-gray-900 shadow-sm' 
                    : 'text-gray-600 hover:text-gray-900'
                ]"
              >
                Specials ({{ specialResults.length }})
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Loading State -->
      <div v-if="isSearching" class="space-y-8">
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          <div v-for="i in 6" :key="i" class="bg-white rounded-lg shadow-md p-6 animate-pulse">
            <div class="bg-gray-300 h-40 rounded-lg mb-4"></div>
            <div class="bg-gray-300 h-4 rounded mb-2"></div>
            <div class="bg-gray-300 h-4 rounded w-2/3"></div>
          </div>
        </div>
      </div>

      <!-- No Search Performed -->
      <div v-else-if="!searchTerm && !latitude" class="text-center py-16">
        <MagnifyingGlassIcon class="h-16 w-16 text-gray-300 mx-auto mb-4" />
        <h3 class="text-lg font-medium text-gray-900 mb-2">Start Your Search</h3>
        <p class="text-gray-600 mb-8 max-w-md mx-auto">
          Enter a search term, select a location, or use the filters above to find venues and specials.
        </p>
        
        <!-- Quick Search Suggestions -->
        <div class="max-w-2xl mx-auto">
          <h4 class="text-sm font-medium text-gray-700 mb-4">Popular Searches</h4>
          <div class="flex flex-wrap justify-center gap-2">
            <button
              v-for="suggestion in ['Pizza', 'Happy Hour', 'Rooftop', 'Live Music', 'Brunch', 'Cocktails']"
              :key="suggestion"
              @click="handleSearch({ searchTerm: suggestion })"
              class="px-4 py-2 bg-gray-100 text-gray-700 rounded-full hover:bg-gray-200 transition-colors"
            >
              {{ suggestion }}
            </button>
          </div>
        </div>
      </div>

      <!-- No Results -->
      <div v-else-if="!hasResults" class="text-center py-16">
        <MagnifyingGlassIcon class="h-16 w-16 text-gray-300 mx-auto mb-4" />
        <h3 class="text-lg font-medium text-gray-900 mb-2">No results found</h3>
        <p class="text-gray-600 mb-6">
          Try adjusting your search terms or expanding your search radius.
        </p>
        <button
          @click="clearSearch"
          class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
        >
          Clear Search
        </button>
      </div>

      <!-- Results -->
      <div v-else class="space-y-12">
        <!-- Venue Results -->
        <div v-if="filteredResults.venues.length > 0">
          <div class="flex items-center mb-6">
            <BuildingStorefrontIcon class="h-6 w-6 text-blue-600 mr-3" />
            <h3 class="text-xl font-bold text-gray-900">
              Venues ({{ filteredResults.venues.length }})
            </h3>
          </div>
          
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            <VenueCard 
              v-for="venue in filteredResults.venues" 
              :key="venue.id"
              :venue="venue"
              class="hover:shadow-lg transform hover:-translate-y-1 transition-all duration-200"
            />
          </div>
        </div>

        <!-- Special Results -->
        <div v-if="filteredResults.specials.length > 0">
          <div class="flex items-center mb-6">
            <StarIcon class="h-6 w-6 text-red-500 mr-3" />
            <h3 class="text-xl font-bold text-gray-900">
              Specials ({{ filteredResults.specials.length }})
            </h3>
          </div>
          
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            <SpecialCard 
              v-for="special in filteredResults.specials" 
              :key="special.id"
              :special="special"
              class="hover:shadow-lg transform hover:-translate-y-1 transition-all duration-200"
            />
          </div>
        </div>
      </div>

      <!-- Search Tips -->
      <div v-if="hasResults" class="mt-16 bg-gray-50 rounded-lg p-6">
        <h4 class="text-lg font-medium text-gray-900 mb-4">Search Tips</h4>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6 text-sm text-gray-600">
          <div>
            <h5 class="font-medium text-gray-900 mb-2">For better results:</h5>
            <ul class="space-y-1">
              <li>• Try different keywords or synonyms</li>
              <li>• Use location names or neighborhoods</li>
              <li>• Filter by category or cuisine type</li>
            </ul>
          </div>
          <div>
            <h5 class="font-medium text-gray-900 mb-2">Location search:</h5>
            <ul class="space-y-1">
              <li>• Enable location access for nearby results</li>
              <li>• Adjust the search radius</li>
              <li>• Enter specific addresses or landmarks</li>
            </ul>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
