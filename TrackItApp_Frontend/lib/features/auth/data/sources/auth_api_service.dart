import 'package:dartz/dartz.dart';
import 'package:track_it_health/core/constants/api_urls.dart';
import 'package:track_it_health/core/network/dio_client.dart';
import 'package:track_it_health/core/error/exceptions.dart';
import 'package:track_it_health/features/auth/data/models/token_model.dart';

abstract interface class AuthApiService {
  Future<String> signup(Map<String, dynamic> data);

  Future<Either<String, TokenModel>> login(Map<String, dynamic> data);

  Future<TokenModel> verifyAccount(Map<String, dynamic> data);
}

////////////////////////////////////////////////////////////////////////////////
class AuthApiServiceImpl implements AuthApiService {
  final DioClient _dioClient;

  const AuthApiServiceImpl(this._dioClient);

  @override
  Future<String> signup(Map<String, dynamic> data) async {
    return await _dioClient.post(ApiUrls.register, data: data);
  }

  @override
  Future<Either<String, TokenModel>> login(Map<String, dynamic> data) async {
    try {
      final result = await _dioClient.post(ApiUrls.login, data: data);

      // إذا رجع String معناها رسالة "verify your account"
      if (result is String) {
        return left(result);
      }

      // إذا رجعت Map → حولها لـ TokenModel
      return right(TokenModel.fromJson(result));
    } on ServerExceptions catch (e) {
      throw ServerExceptions(e.message);
    }
  }

  @override
  Future<TokenModel> verifyAccount(Map<String, dynamic> data) async {
    return await _dioClient.post(ApiUrls.verifyAccount, data: data);
  }
}
