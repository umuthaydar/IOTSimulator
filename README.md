# IoT Simulator

.NET, Entity Framework, MQTT ve Angular ile geliştirilmiş kapsamlı bir IoT (Nesnelerin İnterneti) simülatör uygulaması. Bu çözüm, IoT sensör verilerini simüle eder, MQTT protokolü üzerinden yayınlar ve evler ve odalar arasındaki IoT cihazlarını yönetmek ve gerçek zamanlı olarak izlemek için web tabanlı bir dashboard sağlar.

## 📝 Açıklama

IoT Simulator, eksiksiz bir IoT ekosistemini gösteren full-stack bir uygulamadır. Üç ana bileşenden oluşur:
	• Publisher (Yayıncı) - Simüle edilmiş sensör verilerini (sıcaklık & nem) MQTT üzerinden yayınlar
	• Subscriber/API (Abone/API) - MQTT mesajlarını alır, verileri PostgreSQL’e kaydeder ve REST API uç noktaları sağlar
	• Web UI - Angular tabanlı dashboard ile gerçek zamanlı izleme ve cihaz yönetimi

Sistem, Onion Architecture prensiplerini takip eder ve temiz bir sorumluluk ayrımı sunar; bu da bakım ve ölçeklenebilirliği kolaylaştırır.

## ✨ Özellikler

	• 🌡️ Gerçek Zamanlı Sensör Simülasyonu - Gerçekçi sıcaklık ve nem verileri üretir
	• 📡 MQTT İletişimi - Bileşenler arasında Pub/Sub mesajlaşması
	• 🏠 Hiyerarşik Organizasyon - Evler → Odalar → Cihazlar yapısı
	• 📊 Canlı Dashboard - SignalR ile gerçek zamanlı veri görselleştirme
	• 🗄️ Veri Kalıcılığı - Entity Framework Core ile PostgreSQL veritabanı
	• 🔄 RESTful API - Tüm varlıklar için CRUD işlemleri
	• 📱 Duyarlı UI - Angular Material tasarım arayüzü
	• 📈 Veri Analizi - Geçmiş sensör verilerini analiz etme
	• 🔧 Cihaz Yönetimi - IoT cihazları ekleme, düzenleme, aktif/pasif yapma

## 🏗️ Mimari Genel Bakış

```
┌─────────────────┐    MQTT     ┌─────────────────┐    HTTP/SignalR     ┌─────────────────┐
│   IoT Publisher │ ─────────── │ IoT Subscriber  │ ─────────────────── │    Angular UI   │
│   (.NET Core)   │             │   (.NET Web)    │                     │   (Frontend)    │
└─────────────────┘             └─────────────────┘                     └─────────────────┘
         │                               │                                         │
         │                               │                                         │
         └─────────── MQTT Broker ───────┼─────────────────────────────────────────┘
                   (Eclipse Mosquitto)   │
                                         │
                                   ┌─────────────┐
                                   │ PostgreSQL  │
                                   │  Database   │
                                   └─────────────┘
```

### Kullanılan Teknolojiler

    • Backend: .NET 9, Entity Framework Core, AutoMapper
	• Veritabanı: PostgreSQL
	• Mesajlaşma: MQTT (MQTTnet kütüphanesi)
	• Gerçek Zamanlı İletişim: SignalR
	• Frontend: Angular 17, Angular Material, TypeScript
	• Mesaj Broker: Eclipse Mosquitto MQTT Broker

## 🚀  Kurulum Talimatları

### Gereksinimler

	• NET 9 SDK
	• Node.js (v18 veya üstü)
	• PostgreSQL
	• MQTT Broker (Eclipse Mosquitto önerilir)

### Backend Kurulumu

# 1.Repository’yi klonlayın
   ```bash
   git clone <your-repository-url>
   cd IOTSimulator
   ```

# 2.PostgreSQL veritabanını oluşturun
   ```bash
   # Install PostgreSQL (macOS için Homebrew)
   brew install postgresql
   brew services start postgresql
   
   # Create database
   createdb IoTDb
   ```

# 3.Veritabanı bağlantısını yapılandırın
   
   `IoTSimulator.Subscriber/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=IoTDb;Username=postgres;Password=your_password;Pooling=true;Connection Lifetime=0;"
     }
   }
   ```

# 4.Veritabanı migrasyonlarını çalıştırın
   ```bash
   cd IoTSimulator.Subscriber
   dotnet ef database update
   ```

# 5.MQTT Broker kurun
   ```bash
   docker run -it -p 1883:1883 eclipse-mosquitto
   ```

### Frontend Kurulumu

# 1.UI projesine gidin
   ```bash
   cd IoTSimulator.UI
   ```

# 2.Bağımlılıkları yükleyin
   ```bash
   npm install
   ```

# 3.API konfigürasyonunu kontrol edin
   
   `src/environments/environment.ts`:
   ```typescript
   export const environment = {
     production: false,
     apiUrl: 'https://localhost:5001/api'
   };
   ```

## ⚙️ Configuration

### MQTT Broker Ayarları

Varsayılan `appsettings.json` ayarları:

```json
{
  "MqttSettings": {
    "BrokerHost": "localhost",
    "BrokerPort": 1883,
    "ClientId": "IoTSubscriber",
    "Topics": [
      "sensors/+/data"
    ]
  }
}
```

### Environment Variables

Aşağıdaki değişkenler ile konfigürasyonları geçersiz kılabilirsiniz:

- `MQTT_BROKER_HOST`: MQTT broker hostname
- `MQTT_BROKER_PORT`: MQTT broker port
- `CONNECTION_STRING`: Veritabanı bağlantı stringi

## 🎮 Kullanım Kılavuzu

### Uygulamayı Başlatma

# 1.Subscriber/API’yi başlatın (Terminal 1)
   ```bash
   cd IoTSimulator.Subscriber
   dotnet run
   ```
   API  `https://localhost:5001` adresinde çalışacaktır

# 	2.Publisher’ı başlatın (Terminal 2)
   ```bash
   cd IoTSimulator.Publisher
   dotnet run
   ```
   Bu, her saniye sensör verilerini üretip yayınlayacaktır.

# 3.Web UI’yi başlatın (Terminal 3)
   ```bash
   cd IoTSimulator.UI
   npm start
   ```
   UI `http://localhost:4200` adresinde kullanılabilir.

### Dashboard Kullanımı

# 1.House management
    • Yeni evler oluşturun (isim ve adres ile)
	• Tüm evleri grid görünümünde görüntüleyin
	• Mevcut evleri düzenleyin veya silin

# 2.Room management  
    • Evlerinize odalar ekleyin
	• Cihazları odalara göre organize edin
	• Oda bazlı sensör verilerini izleyin

# 3.Device management
    • IoT cihazları odalara ekleyin
	• Cihaz özelliklerini yapılandırın (isim, tür, üretici)
	• Cihazları aktif/pasif yapın
	• Gerçek zamanlı sensör verilerini görüntüleyin

# 4.Sensor data operations
   	• SignalR ile canlı veri güncellemeleri
	• Sıcaklık ve nem grafikleri
	• Cihaz durum göstergeleri
    • Geçmiş veri görselleştirme


### API Endpoints

- `GET/POST/PUT/DELETE /api/houses` - House management
- `GET/POST/PUT/DELETE /api/rooms` - Room management  
- `GET/POST/PUT/DELETE /api/devices` - Device management
- `GET/POST/PUT/DELETE /api/sensordata` - Sensor data operations
- `GET /api/dashboard` - Dashboard aggregated data

### MQTT Topics

- **Publish**: `sensors/{sensorId}/data`
- **Subscribe**: `sensors/+/data`

**Örnek MQTT Mesajı:**
```json
{
  "sensorId": "TEMP_001",
  "sensorName": "Living Room Sensor",
  "location": "Living Room",
  "temperature": 23.5,
  "humidity": 65.2,
  "timestamp": "2025-01-20T10:30:00Z"
}
```

## 📁 Kod Yapısı

### IoTSimulator.Publisher
- **Amaç**: IoT sensörlerini simüle eder ve MQTT üzerinden veri yayınlar
- **Ana Bileşenler:**:
  - `Services/SensorService.cs` - Gerçekçi sensör verisi üretir
  - `Services/MqttPublisherService.cs` - MQTT broker’a yayın yapar
  - `Models/SensorData.cs` - Sensör veri modeli

### IoTSimulator.Subscriber
- **Amaç**: MQTT mesajlarını alan ve REST API sağlayan Web API
- **Mimari**: Onion Architecture ile temiz katmanlı yapı
- **Ana Bileşenler:**:
  - `Controllers/` - REST API endpoints
  - `Application/Services/` - Business logic katmanı
  - `Domain/Models/` - Core entities (House, Room, IoTDevice, SensorData)
  - `Infrastructure/` - Veri erişim ve dış servisler
  - `Hubs/SensorDataHub.cs` - SignalR hub, gerçek zamanlı güncellemeler

### IoTSimulator.UI
- **Amaç**: İzleme ve yönetim için Angular web uygulaması
- **Ana Bileşenler:**:
  - `src/app/core/services/` - API ile iletişim servisleri
  - `src/app/components/` - UI bileşenleri
  - `src/app/models/` - TypeScript modelleri
  - `services/signalr.service.ts` - Gerçek zamanlı veri güncellemeleri

### Veritabanı Şeması

```
House (Id, Name, Address, CreatedAt, UpdatedAt)
  ↓
Room (Id, Name, HouseId, CreatedAt, UpdatedAt)
  ↓  
IoTDevice (Id, Name, RoomId, DeviceType, Manufacturer, Model, SerialNumber, IsActive, CreatedAt, UpdatedAt)
  ↓
SensorData (Id, DeviceId, SensorId, SensorName, Location, Temperature, Humidity, Timestamp, CreatedAt, UpdatedAt)
```