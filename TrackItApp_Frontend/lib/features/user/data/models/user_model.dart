import 'dart:ui';

import 'package:track_it_health/core/common/entities/user_entity.dart';

class UserModel extends UserEntity {
  UserModel({
    required super.userId,
    required super.name,
    required super.username,
    required super.email,
  });

  Map<String, dynamic> toJson() {
    return {
      'userId': userId,
      'name': name,
      'username': username,
      'email': email,
    };
  }

  factory UserModel.fromJson(Map<String, dynamic> map) {
    return UserModel(
      userId: map['userId'] as int ?? 0,
      name: map['name'] as String ?? '',
      username: map['username'] as String ?? '',
      email: map['email'] as String ?? '',
    );
  }

  UserEntity toEntity() =>
      UserEntity(userId: userId, name: name, username: username, email: email);
}
