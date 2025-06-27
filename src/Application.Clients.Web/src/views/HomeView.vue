<script setup lang="ts">
import { onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useVenueStore } from '../stores/venue'
import { useSpecialStore } from '../stores/special'
import VenueCard from '../components/VenueCard.vue'
import SpecialCard from '../components/SpecialCard.vue'
import SearchBar from '../components/SearchBar.vue'
import { 
  BuildingStorefrontIcon, 
  StarIcon, 
  MapPinIcon,
  ClockIcon,
  FireIcon
} from '@heroicons/vue/24/outline'

const router = useRouter()
const venueStore = useVenueStore()
const specialStore = useSpecialStore()

const featuredVenues = computed(() => venueStore.venues.slice(0, 6))
const activeSpecials = computed(() => specialStore.specials.slice(0, 6))

onMounted(async () => {
  await Promise.all([
    venueStore.fetchVenues(),
    specialStore.fetchSpecials()
  ])
})

const handleSearch = (params: { searchTerm: string; categoryId?: number; latitude?: number; longitude?: number; radius?: number; date?: string }) => {
  router.push({ 
    name: 'Search', 
    query: { 
      q: params.searchTerm,
      ...(params.categoryId && { category: params.categoryId.toString() }),
      ...(params.latitude && { lat: params.latitude.toString() }),
      ...(params.longitude && { lng: params.longitude.toString() }),
      ...(params.radius && { radius: params.radius.toString() }),
      ...(params.date && { date: params.date })
    }
  })
}

const stats = [
  { name: 'Active Venues', value: '50+', icon: BuildingStorefrontIcon },
  { name: 'Daily Specials', value: '100+', icon: StarIcon },
  { name: 'Cities Covered', value: '5', icon: MapPinIcon },
  { name: 'Hours Saved', value: '1000+', icon: ClockIcon },
]
</script>

<template>
  <div>
    <!-- Hero Section -->
    <section class="relative bg-gradient-to-r from-blue-600 via-purple-600 to-blue-800 text-white">
      <div class="absolute inset-0 bg-black/20"></div>
      <div class="relative max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-24 lg:py-32">
        <div class="text-center">
          <h1 class="text-4xl md:text-6xl font-bold mb-6">
            Discover Amazing 
            <span class="bg-gradient-to-r from-yellow-400 to-orange-500 bg-clip-text text-transparent">
              Venues & Specials
            </span>
          </h1>
          <p class="text-xl md:text-2xl mb-8 text-blue-100 max-w-3xl mx-auto">
            Find the best restaurants, bars, and entertainment venues with exclusive deals happening right now.
          </p>
          
          <!-- Hero Search Bar -->
          <div class="max-w-2xl mx-auto mb-8">
            <SearchBar 
              @search="handleSearch"
              placeholder="Search venues, specials, or cuisine types..."
              :show-category-filter="true"
              class="shadow-2xl"
            />
          </div>
          
          <div class="flex flex-col sm:flex-row gap-4 justify-center items-center">
            <router-link 
              to="/venues"
              class="px-8 py-4 bg-white text-blue-600 font-semibold rounded-lg hover:bg-gray-100 transition-colors shadow-lg"
            >
              Browse Venues
            </router-link>
            <router-link 
              to="/specials"
              class="px-8 py-4 bg-transparent border-2 border-white text-white font-semibold rounded-lg hover:bg-white hover:text-blue-600 transition-colors"
            >
              View Specials
            </router-link>
          </div>
        </div>
      </div>
    </section>

    <!-- Stats Section -->
    <section class="py-16 bg-white">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="grid grid-cols-2 lg:grid-cols-4 gap-8">
          <div 
            v-for="stat in stats" 
            :key="stat.name"
            class="text-center"
          >
            <component 
              :is="stat.icon" 
              class="h-8 w-8 text-blue-600 mx-auto mb-4"
            />
            <div class="text-3xl font-bold text-gray-900">{{ stat.value }}</div>
            <div class="text-gray-600">{{ stat.name }}</div>
          </div>
        </div>
      </div>
    </section>

    <!-- Featured Venues Section -->
    <section class="py-16 bg-gray-50">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="text-center mb-12">
          <div class="flex items-center justify-center mb-4">
            <BuildingStorefrontIcon class="h-8 w-8 text-blue-600 mr-3" />
            <h2 class="text-3xl font-bold text-gray-900">Featured Venues</h2>
          </div>
          <p class="text-xl text-gray-600 max-w-2xl mx-auto">
            Discover popular venues loved by our community
          </p>
        </div>
        
        <div v-if="venueStore.loading" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          <div v-for="i in 6" :key="i" class="bg-white rounded-lg shadow-md p-6 animate-pulse">
            <div class="bg-gray-300 h-48 rounded-lg mb-4"></div>
            <div class="bg-gray-300 h-4 rounded mb-2"></div>
            <div class="bg-gray-300 h-4 rounded w-2/3"></div>
          </div>
        </div>
        
        <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          <VenueCard 
            v-for="venue in featuredVenues" 
            :key="venue.id"
            :venue="venue"
            class="hover:shadow-lg transform hover:-translate-y-1 transition-all duration-200"
          />
        </div>
        
        <div class="text-center mt-12">
          <router-link 
            to="/venues"
            class="inline-flex items-center px-6 py-3 bg-blue-600 text-white font-medium rounded-lg hover:bg-blue-700 transition-colors"
          >
            View All Venues
            <svg class="ml-2 h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
            </svg>
          </router-link>
        </div>
      </div>
    </section>

    <!-- Active Specials Section -->
    <section class="py-16 bg-white">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="text-center mb-12">
          <div class="flex items-center justify-center mb-4">
            <FireIcon class="h-8 w-8 text-red-500 mr-3" />
            <h2 class="text-3xl font-bold text-gray-900">Hot Specials</h2>
          </div>
          <p class="text-xl text-gray-600 max-w-2xl mx-auto">
            Don't miss out on these amazing deals happening right now
          </p>
        </div>
        
        <div v-if="specialStore.loading" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          <div v-for="i in 6" :key="i" class="bg-white rounded-lg shadow-md p-6 animate-pulse">
            <div class="bg-gray-300 h-32 rounded-lg mb-4"></div>
            <div class="bg-gray-300 h-4 rounded mb-2"></div>
            <div class="bg-gray-300 h-4 rounded w-3/4"></div>
          </div>
        </div>
        
        <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          <SpecialCard 
            v-for="special in activeSpecials" 
            :key="special.id"
            :special="special"
            class="hover:shadow-lg transform hover:-translate-y-1 transition-all duration-200"
          />
        </div>
        
        <div class="text-center mt-12">
          <router-link 
            to="/specials"
            class="inline-flex items-center px-6 py-3 bg-red-500 text-white font-medium rounded-lg hover:bg-red-600 transition-colors"
          >
            View All Specials
            <svg class="ml-2 h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
            </svg>
          </router-link>
        </div>
      </div>
    </section>

    <!-- CTA Section -->
    <section class="py-16 bg-gradient-to-r from-blue-600 to-purple-600 text-white">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 text-center">
        <ClockIcon class="h-16 w-16 text-yellow-300 mx-auto mb-6" />
        <h2 class="text-3xl font-bold mb-4">Never Miss a Great Deal Again</h2>
        <p class="text-xl mb-8 text-blue-100 max-w-2xl mx-auto">
          Join thousands of food lovers who use Pulse to discover the best venues and specials in their city.
        </p>
        <router-link 
          to="/search"
          class="inline-flex items-center px-8 py-4 bg-yellow-400 text-blue-900 font-bold rounded-lg hover:bg-yellow-300 transition-colors shadow-lg"
        >
          Start Exploring
          <StarIcon class="ml-2 h-5 w-5" />
        </router-link>
      </div>
    </section>
  </div>
</template>
