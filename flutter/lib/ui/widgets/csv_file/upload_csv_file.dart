import 'package:flutter/material.dart';
import 'package:frontend/core/config/constants.dart';
import 'package:frontend/ui/view_models/upload_csv_view_model.dart';

class UploadCsvFile extends StatelessWidget {
  //
  final UploadCsvViewModel viewModel;

  const UploadCsvFile({super.key, required this.viewModel});

  @override
  Widget build(BuildContext context) {
    final textTheme = Theme.of(context).textTheme;

    return Column(
      crossAxisAlignment: .stretch,
      children: [
        const SizedBox(height: Constants.basePadding * 2),
        Center(child: Text('Importing products', style: textTheme.titleLarge)),
        const SizedBox(height: Constants.basePadding),
        Text(
          '"${viewModel.fileName}" is beeing uploaded and processed by the server...',
        ),
        const SizedBox(height: Constants.basePadding),
        Center(
          child: Text('${viewModel.progress}%', style: textTheme.titleMedium),
        ),
        LinearProgressIndicator(
          value: viewModel.progress / 100,
          minHeight: Constants.basePadding * 3,
        ),
        const SizedBox(height: Constants.basePadding * 4),
        OutlinedButton(
          onPressed: viewModel.restart,
          child: const Text('Import another file'),
        ),
      ],
    );
  }
}
