using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace DynamicMesh2D {
    class ExtrudeToolComponent : EditorComponent {
        public ExtrudeToolComponent(DynamicMesh2DEditor editor) : base(editor) {
        }

        public override void OnGUI() {
            if (GUILayout.Button("Extrude")) {
                Extrude();
            }
        }

        private void Extrude() {
            var mesh = Editor.DynamicMesh;
            var selectedVertices = Editor.SelectedVertices.ToArray();
            if (selectedVertices.Count() != 2) {
                Debug.LogError("Must select two vertices to extrude");
                return;
            }

            var faces = mesh.Faces.Where( face => { 
                return selectedVertices.All( v => face.Vertices.Contains(v));
            } );

            if (faces.Count() != 1) {
                Debug.LogError("Selected vertices must belong to exactly one face");
                return;
            }

            var selectedFace = faces.First();
            var index = System.Array.IndexOf(selectedFace.Vertices, selectedVertices.First());

            var nextIndex = (index + 1) % selectedFace.Vertices.Length;
            var previousIndex = (index - 1) % selectedFace.Vertices.Length;

            if (selectedFace.Vertices[nextIndex] == selectedVertices.Last()) {
                selectedVertices = selectedVertices.Reverse().ToArray();
            } else if (selectedFace.Vertices[previousIndex] != selectedVertices.Last()) {
                Debug.LogError("Selected vertices must be connected by an edge");
                return;
            }

            Debug.Log("Extruding");

            var v1 = selectedVertices[0];
            var v2 = selectedVertices[1];
            mesh.Vertices.Add(mesh.Vertices[selectedVertices[1]]);
            var v3 = mesh.Vertices.Count() - 1;
            mesh.Vertices.Add(mesh.Vertices[selectedVertices[0]]);
            var v4 = mesh.Vertices.Count() - 1;

            Debug.Log(v1);
            Debug.Log(v2);
            Debug.Log(v3);
            Debug.Log(v4);

            var newFace = new Face();
            newFace.Vertices = new int[] { v1, v2, v3, v4};

            mesh.Faces.Add(newFace);

            // TODO: make method for this
            Editor.DynamicMeshComponent.Mesh = mesh;
            Editor.SelectedVertices = new HashSet<int>(new int[] { v3, v4 });
        }
    }
}