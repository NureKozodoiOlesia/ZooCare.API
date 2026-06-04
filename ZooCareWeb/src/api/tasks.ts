import { api } from './client'
import type { CreateTaskDto, TaskDto } from './types'

export const getTasks = async (): Promise<TaskDto[]> => (await api.get<TaskDto[]>('/api/tasks')).data

export const getMyTasks = async (userId: number): Promise<TaskDto[]> =>
  (await api.get<TaskDto[]>(`/api/tasks/my-tasks/${userId}`)).data

export const createTask = async (dto: CreateTaskDto): Promise<TaskDto> =>
  (await api.post<TaskDto>('/api/tasks', dto)).data

export const updateTaskStatus = async (id: number, status: string): Promise<TaskDto> =>
  (await api.put<TaskDto>(`/api/tasks/${id}/status`, { status })).data

export const deleteTask = async (id: number): Promise<void> => {
  await api.delete(`/api/tasks/${id}`)
}
