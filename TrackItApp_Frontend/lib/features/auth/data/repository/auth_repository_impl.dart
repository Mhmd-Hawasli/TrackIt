import 'package:dartz/dartz.dart';
import 'package:track_it_health/core/error/exceptions.dart';
import 'package:track_it_health/core/error/failure.dart';
import 'package:track_it_health/core/service_locator.dart';
import 'package:track_it_health/features/auth/data/models/signup_req_params.dart';
import 'package:track_it_health/features/auth/data/sources/auth_api_service.dart';
import 'package:track_it_health/features/auth/domain/entities/user_entity.dart';
import 'package:track_it_health/features/auth/domain/repository/auth_repository.dart';

class AuthRepositoryImpl implements AuthRepository {
  final AuthApiService authApiService;

  AuthRepositoryImpl(this.authApiService);

  @override
  Future<Either<Failure, String>> login(Map<String, dynamic> data) {
    // TODO: implement login
    throw UnimplementedError();
  }

  @override
  Future<Either<Failure, String>> signup(Map<String, dynamic> data) async {
    try {
      final userInfo = await authApiService.signup(data);
      return Right(userInfo.email);
    } on ServerExceptions catch (e) {
      return Left(Failure(e.message));
    }
  }

  @override
  Future<Either> signUpOld(SignupReqParams signupRepParams) async {
    return sl<AuthApiService>().signUpOld(signupRepParams);
  }
}
