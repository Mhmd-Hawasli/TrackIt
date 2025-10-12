import 'package:dartz/dartz.dart';
import 'package:track_it_health/common/entities/token_entity.dart';
import 'package:track_it_health/core/error/failure.dart';
import 'package:track_it_health/core/usecase/usecase.dart';
import 'package:track_it_health/features/auth/domain/repository/auth_repository.dart';

class VerifyAccountUseCase
    implements UseCase<TokenEntity, VerifyAccountParams> {
  final AuthRepository _authRepository;

  const VerifyAccountUseCase(AuthRepository authRepository)
    : _authRepository = authRepository;

  @override
  Future<Either<Failure, TokenEntity>> call({
    required VerifyAccountParams params,
  }) async {
    return await _authRepository.verifyAccount(params.toMap());
  }
}

class VerifyAccountParams {
  final String input;
  final String code;

  VerifyAccountParams({required this.input, required this.code});

  Map<String, dynamic> toMap() {
    return {'input': input, 'code': code};
  }

  factory VerifyAccountParams.fromMap(Map<String, dynamic> map) {
    return VerifyAccountParams(
      input: map['input'] as String,
      code: map['code'] as String,
    );
  }
}
