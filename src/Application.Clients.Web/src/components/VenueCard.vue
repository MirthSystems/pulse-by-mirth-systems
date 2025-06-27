<template>
  <div class="bg-white rounded-xl shadow-lg hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1">
    <div class="relative">
      <img 
        :src="venue.profileImage || '/api/placeholder/400/200'" 
        :alt="venue.name"
        class="w-full h-48 object-cover rounded-t-xl"
      />
      <div class="absolute top-4 left-4">
        <span class="inline-flex items-center px-3 py-1 rounded-full text-xs font-medium bg-white/90 text-gray-800">
          {{ venue.categoryIcon }} {{ venue.categoryName }}
        </span>
      </div>
      <div v-if="venue.activeSpecialsCount > 0" class="absolute top-4 right-4">
        <span class="inline-flex items-center px-3 py-1 rounded-full text-xs font-medium bg-red-500 text-white">
          {{ venue.activeSpecialsCount }} Active {{ venue.activeSpecialsCount === 1 ? 'Special' : 'Specials' }}
        </span>
      </div>
    </div>
    
    <div class="p-6">
      <div class="flex items-start justify-between mb-3">
        <h3 class="text-xl font-bold text-gray-900 truncate">{{ venue.name }}</h3>
        <div v-if="venue.distanceInMeters" class="text-sm text-gray-500 ml-2">
          {{ formatDistance(venue.distanceInMeters) }}
        </div>
      </div>
      
      <p class="text-gray-600 text-sm mb-4 line-clamp-2">{{ venue.description }}</p>
      
      <div class="space-y-2 mb-4">
        <div class="flex items-center text-sm text-gray-600">
          <MapPinIcon class="h-4 w-4 mr-2 text-gray-400" />
          {{ venue.streetAddress }}, {{ venue.locality }}, {{ venue.region }}
        </div>
        
        <div v-if="venue.phoneNumber" class="flex items-center text-sm text-gray-600">
          <PhoneIcon class="h-4 w-4 mr-2 text-gray-400" />
          {{ venue.phoneNumber }}
        </div>
        
        <div v-if="venue.website" class="flex items-center text-sm text-gray-600">
          <GlobeAltIcon class="h-4 w-4 mr-2 text-gray-400" />
          <a :href="venue.website" target="_blank" class="text-blue-600 hover:text-blue-800 truncate">
            {{ venue.website }}
          </a>
        </div>
      </div>
      
      <div class="flex items-center justify-between pt-4 border-t border-gray-100">
        <button
          @click="$emit('view-details', venue.id)"
          class="inline-flex items-center px-4 py-2 bg-blue-600 text-white text-sm font-medium rounded-lg hover:bg-blue-700 transition-colors"
        >
          <EyeIcon class="h-4 w-4 mr-2" />
          View Details
        </button>
        
        <div class="flex space-x-2">
          <button
            @click="$emit('get-directions', venue)"
            class="p-2 text-gray-400 hover:text-gray-600 transition-colors"
            title="Get Directions"
          >
            <MapIcon class="h-5 w-5" />
          </button>
          
          <button
            @click="$emit('view-specials', venue.id)"
            class="p-2 text-gray-400 hover:text-gray-600 transition-colors"
            title="View Specials"
          >
            <StarIcon class="h-5 w-5" />
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { MapPinIcon, PhoneIcon, GlobeAltIcon, EyeIcon, MapIcon, StarIcon } from '@heroicons/vue/24/outline'
import type { VenueSummary } from '@/types/api'

interface Props {
  venue: VenueSummary
}

defineProps<Props>()

defineEmits<{
  'view-details': [id: number]
  'get-directions': [venue: VenueSummary]
  'view-specials': [id: number]
}>()

function formatDistance(meters: number): string {
  if (meters < 1000) {
    return `${Math.round(meters)}m`
  } else {
    return `${(meters / 1000).toFixed(1)}km`
  }
}
</script>

<style scoped>
.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>
