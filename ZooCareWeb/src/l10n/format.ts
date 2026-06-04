// Локалезалежне форматування дат/чисел та сортування

function localeTag(lang: string): string {
  return lang.startsWith('uk') ? 'uk-UA' : 'en-US'
}

export function formatDateTime(iso: string | null | undefined, lang: string): string {
  if (!iso) return '—'
  const date = new Date(iso)
  if (Number.isNaN(date.getTime())) return '—'
  return new Intl.DateTimeFormat(localeTag(lang), {
    dateStyle: 'medium',
    timeStyle: 'short',
  }).format(date)
}

export function formatNumber(value: number, lang: string): string {
  return new Intl.NumberFormat(localeTag(lang)).format(value)
}

// Колатор для локалезалежного сортування рядків (укр. абетка vs англ.)
export function getCollator(lang: string): Intl.Collator {
  return new Intl.Collator(localeTag(lang), { sensitivity: 'base', numeric: true })
}
