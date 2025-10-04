import 'package:dartz/dartz.dart';
import 'package:dio/dio.dart';
import 'package:track_it_health/core/constants/api_urls.dart';
import 'package:track_it_health/core/network/dio_client.dart';
import 'package:track_it_health/core/service_locator.dart';
import 'package:track_it_health/features/auth/data/models/signup_req_params.dart';

abstract class AuthApiService {
  Future<Either> signUp(SignupReqParams signupReqParams);
}

class AuthApiServiceImpl extends AuthApiService {
  @override
  Future<Either> signUp(SignupReqParams signupReqParams) async {
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
}
