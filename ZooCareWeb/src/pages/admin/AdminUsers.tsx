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
import EditIcon from '@mui/icons-material/Edit'
import DeleteIcon from '@mui/icons-material/Delete'
import SecurityIcon from '@mui/icons-material/Security'
import KeyIcon from '@mui/icons-material/Key'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useTranslation } from 'react-i18next'
import {
  assignRole,
  createUser,
  deleteUser,
  getUsers,
  removeRole,
  updateUser,
  updateUserPassword,
} from '../../api/users'
import { getRoles } from '../../api/admin'
import type { UserDto } from '../../api/types'
import Loadable from '../../components/Loadable'

interface UserForm {
  userName: string
  email: string
  password: string
  firstName: string
  lastName: string
}

const EMPTY: UserForm = { userName: '', email: '', password: '', firstName: '', lastName: '' }

export default function AdminUsers() {
  const { t } = useTranslation()
  const qc = useQueryClient()
  const { data, isLoading, isError, refetch } = useQuery({ queryKey: ['users'], queryFn: getUsers })
  const roles = useQuery({ queryKey: ['roles'], queryFn: getRoles }).data ?? []
  const users = data ?? []

  const invalidate = () => qc.invalidateQueries({ queryKey: ['users'] })

  // Форма створення/редагування
  const [formOpen, setFormOpen] = useState(false)
  const [editing, setEditing] = useState<UserDto | null>(null)
  const [form, setForm] = useState<UserForm>(EMPTY)

  const saveUser = useMutation({
    mutationFn: () =>
      editing
        ? updateUser(editing.id, {
            firstName: form.firstName,
            lastName: form.lastName,
            email: form.email,
          })
        : createUser(form),
    onSuccess: () => {
      invalidate()
      setFormOpen(false)
    },
  })
  const removeUser = useMutation({ mutationFn: (id: number) => deleteUser(id), onSuccess: invalidate })

  // Діалог ролей
  const [rolesUserId, setRolesUserId] = useState<number | null>(null)
  const [newRole, setNewRole] = useState('')
  const rolesUser = users.find((u) => u.id === rolesUserId) ?? null
  const addRole = useMutation({
    mutationFn: (roleName: string) => assignRole(rolesUserId!, roleName),
    onSuccess: invalidate,
  })
  const dropRole = useMutation({
    mutationFn: (roleName: string) => removeRole(rolesUserId!, roleName),
    onSuccess: invalidate,
  })

  // Діалог пароля
  const [pwdUserId, setPwdUserId] = useState<number | null>(null)
  const [newPassword, setNewPassword] = useState('')
  const changePwd = useMutation({
    mutationFn: () => updateUserPassword(pwdUserId!, newPassword),
    onSuccess: () => {
      setPwdUserId(null)
      setNewPassword('')
    },
  })

  const openCreate = () => {
    setEditing(null)
    setForm(EMPTY)
    setFormOpen(true)
  }
  const openEdit = (u: UserDto) => {
    setEditing(u)
    setForm({ userName: u.userName, email: u.email, password: '', firstName: u.firstName, lastName: u.lastName })
    setFormOpen(true)
  }

  const availableRoles = roles.filter((r) => !rolesUser?.roles.includes(r.name))

  return (
    <Box>
      <Stack component="div" direction="row" sx={{ mb: 2, justifyContent: 'space-between', alignItems: 'center' }}>
        <Typography variant="h4">{t('users.title')}</Typography>
        <Button variant="contained" startIcon={<AddIcon />} onClick={openCreate}>
          {t('users.addUser')}
        </Button>
      </Stack>

      <Loadable isLoading={isLoading} isError={isError} isEmpty={users.length === 0} onRetry={refetch}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>{t('users.userName')}</TableCell>
              <TableCell>{t('users.firstName')}</TableCell>
              <TableCell>{t('users.email')}</TableCell>
              <TableCell>{t('users.roles')}</TableCell>
              <TableCell align="right">{t('common.actions')}</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {users.map((u) => (
              <TableRow key={u.id}>
                <TableCell>{u.userName}</TableCell>
                <TableCell>
                  {u.firstName} {u.lastName}
                </TableCell>
                <TableCell>{u.email}</TableCell>
                <TableCell>
                  <Stack component="div" direction="row" spacing={0.5} sx={{ flexWrap: 'wrap' }}>
                    {u.roles.map((r) => (
                      <Chip key={r} size="small" label={r} />
                    ))}
                  </Stack>
                </TableCell>
                <TableCell align="right">
                  <IconButton title={t('common.edit')} onClick={() => openEdit(u)}>
                    <EditIcon />
                  </IconButton>
                  <IconButton title={t('users.roles')} onClick={() => setRolesUserId(u.id)}>
                    <SecurityIcon />
                  </IconButton>
                  <IconButton title={t('users.changePassword')} onClick={() => setPwdUserId(u.id)}>
                    <KeyIcon />
                  </IconButton>
                  <IconButton
                    color="error"
                    title={t('common.delete')}
                    onClick={() => {
                      if (window.confirm(t('users.deleteConfirm', { name: u.email }))) removeUser.mutate(u.id)
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

      {/* Створення / редагування */}
      <Dialog open={formOpen} onClose={() => setFormOpen(false)} fullWidth maxWidth="sm">
        <DialogTitle>{editing ? t('users.editUser') : t('users.addUser')}</DialogTitle>
        <DialogContent>
          <Stack component="div" spacing={2} sx={{ mt: 1 }}>
            {!editing && (
              <TextField
                label={t('users.userName')}
                value={form.userName}
                onChange={(e) => setForm({ ...form, userName: e.target.value })}
              />
            )}
            <TextField
              label={t('users.firstName')}
              value={form.firstName}
              onChange={(e) => setForm({ ...form, firstName: e.target.value })}
            />
            <TextField
              label={t('users.lastName')}
              value={form.lastName}
              onChange={(e) => setForm({ ...form, lastName: e.target.value })}
            />
            <TextField
              label={t('users.email')}
              type="email"
              value={form.email}
              onChange={(e) => setForm({ ...form, email: e.target.value })}
            />
            {!editing && (
              <TextField
                label={t('users.password')}
                type="password"
                value={form.password}
                onChange={(e) => setForm({ ...form, password: e.target.value })}
              />
            )}
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setFormOpen(false)}>{t('common.cancel')}</Button>
          <Button variant="contained" disabled={saveUser.isPending} onClick={() => saveUser.mutate()}>
            {t('common.save')}
          </Button>
        </DialogActions>
      </Dialog>

      {/* Ролі користувача */}
      <Dialog open={rolesUserId !== null} onClose={() => setRolesUserId(null)} fullWidth maxWidth="xs">
        <DialogTitle>{t('users.roles')}</DialogTitle>
        <DialogContent>
          <Stack component="div" spacing={2} sx={{ mt: 1 }}>
            <Stack component="div" direction="row" spacing={1} sx={{ flexWrap: 'wrap' }}>
              {rolesUser?.roles.map((r) => (
                <Chip key={r} label={r} onDelete={() => dropRole.mutate(r)} />
              ))}
            </Stack>
            <Stack component="div" direction="row" spacing={1}>
              <TextField
                select
                size="small"
                fullWidth
                label={t('users.assignRole')}
                value={newRole}
                onChange={(e) => setNewRole(e.target.value)}
              >
                {availableRoles.map((r) => (
                  <MenuItem key={r.id} value={r.name}>
                    {r.name}
                  </MenuItem>
                ))}
              </TextField>
              <Button
                variant="contained"
                disabled={!newRole || addRole.isPending}
                onClick={() => {
                  addRole.mutate(newRole)
                  setNewRole('')
                }}
              >
                {t('common.add')}
              </Button>
            </Stack>
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setRolesUserId(null)}>{t('common.close')}</Button>
        </DialogActions>
      </Dialog>

      {/* Зміна пароля */}
      <Dialog open={pwdUserId !== null} onClose={() => setPwdUserId(null)} fullWidth maxWidth="xs">
        <DialogTitle>{t('users.changePassword')}</DialogTitle>
        <DialogContent>
          <TextField
            label={t('users.newPassword')}
            type="password"
            fullWidth
            value={newPassword}
            onChange={(e) => setNewPassword(e.target.value)}
            sx={{ mt: 1 }}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setPwdUserId(null)}>{t('common.cancel')}</Button>
          <Button
            variant="contained"
            disabled={!newPassword || changePwd.isPending}
            onClick={() => changePwd.mutate()}
          >
            {t('common.save')}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}
