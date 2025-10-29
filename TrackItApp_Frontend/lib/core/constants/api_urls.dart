class ApiUrls {
  static const baseURL = 'http://192.168.1.50:2222';

  //Auth api
  static const register = '$baseURL/api/Auth/register';
  static const login = '$baseURL/api/Auth/login';
  static const verifyAccount = '$baseURL/api/Auth/account-activations/verify';
  static const myAccount = '$baseURL/api/MyAccount';
}
