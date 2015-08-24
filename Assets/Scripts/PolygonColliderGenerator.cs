using UnityEngine;
using System.Collections;
using System.Linq;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(MeshFilter))]
public class PolygonColliderGenerator : MonoBehaviour, IMeshChangedHandler {
    #region Messages

    void Reset() {
        OnValidate();
    }

    void OnValidate() {
        var filter = GetComponent<MeshFilter>();
        var mesh = filter.sharedMesh;
        var collider = GetComponent<PolygonCollider2D>();

        if (!mesh) {
            return;
        }

        var pathPoints = mesh.vertices.Select(v => new Vector2(v.x, v.y));
        collider.pathCount = 1;
        collider.SetPath(0, pathPoints.ToArray());
    }

    public void OnMeshChanged() { 
        OnValidate();
    }

    #endregion

}
