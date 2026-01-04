import 'package:flutter/material.dart';
import 'package:frontend/domain/models/products_list.dart';
import 'package:frontend/domain/services/product_service.dart';
import 'package:get_it/get_it.dart';

enum ProductStates { needsData, loading, loadingPage, ready, error }

class ProductViewModel extends ChangeNotifier {
  //
  static ProductViewModel get instance => GetIt.instance<ProductViewModel>();

  final ScrollController scrollController = ScrollController();

  ProductStates _state = .needsData;
  ProductStates get state => _state;

  ProductsList? _data;

  ProductsList? get data => _data;

  bool get hasMorePages =>
      (_data != null && _data!.info.page < _data!.info.pageCount);

  bool get isLoadingPage => state == .loadingPage;

  Future<void> fetch() async {
    _state = .loading;
    notifyListeners();

    try {
      _data = await ProductService.fetch(1);
      _state = .ready;
      scrollController.removeListener(_scrollListener);
      scrollController.addListener(_scrollListener);
    } catch (e) {
      _data = null;
      _state = .error;
    } finally {
      notifyListeners();
    }
  }

  @override
  void dispose() {
    scrollController.dispose();
    super.dispose();
  }

  void _scrollListener() {
    //if scroll to the end and has more pages, load the next page
    if (state != .loadingPage &&
        hasMorePages &&
        scrollController.position.pixels >=
            scrollController.position.maxScrollExtent * 0.9) {
      _fetchNextPage();
      return;
    }
  }

  Future<void> _fetchNextPage() async {
    _state = .loadingPage;
    notifyListeners();

    try {
      //load data anda add records to the existing list (yeah, keep the old ones!!!)
      //the listview lazy loading infinite scroll needs it
      final newData = await ProductService.fetch(_data!.info.page + 1);
      _data!.info = newData.info;
      _data!.data.addAll(newData.data);
      _state = .ready;
      scrollController.removeListener(_scrollListener);
      scrollController.addListener(_scrollListener);
    } catch (e) {
      _data = null;
      _state = .error;
    } finally {
      notifyListeners();
    }
  }
}
