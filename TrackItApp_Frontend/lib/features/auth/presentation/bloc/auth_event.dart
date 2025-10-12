part of 'auth_bloc.dart';

@immutable
sealed class AuthEvent {}

final class AuthSignUpEvent extends AuthEvent {
  final UserSignUpParams userSignUpParams;

  AuthSignUpEvent({required this.userSignUpParams});
}

final class AuthLoginEvent extends AuthEvent {
  final UserLoginParams userLoginParams;

  AuthLoginEvent({required this.userLoginParams});
}

final class AuthVerifyAccountEvent extends AuthEvent {
  final VerifyAccountParams verifyAccountParams;

  AuthVerifyAccountEvent({required this.verifyAccountParams});
}

final class AuthForgetPasswordEvent extends AuthEvent {}

final class AuthVerifyWithBackupEmailEvent extends AuthEvent {}
