import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:track_it_health/features/auth/domain/entities/user_entity.dart';
import 'package:track_it_health/features/auth/domain/usecases/signup_use_case.dart';

part 'auth_event.dart';

part 'auth_state.dart';

class AuthBloc extends Bloc<AuthEvent, AuthState> {
  final UserSignUpUseCase _userSignup;

  AuthBloc({required UserSignUpUseCase userSignUpUseCase})
    : _userSignup = userSignUpUseCase,
      super(AuthInitial()) {
    //event one
    on<AuthSignUpEvent>((event, emit) async {
      emit(AuthLoading());
      var response = await _userSignup(params: event.userSignUpParams);
      response.fold(
        (left) => emit(AuthFailure(left.message)),
        (right) => emit(AuthSuccess(right)),
      );
    });
  }
}
