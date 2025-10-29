part of 'app_user_bloc.dart';

@immutable
sealed class AppUserEvent extends Equatable {}

/// Save user info to secure storage
final class AppUserSaveToStorage extends AppUserEvent {
  final UserEntity userEntity;

  AppUserSaveToStorage(this.userEntity);

  @override
  List<Object?> get props => [userEntity];
}

/// Load user info from secure storage
final class AppUserStorageLoaded extends AppUserEvent {
  @override
  List<Object?> get props => [];
}

/// load user info from api "myAccount"
final class AppUserFetchRequested extends AppUserEvent {
  @override
  List<Object?> get props => [];
}

/// Logout
final class AppUserLogout extends AppUserEvent {
  @override
  List<Object?> get props => [];
}
