using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace DynamicMesh2D {
    class SelectAllComponent : EditorComponent {
        public SelectAllComponent(DynamicMesh2DEditor editor) : base(editor) {
        }
        
        public override bool ProcessSceneEvents() {
            ToggleSelectionFromKeyboardShortcut();
            return true;
        }

        public void ToggleSelection() {
            if (Editor.SelectedVertices.Count > 0) {
                Editor.SelectedVertices = new HashSet<int>();
            } else {
                Editor.SelectedVertices = new HashSet<int>(Enumerable.Range(0, Editor.DynamicMesh.Vertices.Count));
            }
        }

        private void ToggleSelectionFromKeyboardShortcut() {
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.A) {
                ToggleSelection();
            }
        }
    }
}
    
