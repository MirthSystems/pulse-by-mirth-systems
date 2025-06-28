<template>
  <div class="bg-white rounded-lg shadow-md hover:shadow-lg transition-all duration-300 border-l-4 cursor-pointer"
       :class="getBorderColor(special.categoryName)"
       @click="$emit('view-details', special.id)">
    <div class="p-4">
      <div class="flex items-start justify-between mb-3">
        <div class="flex items-center space-x-3">
          <span class="text-2xl">{{ special.categoryIcon }}</span>
          <div>
            <h3 class="text-base font-bold text-gray-900">{{ special.title }}</h3>
            <p class="text-sm text-gray-600">{{ special.venueName }}</p>
          </div>
        </div>
        
        <div class="flex flex-col items-end space-y-1">
          <span 
            class="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium"
            :class="getStatusClass(special)"
          >
            {{ getStatusText(special) }}
          </span>
          <span v-if="special.distanceInMeters" class="text-xs text-gray-500">
            {{ formatDistance(special.distanceInMeters) }}
          </span>
        </div>
      </div>
      
      <p class="text-gray-700 text-sm mb-3 line-clamp-2">{{ special.description }}</p>
      
      <div class="grid grid-cols-2 gap-3 text-sm">
        <div>
          <span class="font-medium text-gray-700">When:</span>
          <div class="text-gray-600">
            <div class="text-xs">{{ formatDate(special.startDate) }}</div>
            <div class="text-xs">{{ special.startTime }} - {{ special.endTime || 'Late' }}</div>
          </div>
        </div>
        
        <div>
          <span class="font-medium text-gray-700">Type:</span>
          <div class="text-gray-600">
            <div class="text-xs">{{ special.isRecurring ? 'Recurring' : 'One-time' }}</div>
            <div v-if="special.cronSchedule" class="text-xs text-gray-500">
              {{ formatCronSchedule(special.cronSchedule) }}
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { SpecialSummary } from '@/types/api'
import { format } from 'date-fns'

interface Props {
  special: SpecialSummary
}

defineProps<Props>()

defineEmits<{
  'view-details': [id: number]
}>()

function getBorderColor(category: string): string {
  switch (category.toLowerCase()) {
    case 'food': return 'border-l-orange-500'
    case 'drink': return 'border-l-blue-500'
    case 'entertainment': return 'border-l-purple-500'
    default: return 'border-l-gray-500'
  }
}

function getStatusClass(special: SpecialSummary): string {
  const now = new Date()
  const today = now.toISOString().split('T')[0]
  const currentTime = now.toTimeString().slice(0, 5)
  
  if (!special.isActive) {
    return 'bg-gray-100 text-gray-800'
  }
  
  if (special.startDate <= today && 
      (!special.endDate || special.endDate >= today) &&
      special.startTime <= currentTime &&
      (!special.endTime || special.endTime >= currentTime)) {
    return 'bg-green-100 text-green-800'
  }
  
  return 'bg-yellow-100 text-yellow-800'
}

function getStatusText(special: SpecialSummary): string {
  const now = new Date()
  const today = now.toISOString().split('T')[0]
  const currentTime = now.toTimeString().slice(0, 5)
  
  if (!special.isActive) {
    return 'Inactive'
  }
  
  if (special.startDate <= today && 
      (!special.endDate || special.endDate >= today) &&
      special.startTime <= currentTime &&
      (!special.endTime || special.endTime >= currentTime)) {
    return 'Active Now'
  }
  
  return 'Scheduled'
}

function formatDistance(meters: number): string {
  if (meters < 1000) {
    return `${Math.round(meters)}m`
  } else {
    return `${(meters / 1000).toFixed(1)}km`
  }
}

function formatDate(dateString: string): string {
  try {
    return format(new Date(dateString), 'MMM d, yyyy')
  } catch {
    return dateString
  }
}

function formatCronSchedule(cron: string): string {
  // Simple CRON schedule descriptions
  const patterns: Record<string, string> = {
    '0 16 * * 1-5': 'Weekdays at 4:00 PM',
    '0 21 * * 5,6': 'Fri & Sat at 9:00 PM',
    '0 21 * * 3': 'Wednesdays at 9:00 PM',
    '0 11 * * 2': 'Tuesdays at 11:00 AM',
    '0 16 * * 2-6': 'Tue-Sat at 4:00 PM',
    '0 10 * * 0': 'Sundays at 10:00 AM',
    '* * * * *': 'Every minute',
    '0 19 * * 4': 'Thursdays at 7:00 PM',
    '* 19 * * 4': 'Thursdays 7:00-7:59 PM'
  }
  
  return patterns[cron] || 'Custom schedule'
}
</script>

<style scoped>
.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>
