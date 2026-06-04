package com.zoocare.mobile.ui.util

/**
 * Спрощене форматування ISO-дати від .NET (напр. "2026-05-31T10:00:00") у
 * вигляд "2026-05-31 10:00". Без зовнішніх залежностей — достатньо для відображення.
 */
fun formatDateTime(iso: String?): String {
    if (iso.isNullOrBlank()) return "—"
    val trimmed = if (iso.length >= 16) iso.substring(0, 16) else iso
    return trimmed.replace('T', ' ')
}
