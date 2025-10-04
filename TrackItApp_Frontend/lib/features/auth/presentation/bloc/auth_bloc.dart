import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:track_it_health/features/auth/domain/usecases/signup_use_case.dart';

part 'auth_event.dart';

part 'auth_state.dart';

class AuthBloc extends Bloc<AuthEvent, AuthState> {
  final UserSignUpUseCase _userSignup;

  AuthBloc({required UserSignUpUseCase userSignUpUseCase})
    : _userSignup = userSignUpUseCase,
      super(AuthInitial()) {
    on<AuthSignUp>((event, emit) async {
      var response = await _userSignup(
        params: UserSignUpParams.fromMap(event.toMap()),
      );
      response.fold(
        (l) => emit(AuthFailure(l.message)),
        (r) => emit(AuthSuccess(r)),
      );
    });
  }
}
