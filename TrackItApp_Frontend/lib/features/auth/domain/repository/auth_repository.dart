import 'package:dartz/dartz.dart';
import 'package:track_it_health/common/entities/token_entity.dart';
import 'package:track_it_health/core/error/failure.dart';
import 'package:track_it_health/common/entities/user_entity.dart';

abstract interface class AuthRepository {
  Future<Either<Failure, String>> signup(Map<String, dynamic> data);

  Future<Either<Failure, Either<String, TokenEntity>>> login(
    Map<String, dynamic> data,
  );

  Future<Either<Failure, TokenEntity>> verifyAccount(Map<String, dynamic> data);
}
