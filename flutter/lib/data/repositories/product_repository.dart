import 'package:dio/dio.dart';
import 'package:flutter/material.dart';
import 'package:frontend/core/config/constants.dart';

class ProductRepository {
  //
  static const url = '${Constants.serverUrl}products/';

  static Future<Map<String, dynamic>> fetch(int page) async {
    final dio = Dio();
    dio.options.connectTimeout = const Duration(
      seconds: Constants.timeoutSeconds,
    );

    /*
    dio.interceptors.add(
      LogInterceptor(
        requestBody: true,
        responseBody: true,
        requestHeader: true,
        responseHeader: true,
        request: true,
      ),
    );
    */

    final params = <String, dynamic>{
      'page': page,
      'perPage': Constants.recordsPerPage,
    };

    try {
      final response = await dio.get(url, queryParameters: params);

      int statusCode = response.statusCode ?? 0;
      if (statusCode >= 200 && statusCode <= 299) {
        return response.data;
      }
      throw Exception('${response.statusCode}: ${response.statusMessage}');
    } on DioException catch (e) {
      debugPrint(e.message);
      throw Exception('${e.message}');
    }
  }
}
