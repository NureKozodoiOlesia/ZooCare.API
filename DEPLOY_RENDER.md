# 🚀 Покрокова інструкція: Деплой на Render.com

## ⚠️ ВАЖЛИВО: Порядок дій

**1. Створити базу даних → 2. Завантажити код на GitHub → 3. Задеплоїти на Render**

---

## КРОК 1: Створення бази даних на Render

### 1.1. Відкрийте Render.com

1. Перейдіть на https://render.com
2. Увійдіть або зареєструйтеся (можна через GitHub)

### 1.2. Створіть PostgreSQL базу даних

1. На головній панелі натисніть **"New +"** → **"PostgreSQL"**
2. Заповніть форму:
   - **Name:** `zoocare-db` (або будь-яка назва)
   - **Database:** `zoocare` (або будь-яка назва)
   - **User:** `zoocare_user` (або будь-яка назва)
   - **Region:** виберіть найближчий (наприклад, `Frankfurt`)
   - **PostgreSQL Version:** залиште за замовчуванням
   - **Plan:** `Free` (для початку)
3. Натисніть **"Create Database"**
4. ⏳ Зачекайте 2-3 хвилини, поки база створиться

### 1.3. Скопіюйте connection string

1. Після створення відкрийте вашу базу даних
2. Перейдіть до вкладки **"Connections"**
3. Знайдіть **"Internal Database URL"** (виглядає як: `postgresql://user:password@host:port/database`)
4. **СКОПІЮЙТЕ ЦЕЙ URL** - він знадобиться пізніше!

**Приклад:**
```
postgresql://zoocare_user:abc123xyz@dpg-xxxxx-a.frankfurt-postgres.render.com/zoocare
```

---

## КРОК 2: Підготовка проекту для PostgreSQL

### 2.1. Додайте пакет PostgreSQL

1. Відкрийте `ZooCare.API/ZooCare.API.csproj`
2. Додайте пакет (після рядка з `Microsoft.EntityFrameworkCore.SqlServer`):

```xml
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.0" />
```

**Повний файл має виглядати так:**
```xml
<ItemGroup>
  <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.10" />
  <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.10">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
  </PackageReference>
  <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.0" />
  <PackageReference Include="Swashbuckle.AspNetCore" Version="6.4.0" />
</ItemGroup>
```

### 2.2. Оновіть Program.cs для PostgreSQL

1. Відкрийте `ZooCare.API/Program.cs`
2. Знайдіть рядок:
   ```csharp
   options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
   ```
3. Замініть на:
   ```csharp
   options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
   ```

**Додайте using на початку файлу:**
```csharp
using Npgsql.EntityFrameworkCore.PostgreSQL;
```

---

## КРОК 3: Завантаження проекту на GitHub

### 3.1. Перевірте .gitignore

Переконайтеся, що в корені проекту є файл `.gitignore` (я вже створив його)

### 3.2. Закомітьте зміни

У Visual Studio або через термінал:

```bash
git add .
git commit -m "Prepare for Render deployment with PostgreSQL"
git push
```

**Або через Visual Studio:**
1. View → Git Changes
2. Введіть commit message: "Prepare for Render deployment"
3. Натисніть "Commit All"
4. Натисніть "Sync" → "Push"

---

## КРОК 4: Створення Web Service на Render

### 4.1. Створіть новий Web Service

1. На Render натисніть **"New +"** → **"Web Service"**
2. Якщо ще не підключено GitHub:
   - Натисніть **"Connect GitHub"**
   - Дозвольте доступ до вашого репозиторію
   - Виберіть репозиторій `ZooCare.API`
3. Натисніть **"Connect"**

### 4.2. Налаштуйте сервіс

Заповніть форму:

- **Name:** `zoocare-api` (або будь-яка назва)
- **Region:** той самий, що й база даних (наприклад, `Frankfurt`)
- **Branch:** `main` (або ваша основна гілка)
- **Root Directory:** залиште порожнім
- **Runtime:** `Docker` або `Native` (рекомендую `Native` для .NET)

### 4.3. Налаштуйте Build & Start команди

**Якщо вибрали Native:**

- **Build Command:**
  ```
  dotnet restore ZooCare.API/ZooCare.API.csproj && dotnet build ZooCare.API/ZooCare.API.csproj -c Release
  ```

- **Start Command:**
  ```
  dotnet ZooCare.API/bin/Release/net8.0/ZooCare.API.dll
  ```

**Якщо вибрали Docker:**
- Залиште порожнім (Render автоматично знайде Dockerfile, якщо він є)

### 4.4. Додайте змінні середовища

У секції **"Environment Variables"** додайте:

| Key | Value |
|-----|-------|
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `ASPNETCORE_URLS` | `http://0.0.0.0:$PORT` |
| `ConnectionStrings__DefaultConnection` | **ВСТАВТЕ СКОПІЙОВАНИЙ URL З КРОКУ 1.3** |

**ВАЖЛИВО:** 
- Назва змінної: `ConnectionStrings__DefaultConnection` (з **подвійним підкресленням**)
- Значення: ваш Internal Database URL з PostgreSQL

### 4.5. Налаштуйте план

- **Instance Type:** `Free` (для початку)
- **Auto-Deploy:** `Yes` (автоматичний деплой при push на GitHub)

### 4.6. Створіть сервіс

Натисніть **"Create Web Service"**

---

## КРОК 5: Очікування деплою

1. Render почне збірку проекту (займе 3-5 хвилин)
2. Переглядайте логи в реальному часі
3. Після успішного деплою ви отримаєте URL: `https://zoocare-api.onrender.com`

### Що має бути в логах:

```
✓ Міграції БД успішно застосовано
✓ Ролі успішно ініціалізовано
Now listening on: http://0.0.0.0:10000
```

---

## КРОК 6: Перевірка

### 6.1. Перевірте Swagger

Відкрийте: `https://YOUR-URL.onrender.com/swagger`

Має відкритися Swagger UI з вашим API.

### 6.2. Перевірте API

Спробуйте:
1. `POST /api/auth/register` - створити користувача
2. `POST /api/auth/login` - увійти
3. `GET /api/admin/stats` - отримати статистику (потрібна авторизація)

---

## ❌ Якщо щось пішло не так

### Помилка: "Cannot open database"

- Перевірте connection string
- Перевірте, що використовуєте **Internal Database URL** (не External)
- Перевірте, що змінна називається `ConnectionStrings__DefaultConnection` (з подвійним підкресленням)

### Помилка: "Migration failed"

- Перевірте логи на Render
- Можливо, база даних ще не створена - перевірте статус PostgreSQL

### Помилка: "Build failed"

- Перевірте, що додали пакет `Npgsql.EntityFrameworkCore.PostgreSQL`
- Перевірте, що оновили `Program.cs` для використання `UseNpgsql`

### Помилка: "Port already in use"

- Це нормально, Render автоматично встановлює PORT
- Переконайтеся, що в `Program.cs` є код для використання PORT зі змінних середовища

---

## ✅ Чек-лист перед деплоєм

- [ ] Створена PostgreSQL база даних на Render
- [ ] Скопійований Internal Database URL
- [ ] Додано пакет `Npgsql.EntityFrameworkCore.PostgreSQL` до `.csproj`
- [ ] Оновлено `Program.cs`: `UseSqlServer` → `UseNpgsql`
- [ ] Додано автоматичні міграції в `Program.cs` (вже додано)
- [ ] Додано налаштування порту для Render (вже додано)
- [ ] Створено `.gitignore` (вже створено)
- [ ] Закомічено та завантажено код на GitHub
- [ ] Створено Web Service на Render
- [ ] Додано змінні середовища на Render
- [ ] Деплой успішний, Swagger відкривається

---

## 🎉 Готово!

Після успішного деплою ваш API буде доступний за адресою:
- **API:** `https://YOUR-URL.onrender.com`
- **Swagger:** `https://YOUR-URL.onrender.com/swagger`

**Примітка:** На безкоштовному плані сервіс "засинає" після 15 хвилин неактивності. Перший запит після "сну" може займати 30-60 секунд.

