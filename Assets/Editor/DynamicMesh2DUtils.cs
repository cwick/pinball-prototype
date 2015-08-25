using UnityEngine;
using System.Collections;

namespace DynamicMesh2D {
    public static class Utils {
        private static Shader _simpleColorShader;
        private static Material _simpleColorMaterial;

        public static void DrawVertexHandle(Vector3 vertex) {
            Vector2 screenPoint = Camera.current.WorldToScreenPoint(vertex);
            DrawScreenRect(screenPoint);
        }

        private static void DrawScreenRect(Vector2 location) {
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
