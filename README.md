# 🚗 GarageMate API (on hold)

> I'm currently focusing on a new project, [**DriveOps**](https://github.com/rabdevph/DriveOps), which builds upon many of the ideas from GarageMate with a cleaner architecture and improved design patterns.

GarageMate is a backend system built with **ASP.NET Core Minimal API**, **Entity Framework Core**, and **PostgreSQL**, designed to serve as a foundation for managing operations in automotive service shops and garage businesses.

---

### 📌 Looking for the newer version?

Check out my actively developed project:  
🔗 **[DriveOps GitHub Repository](https://github.com/rabdevph/DriveOps)**

---

## 📦 Current Features (as of last update)

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

---

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
