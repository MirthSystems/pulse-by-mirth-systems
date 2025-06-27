<template>
  <div class="backoffice-page">
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
              @click="testApiCall"
              :disabled="apiLoading"
              class="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-purple-600 hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500 disabled:opacity-50"
            >
              <BeakerIcon class="-ml-1 mr-2 h-4 w-4" />
              {{ apiLoading ? 'Testing...' : 'Test API' }}
            </button>
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

    <!-- API Test Results -->
    <div v-if="apiResult" class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
      <div class="bg-green-50 border border-green-200 rounded-md p-4">
        <div class="flex">
          <CheckCircleIcon class="h-5 w-5 text-green-400" />
          <div class="ml-3">
            <h3 class="text-sm font-medium text-green-800">
              API Test Successful!
            </h3>
            <div class="mt-2 text-sm text-green-700">
              <p>Successfully connected to the API with authentication.</p>
              <p class="mt-1"><strong>Response:</strong> {{ apiResult }}</p>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- API Error -->
    <div v-if="apiError" class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
      <div class="bg-red-50 border border-red-200 rounded-md p-4">
        <div class="flex">
          <XCircleIcon class="h-5 w-5 text-red-400" />
          <div class="ml-3">
            <h3 class="text-sm font-medium text-red-800">
              API Test Failed
            </h3>
            <div class="mt-2 text-sm text-red-700">
              <p>{{ apiError }}</p>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Dashboard Content -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
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
              <router-link 
                to="/backoffice/venues"
                class="text-sm text-blue-600 hover:text-blue-500"
              >
                Manage venues
              </router-link>
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
              <router-link 
                to="/backoffice/venues"
                class="text-sm text-blue-600 hover:text-blue-500"
              >
                Manage specials
              </router-link>
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

      <!-- Recent Activity -->
      <div class="mt-8 bg-white shadow rounded-lg">
        <div class="px-4 py-5 sm:p-6">
          <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">
            Recent Activity
          </h3>
          
          <div v-if="recentActivity.length === 0" class="text-center py-6">
            <ClockIcon class="mx-auto h-12 w-12 text-gray-400" />
            <h3 class="mt-2 text-sm font-medium text-gray-900">No recent activity</h3>
            <p class="mt-1 text-sm text-gray-500">
              Activity will appear here as you manage your venues and specials.
            </p>
          </div>
          
          <div v-else class="flow-root">
            <ul class="-mb-8">
              <li 
                v-for="(activity, index) in recentActivity" 
                :key="activity.id"
                class="relative pb-8"
              >
                <div v-if="index !== recentActivity.length - 1" class="absolute top-4 left-4 -ml-px h-full w-0.5 bg-gray-200"></div>
                <div class="relative flex space-x-3">
                  <div>
                    <span 
                      :class="[
                        'h-8 w-8 rounded-full flex items-center justify-center ring-8 ring-white',
                        activity.type === 'venue' ? 'bg-blue-500' : 'bg-green-500'
                      ]"
                    >
                      <component 
                        :is="activity.type === 'venue' ? BuildingStorefrontIcon : StarIcon" 
                        class="h-4 w-4 text-white"
                      />
                    </span>
                  </div>
                  <div class="min-w-0 flex-1 pt-1.5 flex justify-between space-x-4">
                    <div>
                      <p class="text-sm text-gray-500">
                        {{ activity.description }}
                      </p>
                    </div>
                    <div class="text-right text-sm whitespace-nowrap text-gray-500">
                      {{ activity.timestamp }}
                    </div>
                  </div>
                </div>
              </li>
            </ul>
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
  EyeIcon,
  ClockIcon,
  BeakerIcon,
  CheckCircleIcon,
  XCircleIcon
} from '@heroicons/vue/24/outline'
import apiService from '../services/api'
import { useApiAuth } from '../composables/useApiAuth'
import type { VenueSummary, SpecialSummary } from '../types/api'

const router = useRouter()
const isLoading = ref(true)
const apiLoading = ref(false)
const apiResult = ref<string | null>(null)
const apiError = ref<string | null>(null)

// Initialize API authentication
const { updateApiToken } = useApiAuth()

// API test function
const testApiCall = async () => {
  apiLoading.value = true
  apiError.value = null
  apiResult.value = null
  
  try {
    console.log('Testing API call with authentication...')
    
    // First try to update the API token
    try {
      await updateApiToken()
      console.log('API token updated successfully')
    } catch (tokenError) {
      console.warn('Failed to update API token:', tokenError)
      // Continue with the API call anyway - might work if token is already set
    }
    
    const response = await apiService.getVenues()
    console.log('API Response:', response)
    
    if (response.success && response.data) {
      apiResult.value = `Successfully got ${response.data.length} venues from API`
    } else {
      apiResult.value = `API call completed but returned: ${response.message || 'Unknown response'}`
    }
  } catch (error) {
    console.error('API Error:', error)
    apiError.value = error instanceof Error ? error.message : 'Unknown error occurred'
  } finally {
    apiLoading.value = false
  }
}

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

const recentActivity = ref([
  { 
    id: 1, 
    type: 'special', 
    description: 'Created new special "Happy Hour 50% Off" at The Corner Bistro',
    timestamp: '2 hours ago'
  },
  { 
    id: 2, 
    type: 'venue', 
    description: 'Updated venue information for Sunset Lounge',
    timestamp: '1 day ago'
  },
  { 
    id: 3, 
    type: 'special', 
    description: 'Deactivated special "Summer Sale" at Pizza Palace',
    timestamp: '2 days ago'
  },
])

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
