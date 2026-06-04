import { useState } from 'react'
import {
  Box,
  Button,
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
import EditIcon from '@mui/icons-material/Edit'
import DeleteIcon from '@mui/icons-material/Delete'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useTranslation } from 'react-i18next'
import { createAnimal, deleteAnimal, getAnimals, updateAnimal } from '../../api/animals'
import { getEnclosures } from '../../api/enclosures'
import type { AnimalDto } from '../../api/types'
import Loadable from '../../components/Loadable'

interface FormState {
  name: string
  species: string
  age: number
  enclosureId: number
}

const EMPTY: FormState = { name: '', species: '', age: 0, enclosureId: 0 }

export default function AdminAnimals() {
  const { t } = useTranslation()
  const qc = useQueryClient()
  const { data, isLoading, isError, refetch } = useQuery({ queryKey: ['animals'], queryFn: getAnimals })
  const enclosuresQuery = useQuery({ queryKey: ['enclosures'], queryFn: getEnclosures })
  const enclosures = enclosuresQuery.data ?? []

  const [open, setOpen] = useState(false)
  const [editing, setEditing] = useState<AnimalDto | null>(null)
  const [form, setForm] = useState<FormState>(EMPTY)

  const invalidate = () => qc.invalidateQueries({ queryKey: ['animals'] })
  const save = useMutation({
    mutationFn: () => (editing ? updateAnimal(editing.id, form) : createAnimal(form)),
    onSuccess: () => {
      invalidate()
      setOpen(false)
    },
  })
  const remove = useMutation({ mutationFn: (id: number) => deleteAnimal(id), onSuccess: invalidate })

  const openCreate = () => {
    setEditing(null)
    setForm({ ...EMPTY, enclosureId: enclosures[0]?.id ?? 0 })
    setOpen(true)
  }
  const openEdit = (row: AnimalDto) => {
    setEditing(row)
    setForm({ name: row.name, species: row.species, age: row.age, enclosureId: row.enclosureId })
    setOpen(true)
  }

  const rows = data ?? []

  return (
    <Box>
      <Stack component="div" direction="row" sx={{ mb: 2, justifyContent: 'space-between', alignItems: 'center' }}>
        <Typography variant="h4">{t('animals.title')}</Typography>
        <Button variant="contained" startIcon={<AddIcon />} onClick={openCreate}>
          {t('animals.addAnimal')}
        </Button>
      </Stack>

      <Loadable
        isLoading={isLoading}
        isError={isError}
        isEmpty={rows.length === 0}
        emptyText={t('animals.noAnimals')}
        onRetry={refetch}
      >
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>{t('animals.name')}</TableCell>
              <TableCell>{t('animals.species')}</TableCell>
              <TableCell>{t('animals.age')}</TableCell>
              <TableCell>{t('animals.enclosure')}</TableCell>
              <TableCell align="right">{t('common.actions')}</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {rows.map((row) => (
              <TableRow key={row.id}>
                <TableCell>{row.name}</TableCell>
                <TableCell>{row.species}</TableCell>
                <TableCell>{row.age}</TableCell>
                <TableCell>{row.enclosureName}</TableCell>
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
        <DialogTitle>{editing ? t('animals.editAnimal') : t('animals.addAnimal')}</DialogTitle>
        <DialogContent>
          <Stack component="div" spacing={2} sx={{ mt: 1 }}>
            <TextField
              label={t('animals.name')}
              value={form.name}
              onChange={(e) => setForm({ ...form, name: e.target.value })}
            />
            <TextField
              label={t('animals.species')}
              value={form.species}
              onChange={(e) => setForm({ ...form, species: e.target.value })}
            />
            <TextField
              label={t('animals.age')}
              type="number"
              value={form.age}
              onChange={(e) => setForm({ ...form, age: Number(e.target.value) })}
            />
            <TextField
              select
              label={t('animals.enclosure')}
              value={form.enclosureId || ''}
              onChange={(e) => setForm({ ...form, enclosureId: Number(e.target.value) })}
            >
              {enclosures.map((e) => (
                <MenuItem key={e.id} value={e.id}>
                  {e.name}
                </MenuItem>
              ))}
            </TextField>
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpen(false)}>{t('common.cancel')}</Button>
          <Button
            variant="contained"
            disabled={!form.name || !form.enclosureId || save.isPending}
            onClick={() => save.mutate()}
          >
            {t('common.save')}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}
