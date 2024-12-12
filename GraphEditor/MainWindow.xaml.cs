using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace GraphEditor
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            
            InitializeComponent();
        }

        private void Tab_Loaded(object sender, RoutedEventArgs e)
        {
            TabTemplate tabTemplate = sender as TabTemplate;
            tabTemplate.MainCanvas.Focus();
        }
    }
}