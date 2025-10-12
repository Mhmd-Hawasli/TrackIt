import 'package:flutter/material.dart';

class DashboardPage extends StatelessWidget {
  static route() =>
      MaterialPageRoute(builder: (context) => const DashboardPage());

  const DashboardPage({super.key});

  @override
  Widget build(BuildContext context) {
    return Center(child: Text("Home Page"));
  }
}
