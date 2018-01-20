using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace KeyCounter
{

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string AppPath {
            get {
                string path = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Hotkey Counter");
                Directory.CreateDirectory(path);
                return path;
            }
        }

        private static bool dbInitDone = false;
        private static bool dbMigrateDone = false;
        private static SQLiteConnection _dbConnection =
            new SQLiteConnection("Data Source=" + Path.Combine(AppPath, "Counter.db") + "");

        public static SQLiteConnection DbConnection {
            get {
                if (_dbConnection.State == ConnectionState.Closed) {
                    _dbConnection.Open();
                }

                if (!dbInitDone) {
                    SQLiteCommand command = new SQLiteCommand(_dbConnection);

                    if (!dbMigrateDone) {
                        command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='colors'";
                        if (command.ExecuteScalar() != null) {
                            command.CommandText = "ALTER TABLE colors RENAME TO settings";
                            command.ExecuteNonQuery();
                        }
                        dbMigrateDone = true;
                    }

                    command.CommandText = "CREATE TABLE IF NOT EXISTS settings (name TEXT PRIMARY KEY ON CONFLICT REPLACE, value TEXT)";
                    command.ExecuteNonQuery();

                    command.CommandText = "CREATE TABLE IF NOT EXISTS counter (guid TEXT PRIMARY KEY ON CONFLICT REPLACE, name TEXT, key INTEGER, modifier INTEGER, current INTEGER, total INTEGER)";
                    command.ExecuteNonQuery();
                    command.Reset();
                }

                return _dbConnection;
            }
        }

        App() {
            string culture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
            KeyCounter.Properties.Resources.Culture = new CultureInfo(culture);
        }
    }
}
