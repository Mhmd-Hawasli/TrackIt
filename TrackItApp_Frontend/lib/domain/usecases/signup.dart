import 'package:track_it/core/usecase/usecase.dart';
import 'package:track_it/data/models/signup_req_params.dart';
import 'package:track_it/domain/repository/auth.dart';
import 'package:track_it/service_locator.dart';
import 'package:dartz/dartz.dart';

class SignupUseCase implements UseCase<Either, SignupReqParams> {

  @override
  Future<Either> call({SignupReqParams ? param}) async {
    return sl<AuthRepository>().signup(param!);
  }
  
}
