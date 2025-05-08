# FamilyWallet API

RESTful Web API за управление на лични и семейни финанси, изградена с .NET 8.

## 🛠 Технологии
- ASP.NET Core Web API (.NET 8)
- Entity Framework Core + PostgreSQL
- JWT Authentication
- Docker (контейнеризация)

---

## 🚀 Стартиране с Docker

### 1. Клонирай репото

```bash```
git clone https://github.com/your-username/familywallet.git
cd familywallet


### 2. Построй Docker image-а

```bash```
docker build -t familywallet-api 

### 3. Стартирай контейнер
```bash```
docker run -d -p 5095:8080 --name familywallet-container familywallet-api