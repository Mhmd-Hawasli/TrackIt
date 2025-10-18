import 'dart:async';

import 'package:equatable/equatable.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:track_it_health/common/entities/token_entity.dart';
import 'package:track_it_health/common/entities/user_entity.dart';
import 'package:track_it_health/core/utils/secure_local_storage.dart';
import 'package:track_it_health/features/auth/domain/usecases/login_use_case.dart';
import 'package:track_it_health/features/auth/domain/usecases/signup_use_case.dart';
import 'package:track_it_health/features/auth/domain/usecases/verify_account_use_case.dart';

part 'auth_event.dart';

part 'auth_state.dart';

class AuthBloc extends Bloc<AuthEvent, AuthState> {
  final SignUpUseCase _signupUseCase;
  final LoginUseCase _loginUseCase;
  final VerifyAccountUseCase _verifyAccountUseCase;
  static const Duration _timeoutDurations = Duration(seconds: 10);

  AuthBloc({
    required SignUpUseCase signUpUseCase,
    required LoginUseCase loginUseCase,
    required VerifyAccountUseCase verifyAccountUseCase,
  }) : _signupUseCase = signUpUseCase,
       _loginUseCase = loginUseCase,
       _verifyAccountUseCase = verifyAccountUseCase,
       super(AuthInitialState()) {
    //event 1
    on<AuthSignUpEvent>(_onAuthSignUpEvent);
    //event 2
    on<AuthLoginEvent>(_onAuthLoginEvent);
    //event 3
    on<AuthVerifyAccountEvent>(_onAuthVerifyAccountEvent);
  }

  void _onAuthVerifyAccountEvent(
    AuthVerifyAccountEvent event,
    Emitter<AuthState> emit,
  ) async {
    emit(AuthLoadingState());
    try {
      final response =
          await _verifyAccountUseCase(
            params: event.verifyAccountParams,
          ).timeout(
            _timeoutDurations,
            onTimeout: () {
              throw TimeoutException('verifyAccount request timed out');
            },
          );
      response.fold(
        // Left
        (failure) => emit(AuthFailureState(failure.message)),
        // Right
        (tokenEntity) {
          (emit(AuthSuccessState(tokenEntity)));
        },
      );
    } on TimeoutException {
      emit(AuthFailureState('Request time out. Please try again.'));
    } catch (e) {
      emit(AuthFailureState('Unexpected error: $e'));
    }
  }

  void _onAuthSignUpEvent(
    AuthSignUpEvent event,
    Emitter<AuthState> emit,
  ) async {
    emit(AuthLoadingState());
    try {
      final response = await _signupUseCase(params: event.userSignUpParams)
          .timeout(
            _timeoutDurations,
            onTimeout: () {
              throw TimeoutException('SignUp request timed out');
            },
          );
      response.fold(
        (failure) => emit(AuthFailureState(failure.message)),
        (succeededMessage) => emit(
          AuthNeedVerifyState(event.userSignUpParams.email, succeededMessage),
        ),
      );
    } on TimeoutException {
      emit(AuthFailureState('Request time out. Please try again.'));
    } catch (e) {
      emit(AuthFailureState('Unexpected error: $e'));
    }
  }

  void _onAuthLoginEvent(AuthLoginEvent event, Emitter<AuthState> emit) async {
    emit(AuthLoadingState());
    try {
      final response = await _loginUseCase(params: event.userLoginParams)
          .timeout(
            _timeoutDurations,
            onTimeout: () {
              throw TimeoutException('login request time out.');
            },
          );
      response.fold(
        // Left
        (failure) => emit(AuthFailureState(failure.message)),

        // Right
        (either) {
          either.fold(
            // Account needs verification
            (message) =>
                emit(AuthNeedVerifyState(event.userLoginParams.input, message)),

            // Login success → store tokens & emit success
            (tokenEntity) {
              emit(AuthSuccessState(tokenEntity));
            },
          );
        },
      );
    } on TimeoutException {
      emit(AuthFailureState('Request time out. Please try again.'));
    } catch (e) {
      emit(AuthFailureState('Unexpected error: $e'));
    }
  }
}
