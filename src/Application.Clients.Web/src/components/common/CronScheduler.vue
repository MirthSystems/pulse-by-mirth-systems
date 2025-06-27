<template>
  <div class="cron-scheduler">
    <div class="space-y-4">
      <!-- Quick Presets -->
      <div v-if="showPresets">
        <label class="block text-sm font-medium text-gray-700 mb-2">Quick Presets</label>
        <div class="grid grid-cols-2 gap-2">
          <button
            v-for="preset in cronPresets"
            :key="preset.value"
            @click="selectPreset(preset.value)"
            :disabled="disabled"
            :class="[
              'px-3 py-2 text-sm rounded-md border transition-colors',
              cronExpression === preset.value
                ? 'bg-blue-100 border-blue-300 text-blue-800'
                : 'bg-white border-gray-300 text-gray-700 hover:bg-gray-50',
              disabled ? 'opacity-50 cursor-not-allowed' : ''
            ]"
          >
            {{ preset.label }}
          </button>
        </div>
      </div>

      <!-- Cron Expression Input -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-2">
          Cron Expression
          <span v-if="!isValid" class="text-red-500">*</span>
        </label>
        <div class="relative">
          <input
            v-model="cronExpression"
            type="text"
            :disabled="disabled"
            :class="[
              'w-full px-3 py-2 border rounded-md text-sm font-mono',
              isValid 
                ? 'border-gray-300 focus:border-blue-500 focus:ring-blue-500' 
                : 'border-red-300 focus:border-red-500 focus:ring-red-500',
              disabled ? 'bg-gray-50 cursor-not-allowed' : ''
            ]"
            placeholder="0 18 * * 1-5"
            @input="validateCron"
          />
          <div v-if="!isValid" class="absolute inset-y-0 right-0 pr-3 flex items-center">
            <ExclamationTriangleIcon class="h-5 w-5 text-red-400" />
          </div>
        </div>
        <p class="mt-1 text-xs text-gray-500">
          Format: minute hour day month day-of-week (e.g., "0 18 * * 1-5" = 6 PM weekdays)
        </p>
      </div>

      <!-- Human Readable Description -->
      <div v-if="humanReadable" class="bg-blue-50 border border-blue-200 rounded-md p-3">
        <div class="flex items-start">
          <InformationCircleIcon class="h-5 w-5 text-blue-400 mr-2 mt-0.5 flex-shrink-0" />
          <div>
            <p class="text-sm font-medium text-blue-800">Schedule Description:</p>
            <p class="text-sm text-blue-700">{{ humanReadable }}</p>
          </div>
        </div>
      </div>

      <!-- Next Occurrences -->
      <div v-if="nextOccurrences.length > 0" class="bg-gray-50 border border-gray-200 rounded-md p-3">
        <p class="text-sm font-medium text-gray-900 mb-2">Next {{ nextOccurrences.length }} occurrences:</p>
        <ul class="text-sm text-gray-600 space-y-1">
          <li v-for="occurrence in nextOccurrences" :key="occurrence.getTime()">
            {{ formatDateTime(occurrence) }}
          </li>
        </ul>
      </div>

      <!-- Error Message -->
      <div v-if="error" class="bg-red-50 border border-red-200 rounded-md p-3">
        <div class="flex items-start">
          <ExclamationTriangleIcon class="h-5 w-5 text-red-400 mr-2 mt-0.5 flex-shrink-0" />
          <div>
            <p class="text-sm font-medium text-red-800">Invalid Cron Expression</p>
            <p class="text-sm text-red-700">{{ error }}</p>
          </div>
        </div>
      </div>

      <!-- Visual Builder (Optional) -->
      <div v-if="showBuilder" class="border-t pt-4">
        <h4 class="text-sm font-medium text-gray-900 mb-3">Visual Builder</h4>
        <div class="grid grid-cols-5 gap-4 text-sm">
          <!-- Minute -->
          <div>
            <label class="block text-xs font-medium text-gray-700 mb-1">Minute</label>
            <select 
              v-model="cronParts.minute" 
              @change="buildCron" 
              :disabled="disabled"
              class="w-full text-xs border-gray-300 rounded"
              :class="{ 'bg-gray-50 cursor-not-allowed': disabled }"
            >
              <option value="*">Every minute</option>
              <option value="0">0</option>
              <option value="15">15</option>
              <option value="30">30</option>
              <option value="45">45</option>
            </select>
          </div>
          
          <!-- Hour -->
          <div>
            <label class="block text-xs font-medium text-gray-700 mb-1">Hour</label>
            <select 
              v-model="cronParts.hour" 
              @change="buildCron" 
              :disabled="disabled"
              class="w-full text-xs border-gray-300 rounded"
              :class="{ 'bg-gray-50 cursor-not-allowed': disabled }"
            >
              <option value="*">Every hour</option>
              <option v-for="hour in 24" :key="hour - 1" :value="hour - 1">
                {{ String(hour - 1).padStart(2, '0') }}:00
              </option>
            </select>
          </div>
          
          <!-- Day -->
          <div>
            <label class="block text-xs font-medium text-gray-700 mb-1">Day</label>
            <select 
              v-model="cronParts.day" 
              @change="buildCron" 
              :disabled="disabled"
              class="w-full text-xs border-gray-300 rounded"
              :class="{ 'bg-gray-50 cursor-not-allowed': disabled }"
            >
              <option value="*">Every day</option>
              <option v-for="day in 31" :key="day" :value="day">{{ day }}</option>
            </select>
          </div>
          
          <!-- Month -->
          <div>
            <label class="block text-xs font-medium text-gray-700 mb-1">Month</label>
            <select 
              v-model="cronParts.month" 
              @change="buildCron" 
              :disabled="disabled"
              class="w-full text-xs border-gray-300 rounded"
              :class="{ 'bg-gray-50 cursor-not-allowed': disabled }"
            >
              <option value="*">Every month</option>
              <option v-for="(month, index) in months" :key="index + 1" :value="index + 1">
                {{ month }}
              </option>
            </select>
          </div>
          
          <!-- Day of Week -->
          <div>
            <label class="block text-xs font-medium text-gray-700 mb-1">Day of Week</label>
            <select 
              v-model="cronParts.dayOfWeek" 
              @change="buildCron" 
              :disabled="disabled"
              class="w-full text-xs border-gray-300 rounded"
              :class="{ 'bg-gray-50 cursor-not-allowed': disabled }"
            >
              <option value="*">Every day</option>
              <option value="1-5">Weekdays</option>
              <option value="0,6">Weekends</option>
              <option v-for="(day, index) in daysOfWeek" :key="index" :value="index">
                {{ day }}
              </option>
            </select>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { ExclamationTriangleIcon, InformationCircleIcon } from '@heroicons/vue/24/outline'
import { CronExpressionParser } from 'cron-parser'
import cronstrue from 'cronstrue'
import { format } from 'date-fns'

interface Props {
  modelValue?: string
  startTime?: string
  showPresets?: boolean
  showBuilder?: boolean
  maxOccurrences?: number
  disabled?: boolean
}

interface Emits {
  (e: 'update:modelValue', value: string): void
  (e: 'valid', valid: boolean): void
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: '',
  startTime: '',
  showPresets: true,
  showBuilder: true,
  maxOccurrences: 10,
  disabled: false
})

const emit = defineEmits<Emits>()

const cronExpression = ref(props.modelValue || '')
const isValid = ref(true)
const error = ref('')
const nextOccurrences = ref<Date[]>([])

// Visual builder parts
const cronParts = ref({
  minute: '*',
  hour: '*',
  day: '*',
  month: '*',
  dayOfWeek: '*'
})

const months = [
  'January', 'February', 'March', 'April', 'May', 'June',
  'July', 'August', 'September', 'October', 'November', 'December'
]

const daysOfWeek = [
  'Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'
]

const cronPresets = computed(() => {
  // Parse start time or default to 6 PM (18:00)
  let hour = 18
  let minute = 0
  let timeLabel = '6:00 PM'
  
  if (props.startTime && props.startTime.includes(':')) {
    try {
      const [hourStr, minuteStr] = props.startTime.split(':')
      const parsedHour = parseInt(hourStr)
      const parsedMinute = parseInt(minuteStr)
      
      // Validate the parsed values
      if (!isNaN(parsedHour) && !isNaN(parsedMinute) && 
          parsedHour >= 0 && parsedHour <= 23 && 
          parsedMinute >= 0 && parsedMinute <= 59) {
        hour = parsedHour
        minute = parsedMinute
        
        // Format time for display (12-hour format)
        const date = new Date()
        date.setHours(hour, minute)
        timeLabel = date.toLocaleTimeString('en-US', { 
          hour: 'numeric', 
          minute: '2-digit',
          hour12: true 
        })
      }
    } catch (error) {
      // If parsing fails, keep default values
      console.warn('Invalid start time format:', props.startTime)
    }
  }

  return [
    { label: 'Every minute', value: '* * * * *' },
    { label: 'Every hour', value: '0 * * * *' },
    { label: `Daily at ${timeLabel}`, value: `${minute} ${hour} * * *` },
    { label: `Weekdays at ${timeLabel}`, value: `${minute} ${hour} * * 1-5` },
    { label: `Weekends at ${timeLabel}`, value: `${minute} ${hour} * * 0,6` },
    { label: `Monday at ${timeLabel}`, value: `${minute} ${hour} * * 1` },
    { label: `First of month at ${timeLabel}`, value: `${minute} ${hour} 1 * *` },
    { label: `Every Friday at ${timeLabel}`, value: `${minute} ${hour} * * 5` }
  ]
})

const humanReadable = computed(() => {
  if (!cronExpression.value || !isValid.value) return ''
  
  try {
    return cronstrue.toString(cronExpression.value, { 
      use24HourTimeFormat: true,
      throwExceptionOnParseError: false
    })
  } catch (error) {
    return ''
  }
})

const validateCron = () => {
  const expression = cronExpression.value.trim()
  
  if (!expression) {
    isValid.value = true
    error.value = ''
    nextOccurrences.value = []
    emit('valid', true)
    return
  }

  try {
    const interval = CronExpressionParser.parse(expression)
    isValid.value = true
    error.value = ''
    
    // Generate next occurrences
    const occurrences: Date[] = []
    const tempInterval = CronExpressionParser.parse(expression)
    
    for (let i = 0; i < props.maxOccurrences; i++) {
      try {
        occurrences.push(tempInterval.next().toDate())
      } catch (e) {
        break
      }
    }
    
    nextOccurrences.value = occurrences
    emit('valid', true)
  } catch (err) {
    isValid.value = false
    error.value = err instanceof Error ? err.message : 'Invalid cron expression'
    nextOccurrences.value = []
    emit('valid', false)
  }
}

const selectPreset = (value: string) => {
  cronExpression.value = value
  validateCron()
}

const buildCron = () => {
  const { minute, hour, day, month, dayOfWeek } = cronParts.value
  cronExpression.value = `${minute} ${hour} ${day} ${month} ${dayOfWeek}`
  validateCron()
}

const formatDateTime = (date: Date) => {
  return format(date, 'PPP p') // e.g., "Dec 25, 2024 at 6:00 PM"
}

// Watch for prop changes
watch(() => props.modelValue, (newValue) => {
  cronExpression.value = newValue || ''
  validateCron()
})

// Watch for expression changes and emit
watch(cronExpression, (newValue) => {
  emit('update:modelValue', newValue)
})

// Initialize validation
validateCron()
</script>

<style scoped>
.cron-scheduler {
  font-family: inherit;
}
</style>
