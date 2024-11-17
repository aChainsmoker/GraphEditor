using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GraphEditor
{
    public partial class Edge : UserControl
    {
        public Node StartNode { get; set; }
        public Node EndNode { get; set; }

        public bool isOriented = false;
        public bool isReversed = false;
        public Brush edgeStroke = Brushes.Black;
        public Polyline polyline;
        public List<InflectionNode> inflectionEllipses = new List<InflectionNode>();

        public Edge()
        {
            InitializeComponent();

            this.Focusable = true;
            this.FocusVisualStyle = null;

            polyline = new Polyline
            {
                Stroke = edgeStroke,
                StrokeThickness = 3
            };

            Panel.SetZIndex(polyline, 1);
            Panel.SetZIndex(arrowHead, 2);

            MainGrid.Children.Add(polyline);
            arrowHead.Visibility = Visibility.Collapsed;
        }


        public void CreateInflectionPoints()
        {
            for(int i = 1;i < polyline.Points.Count-1; i++)
            {
                InflectionNode ellipse = new InflectionNode { motherEdge = this };
                inflectionEllipses.Add(ellipse);
                Canvas.SetLeft(ellipse, polyline.Points[i].X-5.5);
                Canvas.SetTop(ellipse, polyline.Points[i].Y-6);
                Panel.SetZIndex(ellipse, 2);
                MainGrid.Children.Add(ellipse);
            }
        }

        // Метод для обновления положения линии между узлами

        public void UpdateAllPositions()
        {
            UpdateNodePositions();
            UpdateMiddlePositions();
        }

        public void UpdateNodePositions()
        {
            if (StartNode != null && EndNode != null)
            {
                // Получаем центр первого узла
                Point startPoint = StartNode.ellipse.TranslatePoint(
                    new Point(StartNode.ellipse.ActualWidth / 2, StartNode.ellipse.ActualHeight / 2),
                    (UIElement)StartNode.Parent);

                // Получаем центр второго узла
                Point endPoint = EndNode.ellipse.TranslatePoint(
                    new Point(EndNode.ellipse.ActualWidth / 2, EndNode.ellipse.ActualHeight / 2),
                    (UIElement)EndNode.Parent);

                polyline.Points.RemoveAt(0);

                polyline.Points.Insert(0, startPoint);

                polyline.Points.RemoveAt(polyline.Points.Count - 1);
                polyline.Points.Add(endPoint);

                UpdateArrowPosition(startPoint, endPoint);
            }
        }

        public void UpdateMiddlePositions()
        {
            for(int i =0; i<inflectionEllipses.Count; i++)
            {
                InflectionNode ellipse = inflectionEllipses[i];
                Point point = ellipse.TranslatePoint(new Point(ellipse.ActualWidth / 2,
                    ellipse.ActualHeight / 2),
                    (UIElement)EndNode.Parent);
                polyline.Points.RemoveAt(i + 1);
                polyline.Points.Insert(i + 1, point);
            }
        }

        public void UpdateMiddlePositions(InflectionNode node)
        {
            int index = inflectionEllipses.IndexOf(node);
            InflectionNode ellipse = inflectionEllipses[index];
            Point point = ellipse.TranslatePoint(new Point(ellipse.ActualWidth / 2,
                ellipse.ActualHeight / 2),
                (UIElement)EndNode.Parent);
            polyline.Points.RemoveAt(index + 1);
            polyline.Points.Insert(index + 1, point);
            
        }

        private void UpdateArrowPosition(Point startPoint, Point endPoint)
        {
            // Находим угол между узлами
            double angle = Math.Atan2(endPoint.Y - polyline.Points[polyline.Points.Count-2].Y, endPoint.X - polyline.Points[polyline.Points.Count - 2].X) * 180 / Math.PI;

            Vector direction = new Vector(endPoint.X - polyline.Points[polyline.Points.Count - 2].X, endPoint.Y - polyline.Points[polyline.Points.Count - 2].Y);
            direction.Normalize();

            // Позиционируем стрелку на конце линии
            Canvas.SetLeft(arrowHead, endPoint.X - direction.X * EndNode.ellipse.Width - arrowHead.Width / 5);
            Canvas.SetTop(arrowHead, endPoint.Y - direction.Y * EndNode.ellipse.Height - arrowHead.Height / 2);

            RotateTransform rotateTransform = new RotateTransform(angle);
            arrowHead.RenderTransform = rotateTransform;
        }


        public void PaintTheEdge()
        {
            polyline.Stroke = edgeStroke;
            arrowHead.Stroke = edgeStroke;
        }
        public void PaintTheEdge(Brush brush)
        {
            polyline.Stroke = brush;
            arrowHead.Stroke = brush;
        }
    }
}

