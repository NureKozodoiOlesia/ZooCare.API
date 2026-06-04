import http from 'k6/http'
import { check } from 'k6'

// Сценарій: 15с розгону до 200 VU, 30с пік, 5с спад.
export const options = {
  stages: [
    { duration: '15s', target: 200 },
    { duration: '30s', target: 200 },
    { duration: '5s', target: 0 },
  ],
}

// Точка входу через nginx (load balancer). За потреби перевизначити: -e BASE_URL=...
const BASE = __ENV.BASE_URL || 'http://host.docker.internal:8080'

export default function () {
  // /api/alerts/unresolved без токена повертає 401, але повноцінно проходить
  // через nginx -> ASP.NET pipeline -> [Authorize], навантажуючи LB + застосунок.
  const res = http.get(`${BASE}/api/alerts/unresolved`)
  check(res, { 'no server error': (r) => r.status < 500 })
}
