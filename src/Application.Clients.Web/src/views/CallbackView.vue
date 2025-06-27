<template>
  <div class="min-h-screen flex items-center justify-center bg-gray-50">
    <div class="max-w-md w-full space-y-8">
      <div class="text-center">
        <div class="animate-spin rounded-full h-16 w-16 border-b-2 border-blue-600 mx-auto"></div>
        <h2 class="mt-6 text-3xl font-extrabold text-gray-900">
          Completing sign in...
        </h2>
        <p class="mt-2 text-sm text-gray-600">
          Please wait while we complete your authentication.
        </p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

onMounted(() => {
  // After a short delay, redirect to home page
  // Auth0 will handle the callback processing automatically
  setTimeout(() => {
    router.push('/')
  }, 2000)
})
</script>
