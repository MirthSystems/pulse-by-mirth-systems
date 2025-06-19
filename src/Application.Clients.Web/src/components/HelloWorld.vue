<script setup lang="ts">
defineProps<{
  msg: string
}>()
</script>

<template>
  <div class="flex flex-col items-center md:items-start mt-8">
    <h1 class="font-medium text-[2.6rem] relative -top-2.5 text-green-600 text-center md:text-left">
      {{ msg }}
    </h1>
    <h3 class="text-lg text-center md:text-left mt-2">
      You’ve successfully created a project with
      <a
        href="https://vite.dev/"
        target="_blank"
        rel="noopener"
        class="text-blue-600 underline hover:text-blue-800"
        >Vite</a
      >
      +
      <a
        href="https://vuejs.org/"
        target="_blank"
        rel="noopener"
        class="text-blue-600 underline hover:text-blue-800"
        >Vue 3</a
      >. What's next?
    </h3>
  </div>
</template>
