import 'package:flutter/material.dart';
import 'package:frontend/core/utils/str_utils.dart';
import 'package:frontend/domain/models/product.dart';
import 'package:frontend/ui/widgets/text_cell.dart';

class ProductTileDetailSmall extends StatelessWidget {
  //
  final Product product;
  final TextTheme textTheme;

  const ProductTileDetailSmall(this.product, this.textTheme, {super.key});

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        Row(
          mainAxisAlignment: .spaceEvenly,
          children: [
            Flexible(
              child: TextCell(
                'Expiration',
                product.expiration.asString(),
                textTheme,
              ),
            ),
            Flexible(child: TextCell('Price', '${product.price}', textTheme)),
            Flexible(child: TextCell('BRL', '${product.brl}', textTheme)),
          ],
        ),
        Row(
          mainAxisAlignment: .spaceEvenly,
          children: [
            Flexible(child: TextCell('EUR', '${product.eur}', textTheme)),
            Flexible(child: TextCell('CAD', '${product.cad}', textTheme)),
            Flexible(child: TextCell('MXN', '${product.mxn}', textTheme)),
            Flexible(child: TextCell('GBP', '${product.gbp}', textTheme)),
          ],
        ),
      ],
    );
  }
}
