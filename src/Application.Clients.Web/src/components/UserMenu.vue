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
        class="absolute right-0 mt-2 w-48 bg-white rounded-md shadow-lg ring-1 ring-black ring-opacity-5 focus:outline-none z-50"
      >
        <div class="py-1">
          <!-- User Info -->
          <div class="px-4 py-2 border-b border-gray-100">
            <p class="text-sm font-medium text-gray-900">{{ displayName }}</p>
            <p class="text-sm text-gray-500">{{ user?.email }}</p>
          </div>
          
          <!-- Backoffice Link -->
          <router-link
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
import UserAvatar from './UserAvatar.vue'

const { logout } = useAuth0()
const authStore = useAuthStore()
const { user, displayName } = storeToRefs(authStore)

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
