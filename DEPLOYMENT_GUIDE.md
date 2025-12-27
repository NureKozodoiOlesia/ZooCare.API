# Покрокова інструкція: Завантаження на GitHub та Деплой на Railway.com

## Частина 1: Підготовка проекту та завантаження на GitHub

### Крок 1: Створення .gitignore

Створіть файл `.gitignore` в корені проекту (рядом з `ZooCare.API.sln`) з таким вмістом:

```
## .NET
bin/
obj/
*.user
*.suo
*.cache
*.dll
*.exe
*.pdb

## Visual Studio
.vs/
*.user
*.suo
*.userosscache
*.sln.docstates

## Rider
.idea/

## User-specific files
*.rsuser
*.user

## Build results
[Dd]ebug/
[Rr]elease/
x64/
x86/
[Aa]rm/
[Aa]rm64/
bld/
[Bb]in/
[Oo]bj/
[Ll]og/

## NuGet Packages
*.nupkg
*.snupkg
**/packages/*
!**/packages/build/
*.nuget.props
*.nuget.targets
project.lock.json
project.fragment.lock.json
artifacts/

## Sensitive files
appsettings.Development.json
*.secrets.json

## Database
*.db
*.mdf
*.ldf
*.ndf

## Logs
logs/
*.log
```

### Крок 2: Ініціалізація Git репозиторію

Відкрийте PowerShell або Command Prompt в корені проекту (`C:\Users\alesi\source\repos\ZooCare.API`) та виконайте:

```bash
# Перевірка, чи встановлений Git
git --version

# Ініціалізація репозиторію
git init

# Додавання всіх файлів
git add .

# Перший коміт
git commit -m "Initial commit: ZooCare API project"
```

### Крок 3: Створення репозиторію на GitHub

1. Відкрийте https://github.com
2. Увійдіть у свій акаунт
3. Натисніть кнопку **"+"** (правый верхній кут) → **"New repository"**
4. Заповніть форму:
   - **Repository name:** `ZooCare.API` (або інша назва)
   - **Description:** `ZooCare API - IoT-based zoo management system`
   - **Visibility:** Public або Private (на ваш вибір)
   - **НЕ ставлять галочки** на "Initialize with README", "Add .gitignore", "Choose a license"
5. Натисніть **"Create repository"**

### Крок 4: Підключення локального репозиторію до GitHub

Після створення репозиторію GitHub покаже інструкції. Виконайте в PowerShell:

```bash
# Додавання remote репозиторію (замініть YOUR_USERNAME на ваш GitHub username)
git remote add origin https://github.com/YOUR_USERNAME/ZooCare.API.git

# Перейменування гілки на main (якщо потрібно)
git branch -M main

# Завантаження коду на GitHub
git push -u origin main
```

Якщо виникне помилка авторизації, GitHub може запросити токен доступу. Створіть Personal Access Token:
1. GitHub → Settings → Developer settings → Personal access tokens → Tokens (classic)
2. Generate new token
3. Виберіть scope: `repo`
4. Скопіюйте токен та використайте його замість пароля

---

## Частина 2: Деплой на Railway.com

### Крок 5: Реєстрація на Railway.com

1. Відкрийте https://railway.com
2. Натисніть **"Login"** або **"Sign Up"**
3. Виберіть **"Login with GitHub"** (найпростіше)
4. Дозвольте Railway доступ до вашого GitHub акаунту

### Крок 6: Створення нового проекту на Railway

1. Після входу натисніть **"New Project"**
2. Виберіть **"Deploy from GitHub repo"**
3. Виберіть ваш репозиторій `ZooCare.API`
4. Railway автоматично визначить, що це .NET проект

### Крок 7: Налаштування змінних середовища

Railway автоматично створить сервіс. Потрібно налаштувати:

1. **Відкрийте ваш проект** на Railway
2. Натисніть на **сервіс** (ZooCare.API)
3. Перейдіть на вкладку **"Variables"**
4. Додайте змінні середовища:

```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:$PORT
```

5. Для бази даних додайте Connection String:

```
ConnectionStrings__DefaultConnection=YOUR_DATABASE_CONNECTION_STRING
```

### Крок 8: Налаштування бази даних

Railway пропонує безкоштовну PostgreSQL. Можна використати її:

1. У проекті Railway натисніть **"+ New"** → **"Database"** → **"Add PostgreSQL"**
2. Railway автоматично створить базу даних
3. Скопіюйте **Connection String** з вкладки **"Variables"**
4. Додайте змінну середовища:

```
ConnectionStrings__DefaultConnection=YOUR_POSTGRESQL_CONNECTION_STRING
```

**АБО** використайте SQL Server (якщо у вас є):

1. Створіть базу даних на Azure SQL, AWS RDS або іншому сервісі
2. Додайте Connection String у змінні середовища

### Крок 9: Оновлення appsettings.json для production

Створіть файл `appsettings.Production.json` (якщо його немає):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": ""
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

Connection String буде братися зі змінних середовища Railway.

### Крок 10: Оновлення Program.cs для Railway

Переконайтеся, що в `Program.cs` Swagger доступний не тільки в Development:

```csharp
// Замість:
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Використайте (для доступу Swagger в production):
app.UseSwagger();
app.UseSwaggerUI();
```

Або залиште умову, але додайте змінну середовища `ASPNETCORE_ENVIRONMENT=Development` на Railway (для тестування).

### Крок 11: Створення railway.json (опціонально)

Створіть файл `railway.json` в корені проекту:

```json
{
  "$schema": "https://railway.app/railway.schema.json",
  "build": {
    "builder": "NIXPACKS"
  },
  "deploy": {
    "startCommand": "dotnet ZooCare.API/ZooCare.API.dll",
    "restartPolicyType": "ON_FAILURE",
    "restartPolicyMaxRetries": 10
  }
}
```

### Крок 12: Деплой

1. Railway автоматично почне деплой після підключення репозиторію
2. Перейдіть на вкладку **"Deployments"** щоб бачити прогрес
3. Після успішного деплою Railway надасть URL (наприклад: `https://zoo-care-api-production.up.railway.app`)

### Крок 13: Застосування міграцій бази даних

Після деплою потрібно застосувати міграції:

**Варіант 1: Через Railway CLI**

1. Встановіть Railway CLI: https://docs.railway.app/develop/cli
2. Виконайте:
```bash
railway login
railway link
railway run dotnet ef database update --project ZooCare.API/ZooCare.API.csproj
```

**Варіант 2: Через Railway Dashboard**

1. Відкрийте ваш сервіс
2. Перейдіть на вкладку **"Settings"**
3. Знайдіть **"Run Command"** або **"One-Click Deploy"**
4. Додайте команду для міграцій

**Варіант 3: Автоматичні міграції при старті**

Додайте в `Program.cs` після `var app = builder.Build();`:

```csharp
// Apply migrations on startup (only in production)
if (app.Environment.IsProduction())
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ZooContext>();
        db.Database.Migrate();
    }
}
```

### Крок 14: Перевірка Swagger

Після деплою відкрийте:
```
https://YOUR-RAILWAY-URL.railway.app/swagger
```

Swagger має відкритися та показувати всі ендпоінти.

---

## Частина 3: Налаштування для Production

### Оновлення Program.cs для Swagger в Production

Якщо хочете Swagger тільки в Development, залиште як є. Якщо потрібен доступ в Production:

```csharp
// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ZooCare API v1");
    c.RoutePrefix = "swagger"; // Swagger available at /swagger
});

// CORS
app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
```

### Налаштування CORS для Production

Оновіть CORS в `Program.cs` для дозволу конкретних доменів:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
    
    // Для production можна обмежити:
    options.AddPolicy("Production", policy =>
    {
        policy.WithOrigins("https://your-frontend-domain.com")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

---

## Частина 4: Перевірка та тестування

### Перевірка деплою

1. **Перевірте логі Railway:**
   - Відкрийте сервіс → вкладка **"Deployments"** → **"View Logs"**
   - Має бути: `Now listening on: http://[::]:PORT`

2. **Перевірте Swagger:**
   - Відкрийте `https://YOUR-URL.railway.app/swagger`
   - Має відкритися Swagger UI

3. **Тестування API:**
   - Спробуйте виконати GET запит до `/api/enclosures`
   - Перевірте, що API відповідає

### Можливі проблеми

**Проблема: "Application failed to start"**
- Перевірте логи Railway
- Переконайтеся, що Connection String правильний
- Перевірте, що всі змінні середовища встановлені

**Проблема: "Database connection failed"**
- Перевірте Connection String
- Переконайтеся, що база даних доступна з Railway
- Застосуйте міграції

**Проблема: "Swagger not found"**
- Переконайтеся, що Swagger включений в Production
- Перевірте URL: `/swagger` або `/swagger/index.html`

---

## Готово! 🎉

Після виконання всіх кроків:
- ✅ Проект завантажено на GitHub
- ✅ Проект задеплоєно на Railway.com
- ✅ Swagger доступний з публічного URL
- ✅ API працює в production

### Ваші наступні кроки:

1. Завантажте код на GitHub (Кроки 1-4)
2. Зареєструйтеся на Railway (Крок 5)
3. Підключіть репозиторій (Крок 6)
4. Налаштуйте змінні середовища (Крок 7-8)
5. Зачекайте деплой (Крок 12)
6. Перевірте Swagger (Крок 14)

