using UnityEngine;
using UnityEditor;
using System.Collections;

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
            mesh.Vertices = new Vector2[] { 
                new Vector2(0,0),
                new Vector2(1, -1),
                new Vector2(-1, -1),
                new Vector2(1, 1),
                new Vector2(2, 0)
            }; 
            var face1 = new Face();
            var face2 = new Face();
            face1.Vertices = new int[] { 0, 1, 2};
            face2.Vertices = new int[] { 0, 3, 4, 1 };

            mesh.Faces = new Face[] { face1, face2 };

            Editor.DynamicMesh.Mesh = mesh;
        }
    }
} 