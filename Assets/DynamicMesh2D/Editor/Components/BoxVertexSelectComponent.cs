﻿using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace DynamicMesh2D {
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
                        Utils.DrawSelectionRectangle(_selectionRectangle);
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
            IEnumerable<int> selectedVertices = Editor.GetVerticesInRect(_selectionRectangle);
            if (Event.current.shift) {
                selectedVertices = selectedVertices.Concat(Editor.SelectedVertices);
            }
            Editor.SelectedVertices = new HashSet<int>(selectedVertices);

            _isDragging = false;
            _dragStart = null;
        }
    }
}    
