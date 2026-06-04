import { api } from './client'
import type { AnimalDto, CreateAnimalDto, UpdateAnimalDto } from './types'

export const getAnimals = async (): Promise<AnimalDto[]> =>
  (await api.get<AnimalDto[]>('/api/animals')).data

export const createAnimal = async (dto: CreateAnimalDto): Promise<AnimalDto> =>
  (await api.post<AnimalDto>('/api/animals', dto)).data

export const updateAnimal = async (id: number, dto: UpdateAnimalDto): Promise<AnimalDto> =>
  (await api.put<AnimalDto>(`/api/animals/${id}`, dto)).data

export const deleteAnimal = async (id: number): Promise<void> => {
  await api.delete(`/api/animals/${id}`)
}
