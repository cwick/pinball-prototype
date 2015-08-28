﻿using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;

namespace DynamicMesh2D {
    class TranslateVerticesComponent : EditorComponent {
        public TranslateVerticesComponent(DynamicMesh2DEditor editor) : base(editor) {
        }
        
        public override bool ProcessSceneEvents() {
            DrawVertexTranslationHandle();
            
            return true;
        }
        
        public override bool ShouldProcessEvent(Event e) {
            return Editor.SelectedVertices.Count > 0;
        }
        
        private void DrawVertexTranslationHandle() {
            var selectedVertices = Editor.SelectedVerticesWorldPositions;
            var oldPosition = GetCenterOfFace(selectedVertices);
            var newPosition = Handles.PositionHandle(oldPosition, Quaternion.identity);
            var delta = newPosition - oldPosition;

            if (delta != Vector3.zero) {
                TranslateVertices(delta);
            }
        }

        private void TranslateVertices(Vector2 amount) {
            var mesh = Editor.Mesh;
            var vertices = Editor.VerticesWorldPositions;

            foreach (var i in Editor.SelectedVertices) {
                vertices[i] += new Vector3(amount.x, amount.y, 0);
            }

            RecordUndoForTranslatedVertexCount(mesh, Editor.SelectedVertices.Count);

            mesh.vertices = Editor.WorldVerticesToLocalVertices(vertices);
            mesh.RecalculateBounds();
        }

        private void RecordUndoForTranslatedVertexCount(Mesh mesh, int count) {
            var message = count == 1 ? "Translate Vertex" : "Translate Vertices";
            Undo.RecordObject(mesh, message);
        }

        private Vector3 GetCenterOfFace(Vector3[] vertices) {
            return vertices.Aggregate(Vector3.zero, (v1, v2) => v1 + v2) / vertices.Length;
        }
    }
}