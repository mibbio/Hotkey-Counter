using GlobalHotKey;
using System;
using System.ComponentModel;
using System.Data.SQLite;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace KeyCounter
{
    class CounterData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        string _name;
        public string Name {
            get { return _name; }
            set {
                if (_name.Equals(value)) { return; }
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        string _hotkeyString = "";
        public string HotkeyString {
            get { return _hotkeyString; }
            set {
                if (_hotkeyString.Equals(value)) { return; }
                _hotkeyString = value;
                OnPropertyChanged(nameof(HotkeyString));
            }
        }

        ModifierKeys _modifiers = ModifierKeys.None;
        public ModifierKeys Modifiers {
            get { return _modifiers; }
            set {
                if (_modifiers.Equals(value)) { return; }
                _modifiers = value;
            }
        }

        Key _key = Key.None;
        public Key Key {
            get { return _key; }
            set {
                if (_key.Equals(value)) { return; }
                _key = value;
            }
        }

        int _sessionCount = 0;
        public int SessionCount {
            get {
                return _sessionCount;
            }
            private set {
                _sessionCount = value;
                OnPropertyChanged(nameof(SessionCount));
            }
        }

        public HotKey Hotkey { get; set; }
        public Guid GUID { get; }

        public void Increment() {
            SessionCount += 1;
        }

        public void Reset() {
            SessionCount = 0;
        }

        public void UpdateHotkey() {
            StringBuilder hotkeyText = new StringBuilder();
            if ((Modifiers & ModifierKeys.Control) != 0) {
                hotkeyText.Append("Ctrl + ");
            }
            if ((Modifiers & ModifierKeys.Shift) != 0) {
                hotkeyText.Append("Shift + ");
            }
            if ((Modifiers & ModifierKeys.Alt) != 0) {
                hotkeyText.Append("Alt + ");
            }
            hotkeyText.Append(Key.ToString());
            HotkeyString = hotkeyText.ToString();
        }

        public CounterData(Guid guid, string name, int sessionCount, Key k = Key.None, ModifierKeys m = ModifierKeys.None) {
            GUID = guid;
            _name = name;
            _sessionCount = sessionCount;
            _modifiers = m;
            _key = k;
            UpdateHotkey();
        }
    }
}
