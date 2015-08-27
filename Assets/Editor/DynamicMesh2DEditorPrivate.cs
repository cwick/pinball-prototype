﻿using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace DynamicMesh2D {
    class DynamicMesh2DEditorPrivate {
        private bool _isEditMode = false;
        private bool _isDragging = false;
        private Mesh _mesh;
        private Transform _meshTransform;
        private Vector2? _dragStart;
        private Rect _selectionRectangle;
        private Vector3[] _selectedVertices = new Vector3[0];
        private const int LEFT_MOUSE_BUTTON = 0;
        private const int VERTEX_HANDLE_SIZE = 6;
        private readonly Color VERTEX_HANDLE_COLOR = Color.gray;
        private readonly Color VERTEX_HANDLE_SELECTED_COLOR = Color.yellow;

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
            Tools.hidden = _isEditMode;

            if (!_isEditMode) {
                return;
            }

            CaptureControlInput();

            switch (Event.current.type) {
                case EventType.MouseDown:
                    if (Event.current.button == LEFT_MOUSE_BUTTON && !Event.current.alt) {
                        _dragStart = Event.current.mousePosition;
                        _isDragging = false;
                    }
                    SceneView.RepaintAll();
                    break;
                case EventType.MouseDrag:
                    if (ShouldHandleMouseEvent) {
                        _selectionRectangle = new Rect(_dragStart.Value, Event.current.mousePosition - _dragStart.Value);
                        _isDragging = true;
                        SceneView.RepaintAll();
                    }
                    break;
                case EventType.MouseUp:
                    if (ShouldHandleMouseEvent) {
                        if (_isDragging) {
                            CompleteSelection(_selectionRectangle);
                        } else {
                            CompleteSelection(MouseClickRectangle);
                        }
                    }
                    break;
                case EventType.Ignore:
                    if (_isDragging) {
                        CompleteSelection(_selectionRectangle);
                    }
                    break;
                case EventType.Repaint:
                    DrawSceneGUI();
                    break;
            }

            DrawVertexTranslationHandle();
        }

        private void DrawSceneGUI() {
            foreach (var localVertex in _mesh.vertices) {
                Vector3 worldVertex = _meshTransform.TransformPoint(localVertex);
                DrawVertexSelectionHandle(worldVertex, VERTEX_HANDLE_COLOR);
            }

            if (_isDragging) { 
                DynamicMesh2D.Utils.DrawSelectionRectangle(_selectionRectangle);
            }

            DrawSelectedVertices();
        }

        private void CompleteSelection(Rect rectangle) {
            _selectedVertices = GetSelectedVertices(rectangle);
            _isDragging = false;
            _dragStart = null;
            SceneView.RepaintAll();
        }

        private void ToggleEditModeFromButton() {
            _isEditMode = GUILayout.Toggle(_isEditMode, "Edit Mode", "Button");
        }

        private void CaptureControlInput() {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            HandleUtility.AddDefaultControl(controlID);
        }

        private Vector3[] GetSelectedVertices(Rect rectangle) {
            var selected = new List<Vector3>();

            foreach (var localVertex in _mesh.vertices) {
                var worldVertex = _meshTransform.TransformPoint(localVertex);
                var guiVertex = HandleUtility.WorldToGUIPoint(worldVertex);

                if (rectangle.Contains(guiVertex, true)) {
                    selected.Add(worldVertex);
                }
            }

            return selected.ToArray();
        }

        private void DrawSelectedVertices() {
            foreach (var vertex in _selectedVertices) {
                DrawVertexSelectionHandle(vertex, VERTEX_HANDLE_SELECTED_COLOR);
            }
        }

        private void DrawVertexTranslationHandle() {
            if (_selectedVertices.Length == 0) {
                return;
            }
            var bounds = new Bounds(_selectedVertices[0], Vector3.zero);
            foreach (var vertex in _selectedVertices) {
                bounds.Encapsulate(vertex); 
            }
            Handles.PositionHandle(bounds.center, Quaternion.identity);
        }

        private void DrawVertexSelectionHandle(Vector3 vertex, Color color) {
            DynamicMesh2D.Utils.DrawVertexHandle(vertex, color, VERTEX_HANDLE_SIZE);
        }

        private bool ShouldHandleMouseEvent {
            get {
                return (Event.current.button == LEFT_MOUSE_BUTTON && GUIUtility.hotControl == 0 && _dragStart.HasValue);
            }
        }

        private Rect MouseClickRectangle {
            get {
                float size = VERTEX_HANDLE_SIZE * 2.5f;
                return new Rect(Event.current.mousePosition - Vector2.one * (size + 2) / 2.0f,
                                Vector2.one * size);
            }
        }
    }
}
