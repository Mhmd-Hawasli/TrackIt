import 'package:dio/dio.dart';
import 'package:track_it_health/core/error/exceptions.dart';
import 'package:track_it_health/core/network/interceptors.dart';

class DioClient {
  late final Dio _dio;

  DioClient()
    : _dio = Dio(
        BaseOptions(
          headers: {
            'Content-Type': 'application/json; charset=UTF-8',
            'Device-Id': 'Flutter01',
          },
          responseType: ResponseType.json,
          sendTimeout: const Duration(minutes: 5), // ← مرفوع الزمن
          receiveTimeout: const Duration(minutes: 5),
        ),
      )..interceptors.addAll([LoggerInterceptor()]);

  // ==========================================================
  // Unified Error Handler
  // ==========================================================
  dynamic _handleResponse(Response response) {
    final data = response.data;

    if (data == null) {
      throw ServerExceptions('Response data is null.');
    }

    if (data is Map<String, dynamic>) {
      final succeeded = data['succeeded'];
      final message = data['message'] ?? 'Unknown error occurred.';

      if (succeeded == false) {
        throw ServerExceptions(message);
      }

      return data['data'] ?? message;
    }

    // For non-standard responses
    return data;
  }

  Never _handleError(dynamic error) {
    if (error is DioException) {
      final statusCode = error.response?.statusCode;
      final message =
          error.response?.data?['message'] ?? error.message ?? 'Network error.';

      // Handle timeouts separately
      if (error.type == DioExceptionType.connectionTimeout ||
          error.type == DioExceptionType.receiveTimeout ||
          error.type == DioExceptionType.sendTimeout) {
        throw ServerExceptions('Request timeout. Please try again later.');
      }

      // Unauthorized case
      if (statusCode == 401) {
        throw ServerExceptions('Unauthorized. Please login again.');
      }

      // Server down or invalid response
      if (statusCode == null) {
        throw ServerExceptions('No response from server.');
      }

      throw ServerExceptions(message);
    }

    // Unknown type of exception
    throw ServerExceptions(error.toString());
  }

  // ==========================================================
  // GET
  // ==========================================================
  Future<dynamic> get(
    String url, {
    Map<String, dynamic>? queryParameters,
    Options? options,
    CancelToken? cancelToken,
    ProgressCallback? onReceiveProgress,
  }) async {
    try {
      final response = await _dio.get(
        url,
        queryParameters: queryParameters,
        options: options,
        cancelToken: cancelToken,
        onReceiveProgress: onReceiveProgress,
      );
      return _handleResponse(response);
    } catch (e) {
      _handleError(e);
    }
  }

  // ==========================================================
  // POST
  // ==========================================================
  Future<dynamic> post(
    String url, {
    dynamic data,
    Map<String, dynamic>? queryParameters,
    Options? options,
    CancelToken? cancelToken,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    try {
      final response = await _dio.post(
        url,
        data: data,
        queryParameters: queryParameters,
        options: options,
        cancelToken: cancelToken,
        onSendProgress: onSendProgress,
        onReceiveProgress: onReceiveProgress,
      );
      return _handleResponse(response);
    } catch (e) {
      _handleError(e);
    }
  }

  // ==========================================================
  // PUT
  // ==========================================================
  Future<dynamic> put(
    String url, {
    dynamic data,
    Map<String, dynamic>? queryParameters,
    Options? options,
    CancelToken? cancelToken,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    try {
      final response = await _dio.put(
        url,
        data: data,
        queryParameters: queryParameters,
        options: options,
        cancelToken: cancelToken,
        onSendProgress: onSendProgress,
        onReceiveProgress: onReceiveProgress,
      );
      return _handleResponse(response);
    } catch (e) {
      _handleError(e);
    }
  }

  // ==========================================================
  // DELETE
  // ==========================================================
  Future<dynamic> delete(
    String url, {
    dynamic data,
    Map<String, dynamic>? queryParameters,
    Options? options,
    CancelToken? cancelToken,
  }) async {
    try {
      final response = await _dio.delete(
        url,
        data: data,
        queryParameters: queryParameters,
        options: options,
        cancelToken: cancelToken,
      );
      return _handleResponse(response);
    } catch (e) {
      _handleError(e);
    }
  }
}
