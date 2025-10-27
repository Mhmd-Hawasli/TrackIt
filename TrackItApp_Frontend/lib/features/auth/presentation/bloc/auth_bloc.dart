import 'dart:async';
import 'package:equatable/equatable.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:track_it_health/core/utils/secure_local_storage.dart';
import 'package:track_it_health/features/auth/domain/entities/token_entity.dart';
import 'package:track_it_health/features/auth/domain/usecases/login_use_case.dart';
import 'package:track_it_health/features/auth/domain/usecases/signup_use_case.dart';
import 'package:track_it_health/features/auth/domain/usecases/verify_account_use_case.dart';
import 'package:track_it_health/features/auth/domain/usecases/refresh_token_use_case.dart';

part 'auth_event.dart';

part 'auth_state.dart';

class AuthBloc extends Bloc<AuthEvent, AuthState> {
  final SignUpUseCase _signUpUseCase;
  final LoginUseCase _loginUseCase;
  final VerifyAccountUseCase _verifyAccountUseCase;
  final RefreshTokenUseCase _refreshTokenUseCase;
  final SecureLocalStorage _secureStorage;

  static const Duration _timeoutDuration = Duration(seconds: 10);

  AuthBloc({
    required SignUpUseCase signUpUseCase,
    required LoginUseCase loginUseCase,
    required VerifyAccountUseCase verifyAccountUseCase,
    required RefreshTokenUseCase refreshTokenUseCase,
    required SecureLocalStorage secureStorage,
  }) : _signUpUseCase = signUpUseCase,
       _loginUseCase = loginUseCase,
       _verifyAccountUseCase = verifyAccountUseCase,
       _refreshTokenUseCase = refreshTokenUseCase,
       _secureStorage = secureStorage,
       super(AuthInitialState()) {
    on<AuthEvent>((_, emit) => emit(AuthLoadingState()));

    on<AuthSignUpEvent>(_onSignUpEvent);
    on<AuthLoginEvent>(_onLoginEvent);
    on<AuthVerifyAccountEvent>(_onVerifyAccountEvent);
    on<AuthLoadTokensEvent>(_onLoadTokensEvent);
    on<AuthRefreshTokenEvent>(_onRefreshTokenEvent);
  }

  /// -----------------------------
  /// Private helper method
  /// -----------------------------
  Future<void> _emitSuccessState(
    TokenEntity newToken,
    Emitter<AuthState> emit,
  ) async {
    await _secureStorage.saveTokens(
      accessToken: newToken.accessToken,
      refreshToken: newToken.refreshToken,
    );
    emit(AuthSuccessState(newToken.accessToken));
  }

  /// -----------------------------
  /// Handle User Sign Up
  /// -----------------------------
  Future<void> _onSignUpEvent(
    AuthSignUpEvent event,
    Emitter<AuthState> emit,
  ) async {
    try {
      final response = await _signUpUseCase(
        params: event.userSignUpParams,
      ).timeout(_timeoutDuration);

      response.fold(
        (failure) {
          emit(AuthFailureState(failure.message));
        },
        (succeededMessage) {
          emit(
            AuthNeedVerifyState(event.userSignUpParams.email, succeededMessage),
          );
        },
      );
    } on TimeoutException {
      emit(AuthFailureState('Sign-up request timed out.'));
    } catch (e) {
      emit(AuthFailureState('Unexpected error: $e'));
    }
  }

  /// -----------------------------
  /// Handle User Login
  /// -----------------------------
  Future<void> _onLoginEvent(
    AuthLoginEvent event,
    Emitter<AuthState> emit,
  ) async {
    try {
      final response = await _loginUseCase(
        params: event.userLoginParams,
      ).timeout(_timeoutDuration);

      response.fold(
        (failure) {
          emit(AuthFailureState(failure.message));
        },
        (either) {
          either.fold(
            // Account needs verification
            (message) {
              emit(AuthNeedVerifyState(event.userLoginParams.input, message));
            },
            // Login success
            (tokenEntity) async {
              await _emitSuccessState(tokenEntity, emit);
            },
          );
        },
      );
    } on TimeoutException {
      emit(AuthFailureState('Login request timed out.'));
    } catch (e) {
      emit(AuthFailureState('Unexpected error: $e'));
    }
  }

  /// -----------------------------
  /// Handle Account Verification
  /// -----------------------------
  Future<void> _onVerifyAccountEvent(
    AuthVerifyAccountEvent event,
    Emitter<AuthState> emit,
  ) async {
    try {
      final response = await _verifyAccountUseCase(
        params: event.verifyAccountParams,
      ).timeout(_timeoutDuration);

      response.fold(
        (failure) {
          emit(AuthFailureState(failure.message));
        },
        (tokenEntity) async {
          await _emitSuccessState(tokenEntity, emit);
        },
      );
    } on TimeoutException {
      emit(AuthFailureState('Verification request timed out.'));
    } catch (e) {
      emit(AuthFailureState('Unexpected error: $e'));
    }
  }

  /// -----------------------------
  /// Load Tokens From Secure Storage
  /// -----------------------------
  Future<void> _onLoadTokensEvent(
    AuthLoadTokensEvent event,
    Emitter<AuthState> emit,
  ) async {
    try {
      final accessToken = await _secureStorage.getAccessToken();
      final refreshToken = await _secureStorage.getRefreshToken();

      if (accessToken != null && refreshToken != null) {
        emit(AuthSuccessState(accessToken));
      } else {
        emit(AuthInitialState());
      }
    } catch (e) {
      emit(AuthFailureState('Failed to load tokens: $e'));
    }
  }

  /// -----------------------------
  /// Refresh Tokens When Expired
  /// -----------------------------
  Future<void> _onRefreshTokenEvent(
    AuthRefreshTokenEvent event,
    Emitter<AuthState> emit,
  ) async {
    try {
      final accessToken = await _secureStorage.getAccessToken();
      final refreshToken = await _secureStorage.getRefreshToken();

      if (accessToken == null || refreshToken == null) {
        emit(AuthFailureState('No tokens found for refresh.'));
        return;
      }

      final response = await _refreshTokenUseCase.call(
        params: RefreshTokenParams(
          accessToken: accessToken,
          refreshToken: refreshToken,
        ),
      );

      response.fold(
        (failure) {
          emit(AuthFailureState(failure.message));
        },
        (newToken) async {
          await _emitSuccessState(newToken, emit);
        },
      );
    } catch (e) {
      emit(AuthFailureState('Token refresh failed: $e'));
    }
  }
}
