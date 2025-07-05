<template>
  <div class="bg-white rounded-lg shadow-md hover:shadow-lg transition-all duration-300 border-l-4 relative"
       :class="getBorderColor(special.categoryName)">
    <div class="p-3 sm:p-4 pb-12 sm:pb-4">
      <div class="flex flex-col space-y-3 sm:flex-row sm:items-start sm:justify-between sm:space-y-0 mb-3">
        <div class="flex items-center space-x-3 min-w-0 flex-1">
          <span class="text-xl sm:text-2xl flex-shrink-0">{{ special.categoryIcon }}</span>
          <div class="min-w-0 flex-1">
            <h3 class="text-sm sm:text-base font-bold text-gray-900 break-words">{{ special.title }}</h3>
            <p class="text-xs sm:text-sm text-gray-600">{{ special.categoryName }}</p>
          </div>
        </div>
        
        <div class="flex items-center justify-between sm:flex-col sm:items-end sm:space-y-1">
          <span 
            class="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium"
            :class="getStatusClass(special)"
          >
            {{ getStatusText(special) }}
          </span>
          <div v-if="special.isRecurring" class="text-xs text-blue-600 font-medium">
            Recurring
          </div>
        </div>
      </div>
      
      <p class="text-gray-700 text-xs sm:text-sm mb-3 break-words">{{ special.description || 'No description' }}</p>
      
      <div class="space-y-2 text-xs sm:text-sm">
        <div>
          <span class="font-medium text-gray-700">Schedule:</span>
          <div class="text-gray-600 mt-1">
            <div class="break-words">
              {{ special.startDate }} {{ special.startTime }}
              <span v-if="special.endTime"> - {{ special.endTime }}</span>
            </div>
            <div v-if="special.isRecurring" class="text-blue-600 mt-1">
              {{ formatCronSchedule(special.cronSchedule || '') }}
            </div>
            <div v-if="special.endDate && !special.isRecurring" class="text-gray-500 mt-1">
              Ends {{ formatDate(special.endDate) }}
            </div>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Action buttons -->
    <div class="absolute bottom-2 right-2 sm:bottom-3 sm:right-3 flex items-center space-x-1 sm:space-x-2">
      <button
        @click="$emit('edit', special)"
        class="p-1.5 sm:p-2 text-blue-600 hover:text-blue-800 hover:bg-blue-50 rounded-full transition-colors"
        title="Edit special"
      >
        <PencilIcon class="h-3 w-3 sm:h-4 sm:w-4" />
      </button>
      <button
        @click="$emit('delete', special)"
        class="p-1.5 sm:p-2 text-red-600 hover:text-red-800 hover:bg-red-50 rounded-full transition-colors"
        title="Delete special"
      >
        <TrashIcon class="h-3 w-3 sm:h-4 sm:w-4" />
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { SpecialSummary } from '@/types/api'
import { format } from 'date-fns'
import { PencilIcon, TrashIcon } from '@heroicons/vue/24/outline'

interface Props {
  special: SpecialSummary
}

defineProps<Props>()

defineEmits<{
  'edit': [special: SpecialSummary]
  'delete': [special: SpecialSummary]
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
  if (!special.isActive) {
    return 'bg-gray-100 text-gray-800'
  }
  
  if (isSpecialRunningNow(special)) {
    return 'bg-green-100 text-green-800'
  }
  
  return 'bg-yellow-100 text-yellow-800'
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
