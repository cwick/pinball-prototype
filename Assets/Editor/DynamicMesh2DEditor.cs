using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace DynamicMesh2D {
    enum MouseButton {
        LEFT = 0
    };
    
    class EditorComponent {
        private DynamicMesh2DEditor _editor;
        
        public EditorComponent(DynamicMesh2DEditor editor) {
            _editor = editor;
        }
        
        protected DynamicMesh2DEditor Editor {
            get { return _editor; }
        }
        
        public virtual bool ProcessSceneEvents() {
            return true;
        }
        
        public virtual void OnGUI() {}
        
        public virtual bool ShouldProcessEvent(Event e) {
            return GUIUtility.hotControl == 0;
        }
    }
    
    class EditModeComponent : EditorComponent {
        private bool _isEditMode;
        
        public EditModeComponent(DynamicMesh2DEditor editor) : base(editor) {
        }
        
        public override bool ProcessSceneEvents() {
            Tools.hidden = _isEditMode;
            if (!_isEditMode) {
                return false;
            }
            
            CaptureControlInput();
            return true;
        }
        
        public override void OnGUI() {
            ToggleEditModeFromButton();
        }
        
        public override bool ShouldProcessEvent(Event e) {
            return true;
        }
        
        private void ToggleEditModeFromButton() {
            _isEditMode = GUILayout.Toggle(_isEditMode, "Edit Mode", "Button");
        }
        
        private void CaptureControlInput() {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            HandleUtility.AddDefaultControl(controlID);
        }
    }
    
    class SingleVertexSelectComponent : EditorComponent {
        private bool _clickBegin = false;
        
        public SingleVertexSelectComponent(DynamicMesh2DEditor editor) : base(editor) {
        }
        
        public override bool ProcessSceneEvents() {
            if (Event.current.type == EventType.MouseDown) {
                _clickBegin = true;
            } else if (Event.current.type == EventType.MouseDrag) {
                _clickBegin = false;
            } else if (Event.current.type == EventType.MouseUp && _clickBegin) {
                _clickBegin = false;
                Editor.SelectedVertices = Editor.GetVerticesInRect(MouseClickRectangle);
                SceneView.RepaintAll();
            }
            
            return true;
        }
        
        public override bool ShouldProcessEvent(Event e) {
            return base.ShouldProcessEvent(e) && (MouseButton)e.button == MouseButton.LEFT && !e.alt;
        }
        
        private Rect MouseClickRectangle {
            get {
                float size = DrawVerticesComponent.VERTEX_HANDLE_SIZE * 2.5f;
                return new Rect(Event.current.mousePosition - Vector2.one * (size + 2) / 2.0f,
                                Vector2.one * size);
            }
        }
    }
    
    class BoxVertexSelectComponent : EditorComponent {
        private bool _isDragging;
        private Vector2? _dragStart;
        private Rect _selectionRectangle;
        
        public BoxVertexSelectComponent(DynamicMesh2DEditor editor) : base(editor) {
        }
        
        public override bool ProcessSceneEvents() {
            switch (Event.current.type) {
                case EventType.MouseDown:
                    _dragStart = Event.current.mousePosition;
                    _isDragging = false;
                    break;
                case EventType.MouseDrag:
                    if (_dragStart.HasValue) {
                        _selectionRectangle = new Rect(_dragStart.Value, Event.current.mousePosition - _dragStart.Value);
                        _isDragging = true;
                        SceneView.RepaintAll();
                    }
                    break;
                case EventType.Ignore:
                case EventType.MouseUp:
                    if (_isDragging) {
                        CompleteSelection();
                    }
                    break;
                case EventType.Repaint:
                    if (_isDragging) {
                        DynamicMesh2D.Utils.DrawSelectionRectangle(_selectionRectangle);
                    }
                    break;
                case EventType.KeyDown:
                    if (Event.current.keyCode == KeyCode.LeftAlt) {
                        Event.current.Use();
                    }
                    break;
            }
            
            return true;
        }
        
        public override bool ShouldProcessEvent(Event e) {
            return base.ShouldProcessEvent(e) && (MouseButton)e.button == MouseButton.LEFT && (_isDragging || !e.alt);
        }
        
        private void CompleteSelection() {
            Editor.SelectedVertices = Editor.GetVerticesInRect(_selectionRectangle);
            _isDragging = false;
            _dragStart = null;
        }
    }
    
    class DrawVerticesComponent : EditorComponent {
        public const int VERTEX_HANDLE_SIZE = 6;
        private readonly Color VERTEX_HANDLE_COLOR = Color.gray;
        private readonly Color VERTEX_HANDLE_SELECTED_COLOR = Color.yellow;
        
        public DrawVerticesComponent(DynamicMesh2DEditor editor) : base(editor) {
        }
        
        public override bool ProcessSceneEvents() {
            DrawVertexSelectionHandles();
            DrawSelectedVertices();
            
            return true;
        }
        
        public override bool ShouldProcessEvent(Event e) {
            return e.type == EventType.Repaint;
        }
        
        private void DrawSelectedVertices() {
            foreach (var vertex in Editor.SelectedVertices) {
                DrawVertexSelectionHandle(vertex, VERTEX_HANDLE_SELECTED_COLOR);
            }
        }
        
        private void DrawVertexSelectionHandles() {
            foreach (var localVertex in Editor.Mesh.vertices) {
                Vector3 worldVertex = Editor.MeshTransform.TransformPoint(localVertex);
                DrawVertexSelectionHandle(worldVertex, VERTEX_HANDLE_COLOR);
            }
        }
        
        private void DrawVertexSelectionHandle(Vector3 vertex, Color color) {
            DynamicMesh2D.Utils.DrawVertexHandle(vertex, color, VERTEX_HANDLE_SIZE);
        }
    }
    
    class TranslateVerticesComponent : EditorComponent {
        public TranslateVerticesComponent(DynamicMesh2DEditor editor) : base(editor) {
        }
        
        public override bool ProcessSceneEvents() {
            DrawVertexTranslationHandle();
            
            return true;
        }
        
        public override bool ShouldProcessEvent(Event e) {
            return Editor.SelectedVertices.Length > 0;
        }
        
        private void DrawVertexTranslationHandle() {
            var bounds = new Bounds(Editor.SelectedVertices[0], Vector3.zero);
            foreach (var vertex in Editor.SelectedVertices) {
                bounds.Encapsulate(vertex);
            }
            Handles.PositionHandle(bounds.center, Quaternion.identity);
        }
    }

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
