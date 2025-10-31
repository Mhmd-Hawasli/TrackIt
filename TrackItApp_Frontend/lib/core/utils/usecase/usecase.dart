import 'package:dartz/dartz.dart';
import 'package:track_it_health/core/error/failure.dart';

abstract interface class UseCase<SuccessType, Params> {
  Future<Either<Failure, SuccessType>> call({required Params params});
}

class EmptyParams {}
