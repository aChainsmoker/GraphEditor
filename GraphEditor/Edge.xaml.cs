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

        public bool isOriented = true;
        public Brush edgeStroke = Brushes.Black;


        public Edge()
        {
            InitializeComponent();

            this.Focusable = true;
            this.FocusVisualStyle = null;

            Panel.SetZIndex(this, 1);
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

                // Обновляем координаты линии
                edgeLine.X1 = startPoint.X;
                edgeLine.Y1 = startPoint.Y;
                edgeLine.X2 = endPoint.X;
                edgeLine.Y2 = endPoint.Y;


                UpdateArrowPosition(startPoint, endPoint);
            }

        }
        private void UpdateArrowPosition(Point startPoint, Point endPoint)
        {

            // Находим угол между узлами
            double angle = Math.Atan2(endPoint.Y - startPoint.Y, endPoint.X - startPoint.X) * 180 / Math.PI;


            Vector direction = new Vector(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y);
            direction.Normalize(); // Нормализуем вектор, чтобы его длина стала 1

            // Позиционируем стрелку на конце линии
            Canvas.SetLeft(arrowHead, endPoint.X - direction.X*EndNode.ellipse.Width - arrowHead.Width/5);
            Canvas.SetTop(arrowHead, endPoint.Y - direction.Y * EndNode.ellipse.Height - arrowHead.Height/2);


            RotateTransform rotateTransform = new RotateTransform(angle);

            arrowHead.RenderTransform = rotateTransform;
        }
    }
}

