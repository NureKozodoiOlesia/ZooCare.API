import { useMemo } from 'react'
import {
  Box,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  Typography,
} from '@mui/material'
import { useQuery } from '@tanstack/react-query'
import { useTranslation } from 'react-i18next'
import { getEnclosures } from '../../api/enclosures'
import Loadable from '../../components/Loadable'
import { getCollator } from '../../l10n/format'

export default function UserEnclosures() {
  const { t, i18n } = useTranslation()

  const { data, isLoading, isError, refetch } = useQuery({
    queryKey: ['enclosures'],
    queryFn: getEnclosures,
  })

  const rows = useMemo(() => {
    const collator = getCollator(i18n.language)
    return [...(data ?? [])].sort((a, b) => collator.compare(a.name, b.name))
  }, [data, i18n.language])

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        {t('enclosures.title')}
      </Typography>
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
            </TableRow>
          </TableHead>
          <TableBody>
            {rows.map((e) => (
              <TableRow key={e.id}>
                <TableCell>{e.name}</TableCell>
                <TableCell>{e.type}</TableCell>
                <TableCell>{e.location}</TableCell>
                <TableCell>{e.animalCount}</TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </Loadable>
    </Box>
  )
}
