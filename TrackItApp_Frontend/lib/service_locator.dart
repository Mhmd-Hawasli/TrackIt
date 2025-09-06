
import 'package:track_it/core/network/dio_client.dart';
import 'package:track_it/data/repository/auth.dart';
import 'package:track_it/data/source/auth_api_service.dart';
import 'package:track_it/data/source/auth_local_service.dart';
import 'package:track_it/domain/repository/auth.dart';
import 'package:track_it/domain/usecases/get_user.dart';
import 'package:track_it/domain/usecases/is_logged_in.dart';
import 'package:track_it/domain/usecases/signin.dart';
import 'package:track_it/domain/usecases/logout.dart';
import 'package:track_it/domain/usecases/signup.dart';
import 'package:get_it/get_it.dart';

final sl = GetIt.instance;

void setupServiceLocator() {

  sl.registerSingleton<DioClient>(DioClient());
  
  // Services
  sl.registerSingleton<AuthApiService>(
    AuthApiServiceImpl()
  );

  sl.registerSingleton<AuthLocalService>(
    AuthLocalServiceImpl()
  );

  // Repositories
  sl.registerSingleton<AuthRepository>(
    AuthRepositoryImpl()
  );

  // Usecases
  sl.registerSingleton<SignupUseCase>(
    SignupUseCase()
  );

  sl.registerSingleton<IsLoggedInUseCase>(
    IsLoggedInUseCase()
  );

  sl.registerSingleton<GetUserUseCase>(
    GetUserUseCase()
  );

  sl.registerSingleton<LogoutUseCase>(
    LogoutUseCase()
  );

  sl.registerSingleton<SigninUseCase>(
    SigninUseCase()
  );
}
