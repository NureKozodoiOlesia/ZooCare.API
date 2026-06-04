package com.zoocare.mobile.data.remote.dto

/** Завдання догляду (TaskDto з бекенда). */
data class TaskDto(
    val id: Int,
    val enclosureId: Int,
    val enclosureName: String,
    val assignedUserId: Int,
    val assignedUserName: String,
    val title: String,
    val description: String,
    val status: String,
    val dueDate: String,
    val completedAt: String?
) {
    val isDone: Boolean get() = status.equals("Done", ignoreCase = true)
}

/** Тіло запиту на зміну статусу: PUT /api/tasks/{id}/status */
data class UpdateStatusRequest(
    val status: String
)
