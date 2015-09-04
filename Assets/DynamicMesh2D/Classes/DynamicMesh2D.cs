using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace DynamicMesh2D {
    [System.Serializable]
    public class DynamicMesh2D {
        public List<Vector2> Vertices = new List<Vector2>();
        public List<Face> Faces = new List<Face>();

        public Mesh BuildMesh() {
            var mesh = new Mesh();
            mesh.vertices = Vertices.Select( x => (Vector3)x).ToArray();
            mesh.triangles = Triangulate();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        public int[] Triangulate() {
            var triangles = new List<int>();
          
            foreach (var face in Faces) {
                triangles.AddRange(face.Triangulate());
            }

            return triangles.ToArray();
        }
    }
}