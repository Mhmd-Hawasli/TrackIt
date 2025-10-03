import 'package:dartz/dartz.dart';
import 'package:track_it_health/core/error/failure.dart';

abstract interface class AuthRepository {

  Future<Either<Failure, String>> signUp({
    required String name,
    required String username,
    required String email,
    required String password,
  });

  Future<Either<Failure, String>> login({
   required String usernameOrEmail,
   required String password
});
}
