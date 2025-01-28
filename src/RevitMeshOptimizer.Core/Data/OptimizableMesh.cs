using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitMeshOptimizer.Core.Data
{
    public class OptimizableMesh
    {
        public List<MeshVertex> Vertices { get; private set; }
        public List<int> Triangles { get; private set; }
        public List<bool> ValidTriangles { get; private set; }
        private List<EdgeCollapse> _possibleCollapses;
        
        public OptimizableMesh(Mesh revitMesh)
        {
            InitializeFromRevitMesh(revitMesh);
            BuildEdgeCollapses();
        }

        private void InitializeFromRevitMesh(Mesh revitMesh)
        {
            Vertices = new List<MeshVertex>();
            Triangles = new List<int>();
            ValidTriangles = new List<bool>();

            // Extract vertices
            for (int i = 0; i < revitMesh.Vertices.Count; i++)
            {
                Vertices.Add(new MeshVertex(revitMesh.Vertices[i]));
            }

            // Extract triangles
            for (int i = 0; i < revitMesh.NumTriangles; i++)
            {
                MeshTriangle triangle = revitMesh.get_Triangle(i);
                Triangles.Add(triangle.get_Index(0));
                Triangles.Add(triangle.get_Index(1));
                Triangles.Add(triangle.get_Index(2));
                ValidTriangles.Add(true);

                // Add triangle references to vertices
                for (int j = 0; j < 3; j++)
                {
                    Vertices[triangle.get_Index(j)].ConnectedTriangles.Add(i);
                }
            }
        }

        private void BuildEdgeCollapses()
        {
            _possibleCollapses = new List<EdgeCollapse>();
            HashSet<string> processedEdges = new HashSet<string>();

            // For each triangle
            for (int i = 0; i < Triangles.Count; i += 3)
            {
                if (!ValidTriangles[i / 3]) continue;

                // For each edge in the triangle
                for (int j = 0; j < 3; j++)
                {
                    int v1 = Triangles[i + j];
                    int v2 = Triangles[i + ((j + 1) % 3)];

                    // Create unique edge identifier
                    string edgeId = Math.Min(v1, v2) + "-" + Math.Max(v1, v2);
                    
                    if (processedEdges.Contains(edgeId)) continue;
                    processedEdges.Add(edgeId);

                    // Calculate optimal position and cost
                    XYZ optimalPosition = CalculateOptimalPosition(v1, v2);
                    double cost = CalculateCollapseCost(v1, v2, optimalPosition);

                    _possibleCollapses.Add(new EdgeCollapse(v1, v2, optimalPosition, cost));
                }
            }

            // Sort collapses by cost
            _possibleCollapses.Sort((a, b) => a.Cost.CompareTo(b.Cost));
        }

        private XYZ CalculateOptimalPosition(int v1Index, int v2Index)
        {
            var v1 = Vertices[v1Index];
            var v2 = Vertices[v2Index];

            // For now, simply use the midpoint
            // TODO: Implement more sophisticated position calculation using quadric error metrics
            return new XYZ(
                (v1.Position.X + v2.Position.X) / 2,
                (v1.Position.Y + v2.Position.Y) / 2,
                (v1.Position.Z + v2.Position.Z) / 2
            );
        }

        private double CalculateCollapseCost(int v1Index, int v2Index, XYZ newPosition)
        {
            var v1 = Vertices[v1Index];
            var v2 = Vertices[v2Index];

            // Combine quadric error metrics from both vertices
            double cost = v1.Quadric.ComputeError(newPosition) + v2.Quadric.ComputeError(newPosition);

            // Add penalties for feature preservation
            if (IsOnBoundary(v1Index) || IsOnBoundary(v2Index))
            {
                cost *= 10.0; // Significant penalty for boundary vertices
            }

            return cost;
        }

        private bool IsOnBoundary(int vertexIndex)
        {
            // TODO: Implement boundary detection
            // A vertex is on a boundary if it has any edge that belongs to only one triangle
            return false;
        }

        public bool CollapseNextEdge()
        {
            while (_possibleCollapses.Count > 0)
            {
                var collapse = _possibleCollapses[0];
                _possibleCollapses.RemoveAt(0);

                if (TryCollapseEdge(collapse))
                {
                    return true;
                }
            }
            return false;
        }

        private bool TryCollapseEdge(EdgeCollapse collapse)
        {
            // TODO: Implement actual edge collapse
            // 1. Update vertex position
            // 2. Update triangle references
            // 3. Mark collapsed triangles as invalid
            // 4. Update affected edge collapses
            return false;
        }

        public Mesh ToRevitMesh()
        {
            // Create new Revit mesh from optimized data
            Mesh result = new Mesh();
            
            // Add all valid vertices
            foreach (var vertex in Vertices)
            {
                result.Vertices.Add(vertex.Position);
            }

            // Add all valid triangles
            for (int i = 0; i < Triangles.Count; i += 3)
            {
                if (ValidTriangles[i / 3])
                {
                    result.Triangles.Add(
                        Triangles[i],
                        Triangles[i + 1],
                        Triangles[i + 2]
                    );
                }
            }

            return result;
        }
    }
}