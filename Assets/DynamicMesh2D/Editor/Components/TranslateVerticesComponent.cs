using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace DynamicMesh2D {
    class TranslateVerticesComponent : EditorComponent {
        private bool _isTranslating;

        public TranslateVerticesComponent(DynamicMesh2DEditor editor) : base(editor) {
        }
        
        public override bool ProcessSceneEvents() {
            DrawVertexTranslationHandle();
            
            return true;
        }
        
        public override bool ShouldProcessEvent(Event e) {
            return Tools.current == Tool.Move && Editor.SelectedVertices.Count > 0;
        }
        
        private void DrawVertexTranslationHandle() {
            var selectedVertices = Editor.SelectedVerticesWorldPositions;
            var oldPosition = Utils.GetCenterPoint(selectedVertices);
            var newPosition = Handles.PositionHandle(oldPosition, Quaternion.identity);
            var delta = newPosition - oldPosition;

            ProcessTranslation(delta);
        }

        private void ProcessTranslation(Vector2 delta) {
            if (delta == Vector2.zero) {
                if (_isTranslating && GUIUtility.hotControl == 0) {
                    _isTranslating = false;
                    OnTranslationEnd();
                }
            } else {
                if (!_isTranslating) {
                    _isTranslating = true;
                    OnTranslationBegin();
                }

                OnTranslate(delta);
            }
        }

        private void OnTranslationBegin() {
            if (Event.current.shift) {
                ExtrudeVertices();
            }
        }

        private void OnTranslate(Vector2 delta) {
            RecordUndoForTranslatedVertexCount(Editor.SelectedVertices.Count());
            TranslateVertices(delta);
            Editor.SetMeshDirty();
        }

        private void OnTranslationEnd() {
        }

        private void TranslateVertices(Vector2 amount) {
            var mesh = Editor.DynamicMesh;

            foreach (var i in Editor.SelectedVertices) {
                var worldVertex = Editor.MeshTransform.TransformPoint(mesh.Vertices[i]);
                worldVertex += new Vector3(amount.x, amount.y);
                mesh.Vertices[i] = Editor.MeshTransform.InverseTransformPoint(worldVertex);
            }
        }

        private void ExtrudeVertices() {
            Editor.RecordUndoState("Extrude");
            var mesh = Editor.DynamicMesh;
            var selectedVertices = Editor.SelectedVertices.ToArray();
            if (selectedVertices.Count() != 2) {
//                Debug.LogError("Must select two vertices to extrude");
                return;
            }

            var faces = mesh.Faces.Where( face => { 
                return selectedVertices.All( v => face.Vertices.Contains(v));
            } );

            if (faces.Count() != 1) {
//                Debug.LogError("Selected vertices must belong to exactly one face");
                return;
            }

            var selectedFace = faces.First();
            var index = System.Array.IndexOf(selectedFace.Vertices, selectedVertices.First());

            var nextIndex = (index + 1) % selectedFace.Vertices.Length;
            var previousIndex = index == 0 ? selectedFace.Vertices.Length - 1 : (index - 1);

            if (selectedFace.Vertices[nextIndex] == selectedVertices.Last()) {
                selectedVertices = selectedVertices.Reverse().ToArray();
            } else if (selectedFace.Vertices[previousIndex] != selectedVertices.Last()) {
//                Debug.LogError("Selected vertices must be connected by an edge");
                return;
            }

            var v1 = selectedVertices[0];
            var v2 = selectedVertices[1];
            mesh.Vertices.Add(mesh.Vertices[selectedVertices[1]]);
            var v3 = mesh.Vertices.Count() - 1;
            mesh.Vertices.Add(mesh.Vertices[selectedVertices[0]]);
            var v4 = mesh.Vertices.Count() - 1;

            var newFace = new Face();
            newFace.Vertices = new int[] { v1, v2, v3, v4};

            mesh.Faces.Add(newFace);

            Editor.SetMeshDirty();
            Editor.SelectedVertices = new HashSet<int>(new int[] { v3, v4 });
        }

        private void RecordUndoForTranslatedVertexCount(int count) {
            var message = count == 1 ? "Translate Vertex" : "Translate Vertices";
            Editor.RecordUndoState(message);
        }
    }
}