import DashboardIcon from '@mui/icons-material/Dashboard'
import PeopleIcon from '@mui/icons-material/People'
import SecurityIcon from '@mui/icons-material/Security'
import PetsIcon from '@mui/icons-material/Pets'
import HomeWorkIcon from '@mui/icons-material/HomeWork'
import ChecklistIcon from '@mui/icons-material/Checklist'
import BackupIcon from '@mui/icons-material/Backup'
import AppShell from '../components/AppShell'
import type { NavItem } from '../components/AppShell'

const items: NavItem[] = [
  { to: '/admin', labelKey: 'nav.dashboard', icon: <DashboardIcon /> },
  { to: '/admin/users', labelKey: 'nav.users', icon: <PeopleIcon /> },
  { to: '/admin/roles', labelKey: 'nav.roles', icon: <SecurityIcon /> },
  { to: '/admin/animals', labelKey: 'nav.animals', icon: <PetsIcon /> },
  { to: '/admin/enclosures', labelKey: 'nav.enclosures', icon: <HomeWorkIcon /> },
  { to: '/admin/tasks', labelKey: 'nav.tasks', icon: <ChecklistIcon /> },
  { to: '/admin/backup', labelKey: 'nav.backup', icon: <BackupIcon /> },
]

export default function AdminLayout() {
  return <AppShell titleKey="nav.adminArea" items={items} />
}
