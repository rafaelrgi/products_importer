import 'package:intl/intl.dart';

extension Extensions on DateTime {
  //
  static final dateFormater = DateFormat('MM/dd/yyyy');

  String asString() {
    return dateFormater.format(this);
  }
}
