<template>
  <div v-if="showInstallPrompt" class="fixed bottom-4 left-4 right-4 z-50">
    <div class="bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 rounded-lg shadow-lg p-4">
      <div class="flex items-start justify-between">
        <div class="flex-1">
          <h3 class="text-sm font-medium text-gray-900 dark:text-white">
            Install Pulse App
          </h3>
          <p class="mt-1 text-xs text-gray-500 dark:text-gray-400">
            Add Pulse to your home screen for quick access and offline use.
          </p>
        </div>
        <button
          @click="dismissPrompt"
          class="ml-3 flex-shrink-0 text-gray-400 hover:text-gray-600 dark:hover:text-gray-200"
        >
          <XMarkIcon class="h-5 w-5" />
        </button>
      </div>
      
      <div class="mt-3 flex space-x-2">
        <button
          @click="installApp"
          :disabled="installing"
          class="flex-1 bg-blue-600 hover:bg-blue-700 disabled:bg-blue-400 text-white text-xs font-medium py-2 px-3 rounded-md transition-colors"
        >
          <span v-if="!installing">Install</span>
          <span v-else class="flex items-center justify-center">
            <svg class="animate-spin -ml-1 mr-2 h-3 w-3 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
              <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
              <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
            </svg>
            Installing...
          </span>
        </button>
        <button
          @click="dismissPrompt"
          class="flex-1 bg-gray-100 hover:bg-gray-200 dark:bg-gray-700 dark:hover:bg-gray-600 text-gray-700 dark:text-gray-300 text-xs font-medium py-2 px-3 rounded-md transition-colors"
        >
          Not now
        </button>
      </div>
    </div>
  </div>

  <!-- Network status indicator -->
  <div v-if="!isOnline" class="fixed top-0 left-0 right-0 z-50">
    <div class="bg-yellow-400 text-yellow-900 text-center py-2 px-4 text-sm font-medium">
      <div class="flex items-center justify-center">
        <ExclamationTriangleIcon class="h-4 w-4 mr-2" />
        You're offline. Some features may not be available.
      </div>
    </div>
  </div>

  <!-- PWA update available notification -->
  <div v-if="updateAvailable" class="fixed top-16 left-4 right-4 z-50">
    <div class="bg-blue-600 text-white rounded-lg shadow-lg p-4">
      <div class="flex items-start justify-between">
        <div class="flex-1">
          <h3 class="text-sm font-medium">Update Available</h3>
          <p class="mt-1 text-xs opacity-90">
            A new version of Pulse is available.
          </p>
        </div>
        <button
          @click="updateAvailable = false"
          class="ml-3 flex-shrink-0 text-white/70 hover:text-white"
        >
          <XMarkIcon class="h-5 w-5" />
        </button>
      </div>
      
      <div class="mt-3 flex space-x-2">
        <button
          @click="reloadApp"
          class="flex-1 bg-white text-blue-600 text-xs font-medium py-2 px-3 rounded-md hover:bg-gray-50 transition-colors"
        >
          Update Now
        </button>
        <button
          @click="updateAvailable = false"
          class="flex-1 bg-blue-500 hover:bg-blue-400 text-white text-xs font-medium py-2 px-3 rounded-md transition-colors"
        >
          Later
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { XMarkIcon, ExclamationTriangleIcon } from '@heroicons/vue/24/outline'
import { installPWA, isPWA } from '@/utils/pwa'

const showInstallPrompt = ref(false)
const installing = ref(false)
const isOnline = ref(navigator.onLine)
const updateAvailable = ref(false)

// Event listeners
const handleInstallAvailable = () => {
  if (!isPWA()) {
    showInstallPrompt.value = true
  }
}

const handleInstallCompleted = () => {
  showInstallPrompt.value = false
}

const handleNetworkChange = (event: CustomEvent) => {
  isOnline.value = event.detail.online
}

const handleUpdateAvailable = () => {
  updateAvailable.value = true
}

const installApp = async () => {
  installing.value = true
  try {
    const installed = await installPWA()
    if (installed) {
      showInstallPrompt.value = false
    }
  } catch (error) {
    console.error('Error installing PWA:', error)
  } finally {
    installing.value = false
  }
}

const dismissPrompt = () => {
  showInstallPrompt.value = false
  // Remember user dismissed it (you might want to store this in localStorage)
  localStorage.setItem('pwa-install-dismissed', 'true')
}

const reloadApp = () => {
  window.location.reload()
}

onMounted(() => {
  // Check if user previously dismissed install prompt
  const dismissed = localStorage.getItem('pwa-install-dismissed')
  if (dismissed) {
    showInstallPrompt.value = false
  }

  // Add event listeners
  window.addEventListener('pwa-install-available', handleInstallAvailable)
  window.addEventListener('pwa-install-completed', handleInstallCompleted)
  window.addEventListener('network-status-changed', handleNetworkChange as EventListener)
  window.addEventListener('pwa-update-available', handleUpdateAvailable)
})

onUnmounted(() => {
  // Remove event listeners
  window.removeEventListener('pwa-install-available', handleInstallAvailable)
  window.removeEventListener('pwa-install-completed', handleInstallCompleted)
  window.removeEventListener('network-status-changed', handleNetworkChange as EventListener)
  window.removeEventListener('pwa-update-available', handleUpdateAvailable)
})
</script>
