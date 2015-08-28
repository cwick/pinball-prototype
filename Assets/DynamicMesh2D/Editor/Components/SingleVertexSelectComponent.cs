using UnityEngine;
using UnityEditor;
using System.Collections;

namespace DynamicMesh2D {
    class SingleVertexSelectComponent : EditorComponent {
        private bool _clickBegin = false;
        
        public SingleVertexSelectComponent(DynamicMesh2DEditor editor) : base(editor) {
        }
        
        public override bool ProcessSceneEvents() {
            if (Event.current.type == EventType.MouseDown) {
                _clickBegin = true;
            } else if (Event.current.type == EventType.MouseDrag) {
                _clickBegin = false;
            } else if (Event.current.type == EventType.MouseUp && _clickBegin) {
                _clickBegin = false;
                Editor.SelectedVertices = Editor.GetVerticesInRect(MouseClickRectangle);
                SceneView.RepaintAll();
            }
            
            return true;
        }
        
        public override bool ShouldProcessEvent(Event e) {
            return base.ShouldProcessEvent(e) && (MouseButton)e.button == MouseButton.LEFT && !e.alt;
        }
        
        private Rect MouseClickRectangle {
            get {
                float size = DrawVerticesComponent.VERTEX_HANDLE_SIZE * 2.5f;
                return new Rect(Event.current.mousePosition - Vector2.one * (size + 2) / 2.0f,
                                Vector2.one * size);
            }
        }
    }
}