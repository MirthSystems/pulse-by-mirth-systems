<script setup lang="ts">
import { onMounted, ref, computed, watch } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useVenueStore } from '../stores/venue'
import VenueCard from '../components/VenueCard.vue'
import SearchBar from '../components/SearchBar.vue'
import { 
  BuildingStorefrontIcon,
  FunnelIcon,
  MapIcon,
  ListBulletIcon,
  AdjustmentsHorizontalIcon
} from '@heroicons/vue/24/outline'

const router = useRouter()
const route = useRoute()
const venueStore = useVenueStore()

const viewMode = ref<'grid' | 'list'>('grid')
const sortBy = ref<'name' | 'distance' | 'rating' | 'specials'>('name')
const sortOrder = ref<'asc' | 'desc'>('asc')
const showFilters = ref(false)
const selectedCategories = ref<number[]>([])

const filteredVenues = computed(() => {
  let venues = [...venueStore.venues]
  
  // Filter by categories
  if (selectedCategories.value.length > 0) {
    venues = venues.filter(venue => 
      selectedCategories.value.some(catId => 
        venueStore.categories.find(cat => cat.id === catId)?.name === venue.categoryName
      )
    )
  }
  
  // Sort venues
  venues.sort((a, b) => {
    let comparison = 0
    
    switch (sortBy.value) {
      case 'name':
        comparison = a.name.localeCompare(b.name)
        break
      case 'distance':
        comparison = (a.distanceInMeters || 0) - (b.distanceInMeters || 0)
        break
      case 'specials':
        comparison = b.activeSpecialsCount - a.activeSpecialsCount
        break
      default:
        comparison = a.name.localeCompare(b.name)
    }
    
    return sortOrder.value === 'desc' ? -comparison : comparison
  })
  
  return venues
})

const handleSearch = (params: { searchTerm: string; categoryId?: number; latitude?: number; longitude?: number; radius?: number; date?: string }) => {
  if (params.latitude && params.longitude) {
    venueStore.fetchNearbyVenues(params.latitude, params.longitude, params.radius || 5000)
  } else if (params.searchTerm || params.categoryId) {
    venueStore.searchVenues(params.searchTerm, params.categoryId)
  }
}

const toggleCategory = (categoryId: number) => {
  const index = selectedCategories.value.indexOf(categoryId)
  if (index > -1) {
    selectedCategories.value.splice(index, 1)
  } else {
    selectedCategories.value.push(categoryId)
  }
}

const clearFilters = () => {
  selectedCategories.value = []
  sortBy.value = 'name'
  sortOrder.value = 'asc'
}

// Load venues and categories on mount
onMounted(async () => {
  await Promise.all([
    venueStore.fetchVenues(),
    venueStore.fetchVenueCategories()
  ])
  
  // Handle query parameters
  const query = route.query
  if (query.category) {
    const categoryId = parseInt(query.category as string)
    if (!isNaN(categoryId)) {
      selectedCategories.value = [categoryId]
    }
  }
})

// Watch for route changes
watch(() => route.query, (newQuery) => {
  if (newQuery.category) {
    const categoryId = parseInt(newQuery.category as string)
    if (!isNaN(categoryId)) {
      selectedCategories.value = [categoryId]
    }
  }
})
</script>

<template>
  <div>
    <!-- Page Header -->
    <div class="bg-white border-b border-gray-200 pb-8">
      <div class="flex items-center justify-between">
        <div>
          <div class="flex items-center mb-4">
            <BuildingStorefrontIcon class="h-8 w-8 text-blue-600 mr-3" />
            <h1 class="text-3xl font-bold text-gray-900">Venues</h1>
          </div>
          <p class="text-lg text-gray-600">
            Discover amazing venues in your area
          </p>
        </div>
        
        <div class="hidden lg:flex items-center space-x-4">
          <div class="flex bg-gray-100 rounded-lg p-1">
            <button
              @click="viewMode = 'grid'"
              :class="[
                'px-3 py-2 rounded-md text-sm font-medium transition-colors',
                viewMode === 'grid' 
                  ? 'bg-white text-gray-900 shadow-sm' 
                  : 'text-gray-600 hover:text-gray-900'
              ]"
            >
              <div class="grid grid-cols-3 gap-1 w-4 h-4">
                <div class="bg-current rounded-sm"></div>
                <div class="bg-current rounded-sm"></div>
                <div class="bg-current rounded-sm"></div>
                <div class="bg-current rounded-sm"></div>
                <div class="bg-current rounded-sm"></div>
                <div class="bg-current rounded-sm"></div>
              </div>
            </button>
            <button
              @click="viewMode = 'list'"
              :class="[
                'px-3 py-2 rounded-md text-sm font-medium transition-colors',
                viewMode === 'list' 
                  ? 'bg-white text-gray-900 shadow-sm' 
                  : 'text-gray-600 hover:text-gray-900'
              ]"
            >
              <ListBulletIcon class="w-4 h-4" />
            </button>
          </div>
        </div>
      </div>
      
      <!-- Search Bar -->
      <div class="mt-8">
        <SearchBar 
          @search="handleSearch"
          :categories="venueStore.categories"
          placeholder="Search venues by name, cuisine, or location..."
          :show-category-filter="true"
          :show-location-filter="true"
        />
      </div>
    </div>

    <!-- Filters and Controls -->
    <div class="bg-white border-b border-gray-200 py-4">
      <div class="flex flex-col lg:flex-row lg:items-center lg:justify-between gap-4">
        <div class="flex items-center space-x-4">
          <button
            @click="showFilters = !showFilters"
            class="inline-flex items-center px-4 py-2 border border-gray-300 rounded-lg text-sm font-medium text-gray-700 bg-white hover:bg-gray-50"
          >
            <FunnelIcon class="h-4 w-4 mr-2" />
            Filters
            <span v-if="selectedCategories.length > 0" class="ml-2 bg-blue-100 text-blue-800 text-xs px-2 py-1 rounded-full">
              {{ selectedCategories.length }}
            </span>
          </button>
          
          <div class="text-sm text-gray-600">
            {{ filteredVenues.length }} venues found
          </div>
        </div>
        
        <div class="flex items-center space-x-4">
          <div class="flex items-center space-x-2">
            <label class="text-sm font-medium text-gray-700">Sort by:</label>
            <select 
              v-model="sortBy"
              class="border border-gray-300 rounded-md px-3 py-1 text-sm focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            >
              <option value="name">Name</option>
              <option value="distance">Distance</option>
              <option value="specials">Special Count</option>
            </select>
          </div>
          
          <button
            @click="sortOrder = sortOrder === 'asc' ? 'desc' : 'asc'"
            class="px-3 py-1 border border-gray-300 rounded-md text-sm hover:bg-gray-50"
          >
            {{ sortOrder === 'asc' ? '↑' : '↓' }}
          </button>
        </div>
      </div>
      
      <!-- Expandable Filters -->
      <div v-if="showFilters" class="mt-4 pt-4 border-t border-gray-200">
        <div class="space-y-4">
          <div>
            <h3 class="text-sm font-medium text-gray-900 mb-3">Categories</h3>
            <div class="flex flex-wrap gap-2">
              <button
                v-for="category in venueStore.categories"
                :key="category.id"
                @click="toggleCategory(category.id)"
                :class="[
                  'px-3 py-2 rounded-full text-sm font-medium border transition-colors',
                  selectedCategories.includes(category.id)
                    ? 'bg-blue-50 border-blue-200 text-blue-700'
                    : 'bg-white border-gray-300 text-gray-700 hover:bg-gray-50'
                ]"
              >
                {{ category.icon }} {{ category.name }}
              </button>
            </div>
          </div>
          
          <div class="flex justify-end">
            <button
              @click="clearFilters"
              class="text-sm text-gray-600 hover:text-gray-900"
            >
              Clear all filters
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="venueStore.loading" class="py-8">
      <div :class="[
        'grid gap-6',
        viewMode === 'grid' 
          ? 'grid-cols-1 md:grid-cols-2 lg:grid-cols-3' 
          : 'grid-cols-1'
      ]">
        <div v-for="i in 9" :key="i" class="bg-white rounded-lg shadow-md p-6 animate-pulse">
          <div class="bg-gray-300 h-48 rounded-lg mb-4"></div>
          <div class="bg-gray-300 h-4 rounded mb-2"></div>
          <div class="bg-gray-300 h-4 rounded w-2/3"></div>
        </div>
      </div>
    </div>

    <!-- Empty State -->
    <div v-else-if="filteredVenues.length === 0" class="text-center py-16">
      <BuildingStorefrontIcon class="h-16 w-16 text-gray-300 mx-auto mb-4" />
      <h3 class="text-lg font-medium text-gray-900 mb-2">No venues found</h3>
      <p class="text-gray-600 mb-6">Try adjusting your search criteria or filters.</p>
      <button
        @click="clearFilters"
        class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
      >
        Clear Filters
      </button>
    </div>

    <!-- Venues Grid/List -->
    <div v-else class="py-8">
      <div :class="[
        'grid gap-6',
        viewMode === 'grid' 
          ? 'grid-cols-1 md:grid-cols-2 lg:grid-cols-3' 
          : 'grid-cols-1'
      ]">
        <VenueCard 
          v-for="venue in filteredVenues" 
          :key="venue.id"
          :venue="venue"
          :class="[
            'hover:shadow-lg transform hover:-translate-y-1 transition-all duration-200',
            viewMode === 'list' ? 'md:flex md:max-w-none' : ''
          ]"
        />
      </div>
    </div>

    <!-- Error State -->
    <div v-if="venueStore.error" class="bg-red-50 border border-red-200 rounded-lg p-4 mt-4">
      <div class="flex">
        <div class="ml-3">
          <h3 class="text-sm font-medium text-red-800">Error loading venues</h3>
          <p class="mt-1 text-sm text-red-700">{{ venueStore.error }}</p>
          <button 
            @click="venueStore.fetchVenues()"
            class="mt-3 text-sm bg-red-100 text-red-800 px-3 py-1 rounded hover:bg-red-200"
          >
            Try again
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
