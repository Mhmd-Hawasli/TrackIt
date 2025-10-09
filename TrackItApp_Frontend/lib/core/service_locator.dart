import 'package:get_it/get_it.dart';
import 'package:track_it_health/core/network/dio_client.dart';
import 'package:track_it_health/features/auth/data/repository/auth_repository_impl.dart';
import 'package:track_it_health/features/auth/data/sources/auth_api_service.dart';
import 'package:track_it_health/features/auth/domain/repository/auth_repository.dart';
import 'package:track_it_health/features/auth/domain/usecases/login_usecase.dart';
import 'package:track_it_health/features/auth/domain/usecases/signup_usecase.dart';
import 'package:track_it_health/features/auth/presentation/bloc/auth_bloc.dart';

final sl = GetIt.instance;

Future<void> setupServiceLocator() async {
  sl.registerLazySingleton<DioClient>(() => DioClient());

  //services
  sl.registerFactory<AuthApiService>(() => AuthApiServiceImpl(sl()));

  //repositories
  sl.registerFactory<AuthRepository>(() => AuthRepositoryImpl(sl()));

  //useCases
  sl.registerFactory(() => UserSignUpUseCase(sl()));
  sl.registerFactory(() => UserLoginUseCase(sl()));

  //bloc
  sl.registerLazySingleton(
    () => AuthBloc(userSignUpUseCase: sl(), userLoginUseCase: sl()),
  );
}
