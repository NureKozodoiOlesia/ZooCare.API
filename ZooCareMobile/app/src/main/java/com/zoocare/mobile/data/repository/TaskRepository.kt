package com.zoocare.mobile.data.repository

import com.zoocare.mobile.data.remote.ZooApi
import com.zoocare.mobile.data.remote.dto.TaskDto
import com.zoocare.mobile.data.remote.dto.UpdateStatusRequest

class TaskRepository(private val api: ZooApi) {

    suspend fun getMyTasks(userId: Int): Result<List<TaskDto>> = runCatching {
        api.getMyTasks(userId)
    }

    suspend fun getTask(id: Int): Result<TaskDto> = runCatching {
        api.getTaskById(id)
    }

    suspend fun setStatus(id: Int, status: String): Result<TaskDto> = runCatching {
        api.updateTaskStatus(id, UpdateStatusRequest(status))
    }

    suspend fun markDone(id: Int): Result<TaskDto> = setStatus(id, "Done")
}
