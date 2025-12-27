# 🔄 Як перезапустити деплой на Render

## Крок 1: Закомітьте Dockerfile на GitHub

1. У Visual Studio:
   - View → Git Changes
   - Введіть commit message: "Add Dockerfile for Render"
   - Натисніть "Commit All"
   - Натисніть "Sync" → "Push"

**Або через термінал:**
```bash
git add Dockerfile
git commit -m "Add Dockerfile for Render"
git push
```

## Крок 2: На Render

### Варіант A: Якщо використовуєте Docker

1. Відкрийте ваш Web Service на Render
2. Перейдіть до **Settings**
3. Перевірте:
   - **Runtime:** має бути `Docker`
   - **Dockerfile Path:** залиште порожнім (Render знайде Dockerfile автоматично)
4. Натисніть **"Save Changes"**

### Варіант B: Якщо використовуєте Native (рекомендовано)

1. Відкрийте ваш Web Service на Render
2. Перейдіть до **Settings**
3. Перевірте:
   - **Runtime:** має бути `Native`
   - **Build Command:** 
     ```
     dotnet restore ZooCare.API/ZooCare.API.csproj && dotnet build ZooCare.API/ZooCare.API.csproj -c Release
     ```
   - **Start Command:**
     ```
     dotnet ZooCare.API/bin/Release/net8.0/ZooCare.API.dll
     ```
4. Натисніть **"Save Changes"**

## Крок 3: Перезапустіть деплой

1. На головній сторінці вашого Web Service
2. Натисніть **"Manual Deploy"** → **"Deploy latest commit"**
3. Або просто натисніть **"Redeploy"**

## Крок 4: Перевірка логів

1. Перейдіть до вкладки **"Logs"**
2. Переглядайте процес збірки та запуску
3. Має бути:
   ```
   ✓ Міграції БД успішно застосовано
   ✓ Ролі успішно ініціалізовано
   Now listening on: http://0.0.0.0:10000
   ```

## Якщо помилки

### "Dockerfile not found"
- Перевірте, що Dockerfile завантажений на GitHub
- Перевірте, що він в корені проекту (рядком з `.sln`)

### "Build failed"
- Перевірте логи - там буде детальна інформація
- Переконайтеся, що всі пакети вказані в `.csproj`

### "Cannot connect to database"
- Перевірте змінну `ConnectionStrings__DefaultConnection`
- Перевірте, що база даних створена та працює

---

## Рекомендація

**Використовуйте Native замість Docker** - це простіше для .NET проектів:
- Швидша збірка
- Менше налаштувань
- Простіше дебажити

Dockerfile створено на випадок, якщо потрібен Docker, але для .NET краще Native.

