import 'package:dartz/dartz.dart';
import 'package:track_it_health/common/entities/token_entity.dart';
import 'package:track_it_health/core/error/exceptions.dart';
import 'package:track_it_health/core/error/failure.dart';
import 'package:track_it_health/features/auth/data/sources/auth_api_service.dart';
import 'package:track_it_health/features/auth/domain/repository/auth_repository.dart';

class AuthRepositoryImpl implements AuthRepository {
  final AuthApiService _authApiService;

  AuthRepositoryImpl(AuthApiService authApiService)
    : _authApiService = authApiService;

  @override
  Future<Either<Failure, Either<String, TokenEntity>>> login(
    Map<String, dynamic> data,
  ) async {
    try {
      final tokenInfo = await _authApiService.login(data);

      // Map TokenModel → TokenEntity or return message
      return Right(
        tokenInfo.fold(
          (message) => left(message),
          (tokenModel) => right(tokenModel.toEntity()),
        ),
      );
    } on ServerExceptions catch (e) {
      return Left(Failure(e.message));
    }
  }

  @override
  Future<Either<Failure, String>> signup(Map<String, dynamic> data) async {
    try {
      final succeededMessage = await _authApiService.signup(data);
      return Right(succeededMessage);
    } on ServerExceptions catch (e) {
      return Left(Failure(e.message));
    }
  }

  @override
  Future<Either<Failure, TokenEntity>> verifyAccount(
    Map<String, dynamic> data,
  ) {
    // TODO: implement verifyAccount
    throw UnimplementedError();
  }
}
