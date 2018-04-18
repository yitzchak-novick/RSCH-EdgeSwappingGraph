using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdgeSwappingSearch
{
    // The purpose of this interface is just so that Graph can expose the properties of Vertex that are needed
    // without returning a Vertex where the properties are actually going to be public
    public interface Vertex
    {
        string Id { get; }
        IEnumerable<Vertex> Neighbors { get; }
        int Degree { get; }
        int SumOfNeighborsDegrees { get; }
        double Fi { get; }
        bool IsHappy { get; }
        bool IsSad { get; }
        bool IsNeutral { get; }
        bool HasNeighbor(Vertex v);
    }
}
