package com.zoocare.mobile

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.material3.Surface
import androidx.compose.ui.Modifier
import com.zoocare.mobile.ui.navigation.ZooApp
import com.zoocare.mobile.ui.theme.ZooCareMobileTheme

class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContent {
            ZooCareMobileTheme {
                Surface(modifier = Modifier.fillMaxSize()) {
                    ZooApp()
                }
            }
        }
    }
}
