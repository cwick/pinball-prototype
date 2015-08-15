using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
public class CircleBuilder : MonoBehaviour
{
    public int vertexCount = 10;
    public float radius = 1;
    public Vector2 pivot = Vector2.zero;

    #region Messages

    void Reset()
    {
        BuildMesh();
    }

    void OnValidate()
    {
        if (vertexCount < 3) {
            vertexCount = 3;
        }

        BuildMesh();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.TransformPoint(Vector3.up * radius));
    }

    #endregion

    void BuildMesh()
    {
        var filter = GetComponent<MeshFilter>();
        var mesh = new Mesh();

        mesh.vertices = BuildVertices();
        mesh.triangles = BuildTriangles();
        filter.sharedMesh = mesh;
    }

    Vector3[] BuildVertices()
    {
        var angleStep = -2 * Mathf.PI / vertexCount;
        var vertices = new Vector3[vertexCount];

        for (var i=0; i<vertexCount; i++) {
            var t = i * angleStep;
            var x = radius * Mathf.Cos(t) + pivot.x;
            var y = radius * Mathf.Sin(t) + pivot.y;

            if (HasOddVertexCount) {
                vertices [i] = new Vector3(y, x, 0); 
            } else {
                vertices [i] = new Vector3(x, y, 0); 
            }
        }

        return vertices;
    }

    int[] BuildTriangles()
    {
        var triangleCount = vertexCount - 2;
        var triangles = new int[triangleCount * 3];

        for (var i=0; i<triangleCount; i++) {
            var offset = i * 3;
            triangles [offset] = 0;
            triangles [offset + 1] = HasOddVertexCount ? i + 2 : i + 1;
            triangles [offset + 2] = HasOddVertexCount ? i + 1 : i + 2;
        }

        return triangles;
    }

    bool HasOddVertexCount {
        get { return vertexCount % 2 != 0; }
    }
}
