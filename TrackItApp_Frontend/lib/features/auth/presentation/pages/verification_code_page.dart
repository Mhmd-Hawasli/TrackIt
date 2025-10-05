import 'package:flutter/gestures.dart';
import 'package:flutter/material.dart';
import 'package:track_it_health/core/theme/app_palette.dart';
import 'package:track_it_health/features/auth/presentation/widgets/auth_field.dart';
import 'package:track_it_health/features/auth/presentation/widgets/auth_gradient_button.dart';

class VerificationCodePage extends StatefulWidget {
  const VerificationCodePage({super.key});

  static route() =>
      MaterialPageRoute(builder: (context) => const VerificationCodePage());

  @override
  State<VerificationCodePage> createState() => _VerificationCodePageState();
}

class _VerificationCodePageState extends State<VerificationCodePage> {
  final formKey = GlobalKey<FormState>();
  final codeController = TextEditingController();

  @override
  void dispose() {
    codeController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: SafeArea(
        child: Center(
          child: Form(
            key: formKey,
            child: SingleChildScrollView(
              reverse: false,
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  const Text(
                    'Verify your account.',
                    style: TextStyle(fontSize: 40, fontWeight: FontWeight.bold),
                  ),
                  const SizedBox(height: 30),
                  AuthField(hintText: 'Code', controller: codeController),
                  const SizedBox(height: 20),
                  AuthGradientButton(buttonText: 'Verify', onPressed: () {}),
                  const SizedBox(height: 20),
                  RichText(
                    text: TextSpan(
                      text: 'Didn\'t receive the verification code? ',
                      style: Theme.of(context).textTheme.titleMedium,
                      children: [
                        TextSpan(
                          text: 'Resend',
                          style: Theme.of(context).textTheme.titleMedium!
                              .copyWith(
                                color: AppPalette.gradient2,
                                fontWeight: FontWeight.bold,
                              ),
                          recognizer: TapGestureRecognizer()..onTap = () {},
                        ),
                      ],
                    ),
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}
