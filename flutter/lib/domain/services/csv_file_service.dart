import 'dart:async';

import 'package:file_picker/file_picker.dart';
import 'package:frontend/core/config/constants.dart';
import 'package:frontend/data/repositories/csv_file_repository.dart';

class CsvFileService {
  //
  static Future<PlatformFile?> chooseCsvFile() async {
    FilePickerResult? result = await FilePicker.platform.pickFiles(
      type: FileType.custom,
      allowedExtensions: ['csv'],
    );
    if (result == null) null;

    return result!.files.single;
  }

  static Future<bool> uploadAndImportCsvFile(
    PlatformFile file,
    OnProgress onProgress,
  ) async {
    final uploadProgress = _UploadProgress(onProgress);

    bool result = await CsvFileRepository.uploadAndImportCsvFile(
      file,
      uploadProgress.doProgress,
    );

    if (result) {
      uploadProgress.finish();
    }

    return result;
  }
}

/*
* Internal private class just to isolate the upload/processing progress logic.
* As we only receive progress updates for the uploading, we reserve the last
* _processingShare % of the progress for the file processing stage, resulting
* in a smoother and more realistic progress bar
* Also, huge files call progress many times for the same progress value
*/
class _UploadProgress {
  //
  static const _processingShare = 0.22; //0.20 = 20%
  final OnProgress _onProgress;
  Timer? _timer;
  bool _stopped = false;
  int _progress = 0;
  int _max = 0;
  int _lastProgress = 0;

  _UploadProgress(this._onProgress);

  void doProgress(int sentBytes, int totalBytes) {
    //reserve _processingShare % of the progress for the file processing
    if (_max == 0) {
      _max = (totalBytes * (1 + _processingShare)).truncate();
    }
    final calcProgress = ((sentBytes / _max) * 100).truncate();
    //move the progress even for very small numbers
    if (calcProgress == 0) {
      _progress = 2;
    } else if (calcProgress > _progress) {
      _progress = calcProgress;
    }
    //huge files call progress many times for the same progress value
    if (_progress == _lastProgress) return;
    _lastProgress = _progress;

    _callOnProgress();
    //keep increasing the progress during the processing phase
    if (_timer == null && _progress > 100 - _processingShare) {
      _timer = Timer.periodic(
        const Duration(milliseconds: 1024),
        _increaseProgress,
      );
    }
  }

  void stop() {
    _stopped = true;
    _timer?.cancel();
  }

  void finish() {
    _timer?.cancel();
    if (_stopped) return;

    _progress = 100;
    _callOnProgress();
    _stopped = true;
  }

  void _callOnProgress() {
    if (_progress > 100) {
      _progress = 100;
    }
    if (_stopped) {
      return;
    }
    _onProgress(_progress);
  }

  void _increaseProgress(Timer timer) {
    if (++_progress > 97) {
      _timer?.cancel();
    }
    if (_stopped) return;
    _callOnProgress();
  }
}
