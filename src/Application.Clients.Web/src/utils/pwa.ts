// Import type definitions for better TypeScript support

export function setupPWA() {
  // Register service worker
  if ('serviceWorker' in navigator && import.meta.env.PROD) {
    navigator.serviceWorker.register('/sw.js')
      .then((registration) => {
        console.log('SW registered: ', registration)
        
        // Check for updates
        registration.addEventListener('updatefound', () => {
          const newWorker = registration.installing
          if (newWorker) {
            newWorker.addEventListener('statechange', () => {
              if (newWorker.state === 'installed' && navigator.serviceWorker.controller) {
                // New content available, please refresh!
                if (confirm('New version available! Click OK to refresh.')) {
                  window.location.reload()
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

  // Check for updates periodically
  if ('serviceWorker' in navigator) {
    setInterval(async () => {
      const registration = await navigator.serviceWorker.getRegistration()
      if (registration) {
        registration.update()
      }
    }, 60000) // Check every minute
  }
}

// PWA Install prompt
let deferredPrompt: any = null

export function setupInstallPrompt() {
  window.addEventListener('beforeinstallprompt', (e) => {
    // Prevent Chrome 67 and earlier from automatically showing the prompt
    e.preventDefault()
    // Stash the event so it can be triggered later
    deferredPrompt = e
    
    // Show your custom install button
    showInstallButton()
  })

  window.addEventListener('appinstalled', () => {
    console.log('PWA was installed')
    hideInstallButton()
    deferredPrompt = null
  })
}

function showInstallButton() {
  // You can emit an event or use a state management solution
  // to show an install button in your UI
  const event = new CustomEvent('pwa-install-available')
  window.dispatchEvent(event)
}

function hideInstallButton() {
  const event = new CustomEvent('pwa-install-completed')
  window.dispatchEvent(event)
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

// Detect if running as PWA
export function isPWA(): boolean {
  return window.matchMedia('(display-mode: standalone)').matches ||
         (window.navigator as any).standalone === true ||
         document.referrer.includes('android-app://')
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
