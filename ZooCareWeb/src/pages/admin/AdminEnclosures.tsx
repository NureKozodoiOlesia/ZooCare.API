import { useState } from 'react'
import {
  Box,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  IconButton,
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
import EditIcon from '@mui/icons-material/Edit'
import DeleteIcon from '@mui/icons-material/Delete'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useTranslation } from 'react-i18next'
import { createEnclosure, deleteEnclosure, getEnclosures, updateEnclosure } from '../../api/enclosures'
import type { EnclosureDto } from '../../api/types'
import Loadable from '../../components/Loadable'

interface FormState {
  name: string
  type: string
  location: string
}

const EMPTY: FormState = { name: '', type: '', location: '' }

export default function AdminEnclosures() {
  const { t } = useTranslation()
  const qc = useQueryClient()
  const { data, isLoading, isError, refetch } = useQuery({ queryKey: ['enclosures'], queryFn: getEnclosures })

  const [open, setOpen] = useState(false)
  const [editing, setEditing] = useState<EnclosureDto | null>(null)
  const [form, setForm] = useState<FormState>(EMPTY)

  const invalidate = () => qc.invalidateQueries({ queryKey: ['enclosures'] })
  const save = useMutation({
    mutationFn: () =>
      editing ? updateEnclosure(editing.id, form) : createEnclosure(form),
    onSuccess: () => {
      invalidate()
      setOpen(false)
    },
  })
  const remove = useMutation({ mutationFn: (id: number) => deleteEnclosure(id), onSuccess: invalidate })

  const openCreate = () => {
    setEditing(null)
    setForm(EMPTY)
    setOpen(true)
  }
  const openEdit = (row: EnclosureDto) => {
    setEditing(row)
    setForm({ name: row.name, type: row.type, location: row.location })
    setOpen(true)
  }

  const rows = data ?? []

  return (
    <Box>
      <Stack component="div" direction="row" sx={{ mb: 2, justifyContent: 'space-between', alignItems: 'center' }}>
        <Typography variant="h4">{t('enclosures.title')}</Typography>
        <Button variant="contained" startIcon={<AddIcon />} onClick={openCreate}>
          {t('enclosures.addEnclosure')}
        </Button>
      </Stack>

      <Loadable
        isLoading={isLoading}
        isError={isError}
        isEmpty={rows.length === 0}
        emptyText={t('enclosures.noEnclosures')}
        onRetry={refetch}
      >
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>{t('enclosures.name')}</TableCell>
              <TableCell>{t('enclosures.type')}</TableCell>
              <TableCell>{t('enclosures.location')}</TableCell>
              <TableCell>{t('enclosures.animalCount')}</TableCell>
              <TableCell align="right">{t('common.actions')}</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {rows.map((row) => (
              <TableRow key={row.id}>
                <TableCell>{row.name}</TableCell>
                <TableCell>{row.type}</TableCell>
                <TableCell>{row.location}</TableCell>
                <TableCell>{row.animalCount}</TableCell>
                <TableCell align="right">
                  <IconButton onClick={() => openEdit(row)}>
                    <EditIcon />
                  </IconButton>
                  <IconButton
                    color="error"
                    onClick={() => {
                      if (window.confirm(`${t('common.delete')}: ${row.name}?`)) remove.mutate(row.id)
                    }}
                  >
                    <DeleteIcon />
                  </IconButton>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </Loadable>

      <Dialog open={open} onClose={() => setOpen(false)} fullWidth maxWidth="sm">
        <DialogTitle>{editing ? t('enclosures.editEnclosure') : t('enclosures.addEnclosure')}</DialogTitle>
        <DialogContent>
          <Stack component="div" spacing={2} sx={{ mt: 1 }}>
            <TextField
              label={t('enclosures.name')}
              value={form.name}
              onChange={(e) => setForm({ ...form, name: e.target.value })}
            />
            <TextField
              label={t('enclosures.type')}
              value={form.type}
              onChange={(e) => setForm({ ...form, type: e.target.value })}
            />
            <TextField
              label={t('enclosures.location')}
              value={form.location}
              onChange={(e) => setForm({ ...form, location: e.target.value })}
            />
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpen(false)}>{t('common.cancel')}</Button>
          <Button variant="contained" disabled={!form.name || save.isPending} onClick={() => save.mutate()}>
            {t('common.save')}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}
