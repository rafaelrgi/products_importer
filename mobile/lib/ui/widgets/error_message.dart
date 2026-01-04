import 'package:flutter/material.dart';
import 'package:frontend/core/config/constants.dart';

class ErrorMessage extends StatelessWidget {
  //
  final VoidCallback onOk;
  final String? buttonLabel;
  final String? text_1;
  final String? text_2;

  const ErrorMessage({
    super.key,
    required this.onOk,
    this.buttonLabel,
    required this.text_1,
    this.text_2,
  });

  @override
  Widget build(BuildContext context) {
    final textTheme = Theme.of(context).textTheme;

    return Column(
      crossAxisAlignment: .stretch,
      children: [
        const SizedBox(height: Constants.basePadding * 2),
        Center(
          child: Text(
            'Oops, something went wrong!',
            style: textTheme.titleLarge,
          ),
        ),
        const SizedBox(height: Constants.basePadding),
        Text(text_1 ?? 'The operation failed.'),
        if (text_2 != null) Text(text_2!),
        const SizedBox(height: Constants.basePadding * 3),
        ElevatedButton(onPressed: onOk, child: Text(buttonLabel ?? 'OK')),
      ],
    );
  }
}
