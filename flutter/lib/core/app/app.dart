import 'package:flutter/material.dart';
import 'package:frontend/core/config/constants.dart';
import 'package:frontend/ui/pages/products.dart';
import 'package:frontend/ui/pages/upload_csv_view.dart';
import 'package:frontend/ui/view_models/product_view_model.dart';
import 'package:frontend/ui/view_models/upload_csv_view_model.dart';
import 'package:get_it/get_it.dart';
import 'package:frontend/ui/app_theme.dart';

class App extends StatelessWidget {
  //
  const App({super.key});

  static void configureDependencies() {
    final getIt = GetIt.instance;
    getIt.registerCachedFactory(() => UploadCsvViewModel());
    getIt.registerCachedFactory(() => ProductViewModel());
  }

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: Constants.appTitle,
      debugShowCheckedModeBanner: false,

      //Theme
      themeMode: ThemeMode.light,
      theme: AppTheme.buildTheme(false),

      //Routes
      initialRoute: '/',
      routes: {
        '/': (context) => const UploadCsvView(),
        '/products': (context) => const ProductsView(),
      },
    );
  }
}
