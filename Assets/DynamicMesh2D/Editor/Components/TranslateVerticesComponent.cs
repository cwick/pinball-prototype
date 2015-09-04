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
            return Editor.SelectedVertices.Count > 0;
        }
        
        private void DrawVertexTranslationHandle() {
            var selectedVertices = Editor.SelectedVerticesWorldPositions;
            var oldPosition = GetCenterOfFace(selectedVertices);
            var newPosition = Handles.PositionHandle(oldPosition, Quaternion.identity);
            var delta = newPosition - oldPosition;

            if (delta != Vector3.zero) {
                RecordUndoForTranslatedVertexCount(selectedVertices.Count());

                TranslateVertices(delta);
                Editor.SetMeshDirty();
            }
        }

        private void TranslateVertices(Vector2 amount) {
            var mesh = Editor.DynamicMesh;

            foreach (var i in Editor.SelectedVertices) {
                var worldVertex = Editor.MeshTransform.TransformPoint(mesh.Vertices[i]);
                worldVertex += new Vector3(amount.x, amount.y);
                mesh.Vertices[i] = Editor.MeshTransform.InverseTransformPoint(worldVertex);
            }
        }

        private Vector3 GetCenterOfFace(Vector3[] vertices) {
            return vertices.Aggregate(Vector3.zero, (v1, v2) => v1 + v2) / vertices.Length;
        }

        private void RecordUndoForTranslatedVertexCount(int count) {
            var message = count == 1 ? "Translate Vertex" : "Translate Vertices";
            Editor.RecordUndoState(message);
        }
    }
}