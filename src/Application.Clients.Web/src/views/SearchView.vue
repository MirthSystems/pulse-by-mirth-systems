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
const hasResults = computed(() => specialStore.searchResults.length > 0)
const specialResults = computed(() => specialStore.searchResults)
const totalResults = computed(() => specialResults.value.length)

// Search handler
const handleSearch = async (params: any) => {
  if (!params.searchTerm && !params.locationName) {
    // Clear results if no search criteria
    specialStore.searchResults = []
    return
  }

  isSearching.value = true
  
  try {
    // Call the search API with proper parameters
    await specialStore.searchSpecials(
      params.searchTerm || '',
      params.categoryId
    )
  } finally {
    isSearching.value = false
  }
}

// Initialize with URL params if any
onMounted(() => {
  const searchTerm = route.query.q as string
  const location = route.query.location as string
  const categoryId = route.query.category ? parseInt(route.query.category as string) : undefined
  
  if (searchTerm || location) {
    handleSearch({ 
      searchTerm, 
      locationName: location,
      categoryId
    })
  }
})

// Watch for route changes
watch(() => route.query, (newQuery) => {
  const searchTerm = newQuery.q as string
  const location = newQuery.location as string
  const categoryId = newQuery.category ? parseInt(newQuery.category as string) : undefined
  
  if (searchTerm || location) {
    handleSearch({ 
      searchTerm, 
      locationName: location,
      categoryId
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
              Specials ({{ totalResults }})
            </h2>
          </div>
        </div>
        
        <!-- Special Cards Grid -->
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          <SpecialCard 
            v-for="special in specialResults" 
            :key="special.id"
            :special="special"
            class="hover:shadow-lg transform hover:-translate-y-1 transition-all duration-200"
          />
        </div>
      </div>
    </div>
  </div>
</template>
