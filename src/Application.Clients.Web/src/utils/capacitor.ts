import { Capacitor } from '@capacitor/core'
import { App } from '@capacitor/app'
import { Device } from '@capacitor/device'
import { SplashScreen } from '@capacitor/splash-screen'
import { StatusBar, Style } from '@capacitor/status-bar'
import { PushNotifications } from '@capacitor/push-notifications'
import { Geolocation } from '@capacitor/geolocation'
import { Camera, CameraResultType, CameraSource } from '@capacitor/camera'
import { Share } from '@capacitor/share'
import { Haptics, ImpactStyle } from '@capacitor/haptics'
import { Keyboard } from '@capacitor/keyboard'
import { Network } from '@capacitor/network'
import { Toast } from '@capacitor/toast'
import { Clipboard } from '@capacitor/clipboard'

export interface DeviceInfo {
  platform: string
  isNative: boolean
  isHybrid: boolean
  model?: string
  operatingSystem?: string
  osVersion?: string
  manufacturer?: string
  isVirtual?: boolean
  webViewVersion?: string
}

export class CapacitorService {
  private static instance: CapacitorService
  private deviceInfo: DeviceInfo | null = null

  private constructor() {}

  static getInstance(): CapacitorService {
    if (!CapacitorService.instance) {
      CapacitorService.instance = new CapacitorService()
    }
    return CapacitorService.instance
  }

  async initialize(): Promise<void> {
    if (!Capacitor.isNativePlatform()) {
      console.log('Running in web mode')
      return
    }

    console.log('Initializing native platform features...')

    try {
      // Get device information
      this.deviceInfo = await this.getDeviceInfo()
      console.log('Device info:', this.deviceInfo)

      // Setup status bar
      await this.setupStatusBar()

      // Setup app state handlers
      this.setupAppStateHandlers()

      // Setup push notifications (optional)
      await this.setupPushNotifications()

      // Setup keyboard listeners
      this.setupKeyboardListener()

      // Setup network monitoring
      this.setupNetworkListener((status) => {
        const event = new CustomEvent('network-status-change', { detail: status })
        window.dispatchEvent(event)
      })

      // Hide splash screen
      await SplashScreen.hide()

      console.log('Native platform initialization complete')
    } catch (error) {
      console.error('Error initializing native platform:', error)
    }
  }

  async getDeviceInfo(): Promise<DeviceInfo> {
    if (this.deviceInfo) {
      return this.deviceInfo
    }

    const platform = Capacitor.getPlatform()
    const isNative = Capacitor.isNativePlatform()
    const isHybrid = Capacitor.isPluginAvailable('Device')

    if (isHybrid) {
      try {
        const info = await Device.getInfo()
        this.deviceInfo = {
          platform,
          isNative,
          isHybrid,
          model: info.model,
          operatingSystem: info.operatingSystem,
          osVersion: info.osVersion,
          manufacturer: info.manufacturer,
          isVirtual: info.isVirtual,
          webViewVersion: info.webViewVersion
        }
      } catch (error) {
        console.error('Error getting device info:', error)
        this.deviceInfo = { platform, isNative, isHybrid }
      }
    } else {
      this.deviceInfo = { platform, isNative, isHybrid }
    }

    return this.deviceInfo
  }

  private async setupStatusBar(): Promise<void> {
    if (!Capacitor.isPluginAvailable('StatusBar')) {
      return
    }

    try {
      // Set status bar style based on theme
      await StatusBar.setStyle({ style: Style.Dark })
      
      // Set status bar background color
      await StatusBar.setBackgroundColor({ color: '#1f2937' })
      
      // Show status bar
      await StatusBar.show()
    } catch (error) {
      console.error('Error setting up status bar:', error)
    }
  }

  private setupAppStateHandlers(): void {
    if (!Capacitor.isPluginAvailable('App')) {
      return
    }

    // Handle app URL opening (deep links)
    App.addListener('appUrlOpen', (event) => {
      console.log('App opened with URL:', event.url)
      this.handleDeepLink(event.url)
    })

    // Handle app state changes
    App.addListener('appStateChange', (state) => {
      console.log('App state changed:', state)
      
      if (state.isActive) {
        // App came to foreground
        this.handleAppResume()
      } else {
        // App went to background
        this.handleAppPause()
      }
    })

    // Handle back button (Android)
    App.addListener('backButton', (event) => {
      console.log('Back button pressed')
      
      // You can handle navigation here
      // For example, navigate back or show exit confirmation
      if (window.location.pathname === '/') {
        // On home page, show exit confirmation
        if (confirm('Exit app?')) {
          App.exitApp()
        }
      } else {
        // Navigate back
        window.history.back()
      }
    })
  }

  private async setupPushNotifications(): Promise<void> {
    if (!Capacitor.isPluginAvailable('PushNotifications')) {
      return
    }

    try {
      // Request permission
      const permission = await PushNotifications.requestPermissions()
      
      if (permission.receive === 'granted') {
        // Register for push notifications
        await PushNotifications.register()

        // Handle registration
        PushNotifications.addListener('registration', (token) => {
          console.log('Push registration token:', token.value)
          // Send token to your server
          this.sendTokenToServer(token.value)
        })

        // Handle registration errors
        PushNotifications.addListener('registrationError', (error) => {
          console.error('Push registration error:', error)
        })

        // Handle received notifications
        PushNotifications.addListener('pushNotificationReceived', (notification) => {
          console.log('Push notification received:', notification)
          // Handle notification
        })

        // Handle notification tap
        PushNotifications.addListener('pushNotificationActionPerformed', (notification) => {
          console.log('Push notification action performed:', notification)
          // Navigate to specific screen based on notification
        })
      }
    } catch (error) {
      console.error('Error setting up push notifications:', error)
    }
  }

  private handleDeepLink(url: string): void {
    // Parse and handle deep links
    try {
      const urlObj = new URL(url)
      const path = urlObj.pathname
      
      // Navigate to the appropriate route
      // You'll need to integrate this with your Vue Router
      console.log('Handling deep link to path:', path)
      
      // Example: navigate to the path
      if (window.location.pathname !== path) {
        window.history.pushState({}, '', path)
        // Trigger route change in your Vue app
        window.dispatchEvent(new PopStateEvent('popstate'))
      }
    } catch (error) {
      console.error('Error handling deep link:', error)
    }
  }

  private handleAppResume(): void {
    // App resumed from background
    console.log('App resumed')
    
    // You can:
    // - Refresh data
    // - Check authentication status
    // - Update UI
    
    const event = new CustomEvent('app-resumed')
    window.dispatchEvent(event)
  }

  private handleAppPause(): void {
    // App went to background
    console.log('App paused')
    
    // You can:
    // - Save current state
    // - Pause timers
    // - Clean up resources
    
    const event = new CustomEvent('app-paused')
    window.dispatchEvent(event)
  }

  private async sendTokenToServer(token: string): Promise<void> {
    try {
      // Send push token to your API
      console.log('Sending push token to server:', token)
      
      // Example API call:
      // await apiService.registerPushToken(token)
    } catch (error) {
      console.error('Error sending token to server:', error)
    }
  }

  // Utility methods
  isNative(): boolean {
    return Capacitor.isNativePlatform()
  }

  getPlatform(): string {
    return Capacitor.getPlatform()
  }

  async share(options: { title: string; text: string; url?: string }): Promise<boolean> {
    if ('share' in navigator) {
      try {
        await navigator.share(options)
        return true
      } catch (error) {
        console.error('Error sharing:', error)
        return false
      }
    }
    return false
  }

  async openUrl(url: string): Promise<void> {
    if (Capacitor.isNativePlatform()) {
      // For native platforms, use the browser to open URLs
      window.open(url, '_system')
    } else {
      window.open(url, '_blank')
    }
  }

  // Geolocation methods
  async getCurrentPosition(): Promise<{ latitude: number; longitude: number; accuracy?: number } | null> {
    if (!Capacitor.isPluginAvailable('Geolocation')) {
      // Fallback to web API
      return new Promise((resolve, reject) => {
        if ('geolocation' in navigator) {
          navigator.geolocation.getCurrentPosition(
            (position) => {
              resolve({
                latitude: position.coords.latitude,
                longitude: position.coords.longitude,
                accuracy: position.coords.accuracy
              })
            },
            (error) => {
              console.error('Geolocation error:', error)
              resolve(null)
            }
          )
        } else {
          resolve(null)
        }
      })
    }

    try {
      const position = await Geolocation.getCurrentPosition()
      return {
        latitude: position.coords.latitude,
        longitude: position.coords.longitude,
        accuracy: position.coords.accuracy
      }
    } catch (error) {
      console.error('Error getting location:', error)
      return null
    }
  }

  async watchPosition(callback: (position: { latitude: number; longitude: number; accuracy?: number } | null) => void): Promise<string | null> {
    if (!Capacitor.isPluginAvailable('Geolocation')) {
      // Fallback to web API
      if ('geolocation' in navigator) {
        const watchId = navigator.geolocation.watchPosition(
          (position) => {
            callback({
              latitude: position.coords.latitude,
              longitude: position.coords.longitude,
              accuracy: position.coords.accuracy
            })
          },
          (error) => {
            console.error('Geolocation watch error:', error)
            callback(null)
          }
        )
        return watchId.toString()
      }
      return null
    }

    try {
      const watchId = await Geolocation.watchPosition(
        {
          enableHighAccuracy: true,
          timeout: 10000
        },
        (position, err) => {
          if (err) {
            console.error('Error watching location:', err)
            callback(null)
          } else if (position) {
            callback({
              latitude: position.coords.latitude,
              longitude: position.coords.longitude,
              accuracy: position.coords.accuracy
            })
          }
        }
      )
      return watchId
    } catch (error) {
      console.error('Error starting location watch:', error)
      return null
    }
  }

  async clearWatch(watchId: string): Promise<void> {
    if (!Capacitor.isPluginAvailable('Geolocation')) {
      if ('geolocation' in navigator) {
        navigator.geolocation.clearWatch(parseInt(watchId))
      }
      return
    }

    try {
      await Geolocation.clearWatch({ id: watchId })
    } catch (error) {
      console.error('Error clearing location watch:', error)
    }
  }

  // Camera methods
  async takePicture(): Promise<string | null> {
    if (!Capacitor.isPluginAvailable('Camera')) {
      return null
    }

    try {
      const image = await Camera.getPhoto({
        quality: 90,
        allowEditing: true,
        resultType: CameraResultType.DataUrl,
        source: CameraSource.Camera
      })

      return image.dataUrl || null
    } catch (error) {
      console.error('Error taking picture:', error)
      return null
    }
  }

  async selectPhoto(): Promise<string | null> {
    if (!Capacitor.isPluginAvailable('Camera')) {
      return null
    }

    try {
      const image = await Camera.getPhoto({
        quality: 90,
        allowEditing: true,
        resultType: CameraResultType.DataUrl,
        source: CameraSource.Photos
      })

      return image.dataUrl || null
    } catch (error) {
      console.error('Error selecting photo:', error)
      return null
    }
  }

  // Haptics methods
  async vibrate(style: 'light' | 'medium' | 'heavy' = 'medium'): Promise<void> {
    if (!Capacitor.isPluginAvailable('Haptics')) {
      // Fallback to web vibration API
      if ('vibrate' in navigator) {
        const duration = style === 'light' ? 50 : style === 'medium' ? 100 : 200
        navigator.vibrate(duration)
      }
      return
    }

    try {
      const impactStyle = style === 'light' ? ImpactStyle.Light : 
                         style === 'medium' ? ImpactStyle.Medium : 
                         ImpactStyle.Heavy

      await Haptics.impact({ style: impactStyle })
    } catch (error) {
      console.error('Error with haptic feedback:', error)
    }
  }

  // Network methods
  async getNetworkStatus(): Promise<{ connected: boolean; connectionType?: string }> {
    if (!Capacitor.isPluginAvailable('Network')) {
      return { connected: navigator.onLine }
    }

    try {
      const status = await Network.getStatus()
      return {
        connected: status.connected,
        connectionType: status.connectionType
      }
    } catch (error) {
      console.error('Error getting network status:', error)
      return { connected: navigator.onLine }
    }
  }

  setupNetworkListener(callback: (status: { connected: boolean; connectionType?: string }) => void): void {
    if (!Capacitor.isPluginAvailable('Network')) {
      // Fallback to web events
      const updateStatus = () => {
        callback({ connected: navigator.onLine })
      }
      window.addEventListener('online', updateStatus)
      window.addEventListener('offline', updateStatus)
      return
    }

    Network.addListener('networkStatusChange', (status) => {
      callback({
        connected: status.connected,
        connectionType: status.connectionType
      })
    })
  }

  // Toast notifications
  async showToast(message: string, duration: 'short' | 'long' = 'short'): Promise<void> {
    if (!Capacitor.isPluginAvailable('Toast')) {
      // Fallback to alert or custom toast
      console.log('Toast:', message)
      return
    }

    try {
      await Toast.show({
        text: message,
        duration: duration
      })
    } catch (error) {
      console.error('Error showing toast:', error)
    }
  }

  // Clipboard methods
  async copyToClipboard(text: string): Promise<boolean> {
    if (!Capacitor.isPluginAvailable('Clipboard')) {
      // Fallback to web clipboard API
      try {
        await navigator.clipboard.writeText(text)
        return true
      } catch (error) {
        console.error('Error copying to clipboard:', error)
        return false
      }
    }

    try {
      await Clipboard.write({ string: text })
      return true
    } catch (error) {
      console.error('Error copying to clipboard:', error)
      return false
    }
  }

  async readFromClipboard(): Promise<string | null> {
    if (!Capacitor.isPluginAvailable('Clipboard')) {
      // Fallback to web clipboard API
      try {
        return await navigator.clipboard.readText()
      } catch (error) {
        console.error('Error reading from clipboard:', error)
        return null
      }
    }

    try {
      const result = await Clipboard.read()
      return result.value || null
    } catch (error) {
      console.error('Error reading from clipboard:', error)
      return null
    }
  }

  // Keyboard methods
  setupKeyboardListener(): void {
    if (!Capacitor.isPluginAvailable('Keyboard')) {
      return
    }

    Keyboard.addListener('keyboardWillShow', (info) => {
      console.log('Keyboard will show with height:', info.keyboardHeight)
      const event = new CustomEvent('keyboard-will-show', { detail: info })
      window.dispatchEvent(event)
    })

    Keyboard.addListener('keyboardDidShow', (info) => {
      console.log('Keyboard did show with height:', info.keyboardHeight)
      const event = new CustomEvent('keyboard-did-show', { detail: info })
      window.dispatchEvent(event)
    })

    Keyboard.addListener('keyboardWillHide', () => {
      console.log('Keyboard will hide')
      const event = new CustomEvent('keyboard-will-hide')
      window.dispatchEvent(event)
    })

    Keyboard.addListener('keyboardDidHide', () => {
      console.log('Keyboard did hide')
      const event = new CustomEvent('keyboard-did-hide')
      window.dispatchEvent(event)
    })
  }

  async hideKeyboard(): Promise<void> {
    if (!Capacitor.isPluginAvailable('Keyboard')) {
      return
    }

    try {
      await Keyboard.hide()
    } catch (error) {
      console.error('Error hiding keyboard:', error)
    }
  }
}

export const capacitorService = CapacitorService.getInstance()
