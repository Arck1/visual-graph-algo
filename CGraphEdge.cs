
//Класс ребер графа

using System.Drawing;

namespace Graph1_ak {
    public class CGraphEdge {
        //поле данных
        //public int Length;
        //private CGraphVertex Start;
        //private CGraphVertex End;
        //public PointF x;
       // public PointF y;
        //public bool highlight;
        //конструктор
        public CGraphEdge(CGraphVertex x, CGraphVertex y) {

            start = x;
            end = y;
            Highlight = false;
            EdgeColor = Color.Transparent;
        }
        public CGraphEdge() {
        }
        //длина
       
        //начальная вершина
        public CGraphVertex start
        {
            get;
            set;
        }
        //конечная вершина
        public CGraphVertex end
        {
            get;
            set;
        }
        //
        public PointF X
        {
            get;
            set;
        }
        //
        public PointF Y
        {
            get;
            set;
        }
        //
        public bool Highlight
        {
            get;
            set;
        }
        public Color EdgeColor { get; set; }

        public void SwapVertex()
        {
            CGraphVertex tmp = this.start;
            this.start = this.end;
            this.end = tmp;
        }
    }
}
