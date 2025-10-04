import 'package:dartz/dartz.dart';
import 'package:track_it_health/core/error/failure.dart';
import 'package:track_it_health/features/auth/domain/entities/user_entity.dart';

abstract interface class AuthRepository {
  Future<Either<Failure, UserEntity>> signup(Map<String, dynamic> data);

  Future<Either<Failure, UserEntity>> login(Map<String, dynamic> data);
}
