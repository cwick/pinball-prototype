using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(PolygonGenerator))]
public class PolygonGeneratorEditor : Editor {

    public override void OnInspectorGUI() {
        var generator = (PolygonGenerator)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Generate")) {
            generator.Generate();
        }
    }
}
