// Типи DTO, що відповідають контракту бекенда ZooCare.API

export interface AuthResult {
  token: string
  refreshToken: string
  userId: number
  email: string
  firstName: string
  lastName: string
  roles: string[]
}

export interface UserDto {
  id: number
  userName: string
  email: string
  firstName: string
  lastName: string
  roles: string[]
}
export interface CreateUserDto {
  userName: string
  email: string
  password: string
  firstName: string
  lastName: string
}
export interface UpdateUserDto {
  firstName?: string
  lastName?: string
  email?: string
}

export interface EnclosureDto {
  id: number
  name: string
  type: string
  location: string
  animalCount: number
}
export interface CreateEnclosureDto {
  name: string
  type: string
  location: string
}
export interface UpdateEnclosureDto {
  name?: string
  type?: string
  location?: string
}

export interface AnimalDto {
  id: number
  enclosureId: number
  enclosureName: string
  name: string
  species: string
  age: number
}
export interface CreateAnimalDto {
  enclosureId: number
  name: string
  species: string
  age: number
}
export interface UpdateAnimalDto {
  enclosureId?: number
  name?: string
  species?: string
  age?: number
}

export interface TaskDto {
  id: number
  enclosureId: number
  enclosureName: string
  assignedUserId: number
  assignedUserName: string
  title: string
  description: string
  status: string
  dueDate: string
  completedAt: string | null
}
export interface CreateTaskDto {
  enclosureId: number
  assignedUserId: number
  title: string
  description?: string
  dueDate: string
}

export interface AlertDto {
  id: number
  enclosureId: number
  enclosureName: string
  message: string
  severity: string
  isResolved: boolean
  createdAt: string
}

export interface RoleDto {
  id: number
  name: string
  normalizedName: string
}

export interface AdminStatsDto {
  totalUsers: number
  totalAnimals: number
  totalEnclosures: number
  totalTasks: number
  pendingTasks: number
  completedTasks: number
  activeAlerts: number
  totalAlerts: number
  onlineDevices: number
  totalDevices: number
}
