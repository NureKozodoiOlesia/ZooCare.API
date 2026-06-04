import { api } from './client'
import type { CreateEnclosureDto, EnclosureDto, UpdateEnclosureDto } from './types'

export const getEnclosures = async (): Promise<EnclosureDto[]> =>
  (await api.get<EnclosureDto[]>('/api/enclosures')).data

export const createEnclosure = async (dto: CreateEnclosureDto): Promise<EnclosureDto> =>
  (await api.post<EnclosureDto>('/api/enclosures', dto)).data

export const updateEnclosure = async (id: number, dto: UpdateEnclosureDto): Promise<EnclosureDto> =>
  (await api.put<EnclosureDto>(`/api/enclosures/${id}`, dto)).data

export const deleteEnclosure = async (id: number): Promise<void> => {
  await api.delete(`/api/enclosures/${id}`)
}
