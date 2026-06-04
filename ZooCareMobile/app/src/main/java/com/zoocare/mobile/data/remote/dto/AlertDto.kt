package com.zoocare.mobile.data.remote.dto

/** Системне сповіщення (AlertDto з бекенда). */
data class AlertDto(
    val id: Int,
    val enclosureId: Int,
    val enclosureName: String,
    val message: String,
    val severity: String,
    val isResolved: Boolean,
    val createdAt: String
) {
    val isCritical: Boolean get() = severity.equals("Critical", ignoreCase = true)
}
