class PaginatedList<T> {
  //
  late PaginatedListInfo info;
  late List<T> data;
}

class PaginatedListInfo {
  //
  PaginatedListInfo({
    required this.page,
    required this.perPage,
    required this.recordCount,
    required this.pageCount,
  });

  late int page;
  late int perPage;
  late int recordCount;
  late int pageCount;
}
