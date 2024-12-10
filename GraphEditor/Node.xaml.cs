using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Serialization;


namespace GraphEditor
{
    [Serializable]
    public partial class Node : UserControl
    {
        public List<Edge> edges;
        public Brush nodeStroke = Brushes.Black;
        public bool isChecked;

        public Node()
        {
            InitializeComponent();
            edges = new List<Edge>();

            this.Focusable = true;
            this.FocusVisualStyle = null;

            Panel.SetZIndex(this, 3);
        }

        // Свойство для доступа к имени (тексту)
        public string EllipseName
        {
            get { return nameTextBox.Text; }
            set { nameTextBox.Text = value; }
        }

        // Свойство для изменения цвета эллипса
        public Brush EllipseFill
        {
            get { return ellipse.Fill; }
            set { ellipse.Fill = value; }
        }

        public SerializableNode ToSerializableNode(int id)
        {
            return new SerializableNode
            {
                Id = id,
                EllipseName = EllipseName,
                X = Canvas.GetLeft(this),
                Y = Canvas.GetTop(this),
                NodeStroke = nodeStroke,
            };
        }

        public static Node FromSerializableNode(SerializableNode serializableNode)
        {
            var node = new Node
            {
                EllipseName = serializableNode.EllipseName,
                nodeStroke = serializableNode.NodeStroke,
            };
            Canvas.SetLeft(node, serializableNode.X);
            Canvas.SetTop(node, serializableNode.Y);
            return node;
        }
    }
}
