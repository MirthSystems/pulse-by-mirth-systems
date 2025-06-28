import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { SpecialSummary, SpecialCategory, Special, VenueWithCategorizedSpecials } from '@/types/api'
import { apiService } from '@/services/api'

export const useSpecialStore = defineStore('special', () => {
  // State
  const specials = ref<SpecialSummary[]>([])
  const activeSpecials = ref<SpecialSummary[]>([])
  const categories = ref<SpecialCategory[]>([])
  const selectedSpecial = ref<Special | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)
  const searchResults = ref<SpecialSummary[]>([])
  const venuesWithSpecials = ref<VenueWithCategorizedSpecials[]>([])

  // Getters
  const specialsByCategory = computed(() => {
    const grouped: Record<string, SpecialSummary[]> = {}
    specials.value.forEach(special => {
      if (!grouped[special.categoryName]) {
        grouped[special.categoryName] = []
      }
      grouped[special.categoryName].push(special)
    })
    return grouped
  })

  const specialsByVenue = computed(() => {
    const grouped: Record<string, SpecialSummary[]> = {}
    specials.value.forEach(special => {
      if (!grouped[special.venueName]) {
        grouped[special.venueName] = []
      }
      grouped[special.venueName].push(special)
    })
    return grouped
  })

  const recurringSpecials = computed(() => 
    specials.value.filter(special => special.isRecurring)
  )

  const todaySpecials = computed(() => {
    const today = new Date().toISOString().split('T')[0]
    return specials.value.filter(special => 
      special.startDate <= today && 
      (!special.endDate || special.endDate >= today)
    )
  })

  // Actions
  async function fetchSpecials() {
    loading.value = true
    error.value = null
    try {
      const response = await apiService.getSpecials()
      if (response.success) {
        specials.value = response.data
      } else {
        error.value = response.message || 'Failed to fetch specials'
      }
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'An error occurred'
    } finally {
      loading.value = false
    }
  }

  async function fetchActiveSpecials() {
    loading.value = true
    error.value = null
    try {
      const response = await apiService.getActiveSpecials()
      if (response.success) {
        activeSpecials.value = response.data
      } else {
        error.value = response.message || 'Failed to fetch active specials'
      }
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'An error occurred'
    } finally {
      loading.value = false
    }
  }

  async function fetchSpecialCategories() {
    try {
      const response = await apiService.getSpecialCategories()
      if (response.success) {
        categories.value = response.data
      }
    } catch (err) {
      console.error('Failed to fetch special categories:', err)
    }
  }

  async function fetchSpecial(id: number) {
    loading.value = true
    error.value = null
    try {
      const response = await apiService.getSpecial(id)
      if (response.success) {
        selectedSpecial.value = response.data
      } else {
        error.value = response.message || 'Failed to fetch special'
      }
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'An error occurred'
    } finally {
      loading.value = false
    }
  }

  async function fetchSpecialsByCategory(categoryId: number) {
    loading.value = true
    try {
      const response = await apiService.getSpecialsByCategory(categoryId)
      if (response.success) {
        specials.value = response.data
      }
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'An error occurred'
    } finally {
      loading.value = false
    }
  }

  async function fetchSpecialsByVenue(venueId: number) {
    loading.value = true
    try {
      const response = await apiService.getSpecialsByVenue(venueId)
      if (response.success) {
        specials.value = response.data
      }
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'An error occurred'
    } finally {
      loading.value = false
    }
  }

  async function searchVenuesWithSpecials(search: any) {
    try {
      loading.value = true
      error.value = null
      
      const response = await apiService.searchVenuesWithSpecials(search)
      
      if (response.success && response.data) {
        // Store the venues with categorized specials
        venuesWithSpecials.value = response.data.items || []
      } else {
        venuesWithSpecials.value = []
      }
    } catch (err) {
      console.error('Failed to search venues with specials:', err)
      error.value = err instanceof Error ? err.message : 'Failed to search venues with specials'
      venuesWithSpecials.value = []
    } finally {
      loading.value = false
    }
  }

  async function searchSpecials(searchTerm: string, categoryId?: number) {
    loading.value = true
    try {
      const searchParams = {
        searchTerm,
        categoryId,
        pageNumber: 1,
        pageSize: 50
      }
      const response = await apiService.searchSpecials(searchParams)
      if (response.success) {
        searchResults.value = response.data.items
      }
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'An error occurred'
    } finally {
      loading.value = false
    }
  }

  async function searchSpecialsEnhanced(params: {
    searchTerm?: string
    latitude?: number
    longitude?: number
    radiusInMeters?: number
    categoryId?: number
    date?: string
    time?: string
  }) {
    loading.value = true
    try {
      const searchParams = {
        searchTerm: params.searchTerm,
        latitude: params.latitude,
        longitude: params.longitude,
        radiusInMeters: params.radiusInMeters,
        date: params.date,
        time: params.time,
        pageNumber: 1,
        pageSize: 50
      }
      // Use the venues search endpoint which returns enhanced results
      const response = await apiService.searchVenuesWithSpecials(searchParams)
      if (response.success && response.data.items) {
        // Extract all specials from all venues
        const allSpecials: any[] = []
        response.data.items.forEach(venueData => {
          // Add specials from each category
          allSpecials.push(...venueData.specials.food)
          allSpecials.push(...venueData.specials.drink)
          allSpecials.push(...venueData.specials.entertainment)
        })
        
        // Filter by category if specified
        if (params.categoryId) {
          searchResults.value = allSpecials.filter(special => 
            special.specialCategoryId === params.categoryId
          )
        } else {
          searchResults.value = allSpecials
        }
      }
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'An error occurred'
      console.error('Enhanced search error:', err)
    } finally {
      loading.value = false
    }
  }

  function clearSelectedSpecial() {
    selectedSpecial.value = null
  }

  function clearError() {
    error.value = null
  }

  return {
    // State
    specials,
    activeSpecials,
    categories,
    selectedSpecial,
    loading,
    error,
    searchResults,
    venuesWithSpecials,
    // Getters
    specialsByCategory,
    specialsByVenue,
    recurringSpecials,
    todaySpecials,
    // Actions
    fetchSpecials,
    fetchActiveSpecials,
    fetchSpecialCategories,
    fetchSpecial,
    fetchSpecialsByCategory,
    fetchSpecialsByVenue,
    searchSpecials,
    searchSpecialsEnhanced,
    searchVenuesWithSpecials,
    clearSelectedSpecial,
    clearError
  }
})
