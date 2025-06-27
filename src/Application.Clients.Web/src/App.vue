<script setup lang="ts">
import { ref } from 'vue'
import { RouterView } from 'vue-router'
import { 
  HomeIcon, 
  BuildingStorefrontIcon, 
  StarIcon, 
  MagnifyingGlassIcon,
  Bars3Icon,
  XMarkIcon
} from '@heroicons/vue/24/outline'

const mobileMenuOpen = ref(false)
</script>

<template>
  <div id="app" class="min-h-screen bg-gray-50">
    <!-- Navigation Header -->
    <nav class="bg-white shadow-lg border-b border-gray-200">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between h-16">
          <div class="flex items-center">
            <router-link to="/" class="flex items-center space-x-3">
              <div class="w-8 h-8 bg-gradient-to-br from-blue-500 to-purple-600 rounded-lg flex items-center justify-center">
                <span class="text-white font-bold text-lg">P</span>
              </div>
              <span class="text-xl font-bold text-gray-900">Pulse</span>
            </router-link>
          </div>
          
          <div class="hidden md:flex items-center space-x-8">
            <router-link 
              to="/" 
              class="nav-link"
              :class="{ 'nav-link-active': $route.name === 'Home' }"
            >
              <HomeIcon class="h-5 w-5 mr-2" />
              Home
            </router-link>
            
            <router-link 
              to="/venues" 
              class="nav-link"
              :class="{ 'nav-link-active': $route.name === 'Venues' }"
            >
              <BuildingStorefrontIcon class="h-5 w-5 mr-2" />
              Venues
            </router-link>
            
            <router-link 
              to="/specials" 
              class="nav-link"
              :class="{ 'nav-link-active': $route.name === 'Specials' }"
            >
              <StarIcon class="h-5 w-5 mr-2" />
              Specials
            </router-link>
            
            <router-link 
              to="/search" 
              class="nav-link"
              :class="{ 'nav-link-active': $route.name === 'Search' }"
            >
              <MagnifyingGlassIcon class="h-5 w-5 mr-2" />
              Search
            </router-link>
          </div>
          
          <!-- Mobile menu button -->
          <div class="md:hidden flex items-center">
            <button
              @click="mobileMenuOpen = !mobileMenuOpen"
              class="inline-flex items-center justify-center p-2 rounded-md text-gray-400 hover:text-gray-500 hover:bg-gray-100"
            >
              <Bars3Icon v-if="!mobileMenuOpen" class="h-6 w-6" />
              <XMarkIcon v-else class="h-6 w-6" />
            </button>
          </div>
        </div>
      </div>
      
      <!-- Mobile menu -->
      <div v-if="mobileMenuOpen" class="md:hidden border-t border-gray-200">
        <div class="px-2 pt-2 pb-3 space-y-1 sm:px-3 bg-white">
          <router-link 
            to="/" 
            class="mobile-nav-link"
            @click="mobileMenuOpen = false"
          >
            <HomeIcon class="h-5 w-5 mr-3" />
            Home
          </router-link>
          
          <router-link 
            to="/venues" 
            class="mobile-nav-link"
            @click="mobileMenuOpen = false"
          >
            <BuildingStorefrontIcon class="h-5 w-5 mr-3" />
            Venues
          </router-link>
          
          <router-link 
            to="/specials" 
            class="mobile-nav-link"
            @click="mobileMenuOpen = false"
          >
            <StarIcon class="h-5 w-5 mr-3" />
            Specials
          </router-link>
          
          <router-link 
            to="/search" 
            class="mobile-nav-link"
            @click="mobileMenuOpen = false"
          >
            <MagnifyingGlassIcon class="h-5 w-5 mr-3" />
            Search
          </router-link>
        </div>
      </div>
    </nav>
    
    <!-- Main Content -->
    <main class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <router-view />
    </main>
    
    <!-- Footer -->
    <footer class="bg-white border-t border-gray-200 mt-16">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div class="text-center text-gray-600">
          <p>&copy; 2025 Pulse. Built with ❤️ for discovering amazing venues and specials.</p>
        </div>
      </div>
    </footer>
  </div>
</template>

<style scoped>
.nav-link {
  @apply inline-flex items-center px-3 py-2 rounded-md text-sm font-medium text-gray-700 hover:text-gray-900 hover:bg-gray-100 transition-colors;
}

.nav-link-active {
  @apply text-blue-600 bg-blue-50;
}

.mobile-nav-link {
  @apply flex items-center px-3 py-2 rounded-md text-base font-medium text-gray-700 hover:text-gray-900 hover:bg-gray-100;
}
</style>
