import 'package:flutter/material.dart';
import 'package:frontend/ui/view_models/product_view_model.dart';
import 'package:frontend/ui/widgets/product/product_card.dart';

class ListProducts extends StatelessWidget {
  //
  final ProductViewModel viewModel;

  const ListProducts({super.key, required this.viewModel});

  @override
  Widget build(BuildContext context) {
    final textTheme = Theme.of(context).textTheme;
    final screenWidth = MediaQuery.sizeOf(context).width;

    return ListenableBuilder(
      listenable: viewModel,
      builder: (context, _) {
        return ListView.builder(
          controller: viewModel.scrollController,
          itemCount: viewModel.data!.data.length + 1,
          itemBuilder: (context, index) {
            if (index == viewModel.data!.data.length) {
              return Center(
                child: Padding(
                  padding: const EdgeInsets.all(8.0),
                  child: viewModel.hasMorePages
                      ? CircularProgressIndicator()
                      : Text('         ---- No more data ----'),
                ),
              );
            }
            return ListTile(
              title: ProductCard(
                viewModel.data!.data[index],
                textTheme,
                screenWidth,
              ),
            );
          },
        );
      },
    );
  }
}
