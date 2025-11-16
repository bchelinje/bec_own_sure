-- Device Ownership & Anti-Theft Platform
-- PostgreSQL Database Schema
-- Version: 1.0.0

-- Enable required extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pgcrypto";
CREATE EXTENSION IF NOT EXISTS "cube";
CREATE EXTENSION IF NOT EXISTS "earthdistance";

-- ============================================================================
-- USERS & AUTHENTICATION
-- ============================================================================

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
    "SubscriptionTier" VARCHAR(50) DEFAULT 'free',
    "Role" VARCHAR(50) DEFAULT 'user',
    "IsActive" BOOLEAN DEFAULT TRUE,
    "IsDeleted" BOOLEAN DEFAULT FALSE,
    "TwoFactorEnabled" BOOLEAN DEFAULT FALSE,
    "TwoFactorSecret" VARCHAR(255),
    "LastLoginAt" TIMESTAMP WITH TIME ZONE,
    "CreatedAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    "UpdatedAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    "DeletedAt" TIMESTAMP WITH TIME ZONE
);

CREATE INDEX "IX_Users_Email" ON "Users"("Email");
CREATE INDEX "IX_Users_PhoneNumber" ON "Users"("PhoneNumber");
CREATE INDEX "IX_Users_SubscriptionTier" ON "Users"("SubscriptionTier");
CREATE INDEX "IX_Users_Role" ON "Users"("Role");

-- ============================================================================
-- DEVICE CATEGORIES (LOOKUP)
-- ============================================================================

CREATE TABLE "DeviceCategories" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "Name" VARCHAR(100) NOT NULL UNIQUE,
    "DisplayName" VARCHAR(100) NOT NULL,
    "Icon" VARCHAR(50),
    "RequiresSerialNumber" BOOLEAN DEFAULT TRUE,
    "IsActive" BOOLEAN DEFAULT TRUE,
    "DisplayOrder" INT DEFAULT 0
);

CREATE INDEX "IX_DeviceCategories_Name" ON "DeviceCategories"("Name");

-- Seed device categories
INSERT INTO "DeviceCategories" ("Name", "DisplayName", "Icon", "RequiresSerialNumber", "DisplayOrder") VALUES
('smartphone', 'Smartphone', 'phone', TRUE, 1),
('laptop', 'Laptop', 'laptop', TRUE, 2),
('tablet', 'Tablet', 'tablet', TRUE, 3),
('smartwatch', 'Smartwatch', 'watch', TRUE, 4),
('camera', 'Camera', 'camera', TRUE, 5),
('gaming_console', 'Gaming Console', 'gamepad', TRUE, 6),
('tv', 'Television', 'tv', TRUE, 7),
('headphones', 'Headphones', 'headphones', FALSE, 8),
('appliance', 'Home Appliance', 'home', TRUE, 9),
('tool', 'Power Tool', 'tool', FALSE, 10),
('bicycle', 'Bicycle', 'bike', TRUE, 11),
('jewelry', 'Jewelry', 'diamond', FALSE, 12),
('other', 'Other', 'box', FALSE, 99);

-- ============================================================================
-- DEVICES
-- ============================================================================

CREATE TABLE "Devices" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "UserId" UUID NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "SerialNumberHash" VARCHAR(255) NOT NULL UNIQUE,
    "SerialNumberEncrypted" TEXT NOT NULL,
    "Category" VARCHAR(100) NOT NULL,
    "Brand" VARCHAR(100),
    "Model" VARCHAR(100),
    "Description" TEXT,
    "Color" VARCHAR(50),
    "PurchaseDate" DATE,
    "PurchasePrice" DECIMAL(10, 2),
    "PurchaseCurrency" VARCHAR(3) DEFAULT 'GBP',
    "WarrantyExpiryDate" DATE,
    "Status" VARCHAR(50) DEFAULT 'active',
    "IsStolen" BOOLEAN DEFAULT FALSE,
    "IsLost" BOOLEAN DEFAULT FALSE,
    "VerificationCode" VARCHAR(50),
    "QRCodeUrl" TEXT,
    "RegisteredAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    "LastUpdatedAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    "DeletedAt" TIMESTAMP WITH TIME ZONE,
    "Metadata" JSONB,

    CONSTRAINT "CHK_Device_Status" CHECK ("Status" IN ('active', 'transferred', 'stolen', 'lost', 'recovered', 'deleted'))
);

CREATE INDEX "IX_Devices_UserId" ON "Devices"("UserId");
CREATE INDEX "IX_Devices_SerialNumberHash" ON "Devices"("SerialNumberHash");
CREATE INDEX "IX_Devices_Status" ON "Devices"("Status");
CREATE INDEX "IX_Devices_IsStolen" ON "Devices"("IsStolen") WHERE "IsStolen" = TRUE;
CREATE INDEX "IX_Devices_IsLost" ON "Devices"("IsLost") WHERE "IsLost" = TRUE;
CREATE INDEX "IX_Devices_Category" ON "Devices"("Category");
CREATE INDEX "IX_Devices_VerificationCode" ON "Devices"("VerificationCode");
CREATE INDEX "IX_Devices_Metadata" ON "Devices" USING GIN ("Metadata");

-- ============================================================================
-- DEVICE PHOTOS & DOCUMENTS
-- ============================================================================

CREATE TABLE "DevicePhotos" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "DeviceId" UUID NOT NULL REFERENCES "Devices"("Id") ON DELETE CASCADE,
    "PhotoUrl" TEXT NOT NULL,
    "PhotoType" VARCHAR(50) NOT NULL,
    "IsPrimary" BOOLEAN DEFAULT FALSE,
    "Caption" VARCHAR(500),
    "UploadedAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW(),

    CONSTRAINT "CHK_Photo_Type" CHECK ("PhotoType" IN ('device', 'serial_number', 'receipt', 'damage', 'other'))
);

CREATE INDEX "IX_DevicePhotos_DeviceId" ON "DevicePhotos"("DeviceId");
CREATE INDEX "IX_DevicePhotos_PhotoType" ON "DevicePhotos"("PhotoType");

CREATE TABLE "DeviceDocuments" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "DeviceId" UUID NOT NULL REFERENCES "Devices"("Id") ON DELETE CASCADE,
    "DocumentType" VARCHAR(50) NOT NULL,
    "DocumentUrl" TEXT NOT NULL,
    "FileName" VARCHAR(255),
    "FileSize" BIGINT,
    "MimeType" VARCHAR(100),
    "UploadedAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW(),

    CONSTRAINT "CHK_Document_Type" CHECK ("DocumentType" IN ('receipt', 'warranty', 'manual', 'insurance', 'other'))
);

CREATE INDEX "IX_DeviceDocuments_DeviceId" ON "DeviceDocuments"("DeviceId");

-- ============================================================================
-- OWNERSHIP MANAGEMENT
-- ============================================================================

CREATE TABLE "OwnershipHistory" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "DeviceId" UUID NOT NULL REFERENCES "Devices"("Id") ON DELETE CASCADE,
    "FromUserId" UUID REFERENCES "Users"("Id"),
    "ToUserId" UUID NOT NULL REFERENCES "Users"("Id"),
    "TransferMethod" VARCHAR(50),
    "TransferredAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    "Notes" TEXT,
    "VerificationDocumentUrl" TEXT,
    "IpAddress" VARCHAR(50),
    "Location" VARCHAR(255),

    CONSTRAINT "CHK_Transfer_Method" CHECK ("TransferMethod" IN ('registered', 'transferred', 'marketplace', 'inherited', 'other'))
);

CREATE INDEX "IX_OwnershipHistory_DeviceId" ON "OwnershipHistory"("DeviceId");
CREATE INDEX "IX_OwnershipHistory_FromUserId" ON "OwnershipHistory"("FromUserId");
CREATE INDEX "IX_OwnershipHistory_ToUserId" ON "OwnershipHistory"("ToUserId");
CREATE INDEX "IX_OwnershipHistory_TransferredAt" ON "OwnershipHistory"("TransferredAt");

CREATE TABLE "OwnershipTransfers" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "DeviceId" UUID NOT NULL REFERENCES "Devices"("Id") ON DELETE CASCADE,
    "FromUserId" UUID NOT NULL REFERENCES "Users"("Id"),
    "ToUserId" UUID NOT NULL REFERENCES "Users"("Id"),
    "Status" VARCHAR(50) DEFAULT 'pending',
    "TransferCode" VARCHAR(10) NOT NULL UNIQUE,
    "Message" TEXT,
    "InitiatedAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    "RespondedAt" TIMESTAMP WITH TIME ZONE,
    "CompletedAt" TIMESTAMP WITH TIME ZONE,
    "ExpiresAt" TIMESTAMP WITH TIME ZONE,
    "RejectionReason" TEXT,

    CONSTRAINT "CHK_Transfer_Status" CHECK ("Status" IN ('pending', 'accepted', 'rejected', 'expired', 'cancelled'))
);

CREATE INDEX "IX_OwnershipTransfers_DeviceId" ON "OwnershipTransfers"("DeviceId");
CREATE INDEX "IX_OwnershipTransfers_FromUserId" ON "OwnershipTransfers"("FromUserId");
CREATE INDEX "IX_OwnershipTransfers_ToUserId" ON "OwnershipTransfers"("ToUserId");
CREATE INDEX "IX_OwnershipTransfers_Status" ON "OwnershipTransfers"("Status");
CREATE INDEX "IX_OwnershipTransfers_TransferCode" ON "OwnershipTransfers"("TransferCode");

-- ============================================================================
-- THEFT & LOSS REPORTING
-- ============================================================================

CREATE TABLE "TheftReports" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "DeviceId" UUID NOT NULL REFERENCES "Devices"("Id") ON DELETE CASCADE,
    "UserId" UUID NOT NULL REFERENCES "Users"("Id"),
    "ReportType" VARCHAR(50) NOT NULL,
    "Title" VARCHAR(255) NOT NULL,
    "Description" TEXT NOT NULL,
    "IncidentDate" TIMESTAMP WITH TIME ZONE,
    "IncidentLocation" VARCHAR(500),
    "IncidentCity" VARCHAR(100),
    "IncidentCountry" VARCHAR(100),
    "IncidentLatitude" DECIMAL(10, 8),
    "IncidentLongitude" DECIMAL(11, 8),
    "PoliceReferenceNumber" VARCHAR(100),
    "PoliceStation" VARCHAR(255),
    "PoliceOfficerName" VARCHAR(255),
    "Status" VARCHAR(50) DEFAULT 'active',
    "IsPublic" BOOLEAN DEFAULT TRUE,
    "RewardAmount" DECIMAL(10, 2),
    "RewardCurrency" VARCHAR(3) DEFAULT 'GBP',
    "ReportedAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    "RecoveredAt" TIMESTAMP WITH TIME ZONE,
    "ClosedAt" TIMESTAMP WITH TIME ZONE,
    "SuspectDescription" TEXT,
    "WitnessInformation" TEXT,
    "AdditionalNotes" TEXT,

    CONSTRAINT "CHK_Report_Type" CHECK ("ReportType" IN ('stolen', 'lost')),
    CONSTRAINT "CHK_Report_Status" CHECK ("Status" IN ('active', 'investigating', 'recovered', 'closed'))
);

CREATE INDEX "IX_TheftReports_DeviceId" ON "TheftReports"("DeviceId");
CREATE INDEX "IX_TheftReports_UserId" ON "TheftReports"("UserId");
CREATE INDEX "IX_TheftReports_ReportType" ON "TheftReports"("ReportType");
CREATE INDEX "IX_TheftReports_Status" ON "TheftReports"("Status");
CREATE INDEX "IX_TheftReports_IncidentDate" ON "TheftReports"("IncidentDate");
CREATE INDEX "IX_TheftReports_IncidentCity" ON "TheftReports"("IncidentCity");

-- ============================================================================
-- POLICE INTEGRATION
-- ============================================================================

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
    "IdDocumentUrl" TEXT,
    "BadgePhotoUrl" TEXT,
    "CreatedAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    "UpdatedAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

CREATE INDEX "IX_PoliceProfiles_UserId" ON "PoliceProfiles"("UserId");
CREATE INDEX "IX_PoliceProfiles_BadgeNumber" ON "PoliceProfiles"("BadgeNumber");
CREATE INDEX "IX_PoliceProfiles_IsVerified" ON "PoliceProfiles"("IsVerified");

CREATE TABLE "PoliceReports" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "TheftReportId" UUID NOT NULL REFERENCES "TheftReports"("Id") ON DELETE CASCADE,
    "PoliceUserId" UUID NOT NULL REFERENCES "Users"("Id"),
    "OfficerBadgeNumber" VARCHAR(50),
    "OfficerName" VARCHAR(255),
    "PoliceStation" VARCHAR(255),
    "CaseNumber" VARCHAR(100),
    "Notes" TEXT,
    "RecoveryStatus" VARCHAR(50),
    "EvidenceUrl" TEXT,
    "CreatedAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    "UpdatedAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW(),

    CONSTRAINT "CHK_Recovery_Status" CHECK ("RecoveryStatus" IN ('under_investigation', 'evidence_collected', 'recovered', 'closed'))
);

CREATE INDEX "IX_PoliceReports_TheftReportId" ON "PoliceReports"("TheftReportId");
CREATE INDEX "IX_PoliceReports_PoliceUserId" ON "PoliceReports"("PoliceUserId");
CREATE INDEX "IX_PoliceReports_CaseNumber" ON "PoliceReports"("CaseNumber");

-- ============================================================================
-- MARKETPLACE
-- ============================================================================

CREATE TABLE "MarketplaceListings" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "DeviceId" UUID NOT NULL REFERENCES "Devices"("Id") ON DELETE CASCADE,
    "SellerId" UUID NOT NULL REFERENCES "Users"("Id"),
    "Title" VARCHAR(255) NOT NULL,
    "Description" TEXT NOT NULL,
    "Price" DECIMAL(10, 2) NOT NULL,
    "Currency" VARCHAR(3) DEFAULT 'GBP',
    "Condition" VARCHAR(50),
    "Status" VARCHAR(50) DEFAULT 'active',
    "Category" VARCHAR(100),
    "Location" VARCHAR(255),
    "IsShippingAvailable" BOOLEAN DEFAULT FALSE,
    "IsFeatured" BOOLEAN DEFAULT FALSE,
    "ViewCount" INT DEFAULT 0,
    "ListedAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    "SoldAt" TIMESTAMP WITH TIME ZONE,
    "ExpiresAt" TIMESTAMP WITH TIME ZONE,
    "BuyerId" UUID REFERENCES "Users"("Id"),
    "UpdatedAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW(),

    CONSTRAINT "CHK_Condition" CHECK ("Condition" IN ('new', 'like_new', 'good', 'fair', 'poor')),
    CONSTRAINT "CHK_Listing_Status" CHECK ("Status" IN ('active', 'sold', 'expired', 'removed'))
);

CREATE INDEX "IX_MarketplaceListings_DeviceId" ON "MarketplaceListings"("DeviceId");
CREATE INDEX "IX_MarketplaceListings_SellerId" ON "MarketplaceListings"("SellerId");
CREATE INDEX "IX_MarketplaceListings_Status" ON "MarketplaceListings"("Status");
CREATE INDEX "IX_MarketplaceListings_Category" ON "MarketplaceListings"("Category");
CREATE INDEX "IX_MarketplaceListings_Price" ON "MarketplaceListings"("Price");
CREATE INDEX "IX_MarketplaceListings_ListedAt" ON "MarketplaceListings"("ListedAt" DESC);

-- ============================================================================
-- SUBSCRIPTIONS & PAYMENTS
-- ============================================================================

CREATE TABLE "Subscriptions" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "UserId" UUID NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "Plan" VARCHAR(50) NOT NULL,
    "Status" VARCHAR(50) DEFAULT 'active',
    "BillingInterval" VARCHAR(50),
    "Amount" DECIMAL(10, 2),
    "Currency" VARCHAR(3) DEFAULT 'GBP',
    "StartedAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    "CurrentPeriodStart" TIMESTAMP WITH TIME ZONE,
    "CurrentPeriodEnd" TIMESTAMP WITH TIME ZONE,
    "CancelledAt" TIMESTAMP WITH TIME ZONE,
    "EndsAt" TIMESTAMP WITH TIME ZONE,
    "TrialStartedAt" TIMESTAMP WITH TIME ZONE,
    "TrialEndsAt" TIMESTAMP WITH TIME ZONE,
    "StripeSubscriptionId" VARCHAR(255) UNIQUE,
    "StripeCustomerId" VARCHAR(255),
    "StripePriceId" VARCHAR(255),
    "CreatedAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    "UpdatedAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW(),

    CONSTRAINT "CHK_Plan" CHECK ("Plan" IN ('free', 'premium_monthly', 'premium_yearly', 'business')),
    CONSTRAINT "CHK_Subscription_Status" CHECK ("Status" IN ('active', 'cancelled', 'expired', 'past_due'))
);

CREATE INDEX "IX_Subscriptions_UserId" ON "Subscriptions"("UserId");
CREATE INDEX "IX_Subscriptions_Status" ON "Subscriptions"("Status");
CREATE INDEX "IX_Subscriptions_StripeSubscriptionId" ON "Subscriptions"("StripeSubscriptionId");
CREATE INDEX "IX_Subscriptions_CurrentPeriodEnd" ON "Subscriptions"("CurrentPeriodEnd");

CREATE TABLE "Payments" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "UserId" UUID NOT NULL REFERENCES "Users"("Id"),
    "SubscriptionId" UUID REFERENCES "Subscriptions"("Id"),
    "Amount" DECIMAL(10, 2) NOT NULL,
    "Currency" VARCHAR(3) DEFAULT 'GBP',
    "Status" VARCHAR(50) NOT NULL,
    "PaymentMethod" VARCHAR(50),
    "Description" TEXT,
    "InvoiceUrl" TEXT,
    "StripePaymentIntentId" VARCHAR(255) UNIQUE,
    "StripeInvoiceId" VARCHAR(255),
    "PaymentDate" TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    "RefundedAt" TIMESTAMP WITH TIME ZONE,
    "RefundAmount" DECIMAL(10, 2),
    "CreatedAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW(),

    CONSTRAINT "CHK_Payment_Status" CHECK ("Status" IN ('pending', 'succeeded', 'failed', 'refunded'))
);

CREATE INDEX "IX_Payments_UserId" ON "Payments"("UserId");
CREATE INDEX "IX_Payments_SubscriptionId" ON "Payments"("SubscriptionId");
CREATE INDEX "IX_Payments_Status" ON "Payments"("Status");
CREATE INDEX "IX_Payments_PaymentDate" ON "Payments"("PaymentDate" DESC);

-- ============================================================================
-- BUSINESS ACCOUNTS
-- ============================================================================

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
    "ApiKeyHash" VARCHAR(255),
    "ApiKeyCreatedAt" TIMESTAMP WITH TIME ZONE,
    "CreatedAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    "UpdatedAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

CREATE INDEX "IX_BusinessProfiles_UserId" ON "BusinessProfiles"("UserId");
CREATE INDEX "IX_BusinessProfiles_IsVerified" ON "BusinessProfiles"("IsVerified");

-- ============================================================================
-- NOTIFICATIONS
-- ============================================================================

CREATE TABLE "Notifications" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "UserId" UUID NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "Type" VARCHAR(50) NOT NULL,
    "Title" VARCHAR(255) NOT NULL,
    "Message" TEXT NOT NULL,
    "IsRead" BOOLEAN DEFAULT FALSE,
    "ReadAt" TIMESTAMP WITH TIME ZONE,
    "Channel" VARCHAR(50),
    "Priority" VARCHAR(20) DEFAULT 'normal',
    "ActionUrl" TEXT,
    "Data" JSONB,
    "CreatedAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    "ExpiresAt" TIMESTAMP WITH TIME ZONE,

    CONSTRAINT "CHK_Channel" CHECK ("Channel" IN ('push', 'email', 'sms', 'in_app')),
    CONSTRAINT "CHK_Priority" CHECK ("Priority" IN ('low', 'normal', 'high', 'urgent'))
);

CREATE INDEX "IX_Notifications_UserId" ON "Notifications"("UserId");
CREATE INDEX "IX_Notifications_IsRead" ON "Notifications"("IsRead") WHERE "IsRead" = FALSE;
CREATE INDEX "IX_Notifications_Type" ON "Notifications"("Type");
CREATE INDEX "IX_Notifications_CreatedAt" ON "Notifications"("CreatedAt" DESC);

-- ============================================================================
-- AUDIT LOGGING
-- ============================================================================

CREATE TABLE "AuditLogs" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "UserId" UUID REFERENCES "Users"("Id"),
    "Action" VARCHAR(100) NOT NULL,
    "EntityType" VARCHAR(100),
    "EntityId" UUID,
    "IpAddress" VARCHAR(50),
    "UserAgent" TEXT,
    "Location" VARCHAR(255),
    "Details" JSONB,
    "CreatedAt" TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

CREATE INDEX "IX_AuditLogs_UserId" ON "AuditLogs"("UserId");
CREATE INDEX "IX_AuditLogs_Action" ON "AuditLogs"("Action");
CREATE INDEX "IX_AuditLogs_EntityType" ON "AuditLogs"("EntityType");
CREATE INDEX "IX_AuditLogs_EntityId" ON "AuditLogs"("EntityId");
CREATE INDEX "IX_AuditLogs_CreatedAt" ON "AuditLogs"("CreatedAt" DESC);
CREATE INDEX "IX_AuditLogs_Details" ON "AuditLogs" USING GIN ("Details");

-- ============================================================================
-- OPENIDDICT TABLES (for OAuth 2.0 / OpenID Connect)
-- ============================================================================
-- These will be created by OpenIddict EF Core migrations
-- Included here for reference

-- OpenIddictApplications
-- OpenIddictAuthorizations
-- OpenIddictScopes
-- OpenIddictTokens

-- ============================================================================
-- VIEWS
-- ============================================================================

CREATE OR REPLACE VIEW "ActiveDevices" AS
SELECT
    d."Id",
    d."UserId",
    d."Category",
    d."Brand",
    d."Model",
    d."Status",
    d."IsStolen",
    d."IsLost",
    d."RegisteredAt",
    u."Email" AS "OwnerEmail",
    u."FirstName" AS "OwnerFirstName",
    u."LastName" AS "OwnerLastName"
FROM "Devices" d
INNER JOIN "Users" u ON d."UserId" = u."Id"
WHERE d."DeletedAt" IS NULL
    AND d."Status" != 'deleted';

CREATE OR REPLACE VIEW "StolenDevices" AS
SELECT
    d."Id",
    d."SerialNumberHash",
    d."Category",
    d."Brand",
    d."Model",
    d."Status",
    tr."ReportType",
    tr."IncidentDate",
    tr."IncidentLocation",
    tr."PoliceReferenceNumber",
    u."Email" AS "OwnerEmail",
    u."PhoneNumber" AS "OwnerPhone"
FROM "Devices" d
INNER JOIN "Users" u ON d."UserId" = u."Id"
INNER JOIN "TheftReports" tr ON d."Id" = tr."DeviceId"
WHERE d."IsStolen" = TRUE
    AND tr."Status" = 'active'
    AND d."DeletedAt" IS NULL;

-- ============================================================================
-- FUNCTIONS
-- ============================================================================

-- Function to update UpdatedAt timestamp
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW."UpdatedAt" = NOW();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Apply to relevant tables
CREATE TRIGGER update_users_updated_at BEFORE UPDATE ON "Users"
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_devices_updated_at BEFORE UPDATE ON "Devices"
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_subscriptions_updated_at BEFORE UPDATE ON "Subscriptions"
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_business_profiles_updated_at BEFORE UPDATE ON "BusinessProfiles"
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_police_profiles_updated_at BEFORE UPDATE ON "PoliceProfiles"
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

-- ============================================================================
-- INITIAL DATA
-- ============================================================================

-- Create default admin user (change password immediately in production!)
INSERT INTO "Users" (
    "Email",
    "PasswordHash",
    "EmailVerified",
    "FirstName",
    "LastName",
    "Role",
    "SubscriptionTier"
) VALUES (
    'admin@deviceownership.local',
    '$2a$11$encrypted_password_hash_here', -- Change this!
    TRUE,
    'System',
    'Administrator',
    'admin',
    'premium'
);

-- ============================================================================
-- PERFORMANCE OPTIMIZATION
-- ============================================================================

-- Analyze tables for query planner
ANALYZE "Users";
ANALYZE "Devices";
ANALYZE "TheftReports";
ANALYZE "OwnershipHistory";
ANALYZE "MarketplaceListings";

-- ============================================================================
-- COMMENTS FOR DOCUMENTATION
-- ============================================================================

COMMENT ON TABLE "Users" IS 'Core user accounts with authentication and profile information';
COMMENT ON TABLE "Devices" IS 'Registered devices with encrypted serial numbers';
COMMENT ON TABLE "OwnershipHistory" IS 'Complete chain of ownership for audit trail';
COMMENT ON TABLE "TheftReports" IS 'Theft and loss reports for stolen/lost devices';
COMMENT ON TABLE "MarketplaceListings" IS 'Second-hand marketplace listings';
COMMENT ON TABLE "AuditLogs" IS 'System-wide audit log for security and compliance';

COMMENT ON COLUMN "Devices"."SerialNumberHash" IS 'SHA256 hash for fast lookup without exposing serial number';
COMMENT ON COLUMN "Devices"."SerialNumberEncrypted" IS 'AES encrypted serial number, decryptable only by owner';
COMMENT ON COLUMN "Users"."PasswordHash" IS 'BCrypt hashed password (min cost factor: 11)';
