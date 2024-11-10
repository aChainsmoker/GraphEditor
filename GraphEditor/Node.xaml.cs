using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace GraphEditor
{
    public partial class Node : UserControl
    {
        public List<Edge> edges;
        public Brush nodeStroke = Brushes.Black;

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



    }
}
