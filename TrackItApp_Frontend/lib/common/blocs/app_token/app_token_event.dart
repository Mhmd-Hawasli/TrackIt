part of 'app_token_bloc.dart';

@immutable
sealed class AppTokenEvent {}

final class SaveTokenEvent extends AppTokenEvent {
  final TokenEntity tokenEntity;

  SaveTokenEvent(this.tokenEntity);
}
