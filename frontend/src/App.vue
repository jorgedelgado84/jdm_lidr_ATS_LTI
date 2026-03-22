<template>
  <div id="app" class="min-h-screen bg-gray-50">
    <!-- Navigation Bar -->
    <nav class="bg-white shadow-md border-b border-gray-200">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between items-center h-16">
          <!-- Logo -->
          <div class="flex items-center">
            <h1 class="text-2xl font-bold text-indigo-600">{{ $t('common.appName') }}</h1>
          </div>

          <!-- Navigation Links -->
          <div class="hidden md:flex items-center space-x-8">
            <router-link to="/" class="text-gray-600 hover:text-gray-900 transition">
              {{ $t('navigation.home') }}
            </router-link>
            <router-link to="/positions" class="text-gray-600 hover:text-gray-900 transition">
              {{ $t('navigation.positions') }}
            </router-link>
            <router-link v-if="isAuthenticated" to="/dashboard" class="text-gray-600 hover:text-gray-900 transition">
              {{ $t('navigation.dashboard') }}
            </router-link>
          </div>

          <!-- Auth & Language Controls -->
          <div class="flex items-center space-x-4">
            <!-- Language Selector -->
            <select
              :value="currentLocale"
              @change="setLanguage"
              class="px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500"
            >
              <option value="en">English</option>
              <option value="es">Español</option>
            </select>

            <!-- Auth Links -->
            <template v-if="!isAuthenticated">
              <router-link
                to="/login"
                class="text-gray-600 hover:text-gray-900 transition"
              >
                {{ $t('navigation.login') }}
              </router-link>
              <router-link
                to="/register"
                class="px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition"
              >
                {{ $t('navigation.register') }}
              </router-link>
            </template>

            <!-- User Menu -->
            <div v-if="isAuthenticated" class="flex items-center space-x-4">
              <span class="text-sm text-gray-700">{{ user?.fullName }}</span>
              <button
                @click="logout"
                class="px-4 py-2 text-sm text-gray-700 hover:text-gray-900 transition"
              >
                {{ $t('common.logout') }}
              </button>
            </div>
          </div>
        </div>
      </div>
    </nav>

    <!-- Main Content -->
    <main class="max-w-7xl mx-auto py-8 px-4 sm:px-6 lg:px-8">
      <router-view></router-view>
    </main>

    <!-- Footer -->
    <footer class="bg-white border-t border-gray-200 mt-16">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div class="text-center text-gray-600 text-sm">
          <p>&copy; 2026 {{ $t('common.appName') }}. All rights reserved.</p>
        </div>
      </div>
    </footer>
  </div>
</template>

<script setup>
import { computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useAuthStore } from './stores/authStore'

const router = useRouter()
const authStore = useAuthStore()
const { locale } = useI18n()

const isAuthenticated = computed(() => authStore.isAuthenticated)
const user = computed(() => authStore.user)
const currentLocale = computed(() => locale.value)

const logout = () => {
  authStore.logout()
  router.push('/login')
}

const setLanguage = (event) => {
  const language = event.target.value
  locale.value = language
  localStorage.setItem('language', language)
}

onMounted(() => {
  authStore.checkAuthStatus()
})
</script>

<style scoped>
/* Component-specific styles - Global styles handled by Tailwind CSS */
</style>
