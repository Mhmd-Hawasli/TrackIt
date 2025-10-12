import 'package:dartz/dartz.dart';
import 'package:dio/dio.dart';
import 'package:track_it_health/core/constants/api_urls.dart';
import 'package:track_it_health/core/error/exceptions.dart';
import 'package:track_it_health/core/network/dio_client.dart';
import 'package:track_it_health/features/auth/data/models/token_model.dart';
import 'package:track_it_health/features/auth/data/models/user_model.dart';

abstract interface class AuthApiService {
  Future<String> signup(Map<String, dynamic> data);

  Future<Either<String, TokenModel>> login(Map<String, dynamic> data);
}

////////////////////////////////////////////////////////////////////////////////
class AuthApiServiceImpl implements AuthApiService {
  final DioClient _dioClient;

  const AuthApiServiceImpl(DioClient dioClient) : _dioClient = dioClient;

  @override
  Future<String> signup(Map<String, dynamic> data) async {
    try {
      var response = await _dioClient.post(ApiUrls.register, data: data);
      if (response.data == null) {
        throw ServerExceptions("response data is null.");
      }
      if (response.data['succeeded'] == true) {
        return response.data['message'];
      }
      throw ServerExceptions(response.data['message']);
    } on DioException catch (e) {
      throw ServerExceptions(e.response?.data['message']);
    }
  }

  @override
  Future<Either<String, TokenModel>> login(Map<String, dynamic> data) async {
    try {
      final response = await _dioClient.post(ApiUrls.login, data: data);

      // Handle missing or invalid response
      if (response.data == null || response.data['succeeded'] == false) {
        throw ServerExceptions("Response data is null or request failed.");
      }

      // Case: Account needs verification (no data but has message)
      if (response.data['data'] == null) {
        return left(response.data['message']);
      }

      // Success case → return TokenModel
      return right(TokenModel.fromJson(response.data['data']));
    } on DioException catch (e) {
      // Handle network/server error
      final message =
          e.response?.data['message'] ?? 'Unexpected error occurred.';
      throw ServerExceptions(message);
    }
  }
}
