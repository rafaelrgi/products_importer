import 'package:frontend/data/repositories/product_repository.dart';
import 'package:frontend/domain/models/products_list.dart';

class ProductService {
  //

  static Future<ProductsList> fetch(int page) async {
    final ProductsList products = ProductsList.fromJsonList(
      await ProductRepository.fetch(page),
    );
    return products;
  }
}
