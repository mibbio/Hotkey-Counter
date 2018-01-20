using System.Collections.Generic;

namespace KeyCounter {
    class Counter
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }
    /// <summary>
    /// Interaction logic for OverlayWindow.xaml
    /// </summary>
    public partial class OverlayWindow
    {
        public OverlayWindow() {
            InitializeComponent();
            List<Counter> cl = new List<Counter>();
            for (int i = 0; i < 10; i++) {
                cl.Add(new Counter() { Name = "Counter" + i, Count = i*i });
            }
            //counterItems.ItemsSource = cl;
        }
    }
}
