<template>
  <div class="space-y-4">
    <!-- Street Address with Autocomplete -->
    <div>
      <label for="streetAddress" class="block text-sm font-medium text-gray-700">Street Address *</label>
      <div class="relative">
        <input
          v-model="localForm.streetAddress"
          @input="onStreetAddressChange"
          @focus="showAddressSuggestions = true"
          @blur="onBlur"
          type="text"
          id="streetAddress"
          :readonly="readonly"
          required
          autocomplete="off"
          class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
          :class="{ 'bg-gray-50': readonly, 'border-red-300': errors.streetAddress }"
          placeholder="Start typing an address..."
        />
        
        <!-- Address Suggestions Dropdown -->
        <div 
          v-if="showAddressSuggestions && addressSuggestions.length > 0 && !readonly"
          class="absolute z-50 w-full mt-1 bg-white border border-gray-300 rounded-md shadow-lg max-h-60 overflow-auto"
        >        <div
          v-for="(suggestion, index) in addressSuggestions"
          :key="index"
          @click="selectAddressSuggestion(suggestion)"
          class="p-3 hover:bg-gray-50 cursor-pointer border-b border-gray-100 last:border-b-0"
        >
          <div class="text-sm font-medium text-gray-900">
            {{ suggestion.formattedAddress }}
          </div>
          <div v-if="suggestion.city || suggestion.region || suggestion.postalCode" class="text-xs text-gray-500 mt-1">
            <span v-if="suggestion.city">{{ suggestion.city }}</span><span v-if="suggestion.city && (suggestion.region || suggestion.postalCode)">, </span><span v-if="suggestion.region">{{ suggestion.region }}</span><span v-if="suggestion.region && suggestion.postalCode"> </span><span v-if="suggestion.postalCode">{{ suggestion.postalCode }}</span>
          </div>
        </div>
        </div>
      </div>
      <p v-if="errors.streetAddress" class="mt-1 text-sm text-red-600">{{ errors.streetAddress }}</p>
    </div>

    <!-- City and State/Region -->
    <div class="grid grid-cols-2 gap-4">
      <div>
        <label for="locality" class="block text-sm font-medium text-gray-700">City *</label>
        <input
          v-model="localForm.locality"
          @input="onAddressFieldChange"
          type="text"
          id="locality"
          :readonly="readonly"
          required
          class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
          :class="{ 'bg-gray-50': readonly, 'border-red-300': errors.locality }"
        />
        <p v-if="errors.locality" class="mt-1 text-sm text-red-600">{{ errors.locality }}</p>
      </div>

      <div>
        <label for="region" class="block text-sm font-medium text-gray-700">State *</label>
        <input
          v-model="localForm.region"
          @input="onAddressFieldChange"
          type="text"
          id="region"
          :readonly="readonly"
          required
          placeholder="e.g., Pennsylvania, PA"
          class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
          :class="{ 'bg-gray-50': readonly, 'border-red-300': errors.region }"
        />
        <p v-if="errors.region" class="mt-1 text-sm text-red-600">{{ errors.region }}</p>
      </div>
    </div>

    <!-- Postal Code and Country -->
    <div class="grid grid-cols-2 gap-4">
      <div>
        <label for="postalCode" class="block text-sm font-medium text-gray-700">Postal Code *</label>
        <input
          v-model="localForm.postalCode"
          @input="onAddressFieldChange"
          type="text"
          id="postalCode"
          :readonly="readonly"
          required
          class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
          :class="{ 'bg-gray-50': readonly, 'border-red-300': errors.postalCode }"
        />
        <p v-if="errors.postalCode" class="mt-1 text-sm text-red-600">{{ errors.postalCode }}</p>
      </div>

      <div>
        <label for="country" class="block text-sm font-medium text-gray-700">Country *</label>
        <input
          v-model="localForm.country"
          @input="onAddressFieldChange"
          type="text"
          id="country"
          :readonly="readonly"
          required
          placeholder="United States"
          class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
          :class="{ 'bg-gray-50': readonly, 'border-red-300': errors.country }"
        />
        <p v-if="errors.country" class="mt-1 text-sm text-red-600">{{ errors.country }}</p>
      </div>
    </div>

    <!-- Geocode Button and Status -->
    <div v-if="!readonly" class="flex items-center space-x-3">
      <button
        type="button"
        @click="geocodeAddress"
        :disabled="isGeocoding || !canGeocode"
        class="inline-flex items-center px-3 py-2 border border-gray-300 shadow-sm text-sm leading-4 font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-50 disabled:cursor-not-allowed"
      >
        <div v-if="isGeocoding" class="animate-spin rounded-full h-4 w-4 border-b-2 border-gray-600 mr-2"></div>
        <MapPinIcon v-else class="h-4 w-4 mr-2" />
        {{ isGeocoding ? 'Geocoding...' : 'Verify Address' }}
      </button>
      
      <div v-if="geocodeStatus" class="flex items-center text-sm">
        <CheckCircleIcon v-if="geocodeStatus === 'success'" class="h-4 w-4 text-green-500 mr-1" />
        <ExclamationTriangleIcon v-else-if="geocodeStatus === 'error'" class="h-4 w-4 text-red-500 mr-1" />
        <span :class="{
          'text-green-600': geocodeStatus === 'success',
          'text-red-600': geocodeStatus === 'error'
        }">
          {{ geocodeMessage }}
        </span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted, onUnmounted } from 'vue'
import { MapPinIcon, CheckCircleIcon, ExclamationTriangleIcon } from '@heroicons/vue/24/outline'
import apiService from '@/services/api'
import type { GeocodeResult } from '@/types/api'

interface AddressForm {
  streetAddress: string
  locality: string
  region: string
  postalCode: string
  country: string
}

interface Props {
  modelValue: AddressForm
  readonly?: boolean
  errors?: Record<string, string>
}

interface Emits {
  'update:modelValue': [value: AddressForm]
  'address-geocoded': [result: GeocodeResult]
}

const props = withDefaults(defineProps<Props>(), {
  readonly: false,
  errors: () => ({})
})

const emit = defineEmits<Emits>()

// Local form state
const localForm = ref<AddressForm>({ ...props.modelValue })

// Address suggestions state
const addressSuggestions = ref<GeocodeResult[]>([])
const showAddressSuggestions = ref(false)
const isLoadingSuggestions = ref(false)
const searchTimeout = ref<number | null>(null)

// Geocoding state
const isGeocoding = ref(false)
const geocodeStatus = ref<'success' | 'error' | null>(null)
const geocodeMessage = ref('')

// Watch for external model changes
watch(() => props.modelValue, (newValue) => {
  localForm.value = { ...newValue }
}, { deep: true })

// Watch for local changes and emit
watch(localForm, (newValue) => {
  emit('update:modelValue', { ...newValue })
}, { deep: true })

// Computed
const canGeocode = computed(() => {
  return localForm.value.streetAddress?.trim() && 
         localForm.value.locality?.trim() && 
         localForm.value.region?.trim() && 
         localForm.value.postalCode?.trim() && 
         localForm.value.country?.trim()
})

// Methods
const onStreetAddressChange = () => {
  onAddressFieldChange()
  searchAddresses()
}

const onAddressFieldChange = () => {
  // Reset geocode status when address changes
  geocodeStatus.value = null
  geocodeMessage.value = ''
}

const searchAddresses = async () => {
  const query = localForm.value.streetAddress?.trim()
  if (!query || query.length < 3) {
    addressSuggestions.value = []
    showAddressSuggestions.value = false
    return
  }

  // Clear existing timeout
  if (searchTimeout.value) {
    clearTimeout(searchTimeout.value)
  }

  // Set new timeout
  searchTimeout.value = setTimeout(async () => {
    isLoadingSuggestions.value = true
    try {
      // Enhance query with US context if no state/country info is present
      let searchQuery = query
      if (!query.toLowerCase().includes('usa') && !query.toLowerCase().includes('united states')) {
        searchQuery = `${query}, USA`
      }
      
      const response = await apiService.searchAddresses(searchQuery)
      if (response.success && response.data) {
        // Filter for US addresses if possible
        const filteredSuggestions = response.data.filter(suggestion => 
          !suggestion.country || 
          suggestion.country.toLowerCase().includes('united states') || 
          suggestion.country.toLowerCase() === 'us' ||
          suggestion.country.toLowerCase() === 'usa'
        )
        
        addressSuggestions.value = filteredSuggestions.length > 0 ? filteredSuggestions : response.data
        showAddressSuggestions.value = addressSuggestions.value.length > 0
      }
    } catch (error) {
      console.error('Error searching addresses:', error)
      addressSuggestions.value = []
      showAddressSuggestions.value = false
    } finally {
      isLoadingSuggestions.value = false
    }
  }, 300)
}

const selectAddressSuggestion = (suggestion: GeocodeResult) => {
  // Parse the formatted address to extract missing components
  const parsedAddress = parseFormattedAddress(suggestion.formattedAddress)
  
  // Fill in all address fields from the suggestion, with fallbacks from parsed address
  localForm.value.streetAddress = suggestion.street || parsedAddress.street || extractStreetFromFormatted(suggestion.formattedAddress)
  localForm.value.locality = suggestion.city || parsedAddress.city || ''
  localForm.value.region = suggestion.region || parsedAddress.region || ''
  localForm.value.postalCode = suggestion.postalCode || parsedAddress.postalCode || ''
  localForm.value.country = suggestion.country || 'United States'
  
  // Clear suggestions and hide dropdown
  addressSuggestions.value = []
  showAddressSuggestions.value = false
  
  // Emit the geocoded result
  emit('address-geocoded', suggestion)
  
  // Set success status
  geocodeStatus.value = 'success'
  geocodeMessage.value = 'Address verified and auto-filled'
}

// Helper function to extract street address from formatted address
const extractStreetFromFormatted = (formattedAddress: string): string => {
  // Try to extract just the street part (first part before comma)
  const parts = formattedAddress.split(',')
  return parts[0]?.trim() || formattedAddress
}

// Helper function to parse formatted address into components
const parseFormattedAddress = (formattedAddress: string) => {
  const parts = formattedAddress.split(',').map(part => part.trim())
  
  // Common format: "Street, City, State ZIP, Country"
  // Example: "215 W 4th St, Williamsport, PA 17701, United States"
  
  let street = ''
  let city = ''
  let region = ''
  let postalCode = ''
  
  if (parts.length >= 3) {
    street = parts[0] || ''
    city = parts[1] || ''
    
    // Parse "State ZIP" from the third part
    const stateZipPart = parts[2] || ''
    const stateZipMatch = stateZipPart.match(/^([A-Z]{2})\s+(\d{5}(?:-\d{4})?)$/)
    
    if (stateZipMatch) {
      region = stateZipMatch[1] // State abbreviation
      postalCode = stateZipMatch[2] // ZIP code
    } else {
      // Try to extract state and ZIP in other formats
      const words = stateZipPart.split(/\s+/)
      if (words.length >= 2) {
        // Last word might be ZIP code
        const lastWord = words[words.length - 1]
        if (/^\d{5}(?:-\d{4})?$/.test(lastWord)) {
          postalCode = lastWord
          region = words.slice(0, -1).join(' ')
        } else {
          region = stateZipPart
        }
      } else {
        region = stateZipPart
      }
    }
  } else if (parts.length === 2) {
    street = parts[0] || ''
    // Try to parse city and state from second part
    const cityStatePart = parts[1] || ''
    const cityStateMatch = cityStatePart.match(/^(.+?),?\s+([A-Z]{2})(?:\s+(\d{5}(?:-\d{4})?))?$/)
    
    if (cityStateMatch) {
      city = cityStateMatch[1]
      region = cityStateMatch[2]
      postalCode = cityStateMatch[3] || ''
    } else {
      city = cityStatePart
    }
  } else if (parts.length === 1) {
    street = parts[0] || ''
  }
  
  return { street, city, region, postalCode }
}

const onBlur = () => {
  // Delay hiding suggestions to allow for clicks
  setTimeout(() => {
    showAddressSuggestions.value = false
  }, 200)
}

const geocodeAddress = async () => {
  if (!canGeocode.value) return

  isGeocoding.value = true
  geocodeStatus.value = null
  geocodeMessage.value = ''

  try {
    const addressString = [
      localForm.value.streetAddress,
      localForm.value.locality,
      localForm.value.region,
      localForm.value.postalCode,
      localForm.value.country
    ].filter(Boolean).join(', ')

    const response = await apiService.geocodeAddress(addressString)
    
    if (response.success && response.data) {
      emit('address-geocoded', response.data)
      geocodeStatus.value = 'success'
      geocodeMessage.value = 'Address verified successfully'
    } else {
      geocodeStatus.value = 'error'
      geocodeMessage.value = 'Unable to verify address. Please check the details.'
    }
  } catch (error) {
    console.error('Error geocoding address:', error)
    geocodeStatus.value = 'error'
    geocodeMessage.value = 'Error verifying address. Please try again.'
  } finally {
    isGeocoding.value = false
  }
}

// Cleanup
onMounted(() => {
  // Clear any stale suggestions on mount
  addressSuggestions.value = []
  showAddressSuggestions.value = false
})

onUnmounted(() => {
  if (searchTimeout.value) {
    clearTimeout(searchTimeout.value)
  }
})
</script>

<style scoped>
/* Ensure no unwanted content appears */
.relative {
  isolation: isolate;
}

/* Hide any potential browser autocomplete that might be showing */
input[autocomplete] {
  -webkit-text-security: none;
}

/* Ensure dropdown positioning is correct */
.absolute.z-50 {
  z-index: 9999 !important;
}
</style>
