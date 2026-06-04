import { useRef, useState } from 'react'
import {
  Alert,
  Box,
  Button,
  Card,
  CardContent,
  CircularProgress,
  Stack,
  Typography,
} from '@mui/material'
import DownloadIcon from '@mui/icons-material/Download'
import UploadIcon from '@mui/icons-material/Upload'
import { useTranslation } from 'react-i18next'
import { getUsers } from '../../api/users'
import { getAnimals, createAnimal } from '../../api/animals'
import { getEnclosures, createEnclosure } from '../../api/enclosures'
import { getTasks } from '../../api/tasks'
import { getAlerts } from '../../api/alerts'
import type { AnimalDto, EnclosureDto } from '../../api/types'

interface BackupFile {
  version: number
  exportedAt: string
  settings: { language: string }
  data: {
    users: unknown[]
    enclosures: EnclosureDto[]
    animals: AnimalDto[]
    tasks: unknown[]
    alerts: unknown[]
  }
}

export default function AdminBackup() {
  const { t, i18n } = useTranslation()
  const fileRef = useRef<HTMLInputElement>(null)
  const [busy, setBusy] = useState<'export' | 'import' | null>(null)
  const [message, setMessage] = useState<{ type: 'success' | 'error'; text: string } | null>(null)

  const handleExport = async () => {
    setBusy('export')
    setMessage(null)
    try {
      const [users, enclosures, animals, tasks, alerts] = await Promise.all([
        getUsers(),
        getEnclosures(),
        getAnimals(),
        getTasks(),
        getAlerts(),
      ])
      const backup: BackupFile = {
        version: 1,
        exportedAt: new Date().toISOString(),
        settings: { language: i18n.language },
        data: { users, enclosures, animals, tasks, alerts },
      }
      const blob = new Blob([JSON.stringify(backup, null, 2)], { type: 'application/json' })
      const url = URL.createObjectURL(blob)
      const a = document.createElement('a')
      a.href = url
      a.download = `zoocare-backup-${new Date().toISOString().slice(0, 10)}.json`
      a.click()
      URL.revokeObjectURL(url)
      setMessage({ type: 'success', text: t('backup.exportDone') })
    } catch {
      setMessage({ type: 'error', text: t('common.error') })
    } finally {
      setBusy(null)
    }
  }

  const handleImport = async (file: File) => {
    setBusy('import')
    setMessage(null)
    try {
      const text = await file.text()
      const backup = JSON.parse(text) as BackupFile
      if (!backup?.data || !backup?.settings) {
        setMessage({ type: 'error', text: t('backup.invalidFile') })
        return
      }

      // 1) Відновлюємо налаштування
      if (backup.settings.language) {
        await i18n.changeLanguage(backup.settings.language)
      }

      // 2) Відновлюємо вольєри, будуємо мапу стара-назва → новий id
      let created = 0
      let failed = 0
      const nameToNewId = new Map<string, number>()
      for (const enc of backup.data.enclosures ?? []) {
        try {
          const res = await createEnclosure({ name: enc.name, type: enc.type, location: enc.location })
          nameToNewId.set(enc.name, res.id)
          created++
        } catch {
          failed++
        }
      }

      // 3) Відновлюємо тварин (вольєр шукаємо за назвою)
      for (const animal of backup.data.animals ?? []) {
        const enclosureId = nameToNewId.get(animal.enclosureName)
        if (!enclosureId) {
          failed++
          continue
        }
        try {
          await createAnimal({
            name: animal.name,
            species: animal.species,
            age: animal.age,
            enclosureId,
          })
          created++
        } catch {
          failed++
        }
      }

      setMessage({ type: 'success', text: t('backup.importDone', { created, failed }) })
    } catch {
      setMessage({ type: 'error', text: t('backup.invalidFile') })
    } finally {
      setBusy(null)
      if (fileRef.current) fileRef.current.value = ''
    }
  }

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        {t('backup.title')}
      </Typography>

      {message && (
        <Alert severity={message.type} sx={{ mb: 2 }}>
          {message.text}
        </Alert>
      )}

      <Stack component="div" direction={{ xs: 'column', md: 'row' }} spacing={2}>
        <Card sx={{ flex: 1 }}>
          <CardContent>
            <Typography variant="h6">{t('backup.exportTitle')}</Typography>
            <Typography color="text.secondary" sx={{ mb: 2 }}>
              {t('backup.exportDesc')}
            </Typography>
            <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
              {t('backup.includesSettings')}
            </Typography>
            <Button
              variant="contained"
              startIcon={busy === 'export' ? <CircularProgress size={18} color="inherit" /> : <DownloadIcon />}
              disabled={busy !== null}
              onClick={handleExport}
            >
              {busy === 'export' ? t('backup.exporting') : t('backup.exportBtn')}
            </Button>
          </CardContent>
        </Card>

        <Card sx={{ flex: 1 }}>
          <CardContent>
            <Typography variant="h6">{t('backup.importTitle')}</Typography>
            <Typography color="text.secondary" sx={{ mb: 2 }}>
              {t('backup.importDesc')}
            </Typography>
            <input
              ref={fileRef}
              type="file"
              accept="application/json"
              hidden
              onChange={(e) => {
                const file = e.target.files?.[0]
                if (file) void handleImport(file)
              }}
            />
            <Button
              variant="outlined"
              startIcon={busy === 'import' ? <CircularProgress size={18} color="inherit" /> : <UploadIcon />}
              disabled={busy !== null}
              onClick={() => fileRef.current?.click()}
            >
              {busy === 'import' ? t('backup.importing') : t('backup.importBtn')}
            </Button>
          </CardContent>
        </Card>
      </Stack>
    </Box>
  )
}
