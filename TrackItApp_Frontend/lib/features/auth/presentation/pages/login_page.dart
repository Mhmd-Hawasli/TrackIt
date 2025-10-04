import 'package:track_it_health/core/theme/app_palette.dart';
import 'package:track_it_health/features/auth/presentation/pages/signup_page.dart';
import 'package:track_it_health/features/auth/presentation/widgets/auth_field.dart';
import 'package:track_it_health/features/auth/presentation/widgets/auth_gradient_button.dart';
import 'package:flutter/material.dart';
import 'package:flutter/gestures.dart';

class LoginPage extends StatefulWidget {
  static route() => MaterialPageRoute(builder: (context) => const LoginPage());

  const LoginPage({super.key});

  @override
  State<LoginPage> createState() => _LoginPageState();
}

class _LoginPageState extends State<LoginPage> {
  final usernameOrEmailController = TextEditingController();
  final passwordController = TextEditingController();
  final formKey = GlobalKey<FormState>();

  @override
  void dispose() {
    usernameOrEmailController.dispose();
    passwordController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Padding(
        padding: const EdgeInsets.all(15.0),
        child: Form(
          key: formKey,
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              const Text(
                'Sign In.',
                style: TextStyle(fontSize: 60, fontWeight: FontWeight.bold),
              ),
              const SizedBox(height: 30),
              AuthField(
                hintText: 'Username or Email',
                controller: usernameOrEmailController,
              ),
              const SizedBox(height: 15),
              AuthField(
                hintText: 'Password',
                controller: passwordController,
                isObscureText: true,
              ),
              const SizedBox(height: 20),
              AuthGradientButton(buttonText: 'Sign In', onPressed: () {}),
              const SizedBox(height: 20),

              RichText(
                text: TextSpan(
                  text: 'Don\'t have an account? ',
                  style: Theme.of(context).textTheme.titleMedium,
                  children: [
                    TextSpan(
                      text: 'Sign Up',
                      style: Theme.of(context).textTheme.titleMedium?.copyWith(
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
            ],
          ),
        ),
      ),
    );
  }
}
