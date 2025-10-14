import 'package:flutter_secure_storage/flutter_secure_storage.dart';

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

  // Save Tokens
  static Future<void> saveTokens({
    required String accessToken,
    required String refreshToken,
  }) async {
    await _storage.write(key: _keyAccessToken, value: accessToken);
    await _storage.write(key: _keyRefreshToken, value: refreshToken);
  }

  // Read Tokens
  static Future<String?> getAccessToken() async =>
      await _storage.read(key: _keyAccessToken);

  static Future<String?> getRefreshToken() async =>
      await _storage.read(key: _keyRefreshToken);

  // Clear Tokens
  static Future<void> clearTokens() async {
    await _storage.delete(key: _keyAccessToken);
    await _storage.delete(key: _keyRefreshToken);
  }
}
