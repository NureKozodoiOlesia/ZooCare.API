import { useState } from 'react'
import {
  Box,
  Button,
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
import DeleteIcon from '@mui/icons-material/Delete'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useTranslation } from 'react-i18next'
import { createRole, deleteRole, getRoles, initializeRoles } from '../../api/admin'
import Loadable from '../../components/Loadable'

export default function AdminRoles() {
  const { t } = useTranslation()
  const qc = useQueryClient()
  const [name, setName] = useState('')

  const { data, isLoading, isError, refetch } = useQuery({ queryKey: ['roles'], queryFn: getRoles })

  const invalidate = () => qc.invalidateQueries({ queryKey: ['roles'] })
  const add = useMutation({ mutationFn: () => createRole(name), onSuccess: () => { setName(''); invalidate() } })
  const remove = useMutation({ mutationFn: (id: number) => deleteRole(id), onSuccess: invalidate })
  const init = useMutation({ mutationFn: initializeRoles, onSuccess: invalidate })

  const roles = data ?? []

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        {t('roles.title')}
      </Typography>

      <Stack component="div" direction="row" spacing={2} sx={{ mb: 3, alignItems: 'center' }}>
        <TextField
          size="small"
          label={t('roles.name')}
          value={name}
          onChange={(e) => setName(e.target.value)}
        />
        <Button variant="contained" disabled={!name || add.isPending} onClick={() => add.mutate()}>
          {t('roles.addRole')}
        </Button>
        <Button variant="outlined" disabled={init.isPending} onClick={() => init.mutate()}>
          {t('roles.initRoles')}
        </Button>
      </Stack>

      <Loadable isLoading={isLoading} isError={isError} onRetry={refetch}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>{t('roles.name')}</TableCell>
              <TableCell align="right">{t('common.actions')}</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {roles.map((r) => (
              <TableRow key={r.id}>
                <TableCell>{r.name}</TableCell>
                <TableCell align="right">
                  <IconButton
                    color="error"
                    onClick={() => {
                      if (window.confirm(t('roles.deleteConfirm', { name: r.name }))) {
                        remove.mutate(r.id)
                      }
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
    </Box>
  )
}
