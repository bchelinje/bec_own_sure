import 'package:flutter/foundation.dart';
import '../models/device_model.dart';
import '../services/device_service.dart';

class DeviceProvider with ChangeNotifier {
  final DeviceService _deviceService;
  List<Device> _devices = [];
  Device? _selectedDevice;
  bool _isLoading = false;
  String? _error;

  DeviceProvider(this._deviceService);

  List<Device> get devices => _devices;
  Device? get selectedDevice => _selectedDevice;
  bool get isLoading => _isLoading;
  String? get error => _error;

  int get activeDevicesCount =>
      _devices.where((d) => d.status == DeviceStatus.active).length;

  Future<void> loadDevices() async {
    try {
      _isLoading = true;
      _error = null;
      notifyListeners();

      _devices = await _deviceService.getUserDevices();
      _isLoading = false;
      notifyListeners();
    } catch (e) {
      _error = e.toString();
      _isLoading = false;
      notifyListeners();
    }
  }

  Future<void> loadDevice(String id) async {
    try {
      _isLoading = true;
      _error = null;
      notifyListeners();

      _selectedDevice = await _deviceService.getDevice(id);
      _isLoading = false;
      notifyListeners();
    } catch (e) {
      _error = e.toString();
      _isLoading = false;
      notifyListeners();
    }
  }

  Future<bool> registerDevice(RegisterDeviceRequest request) async {
    try {
      _isLoading = true;
      _error = null;
      notifyListeners();

      final device = await _deviceService.registerDevice(request);
      _devices.add(device);
      _isLoading = false;
      notifyListeners();
      return true;
    } catch (e) {
      _error = e.toString();
      _isLoading = false;
      notifyListeners();
      return false;
    }
  }

  Future<bool> deleteDevice(String id) async {
    try {
      _isLoading = true;
      _error = null;
      notifyListeners();

      await _deviceService.deleteDevice(id);
      _devices.removeWhere((d) => d.id == id);
      _isLoading = false;
      notifyListeners();
      return true;
    } catch (e) {
      _error = e.toString();
      _isLoading = false;
      notifyListeners();
      return false;
    }
  }
}
