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

        public void PaintTheNode()
        {
            this.ellipse.Stroke = nodeStroke;
        }

        public void PaintTheNode(Brush brush)
        {
            this.nodeStroke = brush;
            this.ellipse.Stroke = brush;
        }

        public void RepresentFocus()
        {
            this.ellipse.Stroke = Brushes.GreenYellow;
        }

        public string EllipseName
        {
            get { return nameTextBox.Text; }
            set { nameTextBox.Text = value; }
        }

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

        public List<Node> GetListOfAvailableNodes()
        {
            List<Node> nodes = new List<Node>();
            for (int i = 0; i < edges.Count; i++)
            {
                Node newNode;
                if (edges[i].isOriented == false)
                {
                    newNode = edges[i].StartNode == this? edges[i].EndNode : edges[i].StartNode;
                }
                else
                {
                    if(edges[i].EndNode != this)
                        newNode = edges[i].EndNode;
                    else
                        continue;
                }
                nodes.Add(newNode);
            }
            return nodes;
        }
        public List<Edge> GetListOfAvailableEdges()
        {
            List<Edge> availableEdges = new List<Edge>();
            for (int i = 0; i < edges.Count; i++)
            {
                if (edges[i].isOriented == false)
                {
                    if (edges[i].StartNode == this || edges[i].EndNode == this)
                        availableEdges.Add(edges[i]);
                }
                else
                {
                    if (edges[i].StartNode == this)
                        availableEdges.Add(edges[i]);  
                }
            }
            return availableEdges;
        }
    }
}
