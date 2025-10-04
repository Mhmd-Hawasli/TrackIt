import 'package:dartz/dartz.dart';
import 'package:dio/dio.dart';
import 'package:track_it_health/core/constants/api_urls.dart';
import 'package:track_it_health/core/error/exceptions.dart';
import 'package:track_it_health/core/network/dio_client.dart';
import 'package:track_it_health/core/service_locator.dart';
import 'package:track_it_health/features/auth/data/models/signup_req_params.dart';
import 'package:track_it_health/features/auth/data/models/user_model.dart';

abstract class AuthApiService {
  Future<Either> signUpOld(SignupReqParams signupReqParams);

  Future<UserModel> signup(Map<String, dynamic> data);

  Future<UserModel> login(Map<String, dynamic> data);
}

////////////////////////////////////////////////////////////////////////////////
class AuthApiServiceImpl extends AuthApiService {
  @override
  Future<Either> signUpOld(SignupReqParams signupReqParams) async {
    try {
      var response = await sl<DioClient>().post(
        ApiUrls.regiter,
        data: signupReqParams.toMap(),
      );
      return right(response);
    } on DioException catch (e) {
      return Left(e.response!.data['message']);
    }
  }

  @override
  Future<UserModel> login(Map<String, dynamic> data) {
    // TODO: implement login
    throw UnimplementedError();
  }

  @override
  Future<UserModel> signup(Map<String, dynamic> data) async {
    try {
      var response = await sl<DioClient>().post(ApiUrls.regiter, data: data);
      return UserModel.fromMap(f);
    } on DioException catch (e) {
      throw ServerExceptions(e.response!.data['message']);
    }
  }
}
