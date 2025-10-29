part of 'app_user_bloc.dart';

@immutable
sealed class AppUserState extends Equatable {}

/// Initial state
final class AppUserInitial extends AppUserState {
  @override
  List<Object?> get props => [];
}

/// Loading state for async operations
final class AppUserLoading extends AppUserState {
  @override
  List<Object?> get props => [];
}

/// User info loaded successfully
final class AppUserLoaded extends AppUserState {
  final UserEntity user;

  AppUserLoaded(this.user);

  @override
  List<Object?> get props => [user];
}

/// Error state
final class AppUserError extends AppUserState {
  final String message;

  AppUserError(this.message);

  @override
  List<Object?> get props => [message];
}

/// User logged out
final class AppUserUnauthenticated extends AppUserState {
  @override
  List<Object?> get props => [];
}
