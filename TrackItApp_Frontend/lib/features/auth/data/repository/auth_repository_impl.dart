import 'package:dartz/dartz.dart';
import 'package:track_it_health/common/entities/token_entity.dart';
import 'package:track_it_health/core/error/exceptions.dart';
import 'package:track_it_health/core/error/failure.dart';
import 'package:track_it_health/features/auth/data/models/token_model.dart';
import 'package:track_it_health/features/auth/data/sources/auth_data_source.dart';
import 'package:track_it_health/features/auth/domain/repository/auth_repository.dart';

class AuthRepositoryImpl implements AuthRepository {
  final AuthDataSource _authApiService;

  AuthRepositoryImpl(AuthDataSource authApiService)
    : _authApiService = authApiService;

  //===========================================
  // login
  //===========================================
  @override
  Future<Either<Failure, Either<String, TokenEntity>>> login(
    Map<String, dynamic> data,
  ) async {
    try {
      final tokenInfo = await _authApiService.loginDataSource(data);

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

  //===========================================
  // signup
  //===========================================
  @override
  Future<Either<Failure, String>> signup(Map<String, dynamic> data) async {
    try {
      final succeededMessage = await _authApiService.signupDataSource(data);
      return Right(succeededMessage);
    } on ServerExceptions catch (e) {
      return Left(Failure(e.message));
    }
  }

  //===========================================
  // verifyAccount
  //===========================================
  @override
  Future<Either<Failure, TokenEntity>> verifyAccount(
    Map<String, dynamic> data,
  ) async {
    try {
      final tokenModel = await _authApiService.verifyAccountDataSource(data);
      return Right(tokenModel.toEntity());
    } on ServerExceptions catch (e) {
      return Left(Failure(e.message));
    }
  }
}
