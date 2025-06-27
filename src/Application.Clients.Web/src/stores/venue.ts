import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { VenueSummary, VenueCategory, Venue } from '@/types/api'
import { apiService } from '@/services/api'

export const useVenueStore = defineStore('venue', () => {
  // State
  const venues = ref<VenueSummary[]>([])
  const categories = ref<VenueCategory[]>([])
  const selectedVenue = ref<Venue | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)
  const searchResults = ref<VenueSummary[]>([])
  const userLocation = ref<{ latitude: number; longitude: number } | null>(null)

  // Getters
  const venuesWithSpecials = computed(() => 
    venues.value.filter(venue => venue.activeSpecialsCount > 0)
  )

  const nearbyVenues = computed(() => {
    if (!userLocation.value) return []
    return venues.value
      .filter(venue => venue.distanceInMeters !== undefined)
      .sort((a, b) => (a.distanceInMeters || 0) - (b.distanceInMeters || 0))
  })

  const venuesByCategory = computed(() => {
    const grouped: Record<string, VenueSummary[]> = {}
    venues.value.forEach(venue => {
      if (!grouped[venue.categoryName]) {
        grouped[venue.categoryName] = []
      }
      grouped[venue.categoryName].push(venue)
    })
    return grouped
  })

  // Actions
  async function fetchVenues() {
    loading.value = true
    error.value = null
    try {
      const response = await apiService.getVenues()
      if (response.success) {
        venues.value = response.data
      } else {
        error.value = response.message || 'Failed to fetch venues'
      }
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'An error occurred'
    } finally {
      loading.value = false
    }
  }

  async function fetchVenueCategories() {
    try {
      const response = await apiService.getVenueCategories()
      if (response.success) {
        categories.value = response.data
      }
    } catch (err) {
      console.error('Failed to fetch venue categories:', err)
    }
  }

  async function fetchVenue(id: number) {
    loading.value = true
    error.value = null
    try {
      const response = await apiService.getVenue(id)
      if (response.success) {
        selectedVenue.value = response.data
      } else {
        error.value = response.message || 'Failed to fetch venue'
      }
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'An error occurred'
    } finally {
      loading.value = false
    }
  }

  async function fetchVenuesWithSpecials() {
    loading.value = true
    try {
      const response = await apiService.getVenuesWithSpecials()
      if (response.success) {
        venues.value = response.data
      }
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'An error occurred'
    } finally {
      loading.value = false
    }
  }

  async function fetchNearbyVenues(latitude: number, longitude: number, radius: number = 5000) {
    loading.value = true
    try {
      const response = await apiService.getVenuesNear(latitude, longitude, radius)
      if (response.success) {
        venues.value = response.data
        userLocation.value = { latitude, longitude }
      }
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'An error occurred'
    } finally {
      loading.value = false
    }
  }

  async function searchVenues(searchTerm: string, categoryId?: number) {
    loading.value = true
    try {
      const searchParams = {
        searchTerm,
        categoryId,
        pageNumber: 1,
        pageSize: 50
      }
      const response = await apiService.searchVenues(searchParams)
      if (response.success) {
        searchResults.value = response.data.items
      }
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'An error occurred'
    } finally {
      loading.value = false
    }
  }

  function setUserLocation(latitude: number, longitude: number) {
    userLocation.value = { latitude, longitude }
  }

  function clearSelectedVenue() {
    selectedVenue.value = null
  }

  function clearError() {
    error.value = null
  }

  return {
    // State
    venues,
    categories,
    selectedVenue,
    loading,
    error,
    searchResults,
    userLocation,
    // Getters
    venuesWithSpecials,
    nearbyVenues,
    venuesByCategory,
    // Actions
    fetchVenues,
    fetchVenueCategories,
    fetchVenue,
    fetchVenuesWithSpecials,
    fetchNearbyVenues,
    searchVenues,
    setUserLocation,
    clearSelectedVenue,
    clearError
  }
})
