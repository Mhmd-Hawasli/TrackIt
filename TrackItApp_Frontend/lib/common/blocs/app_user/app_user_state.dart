part of 'app_user_bloc.dart';

@immutable
sealed class AppUserState extends Equatable {}

final class AppUserInitial extends AppUserState {
  @override
  List<Object?> get props => [];
}
