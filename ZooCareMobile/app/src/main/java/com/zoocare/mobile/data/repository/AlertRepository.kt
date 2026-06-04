package com.zoocare.mobile.data.repository

import com.zoocare.mobile.data.remote.ZooApi
import com.zoocare.mobile.data.remote.dto.AlertDto

class AlertRepository(private val api: ZooApi) {

    suspend fun getUnresolved(): Result<List<AlertDto>> = runCatching {
        api.getUnresolvedAlerts()
    }
}
