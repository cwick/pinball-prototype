using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(FlipperController))]
public class FlipperControllerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		if (GUILayout.Button("Flip")) {
			var controller = (FlipperController)target;
			controller.Flip();
		}
	}


}
