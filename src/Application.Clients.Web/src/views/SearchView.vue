<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useRoute } from 'vue-router'
import { useSpecialStore } from '../stores/special'
import SpecialCard from '../components/SpecialCard.vue'
import SearchSpecials from '../components/SearchSpecials.vue'
import apiService from '@/services/api'
import { StarIcon } from '@heroicons/vue/24/outline'

const route = useRoute()
const specialStore = useSpecialStore()

const isSearching = ref(false)

// Computed
const hasResults = computed(() => {
  const venues = specialStore.venuesWithSpecials
  return Array.isArray(venues) && venues.length > 0
})

const venueResults = computed(() => {
  const venues = specialStore.venuesWithSpecials
  return Array.isArray(venues) ? venues : []
})

const totalResults = computed(() => {
  const venues = venueResults.value
  if (!Array.isArray(venues) || venues.length === 0) return 0
  
  return venues.reduce((total, venue) => {
    const foodCount = Array.isArray(venue.specials?.food) ? venue.specials.food.length : 0
    const drinkCount = Array.isArray(venue.specials?.drink) ? venue.specials.drink.length : 0
    const entertainmentCount = Array.isArray(venue.specials?.entertainment) ? venue.specials.entertainment.length : 0
    return total + foodCount + drinkCount + entertainmentCount
  }, 0)
})

// Search handler
const handleSearch = async (params: any) => {
  if (!params.address && !params.location && !params.searchTerm && !params.latitude) {
    // Clear results if no search criteria
    return
  }

  isSearching.value = true
  
  try {
    // Use the enhanced search API that returns venues with categorized specials
    const currentlyRunning = params.currentlyRunning === 'true' || params.currentlyRunning === true
    
    // If currently running is true, use current date/time if not specified
    let searchDate = params.date
    let searchTime = params.time
    
    if (currentlyRunning && (!searchDate || !searchTime)) {
      const now = new Date()
      searchDate = searchDate || now.toISOString().split('T')[0] // YYYY-MM-DD
      searchTime = searchTime || now.toTimeString().slice(0, 5) // HH:MM
    }
    
    // Prioritize coordinates - if we don't have them but have an address, geocode first
    let latitude = params.latitude ? parseFloat(params.latitude) : undefined
    let longitude = params.longitude ? parseFloat(params.longitude) : undefined
    
    if ((!latitude || !longitude) && (params.address || params.location)) {
      try {
        const addressToGeocode = params.address || params.location
        const geocodeResponse = await apiService.geocodeAddress(addressToGeocode)
        if (geocodeResponse.success && geocodeResponse.data) {
          latitude = geocodeResponse.data.latitude
          longitude = geocodeResponse.data.longitude
          console.log(`Geocoded "${addressToGeocode}" to coordinates:`, { latitude, longitude })
        } else {
          console.warn('Failed to geocode address:', addressToGeocode)
        }
      } catch (error) {
        console.error('Error geocoding address for search:', error)
      }
    }
    
    const searchParams = {
      searchTerm: params.searchTerm || undefined,
      address: params.address || params.location || undefined, // Handle both new and legacy params
      latitude: latitude,
      longitude: longitude,
      radiusInMeters: params.radius || 5000,
      categoryId: params.categoryId || undefined,
      date: searchDate,
      time: searchTime,
      activeOnly: true,
      currentlyRunning: currentlyRunning,
      pageNumber: 1,
      pageSize: 20,
      sortBy: params.sortBy || 'distance',
      sortOrder: params.sortOrder || 'asc'
    }
    
    await specialStore.searchVenuesWithSpecials(searchParams)
  } finally {
    isSearching.value = false
  }
}

// Initialize with URL params if any
onMounted(() => {
  const query = route.query
  if (query.address || query.location || query.searchTerm || query.latitude) {
    handleSearch({
      searchTerm: query.searchTerm as string,
      address: query.address as string,
      location: query.location as string, // Legacy support
      latitude: query.latitude as string,
      longitude: query.longitude as string,
      radius: query.radius ? parseInt(query.radius as string) : 5000,
      date: query.date as string,
      time: query.time as string,
      categoryId: query.category ? parseInt(query.category as string) : undefined,
      currentlyRunning: query.currentlyRunning as string,
      sortBy: query.sortBy as string || 'distance',
      sortOrder: query.sortOrder as string || 'asc'
    })
  }
})

// Watch for route changes
watch(() => route.query, (newQuery) => {
  if (newQuery.address || newQuery.location || newQuery.searchTerm || newQuery.latitude) {
    handleSearch({
      searchTerm: newQuery.searchTerm as string,
      address: newQuery.address as string,
      location: newQuery.location as string, // Legacy support
      latitude: newQuery.latitude as string,
      longitude: newQuery.longitude as string,
      radius: newQuery.radius ? parseInt(newQuery.radius as string) : 5000,
      date: newQuery.date as string,
      time: newQuery.time as string,
      categoryId: newQuery.category ? parseInt(newQuery.category as string) : undefined,
      currentlyRunning: newQuery.currentlyRunning as string,
      sortBy: newQuery.sortBy as string || 'distance',
      sortOrder: newQuery.sortOrder as string || 'asc'
    })
  }
})
</script>

<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Search Component -->
    <div class="bg-gradient-to-br from-blue-600 via-purple-600 to-blue-800 pt-6 pb-8 sm:pt-8 sm:pb-12">
      <div class="max-w-4xl mx-auto px-4 sm:px-6 text-center mb-6">
        <h1 class="text-3xl sm:text-4xl lg:text-5xl font-bold text-white">
          Search
          <span class="text-transparent bg-clip-text bg-gradient-to-r from-yellow-400 to-orange-400">
            Specials
          </span>
        </h1>
      </div>
      <SearchSpecials />
    </div>

    <!-- Content -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 sm:py-8">
      <!-- Loading State -->
      <div v-if="isSearching" class="space-y-4 sm:space-y-6">
        <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4 sm:gap-6">
          <div v-for="i in 6" :key="i" class="bg-white rounded-xl shadow-md p-4 sm:p-6 animate-pulse">
            <div class="bg-gray-300 h-32 sm:h-40 rounded-lg mb-4"></div>
            <div class="bg-gray-300 h-4 rounded mb-2"></div>
            <div class="bg-gray-300 h-4 rounded w-2/3"></div>
          </div>
        </div>
      </div>

      <!-- Welcome State -->
      <div v-else-if="!hasResults && !isSearching" class="text-center py-12 sm:py-16">
        <StarIcon class="h-12 w-12 sm:h-16 sm:w-16 text-gray-300 mx-auto mb-4" />
        <h3 class="text-lg sm:text-xl font-medium text-gray-900 mb-2">Find Amazing Specials</h3>
        <p class="text-gray-600 mb-8 max-w-md mx-auto text-sm sm:text-base px-4">
          Search for special offers, deals, and promotions at restaurants and venues near you.
        </p>
      </div>

      <!-- Results -->
      <div v-else-if="hasResults" class="space-y-4 sm:space-y-6">
        <!-- Results Header -->
        <div class="flex items-center justify-center sm:justify-start px-2">
          <StarIcon class="h-5 w-5 sm:h-6 sm:w-6 text-red-500 mr-2 sm:mr-3 flex-shrink-0" />
          <h2 class="text-lg sm:text-2xl font-bold text-gray-900 text-center sm:text-left break-words">
            {{ totalResults }} Specials at {{ venueResults.length }} Venues
          </h2>
        </div>
        
        <!-- Venues with Categorized Specials -->
        <div class="space-y-4 sm:space-y-6">
          <template v-for="venue in venueResults" :key="venue.id || `venue-${Math.random()}`">
            <div 
              v-if="venue"
              class="bg-white rounded-xl sm:rounded-2xl shadow-lg overflow-hidden transform transition-all duration-300 hover:shadow-xl hover:scale-[1.02]"
            >
              <!-- Venue Header -->
              <div class="bg-gradient-to-r from-blue-500 to-purple-600 text-white p-3 rounded-t-xl sm:rounded-t-2xl">
                <div class="flex items-center justify-between">
                  <div class="min-w-0 flex-1">
                    <h3 class="text-base font-bold break-words mb-1">{{ venue.name }}</h3>
                    <div class="flex items-center justify-between text-xs text-blue-100">
                      <span class="break-words">{{ venue.streetAddress }}, {{ venue.locality }}</span>
                      <span v-if="venue.distanceInMeters" class="ml-2 flex-shrink-0">
                        {{ (venue.distanceInMeters / 1000).toFixed(1) }} km
                      </span>
                    </div>
                  </div>
                  <span class="ml-3 text-lg flex-shrink-0">{{ venue.categoryIcon }}</span>
                </div>
              </div>

              <!-- Specials Grid -->
              <div class="p-3">
                <div class="grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 gap-3">
                  <!-- Food Specials -->
                  <template v-if="venue.specials?.food?.length > 0">
                    <SpecialCard 
                      v-for="special in venue.specials.food" 
                      :key="`food-${special.id}`"
                      :special="special"
                      class="transform transition-all duration-200 hover:scale-[1.02]"
                    />
                  </template>

                  <!-- Drink Specials -->
                  <template v-if="venue.specials?.drink?.length > 0">
                    <SpecialCard 
                      v-for="special in venue.specials.drink" 
                      :key="`drink-${special.id}`"
                      :special="special"
                      class="transform transition-all duration-200 hover:scale-[1.02]"
                    />
                  </template>

                  <!-- Entertainment Specials -->
                  <template v-if="venue.specials?.entertainment?.length > 0">
                    <SpecialCard 
                      v-for="special in venue.specials.entertainment" 
                      :key="`entertainment-${special.id}`"
                      :special="special"
                      class="transform transition-all duration-200 hover:scale-[1.02]"
                    />
                  </template>
                </div>

                <!-- No Specials Message -->
                <div v-if="!venue.specials?.food?.length && !venue.specials?.drink?.length && !venue.specials?.entertainment?.length" 
                     class="text-center py-4 text-gray-500">
                  <p class="text-sm">No specials available at this time</p>
                </div>
              </div>
            </div>
          </template>
        </div>
      </div>
    </div>
  </div>
</template>
