import 'package:dartz/dartz.dart';
import 'package:track_it_health/core/error/failure.dart';
import 'package:track_it_health/core/usecase/usecase.dart';
import 'package:track_it_health/features/auth/domain/entities/user_entity.dart';
import 'package:track_it_health/features/auth/domain/repository/auth_repository.dart';

class UserLoginUseCase implements UseCase<UserEntity, UserLoginParams> {
  final AuthRepository _authRepository;

  const UserLoginUseCase(AuthRepository authRepository)
    : _authRepository = authRepository;

  @override
  Future<Either<Failure, UserEntity>> call({
    required UserLoginParams params,
  }) async {
    return await _authRepository.login(params.toMap());
  }
}

class UserLoginParams {
  final String usernameOrEmail;
  final String password;

  UserLoginParams({required this.usernameOrEmail, required this.password});

  Map<String, dynamic> toMap() {
    return {'username': this.usernameOrEmail, 'password': this.password};
  }

  factory UserLoginParams.fromMap(Map<String, dynamic> map) {
    return UserLoginParams(
      usernameOrEmail: map['usernameOrEmail'] as String,
      password: map['password'] as String,
    );
  }
}
