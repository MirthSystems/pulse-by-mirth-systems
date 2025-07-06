import './assets/main.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'
import { createAuth0 } from '@auth0/auth0-vue'

import App from './App.vue'
import router from './router'
import { setupPWA, setupInstallPrompt, setupNetworkDetection } from './utils/pwa'
import { capacitorService } from './utils/capacitor'

const app = createApp(App)

// Auth0 configuration
const auth0Config = {
  domain: import.meta.env.VITE_AUTH0_DOMAIN || '',
  clientId: import.meta.env.VITE_AUTH0_CLIENT_ID || '',
  authorizationParams: {
    redirect_uri: `${window.location.origin}/callback`,
    audience: import.meta.env.VITE_AUTH0_AUDIENCE || '',
    scope: 'openid profile email offline_access'
  },
  useRefreshTokens: true,
  cacheLocation: 'localstorage' as const
}

app.use(createPinia())
app.use(createAuth0(auth0Config))
app.use(router)

// Initialize PWA features
async function initializeApp() {
  try {
    // Setup PWA features
    setupPWA()
    setupInstallPrompt()
    setupNetworkDetection()
    
    // Initialize native platform features
    await capacitorService.initialize()
    
    console.log('App initialization complete')
  } catch (error) {
    console.error('Error during app initialization:', error)
  }
}

// Mount app and initialize features
app.mount('#app')
initializeApp()
