using GlobalHotKey;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows;
using MahApps.Metro.Controls;
using System.Data.SQLite;
using System;

namespace KeyCounter
{
    class CounterCollection : ObservableCollection<CounterData>
    {
        private HotKeyManager hkm = new HotKeyManager();
        private bool IsInitialized = false;

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
            if (e.Action == NotifyCollectionChangedAction.Add) {
                if (IsInitialized) {
                    using (SQLiteTransaction tr = App.DbConnection.BeginTransaction()) {
                        using (SQLiteCommand cmd = App.DbConnection.CreateCommand()) {
                            cmd.Transaction = tr;
                            cmd.CommandText = "INSERT INTO counter VALUES (@Guid, @Name, @Key, @Modifier, @CurrentCount, @TotalCount)";
                            cmd.Parameters.Add("@Guid", System.Data.DbType.String);
                            cmd.Parameters.Add("@Name", System.Data.DbType.String);
                            cmd.Parameters.Add("@Key", System.Data.DbType.Int32);
                            cmd.Parameters.Add("@Modifier", System.Data.DbType.Int32);
                            cmd.Parameters.Add("@CurrentCount", System.Data.DbType.Int32);
                            cmd.Parameters.Add("@TotalCount", System.Data.DbType.Int32);
                            foreach (CounterData item in e.NewItems) {
                                cmd.Parameters["@Guid"].Value = item.GUID;
                                cmd.Parameters["@Name"].Value = item.Name;
                                cmd.Parameters["@Key"].Value = item.Key;
                                cmd.Parameters["@Modifier"].Value = item.Modifiers;
                                cmd.Parameters["@CurrentCount"].Value = item.SessionCount;
                                cmd.Parameters["@TotalCount"].Value = 0;
                                cmd.ExecuteNonQuery();
                            }
                        }
                        tr.Commit();
                    }
                }
                RegisterPropertyChanged(e.NewItems);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove) {
                using(SQLiteTransaction tr = App.DbConnection.BeginTransaction()) {
                    using (SQLiteCommand cmd = App.DbConnection.CreateCommand()) {
                        foreach (CounterData item in e.OldItems) {
                            cmd.CommandText = "DELETE FROM counter WHERE guid = @Guid";
                            cmd.Parameters.Add("@Guid", System.Data.DbType.String);
                            cmd.Parameters["@Guid"].Value = item.GUID;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    tr.Commit();
                }
                UnregisterPropertyChanged(e.OldItems);
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace) {
                UnregisterPropertyChanged(e.OldItems);
                RegisterPropertyChanged(e.NewItems);
            }
            base.OnCollectionChanged(e);
        }

        protected override void ClearItems() {
            UnregisterPropertyChanged(this);
            base.ClearItems();
        }

        private void RegisterPropertyChanged(IList items) {
            foreach (CounterData cd in items) {
                if (cd != null) {
                    SetHotkey(cd);
                    cd.PropertyChanged += new PropertyChangedEventHandler(ItemPropertyChanged);
                }
            }
        }

        private void UnregisterPropertyChanged(IList items) {
            foreach (CounterData cd in items) {
                if (cd != null) {
                    if (cd.Hotkey != null) {
                        UnsetHotkey(cd);
                    }
                    cd.PropertyChanged -= new PropertyChangedEventHandler(ItemPropertyChanged);
                }
            }
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e) {
            base.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            if (sender is CounterData) {
                CounterData cd = (CounterData)sender;
                using (SQLiteCommand cmd = App.DbConnection.CreateCommand()) {

                    switch (e.PropertyName) {
                        case "HotkeyString":
                            cmd.CommandText = "UPDATE counter SET key = @Key, modifier = @modifier WHERE guid = @Guid";
                            cmd.Parameters.Add("@Guid", System.Data.DbType.String);
                            cmd.Parameters.Add("@Key", System.Data.DbType.Int32);
                            cmd.Parameters.Add("@Modifier", System.Data.DbType.Int32);
                            cmd.Parameters["@Guid"].Value = cd.GUID;
                            cmd.Parameters["@Key"].Value = cd.Key;
                            cmd.Parameters["@Modifier"].Value = cd.Modifiers;
                            UnsetHotkey(cd);
                            SetHotkey(cd);
                            break;
                        case "SessionCount":
                            cmd.CommandText = "UPDATE counter SET current = @CurrentCount, total = total + @TotalCount WHERE guid = @Guid";
                            cmd.Parameters.Add("@Guid", System.Data.DbType.String);
                            cmd.Parameters.Add("@CurrentCount", System.Data.DbType.Int32);
                            cmd.Parameters.Add("@TotalCount", System.Data.DbType.Int32);
                            cmd.Parameters["@Guid"].Value = cd.GUID;
                            cmd.Parameters["@CurrentCount"].Value = cd.SessionCount;
                            cmd.Parameters["@TotalCount"].Value = (cd.SessionCount > 0) ? 1 : 0;
                            break;
                        default:
                            cmd.CommandText = "UPDATE counter SET name = @Name WHERE guid = @Guid";
                            cmd.Parameters.Add("@Guid", System.Data.DbType.String);
                            cmd.Parameters.Add("@Name", System.Data.DbType.String);
                            cmd.Parameters["@Guid"].Value = cd.GUID;
                            cmd.Parameters["@Name"].Value = cd.Name;
                            break;
                    }
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void SetHotkey(CounterData counter) {
            if (counter.Key != Key.None) {
                counter.Hotkey = hkm.Register(counter.Key, counter.Modifiers);
            }
        }

        private void UnsetHotkey(CounterData counter) {
            if (counter.Hotkey != null) {
                hkm.Unregister(counter.Hotkey);
            }
        }

        private void HotkeyHandler(object sender, KeyPressedEventArgs e) {
            if (skipHandler) { return; }
            foreach (CounterData item in Items) {
                if (e.HotKey.Equals(item.Hotkey)) {
                    item.Increment();
                    using (SQLiteCommand cmd = App.DbConnection.CreateCommand()) {
                        cmd.CommandText = "UPDATE counter SET current = @Value WHERE guid = @Guid";
                        cmd.Parameters.Add("@Value", System.Data.DbType.String);
                        cmd.Parameters.Add("@Guid", System.Data.DbType.String);
                        cmd.Parameters["@Guid"].Value = item.GUID;
                        cmd.Parameters["@Value"].Value = item.SessionCount;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private bool skipHandler = false;
        internal void ToggleEditMode(object sender, RoutedEventArgs e) {
            skipHandler = ((Flyout)sender).IsOpen;
        }

        public CounterCollection() : base() {
            using (SQLiteCommand cmd = App.DbConnection.CreateCommand()) {
                cmd.CommandText = "SELECT * FROM counter";
                using (SQLiteDataReader r = cmd.ExecuteReader()) {
                    while (r.Read()) {
                        Add(new CounterData(
                            Guid.Parse(r.GetString(0)),
                            r.GetString(1),
                            r.GetInt32(4),
                            (Key)r.GetInt32(2),
                            (ModifierKeys)r.GetInt32(3)
                            )
                        );
                    }
                }
            }
            hkm.KeyPressed += HotkeyHandler;
            IsInitialized = true;
        }

        public void Dispose() {
            hkm.Dispose();
        }
    }
}
