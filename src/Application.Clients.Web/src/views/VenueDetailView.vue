<script setup lang="ts">
import { onMounted, computed, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useVenueStore } from '../stores/venue'
import { useAuthStore } from '../stores/auth'
import { apiService } from '../services/api'
import type { UserVenuePermission } from '../types/api'
import SpecialCard from '../components/SpecialCard.vue'
import { 
  BuildingStorefrontIcon,
  PhoneIcon,
  GlobeAltIcon,
  EnvelopeIcon,
  MapPinIcon,
  ClockIcon,
  StarIcon,
  ShareIcon,
  HeartIcon,
  ArrowLeftIcon,
  CameraIcon,
  PencilIcon,
  UsersIcon,
  Cog6ToothIcon
} from '@heroicons/vue/24/outline'
import { HeartIcon as HeartIconSolid } from '@heroicons/vue/24/solid'

interface Props {
  id: string
}

const props = defineProps<Props>()
const route = useRoute()
const router = useRouter()
const venueStore = useVenueStore()
const authStore = useAuthStore()

const venue = computed(() => venueStore.selectedVenue)
const loading = computed(() => venueStore.loading)
const error = computed(() => venueStore.error)

const venueId = computed(() => parseInt(props.id))

// User permissions
const userPermissions = ref<UserVenuePermission[]>([])
const loadingPermissions = ref(false)

// Check if user has permission for this venue
const venuePermission = computed(() => {
  return userPermissions.value.find(p => p.venueId === venueId.value)
})

const canManageVenue = computed(() => {
  if (!userPermissions.value || !venueId.value) return false
  console.log('Checking canManageVenue. UserPermissions:', userPermissions.value, 'VenueId:', venueId.value)
  const permission = userPermissions.value.find(p => p.venueId === venueId.value)
  console.log('Found permission:', permission)
  return permission && (permission.name === 'venue:owner' || permission.name === 'venue:manager')
})

const canManageUsers = computed(() => {
  if (!userPermissions.value || !venueId.value) return false
  console.log('Checking canManageUsers. UserPermissions:', userPermissions.value, 'VenueId:', venueId.value)
  const permission = userPermissions.value.find(p => p.venueId === venueId.value)
  console.log('Found permission for users:', permission)
  return permission && permission.name === 'venue:owner'
})

const isSystemAdmin = computed(() => {
  // For now, we'll rely on venue permissions. System admin check would need JWT token inspection
  return false
})

// Load user permissions
const loadUserPermissions = async () => {
  if (!authStore.isAuthenticated) return
  
  try {
    console.log('Loading user permissions...')
    loadingPermissions.value = true
    const response = await apiService.getMyPermissions()
    console.log('Permissions response:', response)
    if (response.success && response.data) {
      userPermissions.value = response.data
      console.log('User permissions loaded:', userPermissions.value)
    }
  } catch (error) {
    console.error('Error loading user permissions:', error)
  } finally {
    loadingPermissions.value = false
  }
}

// Navigation functions
const editVenue = () => {
  router.push(`/venues/${venueId.value}/edit`)
}

const manageVenueUsers = () => {
  router.push(`/venues/${venueId.value}/permissions`)
}

const currentDay = computed(() => {
  const today = new Date().getDay()
  // Convert JS day (0=Sunday) to match your business hours format
  return today === 0 ? 7 : today
})

const todaysHours = computed(() => {
  if (!venue.value?.businessHours) return null
  return venue.value.businessHours.find(hours => hours.dayOfWeekId === currentDay.value)
})

const formatTime = (time: string) => {
  // Convert 24h time to 12h format
  const [hours, minutes] = time.split(':')
  const hour = parseInt(hours)
  const ampm = hour >= 12 ? 'PM' : 'AM'
  const displayHour = hour === 0 ? 12 : hour > 12 ? hour - 12 : hour
  return `${displayHour}:${minutes} ${ampm}`
}

const isOpen = computed(() => {
  if (!todaysHours.value) return false
  if (todaysHours.value.isClosed) return false
  
  const now = new Date()
  const currentTime = now.getHours() * 100 + now.getMinutes()
  const [openHour, openMin] = todaysHours.value.openTime.split(':').map(Number)
  const [closeHour, closeMin] = todaysHours.value.closeTime.split(':').map(Number)
  const openTime = openHour * 100 + openMin
  const closeTime = closeHour * 100 + closeMin
  
  return currentTime >= openTime && currentTime <= closeTime
})

const activeSpecials = computed(() => 
  venue.value?.specials.filter(special => special.isActive) || []
)

const goBack = () => {
  router.back()
}

const shareVenue = async () => {
  if (navigator.share && venue.value) {
    try {
      await navigator.share({
        title: venue.value.name,
        text: venue.value.description,
        url: window.location.href,
      })
    } catch (err) {
      console.log('Error sharing:', err)
    }
  } else {
    // Fallback: copy to clipboard
    navigator.clipboard.writeText(window.location.href)
  }
}

const getDirections = () => {
  if (venue.value) {
    const address = encodeURIComponent(
      `${venue.value.streetAddress}, ${venue.value.locality}, ${venue.value.region}`
    )
    window.open(`https://maps.google.com/maps?q=${address}`, '_blank')
  }
}

onMounted(async () => {
  if (venueId.value) {
    await Promise.all([
      venueStore.fetchVenue(venueId.value),
      loadUserPermissions()
    ])
  }
})
</script>

<template>
  <div>
    <!-- Loading State -->
    <div v-if="loading" class="space-y-6">
      <div class="bg-white rounded-lg shadow-md p-6 animate-pulse">
        <div class="bg-gray-300 h-64 rounded-lg mb-4"></div>
        <div class="bg-gray-300 h-8 rounded mb-4"></div>
        <div class="bg-gray-300 h-4 rounded mb-2"></div>
        <div class="bg-gray-300 h-4 rounded w-2/3"></div>
      </div>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="bg-red-50 border border-red-200 rounded-lg p-6">
      <h3 class="text-lg font-medium text-red-800 mb-2">Error loading venue</h3>
      <p class="text-red-700 mb-4">{{ error }}</p>
      <div class="flex space-x-4">
        <button 
          @click="venueStore.fetchVenue(venueId)"
          class="px-4 py-2 bg-red-100 text-red-800 rounded hover:bg-red-200"
        >
          Try again
        </button>
        <button 
          @click="goBack"
          class="px-4 py-2 bg-gray-100 text-gray-800 rounded hover:bg-gray-200"
        >
          Go back
        </button>
      </div>
    </div>

    <!-- Venue Details -->
    <div v-else-if="venue" class="space-y-6">
      <!-- Header -->
      <div class="bg-white rounded-lg shadow-md overflow-hidden">
        <!-- Hero Image -->
        <div class="relative h-64 md:h-80 bg-gradient-to-r from-blue-500 to-purple-600">
          <div v-if="venue.profileImage" class="absolute inset-0">
            <img 
              :src="venue.profileImage" 
              :alt="venue.name"
              class="w-full h-full object-cover"
            />
          </div>
          <div v-else class="absolute inset-0 flex items-center justify-center">
            <CameraIcon class="h-16 w-16 text-white/50" />
          </div>
          
          <!-- Overlay -->
          <div class="absolute inset-0 bg-black/20"></div>
          
          <!-- Back Button -->
          <button
            @click="goBack"
            class="absolute top-4 left-4 p-2 bg-white/90 hover:bg-white text-gray-900 rounded-full transition-colors"
          >
            <ArrowLeftIcon class="h-5 w-5" />
          </button>
          
          <!-- Action Buttons -->
          <div class="absolute top-4 right-4 flex space-x-2">
            <!-- Management Buttons (for venue owners/managers) -->
            <button
              v-if="canManageVenue"
              @click="editVenue"
              class="p-2 bg-blue-600 hover:bg-blue-700 text-white rounded-full transition-colors"
              title="Edit Venue Details"
            >
              <PencilIcon class="h-5 w-5" />
            </button>
            <button
              v-if="canManageUsers"
              @click="manageVenueUsers"
              class="p-2 bg-green-600 hover:bg-green-700 text-white rounded-full transition-colors"
              title="Manage Users & Permissions"
            >
              <UsersIcon class="h-5 w-5" />
            </button>
            
            <!-- Standard Action Buttons -->
            <button
              @click="shareVenue"
              class="p-2 bg-white/90 hover:bg-white text-gray-900 rounded-full transition-colors"
            >
              <ShareIcon class="h-5 w-5" />
            </button>
            <button class="p-2 bg-white/90 hover:bg-white text-gray-900 rounded-full transition-colors">
              <HeartIcon class="h-5 w-5" />
            </button>
          </div>
          
          <!-- Status Badge -->
          <div class="absolute bottom-4 left-4">
            <div :class="[
              'px-3 py-1 rounded-full text-sm font-medium',
              isOpen 
                ? 'bg-green-100 text-green-800' 
                : 'bg-red-100 text-red-800'
            ]">
              {{ isOpen ? 'Open Now' : 'Closed' }}
            </div>
          </div>
        </div>
        
        <!-- Venue Info -->
        <div class="p-6">
          <div class="flex items-start justify-between mb-4">
            <div>
              <h1 class="text-3xl font-bold text-gray-900 mb-2">{{ venue.name }}</h1>
              <div class="flex items-center text-gray-600 mb-2">
                <span class="text-lg">{{ venue.categoryIcon }}</span>
                <span class="ml-2 text-sm font-medium">{{ venue.categoryName }}</span>
              </div>
              <p class="text-gray-600">{{ venue.description }}</p>
            </div>
            
            <div v-if="venue.activeSpecialsCount > 0" class="flex items-center bg-red-100 text-red-700 px-3 py-1 rounded-full">
              <StarIcon class="h-4 w-4 mr-1" />
              <span class="text-sm font-medium">{{ venue.activeSpecialsCount }} Active Specials</span>
            </div>
          </div>
          
          <!-- Contact Info -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <!-- Contact Details -->
            <div class="space-y-3">
              <h3 class="text-lg font-semibold text-gray-900 mb-3">Contact Information</h3>
              
              <div v-if="venue.phoneNumber" class="flex items-center">
                <PhoneIcon class="h-5 w-5 text-gray-400 mr-3" />
                <a 
                  :href="`tel:${venue.phoneNumber}`"
                  class="text-blue-600 hover:text-blue-800"
                >
                  {{ venue.phoneNumber }}
                </a>
              </div>
              
              <div v-if="venue.website" class="flex items-center">
                <GlobeAltIcon class="h-5 w-5 text-gray-400 mr-3" />
                <a 
                  :href="venue.website"
                  target="_blank"
                  rel="noopener noreferrer"
                  class="text-blue-600 hover:text-blue-800"
                >
                  Visit Website
                </a>
              </div>
              
              <div v-if="venue.email" class="flex items-center">
                <EnvelopeIcon class="h-5 w-5 text-gray-400 mr-3" />
                <a 
                  :href="`mailto:${venue.email}`"
                  class="text-blue-600 hover:text-blue-800"
                >
                  {{ venue.email }}
                </a>
              </div>
              
              <div class="flex items-start">
                <MapPinIcon class="h-5 w-5 text-gray-400 mr-3 mt-0.5" />
                <div>
                  <button
                    @click="getDirections"
                    class="text-blue-600 hover:text-blue-800 text-left"
                  >
                    {{ venue.streetAddress }}<br>
                    <span v-if="venue.secondaryAddress">{{ venue.secondaryAddress }}<br></span>
                    {{ venue.locality }}, {{ venue.region }} {{ venue.postalCode }}
                  </button>
                </div>
              </div>
            </div>
            
            <!-- Hours -->
            <div>
              <h3 class="text-lg font-semibold text-gray-900 mb-3">Business Hours</h3>
              <div class="space-y-2">
                <div 
                  v-for="hours in venue.businessHours" 
                  :key="hours.id"
                  :class="[
                    'flex justify-between text-sm',
                    hours.dayOfWeekId === currentDay 
                      ? 'font-medium text-gray-900' 
                      : 'text-gray-600'
                  ]"
                >
                  <span>{{ hours.dayOfWeekName }}</span>
                  <span v-if="hours.isClosed" class="text-red-600">Closed</span>
                  <span v-else>
                    {{ formatTime(hours.openTime) }} - {{ formatTime(hours.closeTime) }}
                  </span>
                </div>
              </div>
              
              <div v-if="todaysHours" class="mt-4 p-3 bg-gray-50 rounded-lg">
                <div class="flex items-center">
                  <ClockIcon class="h-4 w-4 text-gray-400 mr-2" />
                  <span class="text-sm text-gray-600">
                    Today: 
                    <span v-if="todaysHours.isClosed" class="text-red-600 font-medium">Closed</span>
                    <span v-else class="font-medium">
                      {{ formatTime(todaysHours.openTime) }} - {{ formatTime(todaysHours.closeTime) }}
                    </span>
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Management Section (for venue owners/managers) -->
      <div v-if="canManageVenue || canManageUsers" class="bg-white rounded-lg shadow-md p-6">
        <div class="flex items-center mb-4">
          <Cog6ToothIcon class="h-6 w-6 text-blue-600 mr-3" />
          <h2 class="text-xl font-bold text-gray-900">Venue Management</h2>
        </div>
        
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <button
            v-if="canManageVenue"
            @click="editVenue"
            class="flex items-center p-4 bg-blue-50 hover:bg-blue-100 rounded-lg border border-blue-200 transition-colors group"
          >
            <PencilIcon class="h-8 w-8 text-blue-600 mr-4" />
            <div class="text-left">
              <h3 class="font-semibold text-gray-900 group-hover:text-blue-900">Edit Venue Details</h3>
              <p class="text-sm text-gray-600">Update venue information, hours, and contact details</p>
            </div>
          </button>
          
          <button
            v-if="canManageUsers"
            @click="manageVenueUsers"
            class="flex items-center p-4 bg-green-50 hover:bg-green-100 rounded-lg border border-green-200 transition-colors group"
          >
            <UsersIcon class="h-8 w-8 text-green-600 mr-4" />
            <div class="text-left">
              <h3 class="font-semibold text-gray-900 group-hover:text-green-900">Manage Users</h3>
              <p class="text-sm text-gray-600">Invite staff and manage permissions</p>
            </div>
          </button>
        </div>
      </div>

      <!-- Active Specials -->
      <div v-if="activeSpecials.length > 0" class="bg-white rounded-lg shadow-md p-6">
        <div class="flex items-center mb-6">
          <StarIcon class="h-6 w-6 text-red-500 mr-3" />
          <h2 class="text-2xl font-bold text-gray-900">Active Specials</h2>
        </div>
        
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          <SpecialCard 
            v-for="special in activeSpecials" 
            :key="special.id"
            :special="special"
            class="hover:shadow-lg transform hover:-translate-y-1 transition-all duration-200"
          />
        </div>
      </div>

      <!-- Map Section -->
      <div class="bg-white rounded-lg shadow-md p-6">
        <div class="flex items-center justify-between mb-4">
          <h2 class="text-2xl font-bold text-gray-900">Location</h2>
          <button
            @click="getDirections"
            class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
          >
            Get Directions
          </button>
        </div>
        
        <div class="bg-gray-100 rounded-lg h-64 flex items-center justify-center">
          <div class="text-center text-gray-500">
            <MapPinIcon class="h-12 w-12 mx-auto mb-2" />
            <p>Interactive map would be displayed here</p>
            <p class="text-sm">{{ venue.streetAddress }}, {{ venue.locality }}</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Not Found State -->
    <div v-else class="text-center py-16">
      <BuildingStorefrontIcon class="h-16 w-16 text-gray-300 mx-auto mb-4" />
      <h3 class="text-lg font-medium text-gray-900 mb-2">Venue not found</h3>
      <p class="text-gray-600 mb-6">The venue you're looking for doesn't exist or has been removed.</p>
      <button
        @click="goBack"
        class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
      >
        Go Back
      </button>
    </div>
  </div>
</template>
