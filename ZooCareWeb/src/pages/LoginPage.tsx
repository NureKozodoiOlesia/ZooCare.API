import { useState } from 'react'
import { Navigate, useNavigate } from 'react-router-dom'
import axios from 'axios'
import {
  Alert,
  Box,
  Button,
  Card,
  CardContent,
  CircularProgress,
  Stack,
  TextField,
  Typography,
} from '@mui/material'
import { useTranslation } from 'react-i18next'
import { login } from '../api/auth'
import { useAuth } from '../auth/useAuth'
import LanguageSwitcher from '../components/LanguageSwitcher'

const ADMIN_ROLES = ['Адміністратор', 'Admin']

export default function LoginPage() {
  const { t } = useTranslation()
  const { signIn, isAuthenticated, isAdmin } = useAuth()
  const navigate = useNavigate()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState<string | null>(null)
  const [loading, setLoading] = useState(false)

  if (isAuthenticated) {
    return <Navigate to={isAdmin ? '/admin' : '/app'} replace />
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!email || !password) {
      setError(t('auth.emptyFields'))
      return
    }
    setLoading(true)
    setError(null)
    try {
      const result = await login(email, password)
      signIn(result)
      const admin = result.roles.some((r) => ADMIN_ROLES.includes(r))
      navigate(admin ? '/admin' : '/app', { replace: true })
    } catch (err) {
      if (axios.isAxiosError(err) && err.response?.status === 401) {
        setError(t('auth.invalidCredentials'))
      } else if (axios.isAxiosError(err) && !err.response) {
        setError(t('auth.networkError'))
      } else {
        setError(t('common.error'))
      }
    } finally {
      setLoading(false)
    }
  }

  return (
    <Box
      sx={{
        minHeight: '100vh',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        bgcolor: 'background.default',
        p: 2,
      }}
    >
      <Card sx={{ width: 400, maxWidth: '100%' }}>
        <CardContent>
          <Box sx={{ display: 'flex', justifyContent: 'flex-end', mb: 1 }}>
            <LanguageSwitcher />
          </Box>
          <Typography variant="h5" align="center">
            {t('auth.loginTitle')}
          </Typography>
          <Typography variant="body2" align="center" color="text.secondary" sx={{ mb: 3 }}>
            {t('auth.loginSubtitle')}
          </Typography>
          <form onSubmit={handleSubmit}>
            <Stack component="div" spacing={2}>
              <TextField
                label={t('auth.email')}
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                fullWidth
                autoComplete="email"
              />
              <TextField
                label={t('auth.password')}
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                fullWidth
                autoComplete="current-password"
              />
              {error && <Alert severity="error">{error}</Alert>}
              <Button type="submit" variant="contained" size="large" disabled={loading}>
                {loading ? <CircularProgress size={24} color="inherit" /> : t('auth.signIn')}
              </Button>
            </Stack>
          </form>
        </CardContent>
      </Card>
    </Box>
  )
}
