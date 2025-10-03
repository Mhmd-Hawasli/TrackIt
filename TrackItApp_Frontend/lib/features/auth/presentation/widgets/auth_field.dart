import 'package:flutter/material.dart';

class AuthField extends StatelessWidget {
  final TextEditingController controller;
  final bool isObscureText;
  final String hintText;

  const AuthField({
    super.key,
    required this.hintText,
    required this.controller,
    this.isObscureText = false,
  });

  @override
  Widget build(BuildContext context) {
    return TextFormField(
      controller: controller,
      obscureText: isObscureText,
      decoration: InputDecoration(
        labelText: hintText,
        labelStyle: Theme.of(context).textTheme.bodyMedium,
      ),
      validator: (value) {
        if (value!.isEmpty) {
          return "$hintText is missing.";
        }
        return null;
      },
    );
  }
}
