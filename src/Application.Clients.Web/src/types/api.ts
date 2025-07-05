// API Response types
export interface ApiResponse<T> {
  success: boolean;
  message?: string;
  data: T;
  errors?: string[] | null;
}

export interface PagedResponse<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

// Venue types
export interface VenueCategory {
  id: number;
  name: string;
  description: string;
  icon: string;
  sortOrder: number;
}

export interface VenueSummary {
  id: number;
  categoryId: number;
  categoryName: string;
  categoryIcon: string;
  activeSpecialsCount: number;
  distanceInMeters?: number;
  name: string;
  description: string;
  phoneNumber: string;
  website: string;
  email: string;
  profileImage?: string;
  streetAddress: string;
  secondaryAddress?: string;
  locality: string;
  region: string;
  postalCode: string;
  country: string;
  latitude: number;
  longitude: number;
  isActive: boolean;
}

export interface Venue extends VenueSummary {
  category: VenueCategory;
  businessHours: BusinessHours[];
  specials: Special[];
}

export interface BusinessHours {
  id: number;
  venueId: number;
  dayOfWeekId: number;
  dayOfWeekName: string;
  openTime: string;
  closeTime: string;
  isClosed: boolean;
}

// Special types
export interface SpecialCategory {
  id: number;
  name: string;
  description: string;
  icon: string;
  sortOrder: number;
}

export interface SpecialSummary {
  id: number;
  venueId: number;
  venueName: string;
  specialCategoryId: number;
  categoryName: string;
  categoryIcon: string;
  distanceInMeters?: number;
  title: string;
  description: string;
  startDate: string;
  startTime: string;
  endTime?: string;
  endDate?: string;
  isRecurring: boolean;
  cronSchedule?: string;
  isActive: boolean;
}

export interface Special extends SpecialSummary {
  venue: VenueSummary;
  category: SpecialCategory;
}

// Search types
export interface VenueSearch {
  searchTerm?: string;
  categoryId?: number;
  latitude?: number;
  longitude?: number;
  radiusInMeters?: number;
  activeOnly?: boolean;
  pageNumber: number;
  pageSize: number;
}

export interface SpecialSearch {
  searchTerm?: string;
  categoryId?: number;
  venueId?: number;
  startDate?: string;
  endDate?: string;
  activeOnly?: boolean;
  pageNumber: number;
  pageSize: number;
}

export interface EnhancedSpecialSearch {
  searchTerm?: string;
  address?: string;
  latitude?: number;
  longitude?: number;
  radiusInMeters?: number;
  date?: string;
  time?: string;
  activeOnly?: boolean;
  currentlyRunning?: boolean;
  pageNumber: number;
  pageSize: number;
  sortBy?: string;
  sortOrder?: string;
}

// Azure Maps types
export interface PointOfInterest {
  name: string;
  category: string;
  latitude: number;
  longitude: number;
  address: string;
  phone: string;
  website: string;
  distanceInMeters: number;
  score: number;
}

export interface GeocodeResult {
  latitude: number;
  longitude: number;
  formattedAddress: string;
  street: string;
  city: string;
  region: string;
  postalCode: string;
  country: string;
  confidence: number;
}

export interface ReverseGeocodeResult {
  formattedAddress: string;
  street: string;
  city: string;
  region: string;
  postalCode: string;
  country: string;
  neighborhood: string;
}

export interface TimeZoneInfo {
  id: string;
  hasIanaId: boolean;
  displayName: string;
  standardName: string;
  daylightName: string;
  baseUtcOffset: string;
  supportsDaylightSavingTime: boolean;
}

export interface EnhancedVenueSearchResult {
  databaseResults: PagedResponse<VenueSummary>;
  azureMapsPOIs: PointOfInterest[];
  totalDatabaseResults: number;
  totalAzureMapsPOIs: number;
}

export interface VenueWithCategorizedSpecials {
  id: number;
  name: string;
  description?: string;
  phoneNumber?: string;
  website?: string;
  email?: string;
  profileImage?: string;
  streetAddress: string;
  secondaryAddress?: string;
  locality: string;
  region: string;
  postalCode: string;
  country: string;
  latitude?: number;
  longitude?: number;
  categoryId: number;
  categoryName: string;
  categoryIcon?: string;
  distanceInMeters?: number;
  specials: {
    food: SpecialSummary[];
    drink: SpecialSummary[];
    entertainment: SpecialSummary[];
  };
  totalSpecialCount: number;
}

// Management types for create/update operations
export interface BusinessHoursRequest {
  dayOfWeekId: number;
  openTime?: string;
  closeTime?: string;
  isClosed: boolean;
}

export interface CreateVenueRequest {
  categoryId: number;
  name: string;
  description?: string;
  phoneNumber?: string;
  website?: string;
  email?: string;
  profileImage?: string;
  streetAddress: string;
  secondaryAddress?: string;
  locality: string;
  region: string;
  postalCode: string;
  country: string;
  latitude?: number;
  longitude?: number;
  isActive: boolean;
  businessHours?: BusinessHoursRequest[];
}

export interface UpdateVenueRequest {
  categoryId: number;
  name: string;
  description?: string;
  phoneNumber?: string;
  website?: string;
  email?: string;
  profileImage?: string;
  streetAddress: string;
  secondaryAddress?: string;
  locality: string;
  region: string;
  postalCode: string;
  country: string;
  latitude?: number;
  longitude?: number;
  isActive: boolean;
  businessHours?: BusinessHoursRequest[];
}

export interface CreateSpecialRequest {
  venueId: number;
  specialCategoryId: number;
  title: string;
  description: string;
  startDate: string;
  startTime: string;
  endTime?: string;
  endDate?: string;
  isRecurring: boolean;
  cronSchedule?: string;
  isActive: boolean;
}

export interface UpdateSpecialRequest {
  venueId: number;
  specialCategoryId: number;
  title: string;
  description: string;
  startDate: string;
  startTime: string;
  endTime?: string;
  endDate?: string;
  isRecurring: boolean;
  cronSchedule?: string;
  isActive: boolean;
}

// User and Permission types
export interface User {
  id: number;
  sub: string;
  email: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
  lastLoginAt?: string;
}

export interface UserVenuePermission {
  id: number;
  userId: number;
  venueId: number;
  venueName: string;
  name: string; // "venue:owner" | "venue:manager" | "venue:staff" - matches backend response
  userEmail: string;
  grantedByUserId: number;
  grantedByUserEmail: string;
  grantedAt: string;
  isActive: boolean;
  notes?: string;
}

export interface VenueInvitation {
  id: number;
  email: string;
  venueId: number;
  venueName: string;
  name: string; // "venue:owner" | "venue:manager" | "venue:staff"
  invitedByUserId: number;
  invitedByUserEmail: string;
  invitedAt: string;
  expiresAt: string;
  acceptedAt?: string;
  acceptedByUserId?: number;
  isActive: boolean;
  notes?: string;
}

export interface VenueInvitationResponse {
  id: number;
  email: string;
  venueId: number;
  venueName: string;
  name: string; // "venue:owner" | "venue:manager" | "venue:staff"
  invitedByUserId: number;
  invitedByUserEmail: string;
  invitedAt: string;
  expiresAt: string;
  acceptedAt?: string;
  acceptedByUserId?: number;
  isActive: boolean;
  notes?: string;
}

export interface CreateInvitationRequest {
  email: string;
  venueId: number;
  permission: string; // "venue:owner" | "venue:manager" | "venue:staff"
  notes?: string;
  senderEmail: string; // Add the sender's email to the request
}

export interface UpdatePermissionRequest {
  permission: string; // "venue:owner" | "venue:manager" | "venue:staff" - changed from 'name' to match backend
  notes?: string;
}

export interface PermissionTypeResponse {
  name: string;
  displayName: string;
  description: string;
}
