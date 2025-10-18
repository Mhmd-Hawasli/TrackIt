import 'package:bloc/bloc.dart';
import 'package:equatable/equatable.dart';
import 'package:meta/meta.dart';
import 'package:track_it_health/common/entities/token_entity.dart';

part 'app_token_event.dart';

part 'app_token_state.dart';

class AppTokenBloc extends Bloc<AppTokenEvent, AppTokenState> {
  AppTokenBloc() : super(AppTokenInitial()) {
    on<AppTokenEvent>((event, emit) {
      // TODO: implement event handler
    });
  }
}
