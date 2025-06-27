<template>
  <div class="user-avatar">
    <img 
      v-if="user?.picture" 
      :src="user.picture" 
      :alt="displayName"
      class="w-8 h-8 rounded-full object-cover"
    />
    <div v-else class="w-8 h-8 rounded-full bg-blue-500 flex items-center justify-center text-white text-sm font-medium">
      {{ userInitials }}
    </div>
  </div>
</template>

<script setup lang="ts">
import { storeToRefs } from 'pinia'
import { useAuthStore } from '../stores/auth'

const authStore = useAuthStore()
const { user, displayName, userInitials } = storeToRefs(authStore)
</script>
