import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:track_it_health/core/common/blocs/app_user/app_user_bloc.dart';
import 'package:track_it_health/core/service_locator.dart';
import 'package:track_it_health/core/theme/theme.dart';
import 'package:track_it_health/features/auth/presentation/bloc/auth_bloc.dart';
import 'package:flutter/material.dart';
import 'package:track_it_health/features/auth/presentation/pages/signup_page.dart';
import 'package:track_it_health/features/dashboard/presentation/pages/dashboard_page.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  await setupServiceLocator();
  runApp(
    MultiBlocProvider(
      providers: [
        BlocProvider(create: (_) => serviceLocator<AuthBloc>()),
        BlocProvider(create: (_) => serviceLocator<AppUserBloc>()),
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
      home: const DashboardPage(),
    );
  }
}
