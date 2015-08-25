using UnityEngine;
using UnityEditor;
using System.Collections;

namespace DynamicMesh2D {
    public class DynamicMesh2DEditorWindow : EditorWindow {
//        [MenuItem("Pinball/Dynamic Mesh 2D")]
//        public static void ShowWindow() {
//            EditorWindow.GetWindow(typeof(DynamicMesh2DEditorWindow));
//        }

        private void OnGUI() {
        }

        private void OnEnable() {
            SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
            SceneView.onSceneGUIDelegate += this.OnSceneGUI;
        }

        private void OnDestroy() {
            SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
        }

        private void OnSceneGUI(SceneView view) {
        }

        private void DrawSceneGUI() {
        }

        private void OnSelectionChange() {
        }
    }
}