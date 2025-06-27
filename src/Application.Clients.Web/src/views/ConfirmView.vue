<template>
  <div class="min-h-screen bg-gray-50 flex flex-col justify-center py-12 sm:px-6 lg:px-8">
    <div class="sm:mx-auto sm:w-full sm:max-w-md">
      <div class="bg-white py-8 px-4 shadow sm:rounded-lg sm:px-10">
        <div class="sm:flex sm:items-start">
          <div class="mx-auto flex-shrink-0 flex items-center justify-center h-12 w-12 rounded-full bg-red-100 sm:mx-0 sm:h-10 sm:w-10">
            <ExclamationTriangleIcon class="h-6 w-6 text-red-600" aria-hidden="true" />
          </div>
          <div class="mt-3 text-center sm:mt-0 sm:ml-4 sm:text-left">
            <h3 class="text-lg leading-6 font-medium text-gray-900">
              {{ title }}
            </h3>
            <div class="mt-2">
              <p class="text-sm text-gray-500">
                {{ message }}
              </p>
            </div>
          </div>
        </div>
        
        <div class="mt-5 sm:mt-4 sm:flex sm:flex-row-reverse">
          <button
            @click="handleConfirm"
            :disabled="loading"
            type="button"
            class="w-full inline-flex justify-center rounded-md border border-transparent shadow-sm px-4 py-2 bg-red-600 text-base font-medium text-white hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500 sm:ml-3 sm:w-auto sm:text-sm disabled:opacity-50"
          >
            <span v-if="loading" class="mr-2">
              <svg class="animate-spin h-4 w-4" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
            </span>
            {{ loading ? 'Deleting...' : 'Delete' }}
          </button>
          <button
            @click="handleCancel"
            :disabled="loading"
            type="button"
            class="mt-3 w-full inline-flex justify-center rounded-md border border-gray-300 shadow-sm px-4 py-2 bg-white text-base font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 sm:mt-0 sm:w-auto sm:text-sm disabled:opacity-50"
          >
            Cancel
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ExclamationTriangleIcon } from '@heroicons/vue/24/outline'
import apiService from '../services/api'

const route = useRoute()
const router = useRouter()
const loading = ref(false)

const type = computed(() => route.query.type as string)
const id = computed(() => route.query.id as string)
const name = computed(() => route.query.name as string)
const returnTo = computed(() => route.query.returnTo as string || '/backoffice')

const title = computed(() => {
  if (type.value === 'venue') return 'Delete Venue'
  if (type.value === 'special') return 'Delete Special'
  return 'Confirm Delete'
})

const message = computed(() => {
  if (type.value === 'venue') {
    return `Are you sure you want to delete the venue "${name.value}"? This action cannot be undone and will also delete all associated specials.`
  }
  if (type.value === 'special') {
    return `Are you sure you want to delete the special "${name.value}"? This action cannot be undone.`
  }
  return 'Are you sure you want to delete this item? This action cannot be undone.'
})

const handleConfirm = async () => {
  if (!type.value || !id.value) {
    console.error('Missing type or id parameters')
    return
  }

  loading.value = true
  
  try {
    if (type.value === 'venue') {
      await apiService.deleteVenue(parseInt(id.value))
    } else if (type.value === 'special') {
      await apiService.deleteSpecial(parseInt(id.value))
    }
    
    // Navigate back with success
    router.push({ 
      path: returnTo.value, 
      query: { 
        success: 'true', 
        message: `${type.value} deleted successfully` 
      } 
    })
  } catch (error) {
    console.error('Delete error:', error)
    // Navigate back with error
    router.push({ 
      path: returnTo.value, 
      query: { 
        error: 'true', 
        message: `Failed to delete ${type.value}` 
      } 
    })
  } finally {
    loading.value = false
  }
}

const handleCancel = () => {
  router.push(returnTo.value)
}

// Redirect if missing required parameters
onMounted(() => {
  if (!type.value || !id.value) {
    router.push('/backoffice')
  }
})
</script>
