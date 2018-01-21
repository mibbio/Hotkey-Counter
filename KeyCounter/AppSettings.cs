using System.ComponentModel;
using System.Data.SQLite;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

namespace KeyCounter {
    public class AppSettings
    {
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
    }
}
