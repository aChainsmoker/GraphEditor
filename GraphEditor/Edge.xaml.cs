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


        // Метод для обновления положения линии между узлами

        public void UpdatePosition()
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

                // Обновляем точки Polyline: начальная, промежуточные и конечная
                polyline.Points.RemoveAt(0);

                polyline.Points.Insert(0, startPoint);

                polyline.Points.RemoveAt(polyline.Points.Count - 1);
                polyline.Points.Add(endPoint);

                UpdateArrowPosition(startPoint, endPoint);
            }
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

