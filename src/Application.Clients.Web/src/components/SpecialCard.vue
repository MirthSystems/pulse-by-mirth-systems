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
            <!-- For one-time specials, show the date -->
            <div v-if="!special.isRecurring" class="text-xs">{{ formatDate(special.startDate) }}</div>
            <!-- For recurring specials, show the recurring pattern -->
            <div v-else-if="special.cronSchedule" class="text-xs">{{ formatCronSchedule(special.cronSchedule) }}</div>
            <div class="text-xs">{{ special.startTime }} - {{ special.endTime || 'Late' }}</div>
          </div>
        </div>
        
        <div>
          <span class="font-medium text-gray-700">Type:</span>
          <div class="text-gray-600">
            <div class="text-xs">{{ special.isRecurring ? 'Recurring' : 'One-time' }}</div>
            <div v-if="special.endDate && !special.isRecurring" class="text-xs text-gray-500">
              Ends {{ formatDate(special.endDate) }}
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
  if (!cron || cron.trim() === '') {
    return 'Custom schedule'
  }

  // Parse the CRON expression to extract just the day pattern
  const parts = cron.trim().split(/\s+/)
  if (parts.length < 5) {
    return 'Custom schedule'
  }

  const dayOfWeek = parts[4] // 5th field is day of week (0=Sunday, 1=Monday, etc.)
  
  // Handle specific day patterns
  const dayPatterns: Record<string, string> = {
    // Individual days
    '0': 'Sundays',
    '1': 'Mondays', 
    '2': 'Tuesdays',
    '3': 'Wednesdays',
    '4': 'Thursdays',
    '5': 'Fridays',
    '6': 'Saturdays',
    
    // Common ranges and combinations
    '1-5': 'Weekdays',
    '1,2,3,4,5': 'Weekdays',
    '6,0': 'Weekends',
    '0,6': 'Weekends',
    '5,6': 'Fri & Sat',
    '6,5': 'Fri & Sat',
    '1-6': 'Mon-Sat',
    '2-6': 'Tue-Sat',
    '1-4': 'Mon-Thu',
    '2-5': 'Tue-Fri',
    '3-5': 'Wed-Fri',
    '1,3,5': 'Mon, Wed, Fri',
    '2,4,6': 'Tue, Thu, Sat',
    '1,2,3': 'Mon-Wed',
    '4,5,6': 'Thu-Sat',
    '0,1': 'Sun & Mon',
    '6,0,1': 'Weekends & Mon',
    
    // Every day
    '*': 'Daily'
  }

  return dayPatterns[dayOfWeek] || 'Custom schedule'
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
