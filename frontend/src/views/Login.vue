<template>
  <div class="min-h-screen bg-gradient-to-r from-indigo-600 to-purple-600 flex items-center justify-center px-4">
    <div class="max-w-md w-full bg-white shadow-lg rounded-lg p-8">
      <h2 class="text-3xl font-extrabold text-gray-900 text-center mb-6">{{ $t('auth.signIn') }}</h2>

      <!-- Error Message -->
      <div v-if="error" class="mb-4 p-4 bg-red-100 border border-red-400 text-red-700 rounded-lg">
        {{ error }}
      </div>

      <!-- Login Form -->
      <form @submit.prevent="handleLogin" class="space-y-6">
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
            @blur="validateEmail"
          />
          <span v-if="errors.email" class="text-sm text-red-600 mt-1">{{ errors.email }}</span>
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

        <!-- Submit Button -->
        <button
          type="submit"
          :disabled="loading"
          class="w-full py-2 px-4 bg-indigo-600 text-white font-medium rounded-lg hover:bg-indigo-700 disabled:bg-gray-400 disabled:cursor-not-allowed transition duration-200"
        >
          {{ loading ? $t('auth.signingIn') : $t('auth.signIn') }}
        </button>
      </form>

      <!-- Register Link -->
      <p class="mt-6 text-center text-sm text-gray-600">
        {{ $t('auth.doNotHaveAccount') }}
        <router-link to="/register" class="text-indigo-600 hover:text-indigo-500 font-medium">
          {{ $t('navigation.register') }}
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
  email: '',
  password: ''
})

const error = ref('')
const loading = ref(false)
const errors = ref({})

/**
 * Validate email format
 */
const validateEmail = () => {
  const email = formData.value.email
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  
  if (!email) {
    errors.value.email = ''
  } else if (!emailRegex.test(email)) {
    errors.value.email = 'Please enter a valid email'
  } else {
    errors.value.email = ''
  }
}

/**
 * Handle login submission
 * Implements proper error handling and loading states
 */
const handleLogin = async () => {
  error.value = ''
  validateEmail()
  
  if (errors.value.email) return

  loading.value = true

  try {
    await authStore.login(formData.value.email, formData.value.password)
    router.push('/dashboard')
  } catch (err) {
    error.value = err || 'Login failed. Please check your credentials.'
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
/* Component-specific responsive styles */
@media (max-width: 640px) {
  .container {
    padding: 1rem;
  }
}
</style>
