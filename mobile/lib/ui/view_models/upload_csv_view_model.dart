import 'dart:async';
import 'package:file_picker/file_picker.dart';
import 'package:flutter/material.dart';
import 'package:frontend/domain/services/csv_file_service.dart';
import 'package:get_it/get_it.dart';

enum UploadStates { selecting, selected, uploading, uploaded, error }

class UploadCsvViewModel extends ChangeNotifier {
  //
  static UploadCsvViewModel get instance =>
      GetIt.instance<UploadCsvViewModel>();

  int _progress = 0;
  int get progress => _progress;
  UploadStates _uploadState = .selecting;

  PlatformFile? _file;
  String get fileName => _file?.name ?? '';
  String get filePath => _file?.path ?? '';

  UploadStates get uploadState => _uploadState;

  String get hintText => _uploadState == .selected
      ? '"$fileName" is ready to be imported'
      : 'Please select a CSV file containing products to import';

  String get selectFileText => _uploadState == .selected
      ? 'Select another file '
      : 'Select a CSV file to import';

  UploadCsvViewModel() {
    restart();
  }

  void restart() {
    _progress = 0;
    _uploadState = UploadStates.selecting;
    _file = null;
    notifyListeners();
  }

  Future<void> chooseCsvFile() async {
    _file = await CsvFileService.chooseCsvFile();
    if (_file == null) {
      return;
    }

    _uploadState = UploadStates.selected;
    notifyListeners();
  }

  Future<void> tryAgain() async {
    _uploadState = .selected;
    notifyListeners();
  }

  Future<bool> uploadFile() async {
    if (_file == null) {
      return false;
    }

    _uploadState = UploadStates.uploading;
    notifyListeners();

    bool result = await CsvFileService.uploadAndImportCsvFile(
      _file!,
      _onProgress,
    );

    _uploadState = result ? .uploaded : .error;
    notifyListeners();

    return result;
  }

  void _onProgress(int progress) {
    _progress = progress;
    notifyListeners();
  }
}
