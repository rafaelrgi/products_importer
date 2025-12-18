import 'package:frontend/ui/widgets/error_message.dart';

class UploadCsvFileError extends ErrorMessage {
  //
  const UploadCsvFileError({
    super.key,
    required super.onOk,
    super.buttonLabel = 'Try again',
    super.text_1 = 'Could not load Products from the server.',
    super.text_2 = 'Please check your connection and try again!',
  });
}
