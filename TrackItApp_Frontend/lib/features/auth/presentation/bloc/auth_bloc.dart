import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:track_it_health/features/auth/domain/entities/user_entity.dart';
import 'package:track_it_health/features/auth/domain/usecases/login_usecase.dart';
import 'package:track_it_health/features/auth/domain/usecases/signup_usecase.dart';

part 'auth_event.dart';

part 'auth_state.dart';

class AuthBloc extends Bloc<AuthEvent, AuthState> {
  final UserSignUpUseCase _userSignup;
  final UserLoginUseCase _userLogin;

  AuthBloc({
    required UserSignUpUseCase userSignUpUseCase,
    required UserLoginUseCase userLoginUseCase,
  }) : _userSignup = userSignUpUseCase,
       _userLogin = userLoginUseCase,
       super(AuthInitial()) {
    //event one
    on<AuthSignUpEvent>(_onAuthSignUpEvent);
    //event two
    on<AuthLoginEvent>(_onAuthLoginEvent);
  }

  void _onAuthSignUpEvent(
    AuthSignUpEvent event,
    Emitter<AuthState> emit,
  ) async {
    emit(AuthLoading());
    var response = await _userSignup(params: event.userSignUpParams);
    response.fold(
      (left) => emit(AuthFailure(left.message)),
      (right) => emit(AuthSuccess(right)),
    );
  }

  void _onAuthLoginEvent(AuthLoginEvent event, Emitter<AuthState> emit) async {
    emit(AuthLoading());
    var response = await _userLogin(params: event.userLoginParams);
    response.fold(
      (left) => emit(AuthFailure(left.message)),
      (right) => emit(AuthSuccess(right)),
    );
  }
}
