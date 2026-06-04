package com.zoocare.mobile.data.local

import android.content.Context
import androidx.datastore.preferences.core.edit
import androidx.datastore.preferences.core.intPreferencesKey
import androidx.datastore.preferences.core.stringPreferencesKey
import androidx.datastore.preferences.preferencesDataStore
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.first
import kotlinx.coroutines.flow.map

private val Context.dataStore by preferencesDataStore(name = "zoocare_session")

/**
 * Зберігає сесію доглядача (токен, id, ім'я) у DataStore.
 * Використовується як для авторизації запитів (AuthInterceptor),
 * так і для стартової навігації (login vs tasks).
 */
class TokenStore(private val context: Context) {

    private object Keys {
        val TOKEN = stringPreferencesKey("token")
        val USER_ID = intPreferencesKey("user_id")
        val FIRST_NAME = stringPreferencesKey("first_name")
    }

    val tokenFlow: Flow<String?> = context.dataStore.data.map { it[Keys.TOKEN] }
    val userIdFlow: Flow<Int?> = context.dataStore.data.map { it[Keys.USER_ID] }
    val firstNameFlow: Flow<String?> = context.dataStore.data.map { it[Keys.FIRST_NAME] }

    suspend fun save(token: String, userId: Int, firstName: String) {
        context.dataStore.edit { prefs ->
            prefs[Keys.TOKEN] = token
            prefs[Keys.USER_ID] = userId
            prefs[Keys.FIRST_NAME] = firstName
        }
    }

    suspend fun clear() {
        context.dataStore.edit { it.clear() }
    }

    /** Синхронне читання токена для OkHttp-інтерсептора (викликається у фоновому потоці). */
    suspend fun getTokenOnce(): String? = context.dataStore.data.first()[Keys.TOKEN]

    suspend fun getUserIdOnce(): Int? = context.dataStore.data.first()[Keys.USER_ID]

    suspend fun getFirstNameOnce(): String = context.dataStore.data.first()[Keys.FIRST_NAME] ?: ""
}
