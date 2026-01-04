import 'package:flutter/material.dart';

class AppTheme {
  //
  static const colorLight = Colors.purple;
  static final colorLight2 = Colors.purple[200];
  static const colorDark = Colors.purple;
  static final colorDark2 = Colors.deepPurple;
  static const bool _whiteTitle = true;

  static ThemeData buildTheme(bool isDarkTheme) {
    final color = isDarkTheme ? colorDark : colorLight;

    return ThemeData(
      colorScheme: .fromSeed(
        seedColor: color,
        brightness: isDarkTheme ? .dark : .light,
      ),
      appBarTheme: _appBarTheme(isDarkTheme),
      inputDecorationTheme: InputDecorationTheme(
        enabledBorder: OutlineInputBorder(
          borderSide: BorderSide(width: 2, color: color),
        ),
        focusedBorder: OutlineInputBorder(
          borderSide: BorderSide(width: 2, color: color),
        ),
      ),
      dialogTheme: _dialogTheme(isDarkTheme),
      elevatedButtonTheme: ElevatedButtonThemeData(
        style: ElevatedButton.styleFrom(
          enableFeedback: false,
          backgroundColor: color,
          foregroundColor: _whiteTitle ? Colors.white : Colors.black,
          shape: RoundedRectangleBorder(borderRadius: .circular(2)),
        ),
      ),
      outlinedButtonTheme: OutlinedButtonThemeData(
        style: ElevatedButton.styleFrom(
          enableFeedback: false,
          shape: RoundedRectangleBorder(borderRadius: .circular(2)),
        ),
      ),
      iconButtonTheme: IconButtonThemeData(
        style: IconButton.styleFrom(enableFeedback: false),
      ),
      floatingActionButtonTheme: FloatingActionButtonThemeData(
        enableFeedback: false,
      ),
      textButtonTheme: TextButtonThemeData(
        style: IconButton.styleFrom(enableFeedback: false),
      ),
      listTileTheme: ListTileThemeData(enableFeedback: false),
      progressIndicatorTheme: ProgressIndicatorThemeData(color: color),
    );
  }

  static AppBarTheme _appBarTheme(bool isDarkTheme) {
    //Keep the default color for dark theme
    return isDarkTheme
        ? AppBarTheme()
        : AppBarTheme(
            backgroundColor: colorLight,
            foregroundColor: _whiteTitle ? Colors.white : Colors.black,
            systemOverlayStyle: _whiteTitle ? .light : .dark,
            elevation: 4,
            shadowColor: Colors.black,
          );
  }

  static DialogThemeData _dialogTheme(bool isDarkTheme) {
    return DialogThemeData(
      backgroundColor: isDarkTheme ? colorDark2 : Colors.white,
      shape: const RoundedRectangleBorder(borderRadius: .all(.circular(4))),
    );
  }
}
