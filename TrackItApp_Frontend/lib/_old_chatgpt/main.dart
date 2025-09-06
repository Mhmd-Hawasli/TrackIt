import 'package:flutter/material.dart';
// import 'features/auth/presentation/screens/login_screen.dart';

void main() {
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      debugShowCheckedModeBanner: true, // يشيل شعار الـ debug
      title: 'TrackIt App',
      theme: ThemeData(
        primarySwatch: Colors.blue, // لون أساسي للتطبيق
      ),
      home: const HomeScreen(), // الصفحة الرئيسية
    );
  }
}

class HomeScreen extends StatelessWidget {
  const HomeScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return const Scaffold(
      body: Center(
        child: Text(
          'Hello Flutter 👋',
          style: TextStyle(fontSize: 24),
        ),
      ),
    );
  }
}
