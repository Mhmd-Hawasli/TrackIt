import 'package:track_it_health/core/theme/app_palette.dart';
import 'package:flutter/material.dart';

class AppTheme {
  static _border([Color color = AppPalette.borderColor]) => OutlineInputBorder(
        borderRadius: BorderRadius.circular(10),
        borderSide: BorderSide(
          color: color,
          width: 3,
        ),
      );
  static final darkThemeMode = ThemeData.dark().copyWith(
      scaffoldBackgroundColor: AppPalette.backgroundColor,
      inputDecorationTheme: InputDecorationTheme (
        contentPadding: const EdgeInsets.all(21.0),
        enabledBorder: _border(),
        focusedBorder: _border(AppPalette.gradient2),
        floatingLabelBehavior: FloatingLabelBehavior.auto
      ),
      appBarTheme: const AppBarTheme(
        backgroundColor: AppPalette.backgroundColor,
      ));
}
