using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class DynamicMesh2DComponent : MonoBehaviour {
    [HideInInspector]
    public bool ShouldDrawPivot = false;

    [SerializeField]
    private DynamicMesh2D.DynamicMesh2D _mesh;

    public DynamicMesh2D.DynamicMesh2D Mesh {
        get { 
            return _mesh;
        }

        set {
            _mesh = value;
            BuildMesh();
        }
    }

    public void BuildMesh() {
        GetComponent<MeshFilter>().sharedMesh = _mesh.BuildMesh();
    }

    void OnValidate() {
        BuildMesh();
    }

    void OnDrawGizmosSelected() {
        if (ShouldDrawPivot) {
            Gizmos.color = Color.yellow;
            var position = transform.position;
            var size = HandleUtility.GetHandleSize(position);
            Gizmos.DrawSphere(position, size/14);
        }
    }
}
