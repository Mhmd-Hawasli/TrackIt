import 'package:track_it_health/features/auth/domain/entities/user_entity.dart';

class UserModel extends UserEntity {
  UserModel({
    required super.userId,
    required super.name,
    required super.username,
    required super.email,
  });

  @override
  Map<String, dynamic> toMap() {
    return {
      'userId': userId,
      'name': name,
      'username': username,
      'email': email,
    };
  }

  @override
  factory UserModel.fromMap(Map<String, dynamic> map) {
    return UserModel(
      userId: map['userId'] as int,
      name: map['name'] as String,
      username: map['username'] as String,
      email: map['email'] as String,
    );
  }
}
