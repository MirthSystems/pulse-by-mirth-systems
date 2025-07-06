import { onMounted, onUnmounted, nextTick, getCurrentInstance } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import analyticsService from '@/services/analytics'

/**
 * Vue composable for analytics tracking
 * Provides reactive analytics methods and automatic page tracking
 */
export function useAnalytics() {
  // Try to get router context, but make it optional to avoid injection warnings
  let route: any = null
  let router: any = null
  
  try {
    const instance = getCurrentInstance()
    if (instance) {
      route = useRoute()
      router = useRouter()
    }
  } catch (error) {
    // Router context not available - this is fine for non-router contexts
    console.debug('Router context not available in useAnalytics - page tracking will use fallbacks')
  }
  
  // Page tracking
  const trackPageView = (customProperties?: Record<string, any>) => {
    const properties = {
      title: document.title || (route?.meta?.title || route?.name) as string || 'Unknown Page',
      path: route?.path || window.location.pathname,
      url: window.location.href,
      referrer: document.referrer,
      search: route?.query && Object.keys(route.query).length > 0 ? JSON.stringify(route.query) : undefined,
      ...customProperties
    }
    
    analyticsService.page(properties)
  }

  // Auto track page views on route changes
  const setupPageTracking = () => {
    // Track initial page
    nextTick(() => {
      trackPageView()
    })
    
    // Track route changes - only if router is available
    if (router) {
      const unwatch = router.afterEach((to: any, from: any) => {
        nextTick(() => {
          trackPageView()
          
          // Track navigation
          analyticsService.trackNavigation(
            from.path,
            to.path,
            'click'
          )
        })
      })
      
      return unwatch
    }
    
    // Return a no-op function if router not available
    return () => {}
  }

  // Event tracking methods
  const trackEvent = (eventName: string, properties?: Record<string, any>) => {
    analyticsService.track(eventName, {
      page_path: route?.path || window.location.pathname,
      page_title: document.title,
      ...properties
    })
  }

  const trackClick = (element: string, properties?: Record<string, any>) => {
    trackEvent('click', {
      element_name: element,
      ...properties
    })
  }

  const trackForm = (action: 'start' | 'submit' | 'field_focus', formName: string, properties?: Record<string, any>) => {
    switch (action) {
      case 'start':
        analyticsService.trackFormStart(formName)
        break
      case 'submit':
        analyticsService.trackFormSubmit(formName, properties?.success ?? true, properties?.errorMessage)
        break
      case 'field_focus':
        analyticsService.trackFormFieldFocus(formName, properties?.fieldName || '')
        break
    }
  }

  const trackSearch = (searchTerm: string, filters?: Record<string, any>, resultsCount?: number) => {
    analyticsService.trackSearch(searchTerm, filters, resultsCount)
  }

  const trackEngagement = (type: string, target: string, properties?: Record<string, any>) => {
    trackEvent('engagement', {
      engagement_type: type,
      engagement_target: target,
      ...properties
    })
  }

  // Content tracking
  const trackContentView = (contentType: string, contentId: string, contentName?: string) => {
    analyticsService.trackViewItem(contentType as any, contentId, contentName)
  }

  const trackContentInteraction = (contentType: string, contentId: string, interactionType: string) => {
    trackEvent('content_interaction', {
      content_type: contentType,
      content_id: contentId,
      interaction_type: interactionType
    })
  }

  // Error tracking
  const trackError = (errorType: string, errorMessage: string, context?: Record<string, any>) => {
    analyticsService.trackError(errorType, errorMessage, {
      page_path: route?.path || window.location.pathname,
      page_title: document.title,
      ...context
    })
  }

  // Performance tracking
  const trackPerformanceMetric = (metricName: string, value: number, unit: string = 'ms') => {
    analyticsService.trackPerformance(metricName, value, unit)
  }

  // User identification
  const identifyUser = (userId: string, traits?: Record<string, any>) => {
    analyticsService.identify(userId, traits)
  }

  const resetUser = () => {
    analyticsService.reset()
  }

  // Auto-tracking setup for component lifecycle
  const setupAutoTracking = (options: {
    trackPageViews?: boolean
    trackErrors?: boolean
    trackPerformance?: boolean
  } = {}) => {
    const {
      trackPageViews = true,
      trackErrors = true,
      trackPerformance = false
    } = options

    let unwatch: (() => void) | undefined
    let errorHandler: ((event: ErrorEvent) => void) | undefined
    let performanceObserver: PerformanceObserver | undefined

    onMounted(() => {
      // Set up page tracking
      if (trackPageViews) {
        unwatch = setupPageTracking()
      }

      // Set up error tracking
      if (trackErrors) {
        errorHandler = (event: ErrorEvent) => {
          trackError('javascript_error', event.message, {
            filename: event.filename,
            lineno: event.lineno,
            colno: event.colno,
            stack: event.error?.stack
          })
        }
        window.addEventListener('error', errorHandler)

        // Also track unhandled promise rejections
        const rejectionHandler = (event: PromiseRejectionEvent) => {
          trackError('unhandled_promise_rejection', event.reason?.toString() || 'Unknown promise rejection')
        }
        window.addEventListener('unhandledrejection', rejectionHandler)
      }

      // Set up performance tracking
      if (trackPerformance && 'PerformanceObserver' in window) {
        try {
          performanceObserver = new PerformanceObserver((list) => {
            list.getEntries().forEach((entry) => {
              if (entry.entryType === 'navigation') {
                trackPerformanceMetric('page_load_time', entry.duration)
              } else if (entry.entryType === 'paint') {
                trackPerformanceMetric(entry.name.replace('-', '_'), entry.startTime)
              }
            })
          })
          
          performanceObserver.observe({ entryTypes: ['navigation', 'paint'] })
        } catch (error) {
          console.warn('Performance tracking not supported:', error)
        }
      }
    })

    onUnmounted(() => {
      // Clean up
      if (unwatch) {
        unwatch()
      }
      
      if (errorHandler) {
        window.removeEventListener('error', errorHandler)
      }
      
      if (performanceObserver) {
        performanceObserver.disconnect()
      }
    })
  }

  return {
    // Core tracking
    trackPageView,
    trackEvent,
    trackClick,
    trackForm,
    trackSearch,
    trackEngagement,
    
    // Content tracking
    trackContentView,
    trackContentInteraction,
    
    // Error and performance
    trackError,
    trackPerformance: trackPerformanceMetric,
    
    // User management
    identifyUser,
    resetUser,
    
    // Auto-tracking setup
    setupAutoTracking,
    setupPageTracking,
    
    // Direct access to analytics service
    analytics: analyticsService
  }
}

/**
 * Analytics directive for tracking clicks on DOM elements
 * Usage: v-track-click="'button_name'" or v-track-click="{ event: 'custom_event', properties: { key: 'value' } }"
 */
export const vTrackClick = {
  mounted(el: HTMLElement, binding: any) {
    const handleClick = () => {
      if (typeof binding.value === 'string') {
        analyticsService.track('click', {
          element_name: binding.value,
          element_tag: el.tagName.toLowerCase(),
          element_text: el.textContent?.trim() || '',
          timestamp: new Date().toISOString()
        })
      } else if (typeof binding.value === 'object') {
        const { event = 'click', properties = {} } = binding.value
        analyticsService.track(event, {
          element_tag: el.tagName.toLowerCase(),
          element_text: el.textContent?.trim() || '',
          timestamp: new Date().toISOString(),
          ...properties
        })
      }
    }
    
    el.addEventListener('click', handleClick)
    el._analyticsHandler = handleClick
  },
  
  unmounted(el: HTMLElement) {
    if (el._analyticsHandler) {
      el.removeEventListener('click', el._analyticsHandler)
      delete el._analyticsHandler
    }
  }
}

// Extend HTMLElement interface for TypeScript
declare global {
  interface HTMLElement {
    _analyticsHandler?: () => void
  }
}
