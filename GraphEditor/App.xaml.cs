using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace GraphEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void ChangeColor_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;

            if (menuItem != null && menuItem.Tag is string colorName)
            {
                ContextMenu contextMenu = (ContextMenu)menuItem.Parent;
                Brush newColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorName));

                switch (contextMenu.PlacementTarget)
                {
                    case Node node:
                        node.nodeStroke = newColor;
                        break;
                    case Edge edge:
                        edge.edgeStroke = newColor;
                        break;
                    default:
                        break;
                }
            }
        }
       

    }
}


