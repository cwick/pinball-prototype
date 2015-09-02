using UnityEngine;
using System.Collections.Generic;

namespace DynamicMesh2D {
    public class Face {
        public int[] Vertices;

        public int[] Triangulate() {
            var triangles = new List<int>();
          
            Debug.Assert(Vertices.Length >= 3, "Dynamic mesh must have at least 3 vertices");

            for (int i=1 ; i <= Vertices.Length - 2 ; i++) {
                triangles.Add(Vertices[0]);
                triangles.Add(Vertices[i]);
                triangles.Add(Vertices[i+1]);
            }

            return triangles.ToArray();
        }
    }
}
