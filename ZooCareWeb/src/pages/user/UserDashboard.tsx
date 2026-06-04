import { Box, Card, CardContent, Typography } from '@mui/material'
import { useQuery } from '@tanstack/react-query'
import { useTranslation } from 'react-i18next'
import { getMyTasks } from '../../api/tasks'
import { getUnresolvedAlerts } from '../../api/alerts'
import { useAuth } from '../../auth/useAuth'

function StatCard({ label, value, color }: { label: string; value: number; color?: string }) {
  return (
    <Card sx={{ minWidth: 200, flex: '1 1 200px' }}>
      <CardContent>
        <Typography variant="h3" color={color}>
          {value}
        </Typography>
        <Typography color="text.secondary">{label}</Typography>
      </CardContent>
    </Card>
  )
}

export default function UserDashboard() {
  const { t } = useTranslation()
  const { session } = useAuth()
  const userId = session?.userId ?? 0

  const tasks = useQuery({ queryKey: ['my-tasks', userId], queryFn: () => getMyTasks(userId) })
  const alerts = useQuery({ queryKey: ['alerts', 'unresolved'], queryFn: getUnresolvedAlerts })

  const pending = (tasks.data ?? []).filter((x) => x.status !== 'Done').length
  const activeAlerts = (alerts.data ?? []).length

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        {t('dashboard.userTitle')}
      </Typography>
      <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 2 }}>
        <StatCard label={t('dashboard.myTasks')} value={pending} />
        <StatCard label={t('dashboard.activeAlerts')} value={activeAlerts} color="error.main" />
      </Box>
    </Box>
  )
}
