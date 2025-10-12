import 'package:flutter/gestures.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:track_it_health/common/widgets/loader.dart';
import 'package:track_it_health/core/theme/app_palette.dart';
import 'package:track_it_health/core/utils/show_snakbar.dart';
import 'package:track_it_health/features/auth/domain/usecases/signup_usecase.dart';
import 'package:track_it_health/features/auth/presentation/bloc/auth_bloc.dart';
import 'package:track_it_health/features/auth/presentation/pages/login_page.dart';
import 'package:track_it_health/features/auth/presentation/pages/verification_code_page.dart';
import 'package:track_it_health/features/auth/presentation/widgets/auth_field.dart';
import 'package:track_it_health/features/auth/presentation/widgets/auth_gradient_button.dart';
import 'package:flutter/material.dart';

class SignUpPage extends StatefulWidget {
  static route() => MaterialPageRoute(builder: (context) => const SignUpPage());

  const SignUpPage({super.key});

  @override
  State<SignUpPage> createState() => _SignUpPageState();
}

class _SignUpPageState extends State<SignUpPage> {
  final nameController = TextEditingController();
  final usernameController = TextEditingController();
  final emailController = TextEditingController();
  final passwordController = TextEditingController();
  final formKey = GlobalKey<FormState>();

  @override
  void dispose() {
    nameController.dispose();
    usernameController.dispose();
    emailController.dispose();
    passwordController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      // appBar: AppBar(),
      body: SafeArea(
        minimum: EdgeInsets.all(15.0),
        child: BlocConsumer<AuthBloc, AuthState>(
          listener: (context, state) {
            if (state is AuthFailureState) {
              showSnackBar(context, state.message);
            } else if (state is AuthNeedVerifyState) {
              showSnackBar(context, state.message);
              Navigator.of(
                context,
              ).push(VerificationCodePage.route(input: state.input));
            }
          },
          builder: (context, state) {
            if (state is AuthLoadingState) {
              return const Loader();
            }
            return Form(
              key: formKey,
              child: SingleChildScrollView(
                padding: EdgeInsets.only(
                  bottom: MediaQuery.of(context).viewInsets.bottom,
                ),
                reverse: false,
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    const Text(
                      'Sign Up.',
                      style: TextStyle(
                        fontSize: 60,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                    const SizedBox(height: 30),
                    AuthField(hintText: 'Name', controller: nameController),
                    const SizedBox(height: 15),
                    AuthField(
                      hintText: 'Username',
                      controller: usernameController,
                    ),
                    const SizedBox(height: 15),
                    AuthField(hintText: 'Email', controller: emailController),
                    const SizedBox(height: 15),
                    AuthField(
                      hintText: 'Password',
                      controller: passwordController,
                      isObscureText: true,
                    ),
                    const SizedBox(height: 20),
                    AuthGradientButton(
                      buttonText: 'Sign Up',
                      onPressed: () {
                        if (formKey.currentState!.validate()) {
                          context.read<AuthBloc>().add(
                            AuthSignUpEvent(
                              userSignUpParams: UserSignUpParams(
                                name: nameController.text.trim(),
                                username: usernameController.text.trim(),
                                email: emailController.text.trim(),
                                password: passwordController.text.trim(),
                              ),
                            ),
                          );
                        }
                      },
                    ),
                    const SizedBox(height: 20),
                    RichText(
                      text: TextSpan(
                        text: 'Already have an account? ',
                        style: Theme.of(context).textTheme.titleMedium,
                        children: [
                          TextSpan(
                            text: 'Sign In',
                            style: Theme.of(context).textTheme.titleMedium!
                                .copyWith(
                                  color: AppPalette.gradient2,
                                  fontWeight: FontWeight.bold,
                                ),
                            recognizer: TapGestureRecognizer()
                              ..onTap = () {
                                Navigator.of(context).push(LoginPage.route());
                              },
                          ),
                        ],
                      ),
                    ),
                  ],
                ),
              ),
            );
          },
        ),
      ),
    );
  }
}
