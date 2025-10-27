import 'package:flutter/foundation.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:track_it_health/core/common/blocs/app_user/app_user_bloc.dart';
import 'package:track_it_health/core/common/widgets/loader.dart';
import 'package:track_it_health/core/theme/app_palette.dart';
import 'package:track_it_health/core/utils/show_snakbar.dart';
import 'package:track_it_health/features/auth/domain/usecases/login_use_case.dart';
import 'package:track_it_health/features/auth/presentation/bloc/auth_bloc.dart';
import 'package:track_it_health/features/auth/presentation/pages/signup_page.dart';
import 'package:track_it_health/features/auth/presentation/pages/verification_code_page.dart';
import 'package:track_it_health/features/auth/presentation/widgets/auth_field.dart';
import 'package:track_it_health/features/auth/presentation/widgets/auth_gradient_button.dart';
import 'package:flutter/material.dart';
import 'package:flutter/gestures.dart';
import 'package:track_it_health/features/dashboard/presentation/pages/dashboard_page.dart';

class LoginPage extends StatefulWidget {
  static route() => MaterialPageRoute(builder: (context) => const LoginPage());

  const LoginPage({super.key});

  @override
  State<LoginPage> createState() => _LoginPageState();
}

class _LoginPageState extends State<LoginPage> {
  final inputController = TextEditingController();
  final passwordController = TextEditingController();
  final formKey = GlobalKey<FormState>();

  @override
  void dispose() {
    inputController.dispose();
    passwordController.dispose();
    super.dispose();
  }

  /// Handles login button press and triggers the bloc event
  void _onLoginPressed(BuildContext context) {
    FocusScope.of(context).unfocus(); // Close keyboard
    if (formKey.currentState!.validate()) {
      context.read<AuthBloc>().add(
        AuthLoginEvent(
          userLoginParams: UserLoginParams(
            input: inputController.text.trim(),
            password: passwordController.text.trim(),
          ),
        ),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: BlocConsumer<AuthBloc, AuthState>(
        listener: (context, state) async {
          // Listen for auth state changes
          if (state is AuthFailureState) {
            showSnackBar(context, state.message);
          } else if (state is AuthNeedVerifyState) {
            // Navigate to verification page (replace current)
            if (!mounted) return;
            showSnackBar(context, state.message);
            Navigator.of(
              context,
            ).pushReplacement(VerificationCodePage.route(input: state.input));
          } else if (state is AuthSuccessState) {
            // context.read<AppUserBloc>().add(
            //   SaveTokenEvent(tokenEntity: state.tokenEntity),
            // );
            // Navigate to dashboard after successful login
            if (!mounted) return;
            Navigator.of(
              context,
            ).pushAndRemoveUntil(DashboardPage.route(), (route) => false);
          }
        },
        builder: (context, state) {
          // Show loader when state is loading
          if (state is AuthLoadingState) {
            return const Loader();
          }

          // Main login form
          return SingleChildScrollView(
            padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 60),
            child: Form(
              key: formKey,
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  const SizedBox(height: 60),
                  const Text(
                    'Sign In',
                    style: TextStyle(fontSize: 50, fontWeight: FontWeight.bold),
                  ),
                  const SizedBox(height: 30),

                  /// Username / Email field
                  AuthField(
                    hintText: 'Username or Email',
                    controller: inputController,
                  ),
                  const SizedBox(height: 15),

                  /// Password field
                  AuthField(
                    hintText: 'Password',
                    controller: passwordController,
                    isObscureText: true,
                  ),
                  const SizedBox(height: 25),

                  /// Login button
                  AuthGradientButton(
                    buttonText: 'Sign In',
                    onPressed: () => _onLoginPressed(context),
                  ),
                  const SizedBox(height: 25),

                  /// Sign up navigation
                  RichText(
                    text: TextSpan(
                      text: 'Don\'t have an account? ',
                      style: Theme.of(context).textTheme.titleMedium,
                      children: [
                        TextSpan(
                          text: 'Sign Up',
                          style: Theme.of(context).textTheme.titleMedium
                              ?.copyWith(
                                color: AppPalette.gradient2,
                                fontWeight: FontWeight.bold,
                              ),
                          recognizer: TapGestureRecognizer()
                            ..onTap = () {
                              Navigator.of(context).push(SignUpPage.route());
                            },
                        ),
                      ],
                    ),
                  ),
                  const SizedBox(height: 30),
                ],
              ),
            ),
          );
        },
      ),
    );
  }
}
