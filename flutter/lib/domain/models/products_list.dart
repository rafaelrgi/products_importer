import 'package:frontend/domain/models/paginated_list.dart';
import 'package:frontend/domain/models/product.dart';

class ProductsList extends PaginatedList<Product> {
  //

  static ProductsList fromJsonList(Map<String, dynamic> json) {
    final products = ProductsList();

    products.info = PaginatedListInfo(
      page: json['meta']['page'],
      perPage: json['meta']['perPage'],
      recordCount: json['meta']['recordCount'],
      pageCount: json['meta']['pageCount'],
    );

    final records = json['data'] as List<dynamic>;
    //products.data = records.map((p) => _productFromJson(p)).toList();
    final data = records.map((p) => _productFromJson(p)).toList();
    products.data = data;

    return products;
  }

  static Product _productFromJson(Map<String, dynamic> json) {
    final product = Product(
      id: json['Id'],
      name: json['Name'],
      price: double.tryParse(json['Price']) ?? 0,
      expiration: DateTime.parse(json['Expiration']),
      createdAt: DateTime.parse(json['CreatedAt']),
      brl: double.tryParse(json['BRL'] ?? '0') ?? 0,
      eur: double.tryParse(json['EUR'] ?? '0') ?? 0,
      cad: double.tryParse(json['CAD'] ?? '0') ?? 0,
      mxn: double.tryParse(json['MXN'] ?? '0') ?? 0,
      gbp: double.tryParse(json['GBP'] ?? '0') ?? 0,
    );
    return product;
  }
}
