<template>
  <div class="relative" ref="dropdownRef">
    <!-- Profile button -->
    <button
      @click="toggleDropdown"
      class="flex items-center space-x-2 text-sm rounded-full focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
    >
      <UserAvatar 
        :user="user" 
        :size="8"
        class="hover:opacity-80 transition-opacity"
      />
      <ChevronDownIcon 
        class="h-4 w-4 text-gray-500 transition-transform duration-200"
        :class="{ 'rotate-180': isOpen }"
      />
    </button>

    <!-- Dropdown menu -->
    <Transition
      enter-active-class="transition ease-out duration-200"
      enter-from-class="transform opacity-0 scale-95"
      enter-to-class="transform opacity-100 scale-100"
      leave-active-class="transition ease-in duration-75"
      leave-from-class="transform opacity-100 scale-100"
      leave-to-class="transform opacity-0 scale-95"
    >
      <div
        v-if="isOpen"
        class="absolute right-0 z-50 mt-2 w-56 origin-top-right rounded-md bg-white shadow-lg ring-1 ring-black ring-opacity-5 focus:outline-none"
      >
        <div class="px-4 py-3 border-b border-gray-100">
          <p class="text-sm font-medium text-gray-900">{{ user?.name || 'User' }}</p>
          <p class="text-sm text-gray-500 truncate">{{ user?.email }}</p>
        </div>
        
        <div class="py-1">
          <!-- Backoffice Link (for authenticated users with admin access) -->
          <router-link
            v-if="canAccessBackoffice"
            to="/backoffice"
            @click="closeDropdown"
            class="group flex items-center px-4 py-2 text-sm text-gray-700 hover:bg-gray-100 hover:text-gray-900"
          >
            <BuildingOfficeIcon class="mr-3 h-4 w-4 text-gray-400 group-hover:text-gray-500" />
            Backoffice
          </router-link>
          
          <!-- Profile Settings -->
          <button
            @click="handleProfileSettings"
            class="group flex w-full items-center px-4 py-2 text-sm text-gray-700 hover:bg-gray-100 hover:text-gray-900"
          >
            <UserIcon class="mr-3 h-4 w-4 text-gray-400 group-hover:text-gray-500" />
            Profile Settings
          </button>
          
          <!-- Divider -->
          <div class="border-t border-gray-100 my-1"></div>
          
          <!-- Sign Out -->
          <button
            @click="handleLogout"
            class="group flex w-full items-center px-4 py-2 text-sm text-gray-700 hover:bg-gray-100 hover:text-gray-900"
          >
            <ArrowRightOnRectangleIcon class="mr-3 h-4 w-4 text-gray-400 group-hover:text-gray-500" />
            Sign Out
          </button>
        </div>
      </div>
    </Transition>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuth0 } from '@auth0/auth0-vue'
import {
  UserIcon,
  BuildingOfficeIcon,
  ArrowRightOnRectangleIcon,
  ChevronDownIcon
} from '@heroicons/vue/24/outline'
import { usePermissions } from '@/composables/usePermissions'
import UserAvatar from './UserAvatar.vue'

const router = useRouter()
const { user, logout } = useAuth0()
const { canAccessBackoffice } = usePermissions()

const isOpen = ref(false)
const dropdownRef = ref<HTMLElement>()

const toggleDropdown = () => {
  isOpen.value = !isOpen.value
}

const closeDropdown = () => {
  isOpen.value = false
}

const handleLogout = () => {
  closeDropdown()
  logout({
    logoutParams: {
      returnTo: window.location.origin
    }
  })
}

const handleProfileSettings = () => {
  closeDropdown()
  router.push('/profile')
}

// Close dropdown when clicking outside
const handleClickOutside = (event: Event) => {
  if (dropdownRef.value && !dropdownRef.value.contains(event.target as Node)) {
    closeDropdown()
  }
}

onMounted(() => {
  document.addEventListener('click', handleClickOutside)
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
})
</script>
