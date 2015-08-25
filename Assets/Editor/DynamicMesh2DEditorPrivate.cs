using UnityEngine;
using UnityEditor;
using System.Collections;

namespace DynamicMesh2D {
    class DynamicMesh2DEditorPrivate {
        private bool _isEditMode = false;
        private Mesh _mesh;
        private Transform _meshTransform;

        public DynamicMesh2DEditorPrivate(Mesh mesh, Transform transform) {
            _mesh = mesh;
            _meshTransform = transform;
        }

        public void OnGUI() {
            ToggleEditModeFromButton();
        }

        public void OnEnable() {
        }

        public void OnDestroy() {
        }

        public void OnDisable() {
        }

        public void OnSceneGUI() {
            if (_isEditMode) {
                StealMouseInput();

                DrawSceneGUI();
            }

            Tools.hidden = _isEditMode;
        }

        private void DrawSceneGUI() {
            foreach (var localVertex in _mesh.vertices) {
                Vector3 worldVertex = _meshTransform.localToWorldMatrix.MultiplyPoint3x4(localVertex);
                DynamicMesh2D.Utils.DrawVertexHandle(worldVertex);
            }
        }

        private void ToggleEditModeFromButton() {
            _isEditMode = GUILayout.Toggle(_isEditMode, "Edit Mode", "Button");
        }

        private void StealMouseInput() {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            HandleUtility.AddDefaultControl(controlID);
        }
    }
}
