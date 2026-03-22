<template>
  <div class="space-y-8">
    <router-link to="/positions" class="text-indigo-600 hover:text-indigo-700 font-medium">
      ← {{ $t('navigation.positions') }}
    </router-link>

    <!-- Loading State -->
    <div v-if="loading" class="text-center py-12">
      <p class="text-gray-600">{{ $t('positions.loadingPositions') }}</p>
    </div>

    <!-- Position Details -->
    <div v-else-if="position" class="space-y-6">
      <div class="bg-white rounded-lg shadow p-8 border-t-4 border-indigo-600">
        <!-- Header -->
        <div class="flex justify-between items-start mb-6 gap-4 flex-col sm:flex-row">
          <div class="flex-1">
            <h1 class="text-4xl font-bold text-gray-900">{{ position.title }}</h1>
            <p class="text-lg text-gray-600 mt-2">{{ position.department }} • {{ position.location }}</p>
          </div>
          <button
            v-if="isAuthenticated"
            @click="applyPosition"
            :disabled="hasApplied || applying"
            class="px-6 py-3 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 disabled:bg-gray-400 transition font-medium whitespace-nowrap"
          >
            {{ applying ? $t('positions.applying') : (hasApplied ? $t('positions.alreadyApplied') : $t('positions.applyNow')) }}
          </button>
        </div>

        <!-- Salary and Date -->
        <div class="grid md:grid-cols-2 gap-8 mb-8 border-y py-6">
          <div>
            <h3 class="text-sm font-semibold text-gray-500 uppercase mb-2">{{ $t('positions.salary') }}</h3>
            <p class="text-2xl font-bold text-gray-900">${{ formatCurrency(position.salaryMin) }} - ${{ formatCurrency(position.salaryMax) }}</p>
          </div>
          <div>
            <h3 class="text-sm font-semibold text-gray-500 uppercase mb-2">{{ $t('positions.postedOn') }}</h3>
            <p class="text-lg text-gray-900">{{ formatDate(position.createdAt) }}</p>
          </div>
        </div>

        <!-- Description -->
        <div class="mb-8">
          <h2 class="text-2xl font-bold text-gray-900 mb-4">{{ $t('positions.aboutThisPosition') }}</h2>
          <p class="text-gray-700 whitespace-pre-line">{{ position.description }}</p>
        </div>

        <!-- Details Box -->
        <div class="bg-indigo-50 rounded-lg p-6 mb-8 border border-indigo-200">
          <h3 class="text-lg font-semibold text-gray-900 mb-4">{{ $t('positions.positionDetails') }}</h3>
          <ul class="space-y-2 text-gray-700">
            <li class="flex items-center gap-2">✓ {{ $t('positions.department') }}: {{ position.department }}</li>
            <li class="flex items-center gap-2">✓ {{ $t('positions.location') }}: {{ position.location }}</li>
            <li class="flex items-center gap-2">✓ {{ $t('positions.status') }}: <span :class="statusClass">{{ position.status }}</span></li>
          </ul>
        </div>

        <!-- Apply Button (Full Width) -->
        <button
          v-if="isAuthenticated && !hasApplied"
          @click="applyPosition"
          :disabled="applying"
          class="w-full px-6 py-4 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 disabled:bg-gray-400 transition font-semibold text-lg"
        >
          {{ applying ? $t('positions.submittingApplication') : $t('positions.submitApplication') }}
        </button>

        <!-- Sign In Prompt -->
        <div v-else-if="!isAuthenticated" class="bg-blue-50 border border-blue-200 rounded-lg p-6 text-center">
          <p class="text-gray-700 mb-4">{{ $t('positions.signInToApply') }}</p>
          <router-link
            to="/login"
            class="inline-block px-6 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition font-medium"
          >
            {{ $t('navigation.login') }}
          </router-link>
        </div>
      </div>
    </div>

    <!-- Not Found -->
    <div v-else class="text-center py-12 bg-white rounded-lg shadow">
      <p class="text-gray-600 mb-4">{{ $t('positions.noPositionsFound') }}</p>
      <router-link to="/positions" class="text-indigo-600 hover:text-indigo-700 font-medium">
        {{ $t('navigation.positions') }}
      </router-link>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import axios from 'axios'
import { useAuthStore } from '../stores/authStore'

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:8080/api'

const route = useRoute()
const authStore = useAuthStore()

const position = ref(null)
const loading = ref(true)
const applying = ref(false)
const hasApplied = ref(false)

const isAuthenticated = computed(() => authStore.isAuthenticated)

const statusClass = computed(() => {
  const status = position.value?.status
  if (status === 'Open') return 'text-green-600 font-medium'
  if (status === 'Closed') return 'text-red-600 font-medium'
  return 'text-yellow-600 font-medium'
})

/**
 * Fetch position details from API
 */
const fetchPosition = async () => {
  loading.value = true
  try {
    const response = await axios.get(`${API_URL}/positions/${route.params.id}`)
    position.value = response.data
  } catch (err) {
    console.error('Error fetching position:', err)
  } finally {
    loading.value = false
  }
}

/**
 * Submit application for position
 */
const applyPosition = async () => {
  applying.value = true
  try {
    // Application submission logic
    hasApplied.value = true
    // await axios.post(`${API_URL}/applications`, { ... })
  } catch (err) {
    console.error('Error applying:', err)
  } finally {
    applying.value = false
  }
}

/**
 * Format currency for display
 */
const formatCurrency = (value) => {
  return new Intl.NumberFormat('en-US', {
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  }).format(value)
}

/**
 * Format date for display
 */
const formatDate = (date) => {
  return new Date(date).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
}

onMounted(() => {
  fetchPosition()
})
</script>

<style scoped>
@media (max-width: 768px) {
  h1 {
    font-size: 1.875rem;
  }
}
</style>
