// Import type definitions for better TypeScript support

// PWA Install prompt
let deferredPrompt: any = null

export function setupPWA() {
  // Check if we're on iOS - iOS handles PWA differently
  const isIOS = isIOSDevice()
  console.log('PWA Setup - iOS detected:', isIOS)
  
  // Register service worker (but be more careful on iOS)
  if ('serviceWorker' in navigator && import.meta.env.PROD) {
    navigator.serviceWorker.register('/sw.js')
      .then((registration) => {
        console.log('SW registered: ', registration)
        
        // Check for updates (but don't auto-prompt on iOS)
        registration.addEventListener('updatefound', () => {
          const newWorker = registration.installing
          if (newWorker) {
            newWorker.addEventListener('statechange', () => {
              if (newWorker.state === 'installed' && navigator.serviceWorker.controller) {
                // New content available - only auto-notify on non-iOS
                if (!isIOS) {
                  const event = new CustomEvent('pwa-update-available')
                  window.dispatchEvent(event)
                }
              }
            })
          }
        })
      })
      .catch((registrationError) => {
        console.log('SW registration failed: ', registrationError)
      })
  }

  // Check for updates periodically (less aggressive on iOS)
  if ('serviceWorker' in navigator) {
    const checkInterval = isIOS ? 300000 : 60000 // 5 minutes on iOS, 1 minute elsewhere
    setInterval(async () => {
      const registration = await navigator.serviceWorker.getRegistration()
      if (registration) {
        registration.update()
      }
    }, checkInterval)
  }
}

export function setupInstallPrompt() {
  // Don't set up install prompts on iOS devices at all
  // iOS Safari handles PWA installation natively
  const isIOS = isIOSDevice()
  if (isIOS) {
    console.log('iOS detected - using native Safari PWA installation')
    return
  }

  // Check if user has already dismissed or installed
  const installDismissed = localStorage.getItem('pwa-install-dismissed')
  const installDismissedTime = installDismissed ? parseInt(installDismissed) : 0
  const oneWeekAgo = Date.now() - (7 * 24 * 60 * 60 * 1000)
  
  // Don't show prompts if already installed or recently dismissed
  if (isPWA() || (installDismissedTime > oneWeekAgo)) {
    console.log('PWA already installed or recently dismissed')
    return
  }

  window.addEventListener('beforeinstallprompt', (e) => {
    console.log('PWA install prompt available')
    // Prevent Chrome from automatically showing the prompt
    e.preventDefault()
    // Stash the event so it can be triggered later
    deferredPrompt = e
    
    // Delay showing to not overwhelm users immediately
    setTimeout(() => {
      showInstallButton()
    }, 10000) // Wait 10 seconds before showing (increased from 5)
  })

  window.addEventListener('appinstalled', () => {
    console.log('PWA was installed')
    hideInstallButton()
    deferredPrompt = null
    // Clear any dismissal flags since app is now installed
    localStorage.removeItem('pwa-install-dismissed')
  })
}

function showInstallButton() {
  // Don't show on iOS devices at all
  if (isIOSDevice()) {
    return
  }

  // Check session dismissal flag
  if (sessionStorage.getItem('pwa-dismiss-session') === 'true') {
    return
  }

  // Double-check we should show the prompt
  const installDismissed = localStorage.getItem('pwa-install-dismissed')
  const installDismissedTime = installDismissed ? parseInt(installDismissed) : 0
  const oneWeekAgo = Date.now() - (7 * 24 * 60 * 60 * 1000)
  
  if (isPWA() || (installDismissedTime > oneWeekAgo)) {
    return
  }

  // You can emit an event or use a state management solution
  // to show an install button in your UI
  const event = new CustomEvent('pwa-install-available')
  window.dispatchEvent(event)
}

function hideInstallButton() {
  const event = new CustomEvent('pwa-install-completed')
  window.dispatchEvent(event)
}

// Function to dismiss the install prompt
export function dismissInstallPrompt() {
  // Store dismissal timestamp
  localStorage.setItem('pwa-install-dismissed', Date.now().toString())
  hideInstallButton()
  
  // Also set a flag to prevent prompts for this session
  sessionStorage.setItem('pwa-dismiss-session', 'true')
}

export async function installPWA() {
  if (!deferredPrompt) {
    console.log('Install prompt not available')
    return false
  }

  // Show the prompt
  deferredPrompt.prompt()
  
  // Wait for the user to respond to the prompt
  const { outcome } = await deferredPrompt.userChoice
  console.log(`User response to the install prompt: ${outcome}`)
  
  // Clear the saved prompt since it can't be used again
  deferredPrompt = null
  
  return outcome === 'accepted'
}

// Utility functions
export function isIOSDevice(): boolean {
  return /iPad|iPhone|iPod/.test(navigator.userAgent) || 
         (navigator.platform === 'MacIntel' && navigator.maxTouchPoints > 1) // iPad on iOS 13+
}

export function isInStandaloneMode(): boolean {
  return window.matchMedia('(display-mode: standalone)').matches ||
         (window.navigator as any).standalone === true
}

// Detect if running as PWA
export function isPWA(): boolean {
  return isInStandaloneMode() || document.referrer.includes('android-app://')
}

// Network status detection
export function setupNetworkDetection() {
  const updateOnlineStatus = () => {
    const event = new CustomEvent('network-status-changed', {
      detail: { online: navigator.onLine }
    })
    window.dispatchEvent(event)
  }

  window.addEventListener('online', updateOnlineStatus)
  window.addEventListener('offline', updateOnlineStatus)
  
  // Initial status
  updateOnlineStatus()
}

// Debug utility - call this in browser console if needed
export function clearPWADismissalFlags() {
  localStorage.removeItem('pwa-install-dismissed')
  sessionStorage.removeItem('pwa-dismiss-session')
  console.log('PWA dismissal flags cleared')
}

// Add to window for debugging
if (typeof window !== 'undefined') {
  (window as any).clearPWADismissalFlags = clearPWADismissalFlags
}
