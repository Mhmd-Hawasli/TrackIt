part of 'auth_bloc.dart';

@immutable
sealed class AuthState extends Equatable {
  const AuthState();
}

final class AuthInitialState extends AuthState {
  @override
  List<Object?> get props => [];
}

final class AuthLoadingState extends AuthState {
  @override
  List<Object?> get props => [];
}

final class AuthSuccessState extends AuthState {
  final TokenEntity tokenEntity;

  const AuthSuccessState(this.tokenEntity);

  @override
  List<Object?> get props => [
    tokenEntity.accessToken,
    tokenEntity.refreshToken,
  ];
}

final class AuthFailureState extends AuthState {
  final String message;

  const AuthFailureState(this.message);

  @override
  List<Object?> get props => [message];
}

final class AuthNeedVerifyState extends AuthState {
  final String input;
  final String message;

  const AuthNeedVerifyState(this.input, this.message);

  @override
  List<Object?> get props => [input, message];
}

final class AuthSuccessResetPasswordState extends AuthState {
  @override
  List<Object?> get props => [];
}
