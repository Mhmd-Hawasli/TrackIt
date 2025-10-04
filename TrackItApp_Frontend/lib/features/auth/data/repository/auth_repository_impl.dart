import 'package:dartz/dartz.dart';
import 'package:track_it_health/core/error/failure.dart';
import 'package:track_it_health/core/service_locator.dart';
import 'package:track_it_health/features/auth/data/models/signup_req_params.dart';
import 'package:track_it_health/features/auth/data/sources/auth_remote_data_source.dart';
import 'package:track_it_health/features/auth/domain/repository/auth_repository.dart';

class AuthRepositoryImpl extends AuthRepository {
  @override
  Future<Either<Failure, String>> login({
    required String usernameOrEmail,
    required String password,
  }) {
    // TODO: implement login
    throw UnimplementedError();
  }

  @override
  Future<Either> signUp(SignupReqParams signupRepParams) async {
    return sl<AuthApiService>().signUp(signupRepParams);
  }
}
