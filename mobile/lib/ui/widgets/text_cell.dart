import 'package:flutter/material.dart';
import 'package:frontend/core/config/constants.dart';

class TextCell extends StatelessWidget {
  //
  final String label;
  final String text;
  final TextTheme textTheme;

  const TextCell(this.label, this.text, this.textTheme, {super.key});

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: EdgeInsets.all(Constants.basePadding / 2),
      padding: .all(2),
      decoration: BoxDecoration(
        borderRadius: .circular(4),
        border: .all(color: Colors.grey.shade400, width: 0.8),
      ),
      child: Column(
        crossAxisAlignment: .start,
        children: [
          Text(label, style: textTheme.bodySmall, overflow: .ellipsis),
          Align(
            alignment: .bottomRight,
            child: Text(
              ' $text ',
              style: textTheme.bodyMedium,
              overflow: .ellipsis,
            ),
          ),
        ],
      ),
    );
  }
}
