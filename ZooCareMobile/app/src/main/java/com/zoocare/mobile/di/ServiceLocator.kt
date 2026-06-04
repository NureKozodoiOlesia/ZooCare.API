package com.zoocare.mobile.di

import android.content.Context
import androidx.lifecycle.ViewModelProvider
import androidx.lifecycle.viewmodel.initializer
import androidx.lifecycle.viewmodel.viewModelFactory
import com.zoocare.mobile.data.local.TokenStore
import com.zoocare.mobile.data.remote.ApiClient
import com.zoocare.mobile.data.remote.ZooApi
import com.zoocare.mobile.data.repository.AlertRepository
import com.zoocare.mobile.data.repository.AuthRepository
import com.zoocare.mobile.data.repository.TaskRepository
import com.zoocare.mobile.ui.alerts.AlertsViewModel
import com.zoocare.mobile.ui.login.LoginViewModel
import com.zoocare.mobile.ui.taskdetail.TaskDetailViewModel
import com.zoocare.mobile.ui.tasks.TaskListViewModel

/**
 * Простий ручний DI: створює та роздає синглтони шарів даних,
 * а також єдину фабрику для всіх ViewModel.
 */
object ServiceLocator {

    lateinit var tokenStore: TokenStore
        private set

    private lateinit var api: ZooApi
    lateinit var authRepository: AuthRepository
        private set
    lateinit var taskRepository: TaskRepository
        private set
    lateinit var alertRepository: AlertRepository
        private set

    fun init(context: Context) {
        tokenStore = TokenStore(context.applicationContext)
        api = ApiClient.create(tokenStore)
        authRepository = AuthRepository(api, tokenStore)
        taskRepository = TaskRepository(api)
        alertRepository = AlertRepository(api)
    }

    val viewModelFactory: ViewModelProvider.Factory
        get() = viewModelFactory {
            initializer { LoginViewModel(authRepository) }
            initializer { TaskListViewModel(taskRepository, tokenStore) }
            initializer { TaskDetailViewModel(taskRepository) }
            initializer { AlertsViewModel(alertRepository) }
        }
}
