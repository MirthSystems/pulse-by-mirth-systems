declare module '@analytics/google-analytics' {
  interface GoogleAnalyticsConfig {
    measurementIds: string[]
    gtagConfig?: {
      custom_map?: Record<string, string>
      send_page_view?: boolean
      anonymize_ip?: boolean
      allow_ad_personalization_signals?: boolean
      cookie_expires?: number
      cookie_update?: boolean
      cookie_flags?: string
      [key: string]: any
    }
    [key: string]: any
  }

  function googleAnalytics(config: GoogleAnalyticsConfig): any
  export default googleAnalytics
}

declare module 'analytics' {
  export interface AnalyticsInstance {
    page(properties?: Record<string, any>): void
    track(event: string, properties?: Record<string, any>): void
    identify(userId: string, traits?: Record<string, any>): void
    reset(): void
    [key: string]: any
  }

  export interface AnalyticsConfig {
    app: string
    version?: string
    debug?: boolean
    plugins: any[]
    [key: string]: any
  }

  function Analytics(config: AnalyticsConfig): AnalyticsInstance
  export default Analytics
}
