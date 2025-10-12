import 'package:equatable/equatable.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:track_it_health/common/entities/token_entity.dart';
import 'package:track_it_health/common/entities/user_entity.dart';
import 'package:track_it_health/core/utils/secure_local_storage.dart';
import 'package:track_it_health/features/auth/domain/usecases/login_usecase.dart';
import 'package:track_it_health/features/auth/domain/usecases/signup_usecase.dart';
import 'package:track_it_health/features/auth/domain/usecases/verify_account_usecase.dart';

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
       super(AuthInitialState()) {
    //event one
    on<AuthSignUpEvent>(_onAuthSignUpEvent);
    //event two
    on<AuthLoginEvent>(_onAuthLoginEvent);
  }

  void _onAuthSignUpEvent(
    AuthSignUpEvent event,
    Emitter<AuthState> emit,
  ) async {
    emit(AuthLoadingState());
    final response = await _userSignup(params: event.userSignUpParams);
    response.fold(
      (left) => emit(AuthFailureState(left.message)),
      (succeededMessage) => emit(
        AuthNeedVerifyState(event.userSignUpParams.email, succeededMessage),
      ),
    );
  }

  void _onAuthLoginEvent(AuthLoginEvent event, Emitter<AuthState> emit) async {
    emit(AuthLoadingState());

    final response = await _userLogin(params: event.userLoginParams);

    response.fold(
      // Failure (e.g., network/server error)
      (failure) => emit(AuthFailureState(failure.message)),

      // Success → Either verification message or token
      (either) {
        either.fold(
          // Account needs verification
          (message) =>
              emit(AuthNeedVerifyState(event.userLoginParams.input, message)),

          // Login success → store tokens & emit success
          (tokenEntity) async {
            // Save tokens securely
            await SecureLocalStorage.saveTokens(
              accessToken: tokenEntity.accessToken,
              refreshToken: tokenEntity.refreshToken,
            );
            emit(AuthSuccessState(tokenEntity));
          },
        );
      },
    );
  }
}
