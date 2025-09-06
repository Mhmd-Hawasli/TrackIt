import 'package:track_it/core/usecase/usecase.dart';
import 'package:track_it/domain/repository/auth.dart';
import 'package:track_it/service_locator.dart';
import 'package:dartz/dartz.dart';

class GetUserUseCase implements UseCase<Either, dynamic> {

  @override
  Future<Either> call({dynamic param}) async {
    return sl<AuthRepository>().getUser();
  }
  
}
