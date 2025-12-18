import 'package:dio/dio.dart';
import 'package:file_picker/file_picker.dart';
import 'package:flutter/material.dart';
import 'package:frontend/core/config/constants.dart';

class CsvFileRepository {
  //
  static const url = '${Constants.serverUrl}products/upload';

  static Future<bool> uploadAndImportCsvFile(
    PlatformFile file,
    ProgressCallback onProgress,
  ) async {
    final dio = Dio();
    dio.options.connectTimeout = const Duration(
      seconds: Constants.timeoutSeconds,
    );

    final formData = FormData.fromMap({
      'file': await MultipartFile.fromFile(file.path!, filename: file.name),
    });

    try {
      final response = await dio.post(
        url,
        data: formData,
        onSendProgress: onProgress,
      );

      int statusCode = response.statusCode ?? 0;
      if (statusCode >= 200 && statusCode <= 299) {
        return true;
      }

      return false;
    } on DioException catch (e) {
      debugPrint(e.message);
      return false;
    }
  }
}
