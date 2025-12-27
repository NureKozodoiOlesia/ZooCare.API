# Деплой на Render.com - Покрокова інструкція

## Крок 1: Підготовка проекту

1. Переконайтеся, що всі зміни закомічені на GitHub:
   ```bash
   git add .
   git commit -m "Prepare for Render deployment"
   git push
   ```

## Крок 2: Створення акаунту на Render.com

1. Відкрийте https://render.com
2. Натисніть **"Get Started for Free"**
3. Увійдіть через GitHub (рекомендовано) або створіть акаунт

## Крок 3: Створення нового Web Service

1. На головній панелі натисніть **"New +"** → **"Web Service"**
2. Виберіть **"Connect GitHub"** (якщо ще не підключено)
3. Виберіть ваш репозиторій `ZooCare.API`
4. Натисніть **"Connect"**

## Крок 4: Налаштування сервісу

### Основні налаштування:

- **Name:** `zoocare-api` (або будь-яка назва)
- **Region:** `Frankfurt` (або найближчий до вас)
- **Branch:** `main` (або ваша основна гілка)
- **Root Directory:** залиште порожнім (або `ZooCare.API` якщо проект в підпапці)
- **Runtime:** `Docker` або `Native` (рекомендовано Native для .NET)

### Якщо вибрали Native:

- **Build Command:**
  ```
  dotnet restore ZooCare.API/ZooCare.API.csproj && dotnet build ZooCare.API/ZooCare.API.csproj -c Release
  ```
- **Start Command:**
  ```
  dotnet ZooCare.API/bin/Release/net8.0/ZooCare.API.dll
  ```

### Якщо вибрали Docker:

Render автоматично використає `Dockerfile`, якщо він є в репозиторії.

## Крок 5: Змінні середовища (Environment Variables)

У секції **"Environment Variables"** додайте:

| Key | Value |
|-----|-------|
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `ASPNETCORE_URLS` | `http://0.0.0.0:$PORT` |
| `PORT` | `10000` (Render автоматично встановить свій) |

**ВАЖЛИВО:** Додайте змінну для підключення до бази даних:

| Key | Value |
|-----|-------|
| `ConnectionStrings__DefaultConnection` | Ваш connection string до SQL Server |

### Якщо використовуєте PostgreSQL на Render:

1. Створіть **"New PostgreSQL"** на Render
2. Скопіюйте **Internal Database URL**
3. Додайте змінну:
   ```
   ConnectionStrings__DefaultConnection = <скопійований URL>
   ```

## Крок 6: План та налаштування

- **Instance Type:** `Free` (для початку)
- **Auto-Deploy:** `Yes` (автоматичний деплой при push на GitHub)

## Крок 7: Деплой

1. Натисніть **"Create Web Service"**
2. Render почне збірку проекту
3. Переглядайте логи в реальному часі
4. Після успішного деплою ви отримаєте URL: `https://zoocare-api.onrender.com`

## Крок 8: Перевірка

1. Відкрийте URL вашого сервісу
2. Swagger UI: `https://YOUR-URL.onrender.com/swagger`
3. Перевірте, що API працює

## Налаштування бази даних

### Варіант 1: PostgreSQL на Render (рекомендовано)

1. На Render створіть **"New PostgreSQL"**
2. Скопіюйте **Internal Database URL**
3. Оновіть `appsettings.json` або додайте змінну середовища:
   ```
   ConnectionStrings__DefaultConnection = <PostgreSQL URL>
   ```
4. Оновіть `Program.cs` для підтримки PostgreSQL (якщо потрібно)

### Варіант 2: Зовнішня база даних

Додайте connection string як змінну середовища на Render.

## Важливі примітки

1. **Free план має обмеження:**
   - Сервіс "засинає" після 15 хвилин неактивності
   - Перший запит після "сну" може займати 30-60 секунд

2. **Swagger в Production:**
   - У `Program.cs` Swagger вже доступний завжди
   - Якщо хочете приховати в production, додайте умову:
   ```csharp
   if (app.Environment.IsDevelopment())
   {
       app.UseSwagger();
       app.UseSwaggerUI();
   }
   ```

3. **HTTPS:**
   - Render автоматично надає HTTPS
   - Використовуйте `https://` URL

## Troubleshooting

### Помилка: "Build failed"

- Перевірте логи збірки
- Переконайтеся, що всі залежності вказані в `.csproj`
- Перевірте, що шляхи до файлів правильні

### Помилка: "Application failed to start"

- Перевірте змінні середовища
- Перевірте connection string до БД
- Перегляньте логи запуску

### Помилка: "Port already in use"

- Render автоматично встановлює `PORT`
- Переконайтеся, що в `Program.cs` використовується `$PORT` зі змінних середовища

## Оновлення коду

Після змін у коді:
1. Закомітьте зміни на GitHub
2. Render автоматично перезапустить сервіс (якщо Auto-Deploy увімкнено)
3. Або натисніть **"Manual Deploy"** → **"Deploy latest commit"**

