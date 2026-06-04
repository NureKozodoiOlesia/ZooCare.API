package com.zoocare.mobile.ui.util

import retrofit2.HttpException
import java.net.ConnectException
import java.net.SocketTimeoutException
import java.net.UnknownHostException

/** Перетворює виключення мережі/HTTP у зрозуміле користувачу повідомлення. */
fun Throwable.toUserMessage(): String = when (this) {
    is HttpException -> when (code()) {
        401 -> "Сесія недійсна. Увійдіть знову."
        403 -> "Недостатньо прав доступу"
        404 -> "Не знайдено"
        else -> "Помилка сервера (${code()})"
    }
    is ConnectException,
    is SocketTimeoutException,
    is UnknownHostException -> "Немає зв'язку з сервером. Перевірте, що бекенд запущено."
    else -> message ?: "Невідома помилка"
}
