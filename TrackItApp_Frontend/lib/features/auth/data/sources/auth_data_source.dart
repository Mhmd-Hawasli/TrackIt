import 'dart:ui';

import 'package:dartz/dartz.dart';
import 'package:track_it_health/core/constants/api_urls.dart';
import 'package:track_it_health/core/network/dio_client.dart';
import 'package:track_it_health/core/error/exceptions.dart';
import 'package:track_it_health/features/auth/data/models/token_model.dart';
import 'package:track_it_health/features/auth/data/models/user_model.dart';

abstract interface class AuthDataSource {
  Future<String> signupDataSource(Map<String, dynamic> data);

  Future<Either<String, TokenModel>> loginDataSource(Map<String, dynamic> data);

  Future<TokenModel> verifyAccountDataSource(Map<String, dynamic> data);

  Future<UserModel> getCurrentUser();
}

////////////////////////////////////////////////////////////////////////////////
class AuthApiServiceImpl implements AuthDataSource {
  final DioClient _dioClient;

  const AuthApiServiceImpl(this._dioClient);

  //===========================================
  // signup
  //===========================================
  @override
  Future<String> signupDataSource(Map<String, dynamic> data) async {
    final responseData = await _dioClient.post(
      ApiUrls.register,
      data: data,
      useToken: false,
    );
    return responseData['message'];
  }

  //===========================================
  // login
  //===========================================
  @override
  Future<Either<String, TokenModel>> loginDataSource(
    Map<String, dynamic> data,
  ) async {
    final responseData = await _dioClient.post(
      ApiUrls.login,
      data: data,
      useToken: false,
    );

    if (responseData['data'] == null) {
      return left(responseData['message']);
    }
    return right(TokenModel.fromJson(responseData['data']));
  }

  //===========================================
  // verifyAccount
  //===========================================
  @override
  Future<TokenModel> verifyAccountDataSource(Map<String, dynamic> data) async {
    final responseData = await _dioClient.post(
      ApiUrls.verifyAccount,
      data: data,
      useToken: false,
    );
    if (responseData['data'] == null) {
      throw ServerExceptions(responseData['message'] ?? 'an error occurred.');
    }
    return TokenModel.fromJson(responseData['data']);
  }

  //===========================================
  // getCurrentUser
  //===========================================
  @override
  Future<UserModel> getCurrentUser() async {
    final responseData = await _dioClient.get(ApiUrls.myAccount);
    if (responseData['data'] == null) {
      throw ServerExceptions(responseData['message'] ?? 'an error occurred');
    }
    return UserModel.fromJson(responseData['data']);
  }
}
