using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace DynamicMesh2D {
    class BuilderTestComponent : EditorComponent {
        public BuilderTestComponent(DynamicMesh2DEditor editor) : base(editor) {
        }

        public override void OnGUI() {
            if (GUILayout.Button("Build mesh")) {
                BuildMesh();
            }
        }

        void BuildMesh() {
            var mesh = new DynamicMesh2D();
            mesh.Vertices = new List<Vector2>() { 
                new Vector2(1,1),
                new Vector2(1, -1),
                new Vector2(-1, -1),
                new Vector2(-1, 1),
            }; 
            var face1 = new Face();
            face1.Vertices = new int[] { 0, 1, 2, 3};

            mesh.Faces = new List<Face>() { face1 };

            Editor.DynamicMeshComponent.DynamicMesh = mesh;
        }
    }
} 