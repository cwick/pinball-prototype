using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;

namespace DynamicMesh2D {
    class TranslateVerticesComponent : EditorComponent {
        public TranslateVerticesComponent(DynamicMesh2DEditor editor) : base(editor) {
        }
        
        public override bool ProcessSceneEvents() {
            DrawVertexTranslationHandle();
            
            return true;
        }
        
        public override bool ShouldProcessEvent(Event e) {
            return Editor.SelectedVertices.Length > 0;
        }
        
        private void DrawVertexTranslationHandle() {
            var selectedVertices = Editor.SelectedVerticesWorldPositions;
            var bounds = new Bounds(selectedVertices[0], Vector3.zero);

            foreach (var vertex in selectedVertices) {
                bounds.Encapsulate(vertex);
            }

            var oldPosition = bounds.center;
            var newPosition = Handles.PositionHandle(bounds.center, Quaternion.identity);
            var delta = newPosition - oldPosition;

            TranslateVertices(delta);
        }

        private void TranslateVertices(Vector3 amount) {
            var translatedVertices = Editor.VerticesWorldPositions.Select( (v) => v + amount );
            var mesh = Editor.Mesh;

            Undo.RecordObject(mesh, "Translate Vertices");

            mesh.vertices = translatedVertices.Select( (v) => Editor.MeshTransform.InverseTransformPoint(v) ).ToArray();
            mesh.RecalculateBounds();
        }
    }
}