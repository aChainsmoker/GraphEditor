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
            TabControl.Focus();
        }

        
        
        private void Tab_Loaded(object sender, RoutedEventArgs e)
        {
            TabTemplate tabTemplate = sender as TabTemplate;
            tabTemplate.MainCanvas.Focus();
        }

        private void Tab_PressedKey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.OemOpenBrackets)
            {
                TabControl tabControl = sender as TabControl;
                TabTemplate tabTemplate = new TabTemplate();
                tabTemplate.Loaded += Tab_Loaded;
                
                TabItem tabItem = new TabItem
                {
                    Header = "New Tab",
                    Content = tabTemplate,
                };
                tabControl.Items.Add(tabItem);
                tabControl.SelectedItem = tabItem;
            }
            else if (e.Key == Key.OemCloseBrackets)
            {
                TabControl tabControl = sender as TabControl;
                tabControl.Items.Remove(tabControl.SelectedItem);
                tabControl.Focus();
            }
        }
    }
}