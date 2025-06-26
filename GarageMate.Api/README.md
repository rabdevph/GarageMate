# 🚗 GarageMate API

GarageMate is a backend system built with **ASP.NET Core Minimal API** using **Entity Framework Core** and **PostgreSQL**, designed to serve as the foundation for managing operations in automotive service shops and garage businesses.

## 📦 Current Features

### 🔹 Customers
- ✅ Create individual or company customers  
- ✅ Update contact and subtype details  
- ✅ Set status as **active** or **inactive** (soft delete)  
- ✅ Retrieve customer details (with optional filtering by type)  
- ✅ Paginated list of customers  

### 🔹 Vehicles
- ✅ Create new vehicles  
- ✅ Update vehicle information  
- ✅ Retrieve vehicle details with current/past ownership info  
- ✅ Paginated vehicle listing  

### 🔹 Vehicle Ownership
- ✅ Assign a customer as the current owner of a vehicle  
- ✅ Transfer ownership to a new customer  
- ✅ View individual ownership record  

## 🛠️ Tech Stack

- **Backend Framework**: ASP.NET Core 9 (Minimal API)
- **ORM**: Entity Framework Core
- **Database**: PostgreSQL
- **Language**: C#

## 📁 Folder Structure

```text
GarageMate.Api/
├── Data/                  # DbContext
├── Dtos/                  # Data Transfer Objects
├── Endpoints/             # Minimal API endpoints
├── Mapping/               # Manual mapping between DTOs and Entities
├── Models/                # Entity models
├── Enums/                 # Enum definitions
└── Helpers/               # Validation and shared logic
```

## 📌 Roadmap

- Add vehicle service history tracking
- Invoice and billing support
- Authentication and user roles
- Advanced filtering and search
- Blazor WebAssembly frontend
