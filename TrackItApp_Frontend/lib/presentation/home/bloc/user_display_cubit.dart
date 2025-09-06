import 'package:track_it/domain/usecases/get_user.dart';
import 'package:track_it/presentation/home/bloc/user_display_state.dart';
import 'package:track_it/service_locator.dart';
import 'package:flutter_bloc/flutter_bloc.dart';

class UserDisplayCubit extends Cubit<UserDisplayState> {

  UserDisplayCubit() : super (UserLoading());

  void displayUser() async {
    var result = await sl < GetUserUseCase > ().call();
    result.fold(
      (error) {
        emit(LoadUserFailure(errorMessage: error));
      },
      (data) {
        emit(UserLoaded(userEntity: data));
      }
    );
  }
}
