import 'package:flutter/material.dart';
import 'package:frontend/core/config/constants.dart';
import 'package:frontend/ui/view_models/upload_csv_view_model.dart';

class DoneCsvFile extends StatelessWidget {
  //
  final UploadCsvViewModel viewModel;

  const DoneCsvFile({super.key, required this.viewModel});

  void _viewProducts(BuildContext context) {
    final navigator = Navigator.of(context);
    viewModel.restart();
    navigator.pushNamed('/products');
  }

  @override
  Widget build(BuildContext context) {
    final textTheme = Theme.of(context).textTheme;

    return Column(
      crossAxisAlignment: .stretch,
      children: [
        const SizedBox(height: Constants.basePadding * 2),
        Center(child: Text('File imported!', style: textTheme.titleLarge)),
        const SizedBox(height: Constants.basePadding),
        Text('"${viewModel.fileName}" was successfully imported.'),
        const Text('Choose what you want to do next:'),
        const SizedBox(height: Constants.basePadding * 2),
        ElevatedButton(
          onPressed: () => _viewProducts(context),
          child: const Text('See the uploaded products'),
        ),
        const SizedBox(height: Constants.basePadding),
        OutlinedButton(
          onPressed: viewModel.restart,
          child: const Text('Import another file'),
        ),
      ],
    );
  }
}
