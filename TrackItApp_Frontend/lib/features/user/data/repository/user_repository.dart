import 'package:dartz/dartz.dart';
import 'package:track_it_health/core/common/entities/user_entity.dart';
import 'package:track_it_health/core/error/exceptions.dart';
import 'package:track_it_health/core/error/failure.dart';
import 'package:track_it_health/features/user/data/models/user_model.dart';
import 'package:track_it_health/features/user/data/sources/user_data_source.dart';
import 'package:track_it_health/features/user/domain/repository/user_repository.dart';

class UserRepositoryImpl implements UserRepository {
  final UserDataSource _userDataSource;

  const UserRepositoryImpl(UserDataSource userDataSource)
    : _userDataSource = userDataSource;

  ///========================
  /// get User Info
  ///========================
  @override
  Future<Either<Failure, UserEntity>> getUserInfo() async {
    try {
      final user = await _userDataSource.getUserInfo();
      return right(user.toEntity());
    } on ServerExceptions catch (e) {
      return left(Failure(e.message));
    }
  }
}
