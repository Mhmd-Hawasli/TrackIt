part of 'auth_bloc.dart';

@immutable
sealed class AuthEvent {}

final class AuthSignUp extends AuthEvent {
  final String name;
  final String username;
  final String email;
  final String password;

  AuthSignUp({
    required this.name,
    required this.username,
    required this.email,
    required this.password,
  });

  Map<String, dynamic> toMap() {
    return {
      'name': this.name,
      'username': this.username,
      'email': this.email,
      'password': this.password,
    };
  }

  factory AuthSignUp.fromMap(Map<String, dynamic> map) {
    return AuthSignUp(
      name: map['name'] as String,
      username: map['username'] as String,
      email: map['email'] as String,
      password: map['password'] as String,
    );
  }
}
