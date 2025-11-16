# 🚀 Deployment Guide

## Azure Deployment (Recommended for Production)

### Prerequisites

- Azure subscription
- Azure CLI installed
- .NET 8 SDK
- Docker (optional, for container deployment)

---

## 📋 Pre-Deployment Checklist

- [ ] All tests passing
- [ ] Security audit completed
- [ ] Environment variables configured
- [ ] Database backup created
- [ ] SSL certificates obtained
- [ ] Domain DNS configured
- [ ] Monitoring setup completed
- [ ] Load testing completed

---

## 🏗️ Infrastructure Setup

### Option 1: Azure CLI

```bash
# Login to Azure
az login

# Create resource group
az group create \
  --name rg-deviceownership-prod \
  --location ukwest

# Create App Service Plan
az appservice plan create \
  --name plan-deviceownership-prod \
  --resource-group rg-deviceownership-prod \
  --sku P1V3 \
  --is-linux

# Create Web App
az webapp create \
  --name app-deviceownership-api-prod \
  --resource-group rg-deviceownership-prod \
  --plan plan-deviceownership-prod \
  --runtime "DOTNET|8.0"

# Create PostgreSQL Database
az postgres flexible-server create \
  --name psql-deviceownership-prod \
  --resource-group rg-deviceownership-prod \
  --location ukwest \
  --admin-user dbadmin \
  --admin-password <secure-password> \
  --sku-name Standard_D4s_v3 \
  --tier GeneralPurpose \
  --storage-size 128 \
  --version 16

# Create database
az postgres flexible-server db create \
  --resource-group rg-deviceownership-prod \
  --server-name psql-deviceownership-prod \
  --database-name DeviceOwnership

# Create Redis Cache
az redis create \
  --name redis-deviceownership-prod \
  --resource-group rg-deviceownership-prod \
  --location ukwest \
  --sku Premium \
  --vm-size P1

# Create Storage Account
az storage account create \
  --name stdeviceownershipprod \
  --resource-group rg-deviceownership-prod \
  --location ukwest \
  --sku Standard_LRS

# Create Blob Container
az storage container create \
  --name device-photos \
  --account-name stdeviceownershipprod

az storage container create \
  --name device-documents \
  --account-name stdeviceownershipprod

# Create Key Vault
az keyvault create \
  --name kv-deviceownership-prod \
  --resource-group rg-deviceownership-prod \
  --location ukwest

# Create Application Insights
az monitor app-insights component create \
  --app ai-deviceownership-prod \
  --location ukwest \
  --resource-group rg-deviceownership-prod
```

---

### Option 2: Bicep Template

Create `infrastructure/azure/bicep/main.bicep`:

```bicep
param location string = 'ukwest'
param appName string = 'deviceownership'
param environment string = 'prod'

var resourceGroupName = 'rg-${appName}-${environment}'
var appServicePlanName = 'plan-${appName}-${environment}'
var webAppName = 'app-${appName}-api-${environment}'
var dbServerName = 'psql-${appName}-${environment}'
var redisCacheName = 'redis-${appName}-${environment}'
var storageAccountName = 'st${appName}${environment}'
var keyVaultName = 'kv-${appName}-${environment}'

resource appServicePlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: appServicePlanName
  location: location
  sku: {
    name: 'P1V3'
    tier: 'PremiumV3'
  }
  kind: 'linux'
  properties: {
    reserved: true
  }
}

resource webApp 'Microsoft.Web/sites@2022-03-01' = {
  name: webAppName
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      linuxFxVersion: 'DOTNET|8.0'
      alwaysOn: true
      http20Enabled: true
      minTlsVersion: '1.2'
      ftpsState: 'Disabled'
    }
    httpsOnly: true
  }
}

// Add more resources...
```

Deploy:

```bash
az deployment group create \
  --resource-group rg-deviceownership-prod \
  --template-file infrastructure/azure/bicep/main.bicep
```

---

## 🔐 Security Configuration

### 1. Configure Key Vault

```bash
# Add secrets to Key Vault
az keyvault secret set \
  --vault-name kv-deviceownership-prod \
  --name "ConnectionStrings--DefaultConnection" \
  --value "Host=psql-deviceownership-prod.postgres.database.azure.com;Database=DeviceOwnership;Username=dbadmin;Password=<password>"

az keyvault secret set \
  --vault-name kv-deviceownership-prod \
  --name "Jwt--SigningKey" \
  --value "<generate-secure-key>"

az keyvault secret set \
  --vault-name kv-deviceownership-prod \
  --name "Email--ApiKey" \
  --value "<sendgrid-api-key>"

az keyvault secret set \
  --vault-name kv-deviceownership-prod \
  --name "Stripe--SecretKey" \
  --value "<stripe-secret-key>"
```

### 2. Enable Managed Identity

```bash
# Enable system-assigned managed identity for Web App
az webapp identity assign \
  --name app-deviceownership-api-prod \
  --resource-group rg-deviceownership-prod

# Grant Key Vault access
az keyvault set-policy \
  --name kv-deviceownership-prod \
  --object-id <managed-identity-object-id> \
  --secret-permissions get list
```

### 3. Configure App Settings

```bash
az webapp config appsettings set \
  --name app-deviceownership-api-prod \
  --resource-group rg-deviceownership-prod \
  --settings \
    "ASPNETCORE_ENVIRONMENT=Production" \
    "KeyVault__VaultUri=https://kv-deviceownership-prod.vault.azure.net/" \
    "ApplicationInsights__ConnectionString=<connection-string>"
```

---

## 📦 Backend Deployment

### Option 1: GitHub Actions (Recommended)

Create `.github/workflows/backend-cd.yml`:

```yaml
name: Backend CD

on:
  push:
    branches:
      - main
    paths:
      - 'backend/**'

jobs:
  deploy:
    runs-on: ubuntu-latest

    steps:
      - uses: actions/checkout@v3

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'

      - name: Restore dependencies
        run: dotnet restore
        working-directory: ./backend

      - name: Build
        run: dotnet build --configuration Release --no-restore
        working-directory: ./backend

      - name: Test
        run: dotnet test --no-build --verbosity normal
        working-directory: ./backend

      - name: Publish
        run: dotnet publish src/DeviceOwnership.API/DeviceOwnership.API.csproj -c Release -o ./publish
        working-directory: ./backend

      - name: Deploy to Azure Web App
        uses: azure/webapps-deploy@v2
        with:
          app-name: 'app-deviceownership-api-prod'
          publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
          package: ./backend/publish

      - name: Run Database Migrations
        run: |
          dotnet tool install --global dotnet-ef
          dotnet ef database update --project src/DeviceOwnership.Infrastructure
        working-directory: ./backend
        env:
          ConnectionStrings__DefaultConnection: ${{ secrets.DATABASE_CONNECTION_STRING }}
```

### Option 2: Manual Deployment

```bash
# Build and publish
cd backend
dotnet publish src/DeviceOwnership.API/DeviceOwnership.API.csproj \
  -c Release \
  -o ./publish

# Create deployment package
cd publish
zip -r ../deploy.zip .

# Deploy to Azure
az webapp deployment source config-zip \
  --resource-group rg-deviceownership-prod \
  --name app-deviceownership-api-prod \
  --src ../deploy.zip

# Run migrations
dotnet ef database update \
  --project src/DeviceOwnership.Infrastructure \
  --connection "<connection-string>"
```

---

## 🌐 Frontend Deployment

### Azure Static Web Apps

```bash
# Create Static Web App
az staticwebapp create \
  --name swa-deviceownership-prod \
  --resource-group rg-deviceownership-prod \
  --location ukwest

# Build Angular app
cd frontend-web
npm run build:production

# Deploy
az staticwebapp deploy \
  --app-name swa-deviceownership-prod \
  --resource-group rg-deviceownership-prod \
  --source-location ./dist/device-ownership-web
```

### GitHub Actions for Frontend

Create `.github/workflows/frontend-cd.yml`:

```yaml
name: Frontend CD

on:
  push:
    branches:
      - main
    paths:
      - 'frontend-web/**'

jobs:
  deploy:
    runs-on: ubuntu-latest

    steps:
      - uses: actions/checkout@v3

      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: '20'

      - name: Install dependencies
        run: npm ci
        working-directory: ./frontend-web

      - name: Build
        run: npm run build:production
        working-directory: ./frontend-web
        env:
          API_URL: https://api.deviceownership.com
          OAUTH_CLIENT_ID: web_app

      - name: Deploy to Azure Static Web Apps
        uses: Azure/static-web-apps-deploy@v1
        with:
          azure_static_web_apps_api_token: ${{ secrets.AZURE_STATIC_WEB_APPS_API_TOKEN }}
          repo_token: ${{ secrets.GITHUB_TOKEN }}
          action: "upload"
          app_location: "/frontend-web"
          output_location: "dist/device-ownership-web"
```

---

## 📱 Mobile App Deployment

### iOS App Store

```bash
# Build iOS release
cd mobile-app
flutter build ios --release

# Open Xcode
open ios/Runner.xcworkspace

# In Xcode:
# 1. Select "Any iOS Device" as target
# 2. Product > Archive
# 3. Distribute App > App Store Connect
# 4. Upload
```

### Android Play Store

```bash
# Build Android release
cd mobile-app
flutter build appbundle --release

# Upload to Play Console
# 1. Go to https://play.google.com/console
# 2. Select app
# 3. Production > Create new release
# 4. Upload build/app/outputs/bundle/release/app-release.aab
```

---

## 🗄️ Database Migration

### Production Migration Checklist

- [ ] Backup current database
- [ ] Test migration on staging
- [ ] Review migration scripts
- [ ] Schedule maintenance window
- [ ] Notify users
- [ ] Monitor after deployment

### Safe Migration Process

```bash
# 1. Backup database
az postgres flexible-server backup create \
  --resource-group rg-deviceownership-prod \
  --server-name psql-deviceownership-prod \
  --backup-name pre-deployment-$(date +%Y%m%d)

# 2. Generate migration SQL script (dry run)
dotnet ef migrations script \
  --project src/DeviceOwnership.Infrastructure \
  --output migration.sql

# 3. Review script carefully

# 4. Apply migration
dotnet ef database update \
  --project src/DeviceOwnership.Infrastructure \
  --connection "<connection-string>"

# 5. Verify migration
psql -h psql-deviceownership-prod.postgres.database.azure.com \
  -U dbadmin \
  -d DeviceOwnership \
  -c "SELECT * FROM __EFMigrationsHistory;"
```

---

## 🔄 Blue-Green Deployment

### Setup

```bash
# Create staging slot
az webapp deployment slot create \
  --name app-deviceownership-api-prod \
  --resource-group rg-deviceownership-prod \
  --slot staging

# Deploy to staging
az webapp deployment source config-zip \
  --resource-group rg-deviceownership-prod \
  --name app-deviceownership-api-prod \
  --slot staging \
  --src deploy.zip

# Test staging slot
curl https://app-deviceownership-api-prod-staging.azurewebsites.net/health

# Swap staging to production
az webapp deployment slot swap \
  --resource-group rg-deviceownership-prod \
  --name app-deviceownership-api-prod \
  --slot staging \
  --target-slot production
```

---

## 📊 Monitoring Setup

### Application Insights

```bash
# Get instrumentation key
az monitor app-insights component show \
  --app ai-deviceownership-prod \
  --resource-group rg-deviceownership-prod \
  --query instrumentationKey

# Configure in app settings
az webapp config appsettings set \
  --name app-deviceownership-api-prod \
  --resource-group rg-deviceownership-prod \
  --settings "ApplicationInsights__InstrumentationKey=<key>"
```

### Alerts

```bash
# Create alert for high error rate
az monitor metrics alert create \
  --name "High Error Rate" \
  --resource-group rg-deviceownership-prod \
  --scopes /subscriptions/<sub-id>/resourceGroups/rg-deviceownership-prod/providers/Microsoft.Web/sites/app-deviceownership-api-prod \
  --condition "avg requests/failed > 10" \
  --window-size 5m \
  --evaluation-frequency 1m \
  --action email <your-email>

# Create alert for high response time
az monitor metrics alert create \
  --name "High Response Time" \
  --resource-group rg-deviceownership-prod \
  --scopes /subscriptions/<sub-id>/resourceGroups/rg-deviceownership-prod/providers/Microsoft.Web/sites/app-deviceownership-api-prod \
  --condition "avg requests/duration > 1000" \
  --window-size 5m \
  --evaluation-frequency 1m
```

---

## 🔒 SSL/TLS Configuration

### Custom Domain

```bash
# Add custom domain
az webapp config hostname add \
  --webapp-name app-deviceownership-api-prod \
  --resource-group rg-deviceownership-prod \
  --hostname api.deviceownership.com

# Bind SSL certificate (managed certificate)
az webapp config ssl create \
  --name app-deviceownership-api-prod \
  --resource-group rg-deviceownership-prod \
  --hostname api.deviceownership.com

# Enforce HTTPS
az webapp update \
  --name app-deviceownership-api-prod \
  --resource-group rg-deviceownership-prod \
  --https-only true
```

---

## 🔄 Backup Strategy

### Automated Backups

```bash
# Configure database backup
az postgres flexible-server backup create \
  --resource-group rg-deviceownership-prod \
  --server-name psql-deviceownership-prod \
  --backup-name automated-backup

# Configure blob storage backup
az backup protection enable-for-azurefileshare \
  --resource-group rg-deviceownership-prod \
  --vault-name BackupVault \
  --storage-account stdeviceownershipprod \
  --azure-file-share device-photos \
  --policy-name DailyBackup
```

---

## 📈 Scaling Configuration

### Auto-scaling Rules

```bash
# Add auto-scale rule
az monitor autoscale create \
  --resource-group rg-deviceownership-prod \
  --resource app-deviceownership-api-prod \
  --resource-type Microsoft.Web/serverfarms \
  --name autoscale-deviceownership \
  --min-count 2 \
  --max-count 10 \
  --count 2

# Add scale-out rule (CPU > 70%)
az monitor autoscale rule create \
  --resource-group rg-deviceownership-prod \
  --autoscale-name autoscale-deviceownership \
  --condition "Percentage CPU > 70 avg 5m" \
  --scale out 1

# Add scale-in rule (CPU < 30%)
az monitor autoscale rule create \
  --resource-group rg-deviceownership-prod \
  --autoscale-name autoscale-deviceownership \
  --condition "Percentage CPU < 30 avg 5m" \
  --scale in 1
```

---

## 🧹 Post-Deployment

### Verification Checklist

- [ ] Health check endpoint responding
- [ ] API endpoints accessible
- [ ] Database migrations applied
- [ ] SSL certificate valid
- [ ] Monitoring alerts configured
- [ ] Application Insights receiving data
- [ ] Error rates normal
- [ ] Response times acceptable
- [ ] Mobile apps connecting successfully
- [ ] Email/SMS notifications working

### Smoke Tests

```bash
# Health check
curl https://api.deviceownership.com/health

# API endpoint test
curl https://api.deviceownership.com/api/v1/devices/categories

# Authentication test (should return 401)
curl https://api.deviceownership.com/api/v1/devices

# Load test with Artillery
npm install -g artillery
artillery quick --count 100 --num 10 https://api.deviceownership.com/health
```

---

## 🆘 Rollback Procedure

### Quick Rollback

```bash
# Swap slots back
az webapp deployment slot swap \
  --resource-group rg-deviceownership-prod \
  --name app-deviceownership-api-prod \
  --slot production \
  --target-slot staging

# Or redeploy previous version
az webapp deployment source config-zip \
  --resource-group rg-deviceownership-prod \
  --name app-deviceownership-api-prod \
  --src previous-version.zip
```

### Database Rollback

```bash
# Restore from backup
az postgres flexible-server restore \
  --resource-group rg-deviceownership-prod \
  --name psql-deviceownership-prod-restored \
  --source-server psql-deviceownership-prod \
  --restore-time "2024-01-01T12:00:00Z"
```

---

## 📞 Support Contacts

- **DevOps Team:** devops@deviceownership.com
- **On-Call:** +44 xxx xxx xxxx
- **Azure Support:** Azure Portal > Support

---

## 🔗 Useful Links

- [Azure Portal](https://portal.azure.com)
- [Application Insights Dashboard](https://portal.azure.com/#blade/HubsExtension/BrowseResource/resourceType/microsoft.insights%2Fcomponents)
- [GitHub Actions](https://github.com/your-org/device-ownership-platform/actions)
- [API Documentation](https://api.deviceownership.com/swagger)

---

This deployment guide ensures a reliable, secure, and scalable production environment.
