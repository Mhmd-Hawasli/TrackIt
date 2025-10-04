import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:track_it_health/core/secrets/app_secrets.dart';
import 'package:track_it_health/core/service_locator.dart';
import 'package:track_it_health/core/theme/theme.dart';
import 'package:track_it_health/features/auth/data/repository/auth_repository_impl.dart';
import 'package:track_it_health/features/auth/data/sources/auth_api_service.dart';
import 'package:track_it_health/features/auth/domain/usecases/signup_use_case.dart';
import 'package:track_it_health/features/auth/presentation/bloc/auth_bloc.dart';
import 'package:track_it_health/features/auth/presentation/pages/login_page.dart';
import 'package:flutter/material.dart';
import 'package:track_it_health/features/auth/presentation/pages/signup_page.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  await setupServiceLocator();
  runApp(
    MultiBlocProvider(
      providers: [
        BlocProvider(create: (_) => sl<AuthBloc>(), child: Container()),
      ],
      child: const MyApp(),
    ),
  );
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      debugShowCheckedModeBanner: false,
      title: 'TrackIt-Health',
      theme: AppTheme.darkThemeMode,
      home: const SignUpPage(),
    );
  }
}
