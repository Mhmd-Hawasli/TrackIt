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
}
