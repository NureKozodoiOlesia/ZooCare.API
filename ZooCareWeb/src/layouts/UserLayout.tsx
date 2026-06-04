import DashboardIcon from '@mui/icons-material/Dashboard'
import ChecklistIcon from '@mui/icons-material/Checklist'
import NotificationsIcon from '@mui/icons-material/Notifications'
import PetsIcon from '@mui/icons-material/Pets'
import HomeWorkIcon from '@mui/icons-material/HomeWork'
import AppShell from '../components/AppShell'
import type { NavItem } from '../components/AppShell'

const items: NavItem[] = [
  { to: '/app', labelKey: 'nav.dashboard', icon: <DashboardIcon /> },
  { to: '/app/tasks', labelKey: 'nav.tasks', icon: <ChecklistIcon /> },
  { to: '/app/alerts', labelKey: 'nav.alerts', icon: <NotificationsIcon /> },
  { to: '/app/animals', labelKey: 'nav.animals', icon: <PetsIcon /> },
  { to: '/app/enclosures', labelKey: 'nav.enclosures', icon: <HomeWorkIcon /> },
]

export default function UserLayout() {
  return <AppShell titleKey="nav.userArea" items={items} />
}
