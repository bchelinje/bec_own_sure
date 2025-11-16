# 🌐 API Endpoints Specification

## Base URL

**Production:** `https://api.deviceownership.com`
**Staging:** `https://staging-api.deviceownership.com`
**Development:** `http://localhost:5000`

---

## 🔐 Authentication

All endpoints (except public ones) require an `Authorization` header:

```
Authorization: Bearer <access_token>
```

### OAuth 2.0 Endpoints

#### POST /connect/authorize
Authorization endpoint for OAuth 2.0 flow

**Query Parameters:**
- `response_type` (required): `code`
- `client_id` (required): Application client ID
- `redirect_uri` (required): Callback URL
- `scope` (required): Space-separated scopes
- `code_challenge` (required): PKCE challenge
- `code_challenge_method` (required): `S256`
- `state` (optional): Random state for CSRF protection

#### POST /connect/token
Token endpoint for exchanging authorization code

**Request Body:**
```json
{
  "grant_type": "authorization_code",
  "code": "auth_code_here",
  "redirect_uri": "app://callback",
  "code_verifier": "pkce_verifier",
  "client_id": "mobile_app"
}
```

**Response:**
```json
{
  "access_token": "eyJ...",
  "refresh_token": "xyz...",
  "id_token": "eyJ...",
  "token_type": "Bearer",
  "expires_in": 900,
  "scope": "openid profile email device"
}
```

#### POST /connect/token (Refresh)

**Request Body:**
```json
{
  "grant_type": "refresh_token",
  "refresh_token": "xyz...",
  "client_id": "mobile_app"
}
```

#### POST /connect/logout
Logout endpoint

**Request Body:**
```json
{
  "id_token_hint": "eyJ...",
  "post_logout_redirect_uri": "app://logout"
}
```

---

## 👤 User Management

### POST /api/v1/users/register
Register a new user account

**Scope:** None (public)

**Request:**
```json
{
  "email": "user@example.com",
  "password": "SecureP@ssw0rd",
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+447700900000"
}
```

**Response:** `201 Created`
```json
{
  "id": "uuid",
  "email": "user@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "emailVerified": false,
  "subscriptionTier": "free",
  "createdAt": "2024-01-01T12:00:00Z"
}
```

**Errors:**
- `400 Bad Request` - Validation failed
- `409 Conflict` - Email already registered

---

### POST /api/v1/users/verify-email
Verify user's email address

**Scope:** None (public)

**Request:**
```json
{
  "token": "verification_token_from_email"
}
```

**Response:** `200 OK`
```json
{
  "message": "Email verified successfully",
  "emailVerified": true
}
```

---

### GET /api/v1/users/me
Get current user profile

**Scope:** `profile`

**Response:** `200 OK`
```json
{
  "id": "uuid",
  "email": "user@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+447700900000",
  "emailVerified": true,
  "phoneVerified": false,
  "profilePictureUrl": "https://...",
  "subscriptionTier": "premium",
  "role": "user",
  "createdAt": "2024-01-01T12:00:00Z"
}
```

---

### PUT /api/v1/users/me
Update current user profile

**Scope:** `profile`

**Request:**
```json
{
  "firstName": "John",
  "lastName": "Smith",
  "phoneNumber": "+447700900001"
}
```

**Response:** `200 OK`

---

### POST /api/v1/users/change-password
Change user password

**Scope:** `profile`

**Request:**
```json
{
  "currentPassword": "OldP@ssw0rd",
  "newPassword": "NewP@ssw0rd"
}
```

**Response:** `200 OK`

---

## 📱 Device Management

### POST /api/v1/devices
Register a new device

**Scope:** `device.register`

**Request:**
```json
{
  "serialNumber": "ABC123456789",
  "category": "smartphone",
  "brand": "Apple",
  "model": "iPhone 15 Pro",
  "description": "Space Black, 256GB",
  "color": "Space Black",
  "purchaseDate": "2024-01-01",
  "purchasePrice": 999.00,
  "purchaseCurrency": "GBP"
}
```

**Response:** `201 Created`
```json
{
  "id": "uuid",
  "userId": "uuid",
  "category": "smartphone",
  "brand": "Apple",
  "model": "iPhone 15 Pro",
  "verificationCode": "ABC123",
  "qrCodeUrl": "https://api.deviceownership.com/verify/ABC123",
  "registeredAt": "2024-01-01T12:00:00Z"
}
```

**Errors:**
- `400 Bad Request` - Validation failed or device limit reached
- `409 Conflict` - Serial number already registered

---

### GET /api/v1/devices
Get all user's devices

**Scope:** `device.read`

**Query Parameters:**
- `page` (default: 1)
- `pageSize` (default: 20, max: 100)
- `category` (optional)
- `status` (optional): active, stolen, lost, recovered
- `sortBy` (optional): registeredAt, brand, category
- `sortOrder` (optional): asc, desc

**Response:** `200 OK`
```json
{
  "data": [
    {
      "id": "uuid",
      "category": "smartphone",
      "brand": "Apple",
      "model": "iPhone 15 Pro",
      "status": "active",
      "isStolen": false,
      "isLost": false,
      "registeredAt": "2024-01-01T12:00:00Z",
      "primaryPhoto": "https://..."
    }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 20,
    "totalPages": 1,
    "totalItems": 5
  }
}
```

---

### GET /api/v1/devices/{id}
Get device details

**Scope:** `device.read`

**Response:** `200 OK`
```json
{
  "id": "uuid",
  "category": "smartphone",
  "brand": "Apple",
  "model": "iPhone 15 Pro",
  "description": "Space Black, 256GB",
  "serialNumber": "ABC123456789",
  "color": "Space Black",
  "purchaseDate": "2024-01-01",
  "purchasePrice": 999.00,
  "status": "active",
  "isStolen": false,
  "isLost": false,
  "verificationCode": "ABC123",
  "qrCodeUrl": "https://...",
  "photos": [
    {
      "id": "uuid",
      "photoUrl": "https://...",
      "photoType": "device",
      "isPrimary": true
    }
  ],
  "documents": [
    {
      "id": "uuid",
      "documentType": "receipt",
      "documentUrl": "https://...",
      "fileName": "receipt.pdf"
    }
  ],
  "registeredAt": "2024-01-01T12:00:00Z"
}
```

---

### PUT /api/v1/devices/{id}
Update device information

**Scope:** `device.update`

**Request:**
```json
{
  "description": "Updated description",
  "color": "Space Black"
}
```

**Response:** `200 OK`

---

### DELETE /api/v1/devices/{id}
Delete device (soft delete)

**Scope:** `device.delete`

**Response:** `204 No Content`

---

### POST /api/v1/devices/{id}/photos
Upload device photo

**Scope:** `device.update`

**Request:** `multipart/form-data`
- `file`: Image file (max 10MB)
- `photoType`: device | serial_number | receipt | damage
- `isPrimary`: boolean

**Response:** `201 Created`
```json
{
  "id": "uuid",
  "photoUrl": "https://...",
  "photoType": "device",
  "isPrimary": true
}
```

---

### POST /api/v1/devices/{id}/documents
Upload device document

**Scope:** `device.update`

**Request:** `multipart/form-data`
- `file`: PDF/DOC file (max 10MB)
- `documentType`: receipt | warranty | manual | insurance

**Response:** `201 Created`

---

### GET /api/v1/devices/check/{serialNumber}
Check if device is stolen/lost (Public)

**Scope:** None (public endpoint)

**Response:** `200 OK`
```json
{
  "serialNumberHash": "hash",
  "status": "stolen",
  "isStolen": true,
  "isLost": false,
  "reportedAt": "2024-01-15T10:00:00Z",
  "message": "This device has been reported as stolen. Contact authorities.",
  "ownerCanBeContacted": true
}
```

**Possible statuses:**
- `safe` - Not reported, safe to buy
- `stolen` - Reported as stolen
- `lost` - Reported as lost
- `registered` - Registered to someone else (verify ownership transfer)

---

## 🔄 Ownership Transfer

### POST /api/v1/transfers
Initiate ownership transfer

**Scope:** `transfer`

**Request:**
```json
{
  "deviceId": "uuid",
  "toUserEmail": "buyer@example.com",
  "message": "Selling this device to you"
}
```

**Response:** `201 Created`
```json
{
  "id": "uuid",
  "deviceId": "uuid",
  "fromUserId": "uuid",
  "toUserId": "uuid",
  "status": "pending",
  "transferCode": "XYZ789",
  "expiresAt": "2024-01-08T12:00:00Z",
  "initiatedAt": "2024-01-01T12:00:00Z"
}
```

---

### GET /api/v1/transfers
Get pending transfers

**Scope:** `transfer`

**Query Parameters:**
- `type`: incoming | outgoing | all

**Response:** `200 OK`
```json
{
  "data": [
    {
      "id": "uuid",
      "device": {
        "id": "uuid",
        "brand": "Apple",
        "model": "iPhone 15 Pro"
      },
      "fromUser": {
        "email": "seller@example.com",
        "firstName": "John"
      },
      "toUser": {
        "email": "buyer@example.com",
        "firstName": "Jane"
      },
      "status": "pending",
      "transferCode": "XYZ789",
      "expiresAt": "2024-01-08T12:00:00Z"
    }
  ]
}
```

---

### POST /api/v1/transfers/{id}/accept
Accept ownership transfer

**Scope:** `transfer`

**Request:**
```json
{
  "transferCode": "XYZ789"
}
```

**Response:** `200 OK`
```json
{
  "message": "Ownership transferred successfully",
  "deviceId": "uuid",
  "newOwnerId": "uuid"
}
```

---

### POST /api/v1/transfers/{id}/reject
Reject ownership transfer

**Scope:** `transfer`

**Request:**
```json
{
  "reason": "Changed my mind"
}
```

**Response:** `200 OK`

---

### GET /api/v1/devices/{id}/ownership-history
Get device ownership history

**Scope:** `device.read`

**Response:** `200 OK`
```json
{
  "deviceId": "uuid",
  "history": [
    {
      "id": "uuid",
      "fromUser": null,
      "toUser": {
        "email": "first@owner.com",
        "firstName": "John"
      },
      "transferMethod": "registered",
      "transferredAt": "2024-01-01T12:00:00Z"
    },
    {
      "id": "uuid",
      "fromUser": {
        "email": "first@owner.com",
        "firstName": "John"
      },
      "toUser": {
        "email": "second@owner.com",
        "firstName": "Jane"
      },
      "transferMethod": "transferred",
      "transferredAt": "2024-02-01T12:00:00Z"
    }
  ]
}
```

---

## 🚨 Theft & Loss Reporting

### POST /api/v1/reports
Report device as stolen or lost

**Scope:** `report`

**Request:**
```json
{
  "deviceId": "uuid",
  "reportType": "stolen",
  "title": "iPhone stolen from car",
  "description": "My iPhone was stolen from my car on January 15th",
  "incidentDate": "2024-01-15T18:00:00Z",
  "incidentLocation": "123 Main St, London",
  "incidentCity": "London",
  "incidentCountry": "UK",
  "incidentLatitude": 51.5074,
  "incidentLongitude": -0.1278,
  "policeReferenceNumber": "CR12345",
  "policeStation": "Metropolitan Police",
  "isPublic": true,
  "rewardAmount": 100.00
}
```

**Response:** `201 Created`
```json
{
  "id": "uuid",
  "deviceId": "uuid",
  "reportType": "stolen",
  "status": "active",
  "reportedAt": "2024-01-15T19:00:00Z",
  "policeNotified": true,
  "communityAlerted": true
}
```

---

### GET /api/v1/reports
Get user's theft reports

**Scope:** `report`

**Response:** `200 OK`

---

### GET /api/v1/reports/{id}
Get theft report details

**Scope:** `report`

**Response:** `200 OK`

---

### PUT /api/v1/reports/{id}
Update theft report

**Scope:** `report`

**Request:**
```json
{
  "status": "recovered",
  "additionalNotes": "Device was recovered by police"
}
```

**Response:** `200 OK`

---

### GET /api/v1/reports/nearby
Get nearby theft reports (Public)

**Scope:** None (public)

**Query Parameters:**
- `latitude` (required)
- `longitude` (required)
- `radiusKm` (default: 10, max: 50)
- `reportType` (optional): stolen | lost

**Response:** `200 OK`
```json
{
  "data": [
    {
      "id": "uuid",
      "reportType": "stolen",
      "device": {
        "category": "smartphone",
        "brand": "Apple",
        "model": "iPhone 15 Pro"
      },
      "incidentDate": "2024-01-15T18:00:00Z",
      "incidentLocation": "123 Main St, London",
      "distanceKm": 2.5
    }
  ]
}
```

---

## 👮 Police Portal

### GET /api/v1/police/devices/search
Search all devices (Police only)

**Scope:** `police.search`
**Required Role:** `police`

**Query Parameters:**
- `serialNumber` (optional)
- `brand` (optional)
- `model` (optional)
- `isStolen` (optional): true | false

**Response:** `200 OK`

---

### GET /api/v1/police/reports
Get all theft reports (Police only)

**Scope:** `police.reports`
**Required Role:** `police`

**Query Parameters:**
- `status`: active | investigating | recovered | closed
- `city` (optional)
- `dateFrom` (optional)
- `dateTo` (optional)

**Response:** `200 OK`

---

### POST /api/v1/police/reports/{id}/notes
Add police notes to theft report

**Scope:** `police.reports`
**Required Role:** `police`

**Request:**
```json
{
  "caseNumber": "CR12345",
  "notes": "Investigation ongoing",
  "recoveryStatus": "under_investigation",
  "officerBadgeNumber": "12345",
  "officerName": "Officer Smith",
  "policeStation": "Metropolitan Police"
}
```

**Response:** `201 Created`

---

## 🛒 Marketplace

### POST /api/v1/marketplace/listings
Create marketplace listing

**Scope:** `marketplace.list`

**Request:**
```json
{
  "deviceId": "uuid",
  "title": "iPhone 15 Pro - Excellent Condition",
  "description": "Selling my iPhone 15 Pro in excellent condition...",
  "price": 899.00,
  "currency": "GBP",
  "condition": "like_new",
  "location": "London, UK",
  "isShippingAvailable": true
}
```

**Response:** `201 Created`

---

### GET /api/v1/marketplace/listings
Browse marketplace listings (Public)

**Scope:** None (public)

**Query Parameters:**
- `category` (optional)
- `minPrice` (optional)
- `maxPrice` (optional)
- `condition` (optional)
- `location` (optional)
- `search` (optional): full-text search
- `page` (default: 1)
- `pageSize` (default: 20)

**Response:** `200 OK`
```json
{
  "data": [
    {
      "id": "uuid",
      "title": "iPhone 15 Pro - Excellent Condition",
      "price": 899.00,
      "currency": "GBP",
      "condition": "like_new",
      "location": "London, UK",
      "seller": {
        "firstName": "John",
        "verified": true,
        "rating": 4.8
      },
      "device": {
        "category": "smartphone",
        "brand": "Apple",
        "model": "iPhone 15 Pro",
        "verifiedOwner": true
      },
      "primaryPhoto": "https://...",
      "listedAt": "2024-01-01T12:00:00Z"
    }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 20,
    "totalPages": 10,
    "totalItems": 200
  }
}
```

---

### GET /api/v1/marketplace/listings/{id}
Get listing details (Public)

**Scope:** None (public)

**Response:** `200 OK`

---

### POST /api/v1/marketplace/listings/{id}/purchase
Purchase a device from marketplace

**Scope:** `marketplace.buy`

**Request:**
```json
{
  "shippingAddress": {
    "line1": "123 Main St",
    "city": "London",
    "postCode": "SW1A 1AA",
    "country": "UK"
  },
  "paymentMethodId": "pm_xxx"
}
```

**Response:** `200 OK`

---

## 💳 Subscriptions & Payments

### GET /api/v1/subscriptions/plans
Get available subscription plans (Public)

**Scope:** None (public)

**Response:** `200 OK`
```json
{
  "plans": [
    {
      "id": "free",
      "name": "Free",
      "price": 0,
      "interval": null,
      "features": {
        "deviceLimit": 3,
        "marketplace": true,
        "support": "community"
      }
    },
    {
      "id": "premium_monthly",
      "name": "Premium Monthly",
      "price": 1.99,
      "currency": "GBP",
      "interval": "monthly",
      "stripePriceId": "price_xxx",
      "features": {
        "deviceLimit": null,
        "marketplace": true,
        "support": "priority",
        "certificates": true
      }
    }
  ]
}
```

---

### POST /api/v1/subscriptions/subscribe
Subscribe to a plan

**Scope:** `profile`

**Request:**
```json
{
  "planId": "premium_monthly",
  "paymentMethodId": "pm_xxx"
}
```

**Response:** `201 Created`

---

### POST /api/v1/subscriptions/cancel
Cancel subscription

**Scope:** `profile`

**Response:** `200 OK`

---

### GET /api/v1/subscriptions/current
Get current subscription

**Scope:** `profile`

**Response:** `200 OK`

---

### GET /api/v1/payments/history
Get payment history

**Scope:** `profile`

**Response:** `200 OK`

---

## 📊 Admin Portal

### GET /api/v1/admin/users
Get all users (Admin only)

**Scope:** `admin.users`
**Required Role:** `admin`

**Response:** `200 OK`

---

### PUT /api/v1/admin/users/{id}/role
Update user role (Admin only)

**Scope:** `admin.users`
**Required Role:** `admin`

**Request:**
```json
{
  "role": "police"
}
```

**Response:** `200 OK`

---

### GET /api/v1/admin/police/pending
Get pending police account verifications

**Scope:** `admin`
**Required Role:** `admin`

**Response:** `200 OK`

---

### POST /api/v1/admin/police/{id}/verify
Verify police account

**Scope:** `admin`
**Required Role:** `admin`

**Request:**
```json
{
  "approved": true,
  "notes": "Verified with police station"
}
```

**Response:** `200 OK`

---

### GET /api/v1/admin/analytics
Get system analytics

**Scope:** `admin`
**Required Role:** `admin`

**Response:** `200 OK`
```json
{
  "totalUsers": 10000,
  "totalDevices": 25000,
  "totalTheftReports": 150,
  "activeListings": 500,
  "revenue": {
    "monthly": 5000,
    "yearly": 50000
  }
}
```

---

## 🔔 Notifications

### GET /api/v1/notifications
Get user notifications

**Scope:** `profile`

**Query Parameters:**
- `unreadOnly`: true | false
- `page` (default: 1)
- `pageSize` (default: 20)

**Response:** `200 OK`

---

### PUT /api/v1/notifications/{id}/read
Mark notification as read

**Scope:** `profile`

**Response:** `200 OK`

---

### PUT /api/v1/notifications/read-all
Mark all notifications as read

**Scope:** `profile`

**Response:** `200 OK`

---

## 📈 Rate Limiting

All endpoints are rate-limited:

**Authenticated requests:** 100 requests/minute
**Unauthenticated requests:** 20 requests/minute
**Token endpoint:** 10 requests/minute per IP
**Serial check endpoint:** 20 requests/minute per IP

**Response Headers:**
```
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 95
X-RateLimit-Reset: 1640000000
```

**Error Response:** `429 Too Many Requests`
```json
{
  "error": "rate_limit_exceeded",
  "message": "Too many requests. Please try again later.",
  "retryAfter": 60
}
```

---

## ❌ Error Responses

### Standard Error Format

```json
{
  "error": {
    "code": "validation_error",
    "message": "One or more validation errors occurred",
    "details": [
      {
        "field": "email",
        "message": "Invalid email format"
      }
    ],
    "traceId": "00-xxxxx-xxxxx-00"
  }
}
```

### Common Error Codes

- `validation_error` (400) - Request validation failed
- `unauthorized` (401) - Authentication required
- `forbidden` (403) - Insufficient permissions
- `not_found` (404) - Resource not found
- `conflict` (409) - Resource already exists
- `rate_limit_exceeded` (429) - Rate limit exceeded
- `internal_server_error` (500) - Server error

---

## 🧪 Testing

### Swagger UI
Available at: `/swagger`

### Postman Collection
Download from: `/api/postman-collection.json`

---

This API follows RESTful principles and uses standard HTTP methods and status codes.
