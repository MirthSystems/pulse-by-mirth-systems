import { ref, reactive, onMounted, onUnmounted } from 'vue'
import { capacitorService } from '@/utils/capacitor'

export function useGeolocation() {
  const position = ref<{ latitude: number; longitude: number; accuracy?: number } | null>(null)
  const error = ref<string | null>(null)
  const loading = ref(false)

  const getCurrentPosition = async () => {
    loading.value = true
    error.value = null
    
    try {
      const pos = await capacitorService.getCurrentPosition()
      position.value = pos
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to get location'
    } finally {
      loading.value = false
    }
  }

  return {
    position,
    error,
    loading,
    getCurrentPosition
  }
}

export function useCamera() {
  const takePicture = async (): Promise<string | null> => {
    try {
      await capacitorService.vibrate('light')
      return await capacitorService.takePicture()
    } catch (error) {
      console.error('Error taking picture:', error)
      return null
    }
  }

  const selectPhoto = async (): Promise<string | null> => {
    try {
      return await capacitorService.selectPhoto()
    } catch (error) {
      console.error('Error selecting photo:', error)
      return null
    }
  }

  return {
    takePicture,
    selectPhoto
  }
}

export function useHaptics() {
  const vibrate = async (style: 'light' | 'medium' | 'heavy' = 'medium') => {
    try {
      await capacitorService.vibrate(style)
    } catch (error) {
      console.error('Error with haptic feedback:', error)
    }
  }

  return {
    vibrate
  }
}

export function useNetwork() {
  const networkStatus = reactive({
    connected: true,
    connectionType: 'unknown'
  })

  const updateNetworkStatus = async () => {
    const status = await capacitorService.getNetworkStatus()
    networkStatus.connected = status.connected
    networkStatus.connectionType = status.connectionType || 'unknown'
  }

  onMounted(() => {
    updateNetworkStatus()
    
    // Listen for network changes
    const handleNetworkChange = (event: CustomEvent) => {
      networkStatus.connected = event.detail.connected
      networkStatus.connectionType = event.detail.connectionType || 'unknown'
    }

    window.addEventListener('network-status-change', handleNetworkChange as EventListener)
    
    onUnmounted(() => {
      window.removeEventListener('network-status-change', handleNetworkChange as EventListener)
    })
  })

  return {
    networkStatus,
    updateNetworkStatus
  }
}

export function useToast() {
  const showToast = async (message: string, duration: 'short' | 'long' = 'short') => {
    try {
      await capacitorService.showToast(message, duration)
    } catch (error) {
      console.error('Error showing toast:', error)
    }
  }

  return {
    showToast
  }
}

export function useClipboard() {
  const copyToClipboard = async (text: string): Promise<boolean> => {
    try {
      const success = await capacitorService.copyToClipboard(text)
      if (success) {
        await capacitorService.showToast('Copied to clipboard')
        await capacitorService.vibrate('light')
      }
      return success
    } catch (error) {
      console.error('Error copying to clipboard:', error)
      return false
    }
  }

  const readFromClipboard = async (): Promise<string | null> => {
    try {
      return await capacitorService.readFromClipboard()
    } catch (error) {
      console.error('Error reading from clipboard:', error)
      return null
    }
  }

  return {
    copyToClipboard,
    readFromClipboard
  }
}

export function useShare() {
  const share = async (options: { title: string; text: string; url?: string }): Promise<boolean> => {
    try {
      return await capacitorService.share(options)
    } catch (error) {
      console.error('Error sharing:', error)
      return false
    }
  }

  return {
    share
  }
}

export function useKeyboard() {
  const keyboardVisible = ref(false)
  const keyboardHeight = ref(0)

  onMounted(() => {
    const handleKeyboardShow = (event: CustomEvent) => {
      keyboardVisible.value = true
      keyboardHeight.value = event.detail?.keyboardHeight || 0
    }

    const handleKeyboardHide = () => {
      keyboardVisible.value = false
      keyboardHeight.value = 0
    }

    window.addEventListener('keyboard-did-show', handleKeyboardShow as EventListener)
    window.addEventListener('keyboard-did-hide', handleKeyboardHide as EventListener)

    onUnmounted(() => {
      window.removeEventListener('keyboard-did-show', handleKeyboardShow as EventListener)
      window.removeEventListener('keyboard-did-hide', handleKeyboardHide as EventListener)
    })
  })

  const hideKeyboard = async () => {
    try {
      await capacitorService.hideKeyboard()
    } catch (error) {
      console.error('Error hiding keyboard:', error)
    }
  }

  return {
    keyboardVisible,
    keyboardHeight,
    hideKeyboard
  }
}

export function useAppState() {
  const isAppActive = ref(true)

  onMounted(() => {
    const handleAppResume = () => {
      isAppActive.value = true
    }

    const handleAppPause = () => {
      isAppActive.value = false
    }

    window.addEventListener('app-resumed', handleAppResume)
    window.addEventListener('app-paused', handleAppPause)

    onUnmounted(() => {
      window.removeEventListener('app-resumed', handleAppResume)
      window.removeEventListener('app-paused', handleAppPause)
    })
  })

  return {
    isAppActive
  }
}

export function usePlatform() {
  const isNative = capacitorService.isNative()
  const platform = capacitorService.getPlatform()
  
  return {
    isNative,
    platform,
    isIOS: platform === 'ios',
    isAndroid: platform === 'android',
    isWeb: platform === 'web'
  }
}
