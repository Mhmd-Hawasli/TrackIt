import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:track_it_health/common/blocs/app_token/app_token_bloc.dart';
import 'package:track_it_health/common/blocs/app_user/app_user_bloc.dart';

class DashboardPage extends StatelessWidget {
  static route() =>
      MaterialPageRoute(builder: (context) => const DashboardPage());

  const DashboardPage({super.key});

  @override
  Widget build(BuildContext context) {
    return MultiBlocListener(
      listeners: [
        BlocListener<AppTokenBloc, AppTokenState>(
          listener: (context, state) {},
        ),
        BlocListener<AppUserBloc, AppUserState>(listener: (context, state) {}),
      ],
      child: Center(child: Text("Home Page")),
    );
  }
}
