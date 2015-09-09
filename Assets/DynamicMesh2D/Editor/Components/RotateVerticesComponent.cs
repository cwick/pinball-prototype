using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace DynamicMesh2D {
    class RotateVerticesComponent : EditorComponent {
        private bool _isRotating = false;
        private Quaternion _currentRotation = Quaternion.identity;
        private Vector3 _rotationCenter = Vector3.zero;
        private Dictionary<int, Vector3> _originalVertices = new Dictionary<int, Vector3>();

        public RotateVerticesComponent(DynamicMesh2DEditor editor) : base(editor) {
        }

        public override bool ProcessSceneEvents() {
            DrawVertexRotationHandle();
            
            return true;
        }

        public override bool ShouldProcessEvent(Event e) {
            return Tools.current == Tool.Rotate;
        }

        private void DrawVertexRotationHandle() { 
            var selectedVertices = Editor.SelectedVerticesWorldPositions;
            _rotationCenter = Utils.GetCenterPoint(selectedVertices);
            var newRotation = Handles.RotationHandle(_currentRotation, _rotationCenter);

            ProcessRotation(newRotation);
        }

        private void ProcessRotation(Quaternion newRotation) {
            if (newRotation == _currentRotation) {
                if (GUIUtility.hotControl == 0 && _isRotating) {
                    _isRotating = false;
                    OnRotationEnd();
                }
            } else {
                _currentRotation = newRotation;

                if (!_isRotating) {
                    _isRotating = true;
                    OnRotationBegin();
                }

                OnRotate(_currentRotation);
            }
        }

        private void OnRotationBegin() {
            var mesh = Editor.DynamicMesh;
            foreach (var i in Editor.SelectedVertices) {
                _originalVertices[i] = Editor.MeshTransform.TransformPoint(mesh.Vertices[i]) - _rotationCenter;
            }
        }

        private void OnRotate(Quaternion rotation) {
            Editor.RecordUndoState("Rotate Vertices");
            RotateVertices(rotation);
            Editor.SetMeshDirty();
        }

        private void RotateVertices(Quaternion rotation) {
            var mesh = Editor.DynamicMesh;
            foreach(var e in _originalVertices) {
                var vertex = e.Value;
                var i = e.Key;
                vertex = rotation * vertex;
                mesh.Vertices[i] = Editor.MeshTransform.InverseTransformPoint(vertex + _rotationCenter);
            }
        }

        private void OnRotationEnd() {
            _currentRotation = Quaternion.identity;
            _originalVertices = new Dictionary<int, Vector3>();
            _rotationCenter = Vector3.zero;
        }
    }
} 