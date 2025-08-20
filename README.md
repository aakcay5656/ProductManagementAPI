# 🛍️ Ürün Yönetim API

**Clean Architecture prensipleri ile geliştirilmiş kurumsal seviye RESTful API**

JWT kimlik doğrulama, Redis cache ve PostgreSQL veritabanı entegrasyonu içeren kapsamlı bir Ürün Yönetim sistemi.  
CQRS deseni ve MediatR kullanılarak ölçeklenebilir ve sürdürülebilir kod mimarisiyle geliştirilmiştir.

[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-blue.svg)](https://www.postgresql.org/)
[![Redis](https://img.shields.io/badge/Redis-7.0-red.svg)](https://redis.io/)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)



---

## ✨ Özellikler

- **🔐 JWT Kimlik Doğrulama** - Güvenli kullanıcı kaydı ve giriş  
- **📦 Ürün Yönetimi** - Ürünler için tam CRUD işlemleri  
- **⚡ Redis Cache** - TTL ile yüksek performanslı veri önbellekleme  
- **🗄️ PostgreSQL Veritabanı** - EF Core ile güçlü ilişkisel veritabanı  
- **🎯 CQRS Deseni** - Command Query Responsibility Segregation  
- **✅ Girdi Doğrulama** - FluentValidation pipeline davranışı ile  
- **📊 Swagger Dokümantasyonu** - Etkileşimli API dokümantasyonu  
- **🔄 Unit of Work** - Transaction yönetimi  
- **🎨 Clean Architecture** - Onion Architecture uygulaması  
- **📝 Yapılandırılmış Loglama** - Serilog entegrasyonu  
- **🚦 Hata Yönetimi** - Global exception handling middleware  

---

## 🏗️ Mimari

Bu proje **Clean Architecture (Onion Architecture)** prensiplerini izler:

```

┌─────────────────────────────────────────────┐
│                  🌐 API Katmanı              │
│            (Controller'lar, Middleware)     │
├─────────────────────────────────────────────┤
│              📋 Uygulama Katmanı             │
│        (CQRS, Handler’lar, DTO’lar, Doğrulama) │
├─────────────────────────────────────────────┤
│             🏗️ Altyapı Katmanı              │
│     (EF Core, Redis, JWT, Repository’ler)   │
├─────────────────────────────────────────────┤
│                💎 Çekirdek Katman           │
│          (Entity’ler, Arayüzler, Enum’lar) │
└─────────────────────────────────────────────┘

```

---

## 🛠️ Teknoloji Yığını

### Backend
- **.NET 9** – Son sürüm .NET framework  
- **ASP.NET Core Web API** – RESTful API framework  
- **Entity Framework Core** – ORM  
- **MediatR** – CQRS implementasyonu  
- **FluentValidation** – Girdi doğrulama  
- **AutoMapper** – Nesne eşleme  

### Veritabanı & Cache
- **PostgreSQL 16** – Ana veritabanı  
- **Redis 7** – Cache ve session yönetimi  

### Güvenlik & Kimlik Doğrulama
- **JWT Bearer Token** – Kimlik doğrulama  
- **PBKDF2** – Parola hashing  
- **HTTPS** – Güvenli iletişim  

### Araçlar & Kütüphaneler
- **Swagger/OpenAPI** – API dokümantasyonu  
- **Serilog** – Yapılandırılmış loglama  
- **Npgsql** – PostgreSQL .NET sürücüsü  
- **StackExchange.Redis** – Redis istemcisi  

---

## 📁 Proje Yapısı

```

ProductManagementAPI/
│   ├── 📂 ProductManagement.API/          # 🌐 Sunum Katmanı
│   │   ├── Controllers/                   # API Controller’ları
│   │   └── Middleware/                    # Custom middleware
│   │
│   ├── 📂 ProductManagement.Application/  # 📋 Uygulama Katmanı
│   │   ├── DTOs/                         # Data Transfer Objects
│   │   ├── Features/                     # CQRS Komut & Sorgular
│   │   │   ├── Auth/                     # Kimlik doğrulama işlemleri
│   │   │   └── Products/                 # Ürün işlemleri
│   │   └── Validators/                   # FluentValidation doğrulayıcıları
│   │
│   ├── 📂 ProductManagement.Infrastructure/ # 🏗️ Altyapı Katmanı
│   │   ├── Data/                         # Veritabanı context
│   │   ├── Repositories/                 # Repository implementasyonları
│   │   ├── Services/                     # Servis implementasyonları
│   │   └── Cache/                        # Redis cache servisi
│   │
│   └── 📂 ProductManagement.Core/         # 💎 Çekirdek Katman
│       ├── Entities/                     # Domain entity’leri
│       ├── Interfaces/                   # Repository arayüzleri
│       ├── Enums/                        # Enum’lar
│       └── Common/                       # Ortak yardımcılar
│
└── 📄 .gitignore                         # Git ignore kuralları

````

---

## 🚀 Başlangıç

### Gereksinimler

Aşağıdakilerin yüklü olduğundan emin olun:

- **.NET 9 SDK** - [İndir](https://dotnet.microsoft.com/download/dotnet/9.0)  
- **PostgreSQL 16+** - [İndir](https://www.postgresql.org/download/)  
- **Redis 7+** - [İndir](https://redis.io/download) veya Docker kullanın  
- **Git** - [İndir](https://git-scm.com/downloads)  

### Kurulum

1. **Repository’yi klonla**
   ```bash
   git clone https://github.com/aakcay5656/ProductManagementAPI.git
   cd ProductManagementAPI

2. **Bağımlılıkları yükle**

   ```bash
   dotnet restore
   ```

### Konfigürasyon

`appsettings.json` dosyasında connection string’leri güncelleyin:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5433;Database=ProductManagementDB;Username=postgres;Password=yourpassword;",
    "Redis": "localhost:6379"
  },
  "Jwt": {
    "Secret": "en-az-64-karakterlik-bir-super-secret-key",
    "Issuer": "ProductManagementAPI",
    "Audience": "ProductManagementAPI",
    "ExpirationMinutes": "1440"
  }
}
```

### Veritabanı Kurulumu

1. **PostgreSQL’i başlatın**
2. **Redis’i başlatın**
   veya

   ```bash
   docker run -d -p 6379:6379 redis:7-alpine
   ```
3. **Veritabanı ilk çalıştırmada otomatik oluşturulacaktır**

### Uygulamayı Çalıştırma

```bash
# Geliştirme
dotnet run --project src/ProductManagement.API

# Production
dotnet run --project src/ProductManagement.API --environment Production
```

---

## 📚 API Dokümantasyonu

### Kimlik Doğrulama Endpoint’leri

| Method | Endpoint                | Açıklama                   |
| ------ | ----------------------- | -------------------------- |
| `POST` | `/api/v1/auth/register` | Yeni kullanıcı kaydı       |
| `POST` | `/api/v1/auth/login`    | JWT token almak için giriş |

### Ürün Endpoint’leri

| Method   | Endpoint                | Açıklama                                   | Auth Gerektirir |
| -------- | ----------------------- | ------------------------------------------ | --------------- |
| `GET`    | `/api/v1/products`      | Tüm ürünleri listele (filtreleme destekli) | ❌               |
| `GET`    | `/api/v1/products/{id}` | Ürün ID’ye göre getir                      | ❌               |
| `POST`   | `/api/v1/products`      | Yeni ürün oluştur                          | ✅               |
| `PUT`    | `/api/v1/products/{id}` | Ürünü güncelle                             | ✅               |
| `DELETE` | `/api/v1/products/{id}` | Ürünü sil                                  | ✅               |

**GET `/api/v1/products` Query Parametreleri:**

* `category` – Kategoriye göre filtrele
* `search` – İsim ve açıklamada ara
* `page` – Sayfa numarası (varsayılan: 1)
* `pageSize` – Sayfa başına öğe (varsayılan: 10)

---

## 🧪 API Testi

### Swagger UI ile

1. `https://localhost:7025/index.html` adresine gidin
2. Kullanıcı kaydı oluşturun
3. Giriş yapıp JWT token alın
4. "Authorize" butonuna `Bearer {jwt-token}` girin
5. Ürün endpoint’lerini test edin


## 🔧 Geliştirme

### Hot Reload ile Çalıştırma

```bash
dotnet watch run --project src/ProductManagement.API
```

### Kod Formatlama

```bash
dotnet format
```

---

## 📊 Performans Özellikleri

* **Redis Cache**: Ürün listeleri 5 dakika cache’de tutulur
* **Sorgu Optimizasyonu**: EF Core Include ile ilişkili veriler çekilir
* **Sayfalama**: Page-based pagination desteği
* **Connection Pooling**: Veritabanı bağlantısı optimizasyonu
* **Async Operasyonlar**: Non-blocking veri tabanı işlemleri

---

## 🛡️ Güvenlik

* **JWT Kimlik Doğrulama** – Stateless auth, ayarlanabilir expiration
* **Parola Hashing** – PBKDF2 + Salt
* **Input Validation** – FluentValidation ile
* **SQL Injection Koruması** – EF Core parametreli sorgular
* **HTTPS Zorunluluğu** – Production ortamında güvenli iletişim
* **CORS Yapılandırması** – Cross-origin istek yönetimi



