import 'dart:convert';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:track_it_health/core/common/entities/user_entity.dart';

class SecureLocalStorage {
  // Android Options (to use EncryptedSharedPreferences)
  static const AndroidOptions _androidOptions = AndroidOptions(
    encryptedSharedPreferences: true,
  );

  // Create the secure storage instance
  static final FlutterSecureStorage _storage = FlutterSecureStorage(
    aOptions: _androidOptions,
  );

  // Keys
  static const _keyAccessToken = 'access_token';
  static const _keyRefreshToken = 'refresh_token';
  static const _keyUser = 'user';

  // Save Tokens
  Future<void> saveTokens({
    required String accessToken,
    required String refreshToken,
  }) async {
    await _storage.write(key: _keyAccessToken, value: accessToken);
    await _storage.write(key: _keyRefreshToken, value: refreshToken);
  }

  // Read Tokens
  Future<String?> getAccessToken() async =>
      await _storage.read(key: _keyAccessToken);

  Future<String?> getRefreshToken() async =>
      await _storage.read(key: _keyRefreshToken);

  // Clear Tokens
  Future<void> clearTokens() async {
    await _storage.delete(key: _keyAccessToken);
    await _storage.delete(key: _keyRefreshToken);
  }

  // ----------------------------
  // User Storage
  // ----------------------------

  /// Save user to secure storage
  Future<void> saveUser(UserEntity user) async {
    final userJson = jsonEncode({
      'userId': user.userId,
      'name': user.name,
      'username': user.username,
      'email': user.email,
    });
    await _storage.write(key: _keyUser, value: userJson);
  }

  /// Read user from secure storage
  Future<UserEntity?> getUser() async {
    final userJson = await _storage.read(key: _keyUser);
    if (userJson == null) return null;

    final Map<String, dynamic> data = jsonDecode(userJson);
    return UserEntity(
      userId: data['userId'],
      name: data['name'],
      username: data['username'],
      email: data['email'],
    );
  }

  /// Clear user from storage
  Future<void> clearUser() async {
    await _storage.delete(key: _keyUser);
  }

  /// Clear everything (tokens + user)
  Future<void> clearAll() async {
    await clearTokens();
    await clearUser();
  }
}
