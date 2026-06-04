import { Box, Card, CardContent, Typography } from '@mui/material'
import { useQuery } from '@tanstack/react-query'
import { useTranslation } from 'react-i18next'
import { getStats } from '../../api/admin'
import Loadable from '../../components/Loadable'
import { formatNumber } from '../../l10n/format'

export default function AdminDashboard() {
  const { t, i18n } = useTranslation()
  const { data, isLoading, isError, refetch } = useQuery({ queryKey: ['stats'], queryFn: getStats })

  const cards: { key: string; value: number; color?: string }[] = data
    ? [
        { key: 'totalUsers', value: data.totalUsers },
        { key: 'totalAnimals', value: data.totalAnimals },
        { key: 'totalEnclosures', value: data.totalEnclosures },
        { key: 'totalTasks', value: data.totalTasks },
        { key: 'pendingTasks', value: data.pendingTasks, color: 'warning.main' },
        { key: 'completedTasks', value: data.completedTasks, color: 'success.main' },
        { key: 'activeAlerts', value: data.activeAlerts, color: 'error.main' },
        { key: 'totalAlerts', value: data.totalAlerts },
        { key: 'onlineDevices', value: data.onlineDevices, color: 'success.main' },
        { key: 'totalDevices', value: data.totalDevices },
      ]
    : []

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        {t('dashboard.adminTitle')}
      </Typography>
      <Loadable isLoading={isLoading} isError={isError} onRetry={refetch}>
        <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 2 }}>
          {cards.map((c) => (
            <Card key={c.key} sx={{ minWidth: 180, flex: '1 1 180px' }}>
              <CardContent>
                <Typography variant="h4" color={c.color}>
                  {formatNumber(c.value, i18n.language)}
                </Typography>
                <Typography color="text.secondary">{t(`dashboard.${c.key}`)}</Typography>
              </CardContent>
            </Card>
          ))}
        </Box>
      </Loadable>
    </Box>
  )
}
