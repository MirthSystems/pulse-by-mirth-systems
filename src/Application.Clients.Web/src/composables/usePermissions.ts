import { computed, ref, watch, onMounted, onUnmounted } from 'vue'
import { useAuth0 } from '@auth0/auth0-vue'
import { storeToRefs } from 'pinia'
import { useAuthStore } from '@/stores/auth'
import apiService from '@/services/api'
import type { UserVenuePermission } from '@/types/api'

export function usePermissions() {
  const { user: auth0User, isAuthenticated, isLoading: auth0Loading, getAccessTokenSilently } = useAuth0()
  const authStore = useAuthStore()
  const { user } = storeToRefs(authStore)
  
  // Store user venue permissions locally
  const userVenuePermissions = ref<UserVenuePermission[]>([])
  const permissionsLoaded = ref(false)
  const permissionsLoading = ref(false)
  const retryCount = ref(0)
  const maxRetries = 3
  
  // Store token permissions
  const tokenPermissions = ref<string[]>([])

  // Get permissions from JWT access token (same logic as ProfileView)
  const loadTokenPermissions = async () => {
    try {
      const accessToken = await getAccessTokenSilently()
      if (accessToken) {
        // Parse JWT token to get claims (basic parsing without verification)
        const tokenParts = accessToken.split('.')
        if (tokenParts.length === 3) {
          const payload = JSON.parse(atob(tokenParts[1]))
          tokenPermissions.value = payload.permissions || []
          console.debug('Token permissions loaded:', tokenPermissions.value)
        }
      }
    } catch (error) {
      console.debug('Could not get access token for permissions:', error)
      tokenPermissions.value = []
    }
  }

  // Check if user has specific Auth0 permission
  const hasAuth0Permission = (permission: string) => {
    if (!isAuthenticated.value) {
      console.debug('Auth check failed: not authenticated')
      return false
    }
    
    const hasPermission = tokenPermissions.value.includes(permission)
    console.debug('Checking permission:', permission, 'Found:', hasPermission, 'In:', tokenPermissions.value)
    return hasPermission
  }

  // System-level permissions (try both formats)
  const isSystemAdmin = computed(() => 
    hasAuth0Permission('system:admin')
  )
  const isContentManager = computed(() => 
    hasAuth0Permission('content:manager')
  )
  const hasSystemAccess = computed(() => isSystemAdmin.value || isContentManager.value)

  // Load user venue permissions from API
  const loadUserVenuePermissions = async () => {
    if (!isAuthenticated.value || auth0Loading.value || permissionsLoading.value) return
    
    // Check if API token is available
    if (!apiService.hasAccessToken()) {
      console.debug('API token not available yet, waiting...')
      return
    }
    
    permissionsLoading.value = true
    try {
      const response = await apiService.getMyPermissions()
      if (response.success) {
        userVenuePermissions.value = response.data
        permissionsLoaded.value = true
        retryCount.value = 0
        console.debug('User venue permissions loaded successfully')
      }
    } catch (error: any) {
      // Handle 401 errors (token issues) with retry logic
      if (error.message?.includes('401') && retryCount.value < maxRetries) {
        retryCount.value++
        console.debug(`API token not ready (attempt ${retryCount.value}/${maxRetries}), retrying...`)
        // Exponential backoff: 1s, 2s, 4s
        setTimeout(() => {
          if (isAuthenticated.value && !permissionsLoaded.value) {
            loadUserVenuePermissions()
          }
        }, Math.pow(2, retryCount.value - 1) * 1000)
      } else if (!error.message?.includes('401')) {
        console.error('Failed to load user venue permissions:', error)
        retryCount.value = 0
      }
    } finally {
      permissionsLoading.value = false
    }
  }

  // Watch for authentication changes to load permissions
  watch(
    [() => isAuthenticated.value, () => auth0Loading.value],
    async ([authenticated, loading]) => {
      if (authenticated && !loading) {
        // Load token permissions first
        await loadTokenPermissions()
        
        // Then load venue permissions if not already loaded
        if (!permissionsLoaded.value) {
          setTimeout(() => {
            if (isAuthenticated.value && !auth0Loading.value && !permissionsLoaded.value) {
              loadUserVenuePermissions()
            }
          }, 100)
        }
      } else if (!authenticated) {
        tokenPermissions.value = []
        userVenuePermissions.value = []
        permissionsLoaded.value = false
        retryCount.value = 0
      }
    },
    { immediate: true }
  )

  // Listen for API token updates
  const handleTokenUpdate = async () => {
    if (isAuthenticated.value && !auth0Loading.value) {
      console.debug('API token updated, attempting to load permissions...')
      await loadTokenPermissions()
      if (!permissionsLoaded.value) {
        loadUserVenuePermissions()
      }
    }
  }

  onMounted(() => {
    window.addEventListener('api-token-updated', handleTokenUpdate)
  })

  onUnmounted(() => {
    window.removeEventListener('api-token-updated', handleTokenUpdate)
  })

  const hasVenueAccess = computed(() => {
    return hasSystemAccess.value || userVenuePermissions.value.length > 0
  })

  // Check if user can access backoffice
  const canAccessBackoffice = computed(() => {
    return hasVenueAccess.value
  })

  // Check if user can create venues (only system admins and content managers)
  const canCreateVenues = computed(() => {
    return hasSystemAccess.value
  })

  // Check if user can delete venues (only system admins and content managers)
  const canDeleteVenues = computed(() => {
    return hasSystemAccess.value
  })

  // Check permissions for a specific venue
  const getVenuePermissions = (venueId: number) => {
    if (hasSystemAccess.value) {
      return {
        canView: true,
        canEdit: true,
        canDelete: true,
        canManageUsers: true,
        canManageSpecials: true,
        role: isSystemAdmin.value ? 'system:admin' : 'content:manager'
      }
    }

    // Check venue-specific permissions from userVenuePermissions
    const venuePermission = userVenuePermissions.value.find(
      (p: UserVenuePermission) => p.venueId === venueId && p.isActive
    )

    if (!venuePermission) {
      return {
        canView: false,
        canEdit: false,
        canDelete: false,
        canManageUsers: false,
        canManageSpecials: false,
        role: null
      }
    }

    const role = venuePermission.name
    return {
      canView: true,
      canEdit: role === 'venue:owner' || role === 'venue:manager',
      canDelete: false, // Only system admins can delete venues
      canManageUsers: role === 'venue:owner',
      canManageSpecials: true, // All venue users can manage specials
      role: role
    }
  }

  // Manual retry function for components to call
  const retryLoadPermissions = () => {
    if (isAuthenticated.value && !auth0Loading.value) {
      retryCount.value = 0
      loadUserVenuePermissions()
    }
  }

  return {
    // Auth0 permissions
    isSystemAdmin,
    isContentManager,
    hasSystemAccess,
    
    // Venue permissions
    userVenuePermissions,
    hasVenueAccess,
    
    // Backoffice access
    canAccessBackoffice,
    canCreateVenues,
    canDeleteVenues,
    
    // Venue-specific permissions
    getVenuePermissions,
    
    // Loading state
    permissionsLoading,
    permissionsLoaded,
    
    // Helper functions
    hasAuth0Permission,
    loadUserVenuePermissions,
    retryLoadPermissions
  }
}
