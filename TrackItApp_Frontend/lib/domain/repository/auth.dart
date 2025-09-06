import 'package:track_it/core/api/api_response.dart';
import 'package:track_it/data/models/signin_req_params.dart';
import 'package:track_it/data/models/signup_req_params.dart';
import 'package:dartz/dartz.dart';
import 'package:track_it/domain/entities/user.dart';



//this is like IAuthService in asp.ent
//Future => Task
//Either => ApiResponse
//Abstract => Interface
//SigninReqParams => DTO
abstract class AuthRepository {

  //Task<string> signup()
  Future<Either> signup(SignupReqParams signupReq);
  // Future<ApiResponse<Response>> signup2(SignupReqParams signupReq)
  Future<Either> signin(SigninReqParams signinReq);
  Future<bool> isLoggedIn();
  Future<Either> getUser();
  Future<Either> logout();
}
