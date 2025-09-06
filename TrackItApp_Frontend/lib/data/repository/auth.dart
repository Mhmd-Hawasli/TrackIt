import 'package:track_it/data/models/signin_req_params.dart';
import 'package:track_it/data/models/user.dart';
import 'package:track_it/data/source/auth_api_service.dart';
import 'package:track_it/data/source/auth_local_service.dart';
import 'package:track_it/domain/repository/auth.dart';
import 'package:track_it/service_locator.dart';
import 'package:dartz/dartz.dart';
import 'package:dio/dio.dart';
import 'package:shared_preferences/shared_preferences.dart';

import '../models/signup_req_params.dart';

class AuthRepositoryImpl extends AuthRepository {

  // // هذا هو implementation للـ abstract class
  // @override
  // Future<ApiResponse<Response>> signup(SignupReqParams signupReq) async {
  //   try {
  //     // استدعاء الـ API من خلال service
  //     Response response = await sl<AuthApiService>().signup(signupReq);
  //
  //     // حفظ التوكن بالـ SharedPreferences
  //     SharedPreferences sharedPreferences = await SharedPreferences.getInstance();
  //     sharedPreferences.setString('token', response.data['token']);
  //
  //     // رجّع نتيجة ناجحة
  //     return ApiResponse.success(
  //       data: response,
  //       message: "Signup successful",
  //     );
  //   } catch (error) {
  //     // رجّع نتيجة فشل مع رسالة الخطأ
  //     return ApiResponse.failure(
  //       message: error.toString(),
  //     );
  //   }
  // }

  //this is the implementation for the abstract class
  //Either provide two type of method right for return success = true and life o
  @override
  Future<Either> signup(SignupReqParams signupReq) async {
   Either result = await sl<AuthApiService>().signup(signupReq);
   return result.fold(
    (error){
      return Left(error);
    },
    (data) async {
      Response response = data;
      SharedPreferences sharedPreferences = await SharedPreferences.getInstance();
      sharedPreferences.setString('token', response.data['token']);
      return Right(response);
    }
    );
  }
  
  @override
  Future<bool> isLoggedIn() async {
    return await sl<AuthLocalService>().isLoggedIn();
  }
  
  @override
  Future<Either> getUser() async {
    Either result = await sl<AuthApiService>().getUser();
    return result.fold(
      (error){
        return Left(error);
      },
      (data) {
        Response response = data;
        var userModel = UserModel.fromMap(response.data);
        var userEntity = userModel.toEntity();
        return Right(userEntity);
      }
     );
  }
  
  @override
  Future<Either> logout() async {
    return await sl<AuthLocalService>().logout();
  }

  @override
  Future < Either > signin(SigninReqParams signinReq) async {
    Either result = await sl < AuthApiService > ().signin(signinReq);
    return result.fold(
      (error) {
        return Left(error);
      },
      (data) async {
        Response response = data;
        SharedPreferences sharedPreferences = await SharedPreferences.getInstance();
        sharedPreferences.setString('token', response.data['token']);
        return Right(response);
      }
    );
  }
  
}
