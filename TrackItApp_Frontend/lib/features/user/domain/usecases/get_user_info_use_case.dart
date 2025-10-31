import 'package:dartz/dartz.dart';
import 'package:track_it_health/core/common/entities/user_entity.dart';
import 'package:track_it_health/core/error/failure.dart';
import 'package:track_it_health/core/utils/usecase/usecase.dart';
import 'package:track_it_health/features/user/domain/repository/user_repository.dart';

class GetUserInfoUseCase implements UseCase<UserEntity, EmptyParams> {
  final UserRepository _userRepository;

  const GetUserInfoUseCase(UserRepository userRepository)
    : _userRepository = userRepository;

  @override
  Future<Either<Failure, UserEntity>> call({
    required EmptyParams params,
  }) async {
    return await _userRepository.getUserInfo();
  }
}
