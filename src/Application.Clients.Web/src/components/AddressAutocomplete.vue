<template>
  <div class="relative">
    <div class="relative">
      <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
        <MapPinIcon class="h-5 w-5 text-gray-400" />
      </div>
      <input
        ref="addressInput"
        v-model="inputValue"
        type="text"
        :placeholder="placeholder"
        :required="required"
        class="block w-full pl-10 pr-4 py-3 border border-gray-300 rounded-lg bg-white placeholder-gray-500 text-gray-900 focus:outline-none focus:ring-2 focus:ring-blue-600 focus:border-blue-600 shadow-sm transition-colors"
        @input="handleInput"
        @focus="handleFocus"
        @blur="handleBlur"
        @keydown="handleKeydown"
      />
      
      <!-- Loading indicator -->
      <div
        v-if="isLoading"
        class="absolute inset-y-0 right-0 pr-3 flex items-center"
      >
        <div class="animate-spin rounded-full h-4 w-4 border-b-2 border-blue-600"></div>
      </div>
    </div>

    <!-- Suggestions Dropdown -->
    <div
      v-if="showSuggestions && suggestions.length > 0"
      class="absolute z-50 w-full mt-1 bg-white border border-gray-300 rounded-lg shadow-lg max-h-60 overflow-auto"
    >
      <div
        v-for="(suggestion, index) in suggestions"
        :key="index"
        :class="[
          'px-4 py-3 cursor-pointer border-b border-gray-100 last:border-b-0',
          selectedIndex === index ? 'bg-blue-50 text-blue-900' : 'hover:bg-gray-50'
        ]"
        @click="selectSuggestion(suggestion)"
      >
        <div class="flex items-center">
          <MapPinIcon class="h-4 w-4 text-gray-400 mr-3 flex-shrink-0" />
          <div class="flex-1 min-w-0">
            <div class="text-sm font-medium text-gray-900 truncate">
              {{ suggestion.formattedAddress }}
            </div>
            <div v-if="suggestion.city || suggestion.region" class="text-xs text-gray-500 truncate">
              {{ [suggestion.city, suggestion.region].filter(Boolean).join(', ') }}
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Current Location Button -->
    <button
      v-if="showCurrentLocationButton"
      type="button"
      @click="getCurrentLocation"
      :disabled="isGettingLocation"
      class="absolute inset-y-0 right-0 pr-3 flex items-center text-gray-400 hover:text-blue-600 focus:text-blue-600 transition-colors"
      title="Use current location"
    >
      <div v-if="isGettingLocation" class="animate-spin rounded-full h-5 w-5 border-b-2 border-blue-600"></div>
      <LocationIcon v-else class="h-5 w-5" />
    </button>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, onMounted, onUnmounted, nextTick } from 'vue'
import { MapPinIcon } from '@heroicons/vue/24/outline'
import { useAnalytics } from '@/composables/useAnalytics'
import apiService from '@/services/api'
import type { GeocodeResult } from '@/types/api'

// Custom location icon component (you can replace with an actual icon)
const LocationIcon = MapPinIcon

interface Props {
  modelValue?: string
  placeholder?: string
  required?: boolean
  showCurrentLocationButton?: boolean
}

interface Emits {
  'update:modelValue': [value: string]
  'location-selected': [result: GeocodeResult]
  'coordinates-updated': [latitude: number, longitude: number]
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: '',
  placeholder: 'Enter an address...',
  required: false,
  showCurrentLocationButton: true
})

const emit = defineEmits<Emits>()
const { trackEvent } = useAnalytics()

// Reactive state
const addressInput = ref<HTMLInputElement>()
const inputValue = ref(props.modelValue)
const suggestions = ref<GeocodeResult[]>([])
const isLoading = ref(false)
const isGettingLocation = ref(false)
const showSuggestions = ref(false)
const selectedIndex = ref(-1)

// Debounce timeout
let searchTimeout: ReturnType<typeof setTimeout> | null = null

// Watch for external changes to modelValue
watch(() => props.modelValue, (newValue) => {
  inputValue.value = newValue
})

// Watch for input changes and emit to parent
watch(inputValue, (newValue) => {
  emit('update:modelValue', newValue)
})

// Watch for input changes and track search behavior
watch(inputValue, (newValue) => {
  if (searchTimeout) {
    clearTimeout(searchTimeout)
  }
  searchTimeout = setTimeout(() => {
    if (newValue.length > 2) {
      trackEvent('address_search', {
        query_length: newValue.length,
        component: 'address_autocomplete'
      })
    }
  }, 1000) // Debounce to avoid too many events
})

// Event handlers
const handleInput = () => {
  const query = inputValue.value.trim()
  
  // Clear previous timeout
  if (searchTimeout) {
    clearTimeout(searchTimeout)
  }
  
  // Reset selection
  selectedIndex.value = -1
  
  if (query.length < 3) {
    suggestions.value = []
    showSuggestions.value = false
    return
  }
  
  // Debounce the search
  searchTimeout = setTimeout(async () => {
    await searchAddresses(query)
  }, 300)

  // Analytics for address search
  handleSearchInput()
}

const handleFocus = () => {
  if (suggestions.value.length > 0) {
    showSuggestions.value = true
  }
}

const handleBlur = () => {
  // Delay hiding to allow for click events on suggestions
  setTimeout(() => {
    showSuggestions.value = false
    selectedIndex.value = -1
  }, 200)
}

const handleKeydown = (event: KeyboardEvent) => {
  if (!showSuggestions.value || suggestions.value.length === 0) return
  
  switch (event.key) {
    case 'ArrowDown':
      event.preventDefault()
      selectedIndex.value = Math.min(selectedIndex.value + 1, suggestions.value.length - 1)
      break
    case 'ArrowUp':
      event.preventDefault()
      selectedIndex.value = Math.max(selectedIndex.value - 1, -1)
      break
    case 'Enter':
      event.preventDefault()
      if (selectedIndex.value >= 0) {
        selectSuggestion(suggestions.value[selectedIndex.value])
      }
      break
    case 'Escape':
      showSuggestions.value = false
      selectedIndex.value = -1
      addressInput.value?.blur()
      break
  }
}

// Search for address suggestions
const searchAddresses = async (query: string) => {
  if (query.length < 3) return
  
  isLoading.value = true
  try {
    const response = await apiService.searchAddresses(query)
    if (response.success && response.data) {
      suggestions.value = response.data
      showSuggestions.value = suggestions.value.length > 0
    } else {
      suggestions.value = []
      showSuggestions.value = false
    }
  } catch (error) {
    console.error('Error searching addresses:', error)
    suggestions.value = []
    showSuggestions.value = false
  } finally {
    isLoading.value = false
  }
}

// Select a suggestion
const selectSuggestion = (suggestion: GeocodeResult) => {
  inputValue.value = suggestion.formattedAddress
  suggestions.value = []
  showSuggestions.value = false
  selectedIndex.value = -1
  
  emit('location-selected', suggestion)
  emit('coordinates-updated', suggestion.latitude, suggestion.longitude)
  
  // Track selection event
  trackEvent('address_autocomplete_select', {
    address: suggestion.formattedAddress,
    latitude: suggestion.latitude,
    longitude: suggestion.longitude,
    source: 'suggestion_dropdown'
  })
}

// Get current location
const getCurrentLocation = async () => {
  if (!navigator.geolocation) {
    alert('Geolocation is not supported by this browser.')
    return
  }

  isGettingLocation.value = true

  try {
    const position = await new Promise<GeolocationPosition>((resolve, reject) => {
      navigator.geolocation.getCurrentPosition(resolve, reject, {
        enableHighAccuracy: true,
        timeout: 10000,
        maximumAge: 300000 // 5 minutes
      })
    })

    const { latitude, longitude } = position.coords

    // Track successful location acquisition
    trackEvent('location_success', {
      source: 'address_autocomplete',
      accuracy: position.coords.accuracy,
      timestamp: position.timestamp
    })

    // Reverse geocode to get address
    try {
      const response = await apiService.reverseGeocode(latitude, longitude)
      if (response.success && response.data) {
        inputValue.value = response.data.formattedAddress
        
        // Create a GeocodeResult from the reverse geocode result
        const geocodeResult: GeocodeResult = {
          formattedAddress: response.data.formattedAddress,
          latitude,
          longitude,
          street: response.data.street,
          city: response.data.city,
          region: response.data.region,
          postalCode: response.data.postalCode,
          country: response.data.country,
          confidence: 1.0
        }
        
        emit('location-selected', geocodeResult)
        
        trackEvent('reverse_geocode_success', {
          source: 'address_autocomplete',
          address: response.data.formattedAddress
        })
      } else {
        // Fallback to coordinates
        inputValue.value = `${latitude.toFixed(6)}, ${longitude.toFixed(6)}`
        
        trackEvent('reverse_geocode_fallback', {
          source: 'address_autocomplete',
          coordinates: `${latitude}, ${longitude}`
        })
      }
      
      emit('coordinates-updated', latitude, longitude)
    } catch (reverseGeocodeError) {
      console.error('Error reverse geocoding:', reverseGeocodeError)
      // Fallback to coordinates only - don't try to display them as address
      inputValue.value = `Current Location (${latitude.toFixed(4)}, ${longitude.toFixed(4)})`
      
      trackEvent('reverse_geocode_error', {
        source: 'address_autocomplete',
        error: reverseGeocodeError instanceof Error ? reverseGeocodeError.message : 'Unknown error',
        coordinates: `${latitude}, ${longitude}`
      })
      
      // Create a minimal GeocodeResult for coordinates-only mode
      const geocodeResult: GeocodeResult = {
        formattedAddress: inputValue.value,
        latitude,
        longitude,
        street: '',
        city: '',
        region: '',
        postalCode: '',
        country: '',
        confidence: 0.8 // Lower confidence since we don't have address data
      }
      
      emit('location-selected', geocodeResult)
      emit('coordinates-updated', latitude, longitude)
    }
  } catch (error) {
    console.error('Error getting current location:', error)
    alert('Unable to get your current location. Please enter an address manually.')
    
    trackEvent('location_error', {
      source: 'address_autocomplete',
      error: error instanceof Error ? error.message : 'Unknown error'
    })
    console.error('Error getting location:', error)
    let message = 'Unable to get your location.'
    
    if (error instanceof GeolocationPositionError) {
      switch (error.code) {
        case error.PERMISSION_DENIED:
          message = 'Location access denied. Please allow location access and try again.'
          break
        case error.POSITION_UNAVAILABLE:
          message = 'Location information is unavailable.'
          break
        case error.TIMEOUT:
          message = 'Location request timed out. Please try again.'
          break
      }
    }
    
    alert(message)
  } finally {
    isGettingLocation.value = false
  }
}

// Analytics for address search
const handleSearchInput = () => {
  if (inputValue.value.length > 2) {
    trackEvent('address_search', {
      query_length: inputValue.value.length,
      component: 'address_autocomplete'
    })
  }
}

// Analytics for current location request
const handleLocationRequest = () => {
  trackEvent('location_request', {
    source: 'address_autocomplete',
    method: 'current_location'
  })
  getCurrentLocation()
}

// Cleanup
onUnmounted(() => {
  if (searchTimeout) {
    clearTimeout(searchTimeout)
  }
})
</script>

<style scoped>
.v-enter-active, .v-leave-active {
  transition: all 0.2s ease;
}

.v-enter-from {
  opacity: 0;
  transform: translateY(-10px);
}

.v-leave-to {
  opacity: 0;
  transform: translateY(-10px);
}
</style>
