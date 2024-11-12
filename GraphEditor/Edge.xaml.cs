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
        public Brush edgeStroke = Brushes.Black;
        public List<Line> lines = new List<Line>();

        public Edge()
        {
            InitializeComponent();

            this.Focusable = true;
            this.FocusVisualStyle = null;

            Panel.SetZIndex(this, 2);

            //arrowHead.Visibility = Visibility.Collapsed;
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
                Point endPoint = EndNode.ellipse.TranslatePoint(
                    new Point(EndNode.ellipse.ActualWidth / 2, EndNode.ellipse.ActualHeight / 2),
                    (UIElement)EndNode.Parent);

                // Обновляем координаты линии
                lines[0].X1 = startPoint.X;
                lines[0].Y1 = startPoint.Y;
                lines[lines.Count - 1].X2 = endPoint.X;
                lines[lines.Count - 1].Y2 = endPoint.Y;

                Debug.WriteLine(lines[0].X1);
                Debug.WriteLine(lines[0].Y1);
                Debug.WriteLine(lines[lines.Count - 1].X2);
                Debug.WriteLine(lines[lines.Count - 1].Y2);


                UpdateArrowPosition(startPoint, endPoint);
            }

        }
        private void UpdateArrowPosition(Point startPoint, Point endPoint)
        {

            // Находим угол между узлами
            double angle = Math.Atan2(endPoint.Y - lines[lines.Count - 1].Y1, endPoint.X - lines[lines.Count - 1].X1) * 180 / Math.PI;
            

            Vector direction = new Vector(endPoint.X - lines[lines.Count-1].X1, endPoint.Y - lines[lines.Count - 1].Y1);
            direction.Normalize(); // Нормализуем вектор, чтобы его длина стала 1

            // Позиционируем стрелку на конце линии
            Canvas.SetLeft(this, endPoint.X - direction.X*EndNode.ellipse.Width - arrowHead.Width/5);
            Canvas.SetTop(this, endPoint.Y - direction.Y * EndNode.ellipse.Height - arrowHead.Height/2);


            RotateTransform rotateTransform = new RotateTransform(angle);

            arrowHead.RenderTransform = rotateTransform;
        }


        public void PaintTheEdge()
        {
            for(int i =0; i<lines.Count; i++)
            {
                lines[i].Stroke = edgeStroke;
            }
            arrowHead.Stroke = edgeStroke;
        }
        public void PaintTheEdge(Brush brush)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                lines[i].Stroke = brush;
            }
            arrowHead.Stroke = brush;
        }
    }
}

