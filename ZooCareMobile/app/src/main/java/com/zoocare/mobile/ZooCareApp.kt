package com.zoocare.mobile

import android.app.Application
import com.zoocare.mobile.di.ServiceLocator

class ZooCareApp : Application() {
    override fun onCreate() {
        super.onCreate()
        ServiceLocator.init(this)
    }
}
