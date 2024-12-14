using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace GraphEditor
{
    public partial class VertexesDegreeWindow : Window
    {
        public VertexesDegreeWindow(List<Node> nodes)
        {
            InitializeComponent();
            DisplayNodeInfo(nodes);
        }

        private void DisplayNodeInfo(List<Node> nodes)
        {
            foreach (var node in nodes)
            {
                TextBlock nodeInfo = new TextBlock
                {
                    Text = $"Вершина: {node.EllipseName}\nСтепень вершины: {node.edges.Count}",
                    Margin = new Thickness(0, 5, 0, 5)
                };

                InfoPanel.Children.Add(nodeInfo);
            }
        }
    }
}