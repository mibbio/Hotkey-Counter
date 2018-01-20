using System;
using System.ComponentModel;
using System.Data.SQLite;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace KeyCounter
{
    public class AppSettings
    {
        private static string colorPattern = @"^#?[a-f0-9]{6}$";

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        string _textColor = "";
        public string TextColor {
            get {
                if (_textColor.Equals("")) {
                    SQLiteCommand command = new SQLiteCommand("SELECT value FROM settings WHERE name = @Name", App.DbConnection);
                    command.Parameters.Add("@Name", System.Data.DbType.String);
                    command.Parameters["@Name"].Value = "textColor";
                    SQLiteDataReader r = command.ExecuteReader();
                    if (r.Read()) { _textColor = r.GetString(0); }
                    else { _textColor = "#FFFFFF"; }
                    command.Reset();
                }
                return _textColor.ToUpper();
            }
            set {
                if (Regex.IsMatch(value, colorPattern, RegexOptions.IgnoreCase)) {
                    _textColor = value.StartsWith("#") ? value : "#" + value;
                    SQLiteCommand command = new SQLiteCommand("INSERT INTO settings (name, value) VALUES (@Name, @Value)", App.DbConnection);
                    command.Parameters.Add("@Name", System.Data.DbType.String);
                    command.Parameters.Add("@Value", System.Data.DbType.String);
                    command.Parameters["@Name"].Value = "textColor";
                    command.Parameters["@Value"].Value = _textColor;
                    command.ExecuteNonQuery();
                    command.Reset();
                }
                OnPropertyChanged(nameof(TextColor));
            }
        }

        string _bgColor = "";
        public string BgColor {
            get {
                if (_bgColor.Equals("")) {
                    SQLiteCommand command = new SQLiteCommand("SELECT value FROM settings WHERE name = @Name", App.DbConnection);
                    command.Parameters.Add("@Name", System.Data.DbType.String);
                    command.Parameters["@Name"].Value = "bgColor";
                    SQLiteDataReader r = command.ExecuteReader();
                    if (r.Read()) { _bgColor = r.GetString(0); }
                    else { _bgColor = "#FF00FF"; }
                    command.Reset();
                }
                return _bgColor.ToUpper();
            }
            set {
                if (Regex.IsMatch(value, colorPattern, RegexOptions.IgnoreCase)) {
                    _bgColor = value.StartsWith("#") ? value : "#" + value;
                    SQLiteCommand command = new SQLiteCommand("INSERT INTO settings (name, value) VALUES (@Name, @Value)", App.DbConnection);
                    command.Parameters.Add("@Name", System.Data.DbType.String);
                    command.Parameters.Add("@Value", System.Data.DbType.String);
                    command.Parameters["@Name"].Value = "bgColor";
                    command.Parameters["@Value"].Value = _bgColor;
                    command.ExecuteNonQuery();
                    command.Reset();
                }
                OnPropertyChanged(nameof(BgColor));
            }
        }
    }
}
