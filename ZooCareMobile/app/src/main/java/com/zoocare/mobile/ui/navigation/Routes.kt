package com.zoocare.mobile.ui.navigation

object Routes {
    const val LOGIN = "login"
    const val TASKS = "tasks"
    const val ALERTS = "alerts"

    const val TASK_DETAIL = "task/{taskId}"
    const val TASK_ID_ARG = "taskId"
    fun taskDetail(taskId: Int) = "task/$taskId"
}
