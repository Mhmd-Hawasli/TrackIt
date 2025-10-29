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
    on<AppUserEvent>((_, emit) => emit(AppUserLoading()));

    on<AppUserSaveToStorage>(_onSaveUser);
    on<AppUserStorageLoaded>(_onLoadUser);
    on<AppUserFetchRequested>(_onFetchUser);
    on<AppUserLogout>(_onLogout);
  }

  /// -----------------------------
  /// Save user to secure storage event
  /// -----------------------------
  Future<void> _onSaveUser(
    AppUserSaveToStorage event,
    Emitter<AppUserState> emit,
  ) async {
    try {
      await _storage.saveUser(event.userEntity);
      emit(AppUserLoaded(event.userEntity));
    } catch (e) {
      emit(AppUserError(e.toString()));
    }
  }

  /// -----------------------------
  /// Load user from secure storage event
  /// -----------------------------
  Future<void> _onLoadUser(
    AppUserStorageLoaded event,
    Emitter<AppUserState> emit,
  ) async {
    try {
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

  /// -----------------------------
  /// Fetch user data from api
  /// -----------------------------
  Future<void> _onFetchUser(
    AppUserFetchRequested event,
    Emitter<AppUserState> emit,
  ) async {
    try {
      await _storage.clearAll();

      emit(AppUserUnauthenticated());
    } catch (e) {
      emit(AppUserError(e.toString()));
    }
  }

  /// -----------------------------
  /// Logout user and delete his info
  /// -----------------------------
  Future<void> _onLogout(
    AppUserLogout event,
    Emitter<AppUserState> emit,
  ) async {
    try {
      await _storage.clearAll();

      emit(AppUserUnauthenticated());
    } catch (e) {
      emit(AppUserError(e.toString()));
    }
  }
}
