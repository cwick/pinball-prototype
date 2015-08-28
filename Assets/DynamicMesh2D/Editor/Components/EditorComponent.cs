using UnityEngine;
using System.Collections;

namespace DynamicMesh2D {
    enum MouseButton {
        LEFT = 0
    };
    
    class EditorComponent {
        private DynamicMesh2DEditor _editor;
        
        public EditorComponent(DynamicMesh2DEditor editor) {
            _editor = editor;
        }
        
        protected DynamicMesh2DEditor Editor {
            get { return _editor; }
        }
        
        public virtual bool ProcessSceneEvents() {
            return true;
        }
        
        public virtual void OnGUI() {}
        
        public virtual bool ShouldProcessEvent(Event e) {
            return GUIUtility.hotControl == 0;
        }
    }
}
