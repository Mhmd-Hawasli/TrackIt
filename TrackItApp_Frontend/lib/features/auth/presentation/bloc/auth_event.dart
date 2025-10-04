part of 'auth_bloc.dart';

@immutable
sealed class AuthEvent {}

final class AuthSignUpEvent extends AuthEvent {
  final UserSignUpParams userSignUpParams;

  AuthSignUpEvent({required this.userSignUpParams});
}
