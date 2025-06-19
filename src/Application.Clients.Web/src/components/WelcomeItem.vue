<template>
  <div class="flex relative mt-8 md:mt-0 md:py-2 md:pb-4 md:pl-[calc(var(--section-gap)_/_2)]">
    <i
      class="flex items-center justify-center w-8 h-8 md:w-[50px] md:h-[50px] md:absolute md:top-1/2 md:left-[-26px] md:-translate-y-1/2 border border-[var(--color-border)] bg-[var(--color-background)] rounded-lg text-[var(--color-text)]"
    >
      <slot name="icon"></slot>
    </i>
    <div class="flex-1 ml-4">
      <h3 class="text-lg font-medium mb-2 text-[var(--color-heading)]">
        <slot name="heading"></slot>
      </h3>
      <slot></slot>
    </div>
    <!-- Timeline lines for desktop -->
    <div
      v-if="$slots.icon"
      class="hidden md:block absolute left-0 w-px bg-[var(--color-border)]"
      :style="{ top: 'calc(50% + 25px)', height: 'calc(50% - 25px)' }"
    ></div>
    <div
      v-if="$slots.icon"
      class="hidden md:block absolute left-0 w-px bg-[var(--color-border)]"
      :style="{ bottom: 'calc(50% + 25px)', height: 'calc(50% - 25px)' }"
    ></div>
  </div>
</template>
