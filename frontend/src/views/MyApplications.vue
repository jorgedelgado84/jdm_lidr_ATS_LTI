<template>
  <div class="space-y-8">
    <!-- Header -->
    <div>
      <h1 class="text-4xl font-bold text-gray-900">{{ $t('applications.title') }}</h1>
      <p class="mt-2 text-gray-600">{{ $t('applications.trackStatus') }}</p>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="text-center py-12">
      <p class="text-gray-600">{{ $t('positions.loadingPositions') }}</p>
    </div>

    <!-- Empty State -->
    <div v-else-if="applications.length === 0" class="text-center py-12 bg-white rounded-lg shadow">
      <p class="text-gray-600 mb-4">{{ $t('applications.noApplications') }}</p>
      <router-link
        to="/positions"
        class="inline-block px-6 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition font-medium"
      >
        {{ $t('navigation.positions') }}
      </router-link>
    </div>

    <!-- Applications -->
    <div v-else class="space-y-4">
      <!-- Filter Tabs -->
      <div class="flex gap-2 mb-6 flex-wrap">
        <button
          v-for="status in statuses"
          :key="status"
          @click="selectedStatus = status === selectedStatus ? '' : status"
          :class="[
            'px-4 py-2 rounded-lg font-medium transition',
            selectedStatus === status
              ? 'bg-indigo-600 text-white'
              : 'bg-white text-gray-700 border border-gray-300 hover:bg-gray-50'
          ]"
        >
          {{ status === '' ? 'All' : status }}
        </button>
      </div>

      <!-- Applications List -->
      <div class="space-y-4">
        <div
          v-for="app in filteredApplications"
          :key="app.id"
          class="bg-white rounded-lg shadow hover:shadow-lg transition p-6 border-l-4"
          :class="getBorderColor(app.status)"
        >
          <div class="flex justify-between items-start mb-4 gap-4 flex-col sm:flex-row">
            <div>
              <h3 class="text-xl font-bold text-gray-900">{{ app.position }}</h3>
              <p class="text-gray-600 mt-1">{{ app.company }}</p>
            </div>
            <span :class="['px-4 py-2 rounded-full text-sm font-semibold whitespace-nowrap', getStatusColor(app.status)]">
              {{ app.status }}
            </span>
          </div>

          <!-- Application Details -->
          <div class="grid md:grid-cols-3 gap-4 text-sm text-gray-600 mb-4 border-t pt-4">
            <div>
              <p class="text-xs text-gray-500 uppercase">{{ $t('applications.appliedDate') }}</p>
              <p class="font-semibold text-gray-900">{{ formatDate(app.appliedDate) }}</p>
            </div>
            <div>
              <p class="text-xs text-gray-500 uppercase">{{ $t('applications.lastUpdated') }}</p>
              <p class="font-semibold text-gray-900">{{ formatDate(app.lastUpdated) }}</p>
            </div>
            <div>
              <p class="text-xs text-gray-500 uppercase">{{ $t('applications.daysAgo') }}</p>
              <p class="font-semibold text-gray-900">{{ daysAgo(app.appliedDate) }} days</p>
            </div>
          </div>

          <!-- Review Notes -->
          <div v-if="app.reviewNotes" class="bg-blue-50 rounded p-4 mb-4 border border-blue-200">
            <p class="text-sm text-gray-700">
              <strong>{{ $t('applications.reviewNotes') }}:</strong> {{ app.reviewNotes }}
            </p>
          </div>

          <!-- Actions -->
          <div class="flex gap-4">
            <router-link
              :to="`/positions/${app.positionId}`"
              class="text-indigo-600 hover:text-indigo-700 font-medium transition"
            >
              {{ $t('applications.viewPosition') }}
            </router-link>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import axios from 'axios'

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:8080/api'

// Mock data for demonstration
const applications = ref([
  {
    id: 1,
    position: 'Senior Software Engineer',
    company: 'TechCorp',
    status: 'Reviewing',
    appliedDate: new Date(Date.now() - 5 * 24 * 60 * 60 * 1000),
    lastUpdated: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000),
    positionId: 1,
    reviewNotes: null
  },
  {
    id: 2,
    position: 'Full Stack Developer',
    company: 'StartupXYZ',
    status: 'Accepted',
    appliedDate: new Date(Date.now() - 10 * 24 * 60 * 60 * 1000),
    lastUpdated: new Date(Date.now() - 3 * 24 * 60 * 60 * 1000),
    positionId: 2,
    reviewNotes: 'We loved your portfolio! Ready to move to next round.'
  },
  {
    id: 3,
    position: 'Frontend Developer',
    company: 'DesignStudio',
    status: 'Rejected',
    appliedDate: new Date(Date.now() - 15 * 24 * 60 * 60 * 1000),
    lastUpdated: new Date(Date.now() - 12 * 24 * 60 * 60 * 1000),
    positionId: 3,
    reviewNotes: 'Thank you for your interest. We decided to move forward with other candidates.'
  }
])

const loading = ref(false)
const selectedStatus = ref('')
const statuses = ['Submitted', 'Reviewing', 'Shortlisted', 'Rejected', 'Accepted']

const filteredApplications = computed(() => {
  if (!selectedStatus.value) return applications.value
  return applications.value.filter(app => app.status === selectedStatus.value)
})

/**
 * Get status color for badge
 */
const getStatusColor = (status) => {
  const colors = {
    'Submitted': 'bg-gray-100 text-gray-800',
    'Reviewing': 'bg-blue-100 text-blue-800',
    'Shortlisted': 'bg-purple-100 text-purple-800',
    'Accepted': 'bg-green-100 text-green-800',
    'Rejected': 'bg-red-100 text-red-800'
  }
  return colors[status] || 'bg-gray-100 text-gray-800'
}

/**
 * Get border color for application card
 */
const getBorderColor = (status) => {
  const colors = {
    'Submitted': 'border-gray-400',
    'Reviewing': 'border-blue-400',
    'Shortlisted': 'border-purple-400',
    'Accepted': 'border-green-400',
    'Rejected': 'border-red-400'
  }
  return colors[status] || 'border-gray-400'
}

/**
 * Format date for display
 */
const formatDate = (date) => {
  return new Date(date).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}

/**
 * Calculate days since application
 */
const daysAgo = (date) => {
  const now = new Date()
  const diff = now - new Date(date)
  return Math.floor(diff / (1000 * 60 * 60 * 24))
}

onMounted(async () => {
  loading.value = true
  try {
    // In production, fetch from API
    // const response = await axios.get(`${API_URL}/applications/my-applications`)
    // applications.value = response.data
  } catch (err) {
    console.error('Error fetching applications:', err)
  } finally {
    loading.value = false
  }
})
</script>

<style scoped>
@media (max-width: 768px) {
  h1 {
    font-size: 1.875rem;
  }
}
</style>
