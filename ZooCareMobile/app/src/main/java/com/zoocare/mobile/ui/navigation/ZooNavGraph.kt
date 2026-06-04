package com.zoocare.mobile.ui.navigation

import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.material3.CircularProgressIndicator
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.rememberCoroutineScope
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.navigation.NavType
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.rememberNavController
import androidx.navigation.navArgument
import com.zoocare.mobile.di.ServiceLocator
import com.zoocare.mobile.ui.alerts.AlertsScreen
import com.zoocare.mobile.ui.login.LoginScreen
import com.zoocare.mobile.ui.taskdetail.TaskDetailScreen
import com.zoocare.mobile.ui.tasks.TaskListScreen
import kotlinx.coroutines.launch

@Composable
fun ZooApp() {
    val navController = rememberNavController()
    val scope = rememberCoroutineScope()

    // Стартовий екран залежить від наявності збереженого токена.
    var start by remember { mutableStateOf<String?>(null) }
    LaunchedEffect(Unit) {
        val token = ServiceLocator.tokenStore.getTokenOnce()
        start = if (token.isNullOrBlank()) Routes.LOGIN else Routes.TASKS
    }

    val startDestination = start
    if (startDestination == null) {
        Box(modifier = Modifier.fillMaxSize()) {
            CircularProgressIndicator(modifier = Modifier.align(Alignment.Center))
        }
        return
    }

    NavHost(navController = navController, startDestination = startDestination) {

        composable(Routes.LOGIN) {
            LoginScreen(
                onLoggedIn = {
                    navController.navigate(Routes.TASKS) {
                        popUpTo(Routes.LOGIN) { inclusive = true }
                    }
                }
            )
        }

        composable(Routes.TASKS) {
            TaskListScreen(
                onOpenTask = { taskId -> navController.navigate(Routes.taskDetail(taskId)) },
                onOpenAlerts = { navController.navigate(Routes.ALERTS) },
                onLogout = {
                    scope.launch {
                        ServiceLocator.authRepository.logout()
                        navController.navigate(Routes.LOGIN) {
                            popUpTo(Routes.TASKS) { inclusive = true }
                        }
                    }
                }
            )
        }

        composable(
            route = Routes.TASK_DETAIL,
            arguments = listOf(navArgument(Routes.TASK_ID_ARG) { type = NavType.IntType })
        ) { backStackEntry ->
            val taskId = backStackEntry.arguments?.getInt(Routes.TASK_ID_ARG) ?: 0
            TaskDetailScreen(
                taskId = taskId,
                onBack = { navController.popBackStack() }
            )
        }

        composable(Routes.ALERTS) {
            AlertsScreen(onBack = { navController.popBackStack() })
        }
    }
}
