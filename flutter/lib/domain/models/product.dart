class Product {
  //
  Product({
    required this.id,
    required this.name,
    required this.price,
    required this.expiration,
    required this.createdAt,
    this.brl,
    this.eur,
    this.cad,
    this.mxn,
    this.gbp,
  });

  late int id;
  late String name;
  late double price;
  late DateTime expiration;
  late DateTime createdAt;

  // Brazilian Reais
  double? brl;
  // Euro
  double? eur;
  // Canadian dollar
  double? cad;
  // Mexican Pesos
  double? mxn;
  // Libras
  double? gbp;
}
