<script setup lang="ts">
import { onMounted, ref, computed, watch } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useSpecialStore } from '../stores/special'
import { useVenueStore } from '../stores/venue'
import SpecialCard from '../components/SpecialCard.vue'
import SearchBar from '../components/SearchBar.vue'
import { 
  StarIcon,
  ClockIcon,
  FunnelIcon,
  FireIcon,
  CalendarIcon,
  ListBulletIcon
} from '@heroicons/vue/24/outline'
import { format, isToday, isTomorrow } from 'date-fns'

const router = useRouter()
const route = useRoute()
const specialStore = useSpecialStore()
const venueStore = useVenueStore()

const viewMode = ref<'grid' | 'list'>('grid')
const sortBy = ref<'name' | 'venue' | 'category' | 'active'>('active')
const sortOrder = ref<'asc' | 'desc'>('asc')
const showFilters = ref(false)
const selectedCategories = ref<number[]>([])
const selectedTimeFilter = ref<'all' | 'now' | 'today' | 'tomorrow' | 'weekend'>('all')

const timeFilterOptions = [
  { value: 'all', label: 'All Times', icon: ClockIcon },
  { value: 'now', label: 'Active Now', icon: FireIcon },
  { value: 'today', label: 'Today', icon: CalendarIcon },
  { value: 'tomorrow', label: 'Tomorrow', icon: CalendarIcon },
  { value: 'weekend', label: 'This Weekend', icon: CalendarIcon },
]

const filteredSpecials = computed(() => {
  let specials = [...specialStore.specials]
  
  // Filter by categories
  if (selectedCategories.value.length > 0) {
    specials = specials.filter(special => 
      selectedCategories.value.includes(special.specialCategoryId)
    )
  }
  
  // Filter by time
  const now = new Date()
  switch (selectedTimeFilter.value) {
    case 'now':
      // This would require checking if the special is currently active
      // For now, we'll just show all specials
      break
    case 'today':
      // Filter specials that are active today
      break
    case 'tomorrow':
      // Filter specials that are active tomorrow
      break
    case 'weekend':
      // Filter specials that are active on weekend
      break
  }
  
  // Sort specials
  specials.sort((a, b) => {
    let comparison = 0
    
    switch (sortBy.value) {
      case 'name':
        comparison = a.title.localeCompare(b.title)
        break
      case 'venue':
        comparison = a.venueName.localeCompare(b.venueName)
        break
      case 'category':
        comparison = a.categoryName.localeCompare(b.categoryName)
        break
      case 'active':
        // Sort by active status, then by title
        comparison = a.title.localeCompare(b.title)
        break
      default:
        comparison = a.title.localeCompare(b.title)
    }
    
    return sortOrder.value === 'desc' ? -comparison : comparison
  })
  
  return specials
})

const specialsByCategory = computed(() => {
  const grouped: Record<string, typeof filteredSpecials.value> = {}
  filteredSpecials.value.forEach(special => {
    if (!grouped[special.categoryName]) {
      grouped[special.categoryName] = []
    }
    grouped[special.categoryName].push(special)
  })
  return grouped
})

const handleSearch = (params: { searchTerm: string; categoryId?: number; latitude?: number; longitude?: number; radius?: number; date?: string }) => {
  if (params.searchTerm) {
    specialStore.searchSpecials(params.searchTerm)
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
  selectedTimeFilter.value = 'all'
  sortBy.value = 'active'
  sortOrder.value = 'asc'
}

const getTimeFilterLabel = (special: any) => {
  // This would parse the CRON schedule and return a human-readable time
  // For now, return a placeholder
  return special.schedule || 'Check venue for times'
}

// Load specials and categories on mount
onMounted(async () => {
  await Promise.all([
    specialStore.fetchSpecials(),
    specialStore.fetchSpecialCategories()
  ])
  
  // Handle query parameters
  const query = route.query
  if (query.category) {
    const categoryId = parseInt(query.category as string)
    if (!isNaN(categoryId)) {
      selectedCategories.value = [categoryId]
    }
  }
  if (query.time) {
    selectedTimeFilter.value = query.time as any
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
  if (newQuery.time) {
    selectedTimeFilter.value = newQuery.time as any
  }
})
</script>

<template>
  <div>
    <!-- Page Header -->
    <div class="bg-gradient-to-r from-red-500 to-pink-600 text-white">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
        <div class="flex items-center justify-between">
          <div>
            <div class="flex items-center mb-4">
              <FireIcon class="h-8 w-8 text-yellow-300 mr-3" />
              <h1 class="text-4xl font-bold">Hot Specials</h1>
            </div>
            <p class="text-xl text-red-100">
              Discover amazing deals happening right now
            </p>
          </div>
          
          <div class="hidden lg:flex items-center space-x-4">
            <div class="flex bg-red-400/20 rounded-lg p-1">
              <button
                @click="viewMode = 'grid'"
                :class="[
                  'px-3 py-2 rounded-md text-sm font-medium transition-colors',
                  viewMode === 'grid' 
                    ? 'bg-white text-red-600 shadow-sm' 
                    : 'text-red-100 hover:text-white'
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
                    ? 'bg-white text-red-600 shadow-sm' 
                    : 'text-red-100 hover:text-white'
                ]"
              >
                <ListBulletIcon class="w-4 h-4" />
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Search and Filters -->
    <div class="bg-white border-b border-gray-200 py-8">
      <!-- Search Bar -->
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 mb-6">
        <SearchBar 
          @search="handleSearch"
          :categories="specialStore.categories"
          placeholder="Search specials by name, venue, or type..."
          :show-category-filter="true"
          :show-date-filter="true"
        />
      </div>
      
      <!-- Filter Controls -->
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex flex-col lg:flex-row lg:items-center lg:justify-between gap-4">
          <div class="flex items-center space-x-4">
            <button
              @click="showFilters = !showFilters"
              class="inline-flex items-center px-4 py-2 border border-gray-300 rounded-lg text-sm font-medium text-gray-700 bg-white hover:bg-gray-50"
            >
              <FunnelIcon class="h-4 w-4 mr-2" />
              Filters
              <span v-if="selectedCategories.length > 0 || selectedTimeFilter !== 'all'" 
                    class="ml-2 bg-red-100 text-red-800 text-xs px-2 py-1 rounded-full">
                {{ selectedCategories.length + (selectedTimeFilter !== 'all' ? 1 : 0) }}
              </span>
            </button>
            
            <!-- Quick Time Filters -->
            <div class="flex space-x-2">            <button
              v-for="filter in timeFilterOptions.slice(0, 3)"
              :key="filter.value"
              @click="selectedTimeFilter = filter.value as 'all' | 'now' | 'today' | 'tomorrow' | 'weekend'"
              :class="[
                'inline-flex items-center px-3 py-2 rounded-lg text-sm font-medium transition-colors',
                selectedTimeFilter === filter.value
                  ? 'bg-red-100 text-red-700 border border-red-200'
                  : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
              ]"
            >
                <component :is="filter.icon" class="h-4 w-4 mr-1" />
                {{ filter.label }}
              </button>
            </div>
            
            <div class="text-sm text-gray-600">
              {{ filteredSpecials.length }} specials found
            </div>
          </div>
          
          <div class="flex items-center space-x-4">
            <div class="flex items-center space-x-2">
              <label class="text-sm font-medium text-gray-700">Sort by:</label>
              <select 
                v-model="sortBy"
                class="border border-gray-300 rounded-md px-3 py-1 text-sm focus:ring-2 focus:ring-red-500 focus:border-transparent"
              >
                <option value="active">Active First</option>
                <option value="name">Name</option>
                <option value="venue">Venue</option>
                <option value="category">Category</option>
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
        <div v-if="showFilters" class="mt-6 pt-6 border-t border-gray-200">
          <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
            <!-- Category Filters -->
            <div>
              <h3 class="text-sm font-medium text-gray-900 mb-3">Categories</h3>
              <div class="flex flex-wrap gap-2">
                <button
                  v-for="category in specialStore.categories"
                  :key="category.id"
                  @click="toggleCategory(category.id)"
                  :class="[
                    'px-3 py-2 rounded-full text-sm font-medium border transition-colors',
                    selectedCategories.includes(category.id)
                      ? 'bg-red-50 border-red-200 text-red-700'
                      : 'bg-white border-gray-300 text-gray-700 hover:bg-gray-50'
                  ]"
                >
                  {{ category.icon }} {{ category.name }}
                </button>
              </div>
            </div>
            
            <!-- Time Filters -->
            <div>
              <h3 class="text-sm font-medium text-gray-900 mb-3">When</h3>
              <div class="space-y-2">
                <label
                  v-for="filter in timeFilterOptions"
                  :key="filter.value"
                  class="flex items-center cursor-pointer"
                >
                  <input
                    type="radio"
                    :value="filter.value"
                    v-model="selectedTimeFilter"
                    class="h-4 w-4 text-red-600 focus:ring-red-500 border-gray-300"
                  />
                  <component :is="filter.icon" class="h-4 w-4 ml-3 mr-2 text-gray-500" />
                  <span class="text-sm text-gray-700">{{ filter.label }}</span>
                </label>
              </div>
            </div>
          </div>
          
          <div class="flex justify-end mt-6">
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

    <!-- Content -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Loading State -->
      <div v-if="specialStore.loading">
        <div :class="[
          'grid gap-6',
          viewMode === 'grid' 
            ? 'grid-cols-1 md:grid-cols-2 lg:grid-cols-3' 
            : 'grid-cols-1'
        ]">
          <div v-for="i in 9" :key="i" class="bg-white rounded-lg shadow-md p-6 animate-pulse">
            <div class="bg-gray-300 h-32 rounded-lg mb-4"></div>
            <div class="bg-gray-300 h-4 rounded mb-2"></div>
            <div class="bg-gray-300 h-4 rounded w-3/4"></div>
          </div>
        </div>
      </div>

      <!-- Empty State -->
      <div v-else-if="filteredSpecials.length === 0" class="text-center py-16">
        <StarIcon class="h-16 w-16 text-gray-300 mx-auto mb-4" />
        <h3 class="text-lg font-medium text-gray-900 mb-2">No specials found</h3>
        <p class="text-gray-600 mb-6">Try adjusting your search criteria or filters.</p>
        <button
          @click="clearFilters"
          class="px-4 py-2 bg-red-500 text-white rounded-lg hover:bg-red-600 transition-colors"
        >
          Clear Filters
        </button>
      </div>

      <!-- Specials Grid/List -->
      <div v-else>
        <div :class="[
          'grid gap-6',
          viewMode === 'grid' 
            ? 'grid-cols-1 md:grid-cols-2 lg:grid-cols-3' 
            : 'grid-cols-1'
        ]">
          <SpecialCard 
            v-for="special in filteredSpecials" 
            :key="special.id"
            :special="special"
            :class="[
              'hover:shadow-lg transform hover:-translate-y-1 transition-all duration-200',
              viewMode === 'list' ? 'md:flex md:max-w-none' : ''
            ]"
          />
        </div>
      </div>

      <!-- Error State -->
      <div v-if="specialStore.error" class="bg-red-50 border border-red-200 rounded-lg p-4 mt-4">
        <div class="flex">
          <div class="ml-3">
            <h3 class="text-sm font-medium text-red-800">Error loading specials</h3>
            <p class="mt-1 text-sm text-red-700">{{ specialStore.error }}</p>
            <button 
              @click="specialStore.fetchSpecials()"
              class="mt-3 text-sm bg-red-100 text-red-800 px-3 py-1 rounded hover:bg-red-200"
            >
              Try again
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
