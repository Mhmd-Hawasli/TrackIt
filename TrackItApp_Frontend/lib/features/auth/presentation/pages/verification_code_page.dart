import 'package:flutter/gestures.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:track_it_health/core/theme/app_palette.dart';
import 'package:track_it_health/features/auth/domain/usecases/verify_account_use_case.dart';
import 'package:track_it_health/features/auth/presentation/bloc/auth_bloc.dart';
import 'package:track_it_health/features/auth/presentation/widgets/auth_field.dart';
import 'package:track_it_health/features/auth/presentation/widgets/auth_gradient_button.dart';
import 'package:track_it_health/features/dashboard/presentation/pages/dashboard_page.dart';

class VerificationCodePage extends StatefulWidget {
  final String input;

  const VerificationCodePage({super.key, required this.input});

  static Route route({required String input}) => MaterialPageRoute(
    builder: (context) => VerificationCodePage(input: input),
  );

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
      body: BlocConsumer<AuthBloc, AuthState>(
        listener: (context, state) {
          if (state is AuthSuccessState) {
            // context.read<AppTokenBloc>().add(
            //   SaveTokenEvent(tokenEntity: state.tokenEntity),
            // );
            if (!mounted) return;
            Navigator.of(
              context,
            ).pushAndRemoveUntil(DashboardPage.route(), (route) => false);
          }
        },
        builder: (context, state) {
          return SafeArea(
            child: Center(
              child: Form(
                key: formKey,
                child: SingleChildScrollView(
                  padding: const EdgeInsets.symmetric(
                    horizontal: 20,
                    vertical: 60,
                  ),
                  reverse: false,
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      const Text(
                        'Verify your account.',
                        style: TextStyle(
                          fontSize: 30,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                      const SizedBox(height: 30),
                      AuthField(hintText: 'Code', controller: codeController),
                      const SizedBox(height: 20),
                      AuthGradientButton(
                        buttonText: 'Verify',
                        onPressed: () {
                          if (formKey.currentState!.validate()) {
                            context.read<AuthBloc>().add(
                              AuthVerifyAccountEvent(
                                verifyAccountParams: VerifyAccountParams(
                                  input: widget.input.trim(),
                                  code: codeController.text.trim(),
                                ),
                              ),
                            );
                          }
                        },
                      ),
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
          );
        },
      ),
    );
  }
}
