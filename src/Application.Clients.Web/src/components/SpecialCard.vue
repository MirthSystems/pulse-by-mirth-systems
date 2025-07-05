<template>
  <div class="bg-white rounded-2xl shadow-sm hover:shadow-lg transition-all duration-300 border cursor-pointer overflow-hidden group relative"
       :class="getBorderColor(special.categoryName)"
       @click="$emit('view-details', special.id)">
    
    <!-- Main Content -->
    <div class="p-4">
      <!-- Header Row -->
      <div class="flex items-start justify-between mb-3">
        <!-- Icon -->
        <span class="text-xl flex-shrink-0">{{ special.categoryIcon }}</span>
        
        <!-- Status Badge -->
        <span 
          class="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium flex-shrink-0"
          :class="getStatusClass(special)"
        >
          {{ getStatusText(special) }}
        </span>
      </div>

      <!-- Title -->
      <div class="mb-3">
        <h3 class="font-medium text-gray-900 text-sm">{{ special.title }}</h3>
      </div>

      <!-- Description -->
      <p class="text-gray-600 text-xs leading-relaxed mb-3">{{ special.description }}</p>

      <!-- Schedule Info - Compact -->
      <div class="flex items-center justify-between text-sm">
        <!-- Time -->
        <div class="flex items-center gap-1 text-gray-800">
          <span>⏰</span>
          <span class="font-semibold">{{ formatTime(special.startTime) }} - {{ special.endTime ? formatTime(special.endTime) : 'Late' }}</span>
        </div>

        <!-- Type (only for one-time) -->
        <div v-if="!special.isRecurring" class="flex items-center gap-1">
          <span class="inline-flex items-center px-2 py-1 rounded-full text-xs font-semibold bg-green-100 text-green-800">
            One-time
          </span>
        </div>
      </div>

      <!-- Day Info for Recurring with badge -->
      <div v-if="special.isRecurring && special.cronSchedule" class="mt-2 flex items-center justify-between">
        <span class="text-xs text-gray-700 font-medium">{{ formatCronSchedule(special.cronSchedule) }}</span>
        <span class="inline-flex items-center px-2 py-1 rounded-full text-xs font-semibold bg-blue-100 text-blue-800">
          Recurring
        </span>
      </div>

      <!-- Distance -->
      <div v-if="special.distanceInMeters" class="mt-2 text-sm text-gray-600">
        📍 {{ formatDistance(special.distanceInMeters) }}
      </div>
    </div>

    <!-- Category Accent Line -->
    <div class="absolute top-0 left-0 right-0 h-1 rounded-t-2xl" :class="getAccentColor(special.categoryName)"></div>
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
    case 'food': return 'border-orange-200 hover:border-orange-300'
    case 'drink': return 'border-blue-200 hover:border-blue-300'
    case 'entertainment': return 'border-purple-200 hover:border-purple-300'
    default: return 'border-gray-200 hover:border-gray-300'
  }
}

function formatTime(timeString: string): string {
  if (!timeString) return ''
  
  // Parse the time string (assuming format like "16:00")
  const [hours, minutes] = timeString.split(':').map(Number)
  
  // Convert to 12-hour format
  const period = hours >= 12 ? 'PM' : 'AM'
  const displayHours = hours === 0 ? 12 : hours > 12 ? hours - 12 : hours
  
  return `${displayHours}:${minutes.toString().padStart(2, '0')} ${period}`
}

function getAccentColor(category: string): string {
  switch (category.toLowerCase()) {
    case 'food': return 'bg-orange-400'
    case 'drink': return 'bg-blue-400'
    case 'entertainment': return 'bg-purple-400'
    default: return 'bg-gray-400'
  }
}

function getStatusClass(special: SpecialSummary): string {
  if (!special.isActive) {
    return 'bg-gray-100 text-gray-700'
  }
  
  if (isSpecialRunningNow(special)) {
    return 'bg-green-100 text-green-700 border border-green-200'
  }
  
  return 'bg-orange-100 text-orange-700 border border-orange-200'
}

function getStatusText(special: SpecialSummary): string {
  if (!special.isActive) {
    return 'Inactive'
  }
  
  if (isSpecialRunningNow(special)) {
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

function isSpecialRunningNow(special: SpecialSummary): boolean {
  if (!special.isActive) {
    return false
  }
  
  const now = new Date()
  const currentDate = now.toISOString().split('T')[0]
  const currentTime = now.toTimeString().slice(0, 5)
  
  // For one-time specials
  if (!special.isRecurring) {
    // Check if we're on the right date
    if (special.startDate !== currentDate) {
      return false
    }
    
    // Check if we're within the time range
    if (special.startTime > currentTime) {
      return false
    }
    
    if (special.endTime && special.endTime < currentTime) {
      return false
    }
    
    return true
  }
  
  // For recurring specials, do basic day-of-week checking
  // This is a simplified implementation that can be improved later
  if (special.cronSchedule) {
    return isRecurringSpecialActive(special, now)
  }
  
  return false
}

function isRecurringSpecialActive(special: SpecialSummary, currentTime: Date): boolean {
  if (!special.cronSchedule) return false
  
  // Simple CRON parsing for day-of-week patterns
  // This handles common patterns like "0 12 * * 1-5" (weekdays at noon)
  const parts = special.cronSchedule.split(' ')
  if (parts.length < 5) return false
  
  const dayOfWeekPattern = parts[4]
  const currentDayOfWeek = currentTime.getDay() // 0 = Sunday, 1 = Monday, etc.
  const currentTimeStr = currentTime.toTimeString().slice(0, 5)
  
  // Check if current day matches the pattern
  if (!matchesDayOfWeek(currentDayOfWeek, dayOfWeekPattern)) {
    return false
  }
  
  // Check if we're within the time range
  if (special.startTime > currentTimeStr) {
    return false
  }
  
  if (special.endTime && special.endTime < currentTimeStr) {
    return false
  }
  
  return true
}

function matchesDayOfWeek(currentDay: number, pattern: string): boolean {
  if (pattern === '*') return true
  
  // Handle specific days (0-6)
  if (pattern === currentDay.toString()) return true
  
  // Handle ranges like "1-5" (Monday to Friday)
  if (pattern.includes('-')) {
    const [start, end] = pattern.split('-').map(Number)
    return currentDay >= start && currentDay <= end
  }
  
  // Handle comma-separated lists like "1,3,5"
  if (pattern.includes(',')) {
    const days = pattern.split(',').map(Number)
    return days.includes(currentDay)
  }
  
  return false
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
