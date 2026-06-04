package com.zoocare.mobile.ui.taskdetail

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.zoocare.mobile.data.remote.dto.TaskDto
import com.zoocare.mobile.data.repository.TaskRepository
import com.zoocare.mobile.ui.util.toUserMessage
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.flow.update
import kotlinx.coroutines.launch

data class TaskDetailUiState(
    val isLoading: Boolean = false,
    val task: TaskDto? = null,
    val isUpdating: Boolean = false,
    val error: String? = null
)

class TaskDetailViewModel(private val taskRepository: TaskRepository) : ViewModel() {

    private val _state = MutableStateFlow(TaskDetailUiState())
    val state: StateFlow<TaskDetailUiState> = _state.asStateFlow()

    fun load(taskId: Int) {
        _state.update { it.copy(isLoading = true, error = null) }
        viewModelScope.launch {
            taskRepository.getTask(taskId)
                .onSuccess { t -> _state.update { it.copy(isLoading = false, task = t) } }
                .onFailure { e -> _state.update { it.copy(isLoading = false, error = e.toUserMessage()) } }
        }
    }

    fun markDone() {
        val task = _state.value.task ?: return
        if (task.isDone) return
        _state.update { it.copy(isUpdating = true, error = null) }
        viewModelScope.launch {
            taskRepository.markDone(task.id)
                .onSuccess { updated -> _state.update { it.copy(isUpdating = false, task = updated) } }
                .onFailure { e -> _state.update { it.copy(isUpdating = false, error = e.toUserMessage()) } }
        }
    }
}
