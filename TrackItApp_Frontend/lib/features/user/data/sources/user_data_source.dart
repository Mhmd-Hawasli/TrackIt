import 'package:dartz/dartz.dart';
import 'package:track_it_health/core/constants/api_urls.dart';
import 'package:track_it_health/core/error/exceptions.dart';
import 'package:track_it_health/core/error/failure.dart';
import 'package:track_it_health/core/network/dio_client.dart';
import 'package:track_it_health/features/user/data/models/user_model.dart';

abstract interface class UserDataSource {
  Future<UserModel> getUserInfo();
}

//==========================================================================
//==========================================================================
class UserDataSourceImpl implements UserDataSource {
  final DioClient _dioClient;

  const UserDataSourceImpl(this._dioClient);

  ///===============================
  /// get User info
  ///===============================
  @override
  Future<UserModel> getUserInfo() async {
    final responseData = await _dioClient.get(ApiUrls.myAccount);
    if (responseData['data'] == null) {
      throw ServerExceptions(responseData['message'] ?? 'an error occurred');
    }
    return UserModel.fromJson(responseData['data']);
  }
}
