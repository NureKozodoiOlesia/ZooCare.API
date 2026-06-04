import type { ReactNode } from 'react'
import { Link as RouterLink, Outlet, useLocation } from 'react-router-dom'
import {
  AppBar,
  Box,
  Button,
  Drawer,
  List,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Toolbar,
  Typography,
} from '@mui/material'
import LogoutIcon from '@mui/icons-material/Logout'
import { useTranslation } from 'react-i18next'
import LanguageSwitcher from './LanguageSwitcher'
import { useAuth } from '../auth/useAuth'

const DRAWER_WIDTH = 240

export interface NavItem {
  to: string
  labelKey: string
  icon: ReactNode
}

interface Props {
  titleKey: string
  items: NavItem[]
}

export default function AppShell({ titleKey, items }: Props) {
  const { t } = useTranslation()
  const { session, signOut } = useAuth()
  const location = useLocation()

  return (
    <Box sx={{ display: 'flex' }}>
      <AppBar position="fixed" sx={{ zIndex: (theme) => theme.zIndex.drawer + 1 }}>
        <Toolbar sx={{ gap: 2 }}>
          <Typography variant="h6" sx={{ flexGrow: 1 }}>
            {t('common.appName')} · {t(titleKey)}
          </Typography>
          <LanguageSwitcher />
          {session && (
            <Typography variant="body2">
              {session.firstName} {session.lastName}
            </Typography>
          )}
          <Button color="inherit" startIcon={<LogoutIcon />} onClick={signOut}>
            {t('auth.signOut')}
          </Button>
        </Toolbar>
      </AppBar>

      <Drawer
        variant="permanent"
        sx={{
          width: DRAWER_WIDTH,
          flexShrink: 0,
          [`& .MuiDrawer-paper`]: { width: DRAWER_WIDTH, boxSizing: 'border-box' },
        }}
      >
        <Toolbar />
        <Box sx={{ overflow: 'auto' }}>
          <List>
            {items.map((item) => {
              const selected =
                location.pathname === item.to || location.pathname.startsWith(item.to + '/')
              return (
                <ListItemButton
                  key={item.to}
                  component={RouterLink}
                  to={item.to}
                  selected={selected}
                >
                  <ListItemIcon>{item.icon}</ListItemIcon>
                  <ListItemText primary={t(item.labelKey)} />
                </ListItemButton>
              )
            })}
          </List>
        </Box>
      </Drawer>

      <Box component="main" sx={{ flexGrow: 1, p: 3 }}>
        <Toolbar />
        <Outlet />
      </Box>
    </Box>
  )
}
