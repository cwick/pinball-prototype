using UnityEngine;
using UnityEditor;
using System.Collections;

namespace DynamicMesh2D {
    class EditModeComponent : EditorComponent {
        public EditModeComponent(DynamicMesh2DEditor editor) : base(editor) {
        }
        
        public override bool ProcessSceneEvents() {
            Tools.hidden = Editor.IsEditMode;
            Editor.DynamicMeshComponent.ShouldDrawPivot = Editor.IsEditMode;
            ToggleEditModeFromKeyboardShortcut();
            ToggleEditModeFromToolbar();

            if (!Editor.IsEditMode) {
                return false;
            }
            
            CaptureControlInput();
            return true;
        }
        
        public override bool ShouldProcessEvent(Event e) {
            return true;
        }
        
        private void ToggleEditModeFromToolbar() {
            var height = 60;
            var tooltip = "Sets the object interaction mode\nShortcut: TAB";

            Handles.BeginGUI();
            GUILayout.BeginArea(new Rect(0, Screen.height - height, Screen.width, height));

            var objectMode = new GUIContent("Object Mode", tooltip);
            var editMode = new GUIContent("Edit Mode", tooltip);
            var selectedIndex = Editor.IsEditMode ? 1 : 0;
            var width = GUILayout.Width(100);
            var options = new GUIContent[] { objectMode, editMode };

            Editor.IsEditMode = EditorGUILayout.Popup(selectedIndex, options, width) == 1;

            GUILayout.EndArea();
            Handles.EndGUI();
        }

        private void ToggleEditModeFromKeyboardShortcut() {
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Tab) {
                Editor.IsEditMode = !Editor.IsEditMode;
                Event.current.Use();
                SceneView.RepaintAll();
            }
        }
        
        private void CaptureControlInput() {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            HandleUtility.AddDefaultControl(controlID);
        }
    }
}
    
