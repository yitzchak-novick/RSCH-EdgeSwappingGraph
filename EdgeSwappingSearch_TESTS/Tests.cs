using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EdgeSwappingSearch;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static EdgeSwappingSearch.Utils;

namespace EdgeSwappingSearch_TESTS
{
    [TestClass]
    public class Tests
    {
        private class TestGraph : Graph // Expose any private variables that need to be tested
        {

        }

        //private const double TOLERANCE = 0.00000001;

        // Test Graphs - These are to test basic functionality, NOT orgranic growing graphs
        private Graph TwoOrphanVertices;
        private Graph SingleEdgeBetweenTwoVertices;
        private Graph SingleEdgeBetweenTwoVerticesPlusOneOrphan; // "1", "2", "3" is the orphan
        private Graph Triangle;
        private Graph StarWithFiveLeafNodes; // 1 - 6, 1 is the center
        private Graph TrianglePlusOneLeaf; // 1, 2, 3 triangle, 4 is leaf connected to 3
        private Graph LineGraphWithFourVertices; // 1 <--> 2 <--> 3 <--> 4
        private Graph CompleteGraphWithFourVertices;
        private Graph BowTie; // Triangles 1, 2, 3 and 4, 5, 6, with 4 and 3 also connected by an edge

        private Graph LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices;
        // 1 <--> 2 <--> 3 <--> 4, 5,6,7,8,9

        #region TEST_INITIALIZATION
        [TestInitialize]
        public void TestInitialize()
        {
            TwoOrphanVertices = new Graph();
            TwoOrphanVertices.AddEdge("1", "2");
            TwoOrphanVertices.RemoveEdge("1", "2");

            SingleEdgeBetweenTwoVertices = new Graph();
            SingleEdgeBetweenTwoVertices.AddEdge("1", "2");

            SingleEdgeBetweenTwoVerticesPlusOneOrphan = new Graph();
            SingleEdgeBetweenTwoVerticesPlusOneOrphan.AddEdge("1", "2");
            SingleEdgeBetweenTwoVerticesPlusOneOrphan.AddEdge("2", "3");
            SingleEdgeBetweenTwoVerticesPlusOneOrphan.RemoveEdge("2", "3");

            Triangle = new Graph();
            Triangle.AddEdge("1", "2");
            Triangle.AddEdge("2", "3");
            Triangle.AddEdge("1", "3");

            StarWithFiveLeafNodes = new Graph();
            StarWithFiveLeafNodes.AddEdge("1", "2");
            StarWithFiveLeafNodes.AddEdge("1", "3");
            StarWithFiveLeafNodes.AddEdge("1", "4");
            StarWithFiveLeafNodes.AddEdge("1", "5");
            StarWithFiveLeafNodes.AddEdge("1", "6");

            TrianglePlusOneLeaf = new Graph();
            TrianglePlusOneLeaf.AddEdge("1", "2");
            TrianglePlusOneLeaf.AddEdge("2", "3");
            TrianglePlusOneLeaf.AddEdge("3", "1");
            TrianglePlusOneLeaf.AddEdge("4", "3");

            LineGraphWithFourVertices = new Graph();
            LineGraphWithFourVertices.AddEdge("1", "2");
            LineGraphWithFourVertices.AddEdge("2", "3");
            LineGraphWithFourVertices.AddEdge("3", "4");

            CompleteGraphWithFourVertices = new Graph();
            CompleteGraphWithFourVertices.AddEdge("1", "2");
            CompleteGraphWithFourVertices.AddEdge("1", "3");
            CompleteGraphWithFourVertices.AddEdge("1", "4");
            CompleteGraphWithFourVertices.AddEdge("2", "3");
            CompleteGraphWithFourVertices.AddEdge("2", "4");
            CompleteGraphWithFourVertices.AddEdge("3", "4");

            //private Graph LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices; // 1 <--> 2 <--> 3 <--> 4, 5,6,7,8,9
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices = new Graph();
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.AddEdge("1", "2");
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.AddEdge("2", "3");
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.AddEdge("3", "4");
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.AddEdge("5", "6");
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.AddEdge("5", "7");
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.AddEdge("5", "8");
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.AddEdge("5", "9");
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.AddEdge("6", "7");
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.AddEdge("6", "8");
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.AddEdge("6", "9");
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.AddEdge("7", "8");
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.AddEdge("7", "9");
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.AddEdge("8", "9");

            BowTie = new Graph();
            BowTie.AddEdge("1", "2");
            BowTie.AddEdge("2", "3");
            BowTie.AddEdge("1", "3");
            BowTie.AddEdge("4", "5");
            BowTie.AddEdge("5", "6");
            BowTie.AddEdge("6", "4");
            BowTie.AddEdge("3", "4");

        }

        // For methods that test graphs created from edge collection instead of adding each
        // edge manually, use this method to recreate all of the test graphs that way
        // NOTE that it does not recreate graphs with orphan vertices..
        private void RecreateTestGraphsFromEdgeCollections()
        {
            SingleEdgeBetweenTwoVertices = Graph.NewGraphFromEdgeCollection(new List<Tuple<string, string>>
            {
                new Tuple<string, string>("1", "2")
            });

            Triangle = Graph.NewGraphFromEdgeCollection(new List<Tuple<string, string>>
            {
                new Tuple<string, string>("1", "2"),
                new Tuple<string, string>("2", "3"),
                new Tuple<string, string>("3", "1")
            });

            StarWithFiveLeafNodes = Graph.NewGraphFromEdgeCollection(new List<Tuple<string, string>>
            {
                new Tuple<string, string>("1", "2"),
                new Tuple<string, string>("1", "3"),
                new Tuple<string, string>("4", "1"),
                new Tuple<string, string>("5", "1"),
                new Tuple<string, string>("1", "6")
            });

            TrianglePlusOneLeaf = Graph.NewGraphFromEdgeCollection(new List<Tuple<string, string>>
            {
                new Tuple<string, string>("1", "2"),
                new Tuple<string, string>("2", "3"),
                new Tuple<string, string>("1", "3"),
                new Tuple<string, string>("4", "3")
            });

            LineGraphWithFourVertices = Graph.NewGraphFromEdgeCollection(new List<Tuple<string, string>>
            {
                new Tuple<string, string>("1", "2"),
                new Tuple<string, string>("3", "2"),
                new Tuple<string, string>("4", "3")
            });

            CompleteGraphWithFourVertices = Graph.NewGraphFromEdgeCollection(new List<Tuple<string, string>>
            {
                new Tuple<string, string>("1", "2"),
                new Tuple<string, string>("3", "2"),
                new Tuple<string, string>("4", "3"),
                new Tuple<string, string>("4", "1"),
                new Tuple<string, string>("2", "1"), // test duplicates while we're at it
                new Tuple<string, string>("1", "3"),
                new Tuple<string, string>("4", "2")

            });
            
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices = Graph.NewGraphFromEdgeCollection(new List<Tuple<string, string>>
            {
                new Tuple<string, string>("1", "2"),
                new Tuple<string, string>("3", "2"),
                new Tuple<string, string>("4", "3"),
                new Tuple<string, string>("5", "6"),
                new Tuple<string, string>("5", "7"),
                new Tuple<string, string>("5", "8"),
                new Tuple<string, string>("5", "9"),
                new Tuple<string, string>("6", "7"),
                new Tuple<string, string>("6", "8"),
                new Tuple<string, string>("6", "9"),
                new Tuple<string, string>("7", "8"),
                new Tuple<string, string>("7", "9"),
                new Tuple<string, string>("8", "9")
            });

            BowTie = Graph.NewGraphFromEdgeCollection(new List<Tuple<string, string>>
            {
                new Tuple<string, string>("1", "2"),
                new Tuple<string, string>("3", "2"),
                new Tuple<string, string>("1", "2"),
                new Tuple<string, string>("3", "4"),
                new Tuple<string, string>("5", "4"),
                new Tuple<string, string>("5", "6"),
                new Tuple<string, string>("4", "6")
            });
        }

        // For methods that test graphs created from edge collection instead of adding each
        // edge manually, use this method to recreate all of the test graphs that way
        // NOTE that it does not recreate graphs with orphan vertices..
        private void RecreateTestGraphsFromAdjacencyLists()
        {
            SingleEdgeBetweenTwoVertices = Graph.NewGraphFromAdjacencyLists(new List<Tuple<string, IEnumerable<string>>>
            {
                new Tuple<string, IEnumerable<string>>("1", new List<String> {"2"}),
                new Tuple<string, IEnumerable<string>>("2", new List<String> {"1"}),
            });

            Triangle = Graph.NewGraphFromAdjacencyLists(new List<Tuple<string, IEnumerable<string>>>
            {
                new Tuple<string, IEnumerable<string>>("1", new List<String> {"2", "3"}),
                new Tuple<string, IEnumerable<string>>("2", new List<String> {"1", "3"}),
                new Tuple<string, IEnumerable<string>>("3", new List<String> {"1", "2"})
            });

            StarWithFiveLeafNodes = Graph.NewGraphFromAdjacencyLists(new List<Tuple<string, IEnumerable<string>>>
            {
                new Tuple<string, IEnumerable<string>>("1", new List<String> {"2", "3", "4", "5", "6"}),
                new Tuple<string, IEnumerable<string>>("2", new List<String> {"1"}),
                new Tuple<string, IEnumerable<string>>("3", new List<String> {"1"}),
                new Tuple<string, IEnumerable<string>>("4", new List<String> {"1"}),
                new Tuple<string, IEnumerable<string>>("5", new List<String> {"1"}),
                new Tuple<string, IEnumerable<string>>("6", new List<String> {"1"})
            });

            TrianglePlusOneLeaf = Graph.NewGraphFromAdjacencyLists(new List<Tuple<string, IEnumerable<string>>>
            {
                new Tuple<string, IEnumerable<string>>("1", new List<String> {"2", "3"}),
                new Tuple<string, IEnumerable<string>>("2", new List<String> {"1", "3"}),
                new Tuple<string, IEnumerable<string>>("3", new List<String> {"1", "2", "4"}),
                new Tuple<string, IEnumerable<string>>("4", new List<String> {"3"})
            });

            LineGraphWithFourVertices = Graph.NewGraphFromAdjacencyLists(new List<Tuple<string, IEnumerable<string>>>
            {
                new Tuple<string, IEnumerable<string>>("1", new List<String> {"2"}),
                new Tuple<string, IEnumerable<string>>("2", new List<String> {"3", "1"}),
                new Tuple<string, IEnumerable<string>>("3", new List<String> {"2", "4"}),
                new Tuple<string, IEnumerable<string>>("4", new List<String> {"3"})
            });

            CompleteGraphWithFourVertices =
                Graph.NewGraphFromAdjacencyLists(new List<Tuple<string, IEnumerable<string>>>
                {
                    new Tuple<string, IEnumerable<string>>("1", new List<String> {"2", "3", "4"}),
                    new Tuple<string, IEnumerable<string>>("2", new List<String> {"1", "3", "4"}),
                    new Tuple<string, IEnumerable<string>>("3", new List<String> {"1", "2", "4"}),
                    new Tuple<string, IEnumerable<string>>("4", new List<String> {"1", "2", "3"})

                });

            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices = Graph.NewGraphFromAdjacencyLists(new List<Tuple<string, IEnumerable<string>>>
            {
                new Tuple<string, IEnumerable<string>>("1", new List<String> {"2"}),
                new Tuple<string, IEnumerable<string>>("2", new List<String> {"1", "3"}),
                new Tuple<string, IEnumerable<string>>("3", new List<String> {"2", "4"}),
                new Tuple<string, IEnumerable<string>>("4", new List<String> {"3"}),
                new Tuple<string, IEnumerable<string>>("5", new List<String> {"6", "7", "8", "9"}),
                new Tuple<string, IEnumerable<string>>("6", new List<String> {"5", "7", "8", "9"}),
                new Tuple<string, IEnumerable<string>>("7", new List<String> {"6", "5", "8", "9"}),
                new Tuple<string, IEnumerable<string>>("8", new List<String> {"6", "7", "5", "9"}),
                new Tuple<string, IEnumerable<string>>("9", new List<String> {"6", "7", "8", "5"})
            });

            BowTie = Graph.NewGraphFromAdjacencyLists(new List<Tuple<string, IEnumerable<string>>>
            {
                new Tuple<string, IEnumerable<string>>("1", new List<String> {"2", "3"}),
                new Tuple<string, IEnumerable<string>>("2", new List<String> {"1", "3"}),
                new Tuple<string, IEnumerable<string>>("3", new List<String> {"1", "2", "4"}),
                new Tuple<string, IEnumerable<string>>("4", new List<String> {"3", "5", "6"}),
                new Tuple<string, IEnumerable<string>>("5", new List<String> {"6", "4"}),
                new Tuple<string, IEnumerable<string>>("6", new List<String> {"5", "4"})
            });

        }
        #endregion

        #region VERTEX_PROPERTY_TESTS
        [TestMethod]
        public void TestDegreePropertyInTestGraphsIsCorrect()
        {
            Assert.AreEqual(1, SingleEdgeBetweenTwoVertices["1"].Degree);
            Assert.AreEqual(1, SingleEdgeBetweenTwoVertices["2"].Degree);

            Assert.AreEqual(1, SingleEdgeBetweenTwoVerticesPlusOneOrphan["1"].Degree);
            Assert.AreEqual(1, SingleEdgeBetweenTwoVerticesPlusOneOrphan["2"].Degree);

            Assert.IsTrue(Triangle.Vertices.All(v => v.Degree == 2));

            Assert.IsTrue(StarWithFiveLeafNodes.Vertices.All(v => v.Degree == 5 || v.Degree == 1));
            Assert.AreEqual(5, StarWithFiveLeafNodes["1"].Degree);

            Assert.AreEqual(2, TrianglePlusOneLeaf["1"].Degree);
            Assert.AreEqual(2, TrianglePlusOneLeaf["2"].Degree);
            Assert.AreEqual(3, TrianglePlusOneLeaf["3"].Degree);
            Assert.AreEqual(1, TrianglePlusOneLeaf["4"].Degree);

            Assert.AreEqual(1, LineGraphWithFourVertices["1"].Degree);
            Assert.AreEqual(2, LineGraphWithFourVertices["2"].Degree);
            Assert.AreEqual(2, LineGraphWithFourVertices["3"].Degree);
            Assert.AreEqual(1, LineGraphWithFourVertices["4"].Degree);

            Assert.IsTrue(CompleteGraphWithFourVertices.Vertices.All(v => v.Degree == 3));

            Assert.AreEqual(1, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices["1"].Degree);
            Assert.AreEqual(2, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices["2"].Degree);
            Assert.AreEqual(2, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices["3"].Degree);
            Assert.AreEqual(1, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices["4"].Degree);
            Assert.IsTrue( // vertices "5" - "9" are all of degree 4...
                LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.Vertices.Where(
                    v => Int16.Parse(v.Id) >= 5 && Int16.Parse(v.Id) <= 9).All(v => v.Degree == 4));
        }

        [TestMethod]
        public void TestSumOfNeighborsDegreePropertyInTestGraphsIsCorrect()
        {
            Assert.AreEqual(1, SingleEdgeBetweenTwoVertices["1"].SumOfNeighborsDegrees);
            Assert.AreEqual(1, SingleEdgeBetweenTwoVertices["2"].SumOfNeighborsDegrees);

            Assert.AreEqual(1, SingleEdgeBetweenTwoVerticesPlusOneOrphan["1"].SumOfNeighborsDegrees);
            Assert.AreEqual(1, SingleEdgeBetweenTwoVerticesPlusOneOrphan["2"].SumOfNeighborsDegrees);

            Assert.IsTrue(Triangle.Vertices.All(v => v.SumOfNeighborsDegrees == 4));

            // Center has 5 neighbors, 1 each, leaves have 1 neighbor of degree 5
            Assert.IsTrue(StarWithFiveLeafNodes.Vertices.All(v => v.SumOfNeighborsDegrees == 5));

            Assert.AreEqual(5, TrianglePlusOneLeaf["1"].SumOfNeighborsDegrees);
            Assert.AreEqual(5, TrianglePlusOneLeaf["2"].SumOfNeighborsDegrees);
            Assert.AreEqual(5, TrianglePlusOneLeaf["3"].SumOfNeighborsDegrees);
            Assert.AreEqual(3, TrianglePlusOneLeaf["4"].SumOfNeighborsDegrees);

            Assert.AreEqual(2, LineGraphWithFourVertices["1"].SumOfNeighborsDegrees);
            Assert.AreEqual(3, LineGraphWithFourVertices["2"].SumOfNeighborsDegrees);
            Assert.AreEqual(3, LineGraphWithFourVertices["3"].SumOfNeighborsDegrees);
            Assert.AreEqual(2, LineGraphWithFourVertices["4"].SumOfNeighborsDegrees);

            Assert.IsTrue(CompleteGraphWithFourVertices.Vertices.All(v => v.SumOfNeighborsDegrees == 9));

            Assert.AreEqual(2, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices["1"].SumOfNeighborsDegrees);
            Assert.AreEqual(3, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices["2"].SumOfNeighborsDegrees);
            Assert.AreEqual(3, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices["3"].SumOfNeighborsDegrees);
            Assert.AreEqual(2, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices["4"].SumOfNeighborsDegrees);
            Assert.IsTrue( // vertices "5" - "9" are have 4 neighbors of degree 4
                LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.Vertices.Where(
                    v => Int16.Parse(v.Id) >= 5 && Int16.Parse(v.Id) <= 9).All(v => v.SumOfNeighborsDegrees == 16));
        }

        [TestMethod]
        public void TestFiPropertyInTestGraphsIsCorrect()
        {
            Assert.AreEqual(1.0, SingleEdgeBetweenTwoVertices["1"].Fi, TOLERANCE);
            Assert.AreEqual(1.0, SingleEdgeBetweenTwoVertices["2"].Fi, TOLERANCE);

            Assert.AreEqual(1.0, SingleEdgeBetweenTwoVerticesPlusOneOrphan["1"].Fi, TOLERANCE);
            Assert.AreEqual(1.0, SingleEdgeBetweenTwoVerticesPlusOneOrphan["2"].Fi, TOLERANCE);

            Assert.IsTrue(Triangle.Vertices.All(v => Math.Abs(v.Fi - 1.0) < TOLERANCE));

            Assert.AreEqual((1.0/5.0), StarWithFiveLeafNodes["1"].Fi, TOLERANCE);
            Assert.IsTrue(
                StarWithFiveLeafNodes.Vertices.Where(v => v.Id != "1").All(v => Math.Abs(v.Fi - 5.0) < TOLERANCE));

            Assert.AreEqual(1.25, TrianglePlusOneLeaf["1"].Fi, TOLERANCE);
            Assert.AreEqual(1.25, TrianglePlusOneLeaf["2"].Fi, TOLERANCE);
            Assert.AreEqual(5.0/9.0, TrianglePlusOneLeaf["3"].Fi, TOLERANCE);
            Assert.AreEqual(3.0, TrianglePlusOneLeaf["4"].Fi, TOLERANCE);

            Assert.AreEqual(2.0, LineGraphWithFourVertices["1"].Fi, TOLERANCE);
            Assert.AreEqual(0.75, LineGraphWithFourVertices["2"].Fi, TOLERANCE);
            Assert.AreEqual(0.75, LineGraphWithFourVertices["3"].Fi, TOLERANCE);
            Assert.AreEqual(2.0, LineGraphWithFourVertices["4"].Fi, TOLERANCE);

            Assert.IsTrue(CompleteGraphWithFourVertices.Vertices.All(v => Math.Abs(v.Fi - 1.0) < TOLERANCE));

            Assert.AreEqual(2.0, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices["1"].Fi, TOLERANCE);
            Assert.AreEqual(0.75, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices["2"].Fi, TOLERANCE);
            Assert.AreEqual(0.75, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices["3"].Fi, TOLERANCE);
            Assert.AreEqual(2.0, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices["4"].Fi, TOLERANCE);
            Assert.IsTrue( // vertices "5" - "9" are have 4 neighbors of degree 4
                LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.Vertices.Where(
                    v => Int16.Parse(v.Id) >= 5 && Int16.Parse(v.Id) <= 9).All(v => Math.Abs(v.Fi - 1.0) < TOLERANCE));
        }

        [TestMethod]
        public void TestIsHappyPropertyInTestGraphsIsCorrect()
        {
            Assert.IsFalse(SingleEdgeBetweenTwoVertices["1"].IsHappy);
            Assert.IsFalse(SingleEdgeBetweenTwoVertices["2"].IsHappy);

            Assert.IsFalse(SingleEdgeBetweenTwoVerticesPlusOneOrphan["1"].IsHappy);
            Assert.IsFalse(SingleEdgeBetweenTwoVerticesPlusOneOrphan["2"].IsHappy);
            ;

            Assert.IsTrue(Triangle.Vertices.All(v => !v.IsHappy));

            Assert.IsTrue(StarWithFiveLeafNodes["1"].IsHappy);
            Assert.IsTrue(StarWithFiveLeafNodes.Vertices.Where(v => v.Id != "1").All(v => !v.IsHappy));

            Assert.IsFalse(TrianglePlusOneLeaf["1"].IsHappy);
            Assert.IsFalse(TrianglePlusOneLeaf["2"].IsHappy);
            Assert.IsTrue(TrianglePlusOneLeaf["3"].IsHappy);
            Assert.IsFalse(TrianglePlusOneLeaf["4"].IsHappy);

            Assert.IsFalse(LineGraphWithFourVertices["1"].IsHappy);
            Assert.IsTrue(LineGraphWithFourVertices["2"].IsHappy);
            Assert.IsTrue(LineGraphWithFourVertices["3"].IsHappy);
            Assert.IsFalse(LineGraphWithFourVertices["4"].IsHappy);

            Assert.IsTrue(CompleteGraphWithFourVertices.Vertices.All(v => !v.IsHappy));

            Assert.IsFalse(LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices["1"].IsHappy);
            Assert.IsTrue(LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices["2"].IsHappy);
            Assert.IsTrue(LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices["3"].IsHappy);
            Assert.IsFalse(LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices["4"].IsHappy);
            Assert.IsTrue( // vertices "5" - "9" are have 4 neighbors of degree 4
                LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.Vertices.Where(
                    v => Int16.Parse(v.Id) >= 5 && Int16.Parse(v.Id) <= 9).All(v => !v.IsHappy));
        }

        [TestMethod]
        public void TestIsSadPropertyInTestGraphsIsCorrect()
        {
            Assert.IsFalse(SingleEdgeBetweenTwoVertices["1"].IsSad);
            Assert.IsFalse(SingleEdgeBetweenTwoVertices["2"].IsSad);

            Assert.IsFalse(SingleEdgeBetweenTwoVerticesPlusOneOrphan["1"].IsSad);
            Assert.IsFalse(SingleEdgeBetweenTwoVerticesPlusOneOrphan["2"].IsSad);
            ;

            Assert.IsTrue(Triangle.Vertices.All(v => !v.IsSad));

            Assert.IsFalse(StarWithFiveLeafNodes["1"].IsSad);
            Assert.IsTrue(StarWithFiveLeafNodes.Vertices.Where(v => v.Id != "1").All(v => v.IsSad));

            Assert.IsTrue(TrianglePlusOneLeaf["1"].IsSad);
            Assert.IsTrue(TrianglePlusOneLeaf["2"].IsSad);
            Assert.IsFalse(TrianglePlusOneLeaf["3"].IsSad);
            Assert.IsTrue(TrianglePlusOneLeaf["4"].IsSad);

            Assert.IsTrue(LineGraphWithFourVertices["1"].IsSad);
            Assert.IsFalse(LineGraphWithFourVertices["2"].IsSad);
            Assert.IsFalse(LineGraphWithFourVertices["3"].IsSad);
            Assert.IsTrue(LineGraphWithFourVertices["4"].IsSad);

            Assert.IsTrue(CompleteGraphWithFourVertices.Vertices.All(v => !v.IsSad));

            Assert.IsTrue(LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices["1"].IsSad);
            Assert.IsFalse(LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices["2"].IsSad);
            Assert.IsFalse(LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices["3"].IsSad);
            Assert.IsTrue(LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices["4"].IsSad);
            Assert.IsTrue( // vertices "5" - "9" are have 4 neighbors of degree 4
                LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.Vertices.Where(
                    v => Int16.Parse(v.Id) >= 5 && Int16.Parse(v.Id) <= 9).All(v => !v.IsSad));
        }

        [TestMethod]
        public void TestIsNeutralPropertyInTestGraphsIsCorrect()
        {
            Assert.IsTrue(SingleEdgeBetweenTwoVertices["1"].IsNeutral);
            Assert.IsTrue(SingleEdgeBetweenTwoVertices["2"].IsNeutral);

            Assert.IsTrue(SingleEdgeBetweenTwoVerticesPlusOneOrphan["1"].IsNeutral);
            Assert.IsTrue(SingleEdgeBetweenTwoVerticesPlusOneOrphan["2"].IsNeutral);
            ;

            Assert.IsTrue(Triangle.Vertices.All(v => v.IsNeutral));

            Assert.IsTrue(StarWithFiveLeafNodes.Vertices.All(v => !v.IsNeutral));

            Assert.IsFalse(TrianglePlusOneLeaf["1"].IsNeutral);
            Assert.IsFalse(TrianglePlusOneLeaf["2"].IsNeutral);
            Assert.IsFalse(TrianglePlusOneLeaf["3"].IsNeutral);
            Assert.IsFalse(TrianglePlusOneLeaf["4"].IsNeutral);

            Assert.IsFalse(LineGraphWithFourVertices["1"].IsNeutral);
            Assert.IsFalse(LineGraphWithFourVertices["2"].IsNeutral);
            Assert.IsFalse(LineGraphWithFourVertices["3"].IsNeutral);
            Assert.IsFalse(LineGraphWithFourVertices["4"].IsNeutral);

            Assert.IsTrue(CompleteGraphWithFourVertices.Vertices.All(v => v.IsNeutral));

            Assert.IsFalse(LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices["1"].IsNeutral);
            Assert.IsFalse(LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices["2"].IsNeutral);
            Assert.IsFalse(LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices["3"].IsNeutral);
            Assert.IsFalse(LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices["4"].IsNeutral);
            Assert.IsTrue( // vertices "5" - "9" are have 4 neighbors of degree 4
                LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.Vertices.Where(
                    v => Int16.Parse(v.Id) >= 5 && Int16.Parse(v.Id) <= 9).All(v => v.IsNeutral));
        }

        [TestMethod]
        public void TestHasNeighborInTestGraphsGivesTrueForNeighbor()
        {
            Assert.IsTrue(SingleEdgeBetweenTwoVertices["1"].HasNeighbor(SingleEdgeBetweenTwoVertices["2"]));
            Assert.IsTrue(SingleEdgeBetweenTwoVertices["2"].HasNeighbor(SingleEdgeBetweenTwoVertices["1"]));

            Assert.IsTrue(
                SingleEdgeBetweenTwoVerticesPlusOneOrphan["1"].HasNeighbor(
                    SingleEdgeBetweenTwoVerticesPlusOneOrphan["2"]));
            Assert.IsTrue(
                SingleEdgeBetweenTwoVerticesPlusOneOrphan["2"].HasNeighbor(
                    SingleEdgeBetweenTwoVerticesPlusOneOrphan["1"]));

            Assert.IsTrue(
                Triangle.Vertices.All(
                    v1 => Triangle.Vertices.Where(v2 => v2 != v1).All(v2 => v1.HasNeighbor(v2) && v2.HasNeighbor(v1))));

            var centerOfStar = StarWithFiveLeafNodes["1"];
            Assert.IsTrue(
                StarWithFiveLeafNodes.Vertices.Where(v => v != centerOfStar)
                    .All(v => v.HasNeighbor(centerOfStar) && centerOfStar.HasNeighbor(v)));

            Assert.IsTrue(TrianglePlusOneLeaf["1"].HasNeighbor(TrianglePlusOneLeaf["2"]) &&
                          TrianglePlusOneLeaf["2"].HasNeighbor(TrianglePlusOneLeaf["1"]));
            Assert.IsTrue(TrianglePlusOneLeaf["1"].HasNeighbor(TrianglePlusOneLeaf["3"]) &&
                          TrianglePlusOneLeaf["3"].HasNeighbor(TrianglePlusOneLeaf["1"]));
            Assert.IsTrue(TrianglePlusOneLeaf["2"].HasNeighbor(TrianglePlusOneLeaf["3"]) &&
                          TrianglePlusOneLeaf["3"].HasNeighbor(TrianglePlusOneLeaf["2"]));
            Assert.IsTrue(TrianglePlusOneLeaf["3"].HasNeighbor(TrianglePlusOneLeaf["4"]) &&
                          TrianglePlusOneLeaf["4"].HasNeighbor(TrianglePlusOneLeaf["3"]));

            Assert.IsTrue(LineGraphWithFourVertices["1"].HasNeighbor(LineGraphWithFourVertices["2"]) &&
                          LineGraphWithFourVertices["2"].HasNeighbor(LineGraphWithFourVertices["1"]));
            Assert.IsTrue(LineGraphWithFourVertices["2"].HasNeighbor(LineGraphWithFourVertices["3"]) &&
                          LineGraphWithFourVertices["3"].HasNeighbor(LineGraphWithFourVertices["2"]));
            Assert.IsTrue(LineGraphWithFourVertices["3"].HasNeighbor(LineGraphWithFourVertices["4"]) &&
                          LineGraphWithFourVertices["4"].HasNeighbor(LineGraphWithFourVertices["3"]));

            Assert.IsTrue(
                CompleteGraphWithFourVertices.Vertices.All(
                    v1 =>
                        CompleteGraphWithFourVertices.Vertices.Where(v2 => v2 != v1)
                            .All(v2 => v1.HasNeighbor(v2) && v2.HasNeighbor(v1))));

            Assert.IsTrue(
                LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices["1"].HasNeighbor(
                    LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices["2"]) &&
                LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices["2"].HasNeighbor(
                    LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices["1"]));
            Assert.IsTrue(
                LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices["2"].HasNeighbor(
                    LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices["3"]) &&
                LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices["3"].HasNeighbor(
                    LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices["2"]));
            Assert.IsTrue(
                LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices["3"].HasNeighbor(
                    LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices["4"]) &&
                LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices["4"].HasNeighbor(
                    LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices["3"]));

            var verticesFiveThroughNine =
                LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.Vertices.Where(
                    v => Int16.Parse(v.Id) >= 5 && Int16.Parse(v.Id) <= 9).ToList();
            Assert.IsTrue( // vertices "5" - "9" are have 4 neighbors of degree 4
                verticesFiveThroughNine.All(
                    v1 =>
                        verticesFiveThroughNine.Where(v2 => v2 != v1)
                            .All(v2 => v1.HasNeighbor(v2) && v2.HasNeighbor(v1))));
        }

        [TestMethod]
        public void TestHasNeighborInTestGraphsGivesFalseForNonNeighbor()
        {
            var starLeafNodes = StarWithFiveLeafNodes.Vertices.Where(v => v.Id != "1").ToList();
            Assert.IsTrue(starLeafNodes.All(v1 => starLeafNodes.All(v2 => !v1.HasNeighbor(v2) && !v2.HasNeighbor(v1))));

            Assert.IsTrue(!TrianglePlusOneLeaf["4"].HasNeighbor(TrianglePlusOneLeaf["1"]) &&
                          !TrianglePlusOneLeaf["1"].HasNeighbor(TrianglePlusOneLeaf["4"]));
            Assert.IsTrue(!TrianglePlusOneLeaf["4"].HasNeighbor(TrianglePlusOneLeaf["2"]) &&
                          !TrianglePlusOneLeaf["2"].HasNeighbor(TrianglePlusOneLeaf["4"]));

            Assert.IsTrue(!LineGraphWithFourVertices["1"].HasNeighbor(LineGraphWithFourVertices["3"]) &&
                          !LineGraphWithFourVertices["3"].HasNeighbor(LineGraphWithFourVertices["1"]));
            Assert.IsTrue(!LineGraphWithFourVertices["1"].HasNeighbor(LineGraphWithFourVertices["4"]) &&
                          !LineGraphWithFourVertices["4"].HasNeighbor(LineGraphWithFourVertices["1"]));

            Assert.IsTrue(
                !LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices["1"].HasNeighbor(
                    LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices["5"]) &&
                !LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices["5"].HasNeighbor(
                    LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices["1"]));
            Assert.IsTrue(
                !LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices["2"].HasNeighbor(
                    LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices["5"]) &&
                !LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices["5"].HasNeighbor(
                    LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices["2"]));
        }
        #endregion

        #region GRAPH_PROPERTY_TESTS

        [TestMethod]
        public void AfiOfTestGraphsIsCorrect()
        {
            Assert.AreEqual(1.0, SingleEdgeBetweenTwoVertices.Afi, TOLERANCE);
            Assert.AreEqual(1.0, SingleEdgeBetweenTwoVerticesPlusOneOrphan.Afi, TOLERANCE);
            Assert.AreEqual(1.0, Triangle.Afi, TOLERANCE);
            Assert.AreEqual(4.2, StarWithFiveLeafNodes.Afi, TOLERANCE);
            Assert.AreEqual(1.513888888888888, TrianglePlusOneLeaf.Afi, TOLERANCE);
            Assert.AreEqual(1.375, LineGraphWithFourVertices.Afi, TOLERANCE);
            Assert.AreEqual(1.0, CompleteGraphWithFourVertices.Afi, TOLERANCE);
            Assert.AreEqual(1.1666666666666666, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.Afi,
                TOLERANCE);

        }

        [TestMethod]
        public void AssortativityOfTestGraphsIsCorrect()
        {
            Assert.AreEqual(1.0, SingleEdgeBetweenTwoVertices.Assortativity, TOLERANCE);
            Assert.AreEqual(1.0, SingleEdgeBetweenTwoVerticesPlusOneOrphan.Assortativity, TOLERANCE);
            Assert.AreEqual(1.0, Triangle.Assortativity, TOLERANCE);
            Assert.AreEqual(-1.0, StarWithFiveLeafNodes.Assortativity, TOLERANCE);
            Assert.AreEqual(-0.714285714, TrianglePlusOneLeaf.Assortativity, TOLERANCE);
            Assert.AreEqual(-0.5, LineGraphWithFourVertices.Assortativity, TOLERANCE);
            Assert.AreEqual(1.0, CompleteGraphWithFourVertices.Assortativity, TOLERANCE);
            Assert.AreEqual(0.924418605, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.Assortativity,
                TOLERANCE);
        }

        [TestMethod]
        public void MinFiIsCorrectForTestGraphs()
        {
            Assert.AreEqual(1.0, SingleEdgeBetweenTwoVertices.MinFi, TOLERANCE);
            Assert.AreEqual(0.2, StarWithFiveLeafNodes.MinFi, TOLERANCE);
            Assert.AreEqual(0.75, LineGraphWithFourVertices.MinFi, TOLERANCE);
            Assert.AreEqual(0.555555555555, TrianglePlusOneLeaf.MinFi, TOLERANCE);
            Assert.AreEqual(0.75, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.MinFi, TOLERANCE);
        }

        [TestMethod]
        public void MaxFiIsCorrectForTestGraphs()
        {
            Assert.AreEqual(1.0, SingleEdgeBetweenTwoVertices.MaxFi, TOLERANCE);
            Assert.AreEqual(5.0, StarWithFiveLeafNodes.MaxFi, TOLERANCE);
            Assert.AreEqual(2.0, LineGraphWithFourVertices.MaxFi, TOLERANCE);
            Assert.AreEqual(3.0, TrianglePlusOneLeaf.MaxFi, TOLERANCE);
            Assert.AreEqual(2.0, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.MaxFi, TOLERANCE);
        }

        [TestMethod]
        public void CountHappyIsCorrectForTestGraphs()
        {
            Assert.AreEqual(0, SingleEdgeBetweenTwoVertices.CountHappy);
            Assert.AreEqual(0, SingleEdgeBetweenTwoVerticesPlusOneOrphan.CountHappy);
            Assert.AreEqual(0, Triangle.CountHappy);
            Assert.AreEqual(1, StarWithFiveLeafNodes.CountHappy);
            Assert.AreEqual(1, TrianglePlusOneLeaf.CountHappy);
            Assert.AreEqual(2, LineGraphWithFourVertices.CountHappy);
            Assert.AreEqual(0, CompleteGraphWithFourVertices.CountHappy);
            Assert.AreEqual(2, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.CountHappy);
        }
        
        [TestMethod]
        public void CountSadIsCorrectForTestGraphs()
        {
            Assert.AreEqual(0, SingleEdgeBetweenTwoVertices.CountSad);
            Assert.AreEqual(0, SingleEdgeBetweenTwoVerticesPlusOneOrphan.CountSad);
            Assert.AreEqual(0, Triangle.CountSad);
            Assert.AreEqual(5, StarWithFiveLeafNodes.CountSad);
            Assert.AreEqual(3, TrianglePlusOneLeaf.CountSad);
            Assert.AreEqual(2, LineGraphWithFourVertices.CountSad);
            Assert.AreEqual(0, CompleteGraphWithFourVertices.CountSad);
            Assert.AreEqual(2, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.CountSad);
        }

        [TestMethod]
        public void CountNeurtalIsCorrectForTestGraphs()
        {
            Assert.AreEqual(2, SingleEdgeBetweenTwoVertices.CountNeutral);
            Assert.AreEqual(2, SingleEdgeBetweenTwoVerticesPlusOneOrphan.CountNeutral);
            Assert.AreEqual(3, Triangle.CountNeutral);
            Assert.AreEqual(0, StarWithFiveLeafNodes.CountNeutral);
            Assert.AreEqual(0, TrianglePlusOneLeaf.CountNeutral);
            Assert.AreEqual(0, LineGraphWithFourVertices.CountNeutral);
            Assert.AreEqual(4, CompleteGraphWithFourVertices.CountNeutral);
            Assert.AreEqual(5, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.CountNeutral);
        }
        #endregion

        #region GRAPH_PROPERTY_TESTS_FOR_EDGE_COLLECTION_CREATED_GRAPHS
        [TestMethod]
        public void MinFiIsCorrectForTestGraphsCreatedFromEdgeCollections()
        {
            RecreateTestGraphsFromEdgeCollections();
            Assert.AreEqual(1.0, SingleEdgeBetweenTwoVertices.MinFi, TOLERANCE);
            Assert.AreEqual(0.2, StarWithFiveLeafNodes.MinFi, TOLERANCE);
            Assert.AreEqual(0.75, LineGraphWithFourVertices.MinFi, TOLERANCE);
            Assert.AreEqual(0.555555555555, TrianglePlusOneLeaf.MinFi, TOLERANCE);
            Assert.AreEqual(0.75, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.MinFi, TOLERANCE);
        }

        [TestMethod]
        public void MaxFiIsCorrectForTestGraphsCreatedFromEdgeCollections()
        {
            RecreateTestGraphsFromEdgeCollections();
            Assert.AreEqual(1.0, SingleEdgeBetweenTwoVertices.MaxFi, TOLERANCE);
            Assert.AreEqual(5.0, StarWithFiveLeafNodes.MaxFi, TOLERANCE);
            Assert.AreEqual(2.0, LineGraphWithFourVertices.MaxFi, TOLERANCE);
            Assert.AreEqual(3.0, TrianglePlusOneLeaf.MaxFi, TOLERANCE);
            Assert.AreEqual(2.0, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.MaxFi, TOLERANCE);
        }

        [TestMethod]
        public void CountHappyIsCorrectForGraphsCreatedFromEdgeCollections()
        {
            RecreateTestGraphsFromEdgeCollections();
            Assert.AreEqual(0, SingleEdgeBetweenTwoVertices.CountHappy);
            Assert.AreEqual(0, SingleEdgeBetweenTwoVerticesPlusOneOrphan.CountHappy);
            Assert.AreEqual(0, Triangle.CountHappy);
            Assert.AreEqual(1, StarWithFiveLeafNodes.CountHappy);
            Assert.AreEqual(1, TrianglePlusOneLeaf.CountHappy);
            Assert.AreEqual(2, LineGraphWithFourVertices.CountHappy);
            Assert.AreEqual(0, CompleteGraphWithFourVertices.CountHappy);
            Assert.AreEqual(2, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.CountHappy);
        }
        
        [TestMethod]
        public void CountSadIsCorrectForGraphsCreatedFromEdgeCollections()
        {
            RecreateTestGraphsFromEdgeCollections();
            Assert.AreEqual(0, SingleEdgeBetweenTwoVertices.CountSad);
            Assert.AreEqual(0, SingleEdgeBetweenTwoVerticesPlusOneOrphan.CountSad);
            Assert.AreEqual(0, Triangle.CountSad);
            Assert.AreEqual(5, StarWithFiveLeafNodes.CountSad);
            Assert.AreEqual(3, TrianglePlusOneLeaf.CountSad);
            Assert.AreEqual(2, LineGraphWithFourVertices.CountSad);
            Assert.AreEqual(0, CompleteGraphWithFourVertices.CountSad);
            Assert.AreEqual(2, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.CountSad);
        }

        [TestMethod]
        public void CountNeurtalIsCorrectForGraphsCreatedFromEdgeCollections()
        {
            RecreateTestGraphsFromEdgeCollections();
            Assert.AreEqual(2, SingleEdgeBetweenTwoVertices.CountNeutral);
            Assert.AreEqual(2, SingleEdgeBetweenTwoVerticesPlusOneOrphan.CountNeutral);
            Assert.AreEqual(3, Triangle.CountNeutral);
            Assert.AreEqual(0, StarWithFiveLeafNodes.CountNeutral);
            Assert.AreEqual(0, TrianglePlusOneLeaf.CountNeutral);
            Assert.AreEqual(0, LineGraphWithFourVertices.CountNeutral);
            Assert.AreEqual(4, CompleteGraphWithFourVertices.CountNeutral);
            Assert.AreEqual(5, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.CountNeutral);
        }

        [TestMethod]
        public void GraphCreatedFromEdgeCollectionHasCorrectAfi()
        {
            RecreateTestGraphsFromEdgeCollections();

            Assert.AreEqual(1.513888888888888, TrianglePlusOneLeaf.Afi, TOLERANCE);
            Assert.AreEqual(1.375, LineGraphWithFourVertices.Afi, TOLERANCE);
            Assert.AreEqual(1.0, CompleteGraphWithFourVertices.Afi, TOLERANCE);
            Assert.AreEqual(1.1666666666666666, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.Afi, TOLERANCE);
        }
        
        [TestMethod]
        public void GraphCreatedFromEdgeCollectionHasCorrectAssortativity()
        {
            RecreateTestGraphsFromEdgeCollections();
            Assert.AreEqual(-0.714285714, TrianglePlusOneLeaf.Assortativity, TOLERANCE);
            Assert.AreEqual(-0.5, LineGraphWithFourVertices.Assortativity, TOLERANCE);
            Assert.AreEqual(1.0, CompleteGraphWithFourVertices.Afi, TOLERANCE);
            Assert.AreEqual(0.924418605, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.Assortativity, TOLERANCE);
        }
        #endregion

        #region GRAPH_PROPERTY_TESTS_FOR_ADJACENCY_LISTS_CREATED_GRAPHS
        [TestMethod]
        public void MinFiIsCorrectForTestGraphsCreatedFromAdjacencyLists()
        {
            RecreateTestGraphsFromAdjacencyLists();
            Assert.AreEqual(1.0, SingleEdgeBetweenTwoVertices.MinFi, TOLERANCE);
            Assert.AreEqual(0.2, StarWithFiveLeafNodes.MinFi, TOLERANCE);
            Assert.AreEqual(0.75, LineGraphWithFourVertices.MinFi, TOLERANCE);
            Assert.AreEqual(0.555555555555, TrianglePlusOneLeaf.MinFi, TOLERANCE);
            Assert.AreEqual(0.75, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.MinFi, TOLERANCE);
        }

        [TestMethod]
        public void MaxFiIsCorrectForTestGraphsCreatedFromAdjacencyLists()
        {
            RecreateTestGraphsFromAdjacencyLists();
            Assert.AreEqual(1.0, SingleEdgeBetweenTwoVertices.MaxFi, TOLERANCE);
            Assert.AreEqual(5.0, StarWithFiveLeafNodes.MaxFi, TOLERANCE);
            Assert.AreEqual(2.0, LineGraphWithFourVertices.MaxFi, TOLERANCE);
            Assert.AreEqual(3.0, TrianglePlusOneLeaf.MaxFi, TOLERANCE);
            Assert.AreEqual(2.0, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.MaxFi, TOLERANCE);
        }

        [TestMethod]
        public void CountHappyIsCorrectForGraphsCreatedFromAdjacencyLists()
        {
            RecreateTestGraphsFromAdjacencyLists();
            Assert.AreEqual(0, SingleEdgeBetweenTwoVertices.CountHappy);
            Assert.AreEqual(0, SingleEdgeBetweenTwoVerticesPlusOneOrphan.CountHappy);
            Assert.AreEqual(0, Triangle.CountHappy);
            Assert.AreEqual(1, StarWithFiveLeafNodes.CountHappy);
            Assert.AreEqual(1, TrianglePlusOneLeaf.CountHappy);
            Assert.AreEqual(2, LineGraphWithFourVertices.CountHappy);
            Assert.AreEqual(0, CompleteGraphWithFourVertices.CountHappy);
            Assert.AreEqual(2, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.CountHappy);
        }

        [TestMethod]
        public void CountSadIsCorrectForGraphsCreatedFromAdjacencyLists()
        {
            RecreateTestGraphsFromAdjacencyLists();
            Assert.AreEqual(0, SingleEdgeBetweenTwoVertices.CountSad);
            Assert.AreEqual(0, SingleEdgeBetweenTwoVerticesPlusOneOrphan.CountSad);
            Assert.AreEqual(0, Triangle.CountSad);
            Assert.AreEqual(5, StarWithFiveLeafNodes.CountSad);
            Assert.AreEqual(3, TrianglePlusOneLeaf.CountSad);
            Assert.AreEqual(2, LineGraphWithFourVertices.CountSad);
            Assert.AreEqual(0, CompleteGraphWithFourVertices.CountSad);
            Assert.AreEqual(2, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.CountSad);
        }

        [TestMethod]
        public void CountNeurtalIsCorrectForGraphsCreatedFromAdjacencyLists()
        {
            RecreateTestGraphsFromAdjacencyLists();
            Assert.AreEqual(2, SingleEdgeBetweenTwoVertices.CountNeutral);
            Assert.AreEqual(2, SingleEdgeBetweenTwoVerticesPlusOneOrphan.CountNeutral);
            Assert.AreEqual(3, Triangle.CountNeutral);
            Assert.AreEqual(0, StarWithFiveLeafNodes.CountNeutral);
            Assert.AreEqual(0, TrianglePlusOneLeaf.CountNeutral);
            Assert.AreEqual(0, LineGraphWithFourVertices.CountNeutral);
            Assert.AreEqual(4, CompleteGraphWithFourVertices.CountNeutral);
            Assert.AreEqual(5, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.CountNeutral);
        }

        [TestMethod]
        public void GraphCreatedFromAdjacencyListsHasCorrectAfi()
        {
            RecreateTestGraphsFromAdjacencyLists();

            Assert.AreEqual(1.513888888888888, TrianglePlusOneLeaf.Afi, TOLERANCE);
            Assert.AreEqual(1.375, LineGraphWithFourVertices.Afi, TOLERANCE);
            Assert.AreEqual(1.0, CompleteGraphWithFourVertices.Afi, TOLERANCE);
            Assert.AreEqual(1.1666666666666666, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.Afi, TOLERANCE);
        }
        
        [TestMethod]
        public void GraphCreatedFromAdjacencyListsHasCorrectAssortativity()
        {
            RecreateTestGraphsFromAdjacencyLists();
            Assert.AreEqual(-0.714285714, TrianglePlusOneLeaf.Assortativity, TOLERANCE);
            Assert.AreEqual(-0.5, LineGraphWithFourVertices.Assortativity, TOLERANCE);
            Assert.AreEqual(1.0, CompleteGraphWithFourVertices.Afi, TOLERANCE);
            Assert.AreEqual(0.924418605, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.Assortativity, TOLERANCE);
        }
        #endregion

        #region ADD_EDGE_TESTS
        [TestMethod]
        public void AddingAnEdgeToTestGraphsGivesCorrectResultsForHasNeighbor()
        {
            TrianglePlusOneLeaf.AddEdge("1", "4");
            Assert.IsTrue(TrianglePlusOneLeaf["1"].HasNeighbor(TrianglePlusOneLeaf["4"]));
            Assert.IsTrue(TrianglePlusOneLeaf["4"].HasNeighbor(TrianglePlusOneLeaf["1"]));
            Assert.IsFalse(TrianglePlusOneLeaf["2"].HasNeighbor(TrianglePlusOneLeaf["4"]));
            Assert.IsFalse(TrianglePlusOneLeaf["4"].HasNeighbor(TrianglePlusOneLeaf["2"]));
        }

        [TestMethod]
        public void AddingAnEdgeUpdatesEdgesCollectionCorrectly()
        {
            Graph g = StarWithFiveLeafNodes;
            g.AddEdge("3", "4");
            Assert.IsTrue(g.Edges.Any(e => e.V1.Id == "3" && e.V2.Id == "4"));
            Assert.IsTrue(g.Edges.Any(e => e.V1.Id == "1" && e.V2.Id == "3"));
            Assert.IsTrue(g.Edges.Any(e => e.V1.Id == "1" && e.V2.Id == "4"));
            Assert.IsTrue(g.Edges.Any(e => e.V1.Id == "1" && e.V2.Id == "2"));
            Assert.IsFalse(g.Edges.Any(e => (e.V1.Id == "2" && e.V2.Id == "3") || (e.V1.Id == "3" && e.V2.Id == "2")));
            Assert.IsFalse(g.Edges.Any(e => (e.V1.Id == "4" && e.V2.Id == "5") || (e.V1.Id == "5" && e.V2.Id == "4")));
            Assert.IsFalse(g.Edges.Any(e => (e.V1.Id == "5" && e.V2.Id == "6") || (e.V1.Id == "6" && e.V2.Id == "5")));
        }

        [TestMethod]
        public void AddingAnEdgeUpdatesMinFiCorrectly()
        {
            TrianglePlusOneLeaf.AddEdge("1", "4");
            Assert.AreEqual(0.7777777777777777, TrianglePlusOneLeaf.MinFi, TOLERANCE);

            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.AddEdge("3", "5");
            Assert.AreEqual(0.76, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.MinFi, TOLERANCE);
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.AddEdge("4", "1");
            Assert.AreEqual(0.76, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.MinFi, TOLERANCE);
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.AddEdge("6", "4");
            Assert.AreEqual(0.8, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.MinFi, TOLERANCE);
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.AddEdge("7", "1");
            Assert.AreEqual(0.84, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.MinFi, TOLERANCE);
        }

        [TestMethod]
        public void AddingAnEdgeUpdatesMaxFiCorrectly()
        {
            TrianglePlusOneLeaf.AddEdge("1", "4");
            Assert.AreEqual(1.5, TrianglePlusOneLeaf.MaxFi, TOLERANCE);
            
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.AddEdge("3", "5");
            Assert.AreEqual(3.0, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.MaxFi, TOLERANCE);
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.AddEdge("4", "1");
            Assert.AreEqual(1.25, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.MaxFi, TOLERANCE);
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.AddEdge("6", "4");
            Assert.AreEqual(1.25, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.MaxFi, TOLERANCE);
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.AddEdge("7", "1");
            Assert.AreEqual(1.5, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.MaxFi, TOLERANCE);
        }

        [TestMethod]
        public void AddingAnEdgeUpdatesCountHappy()
        {
            TrianglePlusOneLeaf.AddEdge("4", "2");
            Assert.AreEqual(2, TrianglePlusOneLeaf.CountHappy);
            TrianglePlusOneLeaf.AddEdge("4", "1");
            Assert.AreEqual(0, TrianglePlusOneLeaf.CountHappy);

            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.AddEdge("3", "5");
            Assert.AreEqual(2, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.CountHappy);
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.AddEdge("4", "1");
            Assert.AreEqual(1, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.CountHappy);
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.AddEdge("6", "4");
            Assert.AreEqual(2, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.CountHappy);
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.AddEdge("7", "1");
            Assert.AreEqual(3, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.CountHappy);
        }

        [TestMethod]
        public void AddingAnEdgeUpdatesCountSad()
        {
            TrianglePlusOneLeaf.AddEdge("4", "2");
            Assert.AreEqual(2, TrianglePlusOneLeaf.CountSad);
            TrianglePlusOneLeaf.AddEdge("4", "1");
            Assert.AreEqual(0, TrianglePlusOneLeaf.CountSad);

            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.AddEdge("3", "5");
            Assert.AreEqual(6, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.CountSad);
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.AddEdge("4", "1");
            Assert.AreEqual(6, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.CountSad);
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.AddEdge("6", "4");
            Assert.AreEqual(7, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.CountSad);
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.AddEdge("7", "1");
            Assert.AreEqual(6, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.CountSad);
        }

        [TestMethod]
        public void AddingAnEdgeUpdatesCountNeutral()
        {
            TrianglePlusOneLeaf.AddEdge("4", "2");
            Assert.AreEqual(0, TrianglePlusOneLeaf.CountNeutral);
            TrianglePlusOneLeaf.AddEdge("4", "1");
            Assert.AreEqual(4, TrianglePlusOneLeaf.CountNeutral);

            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.AddEdge("3", "5");
            Assert.AreEqual(1, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.CountNeutral);
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.AddEdge("4", "1");
            Assert.AreEqual(2, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.CountNeutral);
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.AddEdge("6", "4");
            Assert.AreEqual(0, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.CountNeutral);
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.AddEdge("7", "1");
            Assert.AreEqual(0, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.CountNeutral);
        }

        [TestMethod]
        public void AddingAnEdgeToANewVertexUpdatesStatisticsCorrectly()
        {
            StarWithFiveLeafNodes.AddEdge("7", "3");
            Assert.AreEqual(0.24, StarWithFiveLeafNodes.MinFi, TOLERANCE);
            Assert.AreEqual(5.0, StarWithFiveLeafNodes.MaxFi, TOLERANCE);
            Assert.AreEqual(6, StarWithFiveLeafNodes.CountSad);
            Assert.AreEqual(1, StarWithFiveLeafNodes.CountHappy);
            Assert.AreEqual(0, StarWithFiveLeafNodes.CountNeutral);

            LineGraphWithFourVertices.AddEdge("4", "5");
            Assert.AreEqual(0.75, LineGraphWithFourVertices.MinFi, TOLERANCE);
            Assert.AreEqual(2.0, LineGraphWithFourVertices.MaxFi, TOLERANCE);
            Assert.AreEqual(2, LineGraphWithFourVertices.CountSad);
            Assert.AreEqual(2, LineGraphWithFourVertices.CountHappy);
            Assert.AreEqual(1, LineGraphWithFourVertices.CountNeutral);
        }

        [TestMethod]
        public void AddingAnEdgeToTestGraphsGivesCorrectFiForInvolvedVertices()
        {
            StarWithFiveLeafNodes.AddEdge("3", "4");
            Assert.AreEqual(0.28, StarWithFiveLeafNodes["1"].Fi, TOLERANCE);
            Assert.AreEqual(5.0, StarWithFiveLeafNodes["2"].Fi, TOLERANCE);
            Assert.AreEqual(5.0, StarWithFiveLeafNodes["5"].Fi, TOLERANCE);
            Assert.AreEqual(5.0, StarWithFiveLeafNodes["6"].Fi, TOLERANCE);
            Assert.AreEqual(1.75, StarWithFiveLeafNodes["3"].Fi, TOLERANCE);
            Assert.AreEqual(1.75, StarWithFiveLeafNodes["4"].Fi, TOLERANCE);
        }

        [TestMethod]
        public void AddingAnEdgeToTestGraphsGivesCorrectAfi()
        {
            TwoOrphanVertices.AddEdge("1", "2");
            Assert.AreEqual(1.0, TwoOrphanVertices.Afi, TOLERANCE);

            SingleEdgeBetweenTwoVerticesPlusOneOrphan.AddEdge("2", "3");
            Assert.AreEqual(1.5, SingleEdgeBetweenTwoVerticesPlusOneOrphan.Afi, TOLERANCE);

            StarWithFiveLeafNodes.AddEdge("3", "4");
            Assert.AreEqual(3.13, StarWithFiveLeafNodes.Afi, TOLERANCE);

            LineGraphWithFourVertices.AddEdge("2", "4");
            Assert.AreEqual(1.513888888888888, LineGraphWithFourVertices.Afi, TOLERANCE);

            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.AddEdge("3", "5");
            Assert.AreEqual(1.3220987654320988, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.Afi,
                TOLERANCE);
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.AddEdge("6", "4");
            Assert.AreEqual(1.2149999999999999, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.Afi,
                TOLERANCE);
        }

        #endregion

        #region REMOVE_EDGE_TESTS
        [TestMethod]
        public void RemovingAnEdgeFromTestGraphsGivesCorrectResultsForHasNeighbor()
        {
            CompleteGraphWithFourVertices.RemoveEdge("1", "2");
            Assert.IsFalse(CompleteGraphWithFourVertices["1"].HasNeighbor(CompleteGraphWithFourVertices["2"]));
            Assert.IsFalse(CompleteGraphWithFourVertices["2"].HasNeighbor(CompleteGraphWithFourVertices["1"]));
            Assert.IsTrue(CompleteGraphWithFourVertices["1"].HasNeighbor(CompleteGraphWithFourVertices["3"]));
            Assert.IsTrue(CompleteGraphWithFourVertices["3"].HasNeighbor(CompleteGraphWithFourVertices["1"]));
            Assert.IsTrue(CompleteGraphWithFourVertices["2"].HasNeighbor(CompleteGraphWithFourVertices["4"]));
            Assert.IsTrue(CompleteGraphWithFourVertices["4"].HasNeighbor(CompleteGraphWithFourVertices["2"]));
            Assert.IsTrue(CompleteGraphWithFourVertices["3"].HasNeighbor(CompleteGraphWithFourVertices["4"]));
            Assert.IsTrue(CompleteGraphWithFourVertices["4"].HasNeighbor(CompleteGraphWithFourVertices["3"]));
        }

        [TestMethod]
        public void RemovingAnEdgeUpdatesEdgesCollectionCorrectly()
        {
            Graph g = CompleteGraphWithFourVertices;
            g.RemoveEdge("3", "2");
            Assert.IsFalse(g.Edges.Any(e => (e.V1.Id == "2" && e.V2.Id == "3") || (e.V1.Id == "3" && e.V2.Id == "2")));
            Assert.IsTrue(g.Edges.Any(e => e.V1.Id == "1" && e.V2.Id == "3"));
            Assert.IsTrue(g.Edges.Any(e => e.V1.Id == "1" && e.V2.Id == "4"));
            Assert.IsTrue(g.Edges.Any(e => e.V1.Id == "2" && e.V2.Id == "4"));
            Assert.IsTrue(g.Edges.Any(e => e.V1.Id == "3" && e.V2.Id == "4"));
        }

        [TestMethod]
        public void RemovingAnEdgeFromTestGraphsGivesCorrectFiForInvolvedVertices()
        {
            CompleteGraphWithFourVertices.RemoveEdge("1", "2");
            Assert.AreEqual(1.5, CompleteGraphWithFourVertices["1"].Fi, TOLERANCE);
            Assert.AreEqual(1.5, CompleteGraphWithFourVertices["2"].Fi, TOLERANCE);
            Assert.AreEqual(0.777777777777777777, CompleteGraphWithFourVertices["3"].Fi, TOLERANCE);
            Assert.AreEqual(0.777777777777777777, CompleteGraphWithFourVertices["4"].Fi, TOLERANCE);
        }

        [TestMethod]
        public void RemovingAnEdgeUpdatesMinFiCorrectly()
        {
            TrianglePlusOneLeaf.RemoveEdge("2", "3");
            Assert.AreEqual(0.75, TrianglePlusOneLeaf.MinFi);

            Triangle.RemoveEdge("2", "3");
            Assert.AreEqual(0.5, Triangle.MinFi, TOLERANCE);
            Triangle.RemoveEdge("1", "3");
            Assert.AreEqual(1.0, Triangle.MinFi, TOLERANCE);

            LineGraphWithFourVertices.RemoveEdge("3", "4");
            Assert.AreEqual(0.5, LineGraphWithFourVertices.MinFi, TOLERANCE);
        }

        [TestMethod]
        public void RemovingAnEdgeUpdatesMaxFiCorrectly()
        {
            Triangle.RemoveEdge("2", "3");
            Assert.AreEqual(2.0, Triangle.MaxFi, TOLERANCE);

            TrianglePlusOneLeaf.RemoveEdge("1", "3");
            Assert.AreEqual(2.0, TrianglePlusOneLeaf.MaxFi, TOLERANCE);

            TrianglePlusOneLeaf.RemoveEdge("2", "3");
            Assert.AreEqual(1.0, TrianglePlusOneLeaf.MaxFi, TOLERANCE);
        }

        [TestMethod]
        public void RemovingAnEdgeUpdatesCountHappy()
        {
            BowTie.RemoveEdge("4", "6");
            Assert.AreEqual(2, BowTie.CountHappy);
            BowTie.RemoveEdge("5", "6");
            Assert.AreEqual(1, BowTie.CountHappy);
        }

        [TestMethod]
        public void RemovingAnEdgeUpdatesCountSad()
        {
            BowTie.RemoveEdge("4", "6");
            Assert.AreEqual(4, BowTie.CountSad);
            BowTie.RemoveEdge("5", "6");
            Assert.AreEqual(3, BowTie.CountSad);

            Triangle.RemoveEdge("2", "3");
            Assert.AreEqual(2, Triangle.CountSad);
        }

        [TestMethod]
        public void RemovingAnEdgeUpdatesCountNeutral()
        {
            BowTie.RemoveEdge("4", "6");
            Assert.AreEqual(0, BowTie.CountNeutral);
            BowTie.RemoveEdge("5", "6");
            Assert.AreEqual(1, BowTie.CountNeutral);

            Triangle.RemoveEdge("2", "3");
            Assert.AreEqual(0, Triangle.CountNeutral);
        }

        [TestMethod]
        public void RemovingAnEdgeFromTestGraphsGivesCorrectAfi()
        {
            Triangle.RemoveEdge("1", "3");
            Assert.AreEqual(1.5, Triangle.Afi, TOLERANCE);

            CompleteGraphWithFourVertices.RemoveEdge("1", "4");
            Assert.AreEqual(1.138888888888888, CompleteGraphWithFourVertices.Afi, TOLERANCE);
            CompleteGraphWithFourVertices.RemoveEdge("1", "3");
            Assert.AreEqual(1.513888888888888, CompleteGraphWithFourVertices.Afi, TOLERANCE);

            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.RemoveEdge("6", "9");
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.RemoveEdge("2", "3");
            Assert.AreEqual(1.032407407407407, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.Afi,
                TOLERANCE);
        }

        [TestMethod]
        public void MultipleAddRemovesToTestGraphsGivesCorrectAfi()
        {
            Graph g = LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices;
            g.AddEdge("1", "5");
            g.RemoveEdge("7", "5");
            g.RemoveEdge("7", "9");
            g.AddEdge("8", "1");
            g.AddEdge("5", "1");
            g.AddEdge("6", "3");

            Assert.AreEqual(1.447407407407407, g.Afi, TOLERANCE);
        }

        [TestMethod]
        public void AddingAnEdgeToTestGraphsGivesCorrectAssortativity()
        {
            TwoOrphanVertices.AddEdge("1", "2");
            Assert.AreEqual(1.0, TwoOrphanVertices.Assortativity, TOLERANCE);

            SingleEdgeBetweenTwoVerticesPlusOneOrphan.AddEdge("2", "3");
            Assert.AreEqual(-1.0, SingleEdgeBetweenTwoVerticesPlusOneOrphan.Assortativity, TOLERANCE);

            StarWithFiveLeafNodes.AddEdge("3", "4");
            Assert.AreEqual(-0.83333333333333, StarWithFiveLeafNodes.Assortativity, TOLERANCE);

            LineGraphWithFourVertices.AddEdge("2", "4");
            Assert.AreEqual(-0.714285714285714, LineGraphWithFourVertices.Assortativity, TOLERANCE);

            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.AddEdge("3", "5");
            Assert.AreEqual(0.5585585585585586,
                LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.Assortativity,
                TOLERANCE);
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.AddEdge("6", "4");
            Assert.AreEqual(0.4128113879003564,
                LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.Assortativity,
                TOLERANCE);
        }

        [TestMethod]
        public void RemovingAnEdgeFromTestGraphsGivesCorrectAssortativity()
        {
            Triangle.RemoveEdge("1", "3");
            Assert.AreEqual(-1.0, Triangle.Assortativity, TOLERANCE);

            CompleteGraphWithFourVertices.RemoveEdge("1", "4");
            Assert.AreEqual(-0.66666666666667, CompleteGraphWithFourVertices.Assortativity, TOLERANCE);
            CompleteGraphWithFourVertices.RemoveEdge("1", "3");
            Assert.AreEqual(-0.714285714285714, CompleteGraphWithFourVertices.Assortativity, TOLERANCE);

            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.RemoveEdge("6", "9");
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.RemoveEdge("2", "3");
            Assert.AreEqual(0.780000000000000,
                LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.Assortativity,
                TOLERANCE);
        }

        [TestMethod]
        public void MultipleAddRemovesToTestGraphsGivesCorrectAssortativity()
        {
            Graph g = LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices;
            g.AddEdge("1", "5");
            g.RemoveEdge("7", "5");
            g.RemoveEdge("7", "9");
            g.AddEdge("8", "1");
            g.AddEdge("5", "1");
            g.AddEdge("6", "3");

            Assert.AreEqual(-0.08833922261484, g.Assortativity, TOLERANCE);
        }
        #endregion

        #region EDGE_SWAP_TESTS
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void SwapFailsWhenIdsArentPresentInTheGraph()
        {
            LineGraphWithFourVertices.SwapEdges("2", "1", "4", "3", "4", "6");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void SwapFailsWhenCurrNeighbor1IsntActuallyNeighbor()
        {
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.SwapEdges("2", "4", "6", "5", "6", "2");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void SwapFailsWhenCurrNeighbor2IsntActuallyNeighbor()
        {
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.SwapEdges("2", "3", "6", "5", "4", "2");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void SwapFailsWhenNewNeighbor1AlreadyIsNeighbor()
        {
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.SwapEdges("2", "3", "1", "5", "6", "2");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void SwapFailsWhenNewNeighbor2AlreadyIsNeighbor()
        {
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.SwapEdges("2", "3", "6", "5", "4", "7");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void SwapFailsWhenNewNeighbor1IsntOldNeighborOf2()
        {
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.SwapEdges("2", "3", "6", "5", "7", "3");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void SwapFailsWhenNewNeighbor2IsntOldNeighborOf1()
        {
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.SwapEdges("2", "3", "6", "5", "6", "7");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void SwapFailsWhenBothNewEdgesAreTheSame()
        {
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.SwapEdges("2", "3", "6", "6", "5", "2");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void SwapFailsWhenBothOldEdgesAreTheSame()
        {
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.SwapEdges("2", "3", "6", "3", "2", "5");
        }

        [TestMethod]
        public void SwapResultsInCorrectResultsForHasNeighbor()
        {
            Graph g = LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices;
            g.SwapEdges("2", "3", "6", "5", "6", "3");
            Assert.IsTrue(g["2"].HasNeighbor(g["1"]));
            Assert.IsTrue(g["1"].HasNeighbor(g["2"]));
            Assert.IsTrue(g["2"].HasNeighbor(g["6"]));
            Assert.IsTrue(g["6"].HasNeighbor(g["8"]));
            Assert.IsTrue(g["8"].HasNeighbor(g["6"]));
            Assert.IsTrue(g["6"].HasNeighbor(g["2"]));
            Assert.IsFalse(g["2"].HasNeighbor(g["3"]));
            Assert.IsFalse(g["3"].HasNeighbor(g["2"]));
        }

        [TestMethod]
        public void SwapPreservesAllDegrees()
        {
            Graph g = LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices;
            g.SwapEdges("2", "3", "6", "5", "6", "3");
            Assert.AreEqual(1, g["1"].Degree);
            Assert.AreEqual(2, g["2"].Degree);
            Assert.AreEqual(2, g["3"].Degree);
            Assert.AreEqual(1, g["4"].Degree);
            Assert.AreEqual(4, g["5"].Degree);
            Assert.AreEqual(4, g["6"].Degree);
            Assert.AreEqual(4, g["7"].Degree);
            Assert.AreEqual(4, g["8"].Degree);
            Assert.AreEqual(4, g["9"].Degree);
        }

        [TestMethod]
        public void SwapGivesCorrectFiForAllInvolvedVertices()
        {
            Graph g = LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices;
            g.SwapEdges("2", "3", "6", "5", "6", "3");
            Assert.AreEqual(2.0, g["1"].Fi, TOLERANCE);
            Assert.AreEqual(1.25, g["2"].Fi, TOLERANCE);
            Assert.AreEqual(1.25, g["3"].Fi, TOLERANCE);
            Assert.AreEqual(2.0, g["4"].Fi, TOLERANCE);
            Assert.AreEqual(0.875, g["5"].Fi, TOLERANCE);
            Assert.AreEqual(0.875, g["6"].Fi, TOLERANCE);
            Assert.AreEqual(1.0, g["7"].Fi, TOLERANCE);
            Assert.AreEqual(1.0, g["8"].Fi, TOLERANCE);
            Assert.AreEqual(1.0, g["9"].Fi, TOLERANCE);
        }

        [TestMethod]
        public void SwappingEdgesUpdatesMinFiCorrectly()
        {
            BowTie.SwapEdges("6", "4", "2", "1", "2", "4");
            Assert.AreEqual(0.77777777777777777, BowTie.MinFi, TOLERANCE);

            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.SwapEdges("3", "4", "5", "7", "5", "4");
            Assert.AreEqual(0.75, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.MinFi, TOLERANCE);
            RecreateTestGraphsFromEdgeCollections(); // recreate to try another edge swap
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.SwapEdges("2", "3", "6", "5", "6", "3");
            Assert.AreEqual(0.875, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.MinFi, TOLERANCE);
            
            // Need another case...
            Graph g = Graph.NewGraphFromEdgeCollection(new List<Tuple<string, string>>
                {
                    new Tuple<string, string>("1", "2"),
                    new Tuple<string, string>("1", "3"),
                    new Tuple<string, string>("1", "7"),
                    new Tuple<string, string>("1", "5"),
                    new Tuple<string, string>("4", "6"),
                    new Tuple<string, string>("6", "9"),
                    new Tuple<string, string>("9", "8"),
                    new Tuple<string, string>("8", "7")
                }
            );
            Assert.AreEqual(0.3125, g.MinFi, TOLERANCE);
            g.SwapEdges("7", "1", "6", "4", "6", "1");
            Assert.AreEqual(0.25, g.MinFi, TOLERANCE);

            // A case where it doesn't change
            LineGraphWithFourVertices.SwapEdges("2", "1", "4", "3", "4", "1");
            Assert.AreEqual(0.75, LineGraphWithFourVertices.MinFi);
        }

        [TestMethod]
        public void SwappingEdgesUpdatesMaxFiCorrectly()
        {
            BowTie.SwapEdges("6", "4", "2", "1", "2", "4");
            Assert.AreEqual(1.5, BowTie.MaxFi, TOLERANCE);

            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.SwapEdges("3", "4", "5", "7", "5", "4");
            Assert.AreEqual(4.0, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.MaxFi, TOLERANCE);
            RecreateTestGraphsFromEdgeCollections(); // recreate to try another edge swap
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.SwapEdges("2", "3", "6", "5", "6", "3");
            Assert.AreEqual(2.0, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.MaxFi, TOLERANCE);
            // TODO: This needs a few more scenarios, make sure to have at least one that changes the property and one that doesn't

            // Need another case...
            Graph g = Graph.NewGraphFromEdgeCollection(new List<Tuple<string, string>>
                {
                    new Tuple<string, string>("1", "2"),
                    new Tuple<string, string>("1", "3"),
                    new Tuple<string, string>("1", "4"),
                    new Tuple<string, string>("1", "5"),
                    new Tuple<string, string>("4", "5"),
                    new Tuple<string, string>("2", "3"),
                    new Tuple<string, string>("6", "7"),
                    new Tuple<string, string>("7", "8"),
                    new Tuple<string, string>("7", "9"),
                    new Tuple<string, string>("9", "8"),
                }
            );
            Assert.AreEqual(3.0, g.MaxFi, TOLERANCE);
            g.SwapEdges("6", "7", "1", "5", "1", "7");
            Assert.AreEqual(4.0, g.MaxFi, TOLERANCE);

            // A case where it doesn't change
            LineGraphWithFourVertices.SwapEdges("2", "1", "4", "3", "4", "1");
            Assert.AreEqual(2.0, LineGraphWithFourVertices.MaxFi);
        }

        [TestMethod]
        public void SwappingEdgesUpdatesCountHappy()
        {
            BowTie.SwapEdges("6", "4", "2", "1", "2", "4");
            Assert.AreEqual(2, BowTie.CountHappy);

            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.SwapEdges("3", "4", "5", "7", "5", "4");
            Assert.AreEqual(3, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.CountHappy);
            RecreateTestGraphsFromEdgeCollections(); // recreate to try another edge swap
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.SwapEdges("2", "3", "6", "5", "6", "3");
            Assert.AreEqual(2, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.MaxFi);

            // Need another case...
            Graph g = Graph.NewGraphFromEdgeCollection(new List<Tuple<string, string>>
                {
                    new Tuple<string, string>("1", "2"),
                    new Tuple<string, string>("1", "3"),
                    new Tuple<string, string>("1", "7"),
                    new Tuple<string, string>("1", "5"),
                    new Tuple<string, string>("4", "6"),
                    new Tuple<string, string>("6", "9"),
                    new Tuple<string, string>("9", "8"),
                    new Tuple<string, string>("8", "7")
                }
            );
            Assert.AreEqual(2, g.CountHappy);
            g.SwapEdges("7", "1", "6", "4", "6", "1");
            Assert.AreEqual(1, g.CountHappy);


            // A case where it doesn't change
            LineGraphWithFourVertices.SwapEdges("2", "1", "4", "3", "4", "1");
            Assert.AreEqual(2, LineGraphWithFourVertices.CountHappy);

        }

        [TestMethod]
        public void SwappingEdgesUpdatesCountSad()
        {
            BowTie.SwapEdges("4", "5", "1", "2", "1", "5");
            Assert.AreEqual(2, BowTie.CountHappy);

            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.SwapEdges("3", "4", "5", "7", "5", "4");
            Assert.AreEqual(3, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.CountHappy);
            RecreateTestGraphsFromEdgeCollections(); // recreate to try another edge swap
            LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.SwapEdges("2", "3", "6", "5", "6", "3");
            Assert.AreEqual(2, LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices.MaxFi);

            // Need another case...
            Graph g = Graph.NewGraphFromEdgeCollection(new List<Tuple<string, string>>
                {
                    new Tuple<string, string>("1", "2"),
                    new Tuple<string, string>("1", "3"),
                    new Tuple<string, string>("1", "7"),
                    new Tuple<string, string>("1", "5"),
                    new Tuple<string, string>("4", "6"),
                    new Tuple<string, string>("6", "9"),
                    new Tuple<string, string>("9", "8"),
                    new Tuple<string, string>("8", "7")
                }
            );
            Assert.AreEqual(5, g.CountSad);
            g.SwapEdges("7", "1", "6", "4", "6", "1");
            Assert.AreEqual(4, g.CountSad);


            // A case where it doesn't change
            LineGraphWithFourVertices.SwapEdges("2", "1", "4", "3", "4", "1");
            Assert.AreEqual(2, LineGraphWithFourVertices.CountSad);
        }

        [TestMethod]
        public void SwappingEdgesUpdatesCountNeutral()
        {
            Graph g = Graph.NewGraphFromEdgeCollection(new List<Tuple<string, string>>
                {
                    new Tuple<string, string>("1", "2"),
                    new Tuple<string, string>("1", "3"),
                    new Tuple<string, string>("1", "7"),
                    new Tuple<string, string>("1", "5"),
                    new Tuple<string, string>("4", "6"),
                    new Tuple<string, string>("6", "9"),
                    new Tuple<string, string>("9", "8"),
                    new Tuple<string, string>("8", "7")
                }
            );
            Assert.AreEqual(2, g.CountNeutral);
            g.SwapEdges("7", "1", "6", "4", "6", "1");
            Assert.AreEqual(4, g.CountNeutral);


            // A case where it doesn't change
            LineGraphWithFourVertices.SwapEdges("2", "1", "4", "3", "4", "1");
            Assert.AreEqual(0, LineGraphWithFourVertices.CountNeutral);
        }

        [TestMethod]
        public void SwapGivesCorrectAfiForGraph()
        {
            Graph g = LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices;
            g.SwapEdges("2", "3", "6", "5", "6", "3");
            Assert.AreEqual(1.25, g.Afi, TOLERANCE);
        }

        [TestMethod]
        public void SwapGivesCorrectAssortativityForGraph()
        {
            Graph g = LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices;
            g.SwapEdges("2", "3", "6", "5", "6", "3");
            Assert.AreEqual(0.6220930232558, g.Assortativity, TOLERANCE);
        }

        [TestMethod]
        public void SwappingAnEdgeUpdatesEdgesCollectionCorrectly()
        {
            Graph g = LineOfFourVerticesAndDisconnectedCompleteGraphWithFiveVertices;
            g.SwapEdges("3", "2", "5", "6", "5", "2");
            Assert.IsTrue(g.Edges.Any(e => e.V1.Id == "2" && e.V2.Id == "6"));
            Assert.IsTrue(g.Edges.Any(e => e.V1.Id == "3" && e.V2.Id == "5"));
            Assert.IsTrue(g.Edges.Any(e => e.V1.Id == "1" && e.V2.Id == "2"));
            Assert.IsTrue(g.Edges.Any(e => e.V1.Id == "3" && e.V2.Id == "4"));
            Assert.IsFalse(g.Edges.Any(e => (e.V1.Id == "2" && e.V2.Id == "3") || (e.V1.Id == "3" && e.V2.Id == "2")));
            Assert.IsFalse(g.Edges.Any(e => (e.V1.Id == "5" && e.V2.Id == "6") || (e.V1.Id == "6" && e.V2.Id == "5")));
        }
        #endregion

        // Test swap for AFI and Assortativity, then can probably code the methods
        // Code
        // Write search class
    }
}
