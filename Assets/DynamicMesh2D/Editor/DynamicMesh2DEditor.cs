﻿using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace DynamicMesh2D {
    [CustomEditor(typeof(DynamicMesh2DComponent))]
    class DynamicMesh2DEditor : Editor {
        private Mesh _mesh;
        private Transform _meshTransform;
        private EditorComponent[] _editorComponents;
        private HashSet<int> _selectedVertices = new HashSet<int>();
        
        public HashSet<int> SelectedVertices {
            get { return _selectedVertices; }
            set {
                _selectedVertices = value;
                SceneView.RepaintAll();
            }
        }

        public Vector3[] SelectedVerticesLocalPositions {
            get { 
                var vertices = Mesh.vertices;
                return _selectedVertices.Select( (i) => vertices[i] ).ToArray();
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
                return Mesh.vertices.Select( (v) => _meshTransform.TransformPoint(v) ).ToArray();
            }
        }

        public Vector2[] VerticesGUIPositions {
            get {
                return Mesh.vertices.Select( (v) => HandleUtility.WorldToGUIPoint(_meshTransform.TransformPoint(v)) ).ToArray();
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

        public Vector3[] WorldVerticesToLocalVertices(Vector3[] worldVertices) {
            return worldVertices.Select( (v) => MeshTransform.InverseTransformPoint(v) ).ToArray();
        }
    }
}
