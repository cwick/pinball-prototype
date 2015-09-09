using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace DynamicMesh2D {
    class ScaleVerticesComponent : EditorComponent {
        private bool _isScaling = false;
        private Vector3 _currentScale = Vector3.one;
        private Vector3 _scaleCenter = Vector3.zero;
        private Dictionary<int, Vector3> _originalVertices = new Dictionary<int, Vector3>();

        public ScaleVerticesComponent(DynamicMesh2DEditor editor) : base(editor) {
        }

        public override bool ProcessSceneEvents() {
            DrawVertexScaleHandle();
            
            return true;
        }

        public override bool ShouldProcessEvent(Event e) {
            return Tools.current == Tool.Scale;
        }

        private void DrawVertexScaleHandle() { 
            var selectedVertices = Editor.SelectedVerticesWorldPositions;
            _scaleCenter = Utils.GetCenterPoint(selectedVertices);
            var newScale = Handles.ScaleHandle(_currentScale, _scaleCenter, Quaternion.identity, HandleUtility.GetHandleSize(_scaleCenter));

            ProcessScale(newScale);
        }

        private void ProcessScale(Vector3 newScale) {
            if (newScale == _currentScale) {
                if (GUIUtility.hotControl == 0 && _isScaling) {
                    _isScaling = false;
                    OnScaleEnd();
                }
            } else {
                _currentScale = newScale;

                if (!_isScaling) {
                    _isScaling = true;
                    OnScaleBegin();
                }

                OnScale(_currentScale);
            }
        }

        private void OnScaleBegin() {
            var mesh = Editor.DynamicMesh;
            foreach (var i in Editor.SelectedVertices) {
                _originalVertices[i] = Editor.MeshTransform.TransformPoint(mesh.Vertices[i]) - _scaleCenter;
            }
        }

        private void OnScale(Vector2 scale) {
            Editor.RecordUndoState("Scale Vertices");
            ScaleVertices(scale);
            Editor.SetMeshDirty();
        }

        private void ScaleVertices(Vector2 scale) {
            var mesh = Editor.DynamicMesh;
            foreach(var e in _originalVertices) {
                var vertex = e.Value;
                var i = e.Key;
                vertex.Scale(scale);
                mesh.Vertices[i] = Editor.MeshTransform.InverseTransformPoint(vertex + _scaleCenter);
            }
        }

        private void OnScaleEnd() {
            _currentScale = Vector3.one;
            _originalVertices = new Dictionary<int, Vector3>();
            _scaleCenter = Vector3.zero;
        }
    }
} 