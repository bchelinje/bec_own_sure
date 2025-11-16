# 🔐 Authentication & Authorization Flows

## Overview

This document describes all authentication and authorization flows for the Device Ownership & Anti-Theft Platform using OpenIddict (OAuth 2.0 / OpenID Connect).

---

## 🎯 OAuth 2.0 Flows Used

### 1. Authorization Code Flow with PKCE (Mobile & Web Apps)
**Used by:** Flutter mobile app, Angular web app

**Why:** Most secure for public clients (mobile/SPA), prevents authorization code interception

### 2. Client Credentials Flow
**Used by:** Business API integrations, Police systems (future)

**Why:** Server-to-server authentication without user interaction

---

## 📱 Flow 1: Mobile App Authentication (Flutter)

### Initial Registration & Login

```
┌────────────┐                                  ┌────────────┐                 ┌────────────┐
│   Flutter  │                                  │    API     │                 │  Database  │
│   Mobile   │                                  │ (OpenIddict)│                 │            │
└─────┬──────┘                                  └─────┬──────┘                 └─────┬──────┘
      │                                               │                              │
      │ 1. User taps "Sign Up"                       │                              │
      │                                               │                              │
      │ 2. Generate PKCE code_verifier               │                              │
      │    code_challenge = SHA256(code_verifier)    │                              │
      │                                               │                              │
      │ 3. Open browser with:                         │                              │
      │    /connect/authorize?                        │                              │
      │    response_type=code&                        │                              │
      │    client_id=mobile_app&                      │                              │
      │    redirect_uri=myapp://callback&             │                              │
      │    scope=openid profile email device&         │                              │
      │    code_challenge=<challenge>&                │                              │
      │    code_challenge_method=S256                 │                              │
      ├──────────────────────────────────────────────>│                              │
      │                                               │                              │
      │                                               │ 4. Show login/register form  │
      │                                               │                              │
      │ 5. User enters email/password                 │                              │
      ├──────────────────────────────────────────────>│                              │
      │                                               │                              │
      │                                               │ 6. Validate credentials      │
      │                                               ├─────────────────────────────>│
      │                                               │                              │
      │                                               │ 7. Create user account       │
      │                                               │    Hash password (BCrypt)    │
      │                                               ├─────────────────────────────>│
      │                                               │<─────────────────────────────┤
      │                                               │                              │
      │                                               │ 8. Send verification email   │
      │                                               │                              │
      │ 9. Redirect to:                               │                              │
      │    myapp://callback?code=<auth_code>          │                              │
      │<──────────────────────────────────────────────┤                              │
      │                                               │                              │
      │ 10. Exchange code for tokens:                 │                              │
      │     POST /connect/token                       │                              │
      │     grant_type=authorization_code&            │                              │
      │     code=<auth_code>&                         │                              │
      │     redirect_uri=myapp://callback&            │                              │
      │     code_verifier=<original_verifier>&        │                              │
      │     client_id=mobile_app                      │                              │
      ├──────────────────────────────────────────────>│                              │
      │                                               │                              │
      │                                               │ 11. Verify code_verifier     │
      │                                               │     SHA256(verifier) ==      │
      │                                               │     stored challenge?        │
      │                                               │                              │
      │ 12. Return tokens:                            │                              │
      │     {                                         │                              │
      │       "access_token": "eyJ...",               │                              │
      │       "refresh_token": "xyz...",              │                              │
      │       "id_token": "eyJ...",                   │                              │
      │       "expires_in": 900,                      │                              │
      │       "token_type": "Bearer"                  │                              │
      │     }                                         │                              │
      │<──────────────────────────────────────────────┤                              │
      │                                               │                              │
      │ 13. Store tokens in secure storage            │                              │
      │     (flutter_secure_storage)                  │                              │
      │                                               │                              │
      │ 14. Decode id_token to get user info:         │                              │
      │     {                                         │                              │
      │       "sub": "user_id",                       │                              │
      │       "email": "user@example.com",            │                              │
      │       "email_verified": false,                │                              │
      │       "roles": ["user"]                       │                              │
      │     }                                         │                              │
      │                                               │                              │
```

### Email Verification Flow

```
User                   API                 Email Service
  │                     │                       │
  │ 1. User clicks      │                       │
  │    verification     │                       │
  │    link in email    │                       │
  ├────────────────────>│                       │
  │                     │                       │
  │                     │ 2. Validate token     │
  │                     │                       │
  │                     │ 3. Mark email         │
  │                     │    as verified        │
  │                     │                       │
  │ 4. Redirect to app  │                       │
  │    with success     │                       │
  │<────────────────────┤                       │
  │                     │                       │
  │ 5. Refresh tokens   │                       │
  │    to get updated   │                       │
  │    email_verified   │                       │
  │    claim            │                       │
  ├────────────────────>│                       │
  │                     │                       │
  │ 6. New tokens with  │                       │
  │    email_verified:  │                       │
  │    true             │                       │
  │<────────────────────┤                       │
  │                     │                       │
```

---

## 🌐 Flow 2: Web App Authentication (Angular)

### Login Flow

```
┌────────────┐                                  ┌────────────┐
│  Angular   │                                  │    API     │
│   Web App  │                                  │ (OpenIddict)│
└─────┬──────┘                                  └─────┬──────┘
      │                                               │
      │ 1. User clicks "Login"                        │
      │                                               │
      │ 2. Generate PKCE code_verifier & challenge   │
      │                                               │
      │ 3. Redirect to:                               │
      │    /connect/authorize?                        │
      │    response_type=code&                        │
      │    client_id=web_app&                         │
      │    redirect_uri=https://app.com/callback&     │
      │    scope=openid profile email device&         │
      │    code_challenge=<challenge>&                │
      │    code_challenge_method=S256&                │
      │    state=<random_state>                       │
      ├──────────────────────────────────────────────>│
      │                                               │
      │                                               │ 4. Show login page
      │                                               │
      │ 5. User enters credentials                    │
      ├──────────────────────────────────────────────>│
      │                                               │
      │                                               │ 6. Validate credentials
      │                                               │
      │ 7. Redirect back:                             │
      │    https://app.com/callback?                  │
      │    code=<auth_code>&                          │
      │    state=<original_state>                     │
      │<──────────────────────────────────────────────┤
      │                                               │
      │ 8. Verify state matches                       │
      │                                               │
      │ 9. Exchange code for tokens (background)      │
      │    POST /connect/token                        │
      ├──────────────────────────────────────────────>│
      │                                               │
      │ 10. Return tokens                             │
      │<──────────────────────────────────────────────┤
      │                                               │
      │ 11. Store in sessionStorage/localStorage      │
      │     (with HttpOnly cookie for refresh token)  │
      │                                               │
```

---

## 👮 Flow 3: Police Portal Authentication

### Police Officer Login (Email/Password + 2FA)

```
Police Officer      API (Auth)         Database        Admin
      │                │                   │              │
      │ 1. Login with  │                   │              │
      │    email/pwd   │                   │              │
      ├───────────────>│                   │              │
      │                │                   │              │
      │                │ 2. Verify         │              │
      │                │    credentials    │              │
      │                ├──────────────────>│              │
      │                │<──────────────────┤              │
      │                │                   │              │
      │                │ 3. Check account  │              │
      │                │    status:        │              │
      │                │    - Active?      │              │
      │                │    - Verified by  │              │
      │                │      admin?       │              │
      │                ├──────────────────>│              │
      │                │                   │              │
      │ 4. Send 2FA    │                   │              │
      │    code via    │                   │              │
      │    email       │                   │              │
      │<───────────────┤                   │              │
      │                │                   │              │
      │ 5. Enter 2FA   │                   │              │
      │    code        │                   │              │
      ├───────────────>│                   │              │
      │                │                   │              │
      │                │ 6. Verify code    │              │
      │                │                   │              │
      │ 7. Issue tokens│                   │              │
      │    with "police"│                  │              │
      │    role        │                   │              │
      │<───────────────┤                   │              │
      │                │                   │              │
      │                │ 8. Log access     │              │
      │                │    (audit)        │              │
      │                ├──────────────────>│              │
      │                │                   │              │
```

### Police Account Approval Process

```
Police Officer      API               Admin           Database
      │              │                  │                │
      │ 1. Submit    │                  │                │
      │    registration│                │                │
      │    with badge │                 │                │
      │    number,    │                 │                │
      │    station,   │                 │                │
      │    ID docs    │                 │                │
      ├──────────────>│                 │                │
      │              │                  │                │
      │              │ 2. Create pending│                │
      │              │    account       │                │
      │              ├─────────────────────────────────>│
      │              │                  │                │
      │              │ 3. Notify admin  │                │
      │              ├─────────────────>│                │
      │              │                  │                │
      │              │                  │ 4. Review:     │
      │              │                  │    - Verify    │
      │              │                  │      badge     │
      │              │                  │    - Call      │
      │              │                  │      station   │
      │              │                  │    - Check docs│
      │              │                  │                │
      │              │ 5. Approve/Reject│                │
      │              │<─────────────────┤                │
      │              │                  │                │
      │              │ 6. Update account│                │
      │              │    status        │                │
      │              ├─────────────────────────────────>│
      │              │                  │                │
      │ 7. Send      │                  │                │
      │    notification│                │                │
      │<──────────────┤                 │                │
      │              │                  │                │
```

---

## 🏢 Flow 4: Business Account Authentication

### Business API Integration (Client Credentials)

```
Business System      API (OpenIddict)      Database
      │                    │                   │
      │ 1. Request token:  │                   │
      │    POST /connect/token                 │
      │    grant_type=client_credentials&      │
      │    client_id=business_123&             │
      │    client_secret=<secret>&             │
      │    scope=device.register device.read   │
      ├───────────────────>│                   │
      │                    │                   │
      │                    │ 2. Validate       │
      │                    │    client_id and  │
      │                    │    client_secret  │
      │                    ├──────────────────>│
      │                    │<──────────────────┤
      │                    │                   │
      │                    │ 3. Check scopes   │
      │                    │    allowed for    │
      │                    │    this client    │
      │                    │                   │
      │ 4. Return token:   │                   │
      │    {               │                   │
      │      "access_token": "eyJ...",         │
      │      "expires_in": 3600,               │
      │      "token_type": "Bearer",           │
      │      "scope": "device.register ..."    │
      │    }               │                   │
      │<───────────────────┤                   │
      │                    │                   │
      │ 5. Use token for   │                   │
      │    API calls:      │                   │
      │    POST /api/v1/devices                │
      │    Authorization: Bearer eyJ...        │
      ├───────────────────>│                   │
      │                    │                   │
      │                    │ 6. Validate token │
      │                    │    & scopes       │
      │                    │                   │
      │ 7. Response        │                   │
      │<───────────────────┤                   │
      │                    │                   │
```

---

## 🔄 Flow 5: Token Refresh

### Refreshing Access Token (All Clients)

```
Client App          API (OpenIddict)      Database
    │                     │                   │
    │ 1. Access token     │                   │
    │    expires          │                   │
    │    (900 seconds)    │                   │
    │                     │                   │
    │ 2. POST /connect/token                  │
    │    grant_type=refresh_token&            │
    │    refresh_token=<token>&               │
    │    client_id=mobile_app                 │
    ├────────────────────>│                   │
    │                     │                   │
    │                     │ 3. Validate       │
    │                     │    refresh token  │
    │                     ├──────────────────>│
    │                     │                   │
    │                     │ 4. Check if       │
    │                     │    revoked or     │
    │                     │    expired        │
    │                     │<──────────────────┤
    │                     │                   │
    │                     │ 5. Generate new   │
    │                     │    access token   │
    │                     │                   │
    │                     │ 6. Rotate refresh │
    │                     │    token (new one)│
    │                     ├──────────────────>│
    │                     │                   │
    │ 7. Return new       │                   │
    │    tokens:          │                   │
    │    {                │                   │
    │      "access_token": "new_eyJ...",      │
    │      "refresh_token": "new_xyz...",     │
    │      "expires_in": 900                  │
    │    }                │                   │
    │<────────────────────┤                   │
    │                     │                   │
    │ 8. Update stored    │                   │
    │    tokens           │                   │
    │                     │                   │
```

**Refresh Token Rotation:**
- Each refresh invalidates old refresh token
- New refresh token issued
- Prevents token replay attacks
- Old tokens kept for 60s grace period (network issues)

---

## 🚪 Flow 6: Logout

### User-Initiated Logout

```
Client              API              Database
  │                  │                   │
  │ 1. User clicks   │                   │
  │    Logout        │                   │
  │                  │                   │
  │ 2. POST /connect/logout              │
  │    id_token_hint=<token>             │
  ├─────────────────>│                   │
  │                  │                   │
  │                  │ 3. Revoke all     │
  │                  │    tokens for     │
  │                  │    this session   │
  │                  ├──────────────────>│
  │                  │                   │
  │                  │ 4. Clear session  │
  │                  │                   │
  │ 5. Redirect to   │                   │
  │    post_logout   │                   │
  │    redirect_uri  │                   │
  │<─────────────────┤                   │
  │                  │                   │
  │ 6. Clear local   │                   │
  │    storage/      │                   │
  │    cookies       │                   │
  │                  │                   │
```

---

## 🔒 Token Structure

### Access Token (JWT)

```json
{
  "header": {
    "alg": "RS256",
    "kid": "key_id_1",
    "typ": "at+jwt"
  },
  "payload": {
    "sub": "user_12345",
    "email": "user@example.com",
    "email_verified": true,
    "phone_verified": true,
    "roles": ["user", "verified"],
    "iat": 1699999999,
    "exp": 1700000899,
    "iss": "https://api.deviceownership.com",
    "aud": "device_api",
    "client_id": "mobile_app",
    "scope": "openid profile email device marketplace"
  }
}
```

**Lifetime:** 15 minutes (900 seconds)

### ID Token (JWT)

```json
{
  "header": {
    "alg": "RS256",
    "kid": "key_id_1",
    "typ": "JWT"
  },
  "payload": {
    "sub": "user_12345",
    "email": "user@example.com",
    "email_verified": true,
    "phone_number": "+44xxxxxxxxxx",
    "phone_verified": true,
    "name": "John Doe",
    "given_name": "John",
    "family_name": "Doe",
    "picture": "https://...",
    "subscription_tier": "premium",
    "iat": 1699999999,
    "exp": 1700003599,
    "iss": "https://api.deviceownership.com",
    "aud": "mobile_app"
  }
}
```

**Lifetime:** 1 hour (3600 seconds)

### Refresh Token

**Format:** Opaque token (random string)
**Lifetime:**
- Mobile: 7 days (604800 seconds)
- Web: 1 day (86400 seconds)
- Business: 30 days (2592000 seconds)

**Stored in Database:**
```json
{
  "id": "rt_xyz123",
  "token_hash": "sha256_hash",
  "user_id": "user_12345",
  "client_id": "mobile_app",
  "created_at": "2024-01-01T00:00:00Z",
  "expires_at": "2024-01-08T00:00:00Z",
  "revoked": false,
  "device_info": {
    "device_id": "device_abc",
    "platform": "iOS",
    "app_version": "1.0.0"
  }
}
```

---

## 🎫 Scopes & Permissions

### Available Scopes

| Scope | Description | Required Role |
|-------|-------------|---------------|
| `openid` | OpenID Connect authentication | All |
| `profile` | User profile (name, picture) | All |
| `email` | Email address | All |
| `phone` | Phone number | All |
| `device` | Device registration & management | User+ |
| `device.register` | Register devices | User+ |
| `device.read` | Read device information | User+ |
| `device.update` | Update device information | User+ |
| `device.delete` | Delete devices | User+ |
| `transfer` | Ownership transfer | User+ |
| `report` | Theft/loss reporting | User+ |
| `marketplace` | Marketplace access | Verified User+ |
| `marketplace.list` | List items for sale | Verified User+ |
| `marketplace.buy` | Purchase items | Verified User+ |
| `business` | Business features | Business |
| `business.bulk` | Bulk operations | Business |
| `police` | Police portal access | Police |
| `police.search` | Search all devices | Police |
| `police.reports` | Access theft reports | Police |
| `admin` | Admin portal | Admin |
| `admin.users` | User management | Admin |
| `admin.system` | System configuration | Admin |

### Scope to Permission Mapping

```csharp
// Example: Device Registration endpoint
[Authorize]
[RequireScope("device.register")]
[RequireEmailVerified]
public async Task<IActionResult> RegisterDevice(DeviceDto device)
{
    // User must have:
    // 1. Valid access token
    // 2. "device.register" scope
    // 3. Verified email

    // Additional check: Free tier limit
    if (!User.HasClaim("subscription", "premium"))
    {
        var deviceCount = await _deviceService.GetUserDeviceCount(User.Id);
        if (deviceCount >= 3)
        {
            return BadRequest("Free tier limited to 3 devices");
        }
    }

    // Proceed with registration
}
```

---

## 🛡️ Security Features

### 1. Brute Force Protection

```
Login Attempt Tracking:
┌─────────────────────────────────────────┐
│ IP: 192.168.1.1                         │
│ User: user@example.com                  │
│ Failed Attempts: 3                      │
│ Lockout Until: 2024-01-01 12:15:00     │
└─────────────────────────────────────────┘

Rules:
- 5 failed attempts from same IP = 15 min lockout
- 3 failed attempts for same user = 5 min lockout
- 10 failed attempts from same IP = 1 hour lockout
- Successful login resets counter
```

### 2. Rate Limiting

```
Per Endpoint:
- /connect/token: 10 requests/minute per IP
- /api/v1/devices/check: 20 requests/minute per user
- /api/v1/devices/register: 5 requests/minute per user

Per User (authenticated):
- 100 requests/minute across all endpoints
- Burst: 200 requests

Per IP (unauthenticated):
- 20 requests/minute
```

### 3. Account Takeover Prevention

```
Risk Signals:
1. New device/location detected
   → Send verification email
   → Require 2FA for sensitive operations

2. Impossible travel
   (e.g., London → New York in 1 hour)
   → Force re-authentication
   → Alert user

3. Multiple devices logging in simultaneously
   → Alert user
   → Show active sessions

4. Password changed
   → Revoke all tokens
   → Force re-login everywhere
   → Email notification
```

### 4. Token Security

```
Access Token:
✓ Short-lived (15 min)
✓ JWT with RS256 signing
✓ Audience validation
✓ Issuer validation
✓ Expiration check

Refresh Token:
✓ Opaque (not JWT)
✓ Stored hashed in database
✓ Rotation on use
✓ Device binding (optional)
✓ Can be revoked individually
✓ Expiration tracked

Security Headers:
✓ Strict-Transport-Security
✓ X-Content-Type-Options: nosniff
✓ X-Frame-Options: DENY
✓ Content-Security-Policy
```

---

## 📊 Audit Logging

### Events Logged

```
Authentication Events:
- Login success/failure
- Logout
- Token issued
- Token refreshed
- Token revoked
- Password changed
- 2FA enabled/disabled
- Account locked/unlocked

Authorization Events:
- Permission denied
- Role changed
- Scope requested
- Suspicious activity detected

User Actions:
- Device registered
- Ownership transferred
- Theft reported
- Police search performed
- Admin action taken
```

### Log Format

```json
{
  "event_id": "evt_123",
  "timestamp": "2024-01-01T12:00:00Z",
  "event_type": "authentication.login.success",
  "user_id": "user_12345",
  "ip_address": "192.168.1.1",
  "user_agent": "DeviceOwnerApp/1.0 iOS/17.0",
  "device_info": {
    "device_id": "device_abc",
    "platform": "iOS",
    "location": "London, UK"
  },
  "metadata": {
    "client_id": "mobile_app",
    "scopes_granted": ["openid", "profile", "device"]
  }
}
```

---

## 🔧 Implementation Checklist

### OpenIddict Configuration

```csharp
// Startup.cs / Program.cs
services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
            .UseDbContext<ApplicationDbContext>();
    })
    .AddServer(options =>
    {
        // Endpoints
        options.SetAuthorizationEndpointUris("/connect/authorize")
            .SetTokenEndpointUris("/connect/token")
            .SetUserinfoEndpointUris("/connect/userinfo")
            .SetLogoutEndpointUris("/connect/logout");

        // Flows
        options.AllowAuthorizationCodeFlow()
            .AllowRefreshTokenFlow()
            .AllowClientCredentialsFlow();

        // PKCE
        options.RequireProofKeyForCodeExchange();

        // Token lifetimes
        options.SetAccessTokenLifetime(TimeSpan.FromMinutes(15))
            .SetRefreshTokenLifetime(TimeSpan.FromDays(7))
            .SetIdentityTokenLifetime(TimeSpan.FromHours(1));

        // Refresh token rotation
        options.UseRollingRefreshTokens();

        // Signing & encryption
        options.AddDevelopmentEncryptionCertificate()
            .AddDevelopmentSigningCertificate();
        // In production: Use Azure Key Vault certificates

        // ASP.NET Core integration
        options.UseAspNetCore()
            .EnableAuthorizationEndpointPassthrough()
            .EnableTokenEndpointPassthrough()
            .EnableLogoutEndpointPassthrough();
    })
    .AddValidation(options =>
    {
        options.UseLocalServer();
        options.UseAspNetCore();
    });
```

### Client Registration

```csharp
// Seed data for clients
public class ClientSeeder
{
    public static async Task SeedClients(IServiceProvider services)
    {
        var manager = services.GetRequiredService<IOpenIddictApplicationManager>();

        // Mobile App
        if (await manager.FindByClientIdAsync("mobile_app") == null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "mobile_app",
                DisplayName = "Device Ownership Mobile App",
                Type = ClientTypes.Public, // No client secret (PKCE required)
                RedirectUris = { new Uri("deviceapp://callback") },
                PostLogoutRedirectUris = { new Uri("deviceapp://logout") },
                Permissions =
                {
                    Permissions.Endpoints.Authorization,
                    Permissions.Endpoints.Token,
                    Permissions.Endpoints.Logout,
                    Permissions.GrantTypes.AuthorizationCode,
                    Permissions.GrantTypes.RefreshToken,
                    Permissions.ResponseTypes.Code,
                    Permissions.Scopes.Email,
                    Permissions.Scopes.Profile,
                    Permissions.Scopes.Roles,
                    "scp:device",
                    "scp:transfer",
                    "scp:report",
                    "scp:marketplace"
                },
                Requirements =
                {
                    Requirements.Features.ProofKeyForCodeExchange
                }
            });
        }

        // Web App
        if (await manager.FindByClientIdAsync("web_app") == null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "web_app",
                DisplayName = "Device Ownership Web App",
                Type = ClientTypes.Public,
                RedirectUris = { new Uri("https://app.deviceownership.com/callback") },
                PostLogoutRedirectUris = { new Uri("https://app.deviceownership.com/") },
                Permissions =
                {
                    Permissions.Endpoints.Authorization,
                    Permissions.Endpoints.Token,
                    Permissions.Endpoints.Logout,
                    Permissions.GrantTypes.AuthorizationCode,
                    Permissions.GrantTypes.RefreshToken,
                    Permissions.ResponseTypes.Code,
                    Permissions.Scopes.Email,
                    Permissions.Scopes.Profile,
                    "scp:device",
                    "scp:marketplace"
                },
                Requirements =
                {
                    Requirements.Features.ProofKeyForCodeExchange
                }
            });
        }

        // Business API Client
        if (await manager.FindByClientIdAsync("business_api") == null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "business_api",
                ClientSecret = "secret_from_azure_key_vault",
                DisplayName = "Business API Integration",
                Type = ClientTypes.Confidential,
                Permissions =
                {
                    Permissions.Endpoints.Token,
                    Permissions.GrantTypes.ClientCredentials,
                    "scp:device.register",
                    "scp:device.read",
                    "scp:business.bulk"
                }
            });
        }
    }
}
```

---

This comprehensive authentication flow documentation ensures secure, scalable, and compliant user authentication across all platforms.
