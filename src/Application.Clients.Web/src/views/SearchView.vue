<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useRoute } from 'vue-router'
import { useSpecialStore } from '../stores/special'
import SpecialCard from '../components/SpecialCard.vue'
import SearchSpecials from '../components/SearchSpecials.vue'
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
    const foodCount = Array.isArray(venue.food) ? venue.food.length : 0
    const drinkCount = Array.isArray(venue.drink) ? venue.drink.length : 0
    const entertainmentCount = Array.isArray(venue.entertainment) ? venue.entertainment.length : 0
    return total + foodCount + drinkCount + entertainmentCount
  }, 0)
})

// Search handler
const handleSearch = async (params: any) => {
  if (!params.location && !params.searchTerm) {
    // Clear results if no search criteria
    return
  }

  isSearching.value = true
  
  try {
    // TODO: Convert location string to lat/lng if provided
    // For now, we'll handle this in the API or use a geocoding service
    
    // Use the enhanced search API that returns venues with categorized specials
    const searchParams = {
      searchTerm: params.searchTerm || undefined,
      latitude: params.latitude, // Will be undefined if location is a string
      longitude: params.longitude, // Will be undefined if location is a string
      radiusInMeters: params.radius || 5000,
      date: params.date || undefined,
      time: params.time || undefined,
      activeOnly: true,
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
  if (query.location || query.searchTerm) {
    handleSearch({
      searchTerm: query.searchTerm as string,
      location: query.location as string,
      radius: query.radius ? parseInt(query.radius as string) : 5000,
      date: query.date as string,
      time: query.time as string,
      categoryId: query.category ? parseInt(query.category as string) : undefined,
      sortBy: query.sortBy as string || 'distance',
      sortOrder: query.sortOrder as string || 'asc'
    })
  }
})

// Watch for route changes
watch(() => route.query, (newQuery) => {
  if (newQuery.location || newQuery.searchTerm) {
    handleSearch({
      searchTerm: newQuery.searchTerm as string,
      location: newQuery.location as string,
      radius: newQuery.radius ? parseInt(newQuery.radius as string) : 5000,
      date: newQuery.date as string,
      time: newQuery.time as string,
      categoryId: newQuery.category ? parseInt(newQuery.category as string) : undefined,
      sortBy: newQuery.sortBy as string || 'distance',
      sortOrder: newQuery.sortOrder as string || 'asc'
    })
  }
})
</script>

<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Search Component -->
    <div class="bg-gradient-to-br from-blue-600 via-purple-600 to-blue-800 py-12">
      <SearchSpecials />
    </div>

    <!-- Content -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
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

      <!-- Welcome State -->
      <div v-else-if="!hasResults && !isSearching" class="text-center py-16">
        <StarIcon class="h-16 w-16 text-gray-300 mx-auto mb-4" />
        <h3 class="text-lg font-medium text-gray-900 mb-2">Find Amazing Specials</h3>
        <p class="text-gray-600 mb-8 max-w-md mx-auto">
          Search for special offers, deals, and promotions at restaurants and venues near you.
        </p>
      </div>

      <!-- Results -->
      <div v-else-if="hasResults" class="space-y-8">
        <!-- Results Header -->
        <div class="flex items-center justify-between">
          <div class="flex items-center">
            <StarIcon class="h-6 w-6 text-red-500 mr-3" />
            <h2 class="text-2xl font-bold text-gray-900">
              {{ totalResults }} Specials at {{ venueResults.length }} Venues
            </h2>
          </div>
        </div>
        
        <!-- Venues with Categorized Specials -->
        <div class="space-y-8">
          <template v-for="venue in venueResults" :key="venue.venue?.id || `venue-${Math.random()}`">
            <div 
              v-if="venue?.venue"
              class="bg-white rounded-lg shadow-lg overflow-hidden"
            >
            <!-- Venue Header -->
            <div class="bg-gradient-to-r from-blue-500 to-purple-600 text-white p-6">
              <div class="flex items-center justify-between">
                <div>
                  <h3 class="text-xl font-bold">{{ venue.venue.name }}</h3>
                  <p class="text-blue-100 mt-1">{{ venue.venue.streetAddress }}, {{ venue.venue.locality }}</p>
                  <p v-if="venue.venue.distanceInMeters" class="text-blue-100 text-sm mt-1">
                    {{ (venue.venue.distanceInMeters / 1000).toFixed(1) }} km away
                  </p>
                </div>
                <div class="text-right">
                  <span class="inline-block bg-white/20 px-3 py-1 rounded-full text-sm font-semibold">
                    {{ (venue.food?.length || 0) + (venue.drink?.length || 0) + (venue.entertainment?.length || 0) }} Specials
                  </span>
                </div>
              </div>
            </div>

            <!-- Categories and Specials -->
            <div class="p-6 space-y-6">
              <!-- Food Specials -->
              <div v-if="venue.food?.length > 0" class="border-l-4 border-green-500 pl-4">
                <div class="flex items-center mb-4">
                  <div class="w-8 h-8 bg-green-100 rounded-lg flex items-center justify-center mr-3">
                    <span class="text-lg">🍽️</span>
                  </div>
                  <h4 class="text-lg font-semibold text-gray-900">Food</h4>
                  <span class="ml-2 bg-gray-100 text-gray-600 px-2 py-1 rounded-full text-xs">
                    {{ venue.food.length }} {{ venue.food.length === 1 ? 'special' : 'specials' }}
                  </span>
                </div>
                <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                  <SpecialCard 
                    v-for="special in venue.food" 
                    :key="special.id"
                    :special="special"
                    class="hover:shadow-lg transform hover:-translate-y-1 transition-all duration-200"
                  />
                </div>
              </div>

              <!-- Drink Specials -->
              <div v-if="venue.drink?.length > 0" class="border-l-4 border-blue-500 pl-4">
                <div class="flex items-center mb-4">
                  <div class="w-8 h-8 bg-blue-100 rounded-lg flex items-center justify-center mr-3">
                    <span class="text-lg">🍹</span>
                  </div>
                  <h4 class="text-lg font-semibold text-gray-900">Drinks</h4>
                  <span class="ml-2 bg-gray-100 text-gray-600 px-2 py-1 rounded-full text-xs">
                    {{ venue.drink.length }} {{ venue.drink.length === 1 ? 'special' : 'specials' }}
                  </span>
                </div>
                <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                  <SpecialCard 
                    v-for="special in venue.drink" 
                    :key="special.id"
                    :special="special"
                    class="hover:shadow-lg transform hover:-translate-y-1 transition-all duration-200"
                  />
                </div>
              </div>

              <!-- Entertainment Specials -->
              <div v-if="venue.entertainment?.length > 0" class="border-l-4 border-purple-500 pl-4">
                <div class="flex items-center mb-4">
                  <div class="w-8 h-8 bg-purple-100 rounded-lg flex items-center justify-center mr-3">
                    <span class="text-lg">🎵</span>
                  </div>
                  <h4 class="text-lg font-semibold text-gray-900">Entertainment</h4>
                  <span class="ml-2 bg-gray-100 text-gray-600 px-2 py-1 rounded-full text-xs">
                    {{ venue.entertainment.length }} {{ venue.entertainment.length === 1 ? 'special' : 'specials' }}
                  </span>
                </div>
                <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                  <SpecialCard 
                    v-for="special in venue.entertainment" 
                    :key="special.id"
                    :special="special"
                    class="hover:shadow-lg transform hover:-translate-y-1 transition-all duration-200"
                  />
                </div>
              </div>
            </div>
            </div>
          </template>
        </div>
      </div>
    </div>
  </div>
</template>
