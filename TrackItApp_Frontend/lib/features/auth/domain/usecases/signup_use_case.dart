import 'package:dartz/dartz.dart';
import 'package:track_it_health/core/error/failure.dart';
import 'package:track_it_health/core/utils/usecase/usecase.dart';
import 'package:track_it_health/features/auth/domain/repository/auth_repository.dart';

class UserSignUpUseCase implements UseCase<String, UserSignUpParams> {
  final AuthRepository _authRepository;

  const UserSignUpUseCase(AuthRepository authRepository)
    : _authRepository = authRepository;

  @override
  Future<Either<Failure, String>> call({
    required UserSignUpParams params,
  }) async {
    return await _authRepository.signup(params.toMap());
  }
}

class UserSignUpParams {
  final String name;
  final String username;
  final String email;
  final String password;

  UserSignUpParams({
    required this.name,
    required this.username,
    required this.email,
    required this.password,
  });

  Map<String, dynamic> toMap() {
    return {
      'name': name,
      'username': username,
      'email': email,
      'password': password,
    };
  }

  factory UserSignUpParams.fromMap(Map<String, dynamic> map) {
    return UserSignUpParams(
      name: map['name'] as String,
      username: map['username'] as String,
      email: map['email'] as String,
      password: map['password'] as String,
    );
  }
}
