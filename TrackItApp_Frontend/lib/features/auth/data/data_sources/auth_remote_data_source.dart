import 'package:supabase_flutter/supabase_flutter.dart';

abstract interface class AuthRemoteDataSource {
  Future<String> signUp({
    required String name,
    required String username,
    required String email,
    required String password,
  });

  Future<String> login({
    required String usernameOrEmail,
    required String password,
  });
}

class AuthRemoteDataSourceImpl extends AuthRemoteDataSource {
  final SupabaseClient supabaseClient;

  AuthRemoteDataSourceImpl(this.supabaseClient);

  @override
  Future<String> signUp({
    required String name,
    required String username,
    required String email,
    required String password,
  }) {
    throw UnimplementedError();
  }

  @override
  Future<String> login({
    required String usernameOrEmail,
    required String password,
  }) {
    throw UnimplementedError();
  }
}
