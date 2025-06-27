<script setup lang="ts">
import { 
  HeartIcon,
  UsersIcon,
  MapPinIcon,
  StarIcon,
  ClockIcon,
  SparklesIcon,
  DevicePhoneMobileIcon,
  GlobeAltIcon
} from '@heroicons/vue/24/outline'

const features = [
  {
    icon: MapPinIcon,
    title: 'Location-Based Discovery',
    description: 'Find venues and specials near you with precise location-based search and filtering.'
  },
  {
    icon: ClockIcon,
    title: 'Real-Time Updates',
    description: 'Get up-to-date information on specials, hours, and availability as they happen.'
  },
  {
    icon: StarIcon,
    title: 'Curated Specials',
    description: 'Discover hand-picked deals and specials from the best venues in your area.'
  },
  {
    icon: DevicePhoneMobileIcon,
    title: 'Mobile First',
    description: 'Designed for mobile use, perfect for discovering deals on the go.'
  }
]

const stats = [
  { label: 'Venues Listed', value: '50+' },
  { label: 'Daily Specials', value: '100+' },
  { label: 'Cities Covered', value: '5' },
  { label: 'Happy Users', value: '1000+' }
]

const team = [
  {
    name: 'Alex Thompson',
    role: 'Founder & CEO',
    bio: 'Passionate about connecting people with great food and experiences.',
    avatar: null
  },
  {
    name: 'Sarah Chen',
    role: 'Head of Product',
    bio: 'Focused on creating intuitive user experiences and product innovation.',
    avatar: null
  },
  {
    name: 'Marcus Rodriguez',
    role: 'Lead Developer',
    bio: 'Building the technology that powers venue discovery and special alerts.',
    avatar: null
  }
]
</script>

<template>
  <div>
    <!-- Hero Section -->
    <section class="bg-gradient-to-r from-blue-600 via-purple-600 to-blue-800 text-white">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-16 lg:py-24">
        <div class="text-center">
          <SparklesIcon class="h-16 w-16 text-yellow-300 mx-auto mb-6" />
          <h1 class="text-4xl md:text-6xl font-bold mb-6">
            About Pulse
          </h1>
          <p class="text-xl md:text-2xl text-blue-100 max-w-3xl mx-auto">
            We're passionate about helping people discover amazing venues and unbeatable specials in their city.
          </p>
        </div>
      </div>
    </section>

    <!-- Mission Section -->
    <section class="py-16 bg-white">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="max-w-4xl mx-auto text-center">
          <h2 class="text-3xl font-bold text-gray-900 mb-8">Our Mission</h2>
          <p class="text-xl text-gray-600 leading-relaxed mb-8">
            At Pulse, we believe that great experiences shouldn't be hard to find. Our mission is to connect 
            food lovers, social butterflies, and adventure seekers with the best venues and most exciting 
            specials their city has to offer.
          </p>
          <p class="text-lg text-gray-600 leading-relaxed">
            Whether you're looking for the perfect date night spot, a place to catch the game with friends, 
            or just want to discover something new in your neighborhood, Pulse is here to help you find it.
          </p>
        </div>
      </div>
    </section>

    <!-- Features Section -->
    <section class="py-16 bg-gray-50">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="text-center mb-12">
          <h2 class="text-3xl font-bold text-gray-900 mb-4">What Makes Us Different</h2>
          <p class="text-xl text-gray-600 max-w-2xl mx-auto">
            We're not just another restaurant app. We're your personal guide to the best experiences in your city.
          </p>
        </div>
        
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-8">
          <div 
            v-for="feature in features" 
            :key="feature.title"
            class="text-center"
          >
            <div class="flex justify-center mb-4">
              <div class="p-3 bg-blue-100 rounded-full">
                <component :is="feature.icon" class="h-8 w-8 text-blue-600" />
              </div>
            </div>
            <h3 class="text-lg font-semibold text-gray-900 mb-2">{{ feature.title }}</h3>
            <p class="text-gray-600">{{ feature.description }}</p>
          </div>
        </div>
      </div>
    </section>

    <!-- Stats Section -->
    <section class="py-16 bg-blue-600 text-white">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="text-center mb-12">
          <h2 class="text-3xl font-bold mb-4">Pulse by the Numbers</h2>
          <p class="text-xl text-blue-100">
            Growing every day to serve our community better
          </p>
        </div>
        
        <div class="grid grid-cols-2 lg:grid-cols-4 gap-8">
          <div 
            v-for="stat in stats" 
            :key="stat.label"
            class="text-center"
          >
            <div class="text-4xl lg:text-5xl font-bold text-yellow-300 mb-2">{{ stat.value }}</div>
            <div class="text-blue-100 font-medium">{{ stat.label }}</div>
          </div>
        </div>
      </div>
    </section>

    <!-- Story Section -->
    <section class="py-16 bg-white">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="max-w-4xl mx-auto">
          <div class="text-center mb-12">
            <h2 class="text-3xl font-bold text-gray-900 mb-4">Our Story</h2>
          </div>
          
          <div class="prose prose-lg mx-auto text-gray-600">
            <p>
              Pulse was born from a simple frustration: how hard it was to find great venues and current specials 
              in our own city. We found ourselves constantly asking friends for recommendations, scrolling through 
              outdated websites, or stumbling upon amazing deals by pure luck.
            </p>
            
            <p>
              We knew there had to be a better way. A way to discover not just where to go, but when to go for 
              the best deals. A platform that would surface hidden gems alongside popular favorites, and keep 
              everything up-to-date in real-time.
            </p>
            
            <p>
              So we built Pulse. Starting with our home city, we've partnered with local venues to bring you 
              accurate, timely information about specials, events, and experiences. Our goal is to make every 
              outing an adventure and every meal a discovery.
            </p>
            
            <p>
              Today, we're proud to serve thousands of users across multiple cities, helping them save money, 
              try new places, and make the most of their social lives. But we're just getting started.
            </p>
          </div>
        </div>
      </div>
    </section>

    <!-- Team Section -->
    <section class="py-16 bg-gray-50">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="text-center mb-12">
          <h2 class="text-3xl font-bold text-gray-900 mb-4">Meet the Team</h2>
          <p class="text-xl text-gray-600 max-w-2xl mx-auto">
            The passionate people behind Pulse, working every day to improve your discovery experience.
          </p>
        </div>
        
        <div class="grid grid-cols-1 md:grid-cols-3 gap-8">
          <div 
            v-for="member in team" 
            :key="member.name"
            class="bg-white rounded-lg shadow-md p-6 text-center"
          >
            <div class="w-24 h-24 bg-gradient-to-br from-blue-500 to-purple-600 rounded-full mx-auto mb-4 flex items-center justify-center">
              <span class="text-2xl font-bold text-white">{{ member.name.charAt(0) }}</span>
            </div>
            <h3 class="text-xl font-semibold text-gray-900 mb-1">{{ member.name }}</h3>
            <p class="text-blue-600 font-medium mb-3">{{ member.role }}</p>
            <p class="text-gray-600 text-sm">{{ member.bio }}</p>
          </div>
        </div>
      </div>
    </section>

    <!-- Values Section -->
    <section class="py-16 bg-white">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="text-center mb-12">
          <h2 class="text-3xl font-bold text-gray-900 mb-4">Our Values</h2>
          <p class="text-xl text-gray-600 max-w-2xl mx-auto">
            The principles that guide everything we do at Pulse.
          </p>
        </div>
        
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
          <div class="text-center">
            <div class="flex justify-center mb-4">
              <HeartIcon class="h-12 w-12 text-red-500" />
            </div>
            <h3 class="text-xl font-semibold text-gray-900 mb-3">Community First</h3>
            <p class="text-gray-600">
              We believe in supporting local businesses and bringing communities together through shared experiences.
            </p>
          </div>
          
          <div class="text-center">
            <div class="flex justify-center mb-4">
              <UsersIcon class="h-12 w-12 text-blue-500" />
            </div>
            <h3 class="text-xl font-semibold text-gray-900 mb-3">User-Centric</h3>
            <p class="text-gray-600">
              Every feature we build starts with understanding what our users truly need and want.
            </p>
          </div>
          
          <div class="text-center">
            <div class="flex justify-center mb-4">
              <GlobeAltIcon class="h-12 w-12 text-green-500" />
            </div>
            <h3 class="text-xl font-semibold text-gray-900 mb-3">Transparency</h3>
            <p class="text-gray-600">
              We're honest about our data, clear about our partnerships, and transparent in our communication.
            </p>
          </div>
        </div>
      </div>
    </section>

    <!-- Contact CTA -->
    <section class="py-16 bg-gradient-to-r from-blue-600 to-purple-600 text-white">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 text-center">
        <h2 class="text-3xl font-bold mb-4">Get in Touch</h2>
        <p class="text-xl text-blue-100 mb-8 max-w-2xl mx-auto">
          Have questions, suggestions, or want to partner with us? We'd love to hear from you.
        </p>
        <div class="flex flex-col sm:flex-row gap-4 justify-center">
          <a 
            href="mailto:hello@pulse.app"
            class="inline-flex items-center px-8 py-4 bg-white text-blue-600 font-semibold rounded-lg hover:bg-gray-100 transition-colors"
          >
            Contact Us
          </a>
          <router-link 
            to="/"
            class="inline-flex items-center px-8 py-4 bg-transparent border-2 border-white text-white font-semibold rounded-lg hover:bg-white hover:text-blue-600 transition-colors"
          >
            Start Exploring
          </router-link>
        </div>
      </div>
    </section>
  </div>
</template>
