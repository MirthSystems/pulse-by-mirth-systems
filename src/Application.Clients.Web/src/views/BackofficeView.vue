<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Header -->
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
                <span class="ml-2 text-sm font-medium text-gray-900">Venue Management</span>
              </div>
            </li>
          </ol>
        </nav>
        
        <div class="flex flex-col space-y-4 sm:flex-row sm:items-center sm:justify-between sm:space-y-0">
          <div>
            <h1 class="text-xl sm:text-2xl font-bold text-gray-900">Venue Management</h1>
            <p class="mt-1 text-sm text-gray-500">
              Manage your venues, edit details, and track specials
            </p>
          </div>
          <router-link
            v-if="canCreateVenues"
            to="/backoffice/venues/new"
            class="inline-flex items-center justify-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 w-full sm:w-auto"
          >
            <BuildingStorefrontIcon class="-ml-1 mr-2 h-4 w-4" />
            New Venue
          </router-link>
        </div>
      </div>
    </div>

    <!-- Content -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Stats Cards -->
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 sm:gap-6 mb-8">
        <div class="bg-white overflow-hidden shadow rounded-lg">
          <div class="p-4 sm:p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <BuildingStorefrontIcon class="h-5 w-5 sm:h-6 sm:w-6 text-gray-400" />
              </div>
              <div class="ml-3 sm:ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-xs sm:text-sm font-medium text-gray-500 break-words">Total Venues</dt>
                  <dd class="text-base sm:text-lg font-medium text-gray-900">{{ venues.length }}</dd>
                </dl>
              </div>
            </div>
          </div>
        </div>
        
        <div class="bg-white overflow-hidden shadow rounded-lg">
          <div class="p-4 sm:p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <CheckCircleIcon class="h-5 w-5 sm:h-6 sm:w-6 text-green-400" />
              </div>
              <div class="ml-3 sm:ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-xs sm:text-sm font-medium text-gray-500 break-words">Active Venues</dt>
                  <dd class="text-base sm:text-lg font-medium text-gray-900">{{ activeVenues.length }}</dd>
                </dl>
              </div>
            </div>
          </div>
        </div>
        
        <div class="bg-white overflow-hidden shadow rounded-lg">
          <div class="p-4 sm:p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <StarIcon class="h-5 w-5 sm:h-6 sm:w-6 text-yellow-400" />
              </div>
              <div class="ml-3 sm:ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-xs sm:text-sm font-medium text-gray-500 break-words">Total Specials</dt>
                  <dd class="text-base sm:text-lg font-medium text-gray-900">{{ totalSpecials }}</dd>
                </dl>
              </div>
            </div>
          </div>
        </div>
        
        <div class="bg-white overflow-hidden shadow rounded-lg">
          <div class="p-4 sm:p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <FireIcon class="h-5 w-5 sm:h-6 sm:w-6 text-red-400" />
              </div>
              <div class="ml-3 sm:ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-xs sm:text-sm font-medium text-gray-500 break-words">Active Specials</dt>
                  <dd class="text-base sm:text-lg font-medium text-gray-900">{{ activeSpecials }}</dd>
                </dl>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Search and Filters -->
      <div class="bg-white shadow rounded-lg mb-6">
        <div class="px-4 sm:px-6 py-4">
          <div class="flex flex-col space-y-4 sm:flex-row sm:items-center sm:justify-between sm:space-y-0 gap-4">
            <div class="flex-1 max-w-lg">
              <div class="relative">
                <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                  <MagnifyingGlassIcon class="h-5 w-5 text-gray-400" />
                </div>
                <input
                  v-model="searchTerm"
                  type="text"
                  class="block w-full pl-10 pr-3 py-2 border border-gray-300 rounded-md leading-5 bg-white placeholder-gray-500 focus:outline-none focus:placeholder-gray-400 focus:ring-1 focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                  placeholder="Search venues..."
                />
              </div>
            </div>
            
            <div class="flex flex-col space-y-3 sm:flex-row sm:items-center sm:space-y-0 gap-3 sm:gap-4">
              <select
                v-model="selectedCategory"
                class="block w-full sm:w-40 px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
              >
                <option value="">All Categories</option>
                <option v-for="category in categories" :key="category.id" :value="category.id">
                  {{ category.name }}
                </option>
              </select>
              
              <select
                v-model="selectedStatus"
                class="block w-full sm:w-32 px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
              >
                <option value="">All Status</option>
                <option value="active">Active</option>
                <option value="inactive">Inactive</option>
              </select>
            </div>
          </div>
        </div>
      </div>

      <!-- Venues List -->
      <div v-if="loading" class="space-y-4">
        <div v-for="i in 6" :key="i" class="bg-white rounded-lg shadow animate-pulse">
          <div class="p-6">
            <div class="flex items-start justify-between">
              <div class="flex items-center space-x-4 flex-1">
                <div class="h-12 w-12 bg-gray-300 rounded-full"></div>
                <div class="flex-1">
                  <div class="h-5 bg-gray-300 rounded w-1/3 mb-2"></div>
                  <div class="h-4 bg-gray-300 rounded w-1/4 mb-2"></div>
                  <div class="h-3 bg-gray-300 rounded w-1/2 mb-1"></div>
                  <div class="h-3 bg-gray-300 rounded w-2/3"></div>
                </div>
              </div>
              <div class="flex flex-col items-end space-y-3 ml-4">
                <div class="flex flex-col items-end space-y-2">
                  <div class="h-6 w-16 bg-gray-300 rounded-full"></div>
                  <div class="h-4 w-20 bg-gray-300 rounded"></div>
                </div>
                <div class="flex space-x-2">
                  <div class="h-8 w-16 bg-gray-300 rounded"></div>
                  <div class="h-8 w-20 bg-gray-300 rounded"></div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
      
      <div v-else-if="filteredVenues.length === 0" class="text-center py-12">
        <BuildingStorefrontIcon class="mx-auto h-12 w-12 text-gray-400" />
        <h3 class="mt-2 text-sm font-medium text-gray-900">No venues found</h3>
        <p class="mt-1 text-sm text-gray-500">
          {{ searchTerm || selectedCategory || selectedStatus ? 'Try adjusting your search criteria.' : 'Get started by creating your first venue.' }}
        </p>
        <div class="mt-6">
          <router-link
            to="/backoffice/venues/new"
            class="inline-flex items-center px-4 py-2 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700"
          >
            <BuildingStorefrontIcon class="-ml-1 mr-2 h-4 w-4" />
            New Venue
          </router-link>
        </div>
      </div>
      
      <div v-else class="space-y-4">
        <div 
          v-for="venue in paginatedVenues" 
          :key="venue.id"
          class="bg-white rounded-lg shadow hover:shadow-md transition-shadow"
        >
          <div class="p-4 sm:p-6">
            <div class="flex flex-col space-y-4 sm:flex-row sm:items-start sm:justify-between sm:space-y-0">
              <div class="flex items-start space-x-3 sm:space-x-4 flex-1 min-w-0">
                <div class="flex-shrink-0">
                  <div class="h-10 w-10 sm:h-12 sm:w-12 bg-blue-100 rounded-full flex items-center justify-center">
                    <BuildingStorefrontIcon class="h-5 w-5 sm:h-6 sm:w-6 text-blue-600" />
                  </div>
                </div>
                <div class="flex-1 min-w-0">
                  <h3 class="text-base sm:text-lg font-medium text-gray-900 break-words">{{ venue.name }}</h3>
                  <p class="text-sm text-gray-500 break-words">{{ venue.categoryName }}</p>
                  <div class="text-xs sm:text-sm text-gray-600 mt-2 space-y-1">
                    <p class="break-words">{{ venue.streetAddress }}</p>
                    <p class="break-words">{{ venue.locality }}, {{ venue.region }} {{ venue.postalCode }}</p>
                  </div>
                </div>
              </div>
              
              <div class="flex flex-row justify-between items-center sm:flex-col sm:items-end sm:space-y-3 sm:ml-4">
                <!-- Status and Special Count -->
                <div class="flex flex-col items-start sm:items-end space-y-2">
                  <span 
                    :class="[
                      'inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium',
                      venue.isActive ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-800'
                    ]"
                  >
                    {{ venue.isActive ? 'Active' : 'Inactive' }}
                  </span>
                  <div class="flex items-center text-xs sm:text-sm text-gray-500">
                    <StarIcon class="h-3 w-3 sm:h-4 sm:w-4 mr-1" />
                    <span>{{ venue.activeSpecialsCount }} specials</span>
                  </div>
                </div>
                
                <!-- Action Buttons -->
                <div class="flex items-center space-x-2">
                  <button
                    @click="goToVenueDetail(venue.id)"
                    class="inline-flex items-center px-2 sm:px-3 py-1.5 sm:py-2 text-xs sm:text-sm font-medium text-blue-600 bg-blue-50 rounded-md hover:bg-blue-100 transition-colors"
                    title="View venue details"
                  >
                    <ChevronRightIcon class="h-3 w-3 sm:h-4 sm:w-4 sm:mr-1" />
                    <span class="hidden sm:inline">View</span>
                  </button>
                  <button
                    v-if="canDeleteVenues"
                    @click="confirmDeleteVenue(venue)"
                    class="inline-flex items-center px-2 sm:px-3 py-1.5 sm:py-2 text-xs sm:text-sm font-medium text-red-600 bg-red-50 rounded-md hover:bg-red-100 transition-colors"
                    title="Delete venue"
                  >
                    <TrashIcon class="h-3 w-3 sm:h-4 sm:w-4 sm:mr-1" />
                    <span class="hidden sm:inline">Delete</span>
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Pagination -->
      <div v-if="totalPages > 1" class="mt-8 flex items-center justify-between">
        <div class="flex-1 flex justify-between sm:hidden">
          <button
            @click="currentPage--"
            :disabled="currentPage === 1"
            class="relative inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            Previous
          </button>
          <button
            @click="currentPage++"
            :disabled="currentPage === totalPages"
            class="ml-3 relative inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            Next
          </button>
        </div>
        <div class="hidden sm:flex-1 sm:flex sm:items-center sm:justify-between">
          <div>
            <p class="text-sm text-gray-700">
              Showing <span class="font-medium">{{ (currentPage - 1) * pageSize + 1 }}</span>
              to <span class="font-medium">{{ Math.min(currentPage * pageSize, filteredVenues.length) }}</span>
              of <span class="font-medium">{{ filteredVenues.length }}</span> results
            </p>
          </div>
          <div>
            <nav class="relative z-0 inline-flex rounded-md shadow-sm -space-x-px" aria-label="Pagination">
              <button
                @click="currentPage--"
                :disabled="currentPage === 1"
                class="relative inline-flex items-center px-2 py-2 rounded-l-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                <span class="sr-only">Previous</span>
                <ChevronLeftIcon class="h-5 w-5" />
              </button>
              
              <button
                v-for="page in visiblePages"
                :key="page"
                @click="currentPage = page"
                :class="[
                  'relative inline-flex items-center px-4 py-2 border text-sm font-medium',
                  page === currentPage
                    ? 'z-10 bg-blue-50 border-blue-500 text-blue-600'
                    : 'bg-white border-gray-300 text-gray-500 hover:bg-gray-50'
                ]"
              >
                {{ page }}
              </button>
              
              <button
                @click="currentPage++"
                :disabled="currentPage === totalPages"
                class="relative inline-flex items-center px-2 py-2 rounded-r-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                <span class="sr-only">Next</span>
                <ChevronRightIcon class="h-5 w-5" />
              </button>
            </nav>
          </div>
        </div>
      </div>
    </div>

    <!-- Toast Notifications -->
    <Toast
      v-if="showToast"
      :type="toastType"
      :title="toastMessage"
      @close="showToast = false"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { 
  BuildingStorefrontIcon, 
  StarIcon, 
  CheckCircleIcon,
  FireIcon,
  MagnifyingGlassIcon,
  ChevronRightIcon,
  ChevronLeftIcon,
  TrashIcon
} from '@heroicons/vue/24/outline'
import apiService from '../services/api'
import Toast from '../components/Toast.vue'
import { usePermissions } from '../composables/usePermissions'
import type { VenueSummary, VenueCategory } from '../types/api'

const router = useRouter()
const route = useRoute()

// Permissions
const { 
  canCreateVenues, 
  canDeleteVenues, 
  getVenuePermissions 
} = usePermissions()

// State
const venues = ref<VenueSummary[]>([])
const categories = ref<VenueCategory[]>([])
const loading = ref(true)
const searchTerm = ref('')
const selectedCategory = ref<number | ''>('')
const selectedStatus = ref<'active' | 'inactive' | ''>('')

// Toast notifications
const showToast = ref(false)
const toastMessage = ref('')
const toastType = ref<'success' | 'error' | 'warning' | 'info'>('info')

// Pagination
const currentPage = ref(1)
const pageSize = ref(9)

// Computed properties
const activeVenues = computed(() => venues.value.filter(v => v.isActive))

const totalSpecials = computed(() => 
  venues.value.reduce((sum, venue) => sum + venue.activeSpecialsCount, 0)
)

const activeSpecials = computed(() => totalSpecials.value)

const filteredVenues = computed(() => {
  let filtered = venues.value

  if (searchTerm.value) {
    const searchLower = searchTerm.value.toLowerCase()
    filtered = filtered.filter(venue => 
      venue.name.toLowerCase().includes(searchLower) ||
      venue.streetAddress.toLowerCase().includes(searchLower) ||
      venue.locality.toLowerCase().includes(searchLower)
    )
  }

  if (selectedCategory.value) {
    filtered = filtered.filter(venue => venue.categoryId === selectedCategory.value)
  }

  if (selectedStatus.value) {
    filtered = filtered.filter(venue => 
      selectedStatus.value === 'active' ? venue.isActive : !venue.isActive
    )
  }

  return filtered
})

const totalPages = computed(() => Math.ceil(filteredVenues.value.length / pageSize.value))

const paginatedVenues = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value
  const end = start + pageSize.value
  return filteredVenues.value.slice(start, end)
})

const visiblePages = computed(() => {
  const pages = []
  const start = Math.max(1, currentPage.value - 2)
  const end = Math.min(totalPages.value, currentPage.value + 2)
  
  for (let i = start; i <= end; i++) {
    pages.push(i)
  }
  
  return pages
})

// Methods
const goToVenueDetail = (venueId: number) => {
  router.push(`/backoffice/venues/${venueId}`)
}

const confirmDeleteVenue = (venue: VenueSummary) => {
  router.push({
    path: '/confirm',
    query: {
      type: 'venue',
      id: venue.id.toString(),
      name: venue.name,
      returnTo: '/backoffice'
    }
  })
}

const showToastMessage = (message: string, type: 'success' | 'error' | 'warning' | 'info') => {
  toastMessage.value = message
  toastType.value = type
  showToast.value = true
}

const loadVenues = async () => {
  try {
    loading.value = true
    const response = await apiService.getMyVenues()
    if (response.data) {
      venues.value = response.data
    }
  } catch (error) {
    console.error('Failed to load venues:', error)
  } finally {
    loading.value = false
  }
}

const loadCategories = async () => {
  try {
    const response = await apiService.getVenueCategories()
    if (response.data) {
      categories.value = response.data
    }
  } catch (error) {
    console.error('Failed to load categories:', error)
  }
}

// Watch for search/filter changes
watch([searchTerm, selectedCategory, selectedStatus], () => {
  currentPage.value = 1
})

// Load data on mount
onMounted(async () => {
  await Promise.all([
    loadVenues(),
    loadCategories()
  ])
})
</script>
