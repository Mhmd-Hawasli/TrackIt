part of 'app_user_bloc.dart';

@immutable
sealed class AppUserEvent extends Equatable {}

/// Save token to secure storage and optionally fetch user info
final class AppUserSaveToken extends AppUserEvent {
  final String accessToken;
  final String refreshToken;
  final bool fetchUserAfterSave;

  AppUserSaveToken({
    required this.accessToken,
    required this.refreshToken,
    this.fetchUserAfterSave = true,
  });

  @override
  List<Object?> get props => [accessToken, refreshToken, fetchUserAfterSave];
}

/// Load token from secure storage and optionally fetch user info
final class AppUserLoadToken extends AppUserEvent {
  final bool fetchUserAfterLoad;

  AppUserLoadToken({this.fetchUserAfterLoad = true});

  @override
  List<Object?> get props => [fetchUserAfterLoad];
}

/// Save user info to secure storage
final class AppUserSaveUser extends AppUserEvent {
  final UserEntity user;

  AppUserSaveUser(this.user);

  @override
  List<Object?> get props => [user];
}

/// Load user info from secure storage
final class AppUserLoadUser extends AppUserEvent {
  @override
  List<Object?> get props => [];
}

/// Logout
final class AppUserLogout extends AppUserEvent {
  @override
  List<Object?> get props => [];
}
