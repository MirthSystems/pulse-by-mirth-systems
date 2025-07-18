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
  VenueWithCategorizedSpecials,
  CreateVenueRequest,
  UpdateVenueRequest,
  CreateSpecialRequest,
  UpdateSpecialRequest,
  User,
  UserVenuePermission,
  VenueInvitation,
  VenueInvitationResponse,
  CreateInvitationRequest,
  UpdatePermissionRequest,
  PermissionTypeResponse
} from '@/types/api'

// Get API base URL from environment variable set by Aspire
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || ''

class ApiService {
  private accessToken: string | null = null

  setAccessToken(token: string | null) {
    this.accessToken = token
  }

  hasAccessToken(): boolean {
    return !!this.accessToken
  }

  private async getAuthHeaders(): Promise<Headers> {
    const headers = new Headers({
      'Content-Type': 'application/json',
    })

    if (this.accessToken) {
      headers.set('Authorization', `Bearer ${this.accessToken}`)
    }

    return headers
  }

  private async request<T>(url: string, options?: RequestInit): Promise<ApiResponse<T>> {
    const headers = await this.getAuthHeaders()
    
    // Debug token usage
    console.log('Making API request to:', url, 'hasToken:', !!this.accessToken, 'tokenStart:', this.accessToken?.substring(0, 20) + '...')
    
    // Merge custom headers with auth headers
    if (options?.headers) {
      const customHeaders = new Headers(options.headers)
      customHeaders.forEach((value, key) => {
        headers.set(key, value)
      })
    }

    const response = await fetch(`${API_BASE_URL}${url}`, {
      ...options,
      headers,
    })

    if (!response.ok) {
      console.error('API request failed:', response.status, response.statusText, 'URL:', url)
      const errorData = await response.json().catch(() => ({ 
        message: `HTTP ${response.status}: ${response.statusText}` 
      }))
      throw new Error(errorData.message || `HTTP ${response.status}`)
    }

    return response.json()
  }

  // Venue API methods
  async getVenues(): Promise<ApiResponse<VenueSummary[]>> {
    return this.request<VenueSummary[]>('/api/venues')
  }

  async getVenue(id: number): Promise<ApiResponse<Venue>> {
    return this.request<Venue>(`/api/venues/${id}`)
  }

  async getVenueCategories(): Promise<ApiResponse<VenueCategory[]>> {
    return this.request<VenueCategory[]>('/api/venues/categories')
  }

  async getActiveVenues(): Promise<ApiResponse<VenueSummary[]>> {
    return this.request<VenueSummary[]>('/api/venues/active')
  }

  async getVenuesByCategory(categoryId: number): Promise<ApiResponse<VenueSummary[]>> {
    return this.request<VenueSummary[]>(`/api/venues/category/${categoryId}`)
  }

  async getVenuesWithSpecials(): Promise<ApiResponse<VenueSummary[]>> {
    return this.request<VenueSummary[]>('/api/venues/with-specials')
  }

  async getVenuesNear(
    latitude: number,
    longitude: number,
    radiusInMeters: number = 5000
  ): Promise<ApiResponse<VenueSummary[]>> {
    return this.request<VenueSummary[]>(
      `/api/venues/near?latitude=${latitude}&longitude=${longitude}&radiusInMeters=${radiusInMeters}`
    )
  }

  async searchVenues(search: VenueSearch): Promise<ApiResponse<PagedResponse<VenueSummary>>> {
    return this.request<PagedResponse<VenueSummary>>('/api/venues/search', {
      method: 'POST',
      body: JSON.stringify(search),
    })
  }

  async searchVenuesEnhanced(search: VenueSearch): Promise<ApiResponse<EnhancedVenueSearchResult>> {
    return this.request<EnhancedVenueSearchResult>('/api/venues/search/enhanced', {
      method: 'POST',
      body: JSON.stringify(search),
    })
  }

  // Azure Maps venue methods
  async geocodeVenue(id: number): Promise<ApiResponse<GeocodeResult>> {
    return this.request<GeocodeResult>(`/api/venues/${id}/geocode`, { method: 'POST' })
  }

  async getVenueLocationDetails(id: number): Promise<ApiResponse<ReverseGeocodeResult>> {
    return this.request<ReverseGeocodeResult>(`/api/venues/${id}/location-details`)
  }

  async getVenueTimeZone(id: number): Promise<ApiResponse<TimeZoneInfo>> {
    return this.request<TimeZoneInfo>(`/api/venues/${id}/timezone`)
  }

  async getNearbyPOIs(
    id: number,
    category: string = 'restaurant',
    radiusInMeters: number = 5000
  ): Promise<ApiResponse<PointOfInterest[]>> {
    return this.request<PointOfInterest[]>(
      `/api/venues/${id}/nearby-pois?category=${category}&radiusInMeters=${radiusInMeters}`
    )
  }

  // Management API methods - All management operations
  
  // Get venues user has access to manage
  async getMyVenues(): Promise<ApiResponse<VenueSummary[]>> {
    return this.request<VenueSummary[]>('/api/venues/my')
  }

  // Get specials user has access to manage  
  async getMySpecials(): Promise<ApiResponse<SpecialSummary[]>> {
    return this.request<SpecialSummary[]>('/api/specials/my')
  }

  // Venue Management
  async createVenue(venue: CreateVenueRequest): Promise<ApiResponse<Venue>> {
    return this.request<Venue>('/api/venues', {
      method: 'POST',
      body: JSON.stringify(venue),
    })
  }

  async updateVenue(id: number, venue: UpdateVenueRequest): Promise<ApiResponse<Venue>> {
    return this.request<Venue>(`/api/venues/${id}`, {
      method: 'PUT',
      body: JSON.stringify(venue),
    })
  }

  async deleteVenue(id: number): Promise<ApiResponse<boolean>> {
    return this.request<boolean>(`/api/venues/${id}`, {
      method: 'DELETE',
    })
  }

  // Special Management
  async createSpecial(venueId: number, special: CreateSpecialRequest): Promise<ApiResponse<Special>> {
    return this.request<Special>('/api/specials', {
      method: 'POST',
      body: JSON.stringify({ ...special, venueId }),
    })
  }

  async updateSpecial(id: number, special: UpdateSpecialRequest): Promise<ApiResponse<Special>> {
    return this.request<Special>(`/api/specials/${id}`, {
      method: 'PUT',
      body: JSON.stringify(special),
    })
  }

  async deleteSpecial(id: number): Promise<ApiResponse<boolean>> {
    return this.request<boolean>(`/api/specials/${id}`, {
      method: 'DELETE',
    })
  }

  // Permission Management
  async getMyPermissions(): Promise<ApiResponse<UserVenuePermission[]>> {
    return this.request<UserVenuePermission[]>('/api/permissions/my')
  }

  async getUserPermissions(userId: number): Promise<ApiResponse<UserVenuePermission[]>> {
    return this.request<UserVenuePermission[]>(`/api/permissions/users/${userId}`)
  }

  async getVenuePermissions(venueId: number): Promise<ApiResponse<UserVenuePermission[]>> {
    return this.request<UserVenuePermission[]>(`/api/venues/${venueId}/permissions`)
  }

  async updateUserPermission(
    permissionId: number, 
    permission: UpdatePermissionRequest
  ): Promise<ApiResponse<UserVenuePermission>> {
    return this.request<UserVenuePermission>(`/api/permissions/${permissionId}`, {
      method: 'PUT',
      body: JSON.stringify(permission),
    })
  }

  async revokeUserPermission(permissionId: number): Promise<ApiResponse<boolean>> {
    return this.request<boolean>(`/api/permissions/${permissionId}`, {
      method: 'DELETE',
    })
  }

  // Invitation Management
  async sendInvitation(invitation: CreateInvitationRequest): Promise<ApiResponse<VenueInvitationResponse>> {
    return this.request<VenueInvitationResponse>('/api/invitations', {
      method: 'POST',
      body: JSON.stringify(invitation),
    })
  }

  // User management
  async syncUser(userInfo: any): Promise<ApiResponse<any>> {
    return this.request<any>('/api/users/sync', {
      method: 'POST',
      body: JSON.stringify(userInfo),
    })
  }

  async getVenueInvitations(venueId: number): Promise<ApiResponse<VenueInvitationResponse[]>> {
    return this.request<VenueInvitationResponse[]>(`/api/venues/${venueId}/invitations`)
  }

  async getMyInvitations(email?: string): Promise<ApiResponse<VenueInvitationResponse[]>> {
    const params = email ? `?email=${encodeURIComponent(email)}` : ''
    return this.request<VenueInvitationResponse[]>(`/api/invitations/my${params}`)
  }

  async acceptInvitation(invitationId: number): Promise<ApiResponse<boolean>> {
    return this.request<boolean>(`/api/invitations/${invitationId}/accept`, {
      method: 'POST',
    })
  }

  async declineInvitation(invitationId: number): Promise<ApiResponse<boolean>> {
    return this.request<boolean>(`/api/invitations/${invitationId}/decline`, {
      method: 'POST',
    })
  }

  async cancelInvitation(invitationId: number): Promise<ApiResponse<boolean>> {
    return this.request<boolean>(`/api/invitations/${invitationId}`, {
      method: 'DELETE',
    })
  }

  // Special API methods
  async getSpecials(): Promise<ApiResponse<SpecialSummary[]>> {
    return this.request<SpecialSummary[]>('/api/specials')
  }

  async getSpecial(id: number): Promise<ApiResponse<Special>> {
    return this.request<Special>(`/api/specials/${id}`)
  }

  async getSpecialCategories(): Promise<ApiResponse<SpecialCategory[]>> {
    return this.request<SpecialCategory[]>('/api/specials/categories')
  }

  async getActiveSpecials(): Promise<ApiResponse<SpecialSummary[]>> {
    return this.request<SpecialSummary[]>('/api/specials/active')
  }

  async getSpecialsByCategory(categoryId: number): Promise<ApiResponse<SpecialSummary[]>> {
    return this.request<SpecialSummary[]>(`/api/specials/category/${categoryId}`)
  }

  async getSpecialsByVenue(venueId: number): Promise<ApiResponse<SpecialSummary[]>> {
    return this.request<SpecialSummary[]>(`/api/specials/venue/${venueId}`)
  }

  async searchSpecials(search: SpecialSearch): Promise<ApiResponse<PagedResponse<SpecialSummary>>> {
    return this.request<PagedResponse<SpecialSummary>>('/api/specials/search', {
      method: 'POST',
      body: JSON.stringify(search),
    })
  }

  async searchVenuesWithSpecials(
    search: EnhancedSpecialSearch
  ): Promise<ApiResponse<PagedResponse<VenueWithCategorizedSpecials>>> {
    return this.request<PagedResponse<VenueWithCategorizedSpecials>>('/api/specials/search/venues', {
      method: 'POST',
      body: JSON.stringify(search),
    })
  }

  // Location API methods
  async searchAddresses(query: string): Promise<ApiResponse<GeocodeResult[]>> {
    const params = new URLSearchParams({ query })
    return this.request<GeocodeResult[]>(`/api/location/search?${params}`)
  }

  async geocodeAddress(address: string): Promise<ApiResponse<GeocodeResult | null>> {
    return this.request<GeocodeResult | null>('/api/location/geocode', {
      method: 'POST',
      body: JSON.stringify({ address }),
    })
  }

  async reverseGeocode(latitude: number, longitude: number): Promise<ApiResponse<ReverseGeocodeResult | null>> {
    const params = new URLSearchParams({
      latitude: latitude.toString(),
      longitude: longitude.toString()
    })
    return this.request<ReverseGeocodeResult | null>(`/api/location/reverse-geocode?${params}`, {
      method: 'POST',
    })
  }

  async getTimeZone(latitude: number, longitude: number): Promise<ApiResponse<TimeZoneInfo | null>> {
    const params = new URLSearchParams({
      latitude: latitude.toString(),
      longitude: longitude.toString()
    })
    return this.request<TimeZoneInfo | null>(`/api/location/timezone?${params}`)
  }

  async getPermissionTypes(): Promise<ApiResponse<PermissionTypeResponse[]>> {
    return this.request<PermissionTypeResponse[]>('/api/permissions/types')
  }
}

export const apiService = new ApiService()
export default apiService
