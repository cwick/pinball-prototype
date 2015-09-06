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
    private DynamicMesh2D.DynamicMesh2D _dynamicMesh;

    [SerializeField]
    private int[] _selectedVerticesSerialized;

    private bool _isEditMode;

    public HashSet<int> SelectedVertices = new HashSet<int>();

    public bool IsEditMode {
        get { return _isEditMode; }
        set { _isEditMode = value; }
    }

    public DynamicMesh2D.DynamicMesh2D DynamicMesh {
        get { 
            return _dynamicMesh;
        }

        set {
            _dynamicMesh = value;
            BuildMesh();
        }
    }

    public Mesh Mesh {
        get { 
            return GetComponent<MeshFilter>().sharedMesh;
        }

        set {
            GetComponent<MeshFilter>().sharedMesh = value;
        }
    }

    public void BuildMesh() {
        var mesh = this.Mesh;
        if (mesh == null) {
            mesh = new Mesh();
        }

        _dynamicMesh.CopyToMesh(mesh);
        mesh.name = string.Format("DynamicMesh2D {0}", GetInstanceID());
        this.Mesh = mesh;

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

        collider.pathCount = _dynamicMesh.Faces.Count;

        for (int i=0 ; i<_dynamicMesh.Faces.Count ; i++) {
            var face = _dynamicMesh.Faces[i];
            var vertices = face.Vertices.Select( v => _dynamicMesh.Vertices[v] );
            collider.SetPath(i, vertices.ToArray());
        }
    }

}
