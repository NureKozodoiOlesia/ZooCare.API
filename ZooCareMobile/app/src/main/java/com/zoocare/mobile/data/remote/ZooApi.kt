package com.zoocare.mobile.data.remote

import com.zoocare.mobile.data.remote.dto.AlertDto
import com.zoocare.mobile.data.remote.dto.AuthResult
import com.zoocare.mobile.data.remote.dto.LoginRequest
import com.zoocare.mobile.data.remote.dto.TaskDto
import com.zoocare.mobile.data.remote.dto.UpdateStatusRequest
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.POST
import retrofit2.http.PUT
import retrofit2.http.Path

/** Опис REST API бекенда ZooCare. */
interface ZooApi {

    @POST("api/auth/login")
    suspend fun login(@Body body: LoginRequest): AuthResult

    @GET("api/tasks/my-tasks/{userId}")
    suspend fun getMyTasks(@Path("userId") userId: Int): List<TaskDto>

    @GET("api/tasks/{id}")
    suspend fun getTaskById(@Path("id") id: Int): TaskDto

    @PUT("api/tasks/{id}/status")
    suspend fun updateTaskStatus(
        @Path("id") id: Int,
        @Body body: UpdateStatusRequest
    ): TaskDto

    @GET("api/alerts/unresolved")
    suspend fun getUnresolvedAlerts(): List<AlertDto>
}
