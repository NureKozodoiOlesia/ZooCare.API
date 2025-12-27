-- SQL скрипт для створення IoT пристрою в базі даних
-- Виконайте цей скрипт ПЕРЕД запуском IoT емулятора

-- Крок 1: Перевірте, чи є вольєри в базі даних
SELECT * FROM Enclosures;

-- Якщо вольєрів немає, створіть їх:
-- INSERT INTO Enclosures (Name, Type, Location) 
-- VALUES ('Вольєр 1', 'Великий', 'Сектор A');

-- Крок 2: Створіть IoT пристрій
-- ЗАМІНІТЬ '1' на ID реального вольєра з вашої бази даних!
INSERT INTO IoTDevices (EnclosureId, SerialNumber, DeviceType, Status, LastHeartbeat)
VALUES (1, 'WATER-SENSOR-001', 'WaterLevel', 'Offline', GETUTCDATE());

-- Крок 3: Перевірте, що пристрій створено
SELECT * FROM IoTDevices WHERE SerialNumber = 'WATER-SENSOR-001';

-- Якщо потрібно видалити пристрій (для повторного створення):
-- DELETE FROM IoTDevices WHERE SerialNumber = 'WATER-SENSOR-001';

