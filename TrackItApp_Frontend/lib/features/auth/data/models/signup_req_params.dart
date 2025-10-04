import 'package:flutter/material.dart';

//rename it SignupRequestModel
class SignupReqParams {
  final String name;
  final String username;
  final String email;
  final String password;

  SignupReqParams({
    required this.name,
    required this.username,
    required this.email,
    required this.password,
  });

  Map<String, dynamic> toMap() {
    return <String, dynamic>{
      'name': this.name,
      'username': this.username,
      'email': this.email,
      'password': this.password,
    };
  }
}
