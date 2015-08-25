using UnityEngine;
using UnityEditor;
using System.Collections;

namespace DynamicMesh2D {
    [CustomEditor(typeof(DynamicMesh2DComponent))]
    [RequireComponent(typeof(MeshFilter))]
    public class DynamicMesh2DEditor : Editor {
        private DynamicMesh2DEditorPrivate _editor;

        public override void OnInspectorGUI() {
            PrivateEditor.OnGUI(); 
        }

        private void OnSceneGUI() {
            PrivateEditor.OnSceneGUI();
        }

        private void OnDestroy() {
            PrivateEditor.OnDestroy();
        }

        private void OnDisable() {
            PrivateEditor.OnDisable();
        }

        private void OnEnable() {
            PrivateEditor.OnEnable();
        }

        private DynamicMesh2DEditorPrivate PrivateEditor {
            get { 
                if (_editor == null) {
                    _editor = new DynamicMesh2DEditorPrivate(Mesh, DynamicMesh2DComponent.transform);
                }
                return _editor;
            }
        }

        private Mesh Mesh {
            get { return DynamicMesh2DComponent.GetComponent<MeshFilter>().sharedMesh; }
        }

        private DynamicMesh2DComponent DynamicMesh2DComponent {
            get { return (DynamicMesh2DComponent)target; }
        }
    }
}
