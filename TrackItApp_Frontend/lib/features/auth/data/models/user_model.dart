import 'dart:ui';

import 'package:track_it_health/common/entities/user_entity.dart';

class UserModel extends UserEntity {
  UserModel({
    required super.userId,
    required super.name,
    required super.username,
    required super.email,
    super.backupEmail,
  });

  Map<String, dynamic> toJson() {
    return {
      'userId': userId,
      'name': name,
      'username': username,
      'email': email,
      'backupEmail': backupEmail,
    };
  }

  factory UserModel.fromJson(Map<String, dynamic> map) {
    return UserModel(
      userId: map['userId'] as int ?? 0,
      name: map['name'] as String ?? '',
      username: map['username'] as String ?? '',
      email: map['email'] as String ?? '',
      backupEmail: map['backupEmail'] as String?,
    );
  }

  UserEntity toEntity() => UserEntity(
    userId: userId,
    name: name,
    username: username,
    email: email,
    backupEmail: backupEmail,
  );
}
