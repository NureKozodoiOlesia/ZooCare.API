# Як завантажити проект на GitHub через Visual Studio

## Крок 1: Створення репозиторію на GitHub

1. Відкрийте https://github.com
2. Увійдіть у свій акаунт
3. Натисніть кнопку **"+"** (правий верхній кут) → **"New repository"**
4. Заповніть форму:
   - **Repository name:** `ZooCare.API`
   - **Description:** `ZooCare API - IoT-based zoo management system`
   - **Visibility:** Public або Private (на ваш вибір)
   - **НЕ ставлять галочки** на:
     - ❌ Initialize with README
     - ❌ Add .gitignore
     - ❌ Choose a license
5. Натисніть **"Create repository"**
6. **НЕ додавайте файли через веб-інтерфейс!** Просто закрийте сторінку

## Крок 2: Підготовка проекту в Visual Studio

1. Відкрийте Visual Studio
2. Відкрийте ваш проект: **File → Open → Project/Solution**
3. Виберіть `ZooCare.API.sln`

## Крок 3: Ініціалізація Git через Visual Studio

1. У Visual Studio перейдіть до **View → Git Changes** (або натисніть `Ctrl+0, G`)
2. У панелі **Git Changes** натисніть кнопку **"Create Git Repository"**
3. У діалоговому вікні:
   - **Repository location:** залиште поточну папку проекту
   - **Add a .gitignore file:** виберіть **".NET"** (або залиште як є, якщо вже є)
   - **Add a README file:** можете поставити галочку (опціонально)
4. Натисніть **"Create"**

## Крок 4: Перший коміт

1. У панелі **Git Changes** ви побачите всі файли
2. Перевірте, що `.gitignore` працює (не має бути файлів з `bin/`, `obj/`)
3. У полі **"Enter a commit message"** введіть: `Initial commit: ZooCare API project`
4. Натисніть кнопку **"Commit All"** (або `Ctrl+K, Ctrl+K`)
5. Після коміту натисніть **"Commit Staged"** (якщо з'явиться)

## Крок 5: Підключення до GitHub

1. У панелі **Git Changes** натисніть кнопку **"Sync"** (або стрілку вгору)
2. У діалоговому вікні виберіть **"Publish to GitHub"**
3. Введіть дані:
   - **Repository name:** `ZooCare.API` (або назва, яку ви створили)
   - **Description:** `ZooCare API - IoT-based zoo management system`
   - **Visibility:** Public або Private
4. Натисніть **"Publish"**
5. Visual Studio може запросити авторизацію GitHub - увійдіть у свій акаунт

## Крок 6: Перевірка

1. Після успішного публікування відкрийте ваш репозиторій на GitHub
2. Перевірте, що всі файли завантажені
3. Переконайтеся, що `bin/`, `obj/`, `.vs/` не завантажені (вони в `.gitignore`)

## Альтернативний спосіб: Через командний рядок у Visual Studio

Якщо кнопка "Publish to GitHub" не працює:

1. У Visual Studio відкрийте **View → Terminal** (або `Ctrl+` `)
2. Виконайте команди:

```bash
# Перевірка статусу
git status

# Додавання remote (замініть YOUR_USERNAME)
git remote add origin https://github.com/YOUR_USERNAME/ZooCare.API.git

# Перейменування гілки (якщо потрібно)
git branch -M main

# Завантаження на GitHub
git push -u origin main
```

## Якщо виникли проблеми з авторизацією

GitHub може запросити Personal Access Token:

1. GitHub → **Settings** → **Developer settings** → **Personal access tokens** → **Tokens (classic)**
2. Натисніть **"Generate new token (classic)"**
3. Введіть назву токену (наприклад: "Visual Studio")
4. Виберіть scope: **`repo`** (повний доступ до репозиторіїв)
5. Натисніть **"Generate token"**
6. **ВАЖЛИВО:** Скопіюйте токен одразу (він більше не показується)
7. Використайте токен замість пароля при авторизації в Visual Studio

## Після завантаження

1. Перевірте репозиторій на GitHub
2. Всі файли мають бути завантажені
3. Готово до деплою на Railway.com!

## Наступні кроки

Після завантаження на GitHub:
1. Перейдіть до інструкції з деплою на Railway.com (файл `DEPLOYMENT_GUIDE.md`)
2. Підключіть репозиторій до Railway
3. Налаштуйте змінні середовища
4. Задеплойте проект

