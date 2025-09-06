import 'package:track_it/core/usecase/usecase.dart';
import 'package:track_it/domain/repository/auth.dart';
import 'package:track_it/service_locator.dart';

class IsLoggedInUseCase implements UseCase<bool, dynamic> {

  @override
  Future<bool> call({dynamic param}) async {
    return sl<AuthRepository>().isLoggedIn();
  }
  
}
