import 'package:dio/dio.dart';
import '../config/api_config.dart';
import '../models/device_model.dart';

class DeviceService {
  final Dio _dio;

  DeviceService(this._dio);

  Future<List<Device>> getUserDevices() async {
    try {
      final response = await _dio.get('${ApiConfig.baseUrl}${ApiConfig.devices}');
      return (response.data as List)
          .map((json) => Device.fromJson(json))
          .toList();
    } catch (e) {
      throw _handleError(e);
    }
  }

  Future<Device> getDevice(String id) async {
    try {
      final response = await _dio.get('${ApiConfig.baseUrl}${ApiConfig.deviceById(id)}');
      return Device.fromJson(response.data);
    } catch (e) {
      throw _handleError(e);
    }
  }

  Future<Device> registerDevice(RegisterDeviceRequest request) async {
    try {
      final response = await _dio.post(
        '${ApiConfig.baseUrl}${ApiConfig.devices}',
        data: request.toJson(),
      );
      return Device.fromJson(response.data);
    } catch (e) {
      throw _handleError(e);
    }
  }

  Future<Map<String, dynamic>> checkSerialNumber(String serialNumber) async {
    try {
      final response = await _dio.get(
        '${ApiConfig.baseUrl}${ApiConfig.checkSerialNumber(serialNumber)}',
      );
      return response.data;
    } catch (e) {
      throw _handleError(e);
    }
  }

  Future<void> deleteDevice(String id) async {
    try {
      await _dio.delete('${ApiConfig.baseUrl}${ApiConfig.deviceById(id)}');
    } catch (e) {
      throw _handleError(e);
    }
  }

  String _handleError(dynamic error) {
    if (error is DioException) {
      if (error.response != null) {
        return error.response?.data['message'] ?? 'An error occurred';
      }
      return 'Network error. Please check your connection.';
    }
    return error.toString();
  }
}
