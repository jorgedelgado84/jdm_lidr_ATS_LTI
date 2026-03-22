import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import axios from 'axios'

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:8080/api'

// Configure axios instance with base URL
const apiClient = axios.create({
  baseURL: API_URL
})

export const useAuthStore = defineStore('auth', () => {
  const user = ref(null)
  const token = ref(localStorage.getItem('token') || null)
  const isAuthenticated = computed(() => !!token.value && !!user.value)

  /**
   * Register a new user with validation
   * Implements security best practices
   */
  const register = async (email, fullName, password, role = 'Candidate') => {
    try {
      // Client-side validation
      if (!email || !fullName || !password) {
        throw new Error('All fields are required')
      }

      const response = await apiClient.post('/auth/register', {
        email,
        fullName,
        password,
        role
      })

      setAuthData(response.data)
      return response.data
    } catch (error) {
      console.error('Registration error:', error)
      throw error.response?.data?.message || error.message || 'Registration failed'
    }
  }

  /**
   * Login user with credentials
   */
  const login = async (email, password) => {
    try {
      if (!email || !password) {
        throw new Error('Email and password are required')
      }

      const response = await apiClient.post('/auth/login', {
        email,
        password
      })

      setAuthData(response.data)
      return response.data
    } catch (error) {
      console.error('Login error:', error)
      throw error.response?.data?.message || error.message || 'Login failed'
    }
  }

  /**
   * Logout user and clear authentication data
   */
  const logout = () => {
    user.value = null
    token.value = null
    localStorage.removeItem('token')
    localStorage.removeItem('user')
    delete apiClient.defaults.headers.common['Authorization']
  }

  /**
   * Check authentication status on app load
   * Restores session from localStorage if available
   */
  const checkAuthStatus = () => {
    const storedToken = localStorage.getItem('token')
    const storedUser = localStorage.getItem('user')

    if (storedToken && storedUser) {
      try {
        token.value = storedToken
        user.value = JSON.parse(storedUser)
        apiClient.defaults.headers.common['Authorization'] = `Bearer ${storedToken}`
      } catch (error) {
        console.error('Error restoring auth state:', error)
        logout()
      }
    }
  }

  /**
   * Set authentication data in store and storage
   * Updates API client headers for subsequent requests
   */
  const setAuthData = (authResponse) => {
    const { token: newToken, ...userData } = authResponse
    
    user.value = userData
    token.value = newToken

    localStorage.setItem('token', newToken)
    localStorage.setItem('user', JSON.stringify(userData))

    // Set Authorization header for all API requests
    apiClient.defaults.headers.common['Authorization'] = `Bearer ${newToken}`
  }

  return {
    user: computed(() => user.value),
    token: computed(() => token.value),
    isAuthenticated,
    register,
    login,
    logout,
    checkAuthStatus,
    apiClient
  }
})
