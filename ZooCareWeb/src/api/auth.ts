import { api } from './client'
import type { AuthResult } from './types'

export async function login(email: string, password: string): Promise<AuthResult> {
  const { data } = await api.post<AuthResult>('/api/auth/login', { email, password })
  return data
}
