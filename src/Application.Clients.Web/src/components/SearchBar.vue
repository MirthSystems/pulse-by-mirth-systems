<template>
  <div class="relative">
    <div class="flex items-center space-x-3">
      <div class="relative flex-1">
        <MagnifyingGlassIcon class="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 h-5 w-5" />
        <input
          v-model="localSearchTerm"
          type="text"
          :placeholder="placeholder"
          class="w-full pl-10 pr-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent text-sm"
          @keyup.enter="handleSearch"
        />
      </div>
      
      <div v-if="showCategoryFilter" class="relative">
        <select
          v-model="selectedCategory"
          class="appearance-none bg-white border border-gray-300 rounded-lg px-4 py-3 pr-8 text-sm focus:ring-2 focus:ring-blue-500 focus:border-transparent"
          @change="handleSearch"
        >
          <option value="">All Categories</option>
          <option v-for="category in categories" :key="category.id" :value="category.id">
            {{ category.icon }} {{ category.name }}
          </option>
        </select>
        <ChevronDownIcon class="absolute right-2 top-1/2 transform -translate-y-1/2 text-gray-400 h-4 w-4 pointer-events-none" />
      </div>
      
      <button
        @click="handleSearch"
        class="px-6 py-3 bg-blue-600 text-white font-medium rounded-lg hover:bg-blue-700 transition-colors"
        :disabled="loading"
      >
        <span v-if="loading" class="flex items-center">
          <svg class="animate-spin -ml-1 mr-2 h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
            <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
          </svg>
          Searching...
        </span>
        <span v-else>Search</span>
      </button>
    </div>
    
    <!-- Advanced filters (expandable) -->
    <div v-if="showAdvancedFilters" class="mt-4 p-4 bg-gray-50 rounded-lg">
      <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div v-if="showLocationFilter">
          <label class="block text-sm font-medium text-gray-700 mb-2">Location</label>
          <div class="flex space-x-2">
            <input
              v-model="latitude"
              type="number"
              step="any"
              placeholder="Latitude"
              class="flex-1 px-3 py-2 border border-gray-300 rounded-md text-sm"
            />
            <input
              v-model="longitude"
              type="number"
              step="any"
              placeholder="Longitude"
              class="flex-1 px-3 py-2 border border-gray-300 rounded-md text-sm"
            />
          </div>
          <button
            @click="getCurrentLocation"
            class="mt-2 text-sm text-blue-600 hover:text-blue-800"
          >
            📍 Use Current Location
          </button>
        </div>
        
        <div v-if="showLocationFilter">
          <label class="block text-sm font-medium text-gray-700 mb-2">Radius (km)</label>
          <select
            v-model="radius"
            class="w-full px-3 py-2 border border-gray-300 rounded-md text-sm"
          >
            <option :value="1000">1 km</option>
            <option :value="2000">2 km</option>
            <option :value="5000">5 km</option>
            <option :value="10000">10 km</option>
            <option :value="20000">20 km</option>
          </select>
        </div>
        
        <div v-if="showDateFilter">
          <label class="block text-sm font-medium text-gray-700 mb-2">Date</label>
          <input
            v-model="selectedDate"
            type="date"
            class="w-full px-3 py-2 border border-gray-300 rounded-md text-sm"
          />
        </div>
      </div>
    </div>
    
    <!-- Toggle advanced filters -->
    <button
      v-if="hasAdvancedFilters"
      @click="showAdvancedFilters = !showAdvancedFilters"
      class="mt-3 text-sm text-gray-600 hover:text-gray-800 flex items-center"
    >
      <AdjustmentsHorizontalIcon class="h-4 w-4 mr-1" />
      {{ showAdvancedFilters ? 'Hide' : 'Show' }} Advanced Filters
      <ChevronDownIcon 
        class="h-4 w-4 ml-1 transition-transform"
        :class="{ 'rotate-180': showAdvancedFilters }"
      />
    </button>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { 
  MagnifyingGlassIcon, 
  ChevronDownIcon, 
  AdjustmentsHorizontalIcon 
} from '@heroicons/vue/24/outline'

interface Category {
  id: number
  name: string
  icon: string
}

interface Props {
  placeholder?: string
  categories?: Category[]
  showCategoryFilter?: boolean
  showLocationFilter?: boolean
  showDateFilter?: boolean
  loading?: boolean
  modelValue?: string
}

interface Emits {
  'update:modelValue': [value: string]
  'search': [params: {
    searchTerm: string
    categoryId?: number
    latitude?: number
    longitude?: number
    radius?: number
    date?: string
  }]
  'location-updated': [latitude: number, longitude: number]
}

const props = withDefaults(defineProps<Props>(), {
  placeholder: 'Search...',
  categories: () => [],
  showCategoryFilter: false,
  showLocationFilter: false,
  showDateFilter: false,
  loading: false,
  modelValue: ''
})

const emit = defineEmits<Emits>()

const localSearchTerm = ref(props.modelValue)
const selectedCategory = ref<number | ''>('')
const latitude = ref<number | ''>('')
const longitude = ref<number | ''>('')
const radius = ref<number>(5000)
const selectedDate = ref<string>('')
const showAdvancedFilters = ref(false)

const hasAdvancedFilters = computed(() => 
  props.showLocationFilter || props.showDateFilter
)

watch(localSearchTerm, (newValue) => {
  emit('update:modelValue', newValue)
})

watch(() => props.modelValue, (newValue) => {
  localSearchTerm.value = newValue
})

function handleSearch() {
  const params = {
    searchTerm: localSearchTerm.value,
    categoryId: selectedCategory.value || undefined,
    latitude: latitude.value || undefined,
    longitude: longitude.value || undefined,
    radius: radius.value,
    date: selectedDate.value || undefined
  }
  
  emit('search', params)
}

async function getCurrentLocation() {
  if (!navigator.geolocation) {
    alert('Geolocation is not supported by this browser.')
    return
  }

  try {
    const position = await new Promise<GeolocationPosition>((resolve, reject) => {
      navigator.geolocation.getCurrentPosition(resolve, reject)
    })
    
    latitude.value = position.coords.latitude
    longitude.value = position.coords.longitude
    
    emit('location-updated', position.coords.latitude, position.coords.longitude)
  } catch (error) {
    console.error('Error getting location:', error)
    alert('Unable to get your current location.')
  }
}
</script>
