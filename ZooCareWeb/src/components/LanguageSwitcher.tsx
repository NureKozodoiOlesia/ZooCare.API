import { MenuItem, Select } from '@mui/material'
import type { SelectChangeEvent } from '@mui/material'
import { useTranslation } from 'react-i18next'

export default function LanguageSwitcher() {
  const { i18n, t } = useTranslation()
  const current = i18n.language.startsWith('uk') ? 'uk' : 'en'

  const handleChange = (e: SelectChangeEvent) => {
    void i18n.changeLanguage(e.target.value)
  }

  return (
    <Select
      size="small"
      value={current}
      onChange={handleChange}
      aria-label={t('language.label')}
      sx={{ color: 'inherit', '.MuiOutlinedInput-notchedOutline': { borderColor: 'rgba(255,255,255,0.5)' }, '.MuiSvgIcon-root': { color: 'inherit' } }}
    >
      <MenuItem value="uk">{t('language.uk')}</MenuItem>
      <MenuItem value="en">{t('language.en')}</MenuItem>
    </Select>
  )
}
