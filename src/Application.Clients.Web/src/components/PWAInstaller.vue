<template>
  <div v-if="showInstallButton" class="fixed bottom-4 left-4 right-4 z-50">
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-lg border border-gray-200 dark:border-gray-700 p-4">
      <div class="flex items-center gap-3">
        <div class="flex-shrink-0">
          <svg class="w-8 h-8 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"></path>
          </svg>
        </div>
        <div class="flex-1">
          <h3 class="text-sm font-medium text-gray-900 dark:text-white">Install Pulse</h3>
          <p class="text-xs text-gray-600 dark:text-gray-300">Add to your home screen for quick access</p>
        </div>
        <div class="flex gap-2">
          <button
            @click="dismissInstall"
            class="px-3 py-1 text-xs text-gray-600 dark:text-gray-300 hover:text-gray-800 dark:hover:text-white"
          >
            Later
          </button>
          <button
            @click="installApp"
            class="px-3 py-1 text-xs bg-blue-600 text-white rounded hover:bg-blue-700 transition-colors"
            :disabled="installing"
          >
            {{ installing ? 'Installing...' : 'Install' }}
          </button>
        </div>
      </div>
    </div>
  </div>

  <!-- iOS Installation Instructions -->
  <div v-if="showIOSInstructions" class="fixed inset-0 bg-black bg-opacity-50 z-50 flex items-center justify-center p-4">
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-lg p-6 max-w-sm w-full">
      <div class="text-center">
        <div class="mb-4">
          <svg class="w-12 h-12 text-blue-600 mx-auto" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"></path>
          </svg>
        </div>
        <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-2">Install Pulse</h3>
        <div class="text-sm text-gray-600 dark:text-gray-300 text-left space-y-2">
          <p class="flex items-center gap-2">
            <span>1.</span>
            <span>Tap the</span>
            <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 24 24">
              <path d="M12 2L12 22M5 12L19 12"></path>
            </svg>
            <span>share button</span>
          </p>
          <p class="flex items-center gap-2">
            <span>2.</span>
            <span>Select "Add to Home Screen"</span>
          </p>
          <p class="flex items-center gap-2">
            <span>3.</span>
            <span>Tap "Add"</span>
          </p>
        </div>
        <button
          @click="showIOSInstructions = false"
          class="mt-4 w-full px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 transition-colors"
        >
          Got it
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { installPWA } from '@/utils/pwa'
import { usePlatform } from '@/composables/useNative'

const showInstallButton = ref(false)
const showIOSInstructions = ref(false)
const installing = ref(false)

const { isIOS, isWeb } = usePlatform()

const installApp = async () => {
  if (isIOS) {
    showIOSInstructions.value = true
    return
  }

  installing.value = true
  try {
    const installed = await installPWA()
    if (installed) {
      showInstallButton.value = false
    }
  } catch (error) {
    console.error('Error installing PWA:', error)
  } finally {
    installing.value = false
  }
}

const dismissInstall = () => {
  showInstallButton.value = false
  // Store dismissal in localStorage
  localStorage.setItem('pwa-install-dismissed', Date.now().toString())
}

const handleInstallAvailable = () => {
  // Check if user previously dismissed
  const dismissed = localStorage.getItem('pwa-install-dismissed')
  if (dismissed) {
    const dismissedTime = parseInt(dismissed)
    const oneWeekAgo = Date.now() - (7 * 24 * 60 * 60 * 1000)
    if (dismissedTime > oneWeekAgo) {
      return // Don't show if dismissed within the last week
    }
  }

  showInstallButton.value = true
}

const handleInstallCompleted = () => {
  showInstallButton.value = false
}

onMounted(() => {
  if (isWeb) {
    window.addEventListener('pwa-install-available', handleInstallAvailable)
    window.addEventListener('pwa-install-completed', handleInstallCompleted)
  }
})

onUnmounted(() => {
  if (isWeb) {
    window.removeEventListener('pwa-install-available', handleInstallAvailable)
    window.removeEventListener('pwa-install-completed', handleInstallCompleted)
  }
})
</script>
