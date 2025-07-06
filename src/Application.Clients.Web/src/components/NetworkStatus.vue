<template>
  <Transition
    enter-active-class="transition-all duration-300 ease-out"
    enter-from-class="transform -translate-y-full opacity-0"
    enter-to-class="transform translate-y-0 opacity-100"
    leave-active-class="transition-all duration-300 ease-in"
    leave-from-class="transform translate-y-0 opacity-100"
    leave-to-class="transform -translate-y-full opacity-0"
  >
    <div
      v-if="!networkStatus.connected"
      class="fixed top-0 left-0 right-0 z-50 bg-red-600 text-white px-4 py-2 text-center text-sm font-medium"
    >
      <div class="flex items-center justify-center gap-2">
        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M18.364 5.636l-12.728 12.728m0-12.728l12.728 12.728"></path>
        </svg>
        <span>No internet connection</span>
      </div>
    </div>
  </Transition>

  <!-- Connection restored notification -->
  <Transition
    enter-active-class="transition-all duration-500 ease-out"
    enter-from-class="transform translate-x-full opacity-0"
    enter-to-class="transform translate-x-0 opacity-100"
    leave-active-class="transition-all duration-300 ease-in"
    leave-from-class="transform translate-x-0 opacity-100"
    leave-to-class="transform translate-x-full opacity-0"
  >
    <div
      v-if="showReconnected"
      class="fixed top-4 right-4 z-50 bg-green-600 text-white px-4 py-3 rounded-lg shadow-lg"
    >
      <div class="flex items-center gap-2">
        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"></path>
        </svg>
        <span class="text-sm font-medium">Connection restored</span>
      </div>
    </div>
  </Transition>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { useNetwork, useHaptics } from '@/composables/useNative'

const { networkStatus } = useNetwork()
const { vibrate } = useHaptics()

const showReconnected = ref(false)
let reconnectedTimeout: number | null = null

// Track previous connection state to detect reconnection
let wasDisconnected = false

watch(
  () => networkStatus.connected,
  (connected, previousConnected) => {
    if (connected && previousConnected === false) {
      // Connection restored
      wasDisconnected = false
      showReconnected.value = true
      vibrate('light')
      
      // Hide the reconnected message after 3 seconds
      if (reconnectedTimeout) {
        clearTimeout(reconnectedTimeout)
      }
      reconnectedTimeout = setTimeout(() => {
        showReconnected.value = false
      }, 3000)
    } else if (!connected && previousConnected !== false) {
      // Connection lost
      wasDisconnected = true
      vibrate('medium')
      showReconnected.value = false
    }
  },
  { immediate: false }
)
</script>
