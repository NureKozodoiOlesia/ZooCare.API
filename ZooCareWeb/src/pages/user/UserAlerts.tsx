import {
  Box,
  Button,
  Card,
  CardContent,
  Chip,
  Stack,
  Typography,
} from '@mui/material'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useTranslation } from 'react-i18next'
import { getUnresolvedAlerts, resolveAlert } from '../../api/alerts'
import Loadable from '../../components/Loadable'
import { formatDateTime } from '../../l10n/format'

export default function UserAlerts() {
  const { t, i18n } = useTranslation()
  const qc = useQueryClient()

  const { data, isLoading, isError, refetch } = useQuery({
    queryKey: ['alerts', 'unresolved'],
    queryFn: getUnresolvedAlerts,
  })

  const resolve = useMutation({
    mutationFn: (id: number) => resolveAlert(id),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['alerts', 'unresolved'] }),
  })

  const alerts = data ?? []

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        {t('alerts.title')}
      </Typography>
      <Loadable
        isLoading={isLoading}
        isError={isError}
        isEmpty={alerts.length === 0}
        emptyText={t('alerts.noAlerts')}
        onRetry={refetch}
      >
        <Stack component="div" spacing={2}>
          {alerts.map((alert) => {
            const critical = alert.severity === 'Critical'
            return (
              <Card key={alert.id} sx={{ bgcolor: critical ? 'error.light' : undefined }}>
                <CardContent>
                  <Stack component="div"
                    direction="row"
                    spacing={2}
                    sx={{ justifyContent: 'space-between', alignItems: 'flex-start' }}
                  >
                    <Box>
                      <Chip
                        size="small"
                        color={critical ? 'error' : 'default'}
                        label={alert.severity}
                        sx={{ mb: 1 }}
                      />
                      <Typography variant="body1">{alert.message}</Typography>
                      <Typography variant="body2" color="text.secondary">
                        {alert.enclosureName} · {formatDateTime(alert.createdAt, i18n.language)}
                      </Typography>
                    </Box>
                    <Button
                      size="small"
                      disabled={resolve.isPending}
                      onClick={() => resolve.mutate(alert.id)}
                    >
                      {t('alerts.resolve')}
                    </Button>
                  </Stack>
                </CardContent>
              </Card>
            )
          })}
        </Stack>
      </Loadable>
    </Box>
  )
}
