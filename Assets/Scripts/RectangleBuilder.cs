using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(MeshFilter))]
public class RectangleBuilder : MonoBehaviour
{
	public float width = 1;
	public float height = 1;
	public float thickness = 0.5f;

    #region Messages

	void OnValidate()
	{
		if (width < 0) {
			width = 0;
		}
		if (height < 0) {
			height = 0;
		}

		if (thickness > HalfHeight) {
			thickness = HalfHeight;
		}
		if (thickness > HalfWidth) {
			thickness = HalfWidth;
		}

		if (thickness < 0) {
			thickness = 0;
		}

		BuildMesh();
		BuildCollider();
	}

	#endregion

	float HalfWidth {
		get { return width / 2;}
	}

	float HalfHeight {
		get { return height / 2;}
	}

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
		var vertices = new Vector3[8] {
			new Vector3(-HalfWidth, -HalfHeight),
			new Vector3(-HalfWidth, HalfHeight),
			new Vector3(HalfWidth, HalfHeight),
			new Vector3(HalfWidth, -HalfHeight), 
			Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero
		};

		var translate = Vector2.one * thickness;
		vertices[4] = vertices[0] + new Vector3(translate.x, translate.y, 0);
		vertices[5] = vertices[1] + new Vector3(translate.x, -translate.y, 0);
		vertices[6] = vertices[2] + new Vector3(-translate.x, -translate.y, 0);
		vertices[7] = vertices[3] + new Vector3(-translate.x, translate.y, 0);

		return vertices;
	}

	int[] BuildTriangles()
	{
		var triangles = new List<int>();

		triangles.AddRange(TriangulateQuad(0, 1, 5, 4));
		triangles.AddRange(TriangulateQuad(1, 2, 6, 5));
		triangles.AddRange(TriangulateQuad(2, 3, 7, 6));
		triangles.AddRange(TriangulateQuad(3, 0, 4, 7));

		return triangles.ToArray();
	}

	void BuildCollider()
	{
		var collider = GetComponent<PolygonCollider2D>();
		var filter = GetComponent<MeshFilter>();
		if (!collider) {
			return;
		}

		var outerPath = new Vector2[4];
		var innerPath = new Vector2[4];
		var vertices = filter.sharedMesh.vertices;

		for (int i=0 ; i<outerPath.Length ; i++) {
			outerPath[i] = vertices[i];
		}
		for (int i=0 ; i<innerPath.Length ; i++) {
			innerPath[i] = vertices[i+4];
		}

		collider.pathCount = 2;
		collider.SetPath(0, outerPath);
		collider.SetPath(1, innerPath);
	}

	List<int> TriangulateQuad(int v0, int v1, int v2, int v3) {
		return new List<int>() { v0, v1, v2, v0, v2, v3 };
	}
}
