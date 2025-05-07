# Authentication & Authorization API
## 📋 Overview
Authentication API is a RESTful API developed for robust user authentication and authorization services, adhering to Clean Architecture and SOLID principles. This API provides a secure and scalable identity management solution with JWT token-based authentication that can be integrated with various applications.

## 🚀 Features
👤 User management (registration, login, profile management)

🔑 Authentication with JWT tokens

🔄 Token refresh mechanism

👮‍♀️ Role-based authorization

🔒 Secure password handling and storage

## 🛠️ Technologies Used
| Technology | Description |
|-----------|----------|
| **.NET 9** | Core platform for the API |
| **ASP.NET Identity** | For user management and authentication |
| **JWT Authentication** | For secure, token-based authentication |
| **Redis** | Token storage and blacklisting |
| **Swagger** | API documentation and testing interface |

```
📁 Solution
  ├── 📁 Auth.WebAPI/           
  │   ├── Controllers
  │   ├── API
  │   ├── DI Container
  │   └── Program.cs
  │
  ├── 📁 Auth.Application/      
  │   ├── DTO
  │   ├── Abstract
  │   ├── Services
  │   ├── Common
  │
  ├── 📁 Auth.Domain/           
  │   ├── Entities
  │
  ├── 📁 Auth.Infrastructure/   
  │   ├── Repositories
  │
  ├── 📁 Auth.Persistence/   
  │   ├── Context
  
  
```

## 🔧 Installation
```bash
# Clone the repository
git clone https://github.com/mehmetcanari/dotnet-auth-api.git
cd dotnet-auth-api
dotnet restore
cd Auth.WebAPI
dotnet watch run

```

## 🔑 Environment Variables
Set the following environment variables before running the project:
```
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://localhost:5001

JWT_SECRET=YourSecretKey
JWT_ISSUER=AuthJwtIssuer
JWT_AUDIENCE=AuthJwtAudience
```

## 🌟 Basic API Usage
### Authentication Endpoints
```http

@baseUrl = http://localhost:5001/api

### REGISTER 

POST {{baseUrl}}/auth/register
Content-Type: application/json

{
  "Name": "John", 
  "Surname": "Doe",
  "Email": "johndoe@test.com",
  "Password": "@TestPwd123"
}

### LOGIN

POST {{baseUrl}}/auth/login
Content-Type: application/json

{
  "Email": "johndoe@test.com",
  "Password": "@TestPwd123"
}

### LOGOUT
POST {{baseUrl}}/auth/logout

### REFRESH TOKEN

GET {{baseUrl}}/auth/refresh-token

### GET PROFILE

GET {{baseUrl}}/account/profile
Authorization: Bearer {{accessToken}}
```

## 📧 Contact
Project Owner - [bsn.mehmetcanari@gmail.com](mailto:bsn.mehmetcanari@gmail.com)

[![GitHub](https://img.shields.io/badge/github-%23121011.svg?style=for-the-badge&logo=github&logoColor=white)](https://github.com/mehmetcanari)

---
