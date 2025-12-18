import 'package:flutter/material.dart';
import 'package:frontend/core/config/constants.dart';
import 'package:frontend/domain/models/product.dart';
import 'package:frontend/ui/widgets/product/product_tile_detail_medium.dart';
import 'package:frontend/ui/widgets/product/product_tile_detail_small.dart';

class ProductCard extends StatelessWidget {
  //
  final Product product;
  final TextTheme textTheme;
  final double screenWidth;

  const ProductCard(
    this.product,
    this.textTheme,
    this.screenWidth, {
    super.key,
  });

  @override
  Widget build(BuildContext context) {
    return Material(
      elevation: 3,
      child: Container(
        decoration: BoxDecoration(
          border: Border.all(color: Colors.grey.shade400, width: 1),
          borderRadius: BorderRadius.circular(Constants.basePadding),
        ),
        child: Column(
          crossAxisAlignment: .center,
          children: [
            Padding(
              padding: const EdgeInsets.only(top: 4, left: 4),
              child: Text(product.name, overflow: .ellipsis),
            ),
            Divider(
              indent: Constants.basePadding,
              endIndent: Constants.basePadding,
            ),
            const SizedBox(height: Constants.basePadding / 2),
            if (screenWidth < 600)
              ProductTileDetailSmall(product, textTheme)
            else
              ProductTileDetailMedium(product, textTheme),
            const SizedBox(height: Constants.basePadding),
          ],
        ),
      ),
    );
  }
}
