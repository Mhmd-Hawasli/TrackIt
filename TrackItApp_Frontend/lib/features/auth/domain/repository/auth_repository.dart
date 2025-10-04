import 'package:dartz/dartz.dart';
import 'package:track_it_health/core/error/failure.dart';
import 'package:track_it_health/features/auth/data/models/signup_req_params.dart';

abstract interface class AuthRepository {
  Future<Either> signUpOld(SignupReqParams signupReqParams);

  Future<Either<Failure, String>> signup(Map<String, dynamic> data);

  Future<Either<Failure, String>> login(Map<String, dynamic> data);
}
