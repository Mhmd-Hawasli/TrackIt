import 'package:track_it/core/usecase/usecase.dart';
import 'package:track_it/domain/repository/auth.dart';
import 'package:track_it/service_locator.dart';
import 'package:dartz/dartz.dart';

class LogoutUseCase implements UseCase<Either, dynamic> {

  @override
  Future<Either> call({param}) async {
    return await sl<AuthRepository>().logout();
  }

}
