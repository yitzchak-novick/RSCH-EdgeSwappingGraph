using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static EdgeSwappingSearch.Utils;

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

            public bool IsHappy => Degree * Degree > SumOfNeighborsDegrees;

            public bool IsSad => Degree * Degree < SumOfNeighborsDegrees;

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

                // if all vertices are of the same degree the denominator will be 0, special case of perfect assorativity
                if (denominator == 0)
                    return 1.0;

                return numerator/denominator;
            }
        }

        private double _minFi = double.MaxValue;
        public double MinFi => _minFi;

        private double _maxFi = double.MinValue;
        public double MaxFi => _maxFi;

        private int _countHappy = 0;
        public int CountHappy => _countHappy;

        private int _countSad = 0;
        public int CountSad => _countSad;

        private int _countNeutral = 0;
        public int CountNeutral => _countNeutral;

        public bool AddEdge(String id1, String id2)
        {
            if (id1 == id2)
                throw new Exception("Self Loops Are Not Allowed");

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

            // Remember what the old min and max FI were, if one of these ends up higher/lower, or if 
            // one of these was that val and now isn't, we have to update
            var oldMinFi = MinFi;
            var oldMaxFi = MaxFi;
            bool oldMinFiChanged = false;
            bool oldMaxFiChanged = false;

            // remove the Fis of all affected vertices, add them back in after the attachment
            foreach (var v in affectedVertices)
            {
                if (v.Degree > 0) // The two involved in the edge could be new
                {
                    _sumOfAllFis -= v.Fi;
                    if (Math.Abs(v.Fi - oldMinFi) < TOLERANCE)
                        oldMinFiChanged = true;
                    if (Math.Abs(v.Fi - oldMaxFi) < TOLERANCE)
                        oldMaxFiChanged = true;
                    if (v.IsHappy)
                        _countHappy--;
                    else if (v.IsSad)
                        _countSad--;
                    else
                        _countNeutral--;
                }
            }

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
            
            // Find the min/max of these affected vertices, it could be the new min/max
            var minFiOfAffected = double.MaxValue;
            var maxFiOfAffected = double.MinValue;
            
            // add the new FIs into the total, update other stats
            foreach (var v in affectedVertices)
            {
                _sumOfAllFis += v.Fi; // Add fi back in (we know the vertex is at least degree 1 now)
                // Update counts of happy, sad, neutral
                if (v.IsHappy)
                    _countHappy++;
                else if (v.IsSad)
                    _countSad++;
                else
                    _countNeutral++;
                // note the min/max of the affected
                minFiOfAffected = Math.Min(minFiOfAffected, v.Fi);
                maxFiOfAffected = Math.Max(maxFiOfAffected, v.Fi);
            }

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

            if (minFiOfAffected <= oldMinFi) // new min
                _minFi = minFiOfAffected;
            // if one of these used to be the min and now isn't, we have to scan the whole
            // collection to find the new min. This is O(n) but shouldn't happen very often
            else if (oldMinFiChanged) 
                _minFi = Vertices.Min(v => v.Fi);

            // Update max
            if (maxFiOfAffected >= oldMaxFi)
                _maxFi = maxFiOfAffected;
            else if (oldMaxFiChanged)
                _maxFi = Vertices.Max(v => v.Fi);

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

            // Remember what the old min and max FI were, if one of these ends up higher/lower, or if 
            // one of these was that val and now isn't, we have to update
            var oldMinFi = MinFi;
            var oldMaxFi = MaxFi;
            bool oldMinFiChanged = false;
            bool oldMaxFiChanged = false;

            // remove the Fis of all affected vertices, add them back in after the detachment
            foreach (var v in affectedVertices)
            {
                _sumOfAllFis -= v.Fi;
                if (Math.Abs(v.Fi - oldMinFi) < TOLERANCE)
                    oldMinFiChanged = true;
                if (Math.Abs(v.Fi - oldMaxFi) < TOLERANCE)
                    oldMaxFiChanged = true;
                if (v.IsHappy)
                    _countHappy--;
                else if (v.IsSad)
                    _countSad--;
                else
                    _countNeutral--;
            }

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

            // Find the min/max of these affected vertices, it could be the new min/max
            var minFiOfAffected = double.MaxValue;
            var maxFiOfAffected = double.MinValue;

            // add the new FIs into the total, update other stats
            foreach (var v in affectedVertices)
            {
                if (v.Degree > 0) // removing the edge may have made a vertex degree 0
                {
                    _sumOfAllFis += v.Fi; // Add fi back in 
                    // Update counts of happy, sad, neutral
                    if (v.IsHappy)
                        _countHappy++;
                    else if (v.IsSad)
                        _countSad++;
                    else
                        _countNeutral++;
                    // note the min/max of the affected
                    minFiOfAffected = Math.Min(minFiOfAffected, v.Fi);
                    maxFiOfAffected = Math.Max(maxFiOfAffected, v.Fi);
                }
            }

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

            if (countOfVertices == 0) // if this was the last edge in teh graph
            {
                _minFi = double.NaN;
                _maxFi = double.NaN;
            }
            else
            {
                if (minFiOfAffected <= oldMinFi) // new min
                    _minFi = minFiOfAffected;
                // if one of these used to be the min and now isn't, we have to scan the whole
                // collection to find the new min. This is O(n) but shouldn't happen very often
                else if (oldMinFiChanged)
                    _minFi = Vertices.Min(v => v.Fi);

                // Update max
                if (maxFiOfAffected >= oldMaxFi)
                    _maxFi = maxFiOfAffected;
                else if (oldMaxFiChanged)
                    _maxFi = Vertices.Max(v => v.Fi);
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

            var affectedVertices = new List<Vertex> {v1, v2, v1CurrNeighbor, v2CurrNeighbor};
            double oldMinFi = MinFi;
            double oldMaxFi = MaxFi;
            bool oldMinFiChanged = false;
            bool oldMaxFiChanged = false;
            foreach (var v in affectedVertices)
            {
                // Remove the FI from the total
                _sumOfAllFis -= v.Fi;
                // check if it is the current min or max
                if (Math.Abs(v.Fi - oldMinFi) < TOLERANCE)
                    oldMinFiChanged = true;
                if (Math.Abs(v.Fi - oldMaxFi) < TOLERANCE)
                    oldMaxFiChanged = true;
                // Remove it from the count of happy/sad/neutral
                if (v.IsHappy)
                    _countHappy--;
                else if (v.IsSad)
                    _countSad--;
                else
                    _countNeutral--;
            }

            // Update the sums of neighbors, remove old neighbor and add new
            v1._sumOfNeighborsDegrees += v1NewNeighbor.Degree - v1CurrNeighbor.Degree;
            v2._sumOfNeighborsDegrees += v2NewNeighbor.Degree - v2CurrNeighbor.Degree;
            v1NewNeighbor._sumOfNeighborsDegrees += v1.Degree - v2.Degree;
            v2NewNeighbor._sumOfNeighborsDegrees += v2.Degree - v1.Degree;

            // Find the min/max of these affected vertices, it could be the new min/max
            var minFiOfAffected = double.MaxValue;
            var maxFiOfAffected = double.MinValue;

            // add the new FIs into the total, update other stats
            foreach (var v in affectedVertices)
            {
                _sumOfAllFis += v.Fi; // Add fi back in (we know the vertex is at least degree 1 now)
                // Update counts of happy, sad, neutral
                if (v.IsHappy)
                    _countHappy++;
                else if (v.IsSad)
                    _countSad++;
                else
                    _countNeutral++;
                // note the min/max of the affected
                minFiOfAffected = Math.Min(minFiOfAffected, v.Fi);
                maxFiOfAffected = Math.Max(maxFiOfAffected, v.Fi);
            }

            if (minFiOfAffected <= oldMinFi) // new min
                _minFi = minFiOfAffected;
            // if one of these used to be the min and now isn't, we have to scan the whole
            // collection to find the new min. This is O(n) but shouldn't happen very often
            else if (oldMinFiChanged)
                _minFi = Vertices.Min(v => v.Fi);

            // Update max
            if (maxFiOfAffected >= oldMaxFi)
                _maxFi = maxFiOfAffected;
            else if (oldMaxFiChanged)
                _maxFi = Vertices.Max(v => v.Fi);

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

        #region GRAPH_GENERATING_METHODS

        public static Graph Clone(Graph g)
        {
            return Graph.NewGraphFromEdgeCollection(g.Edges.Select(e => new Tuple<string, string>(e.V1.Id, e.V2.Id)));
        }

        public Graph Clone() => Clone(this);

        // The adding/removing/swapping edges was written to be optimal for swapping, but if we create the
        // graph edge by edge it's actually less efficient than creating it all at once, setting up the 
        // stats, and then allowing changes to update one at a time
        public static Graph NewGraphFromEdgeCollection(IEnumerable<Tuple<string, string>> edgeCollection)
        {
            Graph graph = new Graph();

            foreach (var edgeTuple in edgeCollection)
            {
                var id1 = edgeTuple.Item1;
                var id2 = edgeTuple.Item2;

                if (id1 == id2)
                    throw new Exception("Self Loops Are Not Allowed");

                GraphVertex v1, v2;
                if (!graph._allVertices.ContainsKey(id1))
                {
                    v1 = graph._allVertices[id1] = new GraphVertex(id1);
                    graph._verticesWithDegree.Add(v1);
                    graph.countOfVertices++;
                }
                else
                    v1 = graph._allVertices[id1];
                if (!graph._allVertices.ContainsKey(id2))
                {
                    v2 = graph._allVertices[id2] = new GraphVertex(id2);
                    graph._verticesWithDegree.Add(v2);
                    graph.countOfVertices++;
                }
                else
                    v2 = graph._allVertices[id2];
                
                if (graph._edges.ContainsKey(Edge.GetIdForEdge(v1, v2)))
                    continue;

                v1._neighbors.Add(v2);
                v2._neighbors.Add(v1);

                v1._degree++;
                v2._degree++;

                Edge e = new Edge(v1, v2);
                graph._edges[Edge.GetIdForEdge(v1, v2)] = e;
                graph._numberOfEdges++;
            }
            // now that the degrees have already been calculated set the sum of neighbors for all vertices
            graph._allVertices.Values.ToList().ForEach(v => v._sumOfNeighborsDegrees = v.Neighbors.Sum(n => n.Degree));
            graph._sumOfAllFis = graph._allVertices.Values.Sum(v => v.Fi);

            // now set the assortativity
            foreach (var edge in graph._edges.Values)
            {
                var d1 = edge.V1.Degree;
                var d2 = edge.V2.Degree;

                graph.sumOfProductsOfAllEdgeEndPoints += d1*d2;
                graph.sumOfSquaresOfAllEdgeEndPoints += (int)(Math.Pow(d1, 2) + Math.Pow(d2, 2));
                graph.sumOfAllEdgeEndPoints += d1 + d2;
            }

            graph._minFi = double.MaxValue;
            graph._maxFi = double.MinValue;
            // Now set all other stats
            foreach (var v in graph.Vertices)
            {
                graph._minFi = Math.Min(graph.MinFi, v.Fi);
                graph._maxFi = Math.Max(graph.MaxFi, v.Fi);
                if (v.IsHappy)
                    graph._countHappy++;
                if (v.IsSad)
                    graph._countSad++;
                if (v.IsNeutral)
                    graph._countNeutral++;
            }
            return graph;
        }

        // just to allow another format, will just wrap around the other method, it's just as efficient as it would be
        // to add the edges from the lists
        public static Graph NewGraphFromAdjacencyLists(IEnumerable<Tuple<string, IEnumerable<string>>> adjacencyLists)
        {
            var edgeCollection = new HashSet<Tuple<string, string>>();
            foreach (var adjacencyList in adjacencyLists)
            {
                foreach (var neighborId in adjacencyList.Item2)
                {
                    var id1 = adjacencyList.Item1.CompareTo(neighborId) < 0 ? adjacencyList.Item1 : neighborId;
                    var id2 = id1 == neighborId ? adjacencyList.Item1 : neighborId;

                    edgeCollection.Add(new Tuple<string, string>(id1, id2));
                }
            }
            return NewGraphFromEdgeCollection(edgeCollection);
        }

        public static Graph NewErdosRenyiGraph(int n, double p)
        {
            // Should run through this a few times to make sure it's basically working
            throw new Exception("Test this code");

            // NOTE: Orphan vertices are simply not added to the graph, this is fine because they 
            // are not relevant to any FI calculations and they can't participate in edge swaps, 
            // there's really no way for them to play a role
            RandomGenerator rand = RandomGenerator.NextRandomGenerator();
            Graph graph = new Graph();

            for (int i = 1; i <= n; i++)
                for (int j = i + 1; j <= n; j++)
                    if (rand.GetTrueWithProbability(p))
                        graph.AddEdge(i.ToString(), j.ToString());

            return graph;
        }

        public static Graph NewBarabasiAlbertGraph(int n, int m)
        {
            // have to run through this code a few times and make sure it actually works
            throw new Exception("Test this code before using");

            RandomGenerator rand = RandomGenerator.NextRandomGenerator();

            Graph graph = new Graph();
            
            // Start with one vertex attached to m other vertices
            for (int i = 2; i <= m + 1; i++)
                graph.AddEdge(1.ToString(), i.ToString());

            for (int i = m + 2; i <= n; i++)
            {
                int sumOfAllDegrees = graph.NumberOfEdges*2;
                var vertices = new HashSet<Vertex>(graph.Vertices);

                for (int j = 0; j < m; j++)
                {
                    int curr = 0;
                    double selectedVal = rand.NextDouble()*sumOfAllDegrees;
                   
                    
                    foreach (var v in vertices)
                    {
                        if (curr <= selectedVal && curr + v.Degree > selectedVal)
                        {
                            graph.AddEdge(i.ToString(), v.Id);
                            vertices.Remove(v);
                            sumOfAllDegrees -= v.Degree;
                            break;
                        }
                        curr += v.Degree;
                    }
                }
            }

            return graph;
        }

        // Uses the Erdos Gallai algorithm to establish whether or not a sequence is graphic 
        public static bool SequenceIsGraphic(IEnumerable<int> degreeSequence)
        {
            // taken straight from previous code without any modification
            // so presumably this doesn't need to be tested?
            List<int> degrees = degreeSequence.OrderByDescending(i => i).ToList();

            if (degrees.Sum(i => i) % 2 != 0)
                return false;

            for (int k = 1; k <= degrees.Count; k++)
            {
                var leftSum = 0;
                for (int i = 1; i <= k; i++)
                    leftSum += degrees[i - 1];
                var rightSum = k * (k - 1);
                for (int i = k + 1; i <= degrees.Count; i++)
                    rightSum += Math.Min(k, degrees[i - 1]);
                if (leftSum > rightSum)
                    return false;
            }
            return true;
        }

        public static Graph NewDHavelHakimiGraph(IEnumerable<int> DegreeSequence)
        {
            throw new Exception("Not yet written");
            // Can try to look at the previous code, I think it has the algorithm written
            // out correctly. Point just is to use a stable sort to sort the vertices so
            // that any vertex that has a higher degree than another will come first in the
            // list if they have the same remaining degree. This doesn't provide optimal
            // low AFI, but we have to see if it's a nice head start
        }

        public static Graph NewRandomBlitzsteinDiaconisGraph(IEnumerable<int> DegreeSequence)
        {
            // Taken from previous code but should be run through a few times to verify
            // that it's working
            throw new Exception("Test this code before using");

            var sequence = DegreeSequence.OrderByDescending(i => i).ToList();
            if (!SequenceIsGraphic(sequence))
                return null; // Throw Exception?

            HashSet<Tuple<int, int>> edges = new HashSet<Tuple<int, int>>();

            var rand = RandomGenerator.NextRandomGenerator();

            var totalEdgesToAdd = sequence.Sum()/2;

            while (totalEdgesToAdd > 0)
            {
                int currMinValue = sequence.Where(i => i > 0).Min();
                int chosenIndex = 0;
                int tmpIndx;
                while (sequence[chosenIndex] != currMinValue)
                    chosenIndex++;

                // experimenting with a chosen index that isn't necessarily a min
                tmpIndx = 0;
                var nonZeros =
                    sequence.Select(i => new {location = tmpIndx++, value = i}).Where(v => v.value > 0).ToList();
                chosenIndex = nonZeros[rand.NextInt(0, nonZeros.Count)].location;

                tmpIndx = 0;
                var possibleNeighbors =
                    sequence.Select(i => new {location = tmpIndx++, value = i})
                        .Where(
                            v =>
                                v.value > 0 && v.location != chosenIndex &&
                                !edges.Any(
                                    e =>
                                        e.Item1 == Math.Min(chosenIndex, v.location) &&
                                        e.Item2 == Math.Max(chosenIndex, v.location)))
                        .ToList();

                while (sequence[chosenIndex] > 0)
                {
                    var nextNeighborIndex = rand.NextInt(0, possibleNeighbors.Count);
                    var nextNeighbor = possibleNeighbors[nextNeighborIndex];

                    // check if the resulting sequence would be graphic
                    sequence[chosenIndex]--;
                    sequence[nextNeighbor.location]--;
                    if (!SequenceIsGraphic(sequence))
                    {
                        // put back the degrees we removed
                        sequence[chosenIndex]++;
                        sequence[nextNeighbor.location]++;
                    }
                    else // add an edge
                    {
                        edges.Add(new Tuple<int, int>(Math.Min(chosenIndex, nextNeighbor.location),
                            Math.Max(chosenIndex, nextNeighbor.location)));
                        totalEdgesToAdd--;
                    }
                    possibleNeighbors.RemoveAt(nextNeighborIndex);
                }
            }

            Graph g = new Graph();
            edges.ToList().ForEach(e => g.AddEdge((e.Item1 + 1).ToString(), (e.Item2 + 1).ToString()));
            return g;
        }



        #endregion
        
        
        // Things to add
        // ER, BA, HH (DHH), BD_Random *** ADDED BUT TEST ***
        // Should probably add old IsIsomorphic code just for some sanity testing here and there..
        // A searching class that uses this class and searches...

    }
}
