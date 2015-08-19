using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.EventSystems;

[CustomEditor(typeof(TriangleBuilder))]
public class TriangleEditor : Editor
{
    private bool _editMode = false;

    public void OnSceneGUI()
    {
        if (!_editMode) {
            return;
        }

        Handles.color = Color.blue;

        var position = Vector3.zero;
        var size = HandleUtility.GetHandleSize(position) * 0.05f;
        var component = (Component)target;

        var meshFilter = component.GetComponent<MeshFilter>();
        var vertices = meshFilter.sharedMesh.vertices;

        for (int i=0 ; i<vertices.Length ; i++) {
            var vertexWorldSpace = component.transform.localToWorldMatrix.MultiplyPoint3x4(vertices[i]);
            vertices[i] = Handles.FreeMoveHandle(vertexWorldSpace, Quaternion.identity, size, Vector3.zero, Handles.DotCap);
            vertices[i] = component.transform.worldToLocalMatrix.MultiplyPoint3x4(vertices[i]);
        }

        meshFilter.sharedMesh.vertices = vertices;

        if (GUI.changed) {
            SendMeshChangedMessage(component.gameObject);
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        _editMode = GUILayout.Toggle(_editMode, "Edit Vertices", "Button");
        Tools.hidden = _editMode;
        SceneView.RepaintAll();
    }

    public void OnEnable()
    {
    }

    public void OnDisable()
    {
        Tools.hidden = false;
    }

    void SendMeshChangedMessage(GameObject gameObject)
    {
        ExecuteEvents.Execute<IMeshChangedHandler>(gameObject, null, (component, e) => component.OnMeshChanged());
    }
}

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TriangleBuilder : MonoBehaviour
{
    private MeshFilter _meshFilter;

    void Reset()
    {
        _meshFilter = GetComponent<MeshFilter>();
        BuildTriangle();
    }

    void OnValidate()
    {
    }

    void BuildTriangle()
    {
        var mesh = new Mesh();
        var vertices = new Vector3 [] {
            new Vector2(0, 0),
            new Vector2(0.5f, 1),
            new Vector2(1, 0)
        };
        var triangles = new int [] { 0,1,2 };
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        _meshFilter.sharedMesh = mesh;
    }
}
