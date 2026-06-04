package com.zoocare.mobile.ui.tasks

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.zoocare.mobile.data.local.TokenStore
import com.zoocare.mobile.data.remote.dto.TaskDto
import com.zoocare.mobile.data.repository.TaskRepository
import com.zoocare.mobile.ui.util.toUserMessage
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.flow.update
import kotlinx.coroutines.launch

data class TaskListUiState(
    val isLoading: Boolean = false,
    val tasks: List<TaskDto> = emptyList(),
    val firstName: String = "",
    val error: String? = null
) {
    val pendingCount: Int get() = tasks.count { !it.isDone }
}

class TaskListViewModel(
    private val taskRepository: TaskRepository,
    private val tokenStore: TokenStore
) : ViewModel() {

    private val _state = MutableStateFlow(TaskListUiState())
    val state: StateFlow<TaskListUiState> = _state.asStateFlow()

    init {
        load()
    }

    fun load() {
        _state.update { it.copy(isLoading = true, error = null) }
        viewModelScope.launch {
            _state.update { it.copy(firstName = tokenStore.getFirstNameOnce()) }
            val userId = tokenStore.getUserIdOnce()
            if (userId == null) {
                _state.update { it.copy(isLoading = false, error = "Сесія не знайдена") }
                return@launch
            }
            taskRepository.getMyTasks(userId)
                .onSuccess { list -> _state.update { it.copy(isLoading = false, tasks = list) } }
                .onFailure { e -> _state.update { it.copy(isLoading = false, error = e.toUserMessage()) } }
        }
    }
}
