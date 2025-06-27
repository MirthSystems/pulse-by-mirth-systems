<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { 
  CheckCircleIcon,
  ExclamationTriangleIcon,
  InformationCircleIcon,
  XMarkIcon
} from '@heroicons/vue/24/outline'

interface Props {
  type?: 'success' | 'error' | 'warning' | 'info'
  title: string
  message?: string
  duration?: number
  closable?: boolean
}

interface Emits {
  (e: 'close'): void
}

const props = withDefaults(defineProps<Props>(), {
  type: 'info',
  duration: 5000,
  closable: true
})

const emit = defineEmits<Emits>()

const show = ref(true)

const typeConfig = {
  success: {
    icon: CheckCircleIcon,
    bgClass: 'bg-green-50',
    borderClass: 'border-green-200',
    iconClass: 'text-green-400',
    titleClass: 'text-green-800',
    messageClass: 'text-green-700'
  },
  error: {
    icon: ExclamationTriangleIcon,
    bgClass: 'bg-red-50',
    borderClass: 'border-red-200',
    iconClass: 'text-red-400',
    titleClass: 'text-red-800',
    messageClass: 'text-red-700'
  },
  warning: {
    icon: ExclamationTriangleIcon,
    bgClass: 'bg-yellow-50',
    borderClass: 'border-yellow-200',
    iconClass: 'text-yellow-400',
    titleClass: 'text-yellow-800',
    messageClass: 'text-yellow-700'
  },
  info: {
    icon: InformationCircleIcon,
    bgClass: 'bg-blue-50',
    borderClass: 'border-blue-200',
    iconClass: 'text-blue-400',
    titleClass: 'text-blue-800',
    messageClass: 'text-blue-700'
  }
}

const config = typeConfig[props.type]

const close = () => {
  show.value = false
  emit('close')
}

onMounted(() => {
  if (props.duration > 0) {
    setTimeout(() => {
      close()
    }, props.duration)
  }
})
</script>

<template>
  <Transition
    enter-active-class="transform ease-out duration-300 transition"
    enter-from-class="translate-y-2 opacity-0 sm:translate-y-0 sm:translate-x-2"
    enter-to-class="translate-y-0 opacity-100 sm:translate-x-0"
    leave-active-class="transition ease-in duration-100"
    leave-from-class="opacity-100"
    leave-to-class="opacity-0"
  >
    <div
      v-if="show"
      :class="[
        'max-w-sm w-full shadow-lg rounded-lg pointer-events-auto ring-1 ring-black ring-opacity-5 overflow-hidden',
        config.bgClass,
        config.borderClass
      ]"
    >
      <div class="p-4">
        <div class="flex items-start">
          <div class="flex-shrink-0">
            <component :is="config.icon" :class="['h-6 w-6', config.iconClass]" />
          </div>
          <div class="ml-3 w-0 flex-1 pt-0.5">
            <p :class="['text-sm font-medium', config.titleClass]">
              {{ title }}
            </p>
            <p v-if="message" :class="['mt-1 text-sm', config.messageClass]">
              {{ message }}
            </p>
          </div>
          <div v-if="closable" class="ml-4 flex-shrink-0 flex">
            <button
              @click="close"
              :class="[
                'rounded-md inline-flex focus:outline-none focus:ring-2 focus:ring-offset-2',
                config.titleClass,
                'hover:' + config.titleClass
              ]"
            >
              <span class="sr-only">Close</span>
              <XMarkIcon class="h-5 w-5" />
            </button>
          </div>
        </div>
      </div>
    </div>
  </Transition>
</template>
