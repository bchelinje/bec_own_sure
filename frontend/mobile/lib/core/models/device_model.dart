import 'package:json_annotation/json_annotation.dart';

part 'device_model.g.dart';

enum DeviceStatus {
  @JsonValue('Active')
  active,
  @JsonValue('Stolen')
  stolen,
  @JsonValue('Recovered')
  recovered,
  @JsonValue('ForSale')
  forSale,
  @JsonValue('Inactive')
  inactive,
}

@JsonSerializable()
class Device {
  final String id;
  final String serialNumber;
  final String category;
  final String? brand;
  final String? model;
  final String? description;
  final DeviceStatus status;
  final String userId;
  final String verificationCode;
  final bool isVerified;
  final DateTime registeredAt;
  final DateTime lastUpdatedAt;

  Device({
    required this.id,
    required this.serialNumber,
    required this.category,
    this.brand,
    this.model,
    this.description,
    required this.status,
    required this.userId,
    required this.verificationCode,
    required this.isVerified,
    required this.registeredAt,
    required this.lastUpdatedAt,
  });

  factory Device.fromJson(Map<String, dynamic> json) => _$DeviceFromJson(json);
  Map<String, dynamic> toJson() => _$DeviceToJson(this);

  String get displayName => '${brand ?? ''} ${model ?? category}'.trim();
}

@JsonSerializable()
class RegisterDeviceRequest {
  final String serialNumber;
  final String category;
  final String? brand;
  final String? model;
  final String? description;
  final DateTime? purchaseDate;
  final double? purchasePrice;
  final String? retailer;

  RegisterDeviceRequest({
    required this.serialNumber,
    required this.category,
    this.brand,
    this.model,
    this.description,
    this.purchaseDate,
    this.purchasePrice,
    this.retailer,
  });

  Map<String, dynamic> toJson() => _$RegisterDeviceRequestToJson(this);
}
