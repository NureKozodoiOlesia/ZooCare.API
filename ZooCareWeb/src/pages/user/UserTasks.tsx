import {
  Box,
  Button,
  Chip,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  Typography,
} from '@mui/material'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useTranslation } from 'react-i18next'
import { getMyTasks, updateTaskStatus } from '../../api/tasks'
import { useAuth } from '../../auth/useAuth'
import Loadable from '../../components/Loadable'
import { formatDateTime } from '../../l10n/format'

export default function UserTasks() {
  const { t, i18n } = useTranslation()
  const { session } = useAuth()
  const userId = session?.userId ?? 0
  const qc = useQueryClient()

  const { data, isLoading, isError, refetch } = useQuery({
    queryKey: ['my-tasks', userId],
    queryFn: () => getMyTasks(userId),
  })

  const markDone = useMutation({
    mutationFn: (id: number) => updateTaskStatus(id, 'Done'),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['my-tasks', userId] }),
  })

  const tasks = data ?? []

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        {t('tasks.myChecklist')}
      </Typography>
      <Loadable
        isLoading={isLoading}
        isError={isError}
        isEmpty={tasks.length === 0}
        emptyText={t('tasks.noTasks')}
        onRetry={refetch}
      >
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>{t('tasks.name')}</TableCell>
              <TableCell>{t('tasks.enclosure')}</TableCell>
              <TableCell>{t('tasks.due')}</TableCell>
              <TableCell>{t('tasks.status')}</TableCell>
              <TableCell align="right">{t('common.actions')}</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {tasks.map((task) => {
              const done = task.status === 'Done'
              return (
                <TableRow key={task.id}>
                  <TableCell>{task.title}</TableCell>
                  <TableCell>{task.enclosureName}</TableCell>
                  <TableCell>{formatDateTime(task.dueDate, i18n.language)}</TableCell>
                  <TableCell>
                    <Chip
                      size="small"
                      color={done ? 'success' : 'warning'}
                      label={done ? t('tasks.done') : t('tasks.pending')}
                    />
                  </TableCell>
                  <TableCell align="right">
                    {!done && (
                      <Button
                        size="small"
                        variant="contained"
                        disabled={markDone.isPending}
                        onClick={() => markDone.mutate(task.id)}
                      >
                        {t('tasks.markDone')}
                      </Button>
                    )}
                  </TableCell>
                </TableRow>
              )
            })}
          </TableBody>
        </Table>
      </Loadable>
    </Box>
  )
}
