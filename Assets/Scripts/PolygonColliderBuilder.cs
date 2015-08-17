using UnityEngine;
using System.Collections;
using System.Linq;

public class PolygonColliderBuilder : MonoBehaviour, IMeshChangedHandler
{
    #region Messages

    void Reset()
    {
        OnValidate();
    }

    void OnValidate()
    {
        var filter = GetComponent<MeshFilter>();
        var mesh = filter.sharedMesh;
        var collider = GetComponent<PolygonCollider2D>();

        if (!collider || !mesh || !filter) {
            return;
        }

        var pathPoints = mesh.vertices.Select(v => new Vector2(v.x, v.y));
        collider.pathCount = 1;
        collider.SetPath(0, pathPoints.ToArray());
    }

    public void OnMeshChanged()
    { 
        OnValidate();
    }

    #endregion

}
