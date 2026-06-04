import { createContext, useCallback, useEffect, useMemo, useState } from 'react'
import type { ReactNode } from 'react'
import { TOKEN_KEY } from '../api/client'
import type { AuthResult } from '../api/types'

const SESSION_KEY = 'zoocare_session'
const ADMIN_ROLES = ['Адміністратор', 'Admin']

export interface Session {
  userId: number
  email: string
  firstName: string
  lastName: string
  roles: string[]
}

export interface AuthContextValue {
  session: Session | null
  isAuthenticated: boolean
  isAdmin: boolean
  signIn: (result: AuthResult) => void
  signOut: () => void
}

export const AuthContext = createContext<AuthContextValue | undefined>(undefined)

function readSession(): Session | null {
  const raw = localStorage.getItem(SESSION_KEY)
  if (!raw) return null
  try {
    return JSON.parse(raw) as Session
  } catch {
    return null
  }
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [session, setSession] = useState<Session | null>(readSession)

  const signIn = useCallback((result: AuthResult) => {
    localStorage.setItem(TOKEN_KEY, result.token)
    const next: Session = {
      userId: result.userId,
      email: result.email,
      firstName: result.firstName,
      lastName: result.lastName,
      roles: result.roles,
    }
    localStorage.setItem(SESSION_KEY, JSON.stringify(next))
    setSession(next)
  }, [])

  const signOut = useCallback(() => {
    localStorage.removeItem(TOKEN_KEY)
    localStorage.removeItem(SESSION_KEY)
    setSession(null)
  }, [])

  useEffect(() => {
    const handler = () => signOut()
    window.addEventListener('zoocare:unauthorized', handler)
    return () => window.removeEventListener('zoocare:unauthorized', handler)
  }, [signOut])

  const value = useMemo<AuthContextValue>(
    () => ({
      session,
      isAuthenticated: session !== null,
      isAdmin: session !== null && session.roles.some((r) => ADMIN_ROLES.includes(r)),
      signIn,
      signOut,
    }),
    [session, signIn, signOut],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}
