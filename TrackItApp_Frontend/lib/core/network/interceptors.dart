import 'package:dio/dio.dart';
import 'package:logger/logger.dart';
import 'package:track_it_health/core/utils/secure_local_storage.dart';

/// This interceptor is used to show request and response logs
class LoggerInterceptor extends Interceptor {
  Logger logger = Logger(
    printer: PrettyPrinter(methodCount: 0, colors: true, printEmojis: true),
  );

  @override
  void onError(DioException err, ErrorInterceptorHandler handler) {
    final options = err.requestOptions;
    final requestPath = '${options.baseUrl}${options.path}';

    logger.e('${options.method} request ==> $requestPath');

    if (err.response != null) {
      final data = err.response?.data;
      logger.d("❌ Backend Error: ${data["message"]}");
      logger.d("Errors: ${data["errors"]}");
    } else {
      logger.d("❌ Dio Error: ${err.message}");
    }

    handler.next(err);
  }

  @override
  void onRequest(
    RequestOptions options,
    RequestInterceptorHandler handler,
  ) async {
    // Retrieve access token securely from local storage
    final token = await SecureLocalStorage.getAccessToken();

    // Add Authorization header if token exists
    if (token != null && token.isNotEmpty) {
      options.headers['Authorization'] = 'Bearer $token';
    }

    // Build and log full request path
    final requestPath = '${options.baseUrl}${options.path}';
    logger.i('${options.method} request ==> $requestPath'); // Info log

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
