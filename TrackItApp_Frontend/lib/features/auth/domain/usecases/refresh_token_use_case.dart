import 'package:dartz/dartz.dart';
import 'package:track_it_health/core/error/failure.dart';
import 'package:track_it_health/core/utils/usecase/usecase.dart';
import 'package:track_it_health/features/auth/domain/entities/token_entity.dart';
import 'package:track_it_health/features/auth/domain/repository/auth_repository.dart';

class RefreshTokenUseCase implements UseCase<TokenEntity, RefreshTokenParams> {
  final AuthRepository _authRepository;

  const RefreshTokenUseCase(AuthRepository authRepository)
    : _authRepository = authRepository;

  @override
  Future<Either<Failure, TokenEntity>> call({
    required RefreshTokenParams params,
  }) async {
    return await _authRepository.verifyAccount(params.toMap());
  }
}

class RefreshTokenParams {
  final String accessToken;
  final String refreshToken;

  RefreshTokenParams({required this.accessToken, required this.refreshToken});

  Map<String, dynamic> toMap() {
    return {'accessToken': accessToken, 'refreshToken': refreshToken};
  }

  factory RefreshTokenParams.fromMap(Map<String, dynamic> map) {
    return RefreshTokenParams(
      accessToken: map['accessToken'] as String,
      refreshToken: map['refreshToken'] as String,
    );
  }
}
