<template>
  <div class="cron-editor">
    <div class="mb-4">
      <label class="block text-sm font-medium text-gray-700 mb-2">
        Schedule Type
      </label>
      <select 
        v-model="scheduleType" 
        @change="onScheduleTypeChange"
        class="block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
      >
        <option value="once">Once</option>
        <option value="daily">Daily</option>
        <option value="weekly">Weekly</option>
        <option value="monthly">Monthly</option>
        <option value="custom">Custom Cron</option>
      </select>
    </div>

    <!-- Date/Time Selection for "Once" -->
    <div v-if="scheduleType === 'once'" class="grid grid-cols-2 gap-4 mb-4">
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-2">Date</label>
        <input
          v-model="onceDate"
          type="date"
          class="block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
        />
      </div>
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-2">Time</label>
        <input
          v-model="onceTime"
          type="time"
          class="block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
        />
      </div>
    </div>

    <!-- Time Selection for Daily -->
    <div v-if="scheduleType === 'daily'" class="mb-4">
      <label class="block text-sm font-medium text-gray-700 mb-2">Time</label>
      <input
        v-model="dailyTime"
        type="time"
        class="block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
      />
    </div>

    <!-- Weekly Schedule -->
    <div v-if="scheduleType === 'weekly'" class="mb-4">
      <label class="block text-sm font-medium text-gray-700 mb-2">Days of Week</label>
      <div class="grid grid-cols-7 gap-2 mb-3">
        <label 
          v-for="(day, index) in daysOfWeek" 
          :key="index"
          class="flex flex-col items-center"
        >
          <input
            v-model="selectedDays"
            :value="index"
            type="checkbox"
            class="mb-1"
          />
          <span class="text-xs">{{ day }}</span>
        </label>
      </div>
      <label class="block text-sm font-medium text-gray-700 mb-2">Time</label>
      <input
        v-model="weeklyTime"
        type="time"
        class="block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
      />
    </div>

    <!-- Monthly Schedule -->
    <div v-if="scheduleType === 'monthly'" class="mb-4">
      <div class="grid grid-cols-2 gap-4 mb-3">
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">Day of Month</label>
          <input
            v-model.number="monthlyDay"
            type="number"
            min="1"
            max="31"
            class="block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
          />
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">Time</label>
          <input
            v-model="monthlyTime"
            type="time"
            class="block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
          />
        </div>
      </div>
    </div>

    <!-- Custom Cron -->
    <div v-if="scheduleType === 'custom'" class="mb-4">
      <label class="block text-sm font-medium text-gray-700 mb-2">
        Cron Expression
        <span class="text-xs text-gray-500">(minute hour day month dayofweek)</span>
      </label>
      <input
        v-model="customCron"
        type="text"
        placeholder="0 9 * * 1-5"
        class="block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
      />
      <p class="text-xs text-gray-500 mt-1">
        Example: "0 9 * * 1-5" = Every weekday at 9:00 AM
      </p>
    </div>

    <!-- Generated Cron Expression Display -->
    <div class="bg-gray-50 p-3 rounded-md">
      <div class="flex justify-between items-center mb-2">
        <span class="text-sm font-medium text-gray-700">Generated Cron:</span>
        <button
          @click="copyToClipboard"
          class="text-xs text-blue-600 hover:text-blue-800"
        >
          Copy
        </button>
      </div>
      <code class="text-sm text-gray-900">{{ generatedCron }}</code>
      <div v-if="cronDescription" class="text-xs text-gray-600 mt-1">
        {{ cronDescription }}
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import cronstrue from 'cronstrue'

interface Props {
  modelValue?: string
}

interface Emits {
  (e: 'update:modelValue', value: string): void
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: '0 9 * * *' // Default: daily at 9 AM
})

const emit = defineEmits<Emits>()

const scheduleType = ref<'once' | 'daily' | 'weekly' | 'monthly' | 'custom'>('daily')
const onceDate = ref('')
const onceTime = ref('09:00')
const dailyTime = ref('09:00')
const weeklyTime = ref('09:00')
const monthlyTime = ref('09:00')
const monthlyDay = ref(1)
const customCron = ref(props.modelValue)
const selectedDays = ref<number[]>([1, 2, 3, 4, 5]) // Mon-Fri

const daysOfWeek = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat']

const generatedCron = computed(() => {
  try {
    switch (scheduleType.value) {
      case 'once': {
        if (!onceDate.value || !onceTime.value) return '0 9 * * *'
        const date = new Date(`${onceDate.value}T${onceTime.value}`)
        return `${date.getMinutes()} ${date.getHours()} ${date.getDate()} ${date.getMonth() + 1} *`
      }
      case 'daily': {
        const [hour, minute] = dailyTime.value.split(':')
        return `${minute} ${hour} * * *`
      }
      case 'weekly': {
        if (selectedDays.value.length === 0) return '0 9 * * *'
        const [hour, minute] = weeklyTime.value.split(':')
        const days = selectedDays.value.join(',')
        return `${minute} ${hour} * * ${days}`
      }
      case 'monthly': {
        const [hour, minute] = monthlyTime.value.split(':')
        return `${minute} ${hour} ${monthlyDay.value} * *`
      }
      case 'custom':
        return customCron.value
      default:
        return '0 9 * * *'
    }
  } catch (error) {
    return '0 9 * * *'
  }
})

const cronDescription = computed(() => {
  try {
    return cronstrue.toString(generatedCron.value)
  } catch (error) {
    return 'Invalid cron expression'
  }
})

const onScheduleTypeChange = () => {
  // Reset values when schedule type changes
  selectedDays.value = [1, 2, 3, 4, 5]
  monthlyDay.value = 1
}

const copyToClipboard = async () => {
  try {
    await navigator.clipboard.writeText(generatedCron.value)
  } catch (error) {
    console.warn('Failed to copy to clipboard:', error)
  }
}

// Watch for changes and emit
watch(generatedCron, (newValue) => {
  emit('update:modelValue', newValue)
}, { immediate: true })

// Initialize component with provided value
if (props.modelValue && props.modelValue !== '0 9 * * *') {
  customCron.value = props.modelValue
  scheduleType.value = 'custom'
}
</script>
