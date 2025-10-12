import 'package:dartz/dartz.dart';
import 'package:track_it_health/common/entities/token_entity.dart';
import 'package:track_it_health/core/error/failure.dart';
import 'package:track_it_health/core/usecase/usecase.dart';
import 'package:track_it_health/common/entities/user_entity.dart';
import 'package:track_it_health/features/auth/domain/repository/auth_repository.dart';

class UserLoginUseCase
    implements UseCase<Either<String, TokenEntity>, UserLoginParams> {
  final AuthRepository _authRepository;

  const UserLoginUseCase(AuthRepository authRepository)
    : _authRepository = authRepository;

  @override
  Future<Either<Failure, Either<String, TokenEntity>>> call({
    required UserLoginParams params,
  }) async {
    return await _authRepository.login(params.toMap());
  }
}

class UserLoginParams {
  final String input;
  final String password;

  UserLoginParams({required this.input, required this.password});

  Map<String, dynamic> toMap() {
    return {'input': input, 'password': password};
  }

  factory UserLoginParams.fromMap(Map<String, dynamic> map) {
    return UserLoginParams(
      input: map['input'] as String,
      password: map['password'] as String,
    );
  }
}
