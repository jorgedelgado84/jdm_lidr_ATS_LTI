<template>
  <div class="space-y-8">
    <!-- Header -->
    <div>
      <h1 class="text-4xl font-bold text-gray-900">{{ $t('dashboard.title') }}</h1>
      <p class="mt-2 text-gray-600">{{ $t('dashboard.welcome') }}, {{ user?.fullName }}!</p>
    </div>

    <!-- Stats Cards -->
    <div class="grid md:grid-cols-4 gap-6">
      <div class="bg-white rounded-lg shadow p-6 border-t-4 border-indigo-600">
        <div class="text-sm text-gray-600 mb-2">{{ $t('dashboard.totalApplications') }}</div>
        <div class="text-3xl font-bold text-indigo-600">12</div>
      </div>
      <div class="bg-white rounded-lg shadow p-6 border-t-4 border-yellow-600">
        <div class="text-sm text-gray-600 mb-2">{{ $t('dashboard.pendingReview') }}</div>
        <div class="text-3xl font-bold text-yellow-600">5</div>
      </div>
      <div class="bg-white rounded-lg shadow p-6 border-t-4 border-green-600">
        <div class="text-sm text-gray-600 mb-2">{{ $t('dashboard.accepted') }}</div>
        <div class="text-3xl font-bold text-green-600">2</div>
      </div>
      <div class="bg-white rounded-lg shadow p-6 border-t-4 border-red-600">
        <div class="text-sm text-gray-600 mb-2">{{ $t('dashboard.rejected') }}</div>
        <div class="text-3xl font-bold text-red-600">5</div>
      </div>
    </div>

    <!-- Recent Applications -->
    <div class="bg-white rounded-lg shadow p-6">
      <h2 class="text-2xl font-bold text-gray-900 mb-4">{{ $t('dashboard.recentApplications') }}</h2>
      <div class="space-y-4">
        <div class="border-l-4 border-indigo-500 pl-4 py-2 hover:bg-gray-50 transition p-2">
          <h3 class="font-semibold text-gray-900">Senior Software Engineer</h3>
          <p class="text-sm text-gray-600">Status: <span class="text-yellow-600 font-medium">{{ $t('dashboard.pendingReview') }}</span></p>
          <p class="text-xs text-gray-500">Applied on March 15, 2026</p>
        </div>
        <div class="border-l-4 border-green-500 pl-4 py-2 hover:bg-gray-50 transition p-2">
          <h3 class="font-semibold text-gray-900">Full Stack Developer</h3>
          <p class="text-sm text-gray-600">Status: <span class="text-green-600 font-medium">{{ $t('dashboard.accepted') }}</span></p>
          <p class="text-xs text-gray-500">Applied on March 10, 2026</p>
        </div>
      </div>
    </div>

    <!-- Action Buttons -->
    <div class="flex gap-4 flex-wrap">
      <router-link
        to="/positions"
        class="px-6 py-3 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition font-medium"
      >
        {{ $t('dashboard.browseMorePositions') }}
      </router-link>
      <router-link
        to="/my-applications"
        class="px-6 py-3 bg-gray-200 text-gray-900 rounded-lg hover:bg-gray-300 transition font-medium"
      >
        {{ $t('dashboard.viewAllApplications') }}
      </router-link>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useAuthStore } from '../stores/authStore'

const authStore = useAuthStore()
const user = computed(() => authStore.user)
</script>

<style scoped>
@media (max-width: 768px) {
  h1 {
    font-size: 1.875rem;
  }
  
  .grid {
    grid-template-columns: repeat(2, 1fr);
  }
}
</style>
