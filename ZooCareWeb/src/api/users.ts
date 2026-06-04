import { api } from './client'
import type { CreateUserDto, UpdateUserDto, UserDto } from './types'

export const getUsers = async (): Promise<UserDto[]> => (await api.get<UserDto[]>('/api/users')).data

export const createUser = async (dto: CreateUserDto): Promise<UserDto> =>
  (await api.post<UserDto>('/api/users', dto)).data

export const updateUser = async (id: number, dto: UpdateUserDto): Promise<UserDto> =>
  (await api.put<UserDto>(`/api/users/${id}`, dto)).data

export const deleteUser = async (id: number): Promise<void> => {
  await api.delete(`/api/users/${id}`)
}

export const assignRole = async (userId: number, roleName: string): Promise<void> => {
  await api.post('/api/auth/assign-role', { userId, roleName })
}

export const removeRole = async (userId: number, roleName: string): Promise<void> => {
  await api.delete(`/api/admin/users/${userId}/roles`, { params: { roleName } })
}

export const updateUserPassword = async (userId: number, newPassword: string): Promise<void> => {
  await api.put(`/api/admin/users/${userId}/password`, { newPassword })
}
