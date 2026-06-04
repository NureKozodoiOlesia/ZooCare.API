#include <WiFi.h>
#include <HTTPClient.h>
#include <WiFiClientSecure.h>
#include <ArduinoJson.h>

const char* ssid = "Wokwi-GUEST";
const char* password = "";


// ngrok TCP-тунель: сирий TCP, без TLS і без http→https редіректу.
// Адреса = http://<host>:<port> з рядка Forwarding `tcp://6.tcp.eu.ngrok.io:16813`.
// УВАГА: при кожному перезапуску `ngrok tcp 5049` хост і порт змінюються — онови тут.
const char* apiBaseUrl = "http://7.tcp.eu.ngrok.io:18667";

const char* serialNumber = "WATER-SENSOR-001";
const int readingInterval = 30000; 
const int heartbeatInterval = 60000; 

const int trigPin = 4;  
const int echoPin = 2;  

const float maxDistance = 100.0; 
const float minDistance = 10.0;  

int successfulTelemetry = 0;
int failedTelemetry = 0;
int successfulHeartbeat = 0;
int failedHeartbeat = 0;

void sendTelemetry(float level);
void sendHeartbeat();
float readWaterLevel();
float measureDistance();
void printStatistics();

unsigned long lastReading = 0;
unsigned long lastHeartbeat = 0;

void setup() {
  Serial.begin(115200);
  delay(1000);
  
  Serial.println("IoT Water Level Sensor - ESP32");
  Serial.println("--------------------------------");
  
  pinMode(trigPin, OUTPUT);
  pinMode(echoPin, INPUT);
  
  WiFi.begin(ssid, password);
  Serial.print("Підключення до WiFi");
  
  int attempts = 0;
  while (WiFi.status() != WL_CONNECTED && attempts < 30) {
    delay(500);
    Serial.print(".");
    attempts++;
  }
  
  if (WiFi.status() == WL_CONNECTED) {
    Serial.println();
    Serial.println("WiFi підключено");
    Serial.print("IP адреса: ");
    Serial.println(WiFi.localIP());
    Serial.print("Сила сигналу: ");
    Serial.print(WiFi.RSSI());
    Serial.println(" dBm");
  } else {
    Serial.println();
    Serial.println("Помилка підключення до WiFi");
    Serial.println("Перевірте налаштування WiFi");
  }
  
  Serial.println("--------------------------------");
  Serial.print("Serial Number: ");
  Serial.println(serialNumber);
  Serial.print("API Base URL: ");
  Serial.println(apiBaseUrl);
  Serial.println("--------------------------------");
  Serial.println("Система готова до роботи");
  Serial.println("Телеметрія: кожні 30 секунд");
  Serial.println("Heartbeat: кожну хвилину");
  Serial.println("--------------------------------");
  Serial.println();
}

void loop() {
  unsigned long currentMillis = millis();
  
  if (WiFi.status() != WL_CONNECTED) {
    Serial.println("[WARNING] WiFi з'єднання втрачено. Переподключення...");
    WiFi.begin(ssid, password);
    delay(2000);
    return;
  }
  
  if (currentMillis - lastReading >= readingInterval) {
    lastReading = currentMillis;
    
    float waterLevel = readWaterLevel();
    
    sendTelemetry(waterLevel);
  }
  
  if (currentMillis - lastHeartbeat >= heartbeatInterval) {
    lastHeartbeat = currentMillis;
    sendHeartbeat();
  }
  
  static unsigned long lastStats = 0;
  if (currentMillis - lastStats >= 300000) { 
    lastStats = currentMillis;
    printStatistics();
  }
  
  delay(100);
}

float readWaterLevel() {
  float distance = measureDistance();
  
  float waterLevel = 100.0 - ((distance - minDistance) / (maxDistance - minDistance) * 100.0);
  
  if (waterLevel < 0) waterLevel = 0;
  if (waterLevel > 100) waterLevel = 100;
  
  return waterLevel;
}

float measureDistance() {
  digitalWrite(trigPin, LOW);
  delayMicroseconds(2);
  
  digitalWrite(trigPin, HIGH);
  delayMicroseconds(10);
  digitalWrite(trigPin, LOW);
  
  long duration = pulseIn(echoPin, HIGH);
  
  float distance = duration * 0.0343 / 2;
  
  return distance;
}

void sendTelemetry(float level) {
  if (WiFi.status() != WL_CONNECTED) {
    Serial.println("[ERROR] WiFi не підключено");
    failedTelemetry++;
    return;
  }
  
  HTTPClient http;

  String url = String(apiBaseUrl) + "/api/iot/telemetry";

  http.begin(url);
  http.addHeader("Content-Type", "application/json");
  http.addHeader("ngrok-skip-browser-warning", "true");

  StaticJsonDocument<200> doc;
  doc["serialNumber"] = serialNumber;
  doc["sensorType"] = "WaterLevel";
  doc["value"] = level;
  
  String jsonString;
  serializeJson(doc, jsonString);
  
  Serial.print("[");
  Serial.print(millis() / 1000);
  Serial.print("s] Телеметрія - Рівень води: ");
  Serial.print(level, 2);
  Serial.print("%");
  
  if (level < 15) {
    Serial.print(" [КРИТИЧНО]");
  } else if (level < 30) {
    Serial.print(" [НИЗЬКИЙ]");
  } else if (level > 80) {
    Serial.print(" [ВИСОКИЙ]");
  } else {
    Serial.print(" [НОРМА]");
  }
  
  int httpResponseCode = http.POST(jsonString);
  
  if (httpResponseCode > 0) {
    Serial.print(" - HTTP ");
    Serial.print(httpResponseCode);
    
    if (httpResponseCode == 200) {
      Serial.println(" Успішно відправлено");
      successfulTelemetry++;
      
      String response = http.getString();
      Serial.print("[Сервер] ");
      Serial.println(response);
      
      if (level < 15) {
        Serial.println("[Сервер] [КРИТИЧНЕ СПОВІЩЕННЯ] Низький рівень води");
      }
    } else {
      Serial.println(" Помилка відповіді");
      failedTelemetry++;
      
      String response = http.getString();
      Serial.print("[Сервер] Помилка: ");
      Serial.println(response);
    }
  } else {
    Serial.print(" - Помилка: ");
    Serial.println(http.errorToString(httpResponseCode));
    failedTelemetry++;
  }
  
  http.end();
  Serial.println();
}

void sendHeartbeat() {
  if (WiFi.status() != WL_CONNECTED) {
    failedHeartbeat++;
    return;
  }
  
  HTTPClient http;

  String url = String(apiBaseUrl) + "/api/iot/heartbeat?serialNumber=" + String(serialNumber);

  http.begin(url);
  http.addHeader("ngrok-skip-browser-warning", "true");

  int httpResponseCode = http.POST("");
  
  if (httpResponseCode == 200) {
    Serial.print("[");
    Serial.print(millis() / 1000);
    Serial.println("s] Heartbeat - HTTP 200 OK");
    successfulHeartbeat++;
    
    String response = http.getString();
    Serial.print("[Сервер] ");
    Serial.println(response);
  } else {
    Serial.print("[");
    Serial.print(millis() / 1000);
    Serial.print("s] Heartbeat - HTTP ");
    Serial.print(httpResponseCode);
    Serial.println(" Помилка");
    failedHeartbeat++;
    
    String response = http.getString();
    Serial.print("[Сервер] Помилка: ");
    Serial.println(response);
  }
  
  http.end();
  Serial.println();
}

void printStatistics() {
  Serial.println();
  Serial.println("--------------------------------");
  Serial.println("СТАТИСТИКА РОБОТИ");
  Serial.println("--------------------------------");
  Serial.print("Час роботи: ");
  Serial.print(millis() / 1000);
  Serial.println(" секунд");
  Serial.println();
  Serial.println("Телеметрія:");
  Serial.print("  Успішно: ");
  Serial.print(successfulTelemetry);
  Serial.print(" | Помилок: ");
  Serial.println(failedTelemetry);
  Serial.print("  Успішність: ");
  if (successfulTelemetry + failedTelemetry > 0) {
    float successRate = (float)successfulTelemetry / (successfulTelemetry + failedTelemetry) * 100;
    Serial.print(successRate, 1);
  } else {
    Serial.print("0");
  }
  Serial.println("%");
  Serial.println();
  Serial.println("Heartbeat:");
  Serial.print("  Успішно: ");
  Serial.print(successfulHeartbeat);
  Serial.print(" | Помилок: ");
  Serial.println(failedHeartbeat);
  Serial.print("  Успішність: ");
  if (successfulHeartbeat + failedHeartbeat > 0) {
    float successRate = (float)successfulHeartbeat / (successfulHeartbeat + failedHeartbeat) * 100;
    Serial.print(successRate, 1);
  } else {
    Serial.print("0");
  }
  Serial.println("%");
  Serial.println();
  Serial.print("Поточний рівень води: ");
  Serial.print(readWaterLevel(), 2);
  Serial.println("%");
  Serial.println("--------------------------------");
  Serial.println();
}

