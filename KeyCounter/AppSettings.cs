using System;
using System.ComponentModel;
using System.Data.SQLite;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

namespace KeyCounter {
    public class AppSettings {
        private static string colorPattern = @"^#?[a-f0-9]{6}$";

        // Default / Fallback values
        public static readonly Vector FontSizes = new Vector(20, 72);

        static class Fallback {
            public static readonly string textColor = "#FFFFFF";
            public static readonly string bgColor = "#FF00FF";
            public static readonly string fontName = "Arial";
            public static readonly FamilyTypeface fontStyle = new FamilyTypeface() { Style = FontStyles.Normal, Weight = FontWeights.Normal };
            public static readonly double fontSize = FontSizes.X;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        string _textColor = string.Empty;
        public string TextColor {
            get {
                if (_textColor.Equals(string.Empty)) {
                    _textColor = Fallback.textColor;
                    using (SQLiteCommand command = new SQLiteCommand("SELECT value FROM settings WHERE name = @Name", App.DbConnection)) {
                        command.Parameters.Add("@Name", System.Data.DbType.String);
                        command.Parameters["@Name"].Value = "textColor";
                        SQLiteDataReader r = command.ExecuteReader();
                        if (r.Read()) { _textColor = r.GetString(0); }
                    }
                }
                return _textColor.ToUpper();
            }
            set {
                if (Regex.IsMatch(value, colorPattern, RegexOptions.IgnoreCase)) {
                    _textColor = value.StartsWith("#") ? value : "#" + value;
                    using (SQLiteCommand command = new SQLiteCommand("INSERT INTO settings (name, value) VALUES (@Name, @Value)", App.DbConnection)) {
                        command.Parameters.Add("@Name", System.Data.DbType.String);
                        command.Parameters.Add("@Value", System.Data.DbType.String);
                        command.Parameters["@Name"].Value = "textColor";
                        command.Parameters["@Value"].Value = _textColor;
                        command.ExecuteNonQuery();
                    }
                }
                OnPropertyChanged(nameof(TextColor));
            }
        }

        string _bgColor = string.Empty;
        public string BgColor {
            get {
                if (_bgColor.Equals(string.Empty)) {
                    _bgColor = Fallback.bgColor;
                    using (SQLiteCommand command = new SQLiteCommand("SELECT value FROM settings WHERE name = @Name", App.DbConnection)) {
                        command.Parameters.Add("@Name", System.Data.DbType.String);
                        command.Parameters["@Name"].Value = "bgColor";
                        SQLiteDataReader r = command.ExecuteReader();
                        if (r.Read()) { _bgColor = r.GetString(0); }
                    }
                }
                return _bgColor.ToUpper();
            }
            set {
                if (Regex.IsMatch(value, colorPattern, RegexOptions.IgnoreCase)) {
                    _bgColor = value.StartsWith("#") ? value : "#" + value;
                    using (SQLiteCommand command = new SQLiteCommand("INSERT INTO settings (name, value) VALUES (@Name, @Value)", App.DbConnection)) {
                        command.Parameters.Add("@Name", System.Data.DbType.String);
                        command.Parameters.Add("@Value", System.Data.DbType.String);
                        command.Parameters["@Name"].Value = "bgColor";
                        command.Parameters["@Value"].Value = _bgColor;
                        command.ExecuteNonQuery();
                    }
                }
                OnPropertyChanged(nameof(BgColor));
            }
        }

        FontFamily _fontFamily = null;
        public FontFamily FontType {
            get {
                if (_fontFamily == null) {
                    _fontFamily = (from family in Fonts.SystemFontFamilies select family).First();
                    using (SQLiteCommand command = new SQLiteCommand("SELECT value FROM settings WHERE name = @Name", App.DbConnection)) {
                        command.Parameters.Add("@Name", System.Data.DbType.String);
                        command.Parameters["@Name"].Value = "fontName";
                        SQLiteDataReader r = command.ExecuteReader();
                        if (r.Read()) {
                            _fontFamily = (from family in Fonts.SystemFontFamilies where family.Source == r.GetString(0) select family)
                                .DefaultIfEmpty(new FontFamily(Fallback.fontName))
                                .FirstOrDefault();
                        }
                    }
                }
                return _fontFamily;
            }
            set {
                _fontFamily = value;
                using (SQLiteCommand command = new SQLiteCommand("INSERT INTO settings (name, value) VALUES (@Name, @Value)", App.DbConnection)) {
                    command.Parameters.Add("@Name", System.Data.DbType.String);
                    command.Parameters.Add("@Value", System.Data.DbType.String);
                    command.Parameters["@Name"].Value = "fontName";
                    command.Parameters["@Value"].Value = _fontFamily;
                    command.ExecuteNonQuery();
                }
                OnPropertyChanged(nameof(FontType));
            }
        }

        FamilyTypeface _fontStyle = null;
        public FamilyTypeface FontStyle {
            get {
                if (_fontStyle == null) {
                    _fontStyle = Fallback.fontStyle;
                    using (SQLiteCommand command = new SQLiteCommand("SELECT value FROM settings WHERE name = @Name", App.DbConnection)) {
                        command.Parameters.Add("@Name", System.Data.DbType.String);
                        command.Parameters["@Name"].Value = "fontStyle";
                        SQLiteDataReader r = command.ExecuteReader();
                        if (r.Read()) {
                            string[] styleParts = r.GetString(0).Split('#');
                            try {
                                FontStyle _tempStyle = (FontStyle)new FontStyleConverter().ConvertFromString(styleParts[0]);
                                FontWeight _tempWeight = (FontWeight)new FontWeightConverter().ConvertFromString(styleParts[1]);
                                _fontStyle = (from typeface in FontType.FamilyTypefaces
                                              where typeface.Style == _tempStyle && typeface.Weight == _tempWeight
                                              select typeface).DefaultIfEmpty(Fallback.fontStyle).FirstOrDefault();
                            }
                            catch (Exception) { }
                        }
                    }
                }
                return _fontStyle;
            }
            set {
                _fontStyle = value;
                using (SQLiteCommand command = new SQLiteCommand("INSERT INTO settings (name, value) VALUES (@Name, @Value)", App.DbConnection)) {
                    command.Parameters.Add("@Name", System.Data.DbType.String);
                    command.Parameters.Add("@Value", System.Data.DbType.String);
                    command.Parameters["@Name"].Value = "fontStyle";
                    command.Parameters["@Value"].Value = String.Format("{0}#{1}", _fontStyle.Style, _fontStyle.Weight);
                    command.ExecuteNonQuery();
                }
                OnPropertyChanged(nameof(FontStyle));
            }
        }

        double _textSize = -1;
        public double TextSize {
            get {
                if (_textSize < 0) {
                    _textSize = Fallback.fontSize;
                    using (SQLiteCommand command = new SQLiteCommand("SELECT value FROM settings WHERE name = @Name", App.DbConnection)) {
                        command.Parameters.Add("@Name", System.Data.DbType.String);
                        command.Parameters["@Name"].Value = "fontSize";
                        SQLiteDataReader r = command.ExecuteReader();
                        if (r.Read()) {
                            try {
                                _textSize = double.Parse(r.GetString(0));
                            }
                            catch (Exception) { }
                        }
                    }
                }
                return _textSize;
            }
            set {
                _textSize = value;
                using (SQLiteCommand command = new SQLiteCommand("INSERT INTO settings (name, value) VALUES (@Name, @Value)", App.DbConnection)) {
                    command.Parameters.Add("@Name", System.Data.DbType.String);
                    command.Parameters.Add("@Value", System.Data.DbType.String);
                    command.Parameters["@Name"].Value = "fontSize";
                    command.Parameters["@Value"].Value = value.ToString();
                    command.ExecuteNonQuery();
                }
                OnPropertyChanged(nameof(TextSize));
            }
        }
    }
}
