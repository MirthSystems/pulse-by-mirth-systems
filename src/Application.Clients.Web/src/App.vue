<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router'
import HelloWorld from './components/HelloWorld.vue'
</script>

<template>
  <header
    class="flex flex-col md:flex-row items-center max-h-screen leading-[1.5] pr-0 md:pr-[calc(var(--section-gap)_/_2)]"
  >
    <img
      alt="Vue logo"
      class="block mx-auto md:mr-8 mb-8 md:mb-0 w-[125px] h-[125px]"
      src="@/assets/logo.svg"
      width="125"
      height="125"
    />

    <div class="flex flex-col md:flex-row md:items-start flex-wrap w-full">
      <HelloWorld msg="You did it!" />

      <nav
        class="w-full text-xs md:text-base text-center md:text-left mt-8 md:mt-4 ml-0 md:ml-[-1rem] py-0 md:py-4"
      >
        <RouterLink
          to="/"
          class="inline-block px-4 border-l border-[var(--color-border)] first:border-0 text-[var(--color-text)] hover:bg-transparent"
          active-class="text-[var(--color-text)]"
          >Home</RouterLink
        >
        <RouterLink
          to="/about"
          class="inline-block px-4 border-l border-[var(--color-border)] first:border-0 text-[var(--color-text)] hover:bg-transparent"
          active-class="text-[var(--color-text)]"
          >About</RouterLink
        >
      </nav>
    </div>
  </header>

  <RouterView />
</template>
