using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;
using System.Collections;
using System.Linq;

[CustomEditor(typeof(PolygonBuilder))]
public class PolygonBuilderEditor : Editor {
    private string[] HIDDEN_PROPERTIES = { "radius", "size", "sizingMethod" };

    public override void OnInspectorGUI() {
        serializedObject.Update();

        var serializedProperty = serializedObject.GetIterator();
        if (serializedProperty.NextVisible(true)) {
            do {
                if (!HIDDEN_PROPERTIES.Contains(serializedProperty.name)) {
                    EditorGUILayout.PropertyField(serializedProperty);
                }
            } while(serializedProperty.NextVisible(false));
        }

        var sizingMethodProperty = serializedObject.FindProperty("sizingMethod");
        var sizing = (PolygonSizing)sizingMethodProperty.enumValueIndex;
        EditorGUILayout.PropertyField(sizingMethodProperty);

        switch (sizing) {
            case PolygonSizing.Bounds:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("size"));
                break;
            case PolygonSizing.Radius:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("radius"));
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}

public interface IMeshChangedHandler : IEventSystemHandler
{
    void OnMeshChanged();
}

public enum PolygonSizing {
    Radius, Bounds
};

[RequireComponent(typeof(MeshFilter))]
public class PolygonBuilder : MonoBehaviour
{
    public int vertexCount = 10;
    public float radius = 1;
    public Vector2 size;
    public Vector2 pivot = Vector2.zero;
    [Range(0, 360)]
    public float rotation = 0;
    public Vector2 scale = Vector2.one;
    public PolygonSizing sizingMethod = PolygonSizing.Radius;

    #region Messages

    void Reset()
    {
        size = Vector2.one * 2 * radius;
        BuildMesh();
    }

    void OnValidate()
    {
        if (vertexCount < 3) {
            vertexCount = 3;
        }

        BuildMesh();
    }

    #endregion

    void BuildMesh()
    {
        var filter = GetComponent<MeshFilter>();
        var mesh = new Mesh();

        mesh.vertices = TransformVertices(BuildVertices());
        mesh.triangles = BuildTriangles();
        mesh.RecalculateNormals();
        filter.sharedMesh = mesh;
        SendMeshChangedMessage();
    }

    Vector3[] BuildVertices()
    {
        var angleStep = -2 * Mathf.PI / vertexCount;
        var vertices = new Vector3[vertexCount];
        var rotationRadians = rotation * Mathf.Deg2Rad;

        for (var i=0; i<vertexCount; i++) {
            var t = i * angleStep;
            var x = Mathf.Cos(t + rotationRadians + Mathf.PI / 2);
            var y = Mathf.Sin(t + rotationRadians + Mathf.PI / 2);

            vertices[i] = new Vector3(x, y, 0);
        }

        return vertices;
    }

    int[] BuildTriangles()
    {
        var triangleCount = vertexCount - 2;
        var triangles = new int[triangleCount * 3];

        for (var i=0; i<triangleCount; i++) {
            var offset = i * 3;
            triangles[offset] = 0;
            triangles[offset + 1] = i + 1;
            triangles[offset + 2] = i + 2;
        }

        return triangles;
    }

    Vector3[] TransformVertices(Vector3[] vertices)
    {
        var bounds = CalculateBounds(vertices);
        var translation = CalculateTranslation();
        var scaleFactor = CalculateScale(vertices, bounds);

        if (sizingMethod == PolygonSizing.Bounds) {
            translation -= new Vector2(bounds.center.x * scaleFactor.x, bounds.center.y * scaleFactor.y);
        }

        var transform = Matrix4x4.TRS(translation, Quaternion.identity, scaleFactor);

        return TransformVertices(vertices, transform);
    }

    Vector2 CalculateTranslation()
    {
        var translation = pivot;
        var factor = sizingMethod == PolygonSizing.Bounds ? size : new Vector2(radius, radius);
        translation.Scale(factor);
        return translation;
    }

    Vector2 CalculateScale(Vector3[] vertices, Bounds bounds)
    {
        Vector2 scaleFactor;

        if (sizingMethod == PolygonSizing.Bounds) {
            scaleFactor = new Vector2(size.x / bounds.size.x, size.y / bounds.size.y);
        } else {
            scaleFactor = new Vector2(radius, radius);
        }

        scaleFactor.Scale(this.scale);
        return scaleFactor;
    }

    Bounds CalculateBounds(Vector3[] vertices)
    {
        Bounds bounds = new Bounds();
        foreach (Vector3 v in vertices) {
            bounds.Encapsulate(v);
        }
        return bounds;
    }

    Vector3[] TransformVertices(Vector3[] vertices, Matrix4x4 matrix)
    {
        return vertices.Select((v) => {
            return matrix.MultiplyPoint3x4(v);
        }).ToArray();
    }

    void SendMeshChangedMessage()
    {
        ExecuteEvents.Execute<IMeshChangedHandler>(this.gameObject, null, (component, e) => component.OnMeshChanged());
    }
}

