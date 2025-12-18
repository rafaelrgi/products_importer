import 'package:flutter/material.dart';
import 'package:frontend/core/config/constants.dart';
import 'package:frontend/ui/view_models/product_view_model.dart';
import 'package:frontend/ui/widgets/csv_file/upload_csv_file_error.dart';
import 'package:frontend/ui/widgets/loading.dart';
import 'package:frontend/ui/widgets/product/list_products.dart';

class ProductsView extends StatelessWidget {
  //
  ProductViewModel get viewModel => ProductViewModel.instance;

  const ProductsView({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text(Constants.appTitle)),

      body: Padding(
        padding: const EdgeInsets.only(top: Constants.basePadding),
        child: ListenableBuilder(
          listenable: viewModel,
          builder: (context, _) {
            switch (viewModel.state) {
              case .needsData:
                viewModel.fetch();
                return Loading();
              case .loading:
                return Loading();
              case .ready:
              case .loadingPage:
                return ListProducts(viewModel: viewModel);
              case .error:
                return UploadCsvFileError(onOk: viewModel.fetch);
            }
          },
        ),
      ),
    );
  }
}
