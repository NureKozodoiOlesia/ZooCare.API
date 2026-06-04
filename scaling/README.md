# ZooCare — горизонтальне масштабування (локально)

Локальний стенд для демонстрації горизонтального масштабування бекенда ZooCare:
кілька реплік ASP.NET API за nginx (round-robin) + один MS SQL Server. Навантаження — k6.

```
k6 -> nginx (:8080) -> api x N (:10000) -> mssql
```

## Передумови
- Docker Desktop (запущений, Engine running).
- ~4 ГБ RAM доступно (MS SQL-контейнер).

## Запуск

Усі команди виконувати з теки `scaling/`.

1. Підняти з однією реплікою (міграції БД застосуються один раз):
   ```
   docker compose up -d --build --scale api=1
   docker compose ps
   ```
   Дочекатись, поки `mssql` стане `healthy`, а `api` запуститься.

2. Навантажувальний тест (k6 у контейнері, без локальної установки):
   ```
   docker run --rm -i --add-host=host.docker.internal:host-gateway -v ${PWD}/loadtest:/scripts grafana/k6 run /scripts/script.js
   ```
   Записати `http_reqs` (RPS), `http_req_duration` p95, кількість запитів і помилок.

3. Масштабувати та повторити тест для 2 і 3 реплік (перед кожним тестом — перезапуск nginx,
   щоб LB підхопив нові адреси):
   ```
   docker compose up -d --scale api=2
   docker compose restart nginx
   # знову прогнати k6 (крок 2)

   docker compose up -d --scale api=3
   docker compose restart nginx
   # знову прогнати k6 (крок 2)
   ```

4. Переконатись, що балансування працює (різні `$upstream_addr` у логах nginx):
   ```
   docker compose logs nginx --tail 20
   ```

5. Зупинити стенд:
   ```
   docker compose down          # лишити дані БД
   docker compose down -v       # видалити й дані БД
   ```

## Файли
- `docker-compose.yml` — сервіси `mssql`, `api` (масштабується), `nginx`.
- `nginx/nginx.conf` — конфіг load balancer (upstream `api:10000`, лог з `$upstream_addr`).
- `loadtest/script.js` — сценарій k6 (200 VU).
