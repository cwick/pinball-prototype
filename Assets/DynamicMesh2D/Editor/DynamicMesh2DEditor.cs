using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace DynamicMesh2D {
    [CustomEditor(typeof(DynamicMesh2DComponent))]
    class DynamicMesh2DEditor : Editor {
        private Mesh _mesh;
        private Transform _meshTransform;
        private EditorComponent[] _editorComponents;
        private Vector3[] _selectedVertices = new Vector3[0];
        
        public Vector3[] SelectedVertices {
            get { return _selectedVertices; }
            set {
                _selectedVertices = value;
                SceneView.RepaintAll();
            }
        }
        
        public Mesh Mesh {
            get { return _mesh; }
        }
        
        public Transform MeshTransform {
            get { return _meshTransform; }
        }
        
        public override void OnInspectorGUI() {
            foreach (var component in _editorComponents) {
                component.OnGUI();
            }
        }
        
        public void OnEnable() {
            var gameObject = ((Component)target).gameObject;
            _mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
            _meshTransform = gameObject.transform;

            _editorComponents = new EditorComponent[] {
                new EditModeComponent(this),
                new DrawVerticesComponent(this),
                new SingleVertexSelectComponent(this),
                new BoxVertexSelectComponent(this),
                new TranslateVerticesComponent(this)
            };
        }
        
        public void OnSceneGUI() {
            foreach (var component in _editorComponents) {
                if (component.ShouldProcessEvent(Event.current) && !component.ProcessSceneEvents()) {
                    break;
                }
            }
        }
        
        public Vector3[] GetVerticesInRect(Rect rectangle) {
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
    }
}
