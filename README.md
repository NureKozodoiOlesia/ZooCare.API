# ZooCare - Система управління зоопарком

Проект складається з двох частин:
- **ZooCare.API** - серверна частина (ASP.NET Core Web API)
- **ZooCare.IoTClient** - IoT клієнт для ESP32 (PlatformIO/Arduino)

## Структура проекту

```
ZooCare.API/                    (корінь репозиторію)
├── ZooCare.API/                (.NET проект - відкривається в Visual Studio)
│   ├── Controllers/            (API контролери)
│   ├── Services/               (Бізнес-логіка)
│   ├── Entities/              (Моделі даних)
│   └── ...
├── ZooCare.IoTClient/          (PlatformIO проект - відкривається в VS Code)
│   ├── src/                   (Вихідний код ESP32)
│   ├── platformio.ini         (Конфігурація PlatformIO)
│   └── ...
└── ZooCare.API.sln            (Solution файл для Visual Studio)
```

## Робота з проектами

### ZooCare.API (Серверна частина)

1. Відкрийте `ZooCare.API.sln` в Visual Studio
2. Налаштуйте connection string в `appsettings.json`
3. Запустіть проект (F5)

### ZooCare.IoTClient (IoT пристрій)

1. Відкрийте папку `ZooCare.IoTClient` в VS Code
2. Встановіть розширення PlatformIO
3. Відкрийте `src/main.cpp` та налаштуйте `apiBaseUrl`
4. Завантажте код на ESP32 або запустіть в Wokwi емуляторі

**Примітка:** Visual Studio не підтримує PlatformIO проекти. Використовуйте VS Code для роботи з IoT клієнтом.

## Детальні інструкції

- API документація: `ZooCare.API/README.md` (якщо є)
- IoT налаштування: `ZooCare.IoTClient/SETUP_INSTRUCTIONS.md`

