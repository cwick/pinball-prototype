using UnityEngine;
using UnityEditor;
using System.Collections;

namespace DynamicMesh2D {
    public static class Utils {
        private static Shader _simpleColorShader;
        private static Material _simpleColorMaterial;

        public static void DrawVertexHandle(Vector3 vertex) {
            Vector2 screenPoint = Camera.current.WorldToScreenPoint(vertex);
            DrawScreenVertexHandle(screenPoint);
        }

        public static void DrawSelectionRectangle(Rect selection) {
            var style = new GUIStyle ("SelectionRect");

            // GUI doesn't like negative widths or heights
            if (selection.width < 0) {
                selection.width *= -1;
                selection.position = new Vector2(selection.position.x - selection.width, selection.position.y);
            }
            if (selection.height < 0) {
                selection.height *= -1;
                selection.position = new Vector2(selection.position.x, selection.position.y - selection.height);
            }

            Handles.BeginGUI();
            style.Draw(selection, GUIContent.none, false, false, false, false);
            Handles.EndGUI();
        }

        private static void DrawScreenVertexHandle(Vector2 location) {
            var handleSize = 6;
            var halfSize = handleSize / 2;

            SimpleColorMaterial.SetPass(0);

            GL.PushMatrix();

            GL.LoadPixelMatrix();
            GL.Begin(GL.QUADS);
            GL.Color(Color.gray);

            GL.Vertex3(location.x - halfSize, location.y - halfSize, 0);
            GL.Vertex3(location.x + halfSize, location.y - halfSize, 0);
            GL.Vertex3(location.x + halfSize, location.y + halfSize, 0);
            GL.Vertex3(location.x - halfSize, location.y + halfSize, 0);

            GL.End();

            GL.PopMatrix();
        }

        private static Shader SimpleColorShader {
            get {
                if (_simpleColorShader == null) {
                    _simpleColorShader = Shader.Find("Hidden/Internal-Colored");
                }

                return _simpleColorShader;
            }
        }

        private static Material SimpleColorMaterial {
            get {
                if (_simpleColorMaterial == null) {
                    _simpleColorMaterial = new Material(SimpleColorShader);
                    _simpleColorMaterial.hideFlags = HideFlags.HideAndDontSave;
                    _simpleColorMaterial.SetInt("_ZWrite", 0);
                    _simpleColorMaterial.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
                }

                return _simpleColorMaterial;
            }
        }
    }
}
