using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdgeSwappingSearch
{
    public class Edge
    {
        private Graph.GraphVertex _v1, _v2;
        public Graph.GraphVertex V1 => _v1;
        public Graph.GraphVertex V2 => _v2;

        public readonly String Id;

        // Keep them in order of id, will make comparisons easier if needed
        public Edge(Graph.GraphVertex u, Graph.GraphVertex v)
        {
            _v1 = u.Id.CompareTo(v.Id) < 0 ? u : v;
            _v2 = _v1 == u ? v : u;
            Id = GetIdForEdge(u, v);
        }

        public static string GetIdForEdge(Graph.GraphVertex u, Graph.GraphVertex v)
            => u.Id.CompareTo(v.Id) < 1 ? u.Id + "<-->" + v.Id : v.Id + "<-->" + u.Id;

        // Don't want to override equals, but there may be ways for the same Egde to be created multiple times so may be worth having this
        public bool IsSameAs(Edge e) => Id == e.Id;

        // may be some other methods that will be relevant for swapping, like "ConnectedTo(Edge e)", is one of the vertices in this edge connected to one of the ones in the other
        // Keep this in mind when dealing with swapping
    }
}
