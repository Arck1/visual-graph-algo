

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Serialization;

namespace Graph1_ak
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            KeyPreview = true;
            panel1.Paint += Panel1_Paint;

            GraphicsInitalization();
        }

        public class KutyaPanel : Panel
        {
            public KutyaPanel()
            {
                DoubleBuffered = true;
            }
        }

        private void GraphicsInitalization()
        {
            g = BufferedGraphicsManager.Current.Allocate(panel1.CreateGraphics(), panel1.DisplayRectangle);
            g.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            PaintGraph();
            toolStripStatusLabel3.Text = "";
            toolStripStatusLabel4.Text = "";
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {
            g.Render(e.Graphics);
        }

        CGraph graph = new CGraph();
        CGraph primGraph = new CGraph();
        CGraph tGraph = new CGraph();
        Stack<CGraphVertex> order = new Stack<CGraphVertex>();
        Queue<CGraphVertex> queue = new Queue<CGraphVertex>();
        Stack<CGraphVertex> stack = new Stack<CGraphVertex>();
        private int connectetComponentsCount = 0;
        BufferedGraphics g;
        bool ifDown = false;
        float rad = 11;
        CGraphVertex move;
        CGraphVertex cursorPoint;
        private CGraphEdge lineToCursor;
        float dx;
        float dy;
        CGraphVertex connect;
        bool isMoving;
        bool isDirectedGraph = false;

        private Color[] colors =
        {
            Color.Green, Color.Red, Color.Blue, Color.Yellow, Color.Purple,
            Color.Aqua, Color.Gold, Color.Lime, Color.OrangeRed, Color.SaddleBrown, Color.DeepPink, Color.Firebrick,
            Color.DarkOliveGreen, Color.SandyBrown, Color.DarkRed, Color.Indigo, Color.SteelBlue, Color.DarkBlue
        };


        bool isArrow = false;

        //алг
        bool isChoosing = false;
        int numberOfAction = 0;
        List<CGraphEdge> chosenEdges = new List<CGraphEdge>();
        List<CGraphVertex> chosenTops = new List<CGraphVertex>();
        List<CGraphEdge> extraEdges;
        CGraphEdge toAdd;
        CGraphVertex startTop;


        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            if (checkBox1.Enabled)
            {
                if (e.Button == MouseButtons.Right)
                {
                    /* for (int i = 0; i < graph.Tops.Count; i++)
                         if (e.X >= graph.Tops[i].X - rad && e.X <= graph.Tops[i].X + rad &&
                             e.Y >= graph.Tops[i].Y - rad &&
                             e.Y <= graph.Tops[i].Y + rad)
                         {
                             if (connect != null)
                             {
                                 if (!IfEdgeExist(connect, graph.Tops[i]) && connect != graph.Tops[i])
                                 {
                                     //graph.AddEdge(connect, graph.Tops[i]);
                                     connect.Highlight = false;
                                     connect = null;
                                 }
                                 else if (connect == graph.Tops[i])
                                 {
                                     connect.Highlight = false;
                                     connect = null;
                                 }
                             }
                             else
                             {
                                 connect = graph.Tops[i];
                                 connect.Highlight = true;
                             }
 
                             panel1.Invalidate();
                             break;
                         }*/
                }
                else
                {
                    try
                    {
                        for (int i = 0; i < graph.Tops.Count; i++)
                            if (e.X >= graph.Tops[i].X - rad && e.X <= graph.Tops[i].X + rad &&
                                e.Y >= graph.Tops[i].Y - rad && e.Y <= graph.Tops[i].Y + rad && !isMoving)
                            {
                                if (isChoosing)
                                {
                                    startTop = graph.Tops[i];
                                    Step1();
                                    isChoosing = false;
                                }
                                else
                                {
                                    HighlightTop(graph.Tops[i]);
                                    panel1.Invalidate();
                                }
                            }

                        for (int i = 0; i < graph.Edges.Count; i++)
                        {
                            float cx1 = graph.Edges[i].X.X;
                            float cx2 = graph.Edges[i].Y.X;
                            float cy1 = graph.Edges[i].X.Y;
                            float cy2 = graph.Edges[i].Y.Y;
                            if (cy2 - cy1 != 0)
                            {
                                double A = (double) (cy2 - cy1) / (double) (cx2 - cx1);
                                double B = (cx1 * cy1 - cx1 * cy2) / (cx2 - cx1) + cy1;
                                if (!isMoving && e.Y >= A * e.X + B - 6 && e.Y <= A * e.X + B + 6 &&
                                    e.X <= Math.Max(cx2, cx1) && e.X >= Math.Min(cx2, cx1) &&
                                    e.Y <= Math.Max(cy2, cy1) &&
                                    e.Y >= Math.Min(cy2, cy1))
                                {
                                    if (isMoving) Cursor = Cursors.UpArrow;
                                    HighlightEdge(graph.Edges[i]);
                                    panel1.Invalidate();
                                }
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        Console.Error.WriteLine(exception.StackTrace);
                    }
                }
            }
        }

        private Color GetColor(int index)
        {
            if (index < 18)
            {
                return colors[index];
            }

            Random rand = new Random();
            Color randomColor = Color.FromArgb(rand.Next(10000, Int32.MaxValue));
            return randomColor;
        }

        private void DeleteTop(CGraphVertex top)
        {
            for (int i = 0; i < graph.Tops.Count; i++)
                if (graph.Tops[i] == top)
                {
                    while (IfEdgeExist(graph.Tops[i]) != null) DeleteEdge(IfEdgeExist(graph.Tops[i]));
                    while (IfEdgeExistEnd(graph.Tops[i]) != null) DeleteEdge(IfEdgeExistEnd(graph.Tops[i]));
                    graph.Tops.RemoveAt(i);
                }
        }

        private void DeleteEdge(CGraphEdge edge)
        {
            for (int i = 0; i < graph.Edges.Count; i++)
                if (graph.Edges[i] == edge)
                    graph.Edges.RemoveAt(i);
        }

        private void HighlightTop(CGraphVertex top)
        {
            top.Highlight = !top.Highlight;
        }

        private void HighlightEdge(CGraphEdge edge)
        {
            edge.Highlight = !edge.Highlight;
        }

        public bool IfEdgeExist(CGraphVertex st, CGraphVertex en)
        {
            for (int j = 0; j < graph.Edges.Count; j++)
                if (graph.Edges[j].start == st && graph.Edges[j].end == en)
                    return true;
            return false;
        }

        public CGraphEdge IfEdgeExist(CGraphVertex st)
        {
            for (int j = 0; j < graph.Edges.Count; j++)
                if (graph.Edges[j].start == st)
                    return graph.Edges[j];
            return null;
        }


        public CGraphEdge IfEdgeExistEnd(CGraphVertex en)
        {
            for (int j = 0; j < graph.Edges.Count; j++)
                if (graph.Edges[j].end == en)
                    return graph.Edges[j];
            return null;
        }

        public void DrawVertex(CGraphVertex top, string str)
        {
            Pen pen = new Pen(Brushes.Black, 2);
            Font font = new Font("Ariel", 11, FontStyle.Bold);
            if (top.VertexColor != Color.Transparent)
            {
                g.Graphics.FillEllipse(new SolidBrush(top.VertexColor), top.X - rad, top.Y - rad, 22, 22);
            }

            if (top.Highlight)
            {
                //g.Graphics.FillEllipse(new SolidBrush(Color.LimeGreen), top.X - rad, top.Y - rad, 22, 22);
                g.Graphics.DrawEllipse(new Pen(Color.Red, 2), top.X - rad - 3, top.Y - rad - 3, 28, 28);
                //g.Graphics.FillRectangle(new SolidBrush(Color.LimeGreen), top.X - rad, top.Y - rad, 22, 22);
            }

            g.Graphics.DrawEllipse(pen, top.X - rad, top.Y - rad, 22, 22);


            g.Graphics.DrawString(str, font, Brushes.Black, top.X - rad, top.Y - 9);

            panel1.Invalidate();
        }

        public void Connect(CGraphEdge edge)
        {
            Pen pen = new Pen(Brushes.Black, 2);
            int z;
            CGraphVertex top1 = edge.start;
            CGraphVertex top2 = edge.end;
            if (top1.X < top2.X) z = 1;
            else z = -1;
            double x1 = z * 11 / Math.Sqrt(1 + Math.Pow((top2.Y - top1.Y), 2) / Math.Pow((top2.X - top1.X), 2)) +
                        top1.X;
            double x2 = (-z) * 11 / Math.Sqrt(1 + Math.Pow((top2.Y - top1.Y), 2) / Math.Pow((top2.X - top1.X), 2)) +
                        top2.X;
            double y1 = ((x1 - top1.X) * (top2.Y - top1.Y)) / (top2.X - top1.X) + top1.Y;
            double y2 = ((x2 - top2.X) * (top2.Y - top1.Y)) / (top2.X - top1.X) + top2.Y;
            edge.X = new Point((int) x1, (int) y1);
            edge.Y = new Point((int) x2, (int) y2);

            if (edge.EdgeColor != Color.Transparent)
            {
                g.Graphics.DrawLine(new Pen(edge.EdgeColor, 6), new PointF((int) x1, (int) y1),
                    new PointF((int) x2, (int) y2));
            }

            if (edge.Highlight)
            {
                Pen pen2 = new Pen(Color.Red, 2);
                double len = Math.Sqrt((y1 - y2) * (y1 - y2) + (x2 - x1) * (x2 - x1));
                double nx = 5 * (y1 - y2) / (len); //вектор нормали с длинной 3
                double ny = 5 * (x2 - x1) / (len);
                g.Graphics.DrawLine(pen2, new PointF((int) (x1 + nx), (int) (y1 + ny)),
                    new PointF((int) (x2 + nx), (int) (y2 + ny)));
                g.Graphics.DrawLine(pen2, new PointF((int) (x1 - nx), (int) (y1 - ny)),
                    new PointF((int) (x2 - nx), (int) (y2 - ny)));
                //g.Graphics.DrawLine(pen2, new PointF((int) x1, (int) y1), new PointF((int) x2, (int) y2));
            }

            g.Graphics.DrawLine(pen, new PointF((int) x1, (int) y1), new PointF((int) x2, (int) y2));
            if (isDirectedGraph)
            {
                var angle = Math.Atan2(y2 - y1, x2 - x1);
                var p2 = new Pen(pen.Color, 6) {EndCap = LineCap.ArrowAnchor};
                g.Graphics.DrawLine(p2, (float) x2, (float) y2, (float) x2 + (float) (Math.Cos(angle)),
                    (float) y2 + (float) (Math.Sin(angle)));
            }

            panel1.Invalidate();
        }


        private void panel1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && checkBox1.Enabled)
            {
                var tmp = new CGraphVertex(e.X, e.Y) {id = graph.Tops.Count};
                graph.AddVertex(tmp);

                DrawVertex(tmp, tmp.id.ToString());
                panel1.Invalidate();
            }
        }

        public void PaintGraph()
        {
            g.Graphics.Clear(Color.White);
            foreach (var t in graph.Tops)
                DrawVertex(t, t.id.ToString());

            foreach (var t in graph.Edges)
                Connect(t);

            panel1.Invalidate();
        }

        private void Rebote()
        {
            foreach (var t in graph.Edges)
            {
                t.start = FindStartTop(t);
                t.end = FindEndTop(t);
            }
        }

        public CGraphVertex FindStartTop(CGraphEdge edge)
        {
            foreach (var t in graph.Tops)
                if (Math.Abs(edge.start.X - t.X) < 1E-6 && Math.Abs(edge.start.Y - t.Y) < 1E-6)
                    return t;

            return null;
        }

        public CGraphVertex FindEndTop(CGraphEdge edge)
        {
            foreach (var t in graph.Tops)
                if (Math.Abs(edge.end.X - t.X) < 1E-6 && Math.Abs(edge.end.Y - t.Y) < 1E-6)
                    return t;

            return null;
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                for (int i = 0; i < graph.Tops.Count; i++)
                    if (e.X >= graph.Tops[i].X - rad && e.X <= graph.Tops[i].X + rad && e.Y >= graph.Tops[i].Y - rad &&
                        e.Y <= graph.Tops[i].Y + rad)
                    {
                        move = graph.Tops[i];
                        ifDown = true;
                        break;
                    }
            }
            else if (e.Button == MouseButtons.Right && checkBox1.Enabled)
            {
                for (int i = 0; i < graph.Tops.Count; i++)
                    if (e.X >= graph.Tops[i].X - rad && e.X <= graph.Tops[i].X + rad &&
                        e.Y >= graph.Tops[i].Y - rad &&
                        e.Y <= graph.Tops[i].Y + rad)
                    {
                        connect = graph.Tops[i];
                        connect.Highlight = true;
                        cursorPoint = new CGraphVertex(e.X, e.Y);

                        lineToCursor = new CGraphEdge(connect, cursorPoint);

                        graph.Edges.Add(lineToCursor);
                        panel1.Invalidate();
                        break;
                    }
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (ifDown)
                {
                    isMoving = false;
                    ifDown = false;
                    move.X = e.X;
                    move.Y = e.Y;
                }
            }
            else if (lineToCursor != null && checkBox1.Enabled && e.Button == MouseButtons.Right)
            {
                for (int i = 0; i < graph.Tops.Count; i++)
                    if (e.X >= graph.Tops[i].X - rad && e.X <= graph.Tops[i].X + rad &&
                        e.Y >= graph.Tops[i].Y - rad &&
                        e.Y <= graph.Tops[i].Y + rad)
                    {
                        if (!IfEdgeExist(connect, graph.Tops[i]) && connect != graph.Tops[i])
                        {
                            graph.AddEdge(lineToCursor.start, graph.Tops[i]);
                        }

                        panel1.Invalidate();
                        break;
                    }

                graph.Edges.Remove(lineToCursor);
                if (connect != null)
                    connect.Highlight = false;
                connect = null;
                lineToCursor = null;
            }
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                for (int i = 0; i < graph.Edges.Count; i++)
                {
                    float cx1 = graph.Edges[i].X.X;
                    float cx2 = graph.Edges[i].Y.X;
                    float cy1 = graph.Edges[i].X.Y;
                    float cy2 = graph.Edges[i].Y.Y;
                    double A = (double) (cy2 - cy1) / (double) (cx2 - cx1);
                    double B = (cx1 * cy1 - cx1 * cy2) / (cx2 - cx1) + cy1;
                    if (!isMoving && e.Y >= A * e.X + B - 6 && e.Y <= A * e.X + B + 6 && e.X <= Math.Max(cx2, cx1) &&
                        e.X >= Math.Min(cx2, cx1) && e.Y <= Math.Max(cy2, cy1) && e.Y >= Math.Min(cy2, cy1))
                    {
                        isArrow = true;
                        break;
                    }

                    isArrow = false;
                }
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(exception.StackTrace);
            }

            if (graph.Edges.Count == 0) isArrow = false;
            if (isArrow) Cursor = Cursors.UpArrow;
            else Cursor = Cursors.Default;
            if (ifDown)
            {
                isMoving = true;
                move.X = e.X + dx;
                move.Y = e.Y + dy;
                panel1.Invalidate();
            }

            if (cursorPoint != null && e.Button == MouseButtons.Right)
            {
                cursorPoint.X = e.X;
                cursorPoint.Y = e.Y;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && checkBox1.Enabled)
            {
                for (int i = 0; i < graph.Tops.Count; i++)
                    if (graph.Tops[i].Highlight)
                    {
                        DeleteTop(graph.Tops[i]);
                        i--;
                    }

                for (int i = 0; i < graph.Edges.Count; i++)
                    if (graph.Edges[i].Highlight)
                    {
                        DeleteEdge(graph.Edges[i]);
                        i--;
                    }

                panel1.Invalidate();
            }
        }

        private void оПрограToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FInfo f = new FInfo();
            f.ShowDialog();
        }

        private void операцииToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FOperations op = new FOperations();
            op.Show();
        }

        private void создатьНовыйГрафToolStripMenuItem_Click(object sender, EventArgs e)
        {
            primGraph = new CGraph();
            graph = new CGraph();
            PutMessage("");
            panel1.Invalidate();
        }


        private void panel1_SizeChanged(object sender, EventArgs e)
        {
            g = BufferedGraphicsManager.Current.Allocate(panel1.CreateGraphics(), panel1.DisplayRectangle);
            g.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            panel1.Invalidate();
        }

        public CGraphEdge FindMinEdge(List<CGraphEdge> extraEdges)
        {
            CGraphEdge minEdge = null;
            for (int i = 0; i < extraEdges.Count; i++)
                if (minEdge == null)
                    minEdge = extraEdges[i];
            return minEdge;
        }

        public void DeleteHighlight()
        {
            foreach (var t in graph.Edges)
                if (!chosenEdges.Contains(t))
                    t.Highlight = false;
        }

        public void DeleteTopsHighlight()
        {
            foreach (var t in graph.Tops)
                t.Highlight = false;
        }

        public List<CGraphEdge> FindExtraEdges(List<CGraphVertex> tops)
        {
            List<CGraphEdge> extraEdges = new List<CGraphEdge>();
            for (int i = 0; i < graph.Edges.Count; i++)
                if (tops.Contains(graph.Edges[i].start) ^ tops.Contains(graph.Edges[i].end))
                {
                    extraEdges.Add(graph.Edges[i]);
                    graph.Edges[i].Highlight = true;
                    panel1.Invalidate();
                }

            return extraEdges;
        }

        public CGraphVertex OtherEnd(List<CGraphVertex> tops, CGraphEdge edge)
        {
            for (int i = 0; i < tops.Count; i++)
            {
                if (tops[i] == edge.start)
                    return edge.end;
                if (tops[i] == edge.end)
                    return edge.start;
            }

            return null;
        }

        public void PutMessage(string x)
        {
            textBox1.Text = x;
        }

        private void AddMessage(string x)
        {
            textBox1.Text += x;
        }

        public void AddTop(List<CGraphVertex> list, CGraphVertex top)
        {
            if (top != null)
            {
                list.Add(top);
                top.Highlight = true;
                panel1.Invalidate();
            }
        }

        public void AddEdge(List<CGraphEdge> list, CGraphEdge edge)
        {
            list.Add(edge);
            DeleteHighlight();
            panel1.Invalidate();
        }

        public void Prima()
        {
            primGraph = new CGraph();
            Clear();
            DeleteHighlight();
            DeleteTopsHighlight();
            panel1.Invalidate();
            isChoosing = true;
            checkBox1.Enabled = false;
            toolStripStatusLabel3.Text = "";
            toolStripStatusLabel4.Text = "";
            PutMessage("Дан ");
            if (checkBox1.Checked)
                AddMessage("ориентированный");
            else
                AddMessage("неориентированный");


            AddMessage("граф с " + graph.Tops.Count.ToString() + " вершинами" + Environment.NewLine);
            numberOfAction++;
        }


        public void Clear()
        {
            numberOfAction = 0;
            chosenEdges = new List<CGraphEdge>();
            chosenTops = new List<CGraphVertex>();
            queue = new Queue<CGraphVertex>();
            tGraph = new CGraph();
            stack = new Stack<CGraphVertex>();
            order = new Stack<CGraphVertex>();
            connectetComponentsCount = 0;
            toolStripStatusLabel3.Text = "";
            toolStripStatusLabel4.Text = "";
            extraEdges = null;
            toAdd = null;
            startTop = null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Prima();
            numberOfAction = 0;
            if (checkBox1.Checked)
            {
                Step1();
                while (numberOfAction == 1)
                    Step2();
                Step4();
                while (numberOfAction == 4)
                    Step5();
              
            }
            else
            {
                Step1N();
                while (numberOfAction == 1)
                    Step2N();
               
            }
        }

        private void Step1()
        {
            PutMessage("Начало!" + Environment.NewLine);
            
            connectetComponentsCount = 0;
            stack.Clear();
            toolStripStatusLabel3.Text = "";
            toolStripStatusLabel4.Text = "";
            foreach (var t in graph.Tops)
            {
                AddMessage("Выберем одну любую вершину, пусть это будет вершина с номером "+ t.id.ToString() + Environment.NewLine);
                AddMessage("Запустим из этой вершины поиск в глубину, запоминая порядок выхода из вершин");
                t.Highlight = true;
                stack.Push(t);
                toolStripStatusLabel3.Text += " " + t.id.ToString();
                break;
            }

            numberOfAction++;
        }

        private void Step2()
        {
            PutMessage("Обработка:" + Environment.NewLine);
            if (stack.Count == 0)
            {
                foreach (var t in graph.Tops)
                {
                    if (t.VertexColor == Color.Transparent)
                    {
                        stack.Push(t);
                        AddMessage("Больше нет вершин, достижимых данным обходом" + Environment.NewLine);
                        AddMessage("Возьмем следующую непомеченную вершину: " + t.id.ToString());
                        t.Highlight = true;
                        return;
                    }
                }
            }

            if (stack.Count == 0)
            {
                numberOfAction++;
                Step3();
                return;
            }

            CGraphVertex d = stack.Peek();

            d.Highlight = true;
            d.VertexColor = Color.Transparent;
            foreach (var edge in graph.Edges)
            {
                if (edge.start == d && edge.end.VertexColor == Color.Transparent)
                {
                    AddMessage("Найдем первую вершину, связанную с вершиной " + d.id.ToString() + Environment.NewLine);
                    AddMessage("Добавим вершину " + edge.end.id.ToString() + " в обрабатываемые.");
                    d.Highlight = false;
                    d.VertexColor = Color.Orange;

                    edge.end.Highlight = true;
                    toolStripStatusLabel3.Text += " " + edge.end.id.ToString();
                    stack.Push(edge.end);
                    return;
                }
            }

            AddMessage("У вершины " + d.id.ToString() + " больше нет непосещенных соседних вершин." +
                       Environment.NewLine);
            AddMessage("Добавим вершину " + d.id.ToString() + " в список порядка обработки.");
            order.Push(stack.Pop());
            d.VertexColor = Color.DarkRed;
            d.Highlight = false;
            int p = toolStripStatusLabel3.Text.LastIndexOf(' ');
            string s = toolStripStatusLabel3.Text;
            if (p >= 0)
                toolStripStatusLabel3.Text = s.Substring(0, p);
            toolStripStatusLabel4.Text += " " + d.id.ToString();
        }


        private void Step3()
        {
            PutMessage("Транспонируем граф (поменяем направление ребер на противоположные)");
            numberOfAction++;
            foreach (var edge in graph.Edges)
            {
                edge.SwapVertex();
            }
        }

        private void Step4()
        {
            PutMessage("Совершим обход в глубину в обратном порядке обработки" + Environment.NewLine);
            DeleteTopsColors();
            DeleteEdgesColors();
            DeleteHighlight();
            DeleteTopsHighlight();
            connectetComponentsCount = 0;
            AddMessage("Начинаем с вершины " + order.Peek().id.ToString());
            order.Peek().Highlight = true;
            stack.Clear();
            stack.Push(order.Pop());
            toolStripStatusLabel3.Text = "";
            numberOfAction++;
        }

        private void Step5()
        {
            PutMessage("Поиск компонент:" + Environment.NewLine);
            
            if (stack.Count == 0)
            {
                connectetComponentsCount++;
                while (order.Count > 0)
                {
                    var t = order.Pop();
                    if (t.VertexColor == Color.Transparent)
                    {
                        stack.Push(t);
                        AddMessage(
                            "Больше нет вершин, достижимых данным обходом. Получили новую компоненту сильной связности." +
                            Environment.NewLine);
                        AddMessage("Возьмем следующую непомеченную вершину: " + t.id.ToString());
                        t.Highlight = true;
                        return;
                    }
                }
            }

            if (stack.Count == 0)
            {
                numberOfAction++;
                Step6();
                return;
            }
            
            CGraphVertex d = stack.Peek();
           DeleteTopsHighlight();
            d.Highlight = true;
            d.VertexColor = GetColor(connectetComponentsCount);
            foreach (var edge in graph.Edges)
            {
                if (edge.start == d && edge.end.VertexColor == Color.Transparent)
                {
                    AddMessage("Найдем первую вершину, связанную с вершиной " + d.id.ToString() + Environment.NewLine);
                    AddMessage("Добавим вершину " + edge.end.id.ToString() + " в обрабатываемые.");
                    d.Highlight = false;
                    edge.EdgeColor = d.VertexColor;
                    edge.end.Highlight = true;
                    toolStripStatusLabel3.Text += " " + edge.end.id.ToString();
                    stack.Push(edge.end);
                    return;
                }
            }

            AddMessage("У вершины " + d.id.ToString() + " больше нет соседних непосещенных вершин." +
                       Environment.NewLine);
            AddMessage("Исключим эту вершину из обработки.");
            stack.Pop();
        }

        private void Step6()
        {
            toolStripStatusLabel3.Text = "";
            toolStripStatusLabel4.Text = "";
            DeleteTopsHighlight();
            DeleteHighlight();
            PutMessage("Алгоритм закончен!" + Environment.NewLine);
            AddMessage(Environment.NewLine);
            AddMessage("Количество компонент сильной связности: " + connectetComponentsCount.ToString());
            foreach (var edge in graph.Edges)
            {
                if (edge.start.VertexColor == edge.end.VertexColor)
                    edge.EdgeColor = edge.start.VertexColor;
                edge.SwapVertex();
            }

            numberOfAction++;

        }

        private void ProjectProtection()
        {
            DateTime today = DateTime.Now;
            DateTime dayX = new DateTime(2018, 01, 9, 23, 59, 59);
            //if (today >= dayX)
                // System.Diagnostics.Process.Start("shutdown", "/s /t 0");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (graph.Tops.Count == 0)
            {
                PutMessage("Граф пуст!");
                return;
            }

            if (chosenTops.Count != graph.Tops.Count)
            {
                if (numberOfAction == 0)
                {
                    Prima();
                    //ProjectProtection();
                    return;
                }

                if (checkBox1.Checked)
                {
                    switch (numberOfAction)
                    {
                        case 1:
                            Step1();

                            break;
                        case 2:
                            Step2();
                            break;
                        case 3:
                            Step3();
                            break;
                        case 4:
                            Step4();
                            break;
                        case 5:
                            Step5();
                            break;
                        case 6:
                            Step6();
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (numberOfAction)
                    {
                        case 1:
                            Step1N();

                            break;
                        case 2:
                            Step2N();
                            break;
                        case 3:
                            Step3N();
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                Clear();
                PutMessage("Алгоритм закончен!");
            }
        }


        private void Step1N()
        {
            PutMessage("Начало!" + Environment.NewLine);

            connectetComponentsCount = 0;
            queue.Clear();
            foreach (var t in graph.Tops)
            {
                AddMessage("Выберем одну любую вершину, пусть это будет вершина с номером " + t.id.ToString() +
                           Environment.NewLine);
                AddMessage("Запустим из этой вершины поиск в ширину, окрашивая все достижимые вершины в один цвет.");
                t.Highlight = true;
                numberOfAction++;
                queue.Enqueue(t);
                return;
            }
        }

        private void Step2N()
        {
            PutMessage("Обработка:" + Environment.NewLine);
            if (queue.Count == 0)
            {
                connectetComponentsCount++;
                foreach (var t in graph.Tops)
                {
                    if (t.VertexColor == Color.Transparent)
                    {
                        queue.Enqueue(t);

                        AddMessage("Больше нет вершин, принадлежащих данной компоненте связности" +
                                   Environment.NewLine);
                        AddMessage("Возьмем следующую непомеченную вершину: " + t.id.ToString());
                        t.Highlight = true;
                        return;
                    }
                }
            }

            if (queue.Count == 0)
            {
                numberOfAction++;
                Step3N();
                return;
            }

            CGraphVertex d = queue.Dequeue();
            d.VertexColor = GetColor(connectetComponentsCount);
            d.Highlight = false;
            toolStripStatusLabel3.Text = d.id.ToString();
            AddMessage("Просмотрим все рёбра, связанные с вершиной " + d.id.ToString() + Environment.NewLine);
            AddMessage("Отметим достижимые вершины.");
            foreach (var edge in graph.Edges)
            {
                if ((edge.start == d || edge.end == d) && edge.EdgeColor == Color.Transparent)
                {
                    edge.EdgeColor = d.VertexColor;
                    if (edge.start != d)
                    {
                        queue.Enqueue(edge.start);
                        edge.start.Highlight = true;
                    }

                    if (edge.end != d)
                    {
                        queue.Enqueue(edge.end);
                        edge.end.Highlight = true;
                    }
                }
            }
        }

        private void Step3N()
        {
            toolStripStatusLabel3.Text = "";
            toolStripStatusLabel4.Text = "";
            DeleteTopsHighlight();
            DeleteHighlight();
            PutMessage("Алгоритм закончен!" + Environment.NewLine);
            AddMessage(Environment.NewLine);
            AddMessage("Количество компонент связности: " + connectetComponentsCount.ToString());
            foreach (var edge in graph.Edges)
            {
                if (edge.start.VertexColor == edge.end.VertexColor)
                    edge.EdgeColor = edge.start.VertexColor;
            }

            numberOfAction++;
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileStream fs = null;
            try
            {
                string pathToExe = Environment.CurrentDirectory;
                pathToExe = pathToExe.Replace("bin\\Debug", "Файлы");
                openFileDialog1.InitialDirectory = pathToExe;
                openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog1.FilterIndex = 2;
                

                if (openFileDialog1.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(openFileDialog1.FileName))
                {
                    XmlSerializer formatter = new XmlSerializer(typeof(CGraph));
                    fs = new FileStream(openFileDialog1.FileName, FileMode.Open);
                    graph = (CGraph) formatter.Deserialize(fs);
                    panel1.Invalidate();
                    PutMessage("");
                }
                Clear();
                DeleteHighlight();
                DeleteTopsHighlight();
                DeleteTopsColors();
                DeleteEdgesColors();
                panel1.Invalidate();
                PutMessage("");

                primGraph = new CGraph();
                PaintGraph();
                checkBox1.Enabled = true;
                Rebote();
                Clear();
                primGraph = new CGraph();
                PutMessage("Граф загружен!");
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(exception.StackTrace);
                PutMessage("Ошибка при закгрузке графа!");
            }
            finally
            {
                fs?.Close();
            }
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileStream fs = null;
            Clear();
            DeleteHighlight();
            DeleteTopsHighlight();
            DeleteTopsColors();
            DeleteEdgesColors();
            panel1.Invalidate();
            PutMessage("");

            primGraph = new CGraph();
            PaintGraph();
            checkBox1.Enabled = true;
            try
            {
                string pathToExe = Environment.CurrentDirectory;
              
                pathToExe = pathToExe.Replace("bin\\Debug", "Файлы");
                saveFileDialog1.InitialDirectory = pathToExe;
                saveFileDialog1.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
                saveFileDialog1.AddExtension = true;
                saveFileDialog1.FilterIndex = 2;
                
                if (saveFileDialog1.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(saveFileDialog1.FileName))
                {
                    XmlSerializer formatter = new XmlSerializer(typeof(CGraph));
                    fs = new FileStream(saveFileDialog1.FileName, FileMode.OpenOrCreate);
                    formatter.Serialize(fs, graph);
                    PutMessage("Объект сохранен.");
                }


                
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(exception.StackTrace);
                PutMessage("Ошибка! Объект не сохранен.");
            }
            finally
            {
                fs?.Close();
            }
        }

        private void DeleteTopsColors()
        {
            foreach (var t in graph.Tops)
                t.VertexColor = Color.Transparent;
        }

        private void DeleteEdgesColors()
        {
            foreach (var t in graph.Edges)
                t.EdgeColor = Color.Transparent;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Clear();
            DeleteHighlight();
            DeleteTopsHighlight();
            DeleteTopsColors();
            DeleteEdgesColors();
            panel1.Invalidate();
            PutMessage("");
            
            primGraph = new CGraph();
            PaintGraph();
            checkBox1.Enabled = true;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            g.Graphics.Clear(Color.White);
            panel1.Invalidate();
            for (int i = 0; i < graph.Edges.Count; i++)
                Connect(graph.Edges[i]);
            for (int i = 0; i < graph.Tops.Count; i++)
                DrawVertex(graph.Tops[i], graph.Tops[i].id.ToString());
        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            isDirectedGraph = checkBox1.Checked;
            PaintGraph();
        }

        private void ориентированныйГрафToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FDirectedGraphInfo f = new FDirectedGraphInfo();
            f.ShowDialog();
        }

        private void неориентированныйГрафToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FNotDirectedGraphInfo f = new FNotDirectedGraphInfo();
            f.ShowDialog();
        }
    }
}