<template>
  <div class="p-6 space-y-6">
    <h2 class="text-2xl font-bold text-gray-900 dark:text-white">Native Features Demo</h2>
    
    <!-- Platform Info -->
    <div class="bg-blue-50 dark:bg-blue-900/20 border border-blue-200 dark:border-blue-800 rounded-lg p-4">
      <h3 class="font-semibold text-blue-900 dark:text-blue-100 mb-2">Platform Information</h3>
      <div class="text-sm text-blue-800 dark:text-blue-200 space-y-1">
        <p><strong>Platform:</strong> {{ platform }}</p>
        <p><strong>Native App:</strong> {{ isNative ? 'Yes' : 'No' }}</p>
        <p><strong>iOS:</strong> {{ isIOS ? 'Yes' : 'No' }}</p>
        <p><strong>Android:</strong> {{ isAndroid ? 'Yes' : 'No' }}</p>
      </div>
    </div>

    <!-- Network Status -->
    <div class="bg-gray-50 dark:bg-gray-800 border border-gray-200 dark:border-gray-700 rounded-lg p-4">
      <h3 class="font-semibold text-gray-900 dark:text-white mb-2">Network Status</h3>
      <div class="flex items-center gap-2">
        <div 
          :class="[
            'w-3 h-3 rounded-full',
            networkStatus.connected ? 'bg-green-500' : 'bg-red-500'
          ]"
        ></div>
        <span class="text-sm text-gray-700 dark:text-gray-300">
          {{ networkStatus.connected ? 'Connected' : 'Offline' }}
          {{ networkStatus.connectionType && networkStatus.connected ? `(${networkStatus.connectionType})` : '' }}
        </span>
      </div>
    </div>

    <!-- Geolocation -->
    <div class="space-y-3">
      <h3 class="font-semibold text-gray-900 dark:text-white">Geolocation</h3>
      <button
        @click="getLocation"
        :disabled="loading"
        class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed"
      >
        {{ loading ? 'Getting Location...' : 'Get Current Location' }}
      </button>
      
      <div v-if="position" class="bg-green-50 dark:bg-green-900/20 border border-green-200 dark:border-green-800 rounded-lg p-3">
        <p class="text-sm text-green-800 dark:text-green-200">
          <strong>Latitude:</strong> {{ position.latitude.toFixed(6) }}<br>
          <strong>Longitude:</strong> {{ position.longitude.toFixed(6) }}<br>
          <strong>Accuracy:</strong> {{ position.accuracy ? `${position.accuracy}m` : 'Unknown' }}
        </p>
      </div>
      
      <div v-if="error" class="bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-lg p-3">
        <p class="text-sm text-red-800 dark:text-red-200">{{ error }}</p>
      </div>
    </div>

    <!-- Camera -->
    <div v-if="isNative" class="space-y-3">
      <h3 class="font-semibold text-gray-900 dark:text-white">Camera</h3>
      <div class="flex gap-2">
        <button
          @click="takePhoto"
          class="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700"
        >
          Take Photo
        </button>
        <button
          @click="selectPhoto"
          class="px-4 py-2 bg-purple-600 text-white rounded-lg hover:bg-purple-700"
        >
          Select Photo
        </button>
      </div>
      
      <div v-if="photoUrl" class="border border-gray-200 dark:border-gray-700 rounded-lg p-2">
        <img :src="photoUrl" alt="Captured photo" class="max-w-full h-auto rounded">
      </div>
    </div>

    <!-- Haptics -->
    <div v-if="isNative" class="space-y-3">
      <h3 class="font-semibold text-gray-900 dark:text-white">Haptic Feedback</h3>
      <div class="flex gap-2">
        <button
          @click="() => vibrate('light')"
          class="px-3 py-1 bg-gray-400 text-white rounded hover:bg-gray-500"
        >
          Light
        </button>
        <button
          @click="() => vibrate('medium')"
          class="px-3 py-1 bg-gray-500 text-white rounded hover:bg-gray-600"
        >
          Medium
        </button>
        <button
          @click="() => vibrate('heavy')"
          class="px-3 py-1 bg-gray-600 text-white rounded hover:bg-gray-700"
        >
          Heavy
        </button>
      </div>
    </div>

    <!-- Clipboard -->
    <div class="space-y-3">
      <h3 class="font-semibold text-gray-900 dark:text-white">Clipboard</h3>
      <div class="flex gap-2">
        <input
          v-model="clipboardText"
          type="text"
          placeholder="Text to copy"
          class="flex-1 px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
        >
        <button
          @click="copyText"
          class="px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700"
        >
          Copy
        </button>
      </div>
      <button
        @click="pasteText"
        class="px-4 py-2 bg-orange-600 text-white rounded-lg hover:bg-orange-700"
      >
        Paste from Clipboard
      </button>
    </div>

    <!-- Share -->
    <div class="space-y-3">
      <h3 class="font-semibold text-gray-900 dark:text-white">Share</h3>
      <button
        @click="shareContent"
        class="px-4 py-2 bg-pink-600 text-white rounded-lg hover:bg-pink-700"
      >
        Share This Page
      </button>
    </div>

    <!-- Toast -->
    <div class="space-y-3">
      <h3 class="font-semibold text-gray-900 dark:text-white">Toast Notifications</h3>
      <div class="flex gap-2">
        <button
          @click="() => showToast('Short toast message', 'short')"
          class="px-4 py-2 bg-yellow-600 text-white rounded-lg hover:bg-yellow-700"
        >
          Short Toast
        </button>
        <button
          @click="() => showToast('Long toast message that stays longer', 'long')"
          class="px-4 py-2 bg-amber-600 text-white rounded-lg hover:bg-amber-700"
        >
          Long Toast
        </button>
      </div>
    </div>

    <!-- Keyboard -->
    <div v-if="isNative" class="space-y-3">
      <h3 class="font-semibold text-gray-900 dark:text-white">Keyboard</h3>
      <div class="text-sm text-gray-600 dark:text-gray-400">
        <p><strong>Keyboard Visible:</strong> {{ keyboardVisible ? 'Yes' : 'No' }}</p>
        <p v-if="keyboardHeight > 0"><strong>Height:</strong> {{ keyboardHeight }}px</p>
      </div>
      <input
        type="text"
        placeholder="Focus to show keyboard"
        class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
      >
      <button
        @click="hideKeyboard"
        class="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700"
      >
        Hide Keyboard
      </button>
    </div>

    <!-- App State -->
    <div v-if="isNative" class="bg-gray-50 dark:bg-gray-800 border border-gray-200 dark:border-gray-700 rounded-lg p-4">
      <h3 class="font-semibold text-gray-900 dark:text-white mb-2">App State</h3>
      <div class="flex items-center gap-2">
        <div 
          :class="[
            'w-3 h-3 rounded-full',
            isAppActive ? 'bg-green-500' : 'bg-orange-500'
          ]"
        ></div>
        <span class="text-sm text-gray-700 dark:text-gray-300">
          {{ isAppActive ? 'Active' : 'Background' }}
        </span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { 
  useGeolocation, 
  useCamera, 
  useHaptics, 
  useNetwork, 
  useToast, 
  useClipboard, 
  useShare, 
  useKeyboard, 
  useAppState, 
  usePlatform 
} from '@/composables/useNative'

// Platform info
const { isNative, platform, isIOS, isAndroid } = usePlatform()

// Geolocation
const { position, error, loading, getCurrentPosition } = useGeolocation()

// Camera
const { takePicture, selectPhoto: selectPhotoFromGallery } = useCamera()
const photoUrl = ref<string | null>(null)

// Haptics
const { vibrate } = useHaptics()

// Network
const { networkStatus } = useNetwork()

// Toast
const { showToast } = useToast()

// Clipboard
const { copyToClipboard, readFromClipboard } = useClipboard()
const clipboardText = ref('Hello from Pulse!')

// Share
const { share } = useShare()

// Keyboard
const { keyboardVisible, keyboardHeight, hideKeyboard } = useKeyboard()

// App state
const { isAppActive } = useAppState()

// Methods
const getLocation = async () => {
  await getCurrentPosition()
}

const takePhoto = async () => {
  const photo = await takePicture()
  if (photo) {
    photoUrl.value = photo
  }
}

const selectPhoto = async () => {
  const photo = await selectPhotoFromGallery()
  if (photo) {
    photoUrl.value = photo
  }
}

const copyText = async () => {
  if (clipboardText.value) {
    await copyToClipboard(clipboardText.value)
  }
}

const pasteText = async () => {
  const text = await readFromClipboard()
  if (text) {
    clipboardText.value = text
  }
}

const shareContent = async () => {
  await share({
    title: 'Pulse App',
    text: 'Check out this awesome venue and specials app!',
    url: window.location.href
  })
}
</script>
