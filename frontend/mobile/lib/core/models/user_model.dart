import 'package:json_annotation/json_annotation.dart';

part 'user_model.g.dart';

enum UserRole {
  @JsonValue('User')
  user,
  @JsonValue('Police')
  police,
  @JsonValue('Business')
  business,
  @JsonValue('Admin')
  admin,
}

enum SubscriptionTier {
  @JsonValue('Free')
  free,
  @JsonValue('Basic')
  basic,
  @JsonValue('Premium')
  premium,
  @JsonValue('Enterprise')
  enterprise,
}

@JsonSerializable()
class User {
  final String id;
  final String email;
  final String? firstName;
  final String? lastName;
  final String? phoneNumber;
  final UserRole role;
  final SubscriptionTier subscriptionTier;
  final bool isEmailVerified;
  final bool isPhoneVerified;
  final String? profilePhotoUrl;
  final DateTime createdAt;
  final DateTime updatedAt;

  User({
    required this.id,
    required this.email,
    this.firstName,
    this.lastName,
    this.phoneNumber,
    required this.role,
    required this.subscriptionTier,
    required this.isEmailVerified,
    required this.isPhoneVerified,
    this.profilePhotoUrl,
    required this.createdAt,
    required this.updatedAt,
  });

  factory User.fromJson(Map<String, dynamic> json) => _$UserFromJson(json);
  Map<String, dynamic> toJson() => _$UserToJson(this);

  String get displayName {
    if (firstName != null && lastName != null) {
      return '$firstName $lastName';
    }
    return email;
  }
}
