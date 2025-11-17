# Device Ownership Mobile App

Flutter mobile application for the Device Ownership & Anti-Theft Platform.

## Features

- User authentication (login/register)
- Device registration and management
- Serial number verification
- Marketplace for buying/selling devices
- Theft reporting
- QR code generation for devices

## Getting Started

### Prerequisites

- Flutter SDK (3.0+)
- Dart SDK (3.0+)
- Android Studio or Xcode for mobile development

### Installation

1. Install dependencies:
```bash
flutter pub get
```

2. Generate JSON serialization code:
```bash
flutter pub run build_runner build
```

3. Run the app:
```bash
flutter run
```

## Project Structure

```
lib/
├── core/
│   ├── config/         # API configuration
│   ├── models/         # Data models
│   ├── providers/      # State management (Provider)
│   ├── services/       # API services
│   └── utils/          # Utilities
└── screens/
    ├── auth/           # Authentication screens
    ├── dashboard/      # Dashboard screen
    ├── devices/        # Device management screens
    ├── marketplace/    # Marketplace screens
    └── reports/        # Theft report screens
```

## Building for Production

### Android
```bash
flutter build apk --release
```

### iOS
```bash
flutter build ios --release
```

## State Management

This app uses the Provider package for state management.

## API Integration

The app connects to the ASP.NET Core backend API. Update the base URL in `lib/core/config/api_config.dart`.
