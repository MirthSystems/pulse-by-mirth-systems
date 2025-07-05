<script setup lang="ts">
import { ref } from 'vue'
import { RouterView } from 'vue-router'
import { 
  Bars3Icon,
  XMarkIcon
} from '@heroicons/vue/24/outline'
import Auth from './components/Auth.vue'
import PWAPrompt from './components/PWAPrompt.vue'
import { useApiAuth } from './composables/useApiAuth'

const mobileMenuOpen = ref(false)

// Initialize API authentication
useApiAuth()
</script>

<template>
  <div id="app" class="min-h-screen bg-gray-50">
    <!-- Navigation Header -->
    <nav class="bg-white shadow-sm border-b border-gray-200">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between h-16">
          <!-- Logo -->
          <div class="flex items-center">
            <router-link to="/" class="flex items-center space-x-3">
              <div class="w-8 h-8 bg-gradient-to-br from-blue-500 to-purple-600 rounded-lg flex items-center justify-center">
                <span class="text-white font-bold text-lg">P</span>
              </div>
              <span class="text-xl font-bold text-gray-900">Pulse</span>
            </router-link>
          </div>
          
          <!-- Auth Component -->
          <div class="flex items-center space-x-2 md:space-x-0">
            <Auth />
          </div>
        </div>
      </div>
      
      <!-- Mobile menu -->
      <div v-if="mobileMenuOpen" class="md:hidden border-t border-gray-200">
        <div class="px-2 pt-2 pb-3 space-y-1 sm:px-3 bg-white">
          <!-- Mobile menu items will be populated by Auth component when authenticated -->
        </div>
      </div>
    </nav>
    
    <!-- Main Content -->
    <main>
      <router-view />
    </main>
    
    <!-- Footer -->
    <footer class="bg-white border-t border-gray-200">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div class="text-center text-gray-600">
          <p>&copy; 2025 Pulse. Your real-time guide for food, drink and entertainment specials!</p>
        </div>
      </div>
    </footer>

    <!-- PWA Installation and Status Prompts -->
    <PWAPrompt />
  </div>
</template>

<style scoped>
/* Remove old navigation styles as we're moving to a simpler layout */
</style>
