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

                foreach (var vertex in _mesh.vertices) {
                    DrawVertex(vertex);
                }
//                Handles.BeginGUI();
//                
//                DrawSceneGUI();
//                
//                Handles.EndGUI();
            }
            
            Tools.hidden = _isEditMode;
        }
        
        private void DrawSceneGUI() {
            int vertexHandleSize = 6;
            Vector2 vertexHandleOffset = Vector2.one * (-vertexHandleSize/2) + new Vector2(2, -2);

            foreach (Vector3 vertex in _mesh.vertices) {
                Vector3 worldVertex = _meshTransform.localToWorldMatrix.MultiplyPoint3x4(vertex);
                Vector2 guiPoint = HandleUtility.WorldToGUIPoint(worldVertex) + vertexHandleOffset;
                var rect = new Rect(guiPoint, Vector2.one * vertexHandleSize);
                EditorGUI.DrawRect(rect, Color.gray);
            }
        }

        private void DrawVertex(Vector3 vertex) {
            Vector3 worldVertex = _meshTransform.localToWorldMatrix.MultiplyPoint3x4(vertex);
            Vector2 screenPoint = Camera.current.WorldToScreenPoint(worldVertex);
            DrawRect(screenPoint);
        }

        private void DrawRect(Vector2 point) {
            var shader = Shader.Find("Hidden/Internal-Colored");
            var material = new Material(shader);
            material.hideFlags = HideFlags.HideAndDontSave;
            material.SetInt("_ZWrite", 0);
            material.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);

            material.SetPass(0);
            GL.PushMatrix();

            GL.LoadPixelMatrix();
            GL.Begin(GL.QUADS);
            GL.Color(Color.red);

            GL.Vertex3(point.x - 5,point.y-5,0);
            GL.Vertex3(point.x+5,point.y-5,0);
            GL.Vertex3(point.x+5,point.y+5,0);
            GL.Vertex3(point.x-5,point.y+5,0);

            GL.End();

            GL.PopMatrix();
        }
        
        private void OnSelectionChange() {
            _isEditMode = false;
        }
        
        private MeshFilter SelectedObject {
            get {
                var selectedObjects = Selection.GetFiltered(typeof(MeshFilter), SelectionMode.Editable | SelectionMode.ExcludePrefab);
                MeshFilter selectedObject = null;
                if (selectedObjects.Length == 1) {
                    selectedObject = (MeshFilter)selectedObjects[0];
                }
                return selectedObject;
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