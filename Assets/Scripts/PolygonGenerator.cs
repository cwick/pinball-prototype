using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public interface IMeshChangedHandler : IEventSystemHandler
{
    void OnMeshChanged();
}

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class PolygonGenerator : MonoBehaviour
{
    public int vertexCount = 3;
    private MeshFilter _meshFilter { get { return GetComponent<MeshFilter>(); } }

    void OnValidate()
    {
        if (vertexCount < 3) {
            vertexCount = 3;
        }
    }

#if UNITY_EDITOR
    public void Generate()
    {
        UnityEditor.Undo.RecordObject(_meshFilter, "Generate Mesh");
        GenerateMesh();
    }
#endif

    void GenerateMesh()
    {
        var mesh = new Mesh();
        mesh.vertices = BuildVertices();
        mesh.triangles = BuildTriangles();
        mesh.RecalculateNormals();

        _meshFilter.sharedMesh = mesh;

        SendMeshChangedMessage(gameObject);
    }

    Vector3[] BuildVertices()
    {
        var angleStep = -2 * Mathf.PI / vertexCount;
        var vertices = new Vector3[vertexCount];
        var rotation = (vertexCount % 2 == 0) ? 0 : Mathf.PI / 2;

        for (var i=0; i<vertexCount; i++) {
            var t = i * angleStep;
            var x = Mathf.Cos(t + rotation);
            var y = Mathf.Sin(t + rotation);

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

    void SendMeshChangedMessage(GameObject gameObject)
    {
        ExecuteEvents.Execute<IMeshChangedHandler>(gameObject, null, (component, e) => component.OnMeshChanged());
    }
}
