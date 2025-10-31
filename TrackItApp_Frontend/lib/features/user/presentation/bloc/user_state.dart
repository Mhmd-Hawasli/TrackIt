part of 'user_bloc.dart';

@immutable
sealed class UserState extends Equatable {}

final class UserInitial extends UserState {
  @override
  // TODO: implement props
  List<Object?> get props => throw UnimplementedError();
}

final class UserLoadingData extends UserState {
  @override
  // TODO: implement props
  List<Object?> get props => throw UnimplementedError();
}

final class UserLoadedData extends UserState {
  final UserEntity user;

  UserLoadedData(this.user);

  @override
  // TODO: implement props
  List<Object?> get props => throw UnimplementedError();
}

final class UserFailureState extends UserState {
  final String message;

  UserFailureState(this.message);

  @override
  List<Object?> get props => [message];
}
