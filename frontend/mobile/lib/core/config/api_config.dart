class ApiConfig {
  static const String baseUrl = 'http://localhost:5000/api/v1';

  // Auth endpoints
  static const String login = '/auth/login';
  static const String register = '/auth/register';
  static const String refreshToken = '/auth/refresh';

  // Device endpoints
  static const String devices = '/devices';
  static String deviceById(String id) => '/devices/$id';
  static String checkSerialNumber(String serial) => '/devices/check/$serial';

  // Marketplace endpoints
  static const String marketplace = '/marketplace';
  static String marketplaceById(String id) => '/marketplace/$id';
  static const String myListings = '/marketplace/my-listings';

  // Theft report endpoints
  static const String theftReports = '/reports';
  static String theftReportById(String id) => '/reports/$id';

  // OAuth config
  static const String clientId = 'mobile-client';
  static const List<String> scopes = [
    'openid',
    'profile',
    'email',
    'device',
    'marketplace'
  ];
}
