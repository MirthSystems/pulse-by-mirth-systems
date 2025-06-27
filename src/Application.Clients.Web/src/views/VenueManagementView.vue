<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Header -->
    <div class="bg-white shadow">
      <div class="px-4 sm:px-6 lg:px-8 py-6">
        <div class="flex items-center justify-between">
          <div>
            <h1 class="text-2xl font-bold text-gray-900">Venue Management</h1>
            <p class="mt-1 text-sm text-gray-500">
              Manage your venues, edit details, and track specials
            </p>
          </div>
          <router-link
            to="/backoffice/venues/new"
            class="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
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
      <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
        <div class="bg-white overflow-hidden shadow rounded-lg">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <BuildingStorefrontIcon class="h-6 w-6 text-gray-400" />
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 truncate">Total Venues</dt>
                  <dd class="text-lg font-medium text-gray-900">{{ venues.length }}</dd>
                </dl>
              </div>
            </div>
          </div>
        </div>
        
        <div class="bg-white overflow-hidden shadow rounded-lg">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <CheckCircleIcon class="h-6 w-6 text-green-400" />
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 truncate">Active Venues</dt>
                  <dd class="text-lg font-medium text-gray-900">{{ activeVenues.length }}</dd>
                </dl>
              </div>
            </div>
          </div>
        </div>
        
        <div class="bg-white overflow-hidden shadow rounded-lg">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <StarIcon class="h-6 w-6 text-yellow-400" />
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 truncate">Total Specials</dt>
                  <dd class="text-lg font-medium text-gray-900">{{ totalSpecials }}</dd>
                </dl>
              </div>
            </div>
          </div>
        </div>
        
        <div class="bg-white overflow-hidden shadow rounded-lg">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <FireIcon class="h-6 w-6 text-red-400" />
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 truncate">Active Specials</dt>
                  <dd class="text-lg font-medium text-gray-900">{{ activeSpecials }}</dd>
                </dl>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Search and Filters -->
      <div class="bg-white shadow rounded-lg mb-6">
        <div class="px-6 py-4">
          <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
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
            
            <div class="flex items-center space-x-4">
              <select
                v-model="selectedCategory"
                class="block w-full pl-3 pr-10 py-2 text-base border-gray-300 focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm rounded-md"
              >
                <option value="">All Categories</option>
                <option v-for="category in categories" :key="category.id" :value="category.id">
                  {{ category.icon }} {{ category.name }}
                </option>
              </select>
              
              <select
                v-model="selectedStatus"
                class="block w-full pl-3 pr-10 py-2 text-base border-gray-300 focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm rounded-md"
              >
                <option value="">All Status</option>
                <option value="active">Active</option>
                <option value="inactive">Inactive</option>
              </select>
            </div>
          </div>
        </div>
      </div>

      <!-- Venues Table -->
      <div class="bg-white shadow rounded-lg overflow-hidden">
        <div class="px-6 py-4 border-b border-gray-200">
          <h3 class="text-lg leading-6 font-medium text-gray-900">Venues</h3>
        </div>
        
        <div v-if="loading" class="px-6 py-12">
          <div class="flex items-center justify-center">
            <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
            <span class="ml-2 text-gray-600">Loading venues...</span>
          </div>
        </div>
        
        <div v-else-if="filteredVenues.length === 0" class="px-6 py-12 text-center">
          <BuildingStorefrontIcon class="mx-auto h-12 w-12 text-gray-400" />
          <h3 class="mt-2 text-sm font-medium text-gray-900">No venues found</h3>
          <p class="mt-1 text-sm text-gray-500">
            Get started by creating a new venue.
          </p>
          <div class="mt-6">
            <button
              @click="createVenue"
              class="inline-flex items-center px-4 py-2 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700"
            >
              <BuildingStorefrontIcon class="-ml-1 mr-2 h-4 w-4" />
              New Venue
            </button>
          </div>
        </div>
        
        <div v-else class="overflow-x-auto">
          <table class="min-w-full divide-y divide-gray-200">
            <thead class="bg-gray-50">
              <tr>
                <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Venue
                </th>
                <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Category
                </th>
                <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Location
                </th>
                <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Specials
                </th>
                <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Status
                </th>
                <th scope="col" class="relative px-6 py-3">
                  <span class="sr-only">Actions</span>
                </th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-gray-200">
              <tr v-for="venue in paginatedVenues" :key="venue.id" class="hover:bg-gray-50">
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="flex items-center">
                    <div class="flex-shrink-0 h-10 w-10">
                      <img 
                        v-if="venue.profileImage" 
                        class="h-10 w-10 rounded-lg object-cover" 
                        :src="venue.profileImage" 
                        :alt="venue.name" 
                      />
                      <div 
                        v-else
                        class="h-10 w-10 rounded-lg bg-gray-200 flex items-center justify-center"
                      >
                        <BuildingStorefrontIcon class="h-6 w-6 text-gray-400" />
                      </div>
                    </div>
                    <div class="ml-4">
                      <div class="text-sm font-medium text-gray-900">{{ venue.name }}</div>
                      <div class="text-sm text-gray-500">{{ venue.description || 'No description' }}</div>
                    </div>
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="flex items-center">
                    <span class="text-lg mr-2">{{ venue.categoryIcon }}</span>
                    <span class="text-sm text-gray-900">{{ venue.categoryName }}</span>
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                  <div>
                    {{ venue.streetAddress }}
                  </div>
                  <div>
                    {{ venue.locality }}, {{ venue.region }}
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span class="text-sm text-gray-900">{{ venue.activeSpecialsCount }}</span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span 
                    :class="[
                      'inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium',
                      venue.isActive 
                        ? 'bg-green-100 text-green-800' 
                        : 'bg-red-100 text-red-800'
                    ]"
                  >
                    {{ venue.isActive ? 'Active' : 'Inactive' }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                  <div class="flex items-center justify-end space-x-2">
                    <router-link
                      :to="`/backoffice/venues/${venue.id}`"
                      class="text-blue-600 hover:text-blue-900 p-1"
                      title="View/Edit venue"
                    >
                      <PencilIcon class="h-4 w-4" />
                    </router-link>
                    <button
                      @click="confirmDelete(venue)"
                      class="text-red-600 hover:text-red-900 p-1"
                      title="Delete venue"
                    >
                      <TrashIcon class="h-4 w-4" />
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
        
        <!-- Pagination -->
        <div v-if="totalPages > 1" class="bg-white px-4 py-3 flex items-center justify-between border-t border-gray-200 sm:px-6">
          <div class="flex-1 flex justify-between sm:hidden">
            <button
              @click="previousPage"
              :disabled="currentPage === 1"
              class="relative inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50"
            >
              Previous
            </button>
            <button
              @click="nextPage"
              :disabled="currentPage === totalPages"
              class="ml-3 relative inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50"
            >
              Next
            </button>
          </div>
          <div class="hidden sm:flex-1 sm:flex sm:items-center sm:justify-between">
            <div>
              <p class="text-sm text-gray-700">
                Showing
                <span class="font-medium">{{ (currentPage - 1) * pageSize + 1 }}</span>
                to
                <span class="font-medium">{{ Math.min(currentPage * pageSize, filteredVenues.length) }}</span>
                of
                <span class="font-medium">{{ filteredVenues.length }}</span>
                results
              </p>
            </div>
            <div>
              <nav class="relative z-0 inline-flex rounded-md shadow-sm -space-x-px" aria-label="Pagination">
                <button
                  @click="previousPage"
                  :disabled="currentPage === 1"
                  class="relative inline-flex items-center px-2 py-2 rounded-l-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50 disabled:opacity-50"
                >
                  <ChevronLeftIcon class="h-5 w-5" />
                </button>
                <button
                  v-for="page in visiblePages"
                  :key="page"
                  @click="goToPage(page)"
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
                  @click="nextPage"
                  :disabled="currentPage === totalPages"
                  class="relative inline-flex items-center px-2 py-2 rounded-r-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50 disabled:opacity-50"
                >
                  <ChevronRightIcon class="h-5 w-5" />
                </button>
              </nav>
            </div>
          </div>
        </div>
      </div>
    </div>

  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useVenueStore } from '@/stores/venue'
import apiService from '@/services/api'
import type { VenueSummary, VenueCategory } from '@/types/api'
import {
  BuildingStorefrontIcon,
  StarIcon,
  CheckCircleIcon,
  FireIcon,
  MagnifyingGlassIcon,
  PencilIcon,
  TrashIcon,
  ChevronLeftIcon,
  ChevronRightIcon
} from '@heroicons/vue/24/outline'

const router = useRouter()
const route = useRoute()
const venueStore = useVenueStore()

// State
const venues = ref<VenueSummary[]>([])
const categories = ref<VenueCategory[]>([])
const loading = ref(true)
const searchTerm = ref('')
const selectedCategory = ref<number | ''>('')
const selectedStatus = ref<'active' | 'inactive' | ''>('')

// Pagination
const currentPage = ref(1)
const pageSize = ref(10)

// Computed properties
const activeVenues = computed(() => venues.value.filter(v => v.isActive))

const totalSpecials = computed(() => 
  venues.value.reduce((sum, venue) => sum + venue.activeSpecialsCount, 0)
)

const activeSpecials = computed(() => totalSpecials.value)

const filteredVenues = computed(() => {
  let filtered = venues.value

  if (searchTerm.value) {
    const term = searchTerm.value.toLowerCase()
    filtered = filtered.filter(venue =>
      venue.name.toLowerCase().includes(term) ||
      venue.description?.toLowerCase().includes(term) ||
      venue.streetAddress.toLowerCase().includes(term) ||
      venue.locality.toLowerCase().includes(term)
    )
  }

  if (selectedCategory.value) {
    filtered = filtered.filter(venue => venue.categoryId === selectedCategory.value)
  }

  if (selectedStatus.value) {
    const isActive = selectedStatus.value === 'active'
    filtered = filtered.filter(venue => venue.isActive === isActive)
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
const loadData = async () => {
  loading.value = true
  try {
    const [venuesResponse, categoriesResponse] = await Promise.all([
      apiService.getVenues(),
      apiService.getVenueCategories()
    ])

    if (venuesResponse.success) {
      venues.value = venuesResponse.data
    }

    if (categoriesResponse.success) {
      categories.value = categoriesResponse.data
    }
  } catch (error) {
    console.error('Error loading data:', error)
  } finally {
    loading.value = false
  }
}

const createVenue = () => {
  router.push('/backoffice/venues/new')
}

const confirmDelete = (venue: VenueSummary) => {
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

const deleteVenue = async () => {
  // This function is now handled by ConfirmView
  // Reload venues if we're returning from a successful delete
  if (route.query.success) {
    await loadData()
  }
}

// Pagination methods
const previousPage = () => {
  if (currentPage.value > 1) {
    currentPage.value--
  }
}

const nextPage = () => {
  if (currentPage.value < totalPages.value) {
    currentPage.value++
  }
}

const goToPage = (page: number) => {
  currentPage.value = page
}

// Watch for filter changes to reset pagination
watch([searchTerm, selectedCategory, selectedStatus], () => {
  currentPage.value = 1
})

onMounted(() => {
  loadData()
})
</script>
