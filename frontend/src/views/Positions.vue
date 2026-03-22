<template>
  <div class="py-8">
    <!-- Header -->
    <div class="mb-8">
      <h1 class="text-4xl font-bold text-gray-900">{{ $t('positions.openPositions') }}</h1>
      <p class="mt-2 text-gray-600">{{ $t('positions.browsePositions') }}</p>
    </div>

    <!-- Error Message -->
    <div v-if="error" class="mb-4 p-4 bg-red-100 border border-red-400 text-red-700 rounded-lg">
      {{ error }}
    </div>

    <!-- Search Bar -->
    <div class="mb-6 flex gap-4 flex-col sm:flex-row">
      <input
        v-model="searchQuery"
        type="text"
        :placeholder="$t('positions.search')"
        class="flex-1 px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500"
      />
      <button
        @click="searchPositions"
        class="px-6 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition font-medium"
      >
        {{ $t('buttons.submit') }}
      </button>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="text-center py-12">
      <p class="text-gray-600">{{ $t('positions.loadingPositions') }}</p>
    </div>

    <!-- Empty State -->
    <div v-else-if="filteredPositions.length === 0" class="text-center py-12 bg-white rounded-lg shadow">
      <p class="text-gray-600">{{ $t('positions.noPositionsFound') }}</p>
    </div>

    <!-- Positions Grid -->
    <div v-else class="grid gap-6">
      <div
        v-for="position in filteredPositions"
        :key="position.id"
        class="bg-white rounded-lg shadow hover:shadow-lg transition p-6 border-l-4 border-indigo-600"
      >
        <div class="flex justify-between items-start gap-4">
          <div class="flex-1 min-w-0">
            <h3 class="text-xl font-bold text-gray-900">{{ position.title }}</h3>
            <p class="text-gray-600 mt-1">{{ position.department }} • {{ position.location }}</p>
            <p class="text-gray-700 mt-3 line-clamp-2">{{ position.description }}</p>
            
            <div class="mt-4 flex flex-wrap gap-4 text-sm text-gray-600">
              <span class="flex items-center gap-1">
                💰 {{ $t('positions.salary') }}: ${{ formatCurrency(position.salaryMin) }} - ${{ formatCurrency(position.salaryMax) }}
              </span>
              <span class="flex items-center gap-1">
                📅 {{ $t('positions.postedOn') }}: {{ formatDate(position.createdAt) }}
              </span>
            </div>
          </div>
          
          <router-link
            :to="`/positions/${position.id}`"
            class="ml-4 px-6 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 whitespace-nowrap transition font-medium"
          >
            {{ $t('positions.viewDetails') }}
          </router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import axios from 'axios'
import { useAuthStore } from '../stores/authStore'

const authStore = useAuthStore()
const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:8080/api'

const positions = ref([])
const searchQuery = ref('')
const error = ref('')
const loading = ref(true)

const filteredPositions = computed(() => {
  if (!searchQuery.value) return positions.value
  
  const query = searchQuery.value.toLowerCase()
  return positions.value.filter(p =>
    p.title.toLowerCase().includes(query) ||
    p.department.toLowerCase().includes(query) ||
    p.description.toLowerCase().includes(query)
  )
})

/**
 * Fetch all open positions from API
 */
const fetchPositions = async () => {
  loading.value = true
  error.value = ''
  
  try {
    const response = await axios.get(`${API_URL}/positions/open`)
    positions.value = response.data || []
  } catch (err) {
    error.value = err.response?.data?.message || 'Failed to load positions'
    console.error('Error fetching positions:', err)
  } finally {
    loading.value = false
  }
}

/**
 * Search positions by query
 */
const searchPositions = async () => {
  if (!searchQuery.value.trim()) {
    await fetchPositions()
    return
  }

  loading.value = true
  error.value = ''
  
  try {
    const response = await axios.get(`${API_URL}/positions/search/${searchQuery.value}`)
    positions.value = response.data || []
  } catch (err) {
    error.value = err.response?.data?.message || 'Search failed'
    console.error('Error searching positions:', err)
  } finally {
    loading.value = false
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
    month: 'short',
    day: 'numeric'
  })
}

onMounted(() => {
  fetchPositions()
})
</script>

<style scoped>
@media (max-width: 768px) {
  h1 {
    font-size: 1.875rem;
  }
  
  .gap-4 {
    flex-direction: column;
  }
}
</style>
