using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class DynamicMesh2DComponent : MonoBehaviour, ISerializationCallbackReceiver {
    [HideInInspector]
    public bool ShouldDrawPivot = false;

    [SerializeField]
    private DynamicMesh2D.DynamicMesh2D _mesh;

    [SerializeField]
    private int[] _selectedVerticesSerialized;

    public HashSet<int> SelectedVertices = new HashSet<int>();

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
        BuildCollider();
    }

    public void OnBeforeSerialize() {
        _selectedVerticesSerialized = SelectedVertices.ToArray();
    }

    public void OnAfterDeserialize() {
        SelectedVertices = new HashSet<int>(_selectedVerticesSerialized);
    }

    private void OnValidate() {
        BuildMesh();
    }

    private void OnDrawGizmosSelected() {
        if (ShouldDrawPivot) {
            Gizmos.color = Color.yellow;
            var position = transform.position;
            var size = HandleUtility.GetHandleSize(position);
            Gizmos.DrawSphere(position, size/14);
        }
    }

    private void BuildCollider() {
        var collider = GetComponent<PolygonCollider2D>();
        if (collider == null) {
            return;
        }

        collider.pathCount = _mesh.Faces.Count;

        for (int i=0 ; i<_mesh.Faces.Count ; i++) {
            var face = _mesh.Faces[i];
            var vertices = face.Vertices.Select( v => _mesh.Vertices[v] );
            collider.SetPath(i, vertices.ToArray());
        }
    }

}
