import { fileURLToPath, URL } from 'node:url'

import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import vueDevTools from 'vite-plugin-vue-devtools'
import { VitePWA } from 'vite-plugin-pwa'

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    vue(),
    vueDevTools(),
    VitePWA({
      registerType: 'autoUpdate',
      workbox: {
        globPatterns: ['**/*.{js,css,html,ico,png,svg,json,vue,ts,jsx,tsx}'],
        runtimeCaching: [
          {
            urlPattern: ({ request }) => request.destination === 'document',
            handler: 'NetworkFirst',
            options: {
              cacheName: 'pages',
              expiration: {
                maxEntries: 32,
                maxAgeSeconds: 24 * 60 * 60, // 1 day
              },
            },
          },
          {
            urlPattern: ({ request }) => request.destination === 'script' || request.destination === 'style',
            handler: 'StaleWhileRevalidate',
            options: {
              cacheName: 'assets',
              expiration: {
                maxEntries: 64,
                maxAgeSeconds: 30 * 24 * 60 * 60, // 30 days
              },
            },
          },
          {
            urlPattern: ({ url }) => url.pathname.startsWith('/api/'),
            handler: 'NetworkFirst',
            options: {
              cacheName: 'api-cache',
              expiration: {
                maxEntries: 64,
                maxAgeSeconds: 5 * 60, // 5 minutes
              },
              networkTimeoutSeconds: 10,
            },
          },
        ],
      },
      includeAssets: ['favicon.ico', 'pwa-64x64.png', 'pwa-192x192.png', 'pwa-512x512.png', 'maskable-icon-512x512.png'],
      manifest: {
        name: 'Pulse - Venue & Specials Manager',
        short_name: 'Pulse',
        description: 'Discover and manage venue specials and events near you',
        theme_color: '#1f2937',
        background_color: '#ffffff',
        display: 'standalone',
        orientation: 'portrait-primary',
        scope: '/',
        start_url: '/?utm_source=pwa',
        id: '/?utm_source=pwa',
        lang: 'en-US',
        dir: 'ltr',
        icons: [
          {
            src: 'pwa-64x64.png',
            sizes: '64x64',
            type: 'image/png'
          },
          {
            src: 'pwa-192x192.png',
            sizes: '192x192',
            type: 'image/png'
          },
          {
            src: 'pwa-512x512.png',
            sizes: '512x512',
            type: 'image/png',
            purpose: 'any'
          },
          {
            src: 'maskable-icon-512x512.png',
            sizes: '512x512',
            type: 'image/png',
            purpose: 'maskable'
          }
        ],
        categories: ['lifestyle', 'food', 'entertainment', 'business'],
        screenshots: [
          {
            src: '/screenshots/mobile-screenshot-1.png',
            sizes: '640x1136',
            type: 'image/png',
            form_factor: 'narrow'
          },
          {
            src: '/screenshots/desktop-screenshot-1.png',
            sizes: '1280x800',
            type: 'image/png',
            form_factor: 'wide'
          }
        ],
        shortcuts: [
          {
            name: 'Find Specials',
            short_name: 'Specials',
            description: 'Browse current venue specials near you',
            url: '/specials?utm_source=pwa_shortcut',
            icons: [{ src: 'pwa-192x192.png', sizes: '192x192' }]
          },
          {
            name: 'Find Venues',
            short_name: 'Venues',
            description: 'Discover nearby venues and restaurants',
            url: '/venues?utm_source=pwa_shortcut',
            icons: [{ src: 'pwa-192x192.png', sizes: '192x192' }]
          },
          {
            name: 'My Account',
            short_name: 'Account',
            description: 'Manage your profile and preferences',
            url: '/profile?utm_source=pwa_shortcut',
            icons: [{ src: 'pwa-192x192.png', sizes: '192x192' }]
          }
        ],
        related_applications: [],
        prefer_related_applications: false,
        edge_side_panel: {
          preferred_width: 400
        }
      },
      devOptions: {
        enabled: true,
        type: 'module',
      }
    })
  ],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    },
  },
  server: {
    host: true,
    port: parseInt(process.env.PORT ?? "5173"),
  }
})
