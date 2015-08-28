using UnityEngine;
using UnityEditor;
using System.Collections;

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
            var bounds = new Bounds(Editor.SelectedVertices[0], Vector3.zero);
            foreach (var vertex in Editor.SelectedVertices) {
                bounds.Encapsulate(vertex);
            }
            Handles.PositionHandle(bounds.center, Quaternion.identity);
        }
    }
}