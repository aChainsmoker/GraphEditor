using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GraphEditor
{
    public partial class TabTemplate : UserControl
    {
        public TabTemplate()
        {
            InitializeComponent();
        }

        private bool isDragging = false;      // Флаг, указывающий, происходит ли перемещение
        private Point mouseStartPosition;     // Начальная позиция мыши
        private UIElement movingObject;       // Объект, который мы перемещаем
        private UIElement lastFocusedObject;
        private List<Node> nodes = new List<Node>();
        private List<Edge> edges = new List<Edge>();
        private List<Point> edgePoints = new List<Point>();

        private Node selectedNode1;
        private Node selectedNode2;
        private bool isSelectingEdges = false; // Флаг для режима выбора рёбер
        private bool pointWasAssigned = false;
        private bool ClearFocusLighting()
        {
            if (lastFocusedObject != null)
            {
                if (lastFocusedObject is Node node)
                {
                    node.ellipse.Stroke = node.nodeStroke;
                    return true;
                }
                else if (lastFocusedObject is Edge edge)
                {
                    edge.PaintTheEdge();
                    return true;
                }
            }
            return false;
        }


        private void Edge_PressedKey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.O)
            {
                if (sender is Edge edge)
                {
                    edge.isOriented = !edge.isOriented;
                    if (edge.isOriented)
                    {
                        edge.arrowHead.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        edge.arrowHead.Visibility = Visibility.Collapsed;
                    }
                }
            }
            else if (e.Key == Key.P)
            {
                if (sender is Edge edge)
                {
                    Node buff;
                    buff = edge.StartNode;
                    edge.StartNode = edge.EndNode;
                    edge.EndNode = buff;
                    edge.polyline.Points = new PointCollection(edge.polyline.Points.Reverse());
                    edge.UpdatePosition();
                }

            }
        }


        private void Ellipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;   // Указываем, что началось перемещение
            movingObject = sender as UIElement;   // Получаем ссылку на перемещаемый объект
            mouseStartPosition = e.GetPosition(MainCanvas);   // Запоминаем начальную позицию мыши
            movingObject.CaptureMouse();   // Захватываем мышь для объекта

            ClearFocusLighting();
            lastFocusedObject = movingObject;

            if (movingObject is Node node)
            {
                Keyboard.ClearFocus();

                node.ellipse.Stroke = Brushes.GreenYellow;
                UpdateGraphStats();

                if (isSelectingEdges)
                    SelectNodeForEdgeCreating(node);
            }
            movingObject.Focus();
        }

        private void Edge_MouseDown(object sender, MouseButtonEventArgs e)
        {

            ClearFocusLighting();
            lastFocusedObject = sender as UIElement;

            if (sender is Edge edge)
            {
                Keyboard.ClearFocus();

                edge.PaintTheEdge(Brushes.GreenYellow);
                UpdateGraphStats();
            }
            lastFocusedObject.Focus();
        }

        private void SelectNodeForEdgeCreating(Node node)
        {
            if (selectedNode1 == null)
            {
                selectedNode1 = node;
                pointWasAssigned = true;
            }
            else
            {
                selectedNode2 = node;

                CreateEdge(selectedNode1, selectedNode2);
                // Сбрасываем выбранные узлы после создания дуги
                selectedNode1 = null;
                selectedNode2 = null;
            }
        }


        private void CreateEdge(Node node1, Node node2)
        {
            Point phantomPoint = new Point();
            // Создаем новый экземпляр Edge
            Edge edge = new Edge
            {
                StartNode = node1,
                EndNode = node2,
            };

            for(int i =0; i<edgePoints.Count; i++)
                edge.polyline.Points.Add(edgePoints[i]);
            edge.polyline.Points.Add(phantomPoint);
            edgePoints = new List<Point>();

            edge.MouseDown += Edge_MouseDown;
            edge.KeyDown += Edge_PressedKey;

            edges.Add(edge);
            //MainCanvas.Children.Add(edge.polyline);
            MainCanvas.Children.Add(edge); // Добавляем Edge на Canvas

            edge.UpdatePosition(); // Устанавливаем начальные точки
            node1.edges.Add(edge);
            node2.edges.Add(edge);

            UpdateGraphStats();
        }



        // Обработка перемещения мыши
        private void Ellipse_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging && movingObject != null)
            {
                // Получаем текущее положение мыши относительно канваса
                Point mouseCurrentPosition = e.GetPosition(MainCanvas);

                // Вычисляем, на сколько сдвинулась мышь
                double offsetX = mouseCurrentPosition.X - mouseStartPosition.X;
                double offsetY = mouseCurrentPosition.Y - mouseStartPosition.Y;

                // Получаем текущие координаты объекта
                double currentLeft = Canvas.GetLeft(movingObject);
                double currentTop = Canvas.GetTop(movingObject);

                // Задаем новые координаты объекта, с учетом сдвига
                Canvas.SetLeft(movingObject, currentLeft + offsetX);
                Canvas.SetTop(movingObject, currentTop + offsetY);

                // Обновляем начальную позицию мыши для следующего шага
                mouseStartPosition = mouseCurrentPosition;

                if (sender is Node node)
                {
                    UpdateEdgesPosition(node);
                }
            }
        }

        private void UpdateEdgesPosition(Node node)
        {
            for (int i = 0; i < node.edges.Count; i++)
            {
                node.edges[i].UpdatePosition();
            }
        }

        // Обработка отпускания кнопки мыши
        private void Ellipse_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;   // Прекращаем перемещение
            if (movingObject != null)
            {
                movingObject.ReleaseMouseCapture();   // Освобождаем захват мыши
                movingObject = null;   // Сбрасываем объект
            }
        }
        private void Canvas_PressedKey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.A) // Если нажата клавиша A
            {
                // Получаем текущую позицию курсора относительно Canvas
                Point cursorPosition = Mouse.GetPosition(MainCanvas);

                // Создаем новый эллипс
                Node newNode = new Node();

                // Устанавливаем позицию эллипса на Canvas
                Canvas.SetLeft(newNode, cursorPosition.X - newNode.ellipse.Width / 2); // Центрирование
                Canvas.SetTop(newNode, cursorPosition.Y - newNode.ellipse.Height / 2);

                // Подключаем обработчики для перемещения эллипса
                newNode.MouseDown += Ellipse_MouseDown;
                newNode.MouseUp += Ellipse_MouseUp;
                newNode.MouseMove += Ellipse_MouseMove;
                newNode.KeyDown += Ellipse_PressedKey;

                // Добавляем новый эллипс на Canvas
                MainCanvas.Children.Add(newNode);
                nodes.Add(newNode);
                UpdateGraphStats();
            }
            else if (e.Key == Key.R)
            {
                isSelectingEdges = !isSelectingEdges;
                selectedNode1 = null;
                selectedNode2 = null;
            }
        }


        private void Ellipse_PressedKey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.D) // Если нажата клавиша D — удаляем эллипс
            {
                if (sender is Node node)
                {
                    MainCanvas.Children.Remove(node);
                    MainCanvas.Focus();
                }
            }
            if (e.Key == Key.I)
            {
                // Открываем диалог для ввода имени
                NodeNameInput dialog = new NodeNameInput();
                if (dialog.ShowDialog() == true)
                {
                    // Если окно закрыто и введено имя, можно изменить его в узле
                    string nodeName = dialog.NodeName;

                    // Найдите ваш узел, к которому хотите применить имя, и присвойте его
                    // Пример: Если у вас есть ссылка на текущий узел
                    Node currentNode = sender as Node; // Получите ссылку на нужный узел
                    currentNode.EllipseName = nodeName;
                }
            }
        }

        private void UpdateGraphStats()
        {
            NodeCountText.Text = "В графе " + nodes.Count + " вершин";
            EdgeCountText.Text = "В графе " + edges.Count + " дуг";
            if (lastFocusedObject is Node node)
                NodeMultiplicityCountText.Text = "Степень данной вершины: " + node.edges.Count;
            else
                NodeMultiplicityCountText.Text = String.Empty;
        }

        private void MainCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(isSelectingEdges && selectedNode1 != null && selectedNode2 == null)
            {

                Point startPoint = selectedNode1.ellipse.TranslatePoint(
                    new Point(selectedNode1.ellipse.ActualWidth / 2, selectedNode1.ellipse.ActualHeight / 2),
                    (UIElement)selectedNode1.Parent);
                Point currentPoint = e.GetPosition(MainCanvas);
                if (edgePoints.Count ==0 )
                {
                    Point point = new Point(startPoint.X, startPoint.Y);
                    edgePoints.Add(point);
                }
                else
                {
                    Point point = new Point(currentPoint.X, currentPoint.Y);
                    edgePoints.Add(point);
                }

                
            }

        }
    }
}

