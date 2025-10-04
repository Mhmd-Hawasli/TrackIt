import 'package:track_it_health/features/auth/domain/entities/user_entity.dart';

class UserModel extends UserEntity {
  UserModel({
    required super.userId,
    required super.name,
    required super.username,
    required super.email,
    super.backupEmail,
  });

  Map<String, dynamic> toMap() {
    return {
      'userId': this.userId,
      'name': this.name,
      'username': this.username,
      'email': this.email,
      'backupEmail': this.backupEmail,
    };
  }

  factory UserModel.fromMap(Map<String, dynamic> map) {
    return UserModel(
      userId: map['userId'] as int,
      name: map['name'] as String,
      username: map['username'] as String,
      email: map['email'] as String,
      backupEmail: map['backupEmail'] as String?,
    );
  }
}
