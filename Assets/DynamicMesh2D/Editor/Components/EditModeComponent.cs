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

            if (!_isEditMode) {
                return false;
            }
            
            CaptureControlInput();
            return true;
        }
        
        public override void OnGUI() {
            if (Editor.Mesh != null) {
                ToggleEditModeFromButton();
            }
        }
        
        public override bool ShouldProcessEvent(Event e) {
            return true;
        }
        
        private void ToggleEditModeFromButton() {
            _isEditMode = GUILayout.Toggle(_isEditMode, "Edit Mode", "Button");
            SceneView.RepaintAll();
        }
        
        private void CaptureControlInput() {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            HandleUtility.AddDefaultControl(controlID);
        }
    }
}
    
