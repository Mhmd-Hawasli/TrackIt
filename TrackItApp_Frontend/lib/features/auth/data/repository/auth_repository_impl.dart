import 'package:dartz/dartz.dart';
import 'package:track_it_health/core/error/exceptions.dart';
import 'package:track_it_health/core/error/failure.dart';
import 'package:track_it_health/features/auth/data/sources/auth_api_service.dart';
import 'package:track_it_health/features/auth/domain/entities/user_entity.dart';
import 'package:track_it_health/features/auth/domain/repository/auth_repository.dart';

class AuthRepositoryImpl implements AuthRepository {
  final AuthApiService _authApiService;

  AuthRepositoryImpl(AuthApiService authApiService)
    : _authApiService = authApiService;

  @override
  Future<Either<Failure, UserEntity>> login(Map<String, dynamic> data) {
    // TODO: implement login
    throw UnimplementedError();
  }

  @override
  Future<Either<Failure, UserEntity>> signup(Map<String, dynamic> data) async {
    try {
      final userInfo = await _authApiService.signup(data);
      return Right(userInfo);
    } on ServerExceptions catch (e) {
      return Left(Failure(e.message));
    }
  }
}
