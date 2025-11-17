# API Testing Guide

## Quick Start

### 1. Start the Backend API

```bash
cd backend
ASPNETCORE_ENVIRONMENT=Development dotnet run --project src/DeviceOwnership.API/DeviceOwnership.API.csproj
```

The API will be available at: **http://localhost:5000**
Swagger UI: **http://localhost:5000/swagger**

### 2. Test with Swagger UI

1. Open http://localhost:5000/swagger in your browser
2. Test the authentication flow:

#### Register a User
```
POST /api/v1/auth/register
{
  "email": "test@example.com",
  "password": "Password123!",
  "firstName": "Test",
  "lastName": "User"
}
```

#### Login
```
POST /api/v1/auth/login
{
  "email": "test@example.com",
  "password": "Password123!"
}
```

Copy the `accessToken` from the response.

#### Authorize in Swagger
1. Click the "Authorize" button at the top right
2. Enter: `Bearer YOUR_ACCESS_TOKEN`
3. Click "Authorize"

#### Test Protected Endpoints
```
POST /api/v1/devices
{
  "serialNumber": "SN123456789",
  "category": "Smartphone",
  "brand": "Apple",
  "model": "iPhone 15 Pro"
}
```

### 3. Test with Angular Web App

```bash
cd frontend/web
npm install
ng serve
```

Open http://localhost:4200

**Test Flow:**
1. Click "Register" and create an account
2. Login with your credentials
3. Navigate to "My Devices" and register a device
4. Browse the Marketplace
5. Check a serial number

### 4. Test with Flutter Mobile App

```bash
cd frontend/mobile
flutter pub get
flutter run
```

**Test Flow:**
1. Register/Login
2. View Dashboard
3. Register a device
4. View device list

## API Endpoints Reference

### Authentication
- `POST /api/v1/auth/register` - Create new account
- `POST /api/v1/auth/login` - Login and get JWT token
- `POST /api/v1/auth/refresh` - Refresh access token
- `GET /api/v1/auth/me` - Get current user info

### Devices
- `POST /api/v1/devices` - Register a device
- `GET /api/v1/devices` - Get user's devices
- `GET /api/v1/devices/{id}` - Get device details
- `GET /api/v1/devices/check/{serialNumber}` - Check if stolen
- `DELETE /api/v1/devices/{id}` - Delete device

### Marketplace
- `GET /api/v1/marketplace` - Browse listings (supports filters)
- `GET /api/v1/marketplace/{id}` - Get listing details
- `GET /api/v1/marketplace/my-listings` - Get your listings
- `POST /api/v1/marketplace` - Create listing
- `PUT /api/v1/marketplace/{id}` - Update listing
- `DELETE /api/v1/marketplace/{id}` - Delete listing

### Theft Reports
- `GET /api/v1/reports` - Get your reports
- `GET /api/v1/reports/{id}` - Get report details
- `POST /api/v1/reports` - Create theft report

### File Uploads
- `POST /api/v1/files/devices/{deviceId}/photos` - Upload photo
- `POST /api/v1/files/devices/{deviceId}/documents` - Upload document

## Sample API Calls with cURL

### Register User
```bash
curl -X POST http://localhost:5000/api/v1/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@test.com",
    "password": "Test123!",
    "firstName": "John",
    "lastName": "Doe"
  }'
```

### Login
```bash
curl -X POST http://localhost:5000/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@test.com",
    "password": "Test123!"
  }'
```

### Register Device (with token)
```bash
curl -X POST http://localhost:5000/api/v1/devices \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -d '{
    "serialNumber": "ABC123XYZ",
    "category": "Laptop",
    "brand": "Dell",
    "model": "XPS 15"
  }'
```

### Browse Marketplace
```bash
curl http://localhost:5000/api/v1/marketplace?category=Smartphone&minPrice=100&maxPrice=500
```

## Database Setup (PostgreSQL)

If you haven't set up PostgreSQL yet:

### Option 1: Docker (Easiest)
```bash
docker run --name deviceownership-db \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=DeviceOwnership \
  -p 5432:5432 \
  -d postgres:15
```

### Option 2: Local PostgreSQL
1. Install PostgreSQL
2. Create database:
```sql
CREATE DATABASE DeviceOwnership;
```

The API will automatically run migrations on startup in Development mode.

## Testing Scenarios

### Scenario 1: Complete User Journey
1. Register account → Login → Get token
2. Register 3 devices (Free tier limit)
3. Try to register 4th device (should fail)
4. Create marketplace listing for one device
5. Browse marketplace and view your listing
6. Report one device as stolen
7. Check serial number (should show as stolen)

### Scenario 2: Marketplace Flow
1. User A: Register and create device
2. User A: Create marketplace listing
3. User B: Register and browse marketplace
4. User B: View listing (view count increases)
5. User B: Filter by category/price

### Scenario 3: File Upload
1. Register device
2. Upload device photo (multipart/form-data)
3. Upload purchase receipt document

## Common Issues

### CORS Errors
- The API has CORS enabled with "AllowAll" policy
- Angular: runs on localhost:4200 ✓
- Flutter: uses localhost ✓

### Database Connection
- Update connection string in `appsettings.Development.json`
- Default: `Host=localhost;Database=DeviceOwnership;Username=postgres;Password=postgres`

### JWT Token Expiration
- Access tokens expire after 60 minutes
- Use the refresh token endpoint to get new tokens

## Response Formats

### Success Response (Login)
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "random-base64-string",
  "expiresIn": 3600,
  "tokenType": "Bearer",
  "user": {
    "id": "guid",
    "email": "user@test.com",
    "firstName": "John",
    "lastName": "Doe",
    "role": "User",
    "subscriptionTier": "Free",
    "isEmailVerified": false,
    "isPhoneVerified": false
  }
}
```

### Error Response
```json
{
  "message": "Error description"
}
```

## Frontend Integration Status

✅ **Angular Web App**
- Environment configured: `http://localhost:5000/api/v1`
- All services implemented
- HTTP interceptor for JWT tokens
- Auth guard for protected routes

✅ **Flutter Mobile App**
- API config: `http://localhost:5000/api/v1`
- Dio HTTP client with interceptors
- Secure token storage
- Provider state management

## Next Steps

1. ✅ Backend API is fully implemented
2. ✅ Frontend applications are connected
3. 🔄 Set up PostgreSQL database
4. 🔄 Test complete user flows
5. 🔄 Deploy to Azure (when ready)
