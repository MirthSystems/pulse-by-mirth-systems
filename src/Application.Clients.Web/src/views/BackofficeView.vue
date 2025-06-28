<template>
  <div class="min-h-screen flex flex-col bg-gray-50">
    <!-- Page Header -->
    <div class="bg-white shadow">
      <div class="px-4 sm:px-6 lg:px-8 py-6">
        <!-- Breadcrumb -->
        <nav class="flex mb-4" aria-label="Breadcrumb">
          <ol class="flex items-center space-x-2">
            <li>
              <div class="flex items-center">
                <router-link to="/" class="text-gray-400 hover:text-gray-500">
                  <svg class="flex-shrink-0 h-4 w-4" viewBox="0 0 20 20" fill="currentColor">
                    <path fill-rule="evenodd" d="M9.293 2.293a1 1 0 011.414 0l7 7A1 1 0 0117 10v8a1 1 0 01-1 1h-2a1 1 0 01-1-1v-3a1 1 0 00-1-1H8a1 1 0 00-1 1v3a1 1 0 01-1 1H4a1 1 0 01-1-1v-8a1 1 0 01.293-.707l7-7z" clip-rule="evenodd" />
                  </svg>
                  <span class="sr-only">Home</span>
                </router-link>
              </div>
            </li>
            <li>
              <div class="flex items-center">
                <svg class="flex-shrink-0 h-4 w-4 text-gray-400" viewBox="0 0 20 20" fill="currentColor">
                  <path fill-rule="evenodd" d="M7.293 14.707a1 1 0 010-1.414L10.586 10 7.293 6.707a1 1 0 011.414-1.414l4 4a1 1 0 010 1.414l-4 4a1 1 0 01-1.414 0z" clip-rule="evenodd" />
                </svg>
                <span class="ml-2 text-sm font-medium text-gray-900">Backoffice</span>
              </div>
            </li>
          </ol>
        </nav>
        
        <div class="flex items-center justify-between">
          <div>
            <h1 class="text-2xl font-bold text-gray-900">Backoffice Dashboard</h1>
            <p class="mt-1 text-sm text-gray-500">
              Manage your venues, specials, and business data
            </p>
          </div>
          <div class="flex space-x-3">
            <button
              @click="openVenueDialog"
              class="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
            >
              <BuildingStorefrontIcon class="-ml-1 mr-2 h-4 w-4" />
              Manage Venues
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Dashboard Content -->
    <div class="flex-1 max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8 w-full">
      <!-- Stats Cards -->
      <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
        <div 
          v-for="stat in stats" 
          :key="stat.name"
          class="bg-white overflow-hidden shadow rounded-lg"
        >
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <component 
                  :is="stat.icon" 
                  class="h-6 w-6 text-gray-400"
                />
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 truncate">
                    {{ stat.name }}
                  </dt>
                  <dd class="text-lg font-medium text-gray-900">
                    {{ stat.value }}
                  </dd>
                </dl>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Main Content Area -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-8">
        <!-- Venues Management -->
        <div class="bg-white shadow rounded-lg">
          <div class="px-4 py-5 sm:p-6">
            <div class="flex items-center justify-between mb-4">
              <h3 class="text-lg leading-6 font-medium text-gray-900">
                Recent Venues
              </h3>
            </div>
            
            <div v-if="isLoading" class="space-y-3">
              <div v-for="i in 3" :key="i" class="animate-pulse">
                <div class="flex items-center space-x-3">
                  <div class="h-10 w-10 bg-gray-300 rounded-full"></div>
                  <div class="flex-1 space-y-2">
                    <div class="h-4 bg-gray-300 rounded w-3/4"></div>
                    <div class="h-3 bg-gray-300 rounded w-1/2"></div>
                  </div>
                </div>
              </div>
            </div>
            
            <div v-else-if="recentVenues.length === 0" class="text-center py-6">
              <BuildingStorefrontIcon class="mx-auto h-12 w-12 text-gray-400" />
              <h3 class="mt-2 text-sm font-medium text-gray-900">No venues</h3>
              <p class="mt-1 text-sm text-gray-500">
                Get started by creating a new venue.
              </p>
              <div class="mt-6">
                <button
                  @click="openVenueDialog"
                  class="inline-flex items-center px-4 py-2 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700"
                >
                  <BuildingStorefrontIcon class="-ml-1 mr-2 h-4 w-4" />
                  New Venue
                </button>
              </div>
            </div>
            
            <div v-else class="space-y-3">
              <div 
                v-for="venue in recentVenues" 
                :key="venue.id"
                @click="goToVenueManagement(venue.id)"
                class="flex items-center justify-between p-3 rounded-lg border border-gray-200 hover:bg-gray-50 cursor-pointer transition-colors"
              >
                <div class="flex items-center space-x-3">
                  <div class="flex-shrink-0">
                    <div class="h-10 w-10 bg-blue-100 rounded-full flex items-center justify-center">
                      <BuildingStorefrontIcon class="h-5 w-5 text-blue-600" />
                    </div>
                  </div>
                  <div>
                    <p class="text-sm font-medium text-gray-900">{{ venue.name }}</p>
                    <p class="text-sm text-gray-500">{{ venue.streetAddress || 'No address provided' }}</p>
                  </div>
                </div>
                <div class="flex items-center space-x-2">
                  <span 
                    :class="[
                      'inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium',
                      venue.isActive ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-800'
                    ]"
                  >
                    {{ venue.isActive ? 'Active' : 'Inactive' }}
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Specials Management -->
        <div class="bg-white shadow rounded-lg">
          <div class="px-4 py-5 sm:p-6">
            <div class="flex items-center justify-between mb-4">
              <h3 class="text-lg leading-6 font-medium text-gray-900">
                Active Specials
              </h3>
            </div>
            
            <div v-if="isLoading" class="space-y-3">
              <div v-for="i in 3" :key="i" class="animate-pulse">
                <div class="flex items-center space-x-3">
                  <div class="h-10 w-10 bg-gray-300 rounded-full"></div>
                  <div class="flex-1 space-y-2">
                    <div class="h-4 bg-gray-300 rounded w-3/4"></div>
                    <div class="h-3 bg-gray-300 rounded w-1/2"></div>
                  </div>
                </div>
              </div>
            </div>
            
            <div v-else-if="activeSpecials.length === 0" class="text-center py-6">
              <StarIcon class="mx-auto h-12 w-12 text-gray-400" />
              <h3 class="mt-2 text-sm font-medium text-gray-900">No specials</h3>
              <p class="mt-1 text-sm text-gray-500">
                Get started by creating a new special.
              </p>
              <div class="mt-6">
                <button
                  @click="openSpecialDialog"
                  class="inline-flex items-center px-4 py-2 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-green-600 hover:bg-green-700"
                >
                  <StarIcon class="-ml-1 mr-2 h-4 w-4" />
                  New Special
                </button>
              </div>
            </div>
            
            <div v-else class="space-y-3">
              <div 
                v-for="special in activeSpecials" 
                :key="special.id"
                class="flex items-center justify-between p-3 rounded-lg border border-gray-200 hover:bg-gray-50"
              >
                <div class="flex items-center space-x-3">
                  <div class="flex-shrink-0">
                    <div class="h-10 w-10 bg-green-100 rounded-full flex items-center justify-center">
                      <StarIcon class="h-5 w-5 text-green-600" />
                    </div>
                  </div>
                  <div>
                    <p class="text-sm font-medium text-gray-900">{{ special.title }}</p>
                    <p class="text-sm text-gray-500">{{ special.venueName }}</p>
                  </div>
                </div>
                <div class="flex items-center space-x-2">
                  <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800">
                    Active
                  </span>
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
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { 
  BuildingStorefrontIcon, 
  StarIcon, 
  UserGroupIcon,
  EyeIcon
} from '@heroicons/vue/24/outline'
import apiService from '../services/api'
import type { VenueSummary, SpecialSummary } from '../types/api'

const router = useRouter()
const isLoading = ref(true)

// Navigation function
const goToVenueManagement = (venueId?: number) => {
  router.push('/backoffice/venues')
}

// Data
const stats = ref([
  { name: 'Total Venues', value: '0', icon: BuildingStorefrontIcon },
  { name: 'Active Specials', value: '0', icon: StarIcon },
  { name: 'Total Views', value: '0', icon: EyeIcon },
  { name: 'This Month', value: '0%', icon: UserGroupIcon },
])

const recentVenues = ref<VenueSummary[]>([])
const activeSpecials = ref<SpecialSummary[]>([])

const openVenueDialog = () => {
  router.push('/backoffice/venues')
}

const openSpecialDialog = () => {
  router.push('/backoffice/venues')
}

// Load data
const loadData = async () => {
  try {
    // Load venues
    const venuesResponse = await apiService.getVenues()
    if (venuesResponse.data) {
      recentVenues.value = venuesResponse.data.slice(0, 3)
      stats.value[0].value = venuesResponse.data.length.toString()
    }

    // Load specials
    const specialsResponse = await apiService.getSpecials()
    if (specialsResponse.data) {
      activeSpecials.value = specialsResponse.data.slice(0, 3)
      stats.value[1].value = specialsResponse.data.length.toString()
    }
  } catch (error) {
    console.error('Failed to load dashboard data:', error)
  }
}

onMounted(async () => {
  try {
    await loadData()
  } catch (error) {
    console.error('Error loading data:', error)
  } finally {
    isLoading.value = false
  }
})
</script>
