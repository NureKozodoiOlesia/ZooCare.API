# ZooCare IoT Client - ESP32 Water Level Sensor

IoT клієнт для системи управління зоопарком, який відстежує рівень води у вольєрах за допомогою ультразвукового датчика HC-SR04.

## Компоненти

- **ESP32 DevKit V1** - мікроконтролер
- **HC-SR04** - ультразвуковий датчик відстані (для вимірювання рівня води)

## Підключення датчика

```
HC-SR04    →    ESP32
VCC        →    3V3
GND        →    GND
TRIG       →    D4 (GPIO 4)
ECHO       →    D2 (GPIO 2)
```

## Функціональність

- ✅ Підключення до WiFi (Wokwi-GUEST для емуляції)
- ✅ Читання даних з HC-SR04 датчика кожні 30 секунд
- ✅ Відправка телеметрії на сервер (`POST /api/iot/telemetry`)
- ✅ Відправка heartbeat кожну хвилину (`POST /api/iot/heartbeat`)
- ✅ Автоматичне переподключення при втраті WiFi
- ✅ Статистика успішних/неуспішних запитів

## Налаштування

### 1. Налаштування API URL

Відкрийте `src/main.cpp` та замініть `apiBaseUrl` на ваш ngrok URL:

```cpp
const char* apiBaseUrl = "https://your-ngrok-url.ngrok-free.dev";
```

### 2. Налаштування Serial Number

Переконайтеся, що `serialNumber` відповідає пристрою в базі даних:

```cpp
const char* serialNumber = "WATER-SENSOR-001";
```

### 3. Створення пристрою в базі даних

**ВАЖЛИВО:** Перед запуском емулятора виконайте SQL скрипт `CREATE_DEVICE.sql` для створення IoT пристрою в базі даних.

## Запуск

1. Відкрийте проект в VS Code з PlatformIO
2. Запустіть сервер ZooCare API
3. Запустіть ngrok: `ngrok http 5049`
4. Оновіть `apiBaseUrl` в `main.cpp` на ngrok URL
5. Завантажте код на ESP32 або запустіть в Wokwi емуляторі
6. Спостерігайте за Serial Monitor

## Детальні інструкції

Див. `SETUP_INSTRUCTIONS.md` для повної інструкції з налаштування.

## Формат даних

### Телеметрія (POST /api/iot/telemetry)

```json
{
  "serialNumber": "WATER-SENSOR-001",
  "sensorType": "WaterLevel",
  "value": 75.5
}
```

### Heartbeat (POST /api/iot/heartbeat?serialNumber=WATER-SENSOR-001)

Без body, тільки query параметр `serialNumber`.

## Калібрування датчика

Якщо потрібно налаштувати калібрування, змініть значення в `main.cpp`:

```cpp
const float maxDistance = 100.0; // Максимальна відстань (см)
const float minDistance = 10.0;  // Мінімальна відстань (см)
```

## Усунення проблем

- **404 Not Found**: Перевірте URL та переконайтеся, що сервер працює
- **500 Internal Server Error**: Перевірте, що IoT пристрій створений в базі даних
- **WiFi не підключається**: В Wokwi використовуйте `Wokwi-GUEST` (без пароля)

