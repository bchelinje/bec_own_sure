# 🗄️ Database Schema Design

## Overview

PostgreSQL database schema for the Device Ownership & Anti-Theft Platform with full support for device registration, ownership tracking, theft reporting, and marketplace functionality.

---

## 📊 Entity Relationship Diagram

```
┌─────────────────┐         ┌─────────────────┐         ┌─────────────────┐
│     Users       │         │    Devices      │         │  DevicePhotos   │
├─────────────────┤         ├─────────────────┤         ├─────────────────┤
│ Id (PK)         │────┐    │ Id (PK)         │────────<│ Id (PK)         │
│ Email           │    │    │ UserId (FK)     │         │ DeviceId (FK)   │
│ PasswordHash    │    └──<│ SerialNoHash    │         │ PhotoUrl        │
│ PhoneNumber     │         │ Category        │         │ PhotoType       │
│ EmailVerified   │         │ Brand           │         │ UploadedAt      │
│ PhoneVerified   │         │ Model           │         └─────────────────┘
│ FirstName       │         │ Description     │
│ LastName        │         │ PurchaseDate    │         ┌─────────────────┐
│ SubscriptionTier│         │ Status          │         │DeviceDocuments  │
│ CreatedAt       │         │ RegisteredAt    │         ├─────────────────┤
│ UpdatedAt       │         │ IsStolen        │         │ Id (PK)         │
└─────────────────┘         │ IsLost          │         │ DeviceId (FK)   │
                            │ DeletedAt       │         │ DocumentType    │
                            └─────────────────┘         │ DocumentUrl     │
                                    │                   │ UploadedAt      │
                                    │                   └─────────────────┘
                    ┌───────────────┼───────────────┐
                    │               │               │
                    ▼               ▼               ▼
        ┌─────────────────┐ ┌─────────────┐ ┌──────────────┐
        │OwnershipHistory │ │TheftReports │ │ Marketplace  │
        ├─────────────────┤ ├─────────────┤ ├──────────────┤
        │ Id (PK)         │ │ Id (PK)     │ │ Id (PK)      │
        │ DeviceId (FK)   │ │ DeviceId(FK)│ │ DeviceId(FK) │
        │ FromUserId (FK) │ │ UserId (FK) │ │ SellerId(FK) │
        │ ToUserId (FK)   │ │ ReportType  │ │ Title        │
        │ TransferredAt   │ │ Description │ │ Description  │
        │ TransferMethod  │ │ Location    │ │ Price        │
        │ Status          │ │ PoliceRef   │ │ Status       │
        └─────────────────┘ │ ReportedAt  │ │ ListedAt     │
                            │ RecoveredAt │ │ SoldAt       │
                            │ Status      │ │ BuyerId (FK) │
        ┌─────────────────┐ └─────────────┘ └──────────────┘
        │OwnershipTransfer│
        ├─────────────────┤         ┌─────────────────┐
        │ Id (PK)         │         │  PoliceReports  │
        │ DeviceId (FK)   │         ├─────────────────┤
        │ FromUserId (FK) │         │ Id (PK)         │
        │ ToUserId (FK)   │         │ TheftReportId   │
        │ Status          │         │ PoliceUserId(FK)│
        │ InitiatedAt     │         │ Notes           │
        │ CompletedAt     │         │ RecoveryStatus  │
        │ Code            │         │ CreatedAt       │
        └─────────────────┘         └─────────────────┘

┌─────────────────┐         ┌─────────────────┐
│  Subscriptions  │         │    Payments     │
├─────────────────┤         ├─────────────────┤
│ Id (PK)         │         │ Id (PK)         │
│ UserId (FK)     │         │ UserId (FK)     │
│ Plan            │         │ SubscriptionId  │
│ Status          │         │ Amount          │
│ StartedAt       │         │ Currency        │
│ EndsAt          │         │ Status          │
│ StripeId        │         │ PaymentDate     │
└─────────────────┘         │ StripePaymentId │
                            └─────────────────┘

┌─────────────────┐         ┌─────────────────┐
│   AuditLogs     │         │  Notifications  │
├─────────────────┤         ├─────────────────┤
│ Id (PK)         │         │ Id (PK)         │
│ UserId (FK)     │         │ UserId (FK)     │
│ Action          │         │ Type            │
│ EntityType      │         │ Title           │
│ EntityId        │         │ Message         │
│ IpAddress       │         │ ReadAt          │
│ UserAgent       │         │ CreatedAt       │
│ CreatedAt       │         │ Data (JSONB)    │
└─────────────────┘         └─────────────────┘
```

---

## 📋 Table Definitions

### 1. Users Table

```sql
CREATE TABLE "Users" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "Email" VARCHAR(255) NOT NULL UNIQUE,
    "PasswordHash" VARCHAR(255) NOT NULL,
    "PhoneNumber" VARCHAR(50),
    "EmailVerified" BOOLEAN DEFAULT FALSE,
    "PhoneVerified" BOOLEAN DEFAULT FALSE,
    "FirstName" VARCHAR(100),
    "LastName" VARCHAR(100),
    "ProfilePictureUrl" TEXT,
    "SubscriptionTier" VARCHAR(50) DEFAULT 'free', -- free, premium, business
    "Role" VARCHAR(50) DEFAULT 'user', -- user, police, admin
    "IsActive" BOOLEAN DEFAULT TRUE,
    "IsDeleted" BOOLEAN DEFAULT FALSE,
    "LastLoginAt" TIMESTAMP WITH TIME ZONE,
    "CreatedAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    "UpdatedAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    "DeletedAt" TIMESTAMP WITH TIME ZONE
);

CREATE INDEX "IX_Users_Email" ON "Users"("Email");
CREATE INDEX "IX_Users_PhoneNumber" ON "Users"("PhoneNumber");
CREATE INDEX "IX_Users_SubscriptionTier" ON "Users"("SubscriptionTier");
CREATE INDEX "IX_Users_Role" ON "Users"("Role");
```

**Business Rules:**
- Email must be unique and verified before device registration
- Free tier: max 3 devices
- Premium tier: unlimited devices
- Soft delete: IsDeleted flag instead of physical deletion

---

### 2. Devices Table

```sql
CREATE TABLE "Devices" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "UserId" UUID NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "SerialNumberHash" VARCHAR(255) NOT NULL UNIQUE, -- SHA256 hash
    "SerialNumberEncrypted" TEXT NOT NULL, -- AES encrypted
    "Category" VARCHAR(100) NOT NULL, -- phone, laptop, tablet, appliance, etc.
    "Brand" VARCHAR(100),
    "Model" VARCHAR(100),
    "Description" TEXT,
    "Color" VARCHAR(50),
    "PurchaseDate" DATE,
    "PurchasePrice" DECIMAL(10, 2),
    "PurchaseCurrency" VARCHAR(3) DEFAULT 'GBP',
    "WarrantyExpiryDate" DATE,
    "Status" VARCHAR(50) DEFAULT 'active', -- active, transferred, stolen, lost, recovered, deleted
    "IsStolen" BOOLEAN DEFAULT FALSE,
    "IsLost" BOOLEAN DEFAULT FALSE,
    "VerificationCode" VARCHAR(50), -- 6-digit code for quick verification
    "QRCodeUrl" TEXT,
    "RegisteredAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    "LastUpdatedAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    "DeletedAt" TIMESTAMP WITH TIME ZONE,

    -- Additional metadata (JSONB for flexibility)
    "Metadata" JSONB
);

CREATE INDEX "IX_Devices_UserId" ON "Devices"("UserId");
CREATE INDEX "IX_Devices_SerialNumberHash" ON "Devices"("SerialNumberHash");
CREATE INDEX "IX_Devices_Status" ON "Devices"("Status");
CREATE INDEX "IX_Devices_IsStolen" ON "Devices"("IsStolen") WHERE "IsStolen" = TRUE;
CREATE INDEX "IX_Devices_IsLost" ON "Devices"("IsLost") WHERE "IsLost" = TRUE;
CREATE INDEX "IX_Devices_Category" ON "Devices"("Category");
CREATE INDEX "IX_Devices_VerificationCode" ON "Devices"("VerificationCode");

-- GIN index for JSONB metadata search
CREATE INDEX "IX_Devices_Metadata" ON "Devices" USING GIN ("Metadata");
```

**Security:**
- Serial numbers stored both hashed (for lookup) and encrypted (for display to owner)
- Hash prevents rainbow table attacks
- Encryption key stored in Azure Key Vault

---

### 3. DevicePhotos Table

```sql
CREATE TABLE "DevicePhotos" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "DeviceId" UUID NOT NULL REFERENCES "Devices"("Id") ON DELETE CASCADE,
    "PhotoUrl" TEXT NOT NULL, -- Azure Blob Storage URL
    "PhotoType" VARCHAR(50) NOT NULL, -- device, serial_number, receipt, damage
    "IsPrimary" BOOLEAN DEFAULT FALSE,
    "Caption" VARCHAR(500),
    "UploadedAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

CREATE INDEX "IX_DevicePhotos_DeviceId" ON "DevicePhotos"("DeviceId");
CREATE INDEX "IX_DevicePhotos_PhotoType" ON "DevicePhotos"("PhotoType");
```

---

### 4. DeviceDocuments Table

```sql
CREATE TABLE "DeviceDocuments" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "DeviceId" UUID NOT NULL REFERENCES "Devices"("Id") ON DELETE CASCADE,
    "DocumentType" VARCHAR(50) NOT NULL, -- receipt, warranty, manual, insurance
    "DocumentUrl" TEXT NOT NULL, -- Azure Blob Storage URL
    "FileName" VARCHAR(255),
    "FileSize" BIGINT,
    "MimeType" VARCHAR(100),
    "UploadedAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

CREATE INDEX "IX_DeviceDocuments_DeviceId" ON "DeviceDocuments"("DeviceId");
```

---

### 5. OwnershipHistory Table

```sql
CREATE TABLE "OwnershipHistory" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "DeviceId" UUID NOT NULL REFERENCES "Devices"("Id") ON DELETE CASCADE,
    "FromUserId" UUID REFERENCES "Users"("Id"), -- NULL for initial registration
    "ToUserId" UUID NOT NULL REFERENCES "Users"("Id"),
    "TransferMethod" VARCHAR(50), -- registered, transferred, marketplace
    "TransferredAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    "Notes" TEXT,

    -- Verification
    "VerificationDocumentUrl" TEXT,
    "IpAddress" VARCHAR(50),
    "Location" VARCHAR(255) -- City, Country
);

CREATE INDEX "IX_OwnershipHistory_DeviceId" ON "OwnershipHistory"("DeviceId");
CREATE INDEX "IX_OwnershipHistory_FromUserId" ON "OwnershipHistory"("FromUserId");
CREATE INDEX "IX_OwnershipHistory_ToUserId" ON "OwnershipHistory"("ToUserId");
CREATE INDEX "IX_OwnershipHistory_TransferredAt" ON "OwnershipHistory"("TransferredAt");
```

**Chain of Ownership:**
- Every device has complete history from registration
- Cannot be deleted (audit trail)
- Used for dispute resolution

---

### 6. OwnershipTransfers Table

```sql
CREATE TABLE "OwnershipTransfers" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "DeviceId" UUID NOT NULL REFERENCES "Devices"("Id") ON DELETE CASCADE,
    "FromUserId" UUID NOT NULL REFERENCES "Users"("Id"),
    "ToUserId" UUID NOT NULL REFERENCES "Users"("Id"),
    "Status" VARCHAR(50) DEFAULT 'pending', -- pending, accepted, rejected, expired, cancelled
    "TransferCode" VARCHAR(10) NOT NULL UNIQUE, -- 6-10 char code
    "Message" TEXT,
    "InitiatedAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    "RespondedAt" TIMESTAMP WITH TIME ZONE,
    "CompletedAt" TIMESTAMP WITH TIME ZONE,
    "ExpiresAt" TIMESTAMP WITH TIME ZONE, -- 7 days from initiation
    "RejectionReason" TEXT
);

CREATE INDEX "IX_OwnershipTransfers_DeviceId" ON "OwnershipTransfers"("DeviceId");
CREATE INDEX "IX_OwnershipTransfers_FromUserId" ON "OwnershipTransfers"("FromUserId");
CREATE INDEX "IX_OwnershipTransfers_ToUserId" ON "OwnershipTransfers"("ToUserId");
CREATE INDEX "IX_OwnershipTransfers_Status" ON "OwnershipTransfers"("Status");
CREATE INDEX "IX_OwnershipTransfers_TransferCode" ON "OwnershipTransfers"("TransferCode");
```

---

### 7. TheftReports Table

```sql
CREATE TABLE "TheftReports" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "DeviceId" UUID NOT NULL REFERENCES "Devices"("Id") ON DELETE CASCADE,
    "UserId" UUID NOT NULL REFERENCES "Users"("Id"),
    "ReportType" VARCHAR(50) NOT NULL, -- stolen, lost
    "Title" VARCHAR(255) NOT NULL,
    "Description" TEXT NOT NULL,

    -- Location
    "IncidentDate" TIMESTAMP WITH TIME ZONE,
    "IncidentLocation" VARCHAR(500),
    "IncidentCity" VARCHAR(100),
    "IncidentCountry" VARCHAR(100),
    "IncidentLatitude" DECIMAL(10, 8),
    "IncidentLongitude" DECIMAL(11, 8),

    -- Police
    "PoliceReferenceNumber" VARCHAR(100),
    "PoliceStation" VARCHAR(255),
    "PoliceOfficerName" VARCHAR(255),

    -- Status
    "Status" VARCHAR(50) DEFAULT 'active', -- active, investigating, recovered, closed
    "IsPublic" BOOLEAN DEFAULT TRUE, -- show in community alerts
    "RewardAmount" DECIMAL(10, 2),
    "RewardCurrency" VARCHAR(3) DEFAULT 'GBP',

    "ReportedAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    "RecoveredAt" TIMESTAMP WITH TIME ZONE,
    "ClosedAt" TIMESTAMP WITH TIME ZONE,

    -- Additional details
    "SuspectDescription" TEXT,
    "WitnessInformation" TEXT,
    "AdditionalNotes" TEXT
);

CREATE INDEX "IX_TheftReports_DeviceId" ON "TheftReports"("DeviceId");
CREATE INDEX "IX_TheftReports_UserId" ON "TheftReports"("UserId");
CREATE INDEX "IX_TheftReports_ReportType" ON "TheftReports"("ReportType");
CREATE INDEX "IX_TheftReports_Status" ON "TheftReports"("Status");
CREATE INDEX "IX_TheftReports_IncidentDate" ON "TheftReports"("IncidentDate");
CREATE INDEX "IX_TheftReports_IncidentCity" ON "TheftReports"("IncidentCity");

-- Spatial index for location-based searches
CREATE INDEX "IX_TheftReports_Location" ON "TheftReports" USING GIST (
    ll_to_earth("IncidentLatitude", "IncidentLongitude")
);
```

---

### 8. PoliceReports Table

```sql
CREATE TABLE "PoliceReports" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "TheftReportId" UUID NOT NULL REFERENCES "TheftReports"("Id") ON DELETE CASCADE,
    "PoliceUserId" UUID NOT NULL REFERENCES "Users"("Id"), -- User with police role
    "OfficerBadgeNumber" VARCHAR(50),
    "OfficerName" VARCHAR(255),
    "PoliceStation" VARCHAR(255),
    "CaseNumber" VARCHAR(100),
    "Notes" TEXT,
    "RecoveryStatus" VARCHAR(50), -- under_investigation, evidence_collected, recovered, closed
    "EvidenceUrl" TEXT, -- Document/photo URL
    "CreatedAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    "UpdatedAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

CREATE INDEX "IX_PoliceReports_TheftReportId" ON "PoliceReports"("TheftReportId");
CREATE INDEX "IX_PoliceReports_PoliceUserId" ON "PoliceReports"("PoliceUserId");
CREATE INDEX "IX_PoliceReports_CaseNumber" ON "PoliceReports"("CaseNumber");
```

---

### 9. Marketplace Table

```sql
CREATE TABLE "MarketplaceListings" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "DeviceId" UUID NOT NULL REFERENCES "Devices"("Id") ON DELETE CASCADE,
    "SellerId" UUID NOT NULL REFERENCES "Users"("Id"),
    "Title" VARCHAR(255) NOT NULL,
    "Description" TEXT NOT NULL,
    "Price" DECIMAL(10, 2) NOT NULL,
    "Currency" VARCHAR(3) DEFAULT 'GBP',
    "Condition" VARCHAR(50), -- new, like_new, good, fair, poor
    "Status" VARCHAR(50) DEFAULT 'active', -- active, sold, expired, removed
    "Category" VARCHAR(100),
    "Location" VARCHAR(255),
    "IsShippingAvailable" BOOLEAN DEFAULT FALSE,
    "IsFeatured" BOOLEAN DEFAULT FALSE,
    "ViewCount" INT DEFAULT 0,
    "ListedAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    "SoldAt" TIMESTAMP WITH TIME ZONE,
    "ExpiresAt" TIMESTAMP WITH TIME ZONE, -- 90 days from listing
    "BuyerId" UUID REFERENCES "Users"("Id"),
    "UpdatedAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

CREATE INDEX "IX_MarketplaceListings_DeviceId" ON "MarketplaceListings"("DeviceId");
CREATE INDEX "IX_MarketplaceListings_SellerId" ON "MarketplaceListings"("SellerId");
CREATE INDEX "IX_MarketplaceListings_Status" ON "MarketplaceListings"("Status");
CREATE INDEX "IX_MarketplaceListings_Category" ON "MarketplaceListings"("Category");
CREATE INDEX "IX_MarketplaceListings_Price" ON "MarketplaceListings"("Price");
CREATE INDEX "IX_MarketplaceListings_ListedAt" ON "MarketplaceListings"("ListedAt" DESC);

-- Full-text search on title and description
CREATE INDEX "IX_MarketplaceListings_Search" ON "MarketplaceListings"
    USING GIN (to_tsvector('english', "Title" || ' ' || "Description"));
```

---

### 10. Subscriptions Table

```sql
CREATE TABLE "Subscriptions" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "UserId" UUID NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "Plan" VARCHAR(50) NOT NULL, -- free, premium_monthly, premium_yearly, business
    "Status" VARCHAR(50) DEFAULT 'active', -- active, cancelled, expired, past_due
    "BillingInterval" VARCHAR(50), -- monthly, yearly
    "Amount" DECIMAL(10, 2),
    "Currency" VARCHAR(3) DEFAULT 'GBP',
    "StartedAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    "CurrentPeriodStart" TIMESTAMP WITH TIME ZONE,
    "CurrentPeriodEnd" TIMESTAMP WITH TIME ZONE,
    "CancelledAt" TIMESTAMP WITH TIME ZONE,
    "EndsAt" TIMESTAMP WITH TIME ZONE,
    "TrialStartedAt" TIMESTAMP WITH TIME ZONE,
    "TrialEndsAt" TIMESTAMP WITH TIME ZONE,

    -- Stripe integration
    "StripeSubscriptionId" VARCHAR(255) UNIQUE,
    "StripeCustomerId" VARCHAR(255),
    "StripePriceId" VARCHAR(255),

    "CreatedAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    "UpdatedAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

CREATE INDEX "IX_Subscriptions_UserId" ON "Subscriptions"("UserId");
CREATE INDEX "IX_Subscriptions_Status" ON "Subscriptions"("Status");
CREATE INDEX "IX_Subscriptions_StripeSubscriptionId" ON "Subscriptions"("StripeSubscriptionId");
CREATE INDEX "IX_Subscriptions_CurrentPeriodEnd" ON "Subscriptions"("CurrentPeriodEnd");
```

---

### 11. Payments Table

```sql
CREATE TABLE "Payments" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "UserId" UUID NOT NULL REFERENCES "Users"("Id"),
    "SubscriptionId" UUID REFERENCES "Subscriptions"("Id"),
    "Amount" DECIMAL(10, 2) NOT NULL,
    "Currency" VARCHAR(3) DEFAULT 'GBP',
    "Status" VARCHAR(50) NOT NULL, -- pending, succeeded, failed, refunded
    "PaymentMethod" VARCHAR(50), -- card, paypal, etc.
    "Description" TEXT,
    "InvoiceUrl" TEXT,

    -- Stripe
    "StripePaymentIntentId" VARCHAR(255) UNIQUE,
    "StripeInvoiceId" VARCHAR(255),

    "PaymentDate" TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    "RefundedAt" TIMESTAMP WITH TIME ZONE,
    "RefundAmount" DECIMAL(10, 2),
    "CreatedAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

CREATE INDEX "IX_Payments_UserId" ON "Payments"("UserId");
CREATE INDEX "IX_Payments_SubscriptionId" ON "Payments"("SubscriptionId");
CREATE INDEX "IX_Payments_Status" ON "Payments"("Status");
CREATE INDEX "IX_Payments_PaymentDate" ON "Payments"("PaymentDate" DESC);
```

---

### 12. AuditLogs Table

```sql
CREATE TABLE "AuditLogs" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "UserId" UUID REFERENCES "Users"("Id"), -- NULL for system actions
    "Action" VARCHAR(100) NOT NULL, -- login, device_registered, transfer_initiated, etc.
    "EntityType" VARCHAR(100), -- User, Device, Transfer, etc.
    "EntityId" UUID,
    "IpAddress" VARCHAR(50),
    "UserAgent" TEXT,
    "Location" VARCHAR(255),
    "Details" JSONB, -- Additional context
    "CreatedAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

CREATE INDEX "IX_AuditLogs_UserId" ON "AuditLogs"("UserId");
CREATE INDEX "IX_AuditLogs_Action" ON "AuditLogs"("Action");
CREATE INDEX "IX_AuditLogs_EntityType" ON "AuditLogs"("EntityType");
CREATE INDEX "IX_AuditLogs_EntityId" ON "AuditLogs"("EntityId");
CREATE INDEX "IX_AuditLogs_CreatedAt" ON "AuditLogs"("CreatedAt" DESC);
CREATE INDEX "IX_AuditLogs_Details" ON "AuditLogs" USING GIN ("Details");
```

**Retention:** Keep logs for 2 years for compliance

---

### 13. Notifications Table

```sql
CREATE TABLE "Notifications" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "UserId" UUID NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "Type" VARCHAR(50) NOT NULL, -- transfer_request, theft_alert, marketplace_message, etc.
    "Title" VARCHAR(255) NOT NULL,
    "Message" TEXT NOT NULL,
    "IsRead" BOOLEAN DEFAULT FALSE,
    "ReadAt" TIMESTAMP WITH TIME ZONE,
    "Channel" VARCHAR(50), -- push, email, sms
    "Priority" VARCHAR(20) DEFAULT 'normal', -- low, normal, high, urgent
    "ActionUrl" TEXT, -- Deep link to relevant screen
    "Data" JSONB, -- Additional payload
    "CreatedAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    "ExpiresAt" TIMESTAMP WITH TIME ZONE
);

CREATE INDEX "IX_Notifications_UserId" ON "Notifications"("UserId");
CREATE INDEX "IX_Notifications_IsRead" ON "Notifications"("IsRead") WHERE "IsRead" = FALSE;
CREATE INDEX "IX_Notifications_Type" ON "Notifications"("Type");
CREATE INDEX "IX_Notifications_CreatedAt" ON "Notifications"("CreatedAt" DESC);
```

---

### 14. DeviceCategories Table (Lookup)

```sql
CREATE TABLE "DeviceCategories" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "Name" VARCHAR(100) NOT NULL UNIQUE,
    "DisplayName" VARCHAR(100) NOT NULL,
    "Icon" VARCHAR(50), -- Icon identifier
    "RequiresSerialNumber" BOOLEAN DEFAULT TRUE,
    "IsActive" BOOLEAN DEFAULT TRUE,
    "DisplayOrder" INT DEFAULT 0
);

-- Seed data
INSERT INTO "DeviceCategories" ("Name", "DisplayName", "Icon", "RequiresSerialNumber") VALUES
('smartphone', 'Smartphone', 'phone', TRUE),
('laptop', 'Laptop', 'laptop', TRUE),
('tablet', 'Tablet', 'tablet', TRUE),
('smartwatch', 'Smartwatch', 'watch', TRUE),
('camera', 'Camera', 'camera', TRUE),
('gaming_console', 'Gaming Console', 'gamepad', TRUE),
('tv', 'Television', 'tv', TRUE),
('headphones', 'Headphones', 'headphones', FALSE),
('appliance', 'Home Appliance', 'home', TRUE),
('tool', 'Power Tool', 'tool', FALSE),
('bicycle', 'Bicycle', 'bike', TRUE),
('other', 'Other', 'box', FALSE);
```

---

### 15. BusinessProfiles Table

```sql
CREATE TABLE "BusinessProfiles" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "UserId" UUID NOT NULL UNIQUE REFERENCES "Users"("Id") ON DELETE CASCADE,
    "CompanyName" VARCHAR(255) NOT NULL,
    "CompanyRegistrationNumber" VARCHAR(100),
    "VatNumber" VARCHAR(50),
    "Industry" VARCHAR(100),
    "Website" TEXT,
    "PhoneNumber" VARCHAR(50),
    "Address" TEXT,
    "City" VARCHAR(100),
    "PostCode" VARCHAR(20),
    "Country" VARCHAR(100),
    "IsVerified" BOOLEAN DEFAULT FALSE,
    "VerifiedAt" TIMESTAMP WITH TIME ZONE,
    "VerificationDocumentUrl" TEXT,
    "ApiKeyHash" VARCHAR(255), -- For API access
    "ApiKeyCreatedAt" TIMESTAMP WITH TIME ZONE,
    "CreatedAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    "UpdatedAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

CREATE INDEX "IX_BusinessProfiles_UserId" ON "BusinessProfiles"("UserId");
CREATE INDEX "IX_BusinessProfiles_IsVerified" ON "BusinessProfiles"("IsVerified");
```

---

### 16. PoliceProfiles Table

```sql
CREATE TABLE "PoliceProfiles" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "UserId" UUID NOT NULL UNIQUE REFERENCES "Users"("Id") ON DELETE CASCADE,
    "BadgeNumber" VARCHAR(50) NOT NULL,
    "Rank" VARCHAR(100),
    "Department" VARCHAR(255),
    "PoliceStation" VARCHAR(255),
    "StationAddress" TEXT,
    "City" VARCHAR(100),
    "PostCode" VARCHAR(20),
    "Country" VARCHAR(100),
    "OfficialEmail" VARCHAR(255),
    "OfficialPhone" VARCHAR(50),
    "IsVerified" BOOLEAN DEFAULT FALSE,
    "VerifiedAt" TIMESTAMP WITH TIME ZONE,
    "VerifiedByAdminId" UUID REFERENCES "Users"("Id"),
    "VerificationNotes" TEXT,
    "IdDocumentUrl" TEXT, -- Stored securely
    "BadgePhotoUrl" TEXT,
    "CreatedAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    "UpdatedAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

CREATE INDEX "IX_PoliceProfiles_UserId" ON "PoliceProfiles"("UserId");
CREATE INDEX "IX_PoliceProfiles_BadgeNumber" ON "PoliceProfiles"("BadgeNumber");
CREATE INDEX "IX_PoliceProfiles_IsVerified" ON "PoliceProfiles"("IsVerified");
```

---

## 🔐 Security Considerations

### Data Encryption

```sql
-- Example: Encrypt serial numbers before storing
CREATE EXTENSION IF NOT EXISTS pgcrypto;

-- Function to encrypt data
CREATE OR REPLACE FUNCTION encrypt_serial_number(serial_number TEXT, key TEXT)
RETURNS TEXT AS $$
BEGIN
    RETURN encode(
        pgp_sym_encrypt(serial_number, key),
        'base64'
    );
END;
$$ LANGUAGE plpgsql;

-- Function to decrypt data
CREATE OR REPLACE FUNCTION decrypt_serial_number(encrypted_data TEXT, key TEXT)
RETURNS TEXT AS $$
BEGIN
    RETURN pgp_sym_decrypt(
        decode(encrypted_data, 'base64'),
        key
    );
END;
$$ LANGUAGE plpgsql;

-- Usage in application:
-- INSERT: SerialNumberEncrypted = encrypt_serial_number(@serialNo, @encryptionKey)
-- SELECT: decrypt_serial_number(SerialNumberEncrypted, @encryptionKey)
```

### Row-Level Security (RLS)

```sql
-- Enable RLS on sensitive tables
ALTER TABLE "Devices" ENABLE ROW LEVEL SECURITY;

-- Policy: Users can only see their own devices
CREATE POLICY user_devices_policy ON "Devices"
    FOR SELECT
    TO authenticated_user
    USING ("UserId" = current_user_id());

-- Policy: Police can see all devices
CREATE POLICY police_devices_policy ON "Devices"
    FOR SELECT
    TO police_user
    USING (TRUE);

-- Policy: Users can only update their own devices
CREATE POLICY user_devices_update_policy ON "Devices"
    FOR UPDATE
    TO authenticated_user
    USING ("UserId" = current_user_id());
```

---

## 📈 Performance Optimization

### Partitioning

```sql
-- Partition AuditLogs by month
CREATE TABLE "AuditLogs" (
    "Id" UUID,
    "UserId" UUID,
    "Action" VARCHAR(100),
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL,
    ...
) PARTITION BY RANGE ("CreatedAt");

-- Create partitions
CREATE TABLE "AuditLogs_2024_01" PARTITION OF "AuditLogs"
    FOR VALUES FROM ('2024-01-01') TO ('2024-02-01');

CREATE TABLE "AuditLogs_2024_02" PARTITION OF "AuditLogs"
    FOR VALUES FROM ('2024-02-01') TO ('2024-03-01');
-- etc.
```

### Materialized Views

```sql
-- Popular devices by category
CREATE MATERIALIZED VIEW "PopularDeviceCategories" AS
SELECT
    "Category",
    COUNT(*) AS "TotalDevices",
    COUNT(DISTINCT "UserId") AS "UniqueOwners",
    COUNT(*) FILTER (WHERE "IsStolen" = TRUE) AS "StolenCount",
    COUNT(*) FILTER (WHERE "IsLost" = TRUE) AS "LostCount"
FROM "Devices"
WHERE "DeletedAt" IS NULL
GROUP BY "Category";

CREATE UNIQUE INDEX ON "PopularDeviceCategories"("Category");

-- Refresh daily
REFRESH MATERIALIZED VIEW CONCURRENTLY "PopularDeviceCategories";
```

---

## 🔍 Common Queries

### 1. Check if Serial Number is Stolen/Lost

```sql
SELECT
    d."Id",
    d."Status",
    d."IsStolen",
    d."IsLost",
    u."Email" AS "OwnerEmail",
    u."PhoneNumber" AS "OwnerPhone",
    tr."ReportType",
    tr."IncidentDate",
    tr."PoliceReferenceNumber"
FROM "Devices" d
INNER JOIN "Users" u ON d."UserId" = u."Id"
LEFT JOIN "TheftReports" tr ON d."Id" = tr."DeviceId" AND tr."Status" = 'active'
WHERE d."SerialNumberHash" = @serialNumberHash
    AND d."DeletedAt" IS NULL;
```

### 2. Get Device Ownership History

```sql
SELECT
    oh."Id",
    oh."TransferredAt",
    oh."TransferMethod",
    from_user."Email" AS "FromUserEmail",
    from_user."FirstName" AS "FromFirstName",
    from_user."LastName" AS "FromLastName",
    to_user."Email" AS "ToUserEmail",
    to_user."FirstName" AS "ToFirstName",
    to_user."LastName" AS "ToLastName",
    oh."Notes"
FROM "OwnershipHistory" oh
LEFT JOIN "Users" from_user ON oh."FromUserId" = from_user."Id"
INNER JOIN "Users" to_user ON oh."ToUserId" = to_user."Id"
WHERE oh."DeviceId" = @deviceId
ORDER BY oh."TransferredAt" DESC;
```

### 3. Get Nearby Theft Reports

```sql
SELECT
    tr."Id",
    tr."ReportType",
    tr."Description",
    tr."IncidentDate",
    tr."IncidentLocation",
    d."Category",
    d."Brand",
    d."Model",
    earth_distance(
        ll_to_earth(tr."IncidentLatitude", tr."IncidentLongitude"),
        ll_to_earth(@userLat, @userLon)
    ) / 1000 AS "DistanceKm"
FROM "TheftReports" tr
INNER JOIN "Devices" d ON tr."DeviceId" = d."Id"
WHERE tr."Status" = 'active'
    AND tr."IsPublic" = TRUE
    AND earth_distance(
        ll_to_earth(tr."IncidentLatitude", tr."IncidentLongitude"),
        ll_to_earth(@userLat, @userLon)
    ) <= @radiusMeters
ORDER BY "DistanceKm" ASC
LIMIT 50;
```

### 4. User's Device Count (for subscription limits)

```sql
SELECT COUNT(*)
FROM "Devices"
WHERE "UserId" = @userId
    AND "Status" IN ('active', 'stolen', 'lost')
    AND "DeletedAt" IS NULL;
```

---

## 🔄 Database Migrations Strategy

1. **Version Control**: All migrations in `/database/migrations/`
2. **Naming**: `{timestamp}_{description}.sql` (e.g., `20240101_initial_schema.sql`)
3. **Rollback**: Each migration has corresponding rollback script
4. **Testing**: Test migrations on staging before production
5. **Backup**: Automated backup before migration execution

---

This schema provides a solid foundation for the MVP and can scale to millions of devices with proper indexing and partitioning strategies.
