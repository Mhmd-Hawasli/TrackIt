import 'dart:async';

import 'package:equatable/equatable.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:track_it_health/core/common/entities/user_entity.dart';
import 'package:track_it_health/core/error/exceptions.dart';
import 'package:track_it_health/core/utils/usecase/usecase.dart';
import 'package:track_it_health/features/user/domain/usecases/get_user_info_use_case.dart';

part 'user_event.dart';

part 'user_state.dart';

class UserBloc extends Bloc<UserEvent, UserState> {
  final GetUserInfoUseCase _getUserInfoUseCase;
  static const Duration _timeoutDuration = Duration(seconds: 10);

  UserBloc({required GetUserInfoUseCase getUserInfoUseCase})
    : _getUserInfoUseCase = getUserInfoUseCase,
      super(UserInitial()) {
    on<UserEvent>((_, emit) => emit(UserLoadingData()));

    on<UserLoadDataEvent>(_onUserLoadDataEvent);
  }

  ///=========================
  /// event UserLoadDataEvent
  ///=========================
  Future<void> _onUserLoadDataEvent(
    UserLoadDataEvent event,
    Emitter<UserState> emit,
  ) async {
    try {
      final response = await _getUserInfoUseCase(
        params: EmptyParams(),
      ).timeout(_timeoutDuration);
      response.fold(
        (failure) {
          emit(UserFailureState(failure.message));
        },
        (userEntity) {
          emit(UserLoadedData(userEntity));
        },
      );
    } on TimeoutException {
      emit(UserFailureState('Login request timed out.'));
    } catch (e) {
      emit(UserFailureState(e.toString()));
    }
  }
} //end of bloc
