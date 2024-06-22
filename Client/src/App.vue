<template>
  <head>
    <meta name="viewport" content="width=540, initial-scale=1" />
  </head>
  <header
    :style="{
      backgroundImage: isDarkMode ? 'url(/static/banner-dark.svg)' : 'url(/static/banner-light.svg)'
    }"
    class="shadow-dark-glow dark:shadow-light-glow dark:bg-dark-bg bg-dark-fg text-5xl font-bold text-center px-6 py-12 bg-center bg-cover bg-no-repeat relative border-b-2 border-dark-bg dark:border-dark-fg w-full"
  >
    <div>
      <h1 class="text-[#0A1124] dark:text-[#fff]">Web Crawler</h1>
    </div>
    <div class="mr-[30px] pt-[30px] lg:mr-[40px] lg:pt-[40px] absolute top-0 right-0">
      <div class="flex flex-col border-2 border-gray-700 dark:border-dark-bg rounded-lg">
        <button
          :class="[
            isDarkMode === false ? 'bg-[#ff0]' : 'disabled-state',
            'px-3 py-1 select-none cursor-pointer rounded-t'
          ]"
          @click="toggleDarkMode(false)"
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            class="h-4 w-4"
            viewBox="0 0 20 20"
            fill="currentColor"
          >
            <path
              fill-rule="evenodd"
              d="M10 2a1 1 0 011 1v1a1 1 0 11-2 0V3a1 1 0 011-1zm4 8a4 4 0 11-8 0 4 4 0 018 0zm-.464 4.95l.707.707a1 1 0 001.414-1.414l-.707-.707a1 1 0 00-1.414 1.414zm2.12-10.607a1 1 0 010 1.414l-.706.707a1 1 0 11-1.414-1.414l.707-.707a1 1 0 011.414 0zM17 11a1 1 0 100-2h-1a1 1 0 100 2h1zm-7 4a1 1 0 011 1v1a1 1 0 11-2 0v-1a1 1 0 011-1zM5.05 6.464A1 1 0 106.465 5.05l-.708-.707a1 1 0 00-1.414 1.414l.707.707zm1.414 8.486l-.707.707a1 1 0 01-1.414-1.414l.707-.707a1 1 0 011.414 1.414zM4 11a1 1 0 100-2H3a1 1 0 000 2h1z"
              clip-rule="evenodd"
            />
          </svg>
        </button>
        <button
          :class="[
            isDarkMode === true
              ? 'bg-[#01cdf4] shadow-moon-glow text-[#fff]'
              : 'bg-gray-700 text-gray-400',
            'px-3 py-1 select-none cursor-pointer rounded-b'
          ]"
          @click="toggleDarkMode(true)"
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            class="h-4 w-4"
            viewBox="0 0 20 20"
            fill="currentColor"
          >
            <path d="M17.293 13.293A8 8 0 016.707 2.707a8.001 8.001 0 1010.586 10.586z" />
          </svg>
        </button>
      </div>
    </div>
  </header>
  <main class="min-h-[90vh]">
    <RouterView />
  </main>
  <footer
    class="text-center py-4 bg-dark-bg text-black text-dark-fg border-t-2 border-dark-bg dark:border-dark-fg"
  >
    <p>© 2024 Web Crawler</p>
  </footer>
</template>

<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router'
import { ref, computed } from 'vue'

const isDarkMode = ref(false)
function toggleDarkMode(darkMode: boolean) {
  if (darkMode === isDarkMode.value) {
    return
  }
  isDarkMode.value = darkMode
  if (darkMode) {
    document.documentElement.classList.add('dark')
  } else {
    document.documentElement.classList.remove('dark')
  }
}
</script>
