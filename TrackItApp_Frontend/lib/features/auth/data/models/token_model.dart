import 'package:track_it_health/features/auth/domain/entities/token_entity.dart';

class TokenModel extends TokenEntity {
  const TokenModel({required super.accessToken, required super.refreshToken});

  Map<String, dynamic> toJson() {
    return {
      'accessToken': super.accessToken,
      'refreshToken': super.refreshToken,
    };
  }

  factory TokenModel.fromJson(Map<String, dynamic> map) {
    return TokenModel(
      accessToken: map['accessToken'] as String,
      refreshToken: map['refreshToken'] as String,
    );
  }

  TokenEntity toEntity() =>
      TokenEntity(accessToken: accessToken, refreshToken: refreshToken);
}
