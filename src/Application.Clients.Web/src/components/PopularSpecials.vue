<template>
  <div class="popular-specials">
    <!-- Loading state -->
    <div v-if="loading" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <div 
        v-for="i in 6" 
        :key="i"
        class="bg-white rounded-lg shadow-md overflow-hidden animate-pulse"
      >
        <div class="h-48 bg-gray-300"></div>
        <div class="p-4">
          <div class="h-4 bg-gray-300 rounded mb-2"></div>
          <div class="h-4 bg-gray-300 rounded w-3/4 mb-2"></div>
          <div class="h-3 bg-gray-300 rounded w-1/2"></div>
        </div>
      </div>
    </div>

    <!-- Popular specials grid -->
    <div v-else-if="specials.length > 0" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <SpecialCard
        v-for="special in specials"
        :key="special.id"
        :special="special"
      />
    </div>

    <!-- Empty state -->
    <div v-else class="text-center py-12">
      <StarIcon class="mx-auto h-12 w-12 text-gray-400 mb-4" />
      <h3 class="text-lg font-medium text-gray-900 mb-2">No specials available</h3>
      <p class="text-gray-500">
        Check back later for amazing deals and special offers.
      </p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { StarIcon } from '@heroicons/vue/24/outline'
import SpecialCard from './SpecialCard.vue'
import type { Special } from '../types/api'

const loading = ref(true)
const specials = ref<Special[]>([])

// Mock data for now - replace with actual API call
const fetchPopularSpecials = async () => {
  loading.value = true
  
  // Simulate API call delay
  await new Promise(resolve => setTimeout(resolve, 1000))
  
  // Mock popular specials data
  specials.value = [
    {
      id: 1,
      venueId: 1,
      venueName: 'The Local Bar',
      specialCategoryId: 1,
      categoryName: 'Drinks',
      categoryIcon: '🍹',
      title: 'Happy Hour - 50% Off Cocktails',
      description: 'Join us for our daily happy hour with half-price premium cocktails and appetizers.',
      startDate: '2025-01-01',
      startTime: '17:00',
      endTime: '19:00',
      endDate: '2025-12-31',
      isRecurring: true,
      cronSchedule: '0 17 * * 1-5',
      isActive: true,
      venue: {
        id: 1,
        categoryId: 1,
        categoryName: 'Bar',
        categoryIcon: '🍺',
        activeSpecialsCount: 3,
        name: 'The Local Bar',
        description: 'Cozy neighborhood bar with craft cocktails',
        phoneNumber: '555-0101',
        website: 'https://thelocalbar.com',
        email: 'info@thelocalbar.com',
        profileImage: 'https://images.unsplash.com/photo-1514362545857-3bc16c4c7d1b?w=400',
        streetAddress: '123 Main St',
        locality: 'Downtown',
        region: 'State',
        postalCode: '12345',
        country: 'US',
        latitude: 40.7128,
        longitude: -74.0060,
        isActive: true
      },
      category: {
        id: 1,
        name: 'Drinks',
        description: 'Alcoholic and non-alcoholic beverages',
        icon: '🍹',
        sortOrder: 1
      }
    },
    {
      id: 2,
      venueId: 2,
      venueName: 'Mario\'s Pizzeria',
      specialCategoryId: 2,
      categoryName: 'Food',
      categoryIcon: '🍕',
      title: 'Pizza Tuesday - Buy One Get One Free',
      description: 'Every Tuesday, get a free pizza when you buy one at regular price. All day special!',
      startDate: '2025-01-01',
      startTime: '11:00',
      endTime: '22:00',
      endDate: '2025-12-31',
      isRecurring: true,
      cronSchedule: '0 11 * * 2',
      isActive: true,
      venue: {
        id: 2,
        categoryId: 2,
        categoryName: 'Restaurant',
        categoryIcon: '🍕',
        activeSpecialsCount: 2,
        name: 'Mario\'s Pizzeria',
        description: 'Authentic Italian pizza made fresh daily',
        phoneNumber: '555-0102',
        website: 'https://mariospizza.com',
        email: 'info@mariospizza.com',
        profileImage: 'https://images.unsplash.com/photo-1513104890138-7c749659a591?w=400',
        streetAddress: '456 Oak Ave',
        locality: 'Little Italy',
        region: 'State',
        postalCode: '12346',
        country: 'US',
        latitude: 40.7589,
        longitude: -73.9851,
        isActive: true
      },
      category: {
        id: 2,
        name: 'Food',
        description: 'Delicious food offerings',
        icon: '🍕',
        sortOrder: 2
      }
    },
    {
      id: 3,
      venueId: 3,
      venueName: 'Café Sunrise',
      specialCategoryId: 2,
      categoryName: 'Food',
      categoryIcon: '🥐',
      title: 'Weekend Brunch Special',
      description: 'Saturday and Sunday brunch with bottomless mimosas and fresh pastries.',
      startDate: '2025-01-01',
      startTime: '09:00',
      endTime: '15:00',
      endDate: '2025-12-31',
      isRecurring: true,
      cronSchedule: '0 9 * * 6,0',
      isActive: true,
      venue: {
        id: 3,
        categoryId: 3,
        categoryName: 'Café',
        categoryIcon: '☕',
        activeSpecialsCount: 1,
        name: 'Café Sunrise',
        description: 'Cozy café with fresh pastries and artisan coffee',
        phoneNumber: '555-0103',
        website: 'https://cafesunrise.com',
        email: 'info@cafesunrise.com',
        profileImage: 'https://images.unsplash.com/photo-1551218808-94e220e084d2?w=400',
        streetAddress: '789 Park Blvd',
        locality: 'Midtown',
        region: 'State',
        postalCode: '12347',
        country: 'US',
        latitude: 40.7505,
        longitude: -73.9934,
        isActive: true
      },
      category: {
        id: 2,
        name: 'Food',
        description: 'Delicious food offerings',
        icon: '🥐',
        sortOrder: 2
      }
    }
  ]
  
  loading.value = false
}

onMounted(() => {
  fetchPopularSpecials()
})
</script>
