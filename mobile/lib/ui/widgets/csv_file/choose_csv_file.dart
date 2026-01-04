import 'package:flutter/material.dart';
import 'package:frontend/core/config/constants.dart';
import 'package:frontend/ui/view_models/upload_csv_view_model.dart';

class ChooseCsvFile extends StatelessWidget {
  //
  final UploadCsvViewModel viewModel;

  const ChooseCsvFile({super.key, required this.viewModel});

  @override
  Widget build(BuildContext context) {
    final textTheme = Theme.of(context).textTheme;

    return Column(
      crossAxisAlignment: .stretch,
      children: [
        const SizedBox(height: Constants.basePadding * 2),
        Center(child: Text('Import products', style: textTheme.titleLarge)),
        const SizedBox(height: Constants.basePadding),
        Text(viewModel.hintText),
        if (viewModel.uploadState == UploadStates.selected)
          const Text('The file will be uploaded and processed by the server'),
        const SizedBox(height: Constants.basePadding * 2),
        OutlinedButton(
          onPressed: viewModel.chooseCsvFile,
          child: Text(viewModel.selectFileText),
        ),
        const SizedBox(height: Constants.basePadding * 2),
        ElevatedButton(
          onPressed: viewModel.uploadState == .selected
              ? viewModel.uploadFile
              : null,
          child: const Text('Import products'),
        ),
      ],
    );
  }
}
