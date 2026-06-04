import type { ReactNode } from 'react'
import { Alert, Box, Button, CircularProgress, Typography } from '@mui/material'
import { useTranslation } from 'react-i18next'

interface Props {
  isLoading: boolean
  isError: boolean
  isEmpty?: boolean
  emptyText?: string
  onRetry?: () => void
  children: ReactNode
}

export default function Loadable({ isLoading, isError, isEmpty, emptyText, onRetry, children }: Props) {
  const { t } = useTranslation()

  if (isLoading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
        <CircularProgress />
      </Box>
    )
  }
  if (isError) {
    return (
      <Box sx={{ p: 4, textAlign: 'center' }}>
        <Alert severity="error">{t('common.error')}</Alert>
        {onRetry && (
          <Button onClick={onRetry} sx={{ mt: 2 }}>
            {t('common.retry')}
          </Button>
        )}
      </Box>
    )
  }
  if (isEmpty) {
    return (
      <Typography color="text.secondary" sx={{ p: 2 }}>
        {emptyText ?? t('common.noData')}
      </Typography>
    )
  }
  return <>{children}</>
}
