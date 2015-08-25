using UnityEngine;
using UnityEditor;
using System.Collections;

namespace DynamicMesh2D {
    class DynamicMesh2DEditorPrivate {
        private bool _isEditMode = false;
        private bool _isDragging = false;
        private Mesh _mesh;
        private Transform _meshTransform;
        private Vector2 _dragBegin;

        private const int LEFT_MOUSE_BUTTON = 0;

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

                switch (Event.current.type) {
                    case EventType.MouseDown:
                        if (Event.current.button == LEFT_MOUSE_BUTTON) {
                            Event.current.Use();
                            _dragBegin = Event.current.mousePosition;
                        }
                        break;
                    case EventType.MouseDrag:
                        if (Event.current.button == LEFT_MOUSE_BUTTON) {
                            Event.current.Use();
                            _isDragging = true;
                        }
                        break;
                    case EventType.MouseUp:
                        if (Event.current.button == LEFT_MOUSE_BUTTON) {
                            _isDragging = false;
                            SceneView.RepaintAll();
                        }
                        break;
                    case EventType.Repaint:
                        if (_isDragging) { 
                            Rect selectionRectangle = new Rect(_dragBegin, Event.current.mousePosition - _dragBegin);
                            DynamicMesh2D.Utils.DrawSelectionRectangle(selectionRectangle);
                        }
                        break;
                }

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
