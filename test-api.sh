#!/bin/bash

# Color codes for output
GREEN='\033[0;32m'
BLUE='\033[0;34m'
RED='\033[0;31m'
NC='\033[0m' # No Color

echo -e "${BLUE}=== Device Ownership Platform - Quick Test Script ===${NC}\n"

API_URL="http://localhost:5000/api/v1"

# Check if API is running
echo -e "${BLUE}1. Checking if API is running...${NC}"
if curl -s "${API_URL}/../health" > /dev/null; then
    echo -e "${GREEN}✓ API is running${NC}\n"
else
    echo -e "${RED}✗ API is not running. Please start it first:${NC}"
    echo -e "  cd backend && ASPNETCORE_ENVIRONMENT=Development dotnet run --project src/DeviceOwnership.API/DeviceOwnership.API.csproj\n"
    exit 1
fi

# Register a test user
echo -e "${BLUE}2. Registering test user...${NC}"
REGISTER_RESPONSE=$(curl -s -X POST "${API_URL}/auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "testuser@example.com",
    "password": "Test123!",
    "firstName": "Test",
    "lastName": "User"
  }')

if echo "$REGISTER_RESPONSE" | grep -q "accessToken"; then
    echo -e "${GREEN}✓ User registered successfully${NC}\n"
    ACCESS_TOKEN=$(echo "$REGISTER_RESPONSE" | grep -o '"accessToken":"[^"]*' | cut -d'"' -f4)
else
    echo -e "${BLUE}User might already exist, trying to login...${NC}\n"

    # Try to login
    LOGIN_RESPONSE=$(curl -s -X POST "${API_URL}/auth/login" \
      -H "Content-Type: application/json" \
      -d '{
        "email": "testuser@example.com",
        "password": "Test123!"
      }')

    ACCESS_TOKEN=$(echo "$LOGIN_RESPONSE" | grep -o '"accessToken":"[^"]*' | cut -d'"' -f4)
fi

if [ -z "$ACCESS_TOKEN" ]; then
    echo -e "${RED}✗ Failed to get access token${NC}\n"
    exit 1
fi

echo -e "${GREEN}✓ Got access token: ${ACCESS_TOKEN:0:50}...${NC}\n"

# Register a device
echo -e "${BLUE}3. Registering a device...${NC}"
DEVICE_RESPONSE=$(curl -s -X POST "${API_URL}/devices" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer ${ACCESS_TOKEN}" \
  -d '{
    "serialNumber": "TEST-'$(date +%s)'",
    "category": "Smartphone",
    "brand": "Apple",
    "model": "iPhone 15 Pro",
    "description": "Test device"
  }')

if echo "$DEVICE_RESPONSE" | grep -q "id"; then
    echo -e "${GREEN}✓ Device registered successfully${NC}\n"
    DEVICE_ID=$(echo "$DEVICE_RESPONSE" | grep -o '"id":"[^"]*' | cut -d'"' -f4)
else
    echo -e "${RED}✗ Failed to register device${NC}"
    echo "$DEVICE_RESPONSE\n"
fi

# Get user devices
echo -e "${BLUE}4. Fetching user devices...${NC}"
DEVICES_RESPONSE=$(curl -s -X GET "${API_URL}/devices" \
  -H "Authorization: Bearer ${ACCESS_TOKEN}")

DEVICE_COUNT=$(echo "$DEVICES_RESPONSE" | grep -o '"id"' | wc -l)
echo -e "${GREEN}✓ Found ${DEVICE_COUNT} device(s)${NC}\n"

# Create marketplace listing
if [ ! -z "$DEVICE_ID" ]; then
    echo -e "${BLUE}5. Creating marketplace listing...${NC}"
    LISTING_RESPONSE=$(curl -s -X POST "${API_URL}/marketplace" \
      -H "Content-Type: application/json" \
      -H "Authorization: Bearer ${ACCESS_TOKEN}" \
      -d '{
        "deviceId": "'${DEVICE_ID}'",
        "title": "iPhone 15 Pro - Excellent Condition",
        "description": "Test listing for iPhone 15 Pro",
        "price": 899.99,
        "currency": "GBP",
        "condition": "like_new",
        "category": "Smartphone",
        "location": "London",
        "isShippingAvailable": true
      }')

    if echo "$LISTING_RESPONSE" | grep -q "id"; then
        echo -e "${GREEN}✓ Marketplace listing created${NC}\n"
        LISTING_ID=$(echo "$LISTING_RESPONSE" | grep -o '"id":"[^"]*' | cut -d'"' -f4)
    else
        echo -e "${RED}✗ Failed to create listing${NC}\n"
    fi
fi

# Browse marketplace
echo -e "${BLUE}6. Browsing marketplace...${NC}"
MARKETPLACE_RESPONSE=$(curl -s -X GET "${API_URL}/marketplace")
LISTING_COUNT=$(echo "$MARKETPLACE_RESPONSE" | grep -o '"id"' | wc -l)
echo -e "${GREEN}✓ Found ${LISTING_COUNT} listing(s) in marketplace${NC}\n"

echo -e "${GREEN}=== All tests completed! ===${NC}\n"
echo -e "${BLUE}Access Token (use in Swagger):${NC}"
echo -e "Bearer ${ACCESS_TOKEN}\n"

echo -e "${BLUE}Quick Links:${NC}"
echo -e "Swagger UI: http://localhost:5000/swagger"
echo -e "Health Check: http://localhost:5000/health"
echo -e "Angular App: http://localhost:4200 (if running)\n"
