<template>
  <div class="home-page">
    <!-- Hero Section with Search -->
    <section class="bg-gradient-to-br from-blue-600 via-purple-600 to-blue-800 text-white">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-16 sm:py-24">
        <div class="text-center">
          <h1 class="text-4xl sm:text-5xl lg:text-6xl font-bold mb-6">
            Discover Amazing
            <span class="text-transparent bg-clip-text bg-gradient-to-r from-yellow-400 to-orange-400">
              Specials
            </span>
          </h1>
          <p class="text-xl sm:text-2xl text-blue-100 mb-8 max-w-3xl mx-auto">
            Find the best deals and special offers at restaurants, bars, and venues near you.
          </p>
          
          <!-- Main Search Component -->
          <div class="max-w-2xl mx-auto">
            <SearchSpecials />
          </div>
        </div>
      </div>
    </section>

    <!-- Features Section -->
    <section class="py-16 bg-white">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="text-center mb-12">
          <h2 class="text-3xl font-bold text-gray-900 mb-4">
            Why Choose Pulse?
          </h2>
          <p class="text-lg text-gray-600 max-w-2xl mx-auto">
            Discover, explore, and save money with our comprehensive platform for finding the best local deals.
          </p>
        </div>
        
        <div class="grid grid-cols-1 md:grid-cols-3 gap-8">
          <div class="text-center">
            <div class="w-16 h-16 bg-blue-100 rounded-full flex items-center justify-center mx-auto mb-4">
              <MagnifyingGlassIcon class="h-8 w-8 text-blue-600" />
            </div>
            <h3 class="text-xl font-semibold text-gray-900 mb-2">Easy Discovery</h3>
            <p class="text-gray-600">
              Search and filter through hundreds of venues and their current special offers with ease.
            </p>
          </div>
          
          <div class="text-center">
            <div class="w-16 h-16 bg-green-100 rounded-full flex items-center justify-center mx-auto mb-4">
              <StarIcon class="h-8 w-8 text-green-600" />
            </div>
            <h3 class="text-xl font-semibold text-gray-900 mb-2">Best Deals</h3>
            <p class="text-gray-600">
              Access exclusive deals and special offers you won't find anywhere else.
            </p>
          </div>
          
          <div class="text-center">
            <div class="w-16 h-16 bg-purple-100 rounded-full flex items-center justify-center mx-auto mb-4">
              <MapPinIcon class="h-8 w-8 text-purple-600" />
            </div>
            <h3 class="text-xl font-semibold text-gray-900 mb-2">Local Focus</h3>
            <p class="text-gray-600">
              Discover amazing local businesses and support your community while saving money.
            </p>
          </div>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import { 
  MagnifyingGlassIcon, 
  StarIcon, 
  MapPinIcon 
} from '@heroicons/vue/24/outline'
import SearchSpecials from '../components/SearchSpecials.vue'
</script>
