import 'package:flutter/material.dart';

void main() {
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      debugShowCheckedModeBanner: false, // يشيل شعار الـ debug
      title: 'Flutter Demo',
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
