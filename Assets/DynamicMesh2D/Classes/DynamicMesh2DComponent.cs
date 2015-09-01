using UnityEngine;
using UnityEditor;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
public class DynamicMesh2DComponent : MonoBehaviour {
    [HideInInspector]
    public bool ShouldDrawPivot = false;

    void OnDrawGizmosSelected() {
        if (ShouldDrawPivot) {
            Gizmos.color = Color.yellow;
            var position = transform.position;
            var size = HandleUtility.GetHandleSize(position);
            Gizmos.DrawSphere(position, size/14);
        }
    }
}
