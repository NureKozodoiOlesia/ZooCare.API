package com.zoocare.mobile.ui.alerts

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.zoocare.mobile.data.remote.dto.AlertDto
import com.zoocare.mobile.data.repository.AlertRepository
import com.zoocare.mobile.ui.util.toUserMessage
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.flow.update
import kotlinx.coroutines.launch

data class AlertsUiState(
    val isLoading: Boolean = false,
    val alerts: List<AlertDto> = emptyList(),
    val error: String? = null
)

class AlertsViewModel(private val alertRepository: AlertRepository) : ViewModel() {

    private val _state = MutableStateFlow(AlertsUiState())
    val state: StateFlow<AlertsUiState> = _state.asStateFlow()

    init {
        load()
    }

    fun load() {
        _state.update { it.copy(isLoading = true, error = null) }
        viewModelScope.launch {
            alertRepository.getUnresolved()
                .onSuccess { list -> _state.update { it.copy(isLoading = false, alerts = list) } }
                .onFailure { e -> _state.update { it.copy(isLoading = false, error = e.toUserMessage()) } }
        }
    }
}
