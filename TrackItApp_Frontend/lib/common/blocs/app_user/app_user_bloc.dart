import 'package:bloc/bloc.dart';
import 'package:equatable/equatable.dart';
import 'package:meta/meta.dart';
import 'package:track_it_health/common/entities/user_entity.dart';

part 'app_user_event.dart';

part 'app_user_state.dart';

class AppUserBloc extends Bloc<AppUserEvent, AppUserState> {
  AppUserBloc() : super(AppUserInitial()) {
    on<AppUserEvent>((event, emit) {
      // TODO: implement event handler
    });
  }
}
