import Analytics from 'analytics'
import googleAnalytics from '@analytics/google-analytics'
import type { AnalyticsInstance } from 'analytics'

// Types for analytics events
export interface AnalyticsEvent {
  event: string
  properties?: Record<string, any>
}

export interface UserProperties {
  userId?: string
  email?: string
  name?: string
  plan?: string
  signUpDate?: string
  [key: string]: any
}

export interface PageViewProperties {
  title?: string
  path?: string
  url?: string
  referrer?: string
  search?: string
  [key: string]: any
}

// Analytics service class
class AnalyticsService {
  private analytics: AnalyticsInstance | null = null
  private isInitialized = false
  private debugMode = false

  constructor() {
    this.debugMode = import.meta.env.DEV || import.meta.env.VITE_ANALYTICS_DEBUG === 'true'
  }

  // Initialize analytics
  init() {
    if (this.isInitialized) {
      return this.analytics
    }

    try {
      this.analytics = Analytics({
        app: 'pulse-app',
        version: '1.0.0',
        debug: this.debugMode,
        plugins: [
          googleAnalytics({
            measurementIds: ['G-360H10VSKC'],
            // Enhanced Google Analytics settings
            gtagConfig: {
              // Custom dimensions and metrics
              custom_map: {
                user_type: 'custom_dimension_1',
                venue_category: 'custom_dimension_2',
                search_term: 'custom_dimension_3',
                special_category: 'custom_dimension_4'
              },
              // Enhanced ecommerce settings
              send_page_view: true,
              anonymize_ip: true,
              allow_ad_personalization_signals: false,
              // Cookie settings
              cookie_expires: 63072000, // 2 years
              cookie_update: true,
              cookie_flags: 'SameSite=None;Secure'
            }
          })
        ]
      })

      this.isInitialized = true
      
      if (this.debugMode) {
        console.log('Analytics initialized successfully')
      }

      return this.analytics
    } catch (error) {
      console.error('Failed to initialize analytics:', error)
      return null
    }
  }

  // Track page views
  page(properties?: PageViewProperties) {
    if (!this.analytics) return

    try {
      this.analytics.page(properties)
      
      if (this.debugMode) {
        console.log('Page view tracked:', properties)
      }
    } catch (error) {
      console.error('Failed to track page view:', error)
    }
  }

  // Track custom events
  track(eventName: string, properties?: Record<string, any>) {
    if (!this.analytics) return

    try {
      this.analytics.track(eventName, properties)
      
      if (this.debugMode) {
        console.log('Event tracked:', eventName, properties)
      }
    } catch (error) {
      console.error('Failed to track event:', error)
    }
  }

  // Identify users
  identify(userId: string, traits?: UserProperties) {
    if (!this.analytics) return

    try {
      this.analytics.identify(userId, traits)
      
      if (this.debugMode) {
        console.log('User identified:', userId, traits)
      }
    } catch (error) {
      console.error('Failed to identify user:', error)
    }
  }

  // Reset user (for logout)
  reset() {
    if (!this.analytics) return

    try {
      this.analytics.reset()
      
      if (this.debugMode) {
        console.log('Analytics reset')
      }
    } catch (error) {
      console.error('Failed to reset analytics:', error)
    }
  }

  // Predefined event tracking methods for common actions

  // Authentication events
  trackLogin(method: string, userId?: string) {
    this.track('login', {
      method,
      user_id: userId,
      timestamp: new Date().toISOString()
    })
  }

  trackLogout(userId?: string) {
    this.track('logout', {
      user_id: userId,
      timestamp: new Date().toISOString()
    })
  }

  trackSignUp(method: string, userId?: string) {
    this.track('sign_up', {
      method,
      user_id: userId,
      timestamp: new Date().toISOString()
    })
  }

  // Search events
  trackSearch(searchTerm: string, filters?: Record<string, any>, resultsCount?: number) {
    this.track('search', {
      search_term: searchTerm,
      search_filters: filters,
      results_count: resultsCount,
      timestamp: new Date().toISOString()
    })
  }

  trackSearchResults(searchTerm: string, resultsCount: number, filters?: Record<string, any>) {
    this.track('view_search_results', {
      search_term: searchTerm,
      results_count: resultsCount,
      search_filters: filters,
      timestamp: new Date().toISOString()
    })
  }

  // Content events
  trackViewItem(itemType: 'venue' | 'special', itemId: string, itemName?: string, category?: string) {
    this.track('view_item', {
      item_type: itemType,
      item_id: itemId,
      item_name: itemName,
      item_category: category,
      timestamp: new Date().toISOString()
    })
  }

  trackSelectItem(itemType: 'venue' | 'special', itemId: string, itemName?: string, position?: number) {
    this.track('select_item', {
      item_type: itemType,
      item_id: itemId,
      item_name: itemName,
      item_list_position: position,
      timestamp: new Date().toISOString()
    })
  }

  // Venue events
  trackVenueView(venueId: string, venueName: string, category?: string) {
    this.trackViewItem('venue', venueId, venueName, category)
  }

  trackVenueContact(venueId: string, contactMethod: 'phone' | 'website' | 'directions') {
    this.track('venue_contact', {
      venue_id: venueId,
      contact_method: contactMethod,
      timestamp: new Date().toISOString()
    })
  }

  trackVenueDirections(venueId: string, venueName: string) {
    this.track('get_directions', {
      venue_id: venueId,
      venue_name: venueName,
      timestamp: new Date().toISOString()
    })
  }

  // Special events
  trackSpecialView(specialId: string, specialTitle: string, venueId?: string, category?: string) {
    this.trackViewItem('special', specialId, specialTitle, category)
    
    if (venueId) {
      this.track('view_special_details', {
        special_id: specialId,
        special_title: specialTitle,
        venue_id: venueId,
        special_category: category,
        timestamp: new Date().toISOString()
      })
    }
  }

  trackSpecialInteraction(specialId: string, interactionType: 'share' | 'save' | 'claim') {
    this.track('special_interaction', {
      special_id: specialId,
      interaction_type: interactionType,
      timestamp: new Date().toISOString()
    })
  }

  // Navigation events
  trackNavigation(from: string, to: string, method: 'click' | 'search' | 'direct') {
    this.track('navigate', {
      from_page: from,
      to_page: to,
      navigation_method: method,
      timestamp: new Date().toISOString()
    })
  }

  // Form events
  trackFormStart(formName: string, formId?: string) {
    this.track('form_start', {
      form_name: formName,
      form_id: formId,
      timestamp: new Date().toISOString()
    })
  }

  trackFormSubmit(formName: string, success: boolean, errorMessage?: string) {
    this.track('form_submit', {
      form_name: formName,
      success,
      error_message: errorMessage,
      timestamp: new Date().toISOString()
    })
  }

  trackFormFieldFocus(formName: string, fieldName: string) {
    this.track('form_field_focus', {
      form_name: formName,
      field_name: fieldName,
      timestamp: new Date().toISOString()
    })
  }

  // Business/Admin events
  trackVenueManagement(action: 'create' | 'edit' | 'delete', venueId?: string) {
    this.track('venue_management', {
      action,
      venue_id: venueId,
      timestamp: new Date().toISOString()
    })
  }

  trackSpecialManagement(action: 'create' | 'edit' | 'delete', specialId?: string, venueId?: string) {
    this.track('special_management', {
      action,
      special_id: specialId,
      venue_id: venueId,
      timestamp: new Date().toISOString()
    })
  }

  trackUserPermissionChange(targetUserId: string, permission: string, action: 'granted' | 'revoked', venueId?: string) {
    this.track('user_permission_change', {
      target_user_id: targetUserId,
      permission,
      action,
      venue_id: venueId,
      timestamp: new Date().toISOString()
    })
  }

  // Error tracking
  trackError(errorType: string, errorMessage: string, context?: Record<string, any>) {
    this.track('error', {
      error_type: errorType,
      error_message: errorMessage,
      error_context: context,
      timestamp: new Date().toISOString()
    })
  }

  // Performance tracking
  trackPerformance(metricName: string, value: number, unit: string = 'ms') {
    this.track('performance', {
      metric_name: metricName,
      metric_value: value,
      metric_unit: unit,
      timestamp: new Date().toISOString()
    })
  }

  // Feature usage
  trackFeatureUsage(featureName: string, action: string, context?: Record<string, any>) {
    this.track('feature_usage', {
      feature_name: featureName,
      feature_action: action,
      feature_context: context,
      timestamp: new Date().toISOString()
    })
  }

  // PWA events
  trackPWAInstall() {
    this.track('pwa_install', {
      timestamp: new Date().toISOString()
    })
  }

  trackPWAPromptShown() {
    this.track('pwa_prompt_shown', {
      timestamp: new Date().toISOString()
    })
  }

  trackPWAPromptDismissed() {
    this.track('pwa_prompt_dismissed', {
      timestamp: new Date().toISOString()
    })
  }

  // E-commerce style events for specials engagement
  trackSpecialEngagement(specialId: string, engagementType: 'view' | 'click' | 'share' | 'save', value?: number) {
    this.track('special_engagement', {
      special_id: specialId,
      engagement_type: engagementType,
      engagement_value: value,
      currency: 'USD', // If dealing with monetary values
      timestamp: new Date().toISOString()
    })
  }

  // Location events
  trackLocationPermission(granted: boolean) {
    this.track('location_permission', {
      permission_granted: granted,
      timestamp: new Date().toISOString()
    })
  }

  trackLocationSearch(latitude: number, longitude: number, radius: number) {
    this.track('location_search', {
      latitude,
      longitude,
      search_radius: radius,
      timestamp: new Date().toISOString()
    })
  }
}

// Create and export singleton instance
export const analyticsService = new AnalyticsService()
export default analyticsService
