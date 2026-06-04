import axios from 'axios'

const baseURL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5049'

export const TOKEN_KEY = 'zoocare_token'

export const api = axios.create({ baseURL })

// Додаємо Bearer-токен до кожного запиту
api.interceptors.request.use((config) => {
  const token = localStorage.getItem(TOKEN_KEY)
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

// При 401 сповіщаємо застосунок (AuthProvider зробить logout)
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error?.response?.status === 401) {
      window.dispatchEvent(new Event('zoocare:unauthorized'))
    }
    return Promise.reject(error)
  },
)
