import './assets/main.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'
import { createAuth0 } from '@auth0/auth0-vue'

import App from './App.vue'
import router from './router'

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

app.mount('#app')
