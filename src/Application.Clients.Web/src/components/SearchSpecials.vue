<template>
  <div class="search-specials">
    <div class="max-w-3xl mx-auto">
      <form @submit.prevent="handleSearch" class="space-y-4">
        <!-- Main Search Input -->
        <div class="relative">
          <div class="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
            <MagnifyingGlassIcon class="h-6 w-6 text-gray-400" />
          </div>
          <input
            v-model="searchQuery"
            type="text"
            placeholder="Search for specials, venues, or cuisines..."
            class="block w-full pl-12 pr-4 py-4 border-0 rounded-xl leading-5 bg-white/90 backdrop-blur-sm placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-white/50 focus:bg-white text-lg shadow-lg"
          />
        </div>

        <!-- Location and Quick Filters Row -->
        <div class="grid grid-cols-1 md:grid-cols-3 gap-3">
          <!-- Location Input -->
          <div class="relative">
            <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <MapPinIcon class="h-5 w-5 text-gray-400" />
            </div>
            <input
              v-model="location"
              type="text"
              placeholder="Location"
              class="block w-full pl-10 pr-3 py-3 border-0 rounded-lg leading-5 bg-white/80 backdrop-blur-sm placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-white/50 focus:bg-white shadow-md"
            />
          </div>

          <!-- Category Filter -->
          <select
            v-model="selectedCategory"
            class="block w-full py-3 px-3 border-0 rounded-lg leading-5 bg-white/80 backdrop-blur-sm focus:outline-none focus:ring-2 focus:ring-white/50 focus:bg-white shadow-md"
          >
            <option value="">All Categories</option>
            <option value="food">Food & Dining</option>
            <option value="drinks">Drinks & Bars</option>
            <option value="entertainment">Entertainment</option>
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
      </form>

      <!-- Quick Filters -->
      <div class="mt-6 flex flex-wrap justify-center gap-2">
        <span class="text-sm text-blue-100 mr-2">Popular:</span>
        <button
          v-for="tag in popularTags"
          :key="tag"
          @click="handleQuickSearch(tag)"
          class="inline-flex items-center px-3 py-1.5 rounded-full text-sm font-medium bg-white/20 text-white hover:bg-white/30 transition-all backdrop-blur-sm border border-white/20"
        >
          {{ tag }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { MagnifyingGlassIcon, MapPinIcon } from '@heroicons/vue/24/outline'

const router = useRouter()

const searchQuery = ref('')
const location = ref('')
const selectedCategory = ref('')
const isSearching = ref(false)

const popularTags = [
  'Happy Hour',
  'Pizza',
  'Brunch',
  'Live Music',
  'Date Night',
  'Craft Beer'
]

const handleSearch = async () => {
  isSearching.value = true
  
  // Simulate search delay
  await new Promise(resolve => setTimeout(resolve, 500))
  
  // Navigate to search results with query parameters
  router.push({
    name: 'Search',
    query: {
      q: searchQuery.value || undefined,
      location: location.value || undefined,
      category: selectedCategory.value || undefined
    }
  })
  
  isSearching.value = false
}

const handleQuickSearch = (tag: string) => {
  searchQuery.value = tag
  handleSearch()
}
</script>
