<template>
  <div class="space-y-4">
    <div>
      <h4 class="text-md font-medium text-gray-900">Business Hours</h4>
      <p class="mt-1 text-sm text-gray-500">Set your venue's operating hours for each day of the week.</p>
    </div>
    
    <div class="space-y-3">
      <div 
        v-for="day in daysOfWeek" 
        :key="day.id"
        class="flex flex-col sm:flex-row sm:items-center space-y-3 sm:space-y-0 sm:space-x-4 p-4 border border-gray-200 rounded-lg"
      >
        <!-- Day name and closed toggle section -->
        <div class="flex items-center justify-between sm:justify-start sm:space-x-4">
          <!-- Day name -->
          <div class="w-20 flex-shrink-0">
            <span class="text-sm font-medium text-gray-900">{{ day.name }}</span>
          </div>
          
          <!-- Closed toggle -->
          <div class="flex items-center">
            <input
              :id="`closed-${day.id}`"
              v-model="businessHours[day.id - 1].isClosed"
              type="checkbox"
              :disabled="disabled"
              class="h-4 w-4 text-green-600 focus:ring-green-500 border-gray-300 rounded"
              @change="onClosedChange(day.id - 1)"
            />
            <label :for="`closed-${day.id}`" class="ml-2 block text-sm text-gray-700">
              Closed
            </label>
          </div>
        </div>
        
        <!-- Time inputs -->
        <div v-if="!businessHours[day.id - 1].isClosed" class="w-full sm:flex-1">
          <div class="flex flex-col sm:flex-row sm:items-center space-y-2 sm:space-y-0 sm:space-x-2">
            <div class="flex-1">
              <label :for="`open-${day.id}`" class="block text-xs text-gray-500 mb-1 sm:sr-only">Open</label>
              <input
                :id="`open-${day.id}`"
                v-model="businessHours[day.id - 1].openTime"
                type="time"
                :disabled="disabled"
                class="block w-full border-gray-300 rounded-md shadow-sm focus:ring-green-500 focus:border-green-500 text-sm"
                :class="{ 'bg-gray-50': disabled }"
              />
            </div>
            <span class="text-sm text-gray-500 text-center sm:text-left">to</span>
            <div class="flex-1">
              <label :for="`close-${day.id}`" class="block text-xs text-gray-500 mb-1 sm:sr-only">Close</label>
              <input
                :id="`close-${day.id}`"
                v-model="businessHours[day.id - 1].closeTime"
                type="time"
                :disabled="disabled"
                class="block w-full border-gray-300 rounded-md shadow-sm focus:ring-green-500 focus:border-green-500 text-sm"
                :class="{ 'bg-gray-50': disabled }"
              />
            </div>
          </div>
        </div>
        
        <!-- Closed message -->
        <div v-else class="sm:flex-1 text-sm text-gray-500 italic">
          Closed all day
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, onMounted } from 'vue'
import type { BusinessHours, BusinessHoursRequest } from '@/types/api'

interface Props {
  modelValue?: BusinessHoursRequest[]
  disabled?: boolean
}

interface Emits {
  (e: 'update:modelValue', value: BusinessHoursRequest[]): void
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: () => [],
  disabled: false
})

const emit = defineEmits<Emits>()

const daysOfWeek = [
  { id: 1, name: 'Sunday' },
  { id: 2, name: 'Monday' },
  { id: 3, name: 'Tuesday' },
  { id: 4, name: 'Wednesday' },
  { id: 5, name: 'Thursday' },
  { id: 6, name: 'Friday' },
  { id: 7, name: 'Saturday' }
]

// Initialize business hours for all 7 days
const businessHours = ref<BusinessHoursRequest[]>([
  { dayOfWeekId: 1, openTime: '09:00', closeTime: '17:00', isClosed: false },
  { dayOfWeekId: 2, openTime: '09:00', closeTime: '17:00', isClosed: false },
  { dayOfWeekId: 3, openTime: '09:00', closeTime: '17:00', isClosed: false },
  { dayOfWeekId: 4, openTime: '09:00', closeTime: '17:00', isClosed: false },
  { dayOfWeekId: 5, openTime: '09:00', closeTime: '17:00', isClosed: false },
  { dayOfWeekId: 6, openTime: '09:00', closeTime: '17:00', isClosed: false },
  { dayOfWeekId: 7, openTime: '09:00', closeTime: '17:00', isClosed: false }
])

const onClosedChange = (dayIndex: number) => {
  const day = businessHours.value[dayIndex]
  if (day.isClosed) {
    day.openTime = undefined
    day.closeTime = undefined
  } else {
    day.openTime = '09:00'
    day.closeTime = '17:00'
  }
  emitUpdate()
}

const emitUpdate = () => {
  emit('update:modelValue', [...businessHours.value])
}

// Watch for changes and emit updates
watch(businessHours, () => {
  emitUpdate()
}, { deep: true })

// Initialize from props
const initializeFromProps = () => {
  if (props.modelValue && props.modelValue.length > 0) {
    // Create a map of existing business hours by dayOfWeekId
    const existingHours = new Map()
    props.modelValue.forEach(hour => {
      existingHours.set(hour.dayOfWeekId, hour)
    })
    
    // Create new business hours array
    const newBusinessHours = daysOfWeek.map(day => {
      const existing = existingHours.get(day.id)
      if (existing) {
        return {
          dayOfWeekId: day.id,
          openTime: existing.isClosed ? undefined : existing.openTime,
          closeTime: existing.isClosed ? undefined : existing.closeTime,
          isClosed: existing.isClosed
        }
      } else {
        // Default for missing days
        return {
          dayOfWeekId: day.id,
          openTime: '09:00',
          closeTime: '17:00',
          isClosed: false
        }
      }
    })
    
    // Only update if the data is actually different
    const isEqual = JSON.stringify(businessHours.value) === JSON.stringify(newBusinessHours)
    if (!isEqual) {
      businessHours.value = newBusinessHours
    }
  }
}

onMounted(() => {
  initializeFromProps()
})

// Watch for prop changes
watch(() => props.modelValue, (newValue, oldValue) => {
  // Only re-initialize if the props actually changed and it's not the same reference
  if (newValue !== oldValue) {
    initializeFromProps()
  }
}, { deep: true })
</script>
