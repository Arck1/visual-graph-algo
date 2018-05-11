
//Класс вершин графа

using System.Drawing;

namespace Graph1_ak
{
    public class CGraphVertex
    {
        // public float x;
        //public float y;
        //public bool highlight;

        public CGraphVertex(float cx, float cy)
        {
            X = cx;
            Y = cy;
            Highlight = false;
            VertexColor = Color.Transparent;
        }

        public CGraphVertex()
        {
        }

        public int id { get; set; }

        public float X { get; set; }

        public float Y { get; set; }

        public bool Highlight { get; set; }

        public Color VertexColor { get; set; }
    }
}