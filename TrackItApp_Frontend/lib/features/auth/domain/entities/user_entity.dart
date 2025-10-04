class UserEntity {
  final int userId;
  final String name;
  final String username;
  final String email;
  final String? backupEmail;

  const UserEntity({
    required this.userId,
    required this.name,
    required this.username,
    required this.email,
    this.backupEmail,
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

  factory UserEntity.fromMap(Map<String, dynamic> map) {
    return UserEntity(
      userId: map['userId'] as int,
      name: map['name'] as String,
      username: map['username'] as String,
      email: map['email'] as String,
      backupEmail: map['backupEmail'] as String?,
    );
  }
}
