import 'package:dartz/dartz.dart';
import 'package:track_it_health/core/common/entities/user_entity.dart';
import 'package:track_it_health/core/error/failure.dart';

abstract interface class UserRepository {
  Future<Either<Failure, UserEntity>> getUserInfo();
}
