
//Класс для реализации графа

using System;
using System.Collections.Generic;

namespace Graph1_ak {
    [Serializable]
    public class CGraph {
        public List<CGraphVertex> Tops;
        public List<CGraphEdge> Edges;

        public CGraph() {
            Edges = new List<CGraphEdge>();
            Tops = new List<CGraphVertex>();
        }

        public void AddVertex(CGraphVertex top) {
            Tops.Add(top);
        }

        public void AddEdge(CGraphVertex x, CGraphVertex y) {
            Edges.Add(new CGraphEdge(x, y));
        }

    }
}
