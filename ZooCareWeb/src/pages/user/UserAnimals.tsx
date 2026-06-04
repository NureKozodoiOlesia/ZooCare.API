import { useMemo, useState } from 'react'
import {
  Box,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  TextField,
  Typography,
} from '@mui/material'
import { useQuery } from '@tanstack/react-query'
import { useTranslation } from 'react-i18next'
import { getAnimals } from '../../api/animals'
import Loadable from '../../components/Loadable'
import { getCollator } from '../../l10n/format'

export default function UserAnimals() {
  const { t, i18n } = useTranslation()
  const [search, setSearch] = useState('')

  const { data, isLoading, isError, refetch } = useQuery({
    queryKey: ['animals'],
    queryFn: getAnimals,
  })

  const rows = useMemo(() => {
    const collator = getCollator(i18n.language)
    const list = (data ?? []).filter(
      (a) =>
        a.name.toLowerCase().includes(search.toLowerCase()) ||
        a.species.toLowerCase().includes(search.toLowerCase()),
    )
    return [...list].sort((a, b) => collator.compare(a.name, b.name))
  }, [data, search, i18n.language])

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        {t('animals.title')}
      </Typography>
      <TextField
        size="small"
        label={t('common.search')}
        value={search}
        onChange={(e) => setSearch(e.target.value)}
        sx={{ mb: 2 }}
      />
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
            </TableRow>
          </TableHead>
          <TableBody>
            {rows.map((a) => (
              <TableRow key={a.id}>
                <TableCell>{a.name}</TableCell>
                <TableCell>{a.species}</TableCell>
                <TableCell>{a.age}</TableCell>
                <TableCell>{a.enclosureName}</TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </Loadable>
    </Box>
  )
}
