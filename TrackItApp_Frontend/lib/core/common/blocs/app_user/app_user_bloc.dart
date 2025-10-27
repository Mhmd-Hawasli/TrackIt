import 'package:equatable/equatable.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:track_it_health/core/common/entities/user_entity.dart';
import 'package:track_it_health/core/utils/secure_local_storage.dart';
import 'package:track_it_health/features/auth/data/sources/auth_data_source.dart';

part 'app_user_event.dart';

part 'app_user_state.dart';

class AppUserBloc extends Bloc<AppUserEvent, AppUserState> {
  final SecureLocalStorage _storage;
  final AuthDataSource _authDataSource;

  AppUserBloc({
    required SecureLocalStorage storage,
    required AuthDataSource authDataSource,
  }) : _authDataSource = authDataSource,
       _storage = storage,
       super(AppUserInitial()) {
    on<AppUserSaveToken>(_onSaveToken);
    on<AppUserLoadToken>(_onLoadToken);
    on<AppUserSaveUser>(_onSaveUser);
    on<AppUserLoadUser>(_onLoadUser);
    on<AppUserLogout>(_onLogout);
  }

  Future<void> _onSaveToken(
    AppUserSaveToken event,
    Emitter<AppUserState> emit,
  ) async {
    try {
      emit(AppUserLoading());

      // Save tokens locally
      await _storage.saveTokens(
        accessToken: event.accessToken,
        refreshToken: event.refreshToken,
      );

      emit(
        AppUserTokenUpdated(
          accessToken: event.accessToken,
          refreshToken: event.refreshToken,
        ),
      );

      // Optionally fetch user info
      if (event.fetchUserAfterSave) {
        add(AppUserLoadUser());
      }
    } catch (e) {
      emit(AppUserError(e.toString()));
    }
  }

  Future<void> _onLoadToken(
    AppUserLoadToken event,
    Emitter<AppUserState> emit,
  ) async {
    try {
      emit(AppUserLoading());

      final accessToken = await _storage.getAccessToken();
      final refreshToken = await _storage.getRefreshToken();

      if (accessToken == null || refreshToken == null) {
        emit(AppUserUnauthenticated());
        return;
      }

      emit(
        AppUserTokenUpdated(
          accessToken: accessToken,
          refreshToken: refreshToken,
        ),
      );

      if (event.fetchUserAfterLoad) {
        add(AppUserLoadUser());
      }
    } catch (e) {
      emit(AppUserError(e.toString()));
    }
  }

  Future<void> _onSaveUser(
    AppUserSaveUser event,
    Emitter<AppUserState> emit,
  ) async {
    try {
      emit(AppUserLoading());

      await _storage.saveUser(event.user);
      emit(AppUserLoaded(event.user));
    } catch (e) {
      emit(AppUserError(e.toString()));
    }
  }

  Future<void> _onLoadUser(
    AppUserLoadUser event,
    Emitter<AppUserState> emit,
  ) async {
    try {
      emit(AppUserLoading());

      // Get user from storage
      var user = await _storage.getUser();

      // If not found locally, call API
      if (user == null) {
        final accessToken = await _storage.getAccessToken();
        if (accessToken == null) {
          emit(AppUserUnauthenticated());
          return;
        }
        var userModel = await _authDataSource.getCurrentUser(); // call /me
        user = userModel.toEntity();
        // save locally
        await _storage.saveUser(user);
      }

      emit(AppUserLoaded(user));
    } catch (e) {
      emit(AppUserError(e.toString()));
    }
  }

  Future<void> _onLogout(
    AppUserLogout event,
    Emitter<AppUserState> emit,
  ) async {
    try {
      emit(AppUserLoading());

      await _storage.clearAll();

      emit(AppUserUnauthenticated());
    } catch (e) {
      emit(AppUserError(e.toString()));
    }
  }
}
