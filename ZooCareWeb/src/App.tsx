import { Navigate, Route, Routes } from 'react-router-dom'
import { RequireAdmin, RequireAuth } from './auth/guards'
import LoginPage from './pages/LoginPage'
import UserLayout from './layouts/UserLayout'
import AdminLayout from './layouts/AdminLayout'
import UserDashboard from './pages/user/UserDashboard'
import UserTasks from './pages/user/UserTasks'
import UserAlerts from './pages/user/UserAlerts'
import UserAnimals from './pages/user/UserAnimals'
import UserEnclosures from './pages/user/UserEnclosures'
import AdminDashboard from './pages/admin/AdminDashboard'
import AdminUsers from './pages/admin/AdminUsers'
import AdminRoles from './pages/admin/AdminRoles'
import AdminAnimals from './pages/admin/AdminAnimals'
import AdminEnclosures from './pages/admin/AdminEnclosures'
import AdminTasks from './pages/admin/AdminTasks'
import AdminBackup from './pages/admin/AdminBackup'

export default function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />

      <Route
        path="/app"
        element={
          <RequireAuth>
            <UserLayout />
          </RequireAuth>
        }
      >
        <Route index element={<UserDashboard />} />
        <Route path="tasks" element={<UserTasks />} />
        <Route path="alerts" element={<UserAlerts />} />
        <Route path="animals" element={<UserAnimals />} />
        <Route path="enclosures" element={<UserEnclosures />} />
      </Route>

      <Route
        path="/admin"
        element={
          <RequireAdmin>
            <AdminLayout />
          </RequireAdmin>
        }
      >
        <Route index element={<AdminDashboard />} />
        <Route path="users" element={<AdminUsers />} />
        <Route path="roles" element={<AdminRoles />} />
        <Route path="animals" element={<AdminAnimals />} />
        <Route path="enclosures" element={<AdminEnclosures />} />
        <Route path="tasks" element={<AdminTasks />} />
        <Route path="backup" element={<AdminBackup />} />
      </Route>

      <Route path="*" element={<Navigate to="/login" replace />} />
    </Routes>
  )
}
