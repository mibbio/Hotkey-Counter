using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KeyCounter
{
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
