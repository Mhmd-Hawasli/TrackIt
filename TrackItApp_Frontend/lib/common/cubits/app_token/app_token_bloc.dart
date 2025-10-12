import 'package:bloc/bloc.dart';
import 'package:meta/meta.dart';

part 'app_token_event.dart';
part 'app_token_state.dart';

class AppTokenBloc extends Bloc<AppTokenEvent, AppTokenState> {
  AppTokenBloc() : super(AppTokenInitial()) {
    on<AppTokenEvent>((event, emit) {
      // TODO: implement event handler
    });
  }
}
