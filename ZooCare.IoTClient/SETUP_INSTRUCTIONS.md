# Інструкція з налаштування IoT пристрою для ZooCare API

## Крок 1: Запуск серверної частини

1. Відкрийте проект `ZooCare.API` в Visual Studio
2. Запустіть проект (F5 або `dotnet run`)
3. Переконайтеся, що сервер працює на порту `5049` (або іншому, якщо вказано в `appsettings.json`)
4. Відкрийте Swagger UI: `https://localhost:5049/swagger` (або `http://localhost:5049/swagger`)

## Крок 2: Створення IoT пристрою в базі даних

**ВАЖЛИВО:** Перед підключенням емулятора, потрібно створити IoT пристрій в базі даних!

### Варіант 1: Через Swagger UI

1. Відкрийте Swagger UI
2. Знайдіть ендпоінт `POST /api/admin/enclosures/{enclosureId}/devices` (якщо є)
3. Або використайте SQL запит (див. Варіант 2)

### Варіант 2: Через SQL запит

Виконайте наступний SQL запит в базі даних:

```sql
-- Спочатку переконайтеся, що є вольєр (якщо немає, створіть його)
-- Приклад: вольєр з ID = 1
-- Якщо вольєрів немає, створіть його через API або SQL:

-- INSERT INTO Enclosures (Name, Type, Location) VALUES ('Вольєр 1', 'Великий', 'Сектор A');

-- Тепер створіть IoT пристрій
INSERT INTO IoTDevices (EnclosureId, SerialNumber, DeviceType, Status, LastHeartbeat)
VALUES (1, 'WATER-SENSOR-001', 'WaterLevel', 'Offline', GETUTCDATE());
```

**Примітка:** Замініть `1` на ID реального вольєра з вашої бази даних.

## Крок 3: Налаштування ngrok (для Wokwi)

Wokwi емулятор не може підключатися до `localhost`, тому потрібно використовувати ngrok.

1. Завантажте та встановіть ngrok: https://ngrok.com/download
2. Запустіть ngrok в терміналі:
   ```bash
   ngrok http 5049
   ```
   (або інший порт, якщо ваш сервер працює на іншому порту)

3. Скопіюйте HTTPS URL з ngrok (наприклад: `https://abc123.ngrok-free.dev`)
4. Відкрийте файл `src/main.cpp` в IoT проекті
5. Замініть значення `apiBaseUrl`:
   ```cpp
   const char* apiBaseUrl = "https://abc123.ngrok-free.dev"; // Ваш ngrok URL
   ```

## Крок 4: Налаштування IoT пристрою

1. Відкрийте проект `ZooCare.IoTClient` в VS Code з PlatformIO
2. Переконайтеся, що в `platformio.ini` є бібліотека `ArduinoJson`:
   ```ini
   lib_deps =
       bblanchon/ArduinoJson @ ^6.21.3
   ```
3. Переконайтеся, що `serialNumber` в `main.cpp` відповідає `SerialNumber` в базі даних:
   ```cpp
   const char* serialNumber = "WATER-SENSOR-001";
   ```

## Крок 5: Запуск та тестування

1. Запустіть сервер (якщо ще не запущений)
2. Запустіть ngrok (якщо використовуєте Wokwi)
3. Відкрийте Wokwi емулятор або завантажте код на ESP32
4. Спостерігайте за Serial Monitor:
   - Підключення до WiFi
   - Відправка телеметрії кожні 30 секунд
   - Відправка heartbeat кожну хвилину

## Перевірка роботи

### Перевірка через Swagger UI:

1. Відкрийте `GET /api/alerts` - має з'явитися сповіщення, якщо рівень води < 15%
2. Відкрийте `GET /api/iot/devices` (якщо є такий ендпоінт) - має показувати статус пристрою "Online"

### Перевірка через базу даних:

```sql
-- Перевірка статусу пристрою
SELECT * FROM IoTDevices WHERE SerialNumber = 'WATER-SENSOR-001';

-- Перевірка зчитаних даних
SELECT TOP 10 * FROM SensorReadings 
WHERE DeviceId = (SELECT Id FROM IoTDevices WHERE SerialNumber = 'WATER-SENSOR-001')
ORDER BY RecordedAt DESC;

-- Перевірка сповіщень
SELECT * FROM SystemAlerts 
WHERE Message LIKE '%Water level%' OR Message LIKE '%рівень води%'
ORDER BY CreatedAt DESC;
```

## Усунення проблем

### Помилка 404 Not Found
- Переконайтеся, що URL правильний (з `/api/iot/telemetry`)
- Переконайтеся, що сервер працює

### Помилка 500 Internal Server Error
- Переконайтеся, що IoT пристрій створений в базі даних
- Переконайтеся, що `SerialNumber` відповідає

### Помилка підключення WiFi
- В Wokwi використовуйте `Wokwi-GUEST` (без пароля)
- Переконайтеся, що WiFi підключення активне

### Дані не передаються
- Перевірте ngrok URL (має бути HTTPS)
- Перевірте, що сервер працює
- Перевірте Serial Monitor для деталей помилок

## Налаштування HC-SR04 датчика

Датчик підключений до:
- **VCC** → ESP32 3V3
- **GND** → ESP32 GND
- **TRIG** → ESP32 D4 (GPIO 4)
- **ECHO** → ESP32 D2 (GPIO 2)

Код автоматично читає дані з датчика та конвертує відстань в рівень води (0-100%).

## Калібрування датчика

Якщо потрібно налаштувати калібрування, змініть значення в `main.cpp`:

```cpp
const float maxDistance = 100.0; // Максимальна відстань (см)
const float minDistance = 10.0;  // Мінімальна відстань (см)
```

