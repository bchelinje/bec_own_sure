import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:go_router/go_router.dart';
import 'package:dio/dio.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

import 'core/services/auth_service.dart';
import 'core/services/device_service.dart';
import 'core/providers/auth_provider.dart';
import 'core/providers/device_provider.dart';
import 'core/utils/dio_interceptor.dart';

import 'screens/auth/login_screen.dart';
import 'screens/auth/register_screen.dart';
import 'screens/dashboard/dashboard_screen.dart';
import 'screens/devices/device_list_screen.dart';
import 'screens/devices/device_register_screen.dart';

void main() {
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    // Initialize services
    const storage = FlutterSecureStorage();
    final dio = Dio();
    dio.interceptors.add(AuthInterceptor(storage));

    final authService = AuthService(dio, storage);
    final deviceService = DeviceService(dio);

    // Configure router
    final router = GoRouter(
      initialLocation: '/login',
      routes: [
        GoRoute(
          path: '/login',
          builder: (context, state) => const LoginScreen(),
        ),
        GoRoute(
          path: '/register',
          builder: (context, state) => const RegisterScreen(),
        ),
        GoRoute(
          path: '/dashboard',
          builder: (context, state) => const DashboardScreen(),
        ),
        GoRoute(
          path: '/devices',
          builder: (context, state) => const DeviceListScreen(),
        ),
        GoRoute(
          path: '/devices/register',
          builder: (context, state) => const DeviceRegisterScreen(),
        ),
        GoRoute(
          path: '/devices/:id',
          builder: (context, state) {
            // Device detail screen - to be implemented
            return Scaffold(
              appBar: AppBar(title: const Text('Device Details')),
              body: const Center(child: Text('Device detail coming soon')),
            );
          },
        ),
        GoRoute(
          path: '/marketplace',
          builder: (context, state) {
            // Marketplace screen - to be implemented
            return Scaffold(
              appBar: AppBar(title: const Text('Marketplace')),
              body: const Center(child: Text('Marketplace coming soon')),
            );
          },
        ),
        GoRoute(
          path: '/reports',
          builder: (context, state) {
            // Reports screen - to be implemented
            return Scaffold(
              appBar: AppBar(title: const Text('Reports')),
              body: const Center(child: Text('Reports coming soon')),
            );
          },
        ),
      ],
    );

    return MultiProvider(
      providers: [
        ChangeNotifierProvider(
          create: (_) => AuthProvider(authService)..loadUser(),
        ),
        ChangeNotifierProvider(
          create: (_) => DeviceProvider(deviceService),
        ),
      ],
      child: MaterialApp.router(
        title: 'Device Ownership',
        theme: ThemeData(
          primarySwatch: Colors.blue,
          useMaterial3: true,
        ),
        routerConfig: router,
      ),
    );
  }
}
