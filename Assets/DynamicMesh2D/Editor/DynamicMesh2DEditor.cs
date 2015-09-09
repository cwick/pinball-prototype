using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace DynamicMesh2D {
    [CustomEditor(typeof(DynamicMesh2DComponent))]
    class DynamicMesh2DEditor : Editor {
        private Transform _meshTransform;
        private EditorComponent[] _editorComponents;
        private bool _shouldPreserveEditMode;

        public bool IsEditMode {
            get { return DynamicMeshComponent.IsEditMode; }
            set { DynamicMeshComponent.IsEditMode = value; }
        }

        public HashSet<int> SelectedVertices {
            get { return DynamicMeshComponent.SelectedVertices; }
            set {
                DynamicMeshComponent.SelectedVertices = value;
                SceneView.RepaintAll();
            }
        }

        public Vector3[] SelectedVerticesLocalPositions {
            get { 
                var vertices = DynamicMesh.Vertices;
                return SelectedVertices.Select( (i) => (Vector3)vertices[i] ).ToArray();
            }
        }

        public Vector3[] SelectedVerticesWorldPositions {
            get {
                var localVertices = SelectedVerticesLocalPositions;
                return localVertices.Select( (v) => MeshTransform.TransformPoint(v) ).ToArray();
            }
        }

        public Vector3[] VerticesWorldPositions {
            get {
                return DynamicMesh.Vertices.Select( (v) => _meshTransform.TransformPoint(v) ).ToArray();
            }
        }

        public Vector2[] VerticesGUIPositions {
            get {
                return DynamicMesh.Vertices.Select( (v) => HandleUtility.WorldToGUIPoint(_meshTransform.TransformPoint(v)) ).ToArray();
            }
        }

        public Transform MeshTransform {
            get { return _meshTransform; }
        }

        public DynamicMesh2D DynamicMesh {
            get { return DynamicMeshComponent.DynamicMesh; }
        }

        public DynamicMesh2DComponent DynamicMeshComponent {
            get { return _meshTransform.GetComponent<DynamicMesh2DComponent>(); }
        }

        public override void OnInspectorGUI() {
            foreach (var component in _editorComponents) {
                component.OnGUI();
            }
        }
        
        public void OnEnable() {
            var gameObject = ((Component)target).gameObject;
            _meshTransform = gameObject.transform;

            _editorComponents = new EditorComponent[] {
                new EditModeComponent(this),
                new DrawVerticesComponent(this),
                new SingleVertexSelectComponent(this),
                new BoxVertexSelectComponent(this),
                new TranslateVerticesComponent(this),
                new ScaleVerticesComponent(this),
                new BuilderTestComponent(this),
                new SelectAllComponent(this)
            };

            Undo.undoRedoPerformed += UndoRedoCallback;
        }

        public void OnDisable() {
            if (_shouldPreserveEditMode) {
                _shouldPreserveEditMode = false;
            } else {
                IsEditMode = false;
            }

            Undo.undoRedoPerformed -= UndoRedoCallback;
        }

        public void OnSceneGUI() {
            foreach (var component in _editorComponents) {
                if (component.ShouldProcessEvent(Event.current) && !component.ProcessSceneEvents()) {
                    break;
                }
            }
        }
        
        public HashSet<int> GetVerticesInRect(Rect rectangle) {
            var selected = new HashSet<int>();

            int i=0;
            foreach (var vertex in VerticesGUIPositions) {
                if (rectangle.Contains(vertex, true)) {
                    selected.Add(i);
                }

                i++;
            }

            return selected;
        }

        public int? GetClosestVertexInRect(Rect rectangle) {
            int i=0;
            float minDistance = float.MaxValue;
            int? closestVertex = null;

            foreach (var vertex in VerticesGUIPositions) { 
                if (rectangle.Contains(vertex, true)) {
                    var distance = Mathf.Min(Vector2.Distance(vertex, rectangle.center), minDistance);
                    if (distance < minDistance) {
                        minDistance = distance;
                        closestVertex = i;
                    }
                }

                i++;
            }

            return closestVertex;
        }

        public void SetMeshDirty() {
            EditorUtility.SetDirty(target);
            DynamicMeshComponent.BuildMesh();
        }

        public void RecordUndoState(string command) {
            Undo.RegisterCompleteObjectUndo(DynamicMeshComponent, command);
        }

        private void UndoRedoCallback() {
            _shouldPreserveEditMode = true;
        }
    }
}
