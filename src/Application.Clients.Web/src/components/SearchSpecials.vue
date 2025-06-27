<template>
  <div class="search-specials">
    <div class="max-w-3xl mx-auto">
      <form @submit.prevent="handleSearch" class="space-y-4">
        <!-- Location and Radius Row -->
        <div class="grid grid-cols-1 md:grid-cols-3 gap-3">
          <!-- Location Input -->
          <div class="relative">
            <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <MapPinIcon class="h-5 w-5 text-gray-400" />
            </div>
            <input
              v-model="locationName"
              type="text"
              placeholder="Enter location..."
              class="block w-full pl-10 pr-3 py-3 border-0 rounded-lg leading-5 bg-white/80 backdrop-blur-sm placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-white/50 focus:bg-white shadow-md"
            />
          </div>

          <!-- Radius Dropdown -->
          <select
            v-model="radius"
            class="block w-full py-3 px-3 border-0 rounded-lg leading-5 bg-white/80 backdrop-blur-sm focus:outline-none focus:ring-2 focus:ring-white/50 focus:bg-white shadow-md"
          >
            <option :value="1000">1 km</option>
            <option :value="2000">2 km</option>
            <option :value="5000">5 km</option>
            <option :value="10000">10 km</option>
            <option :value="25000">25 km</option>
          </select>

          <!-- Search Button -->
          <button
            type="submit"
            :disabled="isSearching"
            class="inline-flex items-center justify-center px-6 py-3 border border-transparent text-base font-semibold rounded-lg text-blue-600 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-white/50 disabled:opacity-50 disabled:cursor-not-allowed transition-all shadow-md hover:shadow-lg"
          >
            <MagnifyingGlassIcon v-if="!isSearching" class="mr-2 h-5 w-5" />
            <div v-else class="animate-spin rounded-full h-5 w-5 border-b-2 border-blue-600 mr-2"></div>
            {{ isSearching ? 'Searching...' : 'Search' }}
          </button>
        </div>

        <!-- Optional Filter Expansion -->
        <div class="text-center">
          <button
            type="button"
            @click="showFilters = !showFilters"
            class="inline-flex items-center px-4 py-2 text-sm font-medium text-white bg-white/20 hover:bg-white/30 rounded-lg transition-all backdrop-blur-sm border border-white/20"
          >
            <AdjustmentsHorizontalIcon class="mr-2 h-4 w-4" />
            {{ showFilters ? 'Hide' : 'Show' }} Filters
            <ChevronDownIcon 
              class="ml-2 h-4 w-4 transition-transform"
              :class="{ 'rotate-180': showFilters }"
            />
          </button>
        </div>

        <!-- Expanded Filters -->
        <div v-if="showFilters" class="bg-white/10 backdrop-blur-sm rounded-lg p-4 space-y-3">
          <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
            <!-- Search Term Filter -->
            <div>
              <label class="block text-sm font-medium text-white mb-2">Search Term (Optional)</label>
              <input
                v-model="searchTerm"
                type="text"
                placeholder="Keywords, venue, cuisine..."
                class="block w-full py-2 px-3 border-0 rounded-lg bg-white/90 focus:outline-none focus:ring-2 focus:ring-white/50"
              />
            </div>

            <!-- Category Filter -->
            <div>
              <label class="block text-sm font-medium text-white mb-2">Category</label>
              <select
                v-model="categoryId"
                class="block w-full py-2 px-3 border-0 rounded-lg bg-white/90 focus:outline-none focus:ring-2 focus:ring-white/50"
              >
                <option :value="undefined">All Categories</option>
                <option 
                  v-for="category in categories" 
                  :key="category.id" 
                  :value="category.id"
                >
                  {{ category.name }}
                </option>
              </select>
            </div>

            <!-- Sort By -->
            <div>
              <label class="block text-sm font-medium text-white mb-2">Sort By</label>
              <select
                v-model="sortBy"
                class="block w-full py-2 px-3 border-0 rounded-lg bg-white/90 focus:outline-none focus:ring-2 focus:ring-white/50"
              >
                <option value="relevance">Relevance</option>
                <option value="distance">Distance</option>
                <option value="newest">Newest</option>
                <option value="ending-soon">Ending Soon</option>
              </select>
            </div>
          </div>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useSpecialStore } from '../stores/special'
import { 
  MagnifyingGlassIcon, 
  MapPinIcon, 
  AdjustmentsHorizontalIcon,
  ChevronDownIcon 
} from '@heroicons/vue/24/outline'

const router = useRouter()
const specialStore = useSpecialStore()

// Search parameters
const searchTerm = ref('')
const locationName = ref('')
const radius = ref(5000)
const categoryId = ref<number | undefined>(undefined)
const sortBy = ref('relevance')
const showFilters = ref(false)
const isSearching = ref(false)

// Categories from store
const categories = ref<any[]>([])

// Load categories on mount
onMounted(async () => {
  await specialStore.fetchSpecialCategories()
  categories.value = specialStore.categories
})

const handleSearch = async () => {
  isSearching.value = true
  
  try {
    // Navigate to search page with parameters
    router.push({
      name: 'Search',
      query: {
        q: searchTerm.value.trim() || undefined,
        location: locationName.value || undefined,
        radius: radius.value.toString(),
        category: categoryId.value?.toString() || undefined,
        sortBy: sortBy.value
      }
    })
  } finally {
    isSearching.value = false
  }
}
</script>
