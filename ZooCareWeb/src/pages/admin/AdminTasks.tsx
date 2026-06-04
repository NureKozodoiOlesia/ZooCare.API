import { useState } from 'react'
import {
  Box,
  Button,
  Chip,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  IconButton,
  MenuItem,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  TextField,
  Typography,
} from '@mui/material'
import AddIcon from '@mui/icons-material/Add'
import DeleteIcon from '@mui/icons-material/Delete'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useTranslation } from 'react-i18next'
import { createTask, deleteTask, getTasks } from '../../api/tasks'
import { getEnclosures } from '../../api/enclosures'
import { getUsers } from '../../api/users'
import Loadable from '../../components/Loadable'
import { formatDateTime } from '../../l10n/format'

interface FormState {
  enclosureId: number
  assignedUserId: number
  title: string
  description: string
  dueDate: string
}

const EMPTY: FormState = { enclosureId: 0, assignedUserId: 0, title: '', description: '', dueDate: '' }

export default function AdminTasks() {
  const { t, i18n } = useTranslation()
  const qc = useQueryClient()
  const { data, isLoading, isError, refetch } = useQuery({ queryKey: ['tasks'], queryFn: getTasks })
  const enclosures = useQuery({ queryKey: ['enclosures'], queryFn: getEnclosures }).data ?? []
  const users = useQuery({ queryKey: ['users'], queryFn: getUsers }).data ?? []

  const [open, setOpen] = useState(false)
  const [form, setForm] = useState<FormState>(EMPTY)

  const invalidate = () => qc.invalidateQueries({ queryKey: ['tasks'] })
  const save = useMutation({
    mutationFn: () =>
      createTask({
        enclosureId: form.enclosureId,
        assignedUserId: form.assignedUserId,
        title: form.title,
        description: form.description,
        dueDate: new Date(form.dueDate).toISOString(),
      }),
    onSuccess: () => {
      invalidate()
      setOpen(false)
    },
  })
  const remove = useMutation({ mutationFn: (id: number) => deleteTask(id), onSuccess: invalidate })

  const openCreate = () => {
    setForm({
      ...EMPTY,
      enclosureId: enclosures[0]?.id ?? 0,
      assignedUserId: users[0]?.id ?? 0,
    })
    setOpen(true)
  }

  const rows = data ?? []

  return (
    <Box>
      <Stack component="div" direction="row" sx={{ mb: 2, justifyContent: 'space-between', alignItems: 'center' }}>
        <Typography variant="h4">{t('tasks.title')}</Typography>
        <Button variant="contained" startIcon={<AddIcon />} onClick={openCreate}>
          {t('tasks.addTask')}
        </Button>
      </Stack>

      <Loadable
        isLoading={isLoading}
        isError={isError}
        isEmpty={rows.length === 0}
        emptyText={t('tasks.noTasks')}
        onRetry={refetch}
      >
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>{t('tasks.name')}</TableCell>
              <TableCell>{t('tasks.enclosure')}</TableCell>
              <TableCell>{t('tasks.assignedTo')}</TableCell>
              <TableCell>{t('tasks.due')}</TableCell>
              <TableCell>{t('tasks.status')}</TableCell>
              <TableCell align="right">{t('common.actions')}</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {rows.map((row) => {
              const done = row.status === 'Done'
              return (
                <TableRow key={row.id}>
                  <TableCell>{row.title}</TableCell>
                  <TableCell>{row.enclosureName}</TableCell>
                  <TableCell>{row.assignedUserName}</TableCell>
                  <TableCell>{formatDateTime(row.dueDate, i18n.language)}</TableCell>
                  <TableCell>
                    <Chip
                      size="small"
                      color={done ? 'success' : 'warning'}
                      label={done ? t('tasks.done') : t('tasks.pending')}
                    />
                  </TableCell>
                  <TableCell align="right">
                    <IconButton
                      color="error"
                      onClick={() => {
                        if (window.confirm(`${t('common.delete')}: ${row.title}?`)) remove.mutate(row.id)
                      }}
                    >
                      <DeleteIcon />
                    </IconButton>
                  </TableCell>
                </TableRow>
              )
            })}
          </TableBody>
        </Table>
      </Loadable>

      <Dialog open={open} onClose={() => setOpen(false)} fullWidth maxWidth="sm">
        <DialogTitle>{t('tasks.addTask')}</DialogTitle>
        <DialogContent>
          <Stack component="div" spacing={2} sx={{ mt: 1 }}>
            <TextField
              label={t('tasks.name')}
              value={form.title}
              onChange={(e) => setForm({ ...form, title: e.target.value })}
            />
            <TextField
              label={t('tasks.description')}
              value={form.description}
              onChange={(e) => setForm({ ...form, description: e.target.value })}
              multiline
              minRows={2}
            />
            <TextField
              select
              label={t('tasks.enclosure')}
              value={form.enclosureId || ''}
              onChange={(e) => setForm({ ...form, enclosureId: Number(e.target.value) })}
            >
              {enclosures.map((en) => (
                <MenuItem key={en.id} value={en.id}>
                  {en.name}
                </MenuItem>
              ))}
            </TextField>
            <TextField
              select
              label={t('tasks.assignedTo')}
              value={form.assignedUserId || ''}
              onChange={(e) => setForm({ ...form, assignedUserId: Number(e.target.value) })}
            >
              {users.map((u) => (
                <MenuItem key={u.id} value={u.id}>
                  {u.firstName} {u.lastName} ({u.email})
                </MenuItem>
              ))}
            </TextField>
            <TextField
              label={t('tasks.due')}
              type="datetime-local"
              value={form.dueDate}
              onChange={(e) => setForm({ ...form, dueDate: e.target.value })}
              slotProps={{ inputLabel: { shrink: true } }}
            />
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpen(false)}>{t('common.cancel')}</Button>
          <Button
            variant="contained"
            disabled={!form.title || !form.enclosureId || !form.assignedUserId || !form.dueDate || save.isPending}
            onClick={() => save.mutate()}
          >
            {t('common.save')}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}
