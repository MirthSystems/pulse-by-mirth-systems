<template>
  <div class="welcome-content">
    <!-- Hero Section -->
    <div class="text-center py-12">
      <h2 class="text-3xl font-bold text-gray-900 mb-4">
        Welcome to Pulse
      </h2>
      <p class="text-xl text-gray-600 max-w-3xl mx-auto">
        Your go-to platform for discovering the best specials, deals, and experiences at venues near you. 
        Never miss out on great offers again!
      </p>
    </div>

    <!-- Features Grid -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-8 mb-12">
      <div 
        v-for="feature in features" 
        :key="feature.title"
        class="text-center p-6 bg-white rounded-lg shadow-md hover:shadow-lg transition-shadow"
      >
        <div class="w-16 h-16 mx-auto mb-4 bg-blue-100 rounded-full flex items-center justify-center">
          <component :is="feature.icon" class="h-8 w-8 text-blue-600" />
        </div>
        <h3 class="text-lg font-semibold text-gray-900 mb-2">{{ feature.title }}</h3>
        <p class="text-gray-600">{{ feature.description }}</p>
      </div>
    </div>

    <!-- How It Works Section -->
    <div class="bg-gray-50 rounded-lg p-8">
      <h3 class="text-2xl font-bold text-center text-gray-900 mb-8">
        How It Works
      </h3>
      <div class="grid grid-cols-1 md:grid-cols-3 gap-8">
        <div 
          v-for="(step, index) in steps" 
          :key="step.title"
          class="text-center"
        >
          <div class="w-12 h-12 mx-auto mb-4 bg-blue-600 text-white rounded-full flex items-center justify-center text-lg font-bold">
            {{ index + 1 }}
          </div>
          <h4 class="text-lg font-semibold text-gray-900 mb-2">{{ step.title }}</h4>
          <p class="text-gray-600">{{ step.description }}</p>
        </div>
      </div>
    </div>

    <!-- CTA Section -->
    <div class="text-center py-12">
      <h3 class="text-2xl font-bold text-gray-900 mb-4">
        Ready to discover amazing deals?
      </h3>
      <p class="text-lg text-gray-600 mb-6">
        Join thousands of users who are already saving money and discovering new experiences.
      </p>
      <router-link
        to="/venues"
        class="inline-flex items-center px-6 py-3 border border-transparent text-base font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 transition-colors"
      >
        Explore Venues
        <ArrowRightIcon class="ml-2 h-5 w-5" />
      </router-link>
    </div>
  </div>
</template>

<script setup lang="ts">
import { 
  MagnifyingGlassIcon, 
  MapPinIcon, 
  StarIcon,
  ArrowRightIcon 
} from '@heroicons/vue/24/outline'

const features = [
  {
    title: 'Real-Time Specials',
    description: 'Get instant updates on the latest deals, happy hours, and limited-time offers happening right now.',
    icon: StarIcon
  },
  {
    title: 'Location-Based Discovery',
    description: 'Find amazing venues and specials near your current location or search in any area you want to explore.',
    icon: MapPinIcon
  },
  {
    title: 'Smart Search',
    description: 'Use our intelligent search to find exactly what you\'re looking for - by cuisine, venue type, or deal category.',
    icon: MagnifyingGlassIcon
  }
]

const steps = [
  {
    title: 'Search & Discover',
    description: 'Use our search to find venues and specials that match your preferences and location.'
  },
  {
    title: 'Explore Details',
    description: 'Browse venue information, special details, timing, and customer reviews to make the best choice.'
  },
  {
    title: 'Visit & Enjoy',
    description: 'Head to your chosen venue and enjoy great food, drinks, and experiences at discounted prices.'
  }
]
</script>
