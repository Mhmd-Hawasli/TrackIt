part of 'app_token_bloc.dart';

@immutable
sealed class AppTokenState extends Equatable {}

final class AppTokenInitial extends AppTokenState {
  @override
  List<Object?> get props => [];
}
