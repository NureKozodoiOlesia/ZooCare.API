import { api } from './client'
import type { AlertDto } from './types'

export const getAlerts = async (): Promise<AlertDto[]> => (await api.get<AlertDto[]>('/api/alerts')).data

export const getUnresolvedAlerts = async (): Promise<AlertDto[]> =>
  (await api.get<AlertDto[]>('/api/alerts/unresolved')).data

export const resolveAlert = async (id: number): Promise<AlertDto> =>
  (await api.put<AlertDto>(`/api/alerts/${id}/resolve`, {})).data
