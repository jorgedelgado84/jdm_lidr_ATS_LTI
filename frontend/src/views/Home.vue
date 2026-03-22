<template>
  <div class="space-y-12">
    <!-- Hero Section -->
    <div class="bg-gradient-to-r from-indigo-600 to-purple-600 text-white rounded-lg p-12 text-center">
      <h1 class="text-4xl font-bold mb-4">{{ $t('common.appName') }}</h1>
      <p class="text-lg mb-8">{{ $t('positions.browsePositions') }}</p>
      <div class="flex justify-center gap-4 flex-wrap">
        <router-link
          v-if="!isAuthenticated"
          to="/positions"
          class="px-8 py-3 bg-white text-indigo-600 rounded-lg font-semibold hover:bg-gray-100 transition"
        >
          {{ $t('navigation.positions') }}
        </router-link>
        <router-link
          v-if="!isAuthenticated"
          to="/register"
          class="px-8 py-3 border-2 border-white text-white rounded-lg font-semibold hover:bg-white hover:text-indigo-600 transition"
        >
          {{ $t('navigation.register') }}
        </router-link>
        <router-link
          v-if="isAuthenticated"
          to="/dashboard"
          class="px-8 py-3 bg-white text-indigo-600 rounded-lg font-semibold hover:bg-gray-100 transition"
        >
          {{ $t('navigation.dashboard') }}
        </router-link>
      </div>
    </div>

    <!-- Features Section -->
    <div>
      <h2 class="text-3xl font-bold text-gray-900 mb-8">{{ $t('common.appName') }}</h2>
      <div class="grid md:grid-cols-3 gap-6">
        <!-- Feature 1 -->
        <div class="bg-white p-6 rounded-lg shadow hover:shadow-lg transition">
          <div class="text-4xl mb-4">🔍</div>
          <h3 class="text-xl font-semibold text-gray-900 mb-2">{{ $t('positions.title') }}</h3>
          <p class="text-gray-600">{{ $t('positions.browsePositions') }}</p>
        </div>
        
        <!-- Feature 2 -->
        <div class="bg-white p-6 rounded-lg shadow hover:shadow-lg transition">
          <div class="text-4xl mb-4">📝</div>
          <h3 class="text-xl font-semibold text-gray-900 mb-2">{{ $t('applications.title') }}</h3>
          <p class="text-gray-600">{{ $t('applications.trackStatus') }}</p>
        </div>

        <!-- Feature 3 -->
        <div class="bg-white p-6 rounded-lg shadow hover:shadow-lg transition">
          <div class="text-4xl mb-4">⚡</div>
          <h3 class="text-xl font-semibold text-gray-900 mb-2">{{ $t('dashboard.title') }}</h3>
          <p class="text-gray-600">{{ $t('dashboard.welcome') }}</p>
        </div>
      </div>
    </div>

    <!-- Stats Section -->
    <div class="bg-white rounded-lg shadow p-12">
      <h2 class="text-3xl font-bold text-gray-900 mb-8 text-center">By The Numbers</h2>
      <div class="grid md:grid-cols-4 gap-8 text-center">
        <div>
          <div class="text-4xl font-bold text-indigo-600">500+</div>
          <p class="text-gray-600 mt-2">{{ $t('positions.title') }}</p>
        </div>
        <div>
          <div class="text-4xl font-bold text-indigo-600">10K+</div>
          <p class="text-gray-600 mt-2">Candidates</p>
        </div>
        <div>
          <div class="text-4xl font-bold text-indigo-600">50+</div>
          <p class="text-gray-600 mt-2">Companies</p>
        </div>
        <div>
          <div class="text-4xl font-bold text-indigo-600">95%</div>
          <p class="text-gray-600 mt-2">Success Rate</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useAuthStore } from '../stores/authStore'

const authStore = useAuthStore()
const isAuthenticated = computed(() => authStore.isAuthenticated)
</script>

<style scoped>
/* Responsive design */
@media (max-width: 768px) {
  h1 {
    font-size: 2rem;
  }
}
</style>
