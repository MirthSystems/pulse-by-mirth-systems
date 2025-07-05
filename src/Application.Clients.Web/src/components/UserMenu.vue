<template>
  <div class="relative" ref="menuContainer">
    <!-- Menu Trigger -->
    <button
      @click="toggleMenu"
      class="flex items-center space-x-2 p-1 rounded-full hover:bg-gray-100 transition-colors"
      :class="{ 'bg-gray-100': isOpen }"
    >
      <UserAvatar />
    </button>

    <!-- Dropdown Menu -->
    <Transition
      enter-active-class="transition ease-out duration-100"
      enter-from-class="transform opacity-0 scale-95"
      enter-to-class="transform opacity-100 scale-100"
      leave-active-class="transition ease-in duration-75"
      leave-from-class="transform opacity-100 scale-100"
      leave-to-class="transform opacity-0 scale-95"
    >
      <div
        v-if="isOpen"
        class="absolute right-0 mt-2 w-48 max-w-[calc(100vw-2rem)] bg-white rounded-md shadow-lg ring-1 ring-black ring-opacity-5 focus:outline-none z-50"
      >
        <div class="py-1">
          <!-- User Info -->
          <div class="px-4 py-2 border-b border-gray-100">
            <p class="text-sm font-medium text-gray-900">{{ displayName }}</p>
            <p class="text-sm text-gray-500">{{ user?.email }}</p>
          </div>
          
          <!-- Loading state for permissions -->
          <div v-if="permissionsLoading" class="px-4 py-2 text-xs text-gray-500 flex items-center">
            <svg class="animate-spin -ml-1 mr-2 h-3 w-3 text-gray-400" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
              <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
              <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
            </svg>
            Loading permissions...
          </div>
          
          <!-- Backoffice Link -->
          <router-link
            v-if="canAccessBackoffice && !permissionsLoading"
            to="/backoffice"
            @click="closeMenu"
            class="flex items-center px-4 py-2 text-sm text-gray-700 hover:bg-gray-100 transition-colors"
          >
            <BuildingStorefrontIcon class="mr-3 h-4 w-4" />
            Backoffice
          </router-link>
          
          <!-- Divider -->
          <div class="border-t border-gray-100"></div>
          
          <!-- Logout -->
          <button
            @click="handleLogout"
            class="flex items-center w-full px-4 py-2 text-sm text-gray-700 hover:bg-gray-100 transition-colors"
          >
            <ArrowRightOnRectangleIcon class="mr-3 h-4 w-4" />
            Logout
          </button>
        </div>
      </div>
    </Transition>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { storeToRefs } from 'pinia'
import { useAuth0 } from '@auth0/auth0-vue'
import { BuildingStorefrontIcon, ArrowRightOnRectangleIcon } from '@heroicons/vue/24/outline'
import { useAuthStore } from '../stores/auth'
import { usePermissions } from '../composables/usePermissions'
import UserAvatar from './UserAvatar.vue'

const { logout } = useAuth0()
const authStore = useAuthStore()
const { user, displayName } = storeToRefs(authStore)
const { canAccessBackoffice, permissionsLoading } = usePermissions()

const isOpen = ref(false)
const menuContainer = ref<HTMLElement>()

const toggleMenu = () => {
  isOpen.value = !isOpen.value
}

const closeMenu = () => {
  isOpen.value = false
}

const handleLogout = () => {
  logout({ 
    logoutParams: { 
      returnTo: window.location.origin 
    } 
  })
  authStore.logout()
  closeMenu()
}

// Close menu when clicking outside
const handleClickOutside = (event: MouseEvent) => {
  if (menuContainer.value && !menuContainer.value.contains(event.target as Node)) {
    closeMenu()
  }
}

onMounted(() => {
  document.addEventListener('click', handleClickOutside)
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
})
</script>
