import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:go_router/go_router.dart';
import '../../core/providers/device_provider.dart';
import '../../core/models/device_model.dart';

class DeviceListScreen extends StatefulWidget {
  const DeviceListScreen({super.key});

  @override
  State<DeviceListScreen> createState() => _DeviceListScreenState();
}

class _DeviceListScreenState extends State<DeviceListScreen> {
  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      context.read<DeviceProvider>().loadDevices();
    });
  }

  @override
  Widget build(BuildContext context) {
    final deviceProvider = context.watch<DeviceProvider>();

    return Scaffold(
      appBar: AppBar(
        title: const Text('My Devices'),
        actions: [
          IconButton(
            icon: const Icon(Icons.add),
            onPressed: () => context.go('/devices/register'),
          ),
        ],
      ),
      body: deviceProvider.isLoading
          ? const Center(child: CircularProgressIndicator())
          : deviceProvider.devices.isEmpty
              ? Center(
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      const Icon(Icons.devices_other, size: 64, color: Colors.grey),
                      const SizedBox(height: 16),
                      const Text('No devices registered yet'),
                      const SizedBox(height: 16),
                      ElevatedButton.icon(
                        onPressed: () => context.go('/devices/register'),
                        icon: const Icon(Icons.add),
                        label: const Text('Register Your First Device'),
                      ),
                    ],
                  ),
                )
              : RefreshIndicator(
                  onRefresh: () => deviceProvider.loadDevices(),
                  child: ListView.builder(
                    padding: const EdgeInsets.all(16),
                    itemCount: deviceProvider.devices.length,
                    itemBuilder: (context, index) {
                      final device = deviceProvider.devices[index];
                      return _buildDeviceCard(context, device, deviceProvider);
                    },
                  ),
                ),
      bottomNavigationBar: _buildBottomNav(context),
    );
  }

  Widget _buildDeviceCard(
    BuildContext context,
    Device device,
    DeviceProvider provider,
  ) {
    return Card(
      margin: const EdgeInsets.only(bottom: 16),
      child: ListTile(
        contentPadding: const EdgeInsets.all(16),
        leading: CircleAvatar(
          backgroundColor: _getStatusColor(device.status),
          child: Icon(
            _getDeviceIcon(device.category),
            color: Colors.white,
          ),
        ),
        title: Text(
          device.displayName,
          style: const TextStyle(fontWeight: FontWeight.bold),
        ),
        subtitle: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const SizedBox(height: 4),
            Text('${device.category} - ${device.serialNumber}'),
            const SizedBox(height: 4),
            Chip(
              label: Text(
                device.status.name.toUpperCase(),
                style: const TextStyle(fontSize: 10),
              ),
              backgroundColor: _getStatusColor(device.status),
              padding: EdgeInsets.zero,
              visualDensity: VisualDensity.compact,
            ),
          ],
        ),
        trailing: PopupMenuButton(
          itemBuilder: (context) => [
            const PopupMenuItem(
              value: 'view',
              child: Text('View Details'),
            ),
            const PopupMenuItem(
              value: 'delete',
              child: Text('Delete'),
            ),
          ],
          onSelected: (value) async {
            if (value == 'view') {
              context.go('/devices/${device.id}');
            } else if (value == 'delete') {
              final confirm = await showDialog<bool>(
                context: context,
                builder: (context) => AlertDialog(
                  title: const Text('Delete Device'),
                  content: const Text('Are you sure you want to delete this device?'),
                  actions: [
                    TextButton(
                      onPressed: () => Navigator.pop(context, false),
                      child: const Text('Cancel'),
                    ),
                    TextButton(
                      onPressed: () => Navigator.pop(context, true),
                      child: const Text('Delete'),
                    ),
                  ],
                ),
              );
              if (confirm == true && context.mounted) {
                await provider.deleteDevice(device.id);
              }
            }
          },
        ),
        onTap: () => context.go('/devices/${device.id}'),
      ),
    );
  }

  IconData _getDeviceIcon(String category) {
    switch (category.toLowerCase()) {
      case 'smartphone':
        return Icons.smartphone;
      case 'laptop':
        return Icons.laptop;
      case 'tablet':
        return Icons.tablet;
      case 'camera':
        return Icons.camera_alt;
      case 'watch':
        return Icons.watch;
      default:
        return Icons.devices;
    }
  }

  Color _getStatusColor(DeviceStatus status) {
    switch (status) {
      case DeviceStatus.active:
        return Colors.green;
      case DeviceStatus.stolen:
        return Colors.red;
      case DeviceStatus.recovered:
        return Colors.blue;
      case DeviceStatus.forSale:
        return Colors.orange;
      case DeviceStatus.inactive:
        return Colors.grey;
    }
  }

  Widget _buildBottomNav(BuildContext context) {
    return BottomNavigationBar(
      currentIndex: 1,
      type: BottomNavigationBarType.fixed,
      items: const [
        BottomNavigationBarItem(
          icon: Icon(Icons.dashboard),
          label: 'Dashboard',
        ),
        BottomNavigationBarItem(
          icon: Icon(Icons.devices),
          label: 'Devices',
        ),
        BottomNavigationBarItem(
          icon: Icon(Icons.shopping_cart),
          label: 'Marketplace',
        ),
        BottomNavigationBarItem(
          icon: Icon(Icons.report),
          label: 'Reports',
        ),
      ],
      onTap: (index) {
        switch (index) {
          case 0:
            context.go('/dashboard');
            break;
          case 1:
            context.go('/devices');
            break;
          case 2:
            context.go('/marketplace');
            break;
          case 3:
            context.go('/reports');
            break;
        }
      },
    );
  }
}
