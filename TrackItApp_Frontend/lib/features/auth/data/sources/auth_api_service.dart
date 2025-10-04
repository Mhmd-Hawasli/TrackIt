import 'package:dio/dio.dart';
import 'package:track_it_health/core/constants/api_urls.dart';
import 'package:track_it_health/core/error/exceptions.dart';
import 'package:track_it_health/core/network/dio_client.dart';
import 'package:track_it_health/features/auth/data/models/user_model.dart';

abstract interface class AuthApiService {
  // Future<Either> signUpOld(SignupReqParams signupReqParams);

  Future<UserModel> signup(Map<String, dynamic> data);

  Future<UserModel> login(Map<String, dynamic> data);
}

////////////////////////////////////////////////////////////////////////////////
class AuthApiServiceImpl implements AuthApiService {
  final DioClient _dioClient;

  const AuthApiServiceImpl(DioClient dioClient) : _dioClient = dioClient;

  @override
  Future<UserModel> login(Map<String, dynamic> data) {
    // TODO: implement login
    throw UnimplementedError();
  }

  @override
  Future<UserModel> signup(Map<String, dynamic> data) async {
    try {
      var response = await _dioClient.post(ApiUrls.register, data: data);
      if (response.data == null) {
        throw ServerExceptions("response data is null.");
      }
      return UserModel.fromMap(response.data["data"]);
    } on DioException catch (e) {
      throw ServerExceptions(e.response?.data['message']);
    }
  }
}
