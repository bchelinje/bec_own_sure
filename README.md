# 📘 Device Ownership & Anti-Theft Platform

**Recommended Stack:** ⭐ ASP.NET Core + PostgreSQL + Angular + Flutter + Azure (minimal services)

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/)
[![Angular](https://img.shields.io/badge/Angular-17-DD0031)](https://angular.io/)
[![Flutter](https://img.shields.io/badge/Flutter-3.16-02569B)](https://flutter.dev/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-336791)](https://www.postgresql.org/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

# 🚀 Device Ownership & Anti-Theft Platform

A modern, secure, multi-platform solution for registering devices, verifying ownership, preventing the sale of stolen goods, and transferring device ownership globally.

> **Note:** Replace the project name once finalised.

---

# 🚀 Overview

This system allows users to securely register their electronic devices and valuables using serial numbers, link them to their verified identity, and protect them against loss or theft. Buyers can check devices before purchasing, owners can transfer ownership, and police can view reports of stolen items.

The platform includes:

* **Mobile App (Flutter)**
* **Angular Web App**
* **ASP.NET Core API (OpenIddict Authentication)**
* **Police & Admin Portal**

It is designed for **global rollout**, starting with the UK.

---

# 🎯 Core Purpose

* Keep a **permanent, secure record** of device serial numbers.
* Link devices to verified user identities.
* Enable **ownership transfers**, **global blacklist checks**, and **theft reporting**.
* Build a trusted ecosystem for **second-hand electronics trading**.
* Support police and community involvement to reduce stolen goods circulation.

---

# 🔑 Key Features

## ✔️ 1. Device Registration

Users can register any valuable item by providing:

* Serial number
* Photos of the item and serial number
* Category (phone, laptop, appliance, etc.)
* Receipt or proof of purchase
* Warranty documents

A permanent ownership record is created.

---

## ✔️ 2. Ownership Verification & Certificate

* System generates a **proof-of-ownership certificate**.
* Includes QR code that links to verification page.
* Useful for insurance, resale, and police cases.

---

## ✔️ 3. Transfer of Ownership

* Current owner initiates transfer.
* Buyer receives notification.
* Buyer accepts and device ownership updates.
* Complete ownership history stored.

---

## ✔️ 4. Stolen/Lost Item Reporting

* Mark device as **Lost**, **Stolen**, or **Recovered**.
* Sends alerts to:

  * Police accounts
  * Community in the area (optional)
  * Anyone who checks the serial number
* Helps prevent resale of stolen items.

---

## ✔️ 5. Global Serial Number Blacklist Check

Buyers can check a serial number before buying:

* Stolen ✔️
* Lost ✔️
* Registered to someone else ✔️
* Safe to buy ✔️

This reduces second-hand market fraud.

---

## ✔️ 6. Secure Serial Number Storage

* All serial numbers stored encrypted.
* Provides a safe, permanent backup.
* Helps with warranty claims, insurance, and police reporting.

---

## ✔️ 7. Trusted Second-Hand Marketplace

* Only verified owners can list items.
* Buyers see legitimacy ("Verified Owner").
* Reduces risk of stolen goods.

---

## ✔️ 8. Police Portal

* Police can search serial numbers.
* View stolen reports.
* Contact original owners.
* Add official recovery notes.

---

## ✔️ 9. Business Dashboard (Optional Expansion)

For:

* Builders / Electricians
* IT companies
* Schools
* Retailers

Features include:

* Bulk device registration
* Ownership assignment
* Theft reporting
* Employee device tracking

---

# 🌍 Expansion to Other Countries

The platform supports global rollout with:

* Multi-language support
* Country-specific device categories
* Regional police integrations
* Local fraud and compliance rules
* International serial number formats

---

# 🧱 Planned Tech Stack

### **Frontend**

* Angular (Web + Admin + Police)
* Flutter / MAUI (Mobile Apps)

### **Backend**

* ASP.NET Core Web API
* OpenIddict for authentication (Authorization Code Flow + PKCE)
* SQL Server or PostgreSQL
* Azure Blob Storage

### **Services**

* Email / SMS notifications
* Push notifications for theft alerts
* Optional OCR for reading serial numbers from photos

---

# 💰 Monetisation Model (Draft)

* Free registration for 3 devices
* £0.99 per additional device OR £1.99/month unlimited
* Marketplace fees
* Verification fees for businesses (pawn shops, repair shops)
* Ownership certificate fee
* Insurance partnerships

---

# 📦 Roadmap

## **MVP Phase 1**

* User registration & authentication
* Device registration
* Serial number storage
* Theft/lost reporting
* Ownership transfer
* Basic verification check
* Basic admin panel

## **Phase 2**

* Marketplace
* Police portal
* Business dashboard
* Ownership certificates
* Community alerts
* OCR serial scanning

## **Phase 3**

* Global rollout
* Insurance integration
* API access for partners
* International device database connections

---

# 📣 Mission Statement

To create a secure global network where individuals, communities, and authorities work together to protect valuable items, verify ownership, and eliminate the circulation of stolen goods.

---

# 📚 Documentation

Comprehensive documentation has been created to help you understand, develop, and deploy this platform:

## Architecture & Design
- **[System Architecture](docs/ARCHITECTURE.md)** - Complete system architecture with diagrams, security design, data flows, and deployment architecture
- **[Database Schema](docs/DATABASE_SCHEMA.md)** - Full PostgreSQL database schema with tables, relationships, indexes, and optimization strategies
- **[Authentication Flows](docs/AUTHENTICATION_FLOWS.md)** - OAuth 2.0/OpenID Connect flows using OpenIddict for all user types
- **[API Endpoints](docs/API_ENDPOINTS.md)** - Complete REST API specification with all endpoints, request/response formats, and examples

## Development
- **[Implementation Status](docs/IMPLEMENTATION_STATUS.md)** ⭐ **START HERE** - Current progress, what's complete, and next steps
- **[Project Structure](docs/PROJECT_STRUCTURE.md)** - Detailed folder structure for backend (ASP.NET Core), frontend (Angular), and mobile (Flutter)
- **[Development Guide](docs/DEVELOPMENT_GUIDE.md)** - Setup instructions, coding patterns, testing strategies, and best practices
- **[Deployment Guide](docs/DEPLOYMENT_GUIDE.md)** - Azure deployment, CI/CD pipelines, monitoring, scaling, and production best practices

## Quick Links
- **Backend**: `/backend` - ASP.NET Core 8 Web API with Clean Architecture
- **Frontend**: `/frontend-web` - Angular 17 web application
- **Mobile**: `/mobile-app` - Flutter cross-platform mobile app
- **Database**: `/database` - PostgreSQL schema and migration scripts
- **Infrastructure**: `/infrastructure` - Azure ARM templates, Bicep, and Docker configurations

---

# 🚀 Quick Start

## Prerequisites
- .NET 8 SDK
- Node.js 20+
- Flutter 3.16+
- PostgreSQL 16+
- Redis (for caching)

## Backend Setup
```bash
cd backend
dotnet restore
dotnet ef database update -p src/DeviceOwnership.Infrastructure -s src/DeviceOwnership.API
dotnet run --project src/DeviceOwnership.API
```

## Frontend Setup
```bash
cd frontend-web
npm install
ng serve
```

## Mobile Setup
```bash
cd mobile-app
flutter pub get
flutter run
```

For detailed setup instructions, see the [Development Guide](docs/DEVELOPMENT_GUIDE.md).

---

# 🏗️ Technology Stack

## Backend
- **Framework**: ASP.NET Core 8.0
- **Authentication**: OpenIddict (OAuth 2.0 / OpenID Connect)
- **Database**: PostgreSQL 16
- **ORM**: Entity Framework Core 8
- **Caching**: Redis
- **Storage**: Azure Blob Storage
- **Secrets**: Azure Key Vault
- **Email**: SendGrid
- **SMS**: Twilio
- **Payments**: Stripe

## Frontend
- **Framework**: Angular 17
- **State Management**: RxJS
- **UI**: Tailwind CSS
- **OAuth Client**: angular-oauth2-oidc
- **i18n**: ngx-translate

## Mobile
- **Framework**: Flutter 3.16
- **State Management**: flutter_bloc
- **HTTP Client**: dio
- **Local Storage**: hive
- **Secure Storage**: flutter_secure_storage
- **Push Notifications**: firebase_messaging

## Infrastructure
- **Cloud**: Azure
- **CI/CD**: GitHub Actions
- **Monitoring**: Application Insights
- **Container**: Docker
- **IaC**: Bicep / Terraform

---

# 💡 Key Features Explained

## Marketplace
The platform includes a trusted second-hand marketplace where:
- Only verified device owners can list items for sale
- Buyers can see complete ownership history
- All listings are verified against the theft database
- Automatic ownership transfer upon purchase
- Escrow payment system for security
- Rating and review system for buyers/sellers
- Protection against stolen goods

---

# 📊 Project Status

**Phase 1: Foundation & Design** ✅ **COMPLETE**
- ✅ Architecture Design Complete
- ✅ Database Schema Complete (PostgreSQL ready to deploy)
- ✅ API Specification Complete (60+ endpoints documented)
- ✅ Authentication Flow Design Complete (OAuth 2.0/OpenIddict)
- ✅ Backend Core Layer Complete (13 entities, 5 enums, 4 interfaces)

**Phase 2: Backend Implementation** ⏳ **IN PROGRESS**
- ⏳ Infrastructure Layer (Pending - DbContext, Repositories, Services)
- ⏳ Application Layer (Pending - DTOs, Business Logic, Validators)
- ⏳ API Layer (Pending - Controllers, Middleware, Filters)

**Phase 3: Frontend & Mobile** ⏳ **PLANNED**
- ⏳ Angular Web Application
- ⏳ Flutter Mobile Application

**Phase 4: DevOps & Deployment** ⏳ **PLANNED**
- ⏳ CI/CD Pipelines (GitHub Actions)
- ⏳ Azure Infrastructure (Bicep templates)
- ⏳ Production Deployment

📋 **[View Detailed Implementation Status](docs/IMPLEMENTATION_STATUS.md)** - Complete progress tracking and next steps

---

# 🤝 Contributing

We welcome contributions! Please read our [Development Guide](docs/DEVELOPMENT_GUIDE.md) for:
- Code style guidelines
- Branch naming conventions
- Pull request process
- Testing requirements

---

# 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

---

# 📞 Contact & Support

- **Issues**: [GitHub Issues](https://github.com/your-org/device-ownership-platform/issues)
- **Email**: support@deviceownership.com
- **Documentation**: [Full Documentation](docs/)

---

**Built with ❤️ for a safer second-hand market**
