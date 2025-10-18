import 'package:get_it/get_it.dart';
import 'package:track_it_health/common/blocs/app_token/app_token_bloc.dart';
import 'package:track_it_health/common/blocs/app_user/app_user_bloc.dart';
import 'package:track_it_health/core/network/dio_client.dart';
import 'package:track_it_health/features/auth/data/repository/auth_repository_impl.dart';
import 'package:track_it_health/features/auth/data/sources/auth_data_source.dart';
import 'package:track_it_health/features/auth/domain/repository/auth_repository.dart';
import 'package:track_it_health/features/auth/domain/usecases/login_use_case.dart';
import 'package:track_it_health/features/auth/domain/usecases/signup_use_case.dart';
import 'package:track_it_health/features/auth/domain/usecases/verify_account_use_case.dart';
import 'package:track_it_health/features/auth/presentation/bloc/auth_bloc.dart';

final serviceLocator = GetIt.instance;

Future<void> setupServiceLocator() async {
  serviceLocator.registerLazySingleton<DioClient>(() => DioClient());

  //services
  serviceLocator.registerFactory<AuthDataSource>(
    () => AuthApiServiceImpl(serviceLocator()),
  );

  //repositories
  serviceLocator.registerFactory<AuthRepository>(
    () => AuthRepositoryImpl(serviceLocator()),
  );

  //useCases
  serviceLocator.registerFactory(() => SignUpUseCase(serviceLocator()));
  serviceLocator.registerFactory(() => LoginUseCase(serviceLocator()));
  serviceLocator.registerFactory(() => VerifyAccountUseCase(serviceLocator()));

  //bloc
  serviceLocator.registerLazySingleton(
    () => AuthBloc(
      signUpUseCase: serviceLocator(),
      loginUseCase: serviceLocator(),
      verifyAccountUseCase: serviceLocator(),
    ),
  );
  serviceLocator.registerLazySingleton(() => AppUserBloc());
  serviceLocator.registerLazySingleton(() => AppTokenBloc());
}
