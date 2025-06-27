import type {
  ApiResponse,
  PagedResponse,
  VenueCategory,
  VenueSummary,
  Venue,
  SpecialCategory,
  SpecialSummary,
  Special,
  VenueSearch,
  SpecialSearch,
  EnhancedSpecialSearch,
  PointOfInterest,
  GeocodeResult,
  ReverseGeocodeResult,
  TimeZoneInfo,
  EnhancedVenueSearchResult,
  VenueWithCategorizedSpecials
} from '@/types/api'

// In development, use relative URLs with Vite proxy
// In production, use the environment variable
const API_BASE_URL = import.meta.env.DEV 
  ? '/api' 
  : (import.meta.env.VITE_API_BASE_URL || 'https://localhost:7309/api')

class ApiService {
  private async request<T>(url: string, options?: RequestInit): Promise<ApiResponse<T>> {
    const response = await fetch(`${API_BASE_URL}${url}`, {
      ...options,
      headers: {
        'Content-Type': 'application/json',
        ...options?.headers,
      },
    })

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`)
    }

    return response.json()
  }

  // Venue API methods
  async getVenues(): Promise<ApiResponse<VenueSummary[]>> {
    return this.request<VenueSummary[]>('/venues')
  }

  async getVenue(id: number): Promise<ApiResponse<Venue>> {
    return this.request<Venue>(`/venues/${id}`)
  }

  async getVenueCategories(): Promise<ApiResponse<VenueCategory[]>> {
    return this.request<VenueCategory[]>('/venues/categories')
  }

  async getActiveVenues(): Promise<ApiResponse<VenueSummary[]>> {
    return this.request<VenueSummary[]>('/venues/active')
  }

  async getVenuesByCategory(categoryId: number): Promise<ApiResponse<VenueSummary[]>> {
    return this.request<VenueSummary[]>(`/venues/category/${categoryId}`)
  }

  async getVenuesWithSpecials(): Promise<ApiResponse<VenueSummary[]>> {
    return this.request<VenueSummary[]>('/venues/with-specials')
  }

  async getVenuesNear(
    latitude: number,
    longitude: number,
    radiusInMeters: number = 5000
  ): Promise<ApiResponse<VenueSummary[]>> {
    return this.request<VenueSummary[]>(
      `/venues/near?latitude=${latitude}&longitude=${longitude}&radiusInMeters=${radiusInMeters}`
    )
  }

  async searchVenues(search: VenueSearch): Promise<ApiResponse<PagedResponse<VenueSummary>>> {
    return this.request<PagedResponse<VenueSummary>>('/venues/search', {
      method: 'POST',
      body: JSON.stringify(search),
    })
  }

  async searchVenuesEnhanced(search: VenueSearch): Promise<ApiResponse<EnhancedVenueSearchResult>> {
    return this.request<EnhancedVenueSearchResult>('/venues/search/enhanced', {
      method: 'POST',
      body: JSON.stringify(search),
    })
  }

  // Azure Maps venue methods
  async geocodeVenue(id: number): Promise<ApiResponse<GeocodeResult>> {
    return this.request<GeocodeResult>(`/venues/${id}/geocode`, { method: 'POST' })
  }

  async getVenueLocationDetails(id: number): Promise<ApiResponse<ReverseGeocodeResult>> {
    return this.request<ReverseGeocodeResult>(`/venues/${id}/location-details`)
  }

  async getVenueTimeZone(id: number): Promise<ApiResponse<TimeZoneInfo>> {
    return this.request<TimeZoneInfo>(`/venues/${id}/timezone`)
  }

  async getNearbyPOIs(
    id: number,
    category: string = 'restaurant',
    radiusInMeters: number = 5000
  ): Promise<ApiResponse<PointOfInterest[]>> {
    return this.request<PointOfInterest[]>(
      `/venues/${id}/nearby-pois?category=${category}&radiusInMeters=${radiusInMeters}`
    )
  }

  // Special API methods
  async getSpecials(): Promise<ApiResponse<SpecialSummary[]>> {
    return this.request<SpecialSummary[]>('/specials')
  }

  async getSpecial(id: number): Promise<ApiResponse<Special>> {
    return this.request<Special>(`/specials/${id}`)
  }

  async getSpecialCategories(): Promise<ApiResponse<SpecialCategory[]>> {
    return this.request<SpecialCategory[]>('/specials/categories')
  }

  async getActiveSpecials(): Promise<ApiResponse<SpecialSummary[]>> {
    return this.request<SpecialSummary[]>('/specials/active')
  }

  async getSpecialsByCategory(categoryId: number): Promise<ApiResponse<SpecialSummary[]>> {
    return this.request<SpecialSummary[]>(`/specials/category/${categoryId}`)
  }

  async getSpecialsByVenue(venueId: number): Promise<ApiResponse<SpecialSummary[]>> {
    return this.request<SpecialSummary[]>(`/specials/venue/${venueId}`)
  }

  async searchSpecials(search: SpecialSearch): Promise<ApiResponse<PagedResponse<SpecialSummary>>> {
    return this.request<PagedResponse<SpecialSummary>>('/specials/search', {
      method: 'POST',
      body: JSON.stringify(search),
    })
  }

  async searchVenuesWithSpecials(
    search: EnhancedSpecialSearch
  ): Promise<ApiResponse<PagedResponse<VenueWithCategorizedSpecials>>> {
    return this.request<PagedResponse<VenueWithCategorizedSpecials>>('/specials/search/venues', {
      method: 'POST',
      body: JSON.stringify(search),
    })
  }

  // Utility methods
  async getWeatherForecast(): Promise<any> {
    return this.request<any>('/weatherforecast')
  }
}

export const apiService = new ApiService()
export default apiService
