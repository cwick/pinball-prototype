using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.EventSystems;

[CustomEditor(typeof(MeshFilter))]
public class PolygonEditor : Editor
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

        for (int i=0; i<vertices.Length; i++) {
            var vertexWorldSpace = component.transform.localToWorldMatrix.MultiplyPoint3x4(vertices[i]);
            vertices[i] = Handles.FreeMoveHandle(vertexWorldSpace, Quaternion.identity, size, Vector3.zero, Handles.DotCap);
            vertices[i] = component.transform.worldToLocalMatrix.MultiplyPoint3x4(vertices[i]);
        }

        meshFilter.sharedMesh.vertices = vertices;

        if (GUI.changed) {
            meshFilter.sharedMesh.RecalculateBounds();
            meshFilter.sharedMesh.RecalculateNormals();
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

    public void OnDisable()
    {
        Tools.hidden = false;
    }

    void SendMeshChangedMessage(GameObject gameObject)
    {
        ExecuteEvents.Execute<IMeshChangedHandler>(gameObject, null, (component, e) => component.OnMeshChanged());
    }
}

