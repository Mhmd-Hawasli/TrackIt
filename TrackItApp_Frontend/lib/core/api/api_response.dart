class ApiResponse<T> {
  final bool succeeded;
  final T? data;
  final String message;
  final List<String> errors;

  ApiResponse.success({this.data, String? message})
    : succeeded = true,
      message = message ?? "Operation successful",
      errors = [];

  ApiResponse.failure({String? message, List<String>? errors})
    : succeeded = false,
      data = null,
      message = message ?? "An error occurred.",
      errors =errors ?? [];

  ApiResponse.custom({required this.succeeded,this.data, String? message, List<String>? errors,})
      : message = message ?? (succeeded ? "Operation Successful" : "An error occurred."),
        errors = errors ?? [];
}
