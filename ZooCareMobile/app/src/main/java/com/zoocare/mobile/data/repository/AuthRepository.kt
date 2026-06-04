package com.zoocare.mobile.data.repository

import com.zoocare.mobile.data.local.TokenStore
import com.zoocare.mobile.data.remote.ZooApi
import com.zoocare.mobile.data.remote.dto.AuthResult
import com.zoocare.mobile.data.remote.dto.LoginRequest

class AuthRepository(
    private val api: ZooApi,
    private val tokenStore: TokenStore
) {
    /** Логін: при успіху зберігає сесію у TokenStore. */
    suspend fun login(email: String, password: String): Result<AuthResult> = runCatching {
        val result = api.login(LoginRequest(email.trim(), password))
        tokenStore.save(result.token, result.userId, result.firstName)
        result
    }

    suspend fun logout() = tokenStore.clear()
}
