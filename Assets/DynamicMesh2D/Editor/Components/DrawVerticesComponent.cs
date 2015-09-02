using UnityEngine;
using System.Collections;

namespace DynamicMesh2D {
    class DrawVerticesComponent : EditorComponent {
        public const int VERTEX_HANDLE_SIZE = 6;
        private readonly Color VERTEX_HANDLE_COLOR = Color.gray;
        private readonly Color VERTEX_HANDLE_SELECTED_COLOR = Color.yellow;
        
        public DrawVerticesComponent(DynamicMesh2DEditor editor) : base(editor) {
        }
        
        public override bool ProcessSceneEvents() {
            DrawVertexSelectionHandles();
            DrawSelectedVertices();
            
            return true;
        }
        
        public override bool ShouldProcessEvent(Event e) {
            return e.type == EventType.Repaint;
        }
        
        private void DrawSelectedVertices() {
            foreach (var vertex in Editor.SelectedVerticesWorldPositions) {
                DrawVertexSelectionHandle(vertex, VERTEX_HANDLE_SELECTED_COLOR);
            }
        }
        
        private void DrawVertexSelectionHandles() {
            foreach (var vertex in Editor.VerticesWorldPositions) {
                DrawVertexSelectionHandle(vertex, VERTEX_HANDLE_COLOR);
            }
        }
        
        private void DrawVertexSelectionHandle(Vector3 vertex, Color color) {
            Utils.DrawVertexHandle(vertex, color, VERTEX_HANDLE_SIZE);
        }
    }
} 