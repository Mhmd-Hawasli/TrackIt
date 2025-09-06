import 'package:track_it/core/usecase/usecase.dart';
import 'package:track_it/data/models/signin_req_params.dart';
import 'package:track_it/domain/repository/auth.dart';
import 'package:track_it/service_locator.dart';
import 'package:dartz/dartz.dart';

class SigninUseCase implements UseCase<Either, SigninReqParams> {

  @override
  Future<Either> call({SigninReqParams ? param}) async {
    return sl<AuthRepository>().signin(param!);
  }
  
}
