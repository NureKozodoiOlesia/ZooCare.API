import { api } from './client'
import type { AdminStatsDto, RoleDto } from './types'

export const getStats = async (): Promise<AdminStatsDto> =>
  (await api.get<AdminStatsDto>('/api/admin/stats')).data

export const getRoles = async (): Promise<RoleDto[]> =>
  (await api.get<RoleDto[]>('/api/admin/roles')).data

export const createRole = async (name: string): Promise<RoleDto> =>
  (await api.post<RoleDto>('/api/admin/roles', { name })).data

export const deleteRole = async (id: number): Promise<void> => {
  await api.delete(`/api/admin/roles/${id}`)
}

export const initializeRoles = async (): Promise<void> => {
  await api.post('/api/admin/roles/initialize')
}
