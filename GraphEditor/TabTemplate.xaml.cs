using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.Win32;
using Path = System.Windows.Shapes.Path;


namespace GraphEditor
{
    public partial class TabTemplate : UserControl
    {
        private bool isDragging = false;      // Флаг, указывающий, происходит ли перемещение
        private UIElement movingObject;       // Объект, который мы перемещаем
        private Point mouseStartPosition;     // Начальная позиция мыши
        private UIElement lastFocusedObject;
        private Graph graph = new Graph();
        private List<Point> edgePoints = new List<Point>();
        private List<Node> selectedNodes;
        private List<InflectionNode> selectedInflectionNodes;

        private Node selectedNode1;
        private Node selectedNode2;
        private bool isSelectingEdges = false; // Флаг для режима выбора рёбер
        private bool isSelectingNodesForTheShortestPathFinding = false;
        private bool isSelectingNodesFortPathFinding = false;
        private int foundPathIndex;
        
        private Point startRectanglePoint; // Начальная точка выделения
        private Rectangle selectionRectangle; // Прямоугольник выделения
        private bool isRectangleSelectingMode = false; // Флаг выделения

        private List<Dictionary<Node, Edge>> paths; //Временное решение
        private int theShortestCost;
        
        private ScaleTransform scaleTransform;
        private double scale = 1.0; // Текущий масштаб
        public TabTemplate()
        {
            InitializeComponent();
            graph.AddingEdge += CreateEdge;
            UpdateModesStatus();
            UpdatePathCost(0, false);
            
            scaleTransform = new ScaleTransform(scale, scale);
            MainCanvas.RenderTransform = scaleTransform;
        }

        
        private bool ClearFocusLighting()
        {
            if (lastFocusedObject != null)
            {
                if (lastFocusedObject is Node node)
                {
                    if((selectedNodes != null && !selectedNodes.Contains(node)) || selectedNodes == null)
                    {
                        node.PaintTheNode();
                        return true;
                    }
                }
                else if (lastFocusedObject is Edge edge)
                {
                    edge.PaintTheEdge();
                    return true;
                }
            }
            return false;
        }


        public void SaveGraphToXml(string filePath, List<Node> nodes, List<Edge> edges)
        {
            SerializableGraph serializableGraph = new SerializableGraph();
            Dictionary<Node,int> nodeIdMap = new Dictionary<Node, int>();
            
            
            // Конвертируем узлы
            for (int i = 0; i < nodes.Count; i++)
            {
                nodeIdMap[nodes[i]] = i;
                serializableGraph.Nodes.Add(nodes[i].ToSerializableNode(i));
            }

            // Конвертируем рёбра
            foreach (Edge edge in edges)
            {
                serializableGraph.Edges.Add(edge.ToSerializableEdge(nodeIdMap));
            }

            // Сохраняем в XML
            var serializer = new XmlSerializer(typeof(SerializableGraph));
            using (var writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, serializableGraph);
            }
            
            TabItem tabItem = GetTab();
            tabItem.Header = System.IO.Path.GetFileNameWithoutExtension(filePath);
        }


        private void Edge_PressedKey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.O)
            {
                if (sender is Edge edge)
                {
                    edge.isOriented = !edge.isOriented;
                    edge.SetArrowVisibility();
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
                    edge.inflectionEllipses.Reverse();
                    edge.UpdateAllPositions();
                }
            }
            else if (e.Key == Key.Delete)
            {
                if(sender is Edge edge)
                {
                    edge.StartNode.edges.Remove(edge);
                    edge.EndNode.edges.Remove(edge);
                    graph.RemoveEdge(edge);
                    MainCanvas.Children.Remove(edge);
                    MainCanvas.Focus();
                }
            }
            else if (e.Key == Key.I)
            {
                // Открываем диалог для ввода имени
                EdgeWeightWindow dialog = new EdgeWeightWindow();
                if (dialog.ShowDialog() == true)
                {
                    // Если окно закрыто и введено имя, можно изменить его в узле
                    int weight = dialog.Weight;

                    // Найдите ваш узел, к которому хотите применить имя, и присвойте его
                    // Пример: Если у вас есть ссылка на текущий узел
                    Edge currentEdge = sender as Edge; // Получите ссылку на нужный узел
                    currentEdge.Weight = weight;
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

            if (selectedNodes != null && !(selectedNodes.Contains(movingObject) ||
                                           selectedInflectionNodes.Contains(movingObject)))
            {
                graph.PaintAllNodes();
                selectedNodes = null;
            }

            lastFocusedObject = movingObject;

            if (movingObject is Node node)
            {
                Keyboard.ClearFocus();

                node.RepresentFocus();
                UpdateGraphStats();

                if (isSelectingEdges)
                    SelectNodeForEdgeCreating(node);
                if (isSelectingNodesForTheShortestPathFinding || isSelectingNodesFortPathFinding)
                    SelectNodeForPathFinding(node);
            }
            movingObject.Focus();
        }

        private void SelectNodeForPathFinding(Node node)
        {
            if (selectedNode1 == null)
            {
                selectedNode1 = node;
            }
            else
            {
                selectedNode2 = node;
                if (isSelectingNodesFortPathFinding)
                {
                    foundPathIndex = 0;
                    theShortestCost = PathFinder.AssessThePath(PathFinder.FindShortestWay(graph, selectedNode1, selectedNode2));
                    paths = PathFinder.FindAllPaths(graph, selectedNode1, selectedNode2);
                    ShowTheWay(paths, foundPathIndex);
                }
                else if(isSelectingNodesForTheShortestPathFinding)
                {
                    Dictionary<Node, Edge> path = PathFinder.FindShortestWay(graph, selectedNode1, selectedNode2);
                    ShowTheShortestWay(path);
                }
                selectedNode1 = null;
                selectedNode2 = null;
            }
        }

        private void ShowTheWay(List<Dictionary<Node,Edge>> paths, int index)
        {
            if(index > paths.Count - 1)
                return;
            Brush brush;
            graph.PaintAllEdges(Brushes.Black);
            graph.PaintAllNodes(Brushes.Black);
            
            if(PathFinder.AssessThePath(paths[index]) == theShortestCost)
                brush = Brushes.Red;
            else
                brush = Brushes.Green;
            
            foreach (KeyValuePair<Node, Edge> nodeToPaint in paths[index])
            {
                graph.Nodes[graph.Nodes.IndexOf(nodeToPaint.Key)].PaintTheNode(brush);
                if (nodeToPaint.Value != null)
                    graph.Edges[graph.Edges.IndexOf(nodeToPaint.Value)].PaintTheEdge(brush);
            }
            UpdatePathCost(PathFinder.AssessThePath(paths[index]), true);
        }
        private void ShowTheShortestWay(Dictionary<Node, Edge> path)
        {
            
            graph.PaintAllEdges(Brushes.Black);
            graph.PaintAllNodes(Brushes.Black);
            foreach (KeyValuePair<Node, Edge> nodeToPaint in path)
            {
                graph.Nodes[graph.Nodes.IndexOf(nodeToPaint.Key)].PaintTheNode(Brushes.Red);
                if (nodeToPaint.Value != null)
                    graph.Edges[graph.Edges.IndexOf(nodeToPaint.Value)].PaintTheEdge(Brushes.Red);
            }
            UpdatePathCost(PathFinder.AssessThePath(path), true);
        }
        private void Edge_MouseDown(object sender, MouseButtonEventArgs e)
        {

            ClearFocusLighting();
            lastFocusedObject = sender as UIElement;

            if (sender is Edge edge)
            {
                Keyboard.ClearFocus();

                edge.RepresentFocus();
                UpdateGraphStats();
            }
            lastFocusedObject.Focus();
        }

        private void SelectNodeForEdgeCreating(Node node)
        {
            if (selectedNode1 == null)
            {
                selectedNode1 = node;
            }
            else
            {
                selectedNode2 = node;
                
                CreateEdge(selectedNode1, selectedNode2, false).UpdateNodePositions();
                // Сбрасываем выбранные узлы после создания дуги
                selectedNode1 = null;
                selectedNode2 = null;
                UpdateGraphStats();
            }
        }


        private Edge CreateEdge(Node node1, Node node2, bool isOriented)
        {
            
            Point phantomPoint = new Point();
            edgePoints.Add(phantomPoint);
            // Создаем новый экземпляр Edge
            Edge edge = new Edge
            {
                StartNode = node1,
                EndNode = node2,
                isOriented = isOriented
            };

            for (int i = 0; i < edgePoints.Count; i++)
                edge.polyline.Points.Add(edgePoints[i]);

            return SetUpEdge(edge);
        }

        private Edge CreateEdge(Edge edge)
        {
            return SetUpEdge(edge);
        }


        private Edge SetUpEdge(Edge edge)
        {
            edgePoints = new List<Point>();
            edge.MouseDown += Edge_MouseDown;
            edge.KeyDown += Edge_PressedKey;
            edge.SetArrowVisibility();
            edge.CreateInflectionPoints();
            edge.PaintTheEdge();

            for (int i = 0; i < edge.inflectionEllipses.Count; i++)
            {
                edge.inflectionEllipses[i].MouseMove += Ellipse_MouseMove;
                edge.inflectionEllipses[i].MouseDown += Ellipse_MouseDown;
                edge.inflectionEllipses[i].MouseUp += Ellipse_MouseUp;
            }

            graph.AddEdge(edge);
            MainCanvas.Children.Add(edge);
            edge.StartNode.edges.Add(edge);
            edge.EndNode.edges.Add(edge);
            edge.UpdateNodePositions();

            return edge;
        }

        // Обработка перемещения мыши
        private void Ellipse_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging == true && (selectedNodes != null &&  selectedNodes != null && (selectedNodes.Count + selectedInflectionNodes.Count) > 1 ) &&
                (selectedNodes.Contains(sender) || selectedInflectionNodes.Contains(sender)))
            {
                Point mouseCurrentPosition = e.GetPosition(MainCanvas);
                // Вычисляем, на сколько сдвинулась мышь
                double offsetX = mouseCurrentPosition.X - mouseStartPosition.X;
                double offsetY = mouseCurrentPosition.Y - mouseStartPosition.Y;
                foreach (Node selectedNode in selectedNodes)
                {
                    
                    // Получаем текущие координаты объекта
                    double currentLeft = Canvas.GetLeft(selectedNode);
                    double currentTop = Canvas.GetTop(selectedNode);

                    // Задаем новые координаты объекта, с учетом сдвига
                    Canvas.SetLeft(selectedNode, currentLeft + offsetX);
                    Debug.WriteLine(offsetX);
                    Canvas.SetTop(selectedNode, currentTop + offsetY);

                    UpdateEdgesPosition(selectedNode);
                }
                foreach (InflectionNode ellipse in selectedInflectionNodes)
                {
                    
                    // Получаем текущие координаты объекта
                    double currentLeft = Canvas.GetLeft(ellipse);
                    double currentTop = Canvas.GetTop(ellipse);

                    // Задаем новые координаты объекта, с учетом сдвига
                    Canvas.SetLeft(ellipse, currentLeft + offsetX);
                    Debug.WriteLine(offsetX + "asdasdas");
                    Canvas.SetTop(ellipse, currentTop + offsetY);

                    UpdateEdgesPosition(ellipse);
                }
                mouseStartPosition = mouseCurrentPosition;
            }
            else if (isDragging && movingObject != null)
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

                if (sender is Node node )
                {
                    UpdateEdgesPosition(node);
                }
                else if(sender is InflectionNode ellipse)
                {
                    UpdateEdgesPosition(ellipse);            
                }
            }
        }

        private void UpdateEdgesPosition(Node node)
        {
            for (int i = 0; i < node.edges.Count; i++)
            {
                node.edges[i].UpdateNodePositions();
            }
        }
        private void UpdateEdgesPosition(InflectionNode node)
        {
            node.motherEdge.UpdateMiddlePositions(node);
        }

        // Обработка отпускания кнопки мыши
        private void Ellipse_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (selectedNodes != null)
            {
                foreach (Node selectedNode in selectedNodes)
                    UpdateEdgesPosition(selectedNode);
            }

            if (selectedInflectionNodes != null)
            {
                foreach (InflectionNode inflectionNode in selectedInflectionNodes)
                    UpdateEdgesPosition(inflectionNode);
            }

            
            if (sender is Node node)
                UpdateEdgesPosition(node);
            else if (sender is InflectionNode inflectionNode)
                UpdateEdgesPosition(inflectionNode);
            


            isDragging = false;   // Прекращаем перемещение
            
            if (movingObject != null)
            {
                movingObject.ReleaseMouseCapture();   // Освобождаем захват мыши
                movingObject = null;   // Сбрасываем объект
            }
        }

        private void CreateNode(Node newNode)
        {
            newNode.MouseDown += Ellipse_MouseDown;
            newNode.MouseUp += Ellipse_MouseUp;
            newNode.MouseMove += Ellipse_MouseMove;
            newNode.KeyDown += Ellipse_PressedKey;
            newNode.ellipse.Stroke = newNode.nodeStroke;

            // Добавляем новый эллипс на Canvas
            MainCanvas.Children.Add(newNode);
            graph.AddNode(newNode);
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

                CreateNode(newNode);
                UpdateGraphStats();
            }
            else if (e.Key == Key.R)
            {
                isSelectingEdges = !isSelectingEdges;
                UpdateModesStatus();
                selectedNode1 = null;
                selectedNode2 = null;
            }
            else if (e.Key == Key.K)
            {
                isSelectingNodesForTheShortestPathFinding = !isSelectingNodesForTheShortestPathFinding;
                UpdateModesStatus();
                selectedNode1 = null;
                selectedNode2 = null;
                if(!isSelectingNodesForTheShortestPathFinding)
                    UpdatePathCost(0, false);
            }
            else if (e.Key == Key.Q)
            {
                isSelectingNodesFortPathFinding = !isSelectingNodesFortPathFinding;
                UpdateModesStatus();
                selectedNode1 = null;
                selectedNode2 = null;
                if(!isSelectingNodesFortPathFinding)
                    UpdatePathCost(0, false);
            }
            else if (e.Key == Key.N)
            {
                if(isSelectingNodesFortPathFinding == false)
                    return;
                ShowTheWay(paths, ++foundPathIndex);
            }
            else if(e.Key == Key.L)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "XML Files (*.xml)|*.xml", // Фильтр для выбора XML
                    Title = "Сохранить граф",
                    DefaultExt = "xml" // Расширение файла по умолчанию
                };

                // Показываем диалог и проверяем, выбрал ли пользователь файл
                if (saveFileDialog.ShowDialog() == true)
                {
                    string filePath = saveFileDialog.FileName; // Путь для сохранения

                    try
                    {
                        // Сохраняем граф в файл
                        SaveGraphToXml(filePath, graph.Nodes, graph.Edges);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при сохранении графа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                
                
            }
            else if(e.Key == Key.M)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "XML Files (*.xml)|*.xml", // Фильтр для выбора только XML файлов
                    Title = "Выберите файл с графом"
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    string filePath = openFileDialog.FileName; // Путь к выбранному файлу
                    try
                    {
                        LoadGraphFromXml(filePath, MainCanvas, graph.Nodes, graph.Edges);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при загрузке графа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                
            }
            else if(e.Key == Key.C)
            {
                string message = string.Empty;
                if (GraphConnectivityChecker.CheckGraphConnectivity(graph))
                    message = "Граф связный";
                else
                    message = "Граф несвязный";
                MessageBox.Show(message, "Проверка на графа на связность", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            else if (e.Key == Key.F)
            {
                GraphConnectivityChecker.FixGraphConnectivity(graph);
            }
            else if (e.Key == Key.E)
            {
                ShowEulerianCycles(graph);
            }
            else if (e.Key == Key.J)
            {
                ShowVertexInfo();
            }
        }
        
        private void ShowEulerianCycles(Graph graph)
        {     
            List<List<Node>> eulerianCycles = EulerianCycleFinder.FindEulerianCycles(graph);
            if (eulerianCycles == null)
            {
                MessageBox.Show("Граф не удовлетворяет условиям существования Эйлерова цикла", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var window = new EulerianCyclesWindow(eulerianCycles);
            window.ShowDialog();
        }
        
        private void DeleteEdges(Node node)
        {
            for(int i =0; i<node.edges.Count; i++)
            {
                graph.RemoveEdge(node.edges[i]);
                MainCanvas.Children.Remove(node.edges[i]);
                UpdateGraphStats();
            }
        }

        private void Ellipse_PressedKey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete) // Если нажата клавиша D — удаляем эллипс
            {
                if (sender is Node node)
                {
                    DeleteEdges(node);
                    graph.RemoveNode(node);
                    MainCanvas.Children.Remove(node);
                    MainCanvas.Focus();
                    UpdateGraphStats();
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
            NodeCountText.Text = "В графе " + graph.Nodes.Count + " вершин";
            EdgeCountText.Text = "В графе " + graph.Edges.Count + " дуг";
            if (lastFocusedObject is Node node)
                NodeMultiplicityCountText.Text = "Степень данной вершины: " + node.edges.Count;
            else
                NodeMultiplicityCountText.Text = String.Empty;
        }

        private void UpdateModesStatus()
        {
            string edgeCreatingStatus = isSelectingEdges ? "On" : "Off";
            string waysFindingStatus = isSelectingNodesFortPathFinding ? "On" : "Off";
            string shortestWayFindingStatus = isSelectingNodesForTheShortestPathFinding ? "On" : "Off";
            IsEdgeCreatingMode.Text = "Режим создания рёбер: " + edgeCreatingStatus;
            IsFindingWaysMode.Text = "Режим поиска путей: " + waysFindingStatus;
            IsFindingTheShortestWayMode.Text = "Режим поиска кратчайшего пути: " + shortestWayFindingStatus;
        }

        private void UpdatePathCost(int cost, bool isItVisible)
        {
            if(isItVisible)
                PathCostWindow.Visibility = Visibility.Visible;
            else
                PathCostWindow.Visibility = Visibility.Collapsed;
            PathCostTextBlock.Text = "Стоимость пути: " + cost.ToString();
        }

        private void MainCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isDragging == false && e.LeftButton == MouseButtonState.Pressed)
            {
                selectedNodes = null;
                graph.PaintAllNodes();
                if(lastFocusedObject is Node node)
                    node.RepresentFocus();
                // Указываем, что начинается выделение\
                isRectangleSelectingMode = true;

                // Сохраняем начальную точку
                startRectanglePoint = e.GetPosition(MainCanvas);

                // Создаём прямоугольник выделения
                selectionRectangle = new Rectangle
                {
                    Stroke = Brushes.Blue,
                    StrokeThickness = 1,
                    Fill = new SolidColorBrush(Color.FromArgb(50, 0, 0, 255)) // Полупрозрачный синий цвет
                };

                // Устанавливаем начальное положение прямоугольника
                Canvas.SetLeft(selectionRectangle, startRectanglePoint.X);
                Canvas.SetTop(selectionRectangle, startRectanglePoint.Y);

                // Добавляем прямоугольник на Canvas
                MainCanvas.Children.Add(selectionRectangle);
            }
            
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

        public void LoadGraphFromXml(string filePath, Canvas canvas, List<Node> nodes, List<Edge> edges)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SerializableGraph));
            SerializableGraph serializableGraph;

            canvas.Children.Clear();
            nodes.Clear();
            edges.Clear();

            using (StreamReader reader = new StreamReader(filePath))
            {
                serializableGraph = (SerializableGraph)serializer.Deserialize(reader);
            }

            Dictionary<int, Node> nodeMap = new Dictionary<int, Node>();

            // Восстанавливаем узлы
            foreach (SerializableNode serializableNode in serializableGraph.Nodes)
            {
                Node node = Node.FromSerializableNode(serializableNode);
                nodeMap[serializableNode.Id] = node;
                CreateNode(node);
            }

            // Восстанавливаем рёбра
            foreach (SerializableEdge serializableEdge in serializableGraph.Edges)
            {
                Edge edge = Edge.FromSerializableEdge(serializableEdge, nodeMap);
                for (int i = 0; i < edge.polyline.Points.Count; i++)
                    edgePoints.Add(edge.polyline.Points[i]);
                CreateEdge(edge).UpdateNodePositions();
            }

            TabItem tabItem = GetTab();
            
            tabItem.Header = System.IO.Path.GetFileNameWithoutExtension(filePath);
            UpdateGraphStats();
        }
        private TabItem GetTab()
        {
            DependencyObject  obj = this;
            while (obj != null && !(obj is TabItem))
            {
                obj = LogicalTreeHelper.GetParent(obj);
            }
            return obj as TabItem;
        }

        private void MainCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isRectangleSelectingMode)
            {
                // Завершаем выделение
                isRectangleSelectingMode = false;

                // Пример логики завершения (вы можете удалить или оставить прямоугольник)
                GetNodesInSelection(selectionRectangle);

                // Удаляем прямоугольник с Canvas
                MainCanvas.Children.Remove(selectionRectangle);
                selectionRectangle = null;
            }
        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isRectangleSelectingMode && selectionRectangle != null)
            {
                // Текущая позиция мыши
                Point currentPoint = e.GetPosition(MainCanvas);

                // Вычисляем размеры и положение прямоугольника
                double x = Math.Min(currentPoint.X, startRectanglePoint.X);
                double y = Math.Min(currentPoint.Y, startRectanglePoint.Y);
                double width = Math.Abs(currentPoint.X - startRectanglePoint.X);
                double height = Math.Abs(currentPoint.Y - startRectanglePoint.Y);

                // Обновляем свойства прямоугольника
                Canvas.SetLeft(selectionRectangle, x);
                Canvas.SetTop(selectionRectangle, y);
                selectionRectangle.Width = width;
                selectionRectangle.Height = height;
            }
        }
        
        private void GetNodesInSelection(Rectangle selectionRectangle)
        {
            List<Node> selectedNodes = new List<Node>();
            List<InflectionNode> selectedInflectionNodes = new List<InflectionNode>();

            // Получаем координаты выделенного прямоугольника
            double selectionX = Canvas.GetLeft(selectionRectangle);
            double selectionY = Canvas.GetTop(selectionRectangle);
            double selectionWidth = selectionRectangle.Width;
            double selectionHeight = selectionRectangle.Height;

            // Проходим по всем элементам Canvas
            foreach (var child in MainCanvas.Children)
            {
                if (child is Node node)
                {
                    // Получаем координаты узла
                    double nodeX = Canvas.GetLeft(node) + node.ActualWidth / 2; // Центр узла
                    double nodeY = Canvas.GetTop(node) + node.ActualHeight / 2;
                    
                    // Проверяем, входит ли узел в прямоугольник
                    if (nodeX >= selectionX && nodeX <= selectionX + selectionWidth &&
                        nodeY >= selectionY && nodeY <= selectionY + selectionHeight)
                    {
                        selectedNodes.Add(node);
                        node.RepresentFocus();
                    }
                }

                else if (child is Edge edge)
                {
                    foreach (var inflectionNode in edge.inflectionEllipses)
                    {
                        double nodeX = Canvas.GetLeft(inflectionNode) + inflectionNode.ActualWidth / 2; // Центр узла
                        double nodeY = Canvas.GetTop(inflectionNode) + inflectionNode.ActualHeight / 2;
                    
                        // Проверяем, входит ли узел в прямоугольник
                        if (nodeX >= selectionX && nodeX <= selectionX + selectionWidth &&
                            nodeY >= selectionY && nodeY <= selectionY + selectionHeight)
                        {
                            selectedInflectionNodes.Add(inflectionNode);
                        }
                    }
                    
                }
              
            }

            this.selectedNodes = selectedNodes;
            this.selectedInflectionNodes = selectedInflectionNodes;

        }

        private void MainCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Проверяем, зажата ли клавиша Ctrl
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                // Коэффициент изменения масштаба
                double scaleFactor = e.Delta > 0 ? 1.1 : 0.9;
        
                // Новое значение масштаба
                scale *= scaleFactor;
                scaleTransform.ScaleX = scale;
                scaleTransform.ScaleY = scale;
        
                // Позиция курсора относительно Canvas
                Point cursorPosition = e.GetPosition(MainCanvas);
        
                // Устанавливаем центр масштабирования
                scaleTransform.CenterX = cursorPosition.X;
                scaleTransform.CenterY = cursorPosition.Y;
        
                e.Handled = true; // Указываем, что событие обработано
            }
        }
        private void ShowVertexInfo()
        {
            if (graph.Nodes.Count == 0)
            {
                MessageBox.Show("Граф не содержит вершин.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Создаём и отображаем окно с информацией о вершинах
            VertexesDegreeWindow vertexInfoWindow = new VertexesDegreeWindow(graph.Nodes);
            vertexInfoWindow.ShowDialog();
        }
        
        private void ScrollViewer_GotFocus(object sender, RoutedEventArgs e)
        {
            MainCanvas.Focus();
        }
    }
}

