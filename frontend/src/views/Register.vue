<template>
  <div class="min-h-screen bg-gradient-to-r from-indigo-600 to-purple-600 flex items-center justify-center px-4">
    <div class="max-w-md w-full bg-white shadow-lg rounded-lg p-8">
      <h2 class="text-3xl font-extrabold text-gray-900 text-center mb-6">{{ $t('auth.createAccount') }}</h2>

      <!-- Error Message -->
      <div v-if="error" class="mb-4 p-4 bg-red-100 border border-red-400 text-red-700 rounded-lg">
        {{ error }}
      </div>

      <!-- Registration Form -->
      <form @submit.prevent="handleRegister" class="space-y-6">
        <!-- Full Name Field -->
        <div>
          <label for="fullName" class="block text-sm font-medium text-gray-700 mb-1">
            {{ $t('auth.fullName') }}
          </label>
          <input
            v-model="formData.fullName"
            type="text"
            id="fullName"
            required
            class="w-full px-4 py-2 border border-gray-300 rounded-lg shadow-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent transition"
            :placeholder="$t('auth.fullName')"
          />
        </div>

        <!-- Email Field -->
        <div>
          <label for="email" class="block text-sm font-medium text-gray-700 mb-1">
            {{ $t('auth.email') }}
          </label>
          <input
            v-model="formData.email"
            type="email"
            id="email"
            required
            class="w-full px-4 py-2 border border-gray-300 rounded-lg shadow-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent transition"
            :placeholder="$t('auth.email')"
          />
        </div>

        <!-- Password Field -->
        <div>
          <label for="password" class="block text-sm font-medium text-gray-700 mb-1">
            {{ $t('auth.password') }}
          </label>
          <input
            v-model="formData.password"
            type="password"
            id="password"
            required
            class="w-full px-4 py-2 border border-gray-300 rounded-lg shadow-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent transition"
            :placeholder="$t('auth.password')"
          />
        </div>

        <!-- Account Type Field -->
        <div>
          <label for="role" class="block text-sm font-medium text-gray-700 mb-1">
            {{ $t('auth.accountType') }}
          </label>
          <select
            v-model="formData.role"
            id="role"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg shadow-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent transition"
          >
            <option value="Candidate">{{ $t('auth.jobCandidate') }}</option>
            <option value="Recruiter">{{ $t('auth.recruiter') }}</option>
          </select>
        </div>

        <!-- Submit Button -->
        <button
          type="submit"
          :disabled="loading"
          class="w-full py-2 px-4 bg-indigo-600 text-white font-medium rounded-lg hover:bg-indigo-700 disabled:bg-gray-400 disabled:cursor-not-allowed transition duration-200"
        >
          {{ loading ? $t('auth.creatingAccount') : $t('auth.createAccount') }}
        </button>
      </form>

      <!-- Login Link -->
      <p class="mt-6 text-center text-sm text-gray-600">
        {{ $t('auth.alreadyHaveAccount') }}
        <router-link to="/login" class="text-indigo-600 hover:text-indigo-500 font-medium">
          {{ $t('navigation.login') }}
        </router-link>
      </p>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/authStore'

const router = useRouter()
const authStore = useAuthStore()

const formData = ref({
  fullName: '',
  email: '',
  password: '',
  role: 'Candidate'
})

const error = ref('')
const loading = ref(false)

/**
 * Handle registration submission
 * Implements form validation and error handling
 */
const handleRegister = async () => {
  error.value = ''
  loading.value = true

  try {
    await authStore.register(
      formData.value.email,
      formData.value.fullName,
      formData.value.password,
      formData.value.role
    )
    router.push('/dashboard')
  } catch (err) {
    error.value = err || 'Registration failed. Please try again.'
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
/* Responsive design for mobile */
@media (max-width: 640px) {
  .container {
    padding: 1rem;
  }
}
</style>
