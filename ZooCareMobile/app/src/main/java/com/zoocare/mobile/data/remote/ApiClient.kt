package com.zoocare.mobile.data.remote

import com.zoocare.mobile.data.local.TokenStore
import okhttp3.OkHttpClient
import okhttp3.logging.HttpLoggingInterceptor
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory
import java.util.concurrent.TimeUnit

/**
 * Будує Retrofit-клієнт.
 *
 * BASE_URL: 10.0.2.2 — це адреса хост-машини з точки зору Android-емулятора,
 * тобто локальний бекенд `http://localhost:5049` доступний як `http://10.0.2.2:5049`.
 * Для фізичного пристрою заміни на IP свого ПК у локальній мережі.
 */
object ApiClient {

    const val BASE_URL = "http://10.0.2.2:5049/"

    fun create(tokenStore: TokenStore): ZooApi {
        val logging = HttpLoggingInterceptor().apply {
            level = HttpLoggingInterceptor.Level.BODY
        }

        val client = OkHttpClient.Builder()
            .addInterceptor(AuthInterceptor(tokenStore))
            .addInterceptor(logging)
            .connectTimeout(30, TimeUnit.SECONDS)
            .readTimeout(30, TimeUnit.SECONDS)
            .build()

        return Retrofit.Builder()
            .baseUrl(BASE_URL)
            .client(client)
            .addConverterFactory(GsonConverterFactory.create())
            .build()
            .create(ZooApi::class.java)
    }
}
