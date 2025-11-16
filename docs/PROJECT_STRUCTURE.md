# 📁 Project Structure

## Complete Directory Layout

```
bec_own_sure/
├── README.md
├── .gitignore
├── docs/
│   ├── ARCHITECTURE.md
│   ├── AUTHENTICATION_FLOWS.md
│   ├── DATABASE_SCHEMA.md
│   ├── API_ENDPOINTS.md
│   ├── PROJECT_STRUCTURE.md
│   ├── DEPLOYMENT_GUIDE.md
│   └── DEVELOPMENT_GUIDE.md
│
├── backend/
│   ├── DeviceOwnership.sln
│   ├── .editorconfig
│   ├── Directory.Build.props
│   ├── global.json
│   │
│   ├── src/
│   │   ├── DeviceOwnership.API/
│   │   │   ├── DeviceOwnership.API.csproj
│   │   │   ├── Program.cs
│   │   │   ├── appsettings.json
│   │   │   ├── appsettings.Development.json
│   │   │   ├── appsettings.Production.json
│   │   │   │
│   │   │   ├── Controllers/
│   │   │   │   ├── AuthController.cs
│   │   │   │   ├── DevicesController.cs
│   │   │   │   ├── TransfersController.cs
│   │   │   │   ├── ReportsController.cs
│   │   │   │   ├── MarketplaceController.cs
│   │   │   │   ├── SubscriptionsController.cs
│   │   │   │   ├── NotificationsController.cs
│   │   │   │   ├── PoliceController.cs
│   │   │   │   └── AdminController.cs
│   │   │   │
│   │   │   ├── Filters/
│   │   │   │   ├── ValidateModelAttribute.cs
│   │   │   │   ├── RequireScopeAttribute.cs
│   │   │   │   ├── RequireEmailVerifiedAttribute.cs
│   │   │   │   └── ApiExceptionFilter.cs
│   │   │   │
│   │   │   ├── Middleware/
│   │   │   │   ├── RateLimitingMiddleware.cs
│   │   │   │   ├── AuditLoggingMiddleware.cs
│   │   │   │   └── SecurityHeadersMiddleware.cs
│   │   │   │
│   │   │   ├── Extensions/
│   │   │   │   ├── ServiceCollectionExtensions.cs
│   │   │   │   └── ApplicationBuilderExtensions.cs
│   │   │   │
│   │   │   └── Properties/
│   │   │       └── launchSettings.json
│   │   │
│   │   ├── DeviceOwnership.Core/
│   │   │   ├── DeviceOwnership.Core.csproj
│   │   │   │
│   │   │   ├── Entities/
│   │   │   │   ├── User.cs
│   │   │   │   ├── Device.cs
│   │   │   │   ├── DevicePhoto.cs
│   │   │   │   ├── DeviceDocument.cs
│   │   │   │   ├── DeviceCategory.cs
│   │   │   │   ├── OwnershipHistory.cs
│   │   │   │   ├── OwnershipTransfer.cs
│   │   │   │   ├── TheftReport.cs
│   │   │   │   ├── PoliceReport.cs
│   │   │   │   ├── PoliceProfile.cs
│   │   │   │   ├── MarketplaceListing.cs
│   │   │   │   ├── Subscription.cs
│   │   │   │   ├── Payment.cs
│   │   │   │   ├── BusinessProfile.cs
│   │   │   │   ├── Notification.cs
│   │   │   │   └── AuditLog.cs
│   │   │   │
│   │   │   ├── Enums/
│   │   │   │   ├── DeviceStatus.cs
│   │   │   │   ├── ReportType.cs
│   │   │   │   ├── TransferStatus.cs
│   │   │   │   ├── SubscriptionPlan.cs
│   │   │   │   ├── UserRole.cs
│   │   │   │   └── NotificationType.cs
│   │   │   │
│   │   │   ├── Interfaces/
│   │   │   │   ├── Repositories/
│   │   │   │   │   ├── IUserRepository.cs
│   │   │   │   │   ├── IDeviceRepository.cs
│   │   │   │   │   ├── IOwnershipRepository.cs
│   │   │   │   │   ├── ITheftReportRepository.cs
│   │   │   │   │   ├── IMarketplaceRepository.cs
│   │   │   │   │   ├── ISubscriptionRepository.cs
│   │   │   │   │   └── INotificationRepository.cs
│   │   │   │   │
│   │   │   │   └── Services/
│   │   │   │       ├── IDeviceService.cs
│   │   │   │       ├── IOwnershipService.cs
│   │   │   │       ├── ITheftReportService.cs
│   │   │   │       ├── IMarketplaceService.cs
│   │   │   │       ├── ISubscriptionService.cs
│   │   │   │       ├── IEmailService.cs
│   │   │   │       ├── ISmsService.cs
│   │   │   │       ├── IStorageService.cs
│   │   │   │       ├── IEncryptionService.cs
│   │   │   │       ├── INotificationService.cs
│   │   │   │       └── IAuditService.cs
│   │   │   │
│   │   │   ├── Exceptions/
│   │   │   │   ├── DeviceOwnershipException.cs
│   │   │   │   ├── NotFoundException.cs
│   │   │   │   ├── ValidationException.cs
│   │   │   │   ├── UnauthorizedException.cs
│   │   │   │   └── BusinessRuleException.cs
│   │   │   │
│   │   │   └── ValueObjects/
│   │   │       ├── SerialNumber.cs
│   │   │       ├── VerificationCode.cs
│   │   │       └── TransferCode.cs
│   │   │
│   │   ├── DeviceOwnership.Application/
│   │   │   ├── DeviceOwnership.Application.csproj
│   │   │   │
│   │   │   ├── DTOs/
│   │   │   │   ├── Requests/
│   │   │   │   │   ├── RegisterUserRequest.cs
│   │   │   │   │   ├── RegisterDeviceRequest.cs
│   │   │   │   │   ├── InitiateTransferRequest.cs
│   │   │   │   │   ├── ReportTheftRequest.cs
│   │   │   │   │   ├── CreateListingRequest.cs
│   │   │   │   │   └── SubscribeRequest.cs
│   │   │   │   │
│   │   │   │   └── Responses/
│   │   │   │       ├── UserResponse.cs
│   │   │   │       ├── DeviceResponse.cs
│   │   │   │       ├── DeviceDetailResponse.cs
│   │   │   │       ├── TransferResponse.cs
│   │   │   │       ├── TheftReportResponse.cs
│   │   │   │       ├── ListingResponse.cs
│   │   │   │       └── SubscriptionResponse.cs
│   │   │   │
│   │   │   ├── Services/
│   │   │   │   ├── DeviceService.cs
│   │   │   │   ├── OwnershipService.cs
│   │   │   │   ├── TheftReportService.cs
│   │   │   │   ├── MarketplaceService.cs
│   │   │   │   ├── SubscriptionService.cs
│   │   │   │   ├── NotificationService.cs
│   │   │   │   └── AuditService.cs
│   │   │   │
│   │   │   ├── Validators/
│   │   │   │   ├── RegisterUserValidator.cs
│   │   │   │   ├── RegisterDeviceValidator.cs
│   │   │   │   ├── InitiateTransferValidator.cs
│   │   │   │   └── ReportTheftValidator.cs
│   │   │   │
│   │   │   ├── Mappings/
│   │   │   │   └── AutoMapperProfile.cs
│   │   │   │
│   │   │   ├── BackgroundJobs/
│   │   │   │   ├── ExpireTransfersJob.cs
│   │   │   │   ├── ExpireListingsJob.cs
│   │   │   │   ├── SendNotificationsJob.cs
│   │   │   │   └── CleanupAuditLogsJob.cs
│   │   │   │
│   │   │   └── Extensions/
│   │   │       └── ServiceCollectionExtensions.cs
│   │   │
│   │   └── DeviceOwnership.Infrastructure/
│   │       ├── DeviceOwnership.Infrastructure.csproj
│   │       │
│   │       ├── Data/
│   │       │   ├── ApplicationDbContext.cs
│   │       │   ├── DbContextFactory.cs
│   │       │   │
│   │       │   ├── Configurations/
│   │       │   │   ├── UserConfiguration.cs
│   │       │   │   ├── DeviceConfiguration.cs
│   │       │   │   ├── OwnershipHistoryConfiguration.cs
│   │       │   │   ├── TheftReportConfiguration.cs
│   │       │   │   ├── MarketplaceListingConfiguration.cs
│   │       │   │   └── SubscriptionConfiguration.cs
│   │       │   │
│   │       │   ├── Migrations/
│   │       │   │   └── (EF Core migrations)
│   │       │   │
│   │       │   └── Seeders/
│   │       │       ├── DeviceCategoriesSeeder.cs
│   │       │       ├── AdminUserSeeder.cs
│   │       │       └── OpenIddictClientsSeeder.cs
│   │       │
│   │       ├── Repositories/
│   │       │   ├── BaseRepository.cs
│   │       │   ├── UserRepository.cs
│   │       │   ├── DeviceRepository.cs
│   │       │   ├── OwnershipRepository.cs
│   │       │   ├── TheftReportRepository.cs
│   │       │   ├── MarketplaceRepository.cs
│   │       │   ├── SubscriptionRepository.cs
│   │       │   └── NotificationRepository.cs
│   │       │
│   │       ├── Services/
│   │       │   ├── EmailService.cs (SendGrid)
│   │       │   ├── SmsService.cs (Twilio)
│   │       │   ├── BlobStorageService.cs (Azure Blob)
│   │       │   ├── EncryptionService.cs (Azure Key Vault)
│   │       │   ├── QRCodeService.cs
│   │       │   ├── PaymentService.cs (Stripe)
│   │       │   └── PushNotificationService.cs (Azure Notification Hubs)
│   │       │
│   │       ├── Authentication/
│   │       │   ├── OpenIddictConfiguration.cs
│   │       │   └── JwtConfiguration.cs
│   │       │
│   │       └── Extensions/
│   │           └── ServiceCollectionExtensions.cs
│   │
│   ├── tests/
│   │   ├── DeviceOwnership.Tests/
│   │   │   ├── DeviceOwnership.Tests.csproj
│   │   │   │
│   │   │   ├── Unit/
│   │   │   │   ├── Services/
│   │   │   │   │   ├── DeviceServiceTests.cs
│   │   │   │   │   ├── OwnershipServiceTests.cs
│   │   │   │   │   └── TheftReportServiceTests.cs
│   │   │   │   │
│   │   │   │   └── Validators/
│   │   │   │       ├── RegisterDeviceValidatorTests.cs
│   │   │   │       └── InitiateTransferValidatorTests.cs
│   │   │   │
│   │   │   ├── Integration/
│   │   │   │   ├── Controllers/
│   │   │   │   │   ├── DevicesControllerTests.cs
│   │   │   │   │   ├── TransfersControllerTests.cs
│   │   │   │   │   └── ReportsControllerTests.cs
│   │   │   │   │
│   │   │   │   └── Repositories/
│   │   │   │       ├── DeviceRepositoryTests.cs
│   │   │   │       └── OwnershipRepositoryTests.cs
│   │   │   │
│   │   │   └── Helpers/
│   │   │       ├── TestDataFactory.cs
│   │   │       ├── TestDbContextFactory.cs
│   │   │       └── MockServices.cs
│   │   │
│   │   └── DeviceOwnership.E2ETests/
│   │       ├── DeviceOwnership.E2ETests.csproj
│   │       ├── Scenarios/
│   │       │   ├── DeviceRegistrationScenario.cs
│   │       │   ├── OwnershipTransferScenario.cs
│   │       │   └── TheftReportingScenario.cs
│   │       └── Helpers/
│   │           └── ApiTestFixture.cs
│   │
│   └── scripts/
│       ├── setup-dev-env.sh
│       ├── run-migrations.sh
│       ├── seed-data.sh
│       └── generate-certificates.sh
│
├── frontend-web/
│   ├── package.json
│   ├── angular.json
│   ├── tsconfig.json
│   ├── tailwind.config.js
│   │
│   ├── src/
│   │   ├── app/
│   │   │   ├── app.component.ts
│   │   │   ├── app.routes.ts
│   │   │   ├── app.config.ts
│   │   │   │
│   │   │   ├── core/
│   │   │   │   ├── guards/
│   │   │   │   │   ├── auth.guard.ts
│   │   │   │   │   ├── role.guard.ts
│   │   │   │   │   └── email-verified.guard.ts
│   │   │   │   │
│   │   │   │   ├── interceptors/
│   │   │   │   │   ├── auth.interceptor.ts
│   │   │   │   │   ├── error.interceptor.ts
│   │   │   │   │   └── loading.interceptor.ts
│   │   │   │   │
│   │   │   │   ├── services/
│   │   │   │   │   ├── auth.service.ts
│   │   │   │   │   ├── device.service.ts
│   │   │   │   │   ├── transfer.service.ts
│   │   │   │   │   ├── report.service.ts
│   │   │   │   │   ├── marketplace.service.ts
│   │   │   │   │   ├── subscription.service.ts
│   │   │   │   │   └── notification.service.ts
│   │   │   │   │
│   │   │   │   └── models/
│   │   │   │       ├── user.model.ts
│   │   │   │       ├── device.model.ts
│   │   │   │       ├── transfer.model.ts
│   │   │   │       ├── report.model.ts
│   │   │   │       └── listing.model.ts
│   │   │   │
│   │   │   ├── features/
│   │   │   │   ├── auth/
│   │   │   │   │   ├── login/
│   │   │   │   │   ├── register/
│   │   │   │   │   ├── verify-email/
│   │   │   │   │   └── forgot-password/
│   │   │   │   │
│   │   │   │   ├── devices/
│   │   │   │   │   ├── device-list/
│   │   │   │   │   ├── device-detail/
│   │   │   │   │   ├── register-device/
│   │   │   │   │   └── check-serial/
│   │   │   │   │
│   │   │   │   ├── transfers/
│   │   │   │   │   ├── transfer-list/
│   │   │   │   │   ├── initiate-transfer/
│   │   │   │   │   └── accept-transfer/
│   │   │   │   │
│   │   │   │   ├── reports/
│   │   │   │   │   ├── report-list/
│   │   │   │   │   ├── report-theft/
│   │   │   │   │   └── nearby-reports/
│   │   │   │   │
│   │   │   │   ├── marketplace/
│   │   │   │   │   ├── listing-list/
│   │   │   │   │   ├── listing-detail/
│   │   │   │   │   ├── create-listing/
│   │   │   │   │   └── purchase/
│   │   │   │   │
│   │   │   │   ├── profile/
│   │   │   │   │   ├── user-profile/
│   │   │   │   │   ├── subscription/
│   │   │   │   │   └── payment-history/
│   │   │   │   │
│   │   │   │   ├── police/
│   │   │   │   │   ├── search-devices/
│   │   │   │   │   ├── theft-reports/
│   │   │   │   │   └── case-management/
│   │   │   │   │
│   │   │   │   └── admin/
│   │   │   │       ├── dashboard/
│   │   │   │       ├── user-management/
│   │   │   │       ├── police-verification/
│   │   │   │       └── analytics/
│   │   │   │
│   │   │   └── shared/
│   │   │       ├── components/
│   │   │       │   ├── header/
│   │   │       │   ├── footer/
│   │   │       │   ├── sidebar/
│   │   │       │   ├── device-card/
│   │   │       │   ├── photo-upload/
│   │   │       │   └── qr-code/
│   │   │       │
│   │   │       ├── pipes/
│   │   │       │   ├── date-format.pipe.ts
│   │   │       │   └── currency.pipe.ts
│   │   │       │
│   │   │       └── directives/
│   │   │           └── role-access.directive.ts
│   │   │
│   │   ├── assets/
│   │   │   ├── images/
│   │   │   ├── icons/
│   │   │   └── i18n/
│   │   │       ├── en.json
│   │   │       └── fr.json
│   │   │
│   │   ├── styles/
│   │   │   ├── styles.scss
│   │   │   └── tailwind.scss
│   │   │
│   │   └── environments/
│   │       ├── environment.ts
│   │       ├── environment.development.ts
│   │       └── environment.production.ts
│   │
│   └── README.md
│
├── mobile-app/
│   ├── pubspec.yaml
│   ├── analysis_options.yaml
│   │
│   ├── lib/
│   │   ├── main.dart
│   │   │
│   │   ├── core/
│   │   │   ├── config/
│   │   │   │   ├── app_config.dart
│   │   │   │   └── theme_config.dart
│   │   │   │
│   │   │   ├── constants/
│   │   │   │   ├── api_constants.dart
│   │   │   │   └── app_constants.dart
│   │   │   │
│   │   │   ├── services/
│   │   │   │   ├── api_service.dart
│   │   │   │   ├── auth_service.dart
│   │   │   │   ├── storage_service.dart
│   │   │   │   ├── camera_service.dart
│   │   │   │   ├── qr_service.dart
│   │   │   │   └── notification_service.dart
│   │   │   │
│   │   │   └── utils/
│   │   │       ├── validators.dart
│   │   │       ├── formatters.dart
│   │   │       └── helpers.dart
│   │   │
│   │   ├── data/
│   │   │   ├── models/
│   │   │   │   ├── user_model.dart
│   │   │   │   ├── device_model.dart
│   │   │   │   ├── transfer_model.dart
│   │   │   │   └── report_model.dart
│   │   │   │
│   │   │   ├── repositories/
│   │   │   │   ├── device_repository.dart
│   │   │   │   ├── transfer_repository.dart
│   │   │   │   ├── report_repository.dart
│   │   │   │   └── marketplace_repository.dart
│   │   │   │
│   │   │   └── local/
│   │   │       └── local_database.dart
│   │   │
│   │   ├── presentation/
│   │   │   ├── blocs/
│   │   │   │   ├── auth/
│   │   │   │   │   ├── auth_bloc.dart
│   │   │   │   │   ├── auth_event.dart
│   │   │   │   │   └── auth_state.dart
│   │   │   │   │
│   │   │   │   ├── device/
│   │   │   │   │   ├── device_bloc.dart
│   │   │   │   │   ├── device_event.dart
│   │   │   │   │   └── device_state.dart
│   │   │   │   │
│   │   │   │   └── transfer/
│   │   │   │       ├── transfer_bloc.dart
│   │   │   │       ├── transfer_event.dart
│   │   │   │       └── transfer_state.dart
│   │   │   │
│   │   │   ├── screens/
│   │   │   │   ├── auth/
│   │   │   │   │   ├── login_screen.dart
│   │   │   │   │   ├── register_screen.dart
│   │   │   │   │   └── verify_email_screen.dart
│   │   │   │   │
│   │   │   │   ├── home/
│   │   │   │   │   └── home_screen.dart
│   │   │   │   │
│   │   │   │   ├── devices/
│   │   │   │   │   ├── device_list_screen.dart
│   │   │   │   │   ├── device_detail_screen.dart
│   │   │   │   │   ├── register_device_screen.dart
│   │   │   │   │   └── check_serial_screen.dart
│   │   │   │   │
│   │   │   │   ├── transfers/
│   │   │   │   │   ├── transfer_list_screen.dart
│   │   │   │   │   └── initiate_transfer_screen.dart
│   │   │   │   │
│   │   │   │   ├── reports/
│   │   │   │   │   ├── report_list_screen.dart
│   │   │   │   │   └── report_theft_screen.dart
│   │   │   │   │
│   │   │   │   ├── marketplace/
│   │   │   │   │   ├── marketplace_screen.dart
│   │   │   │   │   └── listing_detail_screen.dart
│   │   │   │   │
│   │   │   │   └── profile/
│   │   │   │       ├── profile_screen.dart
│   │   │   │       └── subscription_screen.dart
│   │   │   │
│   │   │   └── widgets/
│   │   │       ├── device_card.dart
│   │   │       ├── photo_picker.dart
│   │   │       ├── qr_scanner.dart
│   │   │       └── custom_button.dart
│   │   │
│   │   └── routes/
│   │       └── app_routes.dart
│   │
│   ├── android/
│   │   └── (Android-specific files)
│   │
│   ├── ios/
│   │   └── (iOS-specific files)
│   │
│   └── test/
│       ├── unit/
│       ├── widget/
│       └── integration/
│
├── database/
│   ├── schema.sql
│   ├── migrations/
│   │   ├── 001_initial_schema.sql
│   │   ├── 002_add_marketplace.sql
│   │   └── 003_add_police_portal.sql
│   └── seeds/
│       ├── device_categories.sql
│       └── test_data.sql
│
├── infrastructure/
│   ├── azure/
│   │   ├── arm-templates/
│   │   │   ├── app-service.json
│   │   │   ├── database.json
│   │   │   ├── storage.json
│   │   │   └── keyvault.json
│   │   │
│   │   └── bicep/
│   │       └── main.bicep
│   │
│   ├── terraform/
│   │   ├── main.tf
│   │   ├── variables.tf
│   │   ├── outputs.tf
│   │   └── modules/
│   │       ├── app-service/
│   │       ├── database/
│   │       └── storage/
│   │
│   └── docker/
│       ├── Dockerfile.api
│       ├── Dockerfile.web
│       ├── docker-compose.yml
│       └── docker-compose.override.yml
│
├── .github/
│   ├── workflows/
│   │   ├── backend-ci.yml
│   │   ├── backend-cd.yml
│   │   ├── frontend-ci.yml
│   │   ├── frontend-cd.yml
│   │   └── mobile-ci.yml
│   │
│   └── PULL_REQUEST_TEMPLATE.md
│
└── scripts/
    ├── dev/
    │   ├── setup.sh
    │   ├── start-backend.sh
    │   ├── start-frontend.sh
    │   └── start-mobile.sh
    │
    ├── deployment/
    │   ├── deploy-staging.sh
    │   └── deploy-production.sh
    │
    └── database/
        ├── backup.sh
        └── restore.sh
```

---

## 🎯 Architecture Layers

### Backend (ASP.NET Core)

**Clean Architecture Pattern:**

1. **API Layer** (`DeviceOwnership.API`)
   - Controllers
   - Middleware
   - Filters
   - API configuration

2. **Application Layer** (`DeviceOwnership.Application`)
   - Business logic
   - DTOs
   - Validators
   - Services
   - Background jobs

3. **Core Layer** (`DeviceOwnership.Core`)
   - Domain entities
   - Interfaces
   - Domain exceptions
   - Value objects

4. **Infrastructure Layer** (`DeviceOwnership.Infrastructure`)
   - EF Core DbContext
   - Repositories
   - External services (Email, SMS, Storage)
   - OpenIddict configuration

---

### Frontend (Angular)

**Feature-Based Structure:**

1. **Core Module**
   - Singleton services
   - Guards
   - Interceptors
   - Models

2. **Features Modules**
   - Lazy-loaded feature modules
   - Smart components
   - Feature-specific services

3. **Shared Module**
   - Reusable components
   - Pipes
   - Directives

---

### Mobile (Flutter)

**BLoC Pattern:**

1. **Core**
   - Configuration
   - Services
   - Constants
   - Utils

2. **Data Layer**
   - Models
   - Repositories
   - Local database

3. **Presentation Layer**
   - BLoCs (Business Logic Components)
   - Screens
   - Widgets

---

## 🚀 Getting Started

### Backend Setup

```bash
cd backend
dotnet restore
dotnet ef database update -p src/DeviceOwnership.Infrastructure
dotnet run --project src/DeviceOwnership.API
```

### Frontend Setup

```bash
cd frontend-web
npm install
ng serve
```

### Mobile Setup

```bash
cd mobile-app
flutter pub get
flutter run
```

---

## 📦 Key Dependencies

### Backend
- ASP.NET Core 8.0
- Entity Framework Core 8.0
- OpenIddict 5.x
- FluentValidation
- AutoMapper
- Serilog
- Azure.Storage.Blobs
- Azure.Security.KeyVault
- Stripe.net
- SendGrid
- Twilio

### Frontend
- Angular 17
- RxJS
- Tailwind CSS
- angular-oauth2-oidc
- ngx-translate

### Mobile
- Flutter 3.x
- flutter_bloc
- dio (HTTP client)
- hive (local storage)
- flutter_secure_storage
- camera
- qr_code_scanner
- firebase_messaging

---

This structure follows industry best practices and is designed for scalability and maintainability.
