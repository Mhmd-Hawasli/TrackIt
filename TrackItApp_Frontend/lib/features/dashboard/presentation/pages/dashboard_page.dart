import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:track_it_health/core/common/blocs/app_user/app_user_bloc.dart';
import 'package:track_it_health/features/auth/presentation/pages/login_page.dart';

class DashboardPage extends StatefulWidget {
  static route() =>
      MaterialPageRoute(builder: (context) => const DashboardPage());

  const DashboardPage({super.key});

  @override
  State<DashboardPage> createState() => _DashboardPageState();
}

class _DashboardPageState extends State<DashboardPage> {
  @override
  Widget build(BuildContext context) {
    return MultiBlocListener(
      listeners: [
        BlocListener<AppUserBloc, AppUserState>(
          listener: (context, state) {
            if (state is AppUserUnauthenticated) {
              if (!mounted) return;
              Navigator.of(
                context,
              ).pushAndRemoveUntil(LoginPage.route(), (route) => false);
            }
          },
        ),
      ],
      child: Center(child: Text("Home Page")),
    );
  }
}
