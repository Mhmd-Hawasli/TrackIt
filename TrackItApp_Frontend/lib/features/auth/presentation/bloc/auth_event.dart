part of 'auth_bloc.dart';

@immutable
sealed class AuthEvent {}

/// User registration
final class AuthSignUpEvent extends AuthEvent {
  final UserSignUpParams userSignUpParams;

  AuthSignUpEvent({required this.userSignUpParams});
}

/// User login
final class AuthLoginEvent extends AuthEvent {
  final UserLoginParams userLoginParams;

  AuthLoginEvent({required this.userLoginParams});
}

/// Verify account after sign-up
final class AuthVerifyAccountEvent extends AuthEvent {
  final VerifyAccountParams verifyAccountParams;

  AuthVerifyAccountEvent({required this.verifyAccountParams});
}

/// Load tokens from secure storage
final class AuthLoadTokensEvent extends AuthEvent {}

/// Refresh access token when expired
final class AuthRefreshTokenEvent extends AuthEvent {}
