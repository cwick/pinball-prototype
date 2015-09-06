using UnityEngine;
using UnityEditor;
using System.Collections;

namespace DynamicMesh2D {
    class EditModeComponent : EditorComponent {
        private bool _isEditMode;
        
        public EditModeComponent(DynamicMesh2DEditor editor) : base(editor) {
        }
        
        public override bool ProcessSceneEvents() {
            Tools.hidden = _isEditMode;
            Editor.DynamicMeshComponent.ShouldDrawPivot = _isEditMode;
            ToggleEditModeFromKeyboardShortcut();
            ToggleEditModeFromToolbar();

            if (!_isEditMode) {
                return false;
            }
            
            CaptureControlInput();
            return true;
        }
        
        public override void OnGUI() {
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
            var selectedIndex = _isEditMode ? 1 : 0;
            var width = GUILayout.Width(100);
            var options = new GUIContent[] { objectMode, editMode };

            _isEditMode = EditorGUILayout.Popup(selectedIndex, options, width) == 1;

            GUILayout.EndArea();
            Handles.EndGUI();
        }

        private void ToggleEditModeFromKeyboardShortcut() {
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Tab) {
                _isEditMode = !_isEditMode;
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
    
