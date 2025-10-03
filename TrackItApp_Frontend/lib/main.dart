import 'package:supabase_flutter/supabase_flutter.dart';
import 'package:track_it_health/core/secrets/app_secrets.dart';
import 'package:track_it_health/core/theme/theme.dart';
import 'package:track_it_health/features/auth/presentation/pages/login_page.dart';
import 'package:flutter/material.dart';
import 'package:track_it_health/features/auth/presentation/pages/signup_page.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  final supabse = await Supabase.initialize(
    url: AppSecrets.supabaseUrl,
    anonKey: AppSecrets.supabaseAnnonKey,
  );
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      debugShowCheckedModeBanner: false,
      title: 'TrackIt-Health',
      theme: AppTheme.darkThemeMode,
      home: const  SignUpPage(),
    );
  }
}
