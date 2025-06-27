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
  latitude?: number;
  longitude?: number;
  radiusInMeters?: number;
  date?: string;
  time?: string;
  activeOnly?: boolean;
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
  streetNumber: string;
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
  venue: VenueSummary;
  food: SpecialSummary[];
  drink: SpecialSummary[];
  entertainment: SpecialSummary[];
  totalSpecials: number;
}
