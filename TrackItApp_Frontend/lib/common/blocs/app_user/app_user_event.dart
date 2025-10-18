part of 'app_user_bloc.dart';

@immutable
sealed class AppUserEvent {}

final class SaveUserEvent extends AppUserEvent {
  final UserEntity user;

  SaveUserEvent(this.user);
}
