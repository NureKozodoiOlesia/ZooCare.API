import { useEffect, useMemo } from 'react'
import type { ReactNode } from 'react'
import { createTheme, CssBaseline, ThemeProvider } from '@mui/material'
import { useTranslation } from 'react-i18next'

// Мови з письмом справа наліво (механізм готовий; uk/en — LTR)
const RTL_LANGS = ['ar', 'he', 'fa', 'ur']

function directionFor(lang: string): 'ltr' | 'rtl' {
  return RTL_LANGS.some((l) => lang.startsWith(l)) ? 'rtl' : 'ltr'
}

export function AppThemeProvider({ children }: { children: ReactNode }) {
  const { i18n } = useTranslation()
  const lang = i18n.language || 'uk'
  const direction = directionFor(lang)

  // Синхронізуємо <html lang> та <html dir> з обраною мовою
  useEffect(() => {
    document.documentElement.lang = lang
    document.documentElement.dir = direction
  }, [lang, direction])

  const theme = useMemo(
    () =>
      createTheme({
        direction,
        palette: {
          primary: { main: '#2e7d32' },
          secondary: { main: '#00796b' },
        },
      }),
    [direction],
  )

  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      {children}
    </ThemeProvider>
  )
}
