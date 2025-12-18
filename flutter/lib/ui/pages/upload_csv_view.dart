import 'package:flutter/material.dart';
import 'package:frontend/core/config/constants.dart';
import 'package:frontend/ui/view_models/upload_csv_view_model.dart';
import 'package:frontend/ui/widgets/csv_file/choose_csv_file.dart';
import 'package:frontend/ui/widgets/csv_file/done_csv_file.dart';
import 'package:frontend/ui/widgets/csv_file/upload_csv_file.dart';
import 'package:frontend/ui/widgets/error_message.dart';

class UploadCsvView extends StatelessWidget {
  //
  UploadCsvViewModel get viewModel => UploadCsvViewModel.instance;

  const UploadCsvView({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text(Constants.appTitle)),
      body: Padding(
        padding: const EdgeInsets.all(Constants.basePadding),
        child: ListenableBuilder(
          listenable: viewModel,
          builder: (context, _) {
            //This page handles the csv file, loading different widgets for each
            //phase of the process: ChooseCsvFile, UploadCsvFile, DoneCsvFile, etc.
            switch (viewModel.uploadState) {
              case UploadStates.selecting:
              case UploadStates.selected:
                return ChooseCsvFile(viewModel: viewModel);
              case UploadStates.uploading:
                return UploadCsvFile(viewModel: viewModel);
              case UploadStates.uploaded:
                return DoneCsvFile(viewModel: viewModel);
              case UploadStates.error:
                return ErrorMessage(
                  onOk: viewModel.tryAgain,
                  buttonLabel: 'Try again',
                  text_1: 'We were unable to import the file.',
                  text_2:
                      'Please check your connection, your file and try again!',
                );
            }
          },
        ),
      ),
    );
  }
}
