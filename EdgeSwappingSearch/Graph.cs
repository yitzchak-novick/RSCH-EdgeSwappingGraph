using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EdgeSwappingSearch
{
    public class Graph
    {
        public class GraphVertex : Vertex
        {
            private readonly string _id;
            public string Id => _id;
            public HashSet<GraphVertex> _neighbors = new HashSet<GraphVertex>();
            public IEnumerable<Vertex> Neighbors => _neighbors;

            // Cache the degree
            public int _degree = 0;
            public int Degree => _degree;

            public int _sumOfNeighborsDegrees = 0;
            public int SumOfNeighborsDegrees => _sumOfNeighborsDegrees;

            public double Fi => (double)SumOfNeighborsDegrees / (Degree * Degree);

            public bool IsHappy => Degree > (double)SumOfNeighborsDegrees/Degree;

            public bool IsSad => Degree < (double) SumOfNeighborsDegrees/Degree;

            public bool IsNeutral => Degree*Degree == SumOfNeighborsDegrees;

            public GraphVertex(string id)
            {
                _id = id;
            }

            public bool HasNeighbor(Vertex v) => _neighbors.Contains(v);

            public override string ToString()
            {
                return $"Id: {Id}, Deg: {Degree}";
            }
        }

        private Dictionary<String, GraphVertex> _allVertices = new Dictionary<string, GraphVertex>();

        public Vertex this[String Id] => _allVertices[Id];

        // Keep a cached list of vertices that will be included in calculations
        private HashSet<GraphVertex> _verticesWithDegree = new HashSet<GraphVertex>();
        public IEnumerable<Vertex> Vertices => _verticesWithDegree;

        private Dictionary<String, Edge> _edges = new Dictionary<String, Edge>();
        public IEnumerable<Edge> Edges => _edges.Values.ToList();

        // cached values for Afi, remember these should only include non-zero degree
        private double _sumOfAllFis = 0;
        private int countOfVertices = 0;

        public double Afi => _sumOfAllFis/countOfVertices;

        // Assortativity makes use of 3 terms, (1) sum of products of edge endpoints, (2) 1/2 sum of all edgepoints squared, 
        // (3) 1/2 sum of squares of all edgepoints
        private int _numberOfEdges = 0;
        public int NumberOfEdges => _numberOfEdges; // M
        private int sumOfProductsOfAllEdgeEndPoints = 0; // SUM(j_i k_i)
        private int sumOfSquaresOfAllEdgeEndPoints = 0; // SUM (j_i^2 + k_i^2)
        private int sumOfAllEdgeEndPoints = 0; // SUM (j_i + k_i)

        public double Assortativity
        {
            get
            {
                if (_numberOfEdges == 0)
                    return Double.NaN;

                double oneOverM = 1.0/_numberOfEdges;
                double squareOfOneOverMHalfSumOfEdgeEndPoints = Math.Pow(oneOverM*0.5*sumOfAllEdgeEndPoints, 2);
                double numerator = oneOverM*sumOfProductsOfAllEdgeEndPoints - squareOfOneOverMHalfSumOfEdgeEndPoints;
                double denominator = oneOverM*0.5*sumOfSquaresOfAllEdgeEndPoints -
                                     squareOfOneOverMHalfSumOfEdgeEndPoints;

                // think this is only possible with perfect assortativity, should probably come back and
                // examine this more closely at some point
                if (denominator == 0)
                    return 1.0;

                return numerator/denominator;
            }
        }

        public bool AddEdge(String id1, String id2)
        {
            GraphVertex v1, v2;

            if (!_allVertices.ContainsKey(id1))
                _allVertices[id1] = new GraphVertex(id1);
            
            if (!_allVertices.ContainsKey(id2))
                _allVertices[id2] = new GraphVertex(id2);
            
            v1 = _allVertices[id1];
            v2 = _allVertices[id2];
            if (v1.HasNeighbor(v2))
                return false;

            var v1OldDegree = v1.Degree;
            var v1NewDegree = v1.Degree + 1;
            var v2OldDegree = v2.Degree;
            var v2NewDegree = v2.Degree + 1;

            // Change the graph assortativity based on this new edge

            // (1) SUM(j_i k_i) is increased by v1NewDegree * v2NewDegree because of this edge
            sumOfProductsOfAllEdgeEndPoints += v1NewDegree*v2NewDegree;
            // And also, every edge that is attached to
            // one of these vertices has another endpoint whose degree will be mutiplied by v1(or2)NewDegree instead of v1(or2)OldDegree
            // which means we will add each degree once, or add the whole sum in once. So we can use the cached sum for this
            sumOfProductsOfAllEdgeEndPoints += v1._sumOfNeighborsDegrees + v2._sumOfNeighborsDegrees;
            // (2) SUM (j_i^2 + k_i^2) is increased by vNew^2 (just looking at one vertex for the illustration, other is symmetric)
            // because of the new edge. Then, for every existing edge, the increase for this v is vNew^2 - vOld^2, and there are vOld
            // of these edges, so there is an increase of vOld(vNew^2 - vOld^2) = vOld * vNew^2 - vOld * vOld^2 = vOld * vNew^2 - vOld^3
            // Add in the vNew^2 for the new edge, you get (vOld + 1)(vNew^2) - vOld^3 = (vNew)(vNew^2) - vOld^3 = vNew^3 - vOld^3
            sumOfSquaresOfAllEdgeEndPoints += (int) Math.Pow(v1NewDegree, 3) - (int) Math.Pow(v1OldDegree, 3) +
                                              (int) Math.Pow(v2NewDegree, 3) - (int) Math.Pow(v2OldDegree, 3);
            // (3) SUM (j_i + k_i) is increased by vNew for the new edge, and 1 for each of the other edges, and there are vOld of these, so
            // the increase is vNew + vOld
            sumOfAllEdgeEndPoints += v1NewDegree + v1OldDegree + v2NewDegree + v2OldDegree;


            // remove the current FIs from the total, the FIs that will change are the two vertices in question, and all of their neighbors
            var affectedVertices = new HashSet<GraphVertex> {v1, v2}; // Use a HashSet to quickly collect all neighbors without duplicates
            foreach (var v in v1._neighbors)
                affectedVertices.Add(v);
            foreach (var v in v2._neighbors)
                affectedVertices.Add(v);

            // remove the Fis of all affected vertices, add them back in after the attachment
            foreach (var v in affectedVertices)
                _sumOfAllFis -= v.Degree > 0 ? v.Fi : 0;

            // update the degrees and sums of the two vertices
            v1._degree = v1NewDegree;
            v2._degree = v2NewDegree;
            v1._sumOfNeighborsDegrees += v2NewDegree;
            v2._sumOfNeighborsDegrees += v1NewDegree;
            // all neighbors of these vertices now have a neighbor with 1 higher degree than before
            foreach (var neighbor in v1._neighbors)
                neighbor._sumOfNeighborsDegrees++;
            foreach (var neighbor in v2._neighbors)
                neighbor._sumOfNeighborsDegrees++;
            
            // add the new FIs into the total
            foreach (var v in affectedVertices)
                _sumOfAllFis += v.Fi;
            
            // if these were degree 0 (either they are new, or just were degree 0) update the collection
            // of relevant vertices
            if (v1OldDegree == 0)
            {
                _verticesWithDegree.Add(v1);
                countOfVertices++;
            }

            if (v2OldDegree == 0)
            {
                _verticesWithDegree.Add(v2);
                countOfVertices++;
            }

            // actually attach the vertices
            v1._neighbors.Add(v2);
            v2._neighbors.Add(v1);

            // Add an edge to the collection
            Edge newEdge = new Edge(v1, v2);
            _edges[newEdge.Id] = newEdge;
            _numberOfEdges++;

            return true;
        }

        public bool RemoveEdge(String id1, String id2)
        {
            if (!(_allVertices.ContainsKey(id1) && _allVertices.ContainsKey(id2)))
                return false;
            GraphVertex v1 = _allVertices[id1];
            GraphVertex v2 = _allVertices[id2];

            if (!v1.HasNeighbor(v2))
                return false;

            var v1OldDegree = v1.Degree;
            var v1NewDegree = v1.Degree - 1;
            var v2OldDegree = v2.Degree;
            var v2NewDegree = v2.Degree - 1;

            // Change the graph assortativity based on this new edge

            // (1) SUM(j_i k_i) is decreased by v1OldDegree * v2OldDegree because of this edge
            sumOfProductsOfAllEdgeEndPoints -= v1OldDegree * v2OldDegree;
            // And also, every edge that is attached to
            // one of these vertices has another endpoint whose degree will be mutiplied by v1(or2)NewDegree instead of v1(or2)OldDegree
            // which means we will subtract each degree once. This is the sum of v1NeighborsDegrees - v2oldDegree                               
            sumOfProductsOfAllEdgeEndPoints -= (v1._sumOfNeighborsDegrees - v1OldDegree) + (v2._sumOfNeighborsDegrees - v2OldDegree);
            // (2) SUM (j_i^2 + k_i^2) is decreased by vNew (just looking at one vertex for the illustration, other is symmetric)
            // because of the new edge. Then, for every existing edge, the change for this v is -vOld^2 + vNew^2, and there are vNew
            // of these edges, so there we are adding in vNew(-vOld^2 + vNew^2) = vNew * -vOld^2 + vNew * vNew^2 = vNew * -vOld^2 + vNew^3
            // Add in the -vOld^2 for the old edge itself, you get (vNew + 1)(-vOld^2) - vOld^3, vNew + 1 = vOld, so -vOld^3 + vNew^3
            sumOfSquaresOfAllEdgeEndPoints -= (int)Math.Pow(v1OldDegree, 3) - (int)Math.Pow(v1NewDegree, 3) +
                                              (int)Math.Pow(v2OldDegree, 3) - (int)Math.Pow(v2NewDegree, 3);
            // (3) SUM (j_i + k_i) is decreased by vOld for the new edge, and 1 for each of the other edges, and there are vNew of these, so
            // the increase is vOld + vNew
            sumOfAllEdgeEndPoints -= v1OldDegree + v1NewDegree + v2OldDegree + v2NewDegree;

            // remove the current FIs from the total, the FIs that will change are the two vertices in question, and all of their neighbors
            var affectedVertices = new HashSet<GraphVertex>(); // Use a HashSet to quickly collect all neighbors without duplicates
            foreach (var v in v1._neighbors) // Note that v2 is a neighbor of v1 and vice versa
                affectedVertices.Add(v);
            foreach (var v in v2._neighbors)
                affectedVertices.Add(v);

            // remove the Fis of all affected vertices, add them back in after the attachment
            foreach (var v in affectedVertices)
                _sumOfAllFis -= v.Fi;

            // update the degrees and sums of the two vertices
            v1._degree--;
            v2._degree--;
            v1._sumOfNeighborsDegrees -= v2OldDegree;
            v2._sumOfNeighborsDegrees -= v1OldDegree;
            // all other neighbors of these vertices now have a neighbor with 1 lower degree than before
            foreach (var neighbor in v1._neighbors)
            {
                if (neighbor == v2)
                    continue;
                neighbor._sumOfNeighborsDegrees--;
            }
            foreach (var neighbor in v2._neighbors)
            {
                if (neighbor == v1)
                    continue;
                neighbor._sumOfNeighborsDegrees--;
            }

            // add the new FIs into the total
            foreach (var v in affectedVertices)
                _sumOfAllFis += v.Degree == 0 ? 0 : v.Fi;

            // actually detatch the vertices
            v1._neighbors.Remove(v2);
            v2._neighbors.Remove(v1);

            // if a vertex is now degree 0, remove it from the collection of relevant vertices
            if (v1NewDegree == 0)
            {
                _verticesWithDegree.Remove(v1);
                countOfVertices--;
            }

            if (v2NewDegree == 0)
            {
                _verticesWithDegree.Remove(v2);
                countOfVertices--;
            }

            // Remove the edge from the collection
            _edges.Remove(Edge.GetIdForEdge(v1, v2));
            _numberOfEdges--;

            return true;
          
        }

        // Special method for a swap, because only the Fis of the four involved vertices will change, neighbors' values
        // stay the same, so the values can be updated more efficiently for a swap than using add and remove
        public void SwapEdges(string v1Id, string v1CurrNeighborId, string v1NewNeighborId, string v2Id, string v2CurrNeighborId,
            string v2NewNeighborId)
        {
            // Check all necessary conditions to ensure this is a legal swap

            // (1) Do all Ids exist in the graph
            if (
                !(_allVertices.ContainsKey(v1Id) && _allVertices.ContainsKey(v1CurrNeighborId) &&
                  _allVertices.ContainsKey(v1NewNeighborId) &&
                  _allVertices.ContainsKey(v2Id) && _allVertices.ContainsKey(v2CurrNeighborId) &&
                  _allVertices.ContainsKey(v2NewNeighborId)))
                throw new Exception("Illegal swap, ID is not found in graph");

            var v1 = _allVertices[v1Id];
            var v1CurrNeighbor = _allVertices[v1CurrNeighborId];
            var v1NewNeighbor = _allVertices[v1NewNeighborId];
            var v2 = _allVertices[v2Id];
            var v2CurrNeighbor = _allVertices[v2CurrNeighborId];
            var v2NewNeighbor = _allVertices[v2NewNeighborId];

            // (2) Are the specified vertices connected
            if (!(v1._neighbors.Contains(v1CurrNeighbor) && v2._neighbors.Contains(v2CurrNeighbor)))
                throw new Exception("Illegal swap, specified vertices are not connected");

            // (3) Do the new connections already exist?
            if (v1._neighbors.Contains(v1NewNeighbor) || v2._neighbors.Contains(v2NewNeighbor))
                throw new Exception("Illegal swap, vertices to be connected are already connected");

            // (4) Is the new neighbor of each the old neighbor of the other
            if (!(v1CurrNeighborId == v2NewNeighborId && v1NewNeighborId == v2CurrNeighborId))
                throw new Exception("Illegal swap, edges are not an exchange of vertices");

            // Remove the FIs of the 4 involved vertices
            _sumOfAllFis -= v1.Fi + v2.Fi + v1CurrNeighbor.Fi + v2CurrNeighbor.Fi;

            // Update the sums of neighbors, remove old neighbor and add new
            v1._sumOfNeighborsDegrees += v1NewNeighbor.Degree - v1CurrNeighbor.Degree;
            v2._sumOfNeighborsDegrees += v2NewNeighbor.Degree - v2CurrNeighbor.Degree;
            v1NewNeighbor._sumOfNeighborsDegrees += v1.Degree - v2.Degree;
            v2NewNeighbor._sumOfNeighborsDegrees += v2.Degree - v1.Degree;

            // put back the new FIs
            _sumOfAllFis += v1.Fi + v2.Fi + v1CurrNeighbor.Fi + v2CurrNeighbor.Fi;

            // For assortativity, the only term that changes is the sum of products because v is multiplied by u' instead of u,
            // but the two other terms are just sums of v or v^2 and in a swap the vertices will still contribute the same amount
            // the same number of times
            sumOfProductsOfAllEdgeEndPoints += v1.Degree*v1NewNeighbor.Degree - v1.Degree*v1CurrNeighbor.Degree;
            sumOfProductsOfAllEdgeEndPoints += v2.Degree * v2NewNeighbor.Degree - v2.Degree * v2CurrNeighbor.Degree;
            
            // Make the connections
            v1._neighbors.Remove(v1CurrNeighbor);
            v1CurrNeighbor._neighbors.Remove(v1);
            v1._neighbors.Add(v1NewNeighbor);
            v1NewNeighbor._neighbors.Add(v1);
            v2._neighbors.Remove(v2CurrNeighbor);
            v2CurrNeighbor._neighbors.Remove(v2);
            v2._neighbors.Add(v2NewNeighbor);
            v2NewNeighbor._neighbors.Add(v2);

            // remove the edges, put in the new ones
            _edges.Remove(Edge.GetIdForEdge(v1, v1CurrNeighbor));
            _edges.Remove(Edge.GetIdForEdge(v2, v2CurrNeighbor));
            _edges[Edge.GetIdForEdge(v1, v1NewNeighbor)] = new Edge(v1, v1NewNeighbor);
            _edges[Edge.GetIdForEdge(v2, v2NewNeighbor)] = new Edge(v2, v2NewNeighbor);

        }

        // Things to add
        // ER, BA, HH (DHH), BD_Random
        // MAX, MIN, No. Pos, No. Neg, No. Neut, etc.
        // A searching class that uses this class and searches...

    }
}
