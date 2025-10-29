import 'package:dio/dio.dart';
import 'package:logger/logger.dart';
import 'package:track_it_health/core/utils/secure_local_storage.dart';

/// This interceptor is used to show request and response logs
class LoggerInterceptor extends Interceptor {
  final SecureLocalStorage _storage = SecureLocalStorage();
  Logger logger = Logger(
    printer: PrettyPrinter(methodCount: 0, colors: true, printEmojis: true),
  );

  @override
  void onError(DioException err, ErrorInterceptorHandler handler) {
    final options = err.requestOptions;
    final requestPath = '${options.baseUrl}${options.path}';

    final buffer = StringBuffer();

    // 1️⃣ URL
    buffer.writeln('${options.method} request ==> $requestPath');

    // 2️⃣ Empty line
    buffer.writeln();

    // 3️⃣ Status code
    final statusCode = err.response?.statusCode ?? 'No status code';
    buffer.writeln('Status Code: $statusCode');

    // 4️⃣ General Dio error or connection issue
    if (err.response == null) {
      buffer.writeln('Dio/Connection Error: ${err.message}');
    }

    // 5️⃣ Backend response (if available)
    if (err.response != null) {
      final data = err.response?.data;

      final success = data?['succeeded'] ?? data?['Succeeded'];
      final responseData = data?['data'] ?? data?['Data'];
      final message = data?['message'] ?? data?['Message'];
      final errors = data?['errors'] ?? data?['Errors'];

      buffer.writeln('Backend Response:');

      if (success != null) {
        buffer.writeln('  Succeeded: $success');
        buffer.writeln('  Data: $responseData');
        buffer.writeln('  Message: $message');
        buffer.write('  Errors: $errors');
      } else {
        buffer.write(data);
      }
    }
    logger.e(buffer.toString());
    handler.next(err);
  }

  @override
  void onRequest(
    RequestOptions options,
    RequestInterceptorHandler handler,
  ) async {
    // Check if this request should skip token
    final useToken = options.extra['useToken'];

    // Retrieve access token securely from local storage
    final token = await _storage.getAccessToken();
    bool tokenAttached = false;

    // Add Authorization header only if not skipping
    if (useToken && token != null && token.isNotEmpty) {
      tokenAttached = true;
      options.headers['Authorization'] = 'Bearer $token';
    }

    // Build full request path
    final requestPath = '${options.baseUrl}${options.path}';

    // Extract Device-Id header
    final deviceId = options.headers['Device-Id'] ?? 'Not set';

    // Build log message
    final buffer = StringBuffer();
    buffer.writeln('${options.method} request ==> $requestPath\n');
    buffer.writeln('Device-Id header: $deviceId');
    buffer.writeln('Token attached: ${tokenAttached ? "✅ Yes" : "❌ No"}');
    buffer.write('Body: ${options.data}');
    logger.i(buffer.toString());

    // Continue the request
    return handler.next(options);
  }

  @override
  void onResponse(Response response, ResponseInterceptorHandler handler) {
    logger.d(
      'STATUSCODE: ${response.statusCode} \n '
      'STATUSMESSAGE: ${response.statusMessage} \n'
      'HEADERS: ${response.headers} \n'
      'Data: ${response.data}',
    ); // Debug log
    handler.next(response); // continue with the Response
  }
}
