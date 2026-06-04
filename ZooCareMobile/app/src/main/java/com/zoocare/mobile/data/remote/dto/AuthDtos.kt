package com.zoocare.mobile.data.remote.dto

/** Тіло запиту на вхід: POST /api/auth/login */
data class LoginRequest(
    val email: String,
    val password: String
)

/** Відповідь авторизації від сервера (AuthResultDto). */
data class AuthResult(
    val token: String,
    val refreshToken: String,
    val userId: Int,
    val email: String,
    val firstName: String,
    val lastName: String,
    val roles: List<String> = emptyList()
)
