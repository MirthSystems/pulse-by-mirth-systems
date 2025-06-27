<script setup lang="ts">
import { onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useSpecialStore } from '../stores/special'
import { 
  StarIcon,
  BuildingStorefrontIcon,
  ClockIcon,
  CalendarIcon,
  ShareIcon,
  HeartIcon,
  ArrowLeftIcon,
  MapPinIcon,
  PhoneIcon,
  FireIcon,
  CheckCircleIcon,
  XCircleIcon
} from '@heroicons/vue/24/outline'
import { format, parseISO, isValid } from 'date-fns'

interface Props {
  id: string
}

const props = defineProps<Props>()
const route = useRoute()
const router = useRouter()
const specialStore = useSpecialStore()

const special = computed(() => specialStore.selectedSpecial)
const loading = computed(() => specialStore.loading)
const error = computed(() => specialStore.error)

const specialId = computed(() => parseInt(props.id))

const formatDate = (dateString: string) => {
  try {
    const date = parseISO(dateString)
    if (!isValid(date)) return dateString
    return format(date, 'MMMM d, yyyy')
  } catch {
    return dateString
  }
}

const formatTime = (timeString: string) => {
  try {
    const [hours, minutes] = timeString.split(':').map(Number)
    const date = new Date()
    date.setHours(hours, minutes, 0, 0)
    return format(date, 'h:mm a')
  } catch {
    return timeString
  }
}

const getScheduleDescription = (special: any) => {
  if (!special.isRecurring) {
    const startDate = formatDate(special.startDate)
    if (special.endDate) {
      const endDate = formatDate(special.endDate)
      return `${startDate} - ${endDate}`
    }
    return startDate
  }
  
  if (special.cronSchedule) {
    // This would parse the cron schedule into human readable format
    // For now, return a simple description
    return 'Recurring schedule - check venue for specific times'
  }
  
  return 'Ongoing special'
}

const getTimeDescription = (special: any) => {
  const startTime = formatTime(special.startTime)
  if (special.endTime) {
    const endTime = formatTime(special.endTime)
    return `${startTime} - ${endTime}`
  }
  return `Starting at ${startTime}`
}

const goBack = () => {
  router.back()
}

const shareSpecial = async () => {
  if (navigator.share && special.value) {
    try {
      await navigator.share({
        title: special.value.title,
        text: special.value.description,
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

const goToVenue = () => {
  if (special.value) {
    router.push(`/venues/${special.value.venueId}`)
  }
}

const getDirections = () => {
  if (special.value?.venue) {
    const address = encodeURIComponent(
      `${special.value.venue.streetAddress}, ${special.value.venue.locality}, ${special.value.venue.region}`
    )
    window.open(`https://maps.google.com/maps?q=${address}`, '_blank')
  }
}

onMounted(async () => {
  if (specialId.value) {
    await specialStore.fetchSpecial(specialId.value)
  }
})
</script>

<template>
  <div>
    <!-- Loading State -->
    <div v-if="loading" class="space-y-6">
      <div class="bg-white rounded-lg shadow-md p-6 animate-pulse">
        <div class="bg-gray-300 h-48 rounded-lg mb-6"></div>
        <div class="bg-gray-300 h-8 rounded mb-4"></div>
        <div class="bg-gray-300 h-4 rounded mb-2"></div>
        <div class="bg-gray-300 h-4 rounded w-2/3"></div>
      </div>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="bg-red-50 border border-red-200 rounded-lg p-6">
      <h3 class="text-lg font-medium text-red-800 mb-2">Error loading special</h3>
      <p class="text-red-700 mb-4">{{ error }}</p>
      <div class="flex space-x-4">
        <button 
          @click="specialStore.fetchSpecial(specialId)"
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

    <!-- Special Details -->
    <div v-else-if="special" class="space-y-6">
      <!-- Header -->
      <div class="bg-white rounded-lg shadow-md overflow-hidden">
        <div class="relative bg-gradient-to-r from-red-500 to-pink-600 text-white p-8">
          <div class="absolute inset-0 bg-black/10"></div>
          
          <!-- Back Button -->
          <button
            @click="goBack"
            class="absolute top-4 left-4 p-2 bg-white/90 hover:bg-white text-gray-900 rounded-full transition-colors"
          >
            <ArrowLeftIcon class="h-5 w-5" />
          </button>
          
          <!-- Action Buttons -->
          <div class="absolute top-4 right-4 flex space-x-2">
            <button
              @click="shareSpecial"
              class="p-2 bg-white/90 hover:bg-white text-gray-900 rounded-full transition-colors"
            >
              <ShareIcon class="h-5 w-5" />
            </button>
            <button class="p-2 bg-white/90 hover:bg-white text-gray-900 rounded-full transition-colors">
              <HeartIcon class="h-5 w-5" />
            </button>
          </div>
          
          <div class="relative pt-8">
            <div class="flex items-center mb-4">
              <div class="p-3 bg-white/20 rounded-full mr-4">
                <FireIcon class="h-8 w-8 text-yellow-300" />
              </div>
              <div>
                <div class="flex items-center mb-2">
                  <span class="text-lg">{{ special.categoryIcon }}</span>
                  <span class="ml-2 text-sm font-medium text-red-100">{{ special.categoryName }}</span>
                </div>
                <div :class="[
                  'inline-flex items-center px-3 py-1 rounded-full text-sm font-medium',
                  special.isActive 
                    ? 'bg-green-100 text-green-800' 
                    : 'bg-red-100 text-red-800'
                ]">
                  <component 
                    :is="special.isActive ? CheckCircleIcon : XCircleIcon" 
                    class="h-4 w-4 mr-1" 
                  />
                  {{ special.isActive ? 'Active Now' : 'Not Active' }}
                </div>
              </div>
            </div>
            
            <h1 class="text-4xl font-bold mb-3">{{ special.title }}</h1>
            <p class="text-xl text-red-100 leading-relaxed">{{ special.description }}</p>
          </div>
        </div>
      </div>

      <!-- Special Details Grid -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <!-- Main Content -->
        <div class="lg:col-span-2 space-y-6">
          <!-- Schedule Information -->
          <div class="bg-white rounded-lg shadow-md p-6">
            <h2 class="text-2xl font-bold text-gray-900 mb-6">Schedule & Timing</h2>
            
            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <div class="flex items-center mb-3">
                  <CalendarIcon class="h-5 w-5 text-gray-400 mr-3" />
                  <h3 class="text-lg font-semibold text-gray-900">Dates</h3>
                </div>
                <p class="text-gray-600">{{ getScheduleDescription(special) }}</p>
                
                <div v-if="special.isRecurring" class="mt-3 p-3 bg-blue-50 rounded-lg">
                  <div class="flex items-center">
                    <CalendarIcon class="h-4 w-4 text-blue-500 mr-2" />
                    <span class="text-sm text-blue-700 font-medium">Recurring Special</span>
                  </div>
                  <p class="text-sm text-blue-600 mt-1">This special runs on a recurring schedule</p>
                </div>
              </div>
              
              <div>
                <div class="flex items-center mb-3">
                  <ClockIcon class="h-5 w-5 text-gray-400 mr-3" />
                  <h3 class="text-lg font-semibold text-gray-900">Time</h3>
                </div>
                <p class="text-gray-600">{{ getTimeDescription(special) }}</p>
                
                <div v-if="special.cronSchedule" class="mt-3 p-3 bg-gray-50 rounded-lg">
                  <p class="text-sm text-gray-600">
                    <span class="font-medium">Schedule:</span> {{ special.cronSchedule }}
                  </p>
                </div>
              </div>
            </div>
          </div>

          <!-- Terms & Conditions -->
          <div class="bg-white rounded-lg shadow-md p-6">
            <h2 class="text-2xl font-bold text-gray-900 mb-4">Terms & Conditions</h2>
            
            <div class="prose prose-sm text-gray-600">
              <ul class="space-y-2">
                <li>Special is subject to availability</li>
                <li>Cannot be combined with other offers</li>
                <li>Valid for dine-in only unless otherwise specified</li>
                <li>Management reserves the right to modify or cancel special</li>
                <li>Please present this special to your server</li>
              </ul>
            </div>
            
            <div class="mt-6 p-4 bg-yellow-50 border border-yellow-200 rounded-lg">
              <div class="flex items-start">
                <div class="flex-shrink-0">
                  <ClockIcon class="h-5 w-5 text-yellow-600" />
                </div>
                <div class="ml-3">
                  <h4 class="text-sm font-medium text-yellow-800">Important</h4>
                  <p class="text-sm text-yellow-700 mt-1">
                    Special availability and timing may vary. Please call the venue to confirm current offers.
                  </p>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Sidebar -->
        <div class="space-y-6">
          <!-- Venue Information -->
          <div class="bg-white rounded-lg shadow-md p-6">
            <div class="flex items-center mb-4">
              <BuildingStorefrontIcon class="h-6 w-6 text-blue-600 mr-3" />
              <h3 class="text-lg font-semibold text-gray-900">Venue</h3>
            </div>
            
            <button
              @click="goToVenue"
              class="w-full text-left group hover:bg-gray-50 rounded-lg p-3 transition-colors"
            >
              <h4 class="text-lg font-bold text-blue-600 group-hover:text-blue-800 mb-2">
                {{ special.venueName }}
              </h4>
              
              <div v-if="special.venue" class="space-y-3">
                <div class="flex items-start">
                  <MapPinIcon class="h-4 w-4 text-gray-400 mr-2 mt-0.5 flex-shrink-0" />
                  <div class="text-sm text-gray-600">
                    {{ special.venue.streetAddress }}<br>
                    <span v-if="special.venue.secondaryAddress">{{ special.venue.secondaryAddress }}<br></span>
                    {{ special.venue.locality }}, {{ special.venue.region }}
                  </div>
                </div>
                
                <div v-if="special.venue.phoneNumber" class="flex items-center">
                  <PhoneIcon class="h-4 w-4 text-gray-400 mr-2" />
                  <span class="text-sm text-gray-600">{{ special.venue.phoneNumber }}</span>
                </div>
                
                <div v-if="special.venue.distanceInMeters" class="flex items-center">
                  <MapPinIcon class="h-4 w-4 text-gray-400 mr-2" />
                  <span class="text-sm text-gray-600">
                    {{ (special.venue.distanceInMeters / 1000).toFixed(1) }} km away
                  </span>
                </div>
              </div>
            </button>
            
            <div class="mt-4 flex space-x-2">
              <button
                @click="goToVenue"
                class="flex-1 px-4 py-2 bg-blue-600 text-white text-sm font-medium rounded-lg hover:bg-blue-700 transition-colors"
              >
                View Venue
              </button>
              <button
                v-if="special.venue"
                @click="getDirections"
                class="px-4 py-2 bg-gray-100 text-gray-700 text-sm font-medium rounded-lg hover:bg-gray-200 transition-colors"
              >
                Directions
              </button>
            </div>
          </div>

          <!-- Quick Actions -->
          <div class="bg-white rounded-lg shadow-md p-6">
            <h3 class="text-lg font-semibold text-gray-900 mb-4">Quick Actions</h3>
            
            <div class="space-y-3">
              <button
                v-if="special.venue?.phoneNumber"
                :href="`tel:${special.venue.phoneNumber}`"
                class="w-full flex items-center justify-center px-4 py-3 bg-green-600 text-white font-medium rounded-lg hover:bg-green-700 transition-colors"
              >
                <PhoneIcon class="h-5 w-5 mr-2" />
                Call Venue
              </button>
              
              <button
                @click="shareSpecial"
                class="w-full flex items-center justify-center px-4 py-3 bg-blue-600 text-white font-medium rounded-lg hover:bg-blue-700 transition-colors"
              >
                <ShareIcon class="h-5 w-5 mr-2" />
                Share Special
              </button>
              
              <button
                v-if="special.venue"
                @click="getDirections"
                class="w-full flex items-center justify-center px-4 py-3 bg-gray-600 text-white font-medium rounded-lg hover:bg-gray-700 transition-colors"
              >
                <MapPinIcon class="h-5 w-5 mr-2" />
                Get Directions
              </button>
            </div>
          </div>

          <!-- Special Category -->
          <div class="bg-white rounded-lg shadow-md p-6">
            <div class="flex items-center mb-4">
              <StarIcon class="h-6 w-6 text-red-500 mr-3" />
              <h3 class="text-lg font-semibold text-gray-900">Category</h3>
            </div>
            
            <div class="flex items-center p-3 bg-gray-50 rounded-lg">
              <span class="text-2xl mr-3">{{ special.categoryIcon }}</span>
              <div>
                <h4 class="font-medium text-gray-900">{{ special.categoryName }}</h4>
                <p class="text-sm text-gray-600">Special category</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Not Found State -->
    <div v-else class="text-center py-16">
      <StarIcon class="h-16 w-16 text-gray-300 mx-auto mb-4" />
      <h3 class="text-lg font-medium text-gray-900 mb-2">Special not found</h3>
      <p class="text-gray-600 mb-6">The special you're looking for doesn't exist or has been removed.</p>
      <button
        @click="goBack"
        class="px-4 py-2 bg-red-500 text-white rounded-lg hover:bg-red-600 transition-colors"
      >
        Go Back
      </button>
    </div>
  </div>
</template>
