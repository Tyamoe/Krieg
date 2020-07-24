using UnityEngine;
using UnityEditor;

//[CustomEditor(typeof(Transform))]
class DebugMenu : Editor
{
    /*public override void OnInspectorGUI() 
    {
        Transform t = Selection.activeGameObject.GetComponent<Transform>();

        EditorGUILayout.BeginHorizontal();
        t.position = EditorGUILayout.Vector3Field("World Pos", t.position);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        t.localPosition = EditorGUILayout.Vector3Field("Position", t.localPosition);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        t.eulerAngles = EditorGUILayout.Vector3Field("World Rotation", t.eulerAngles);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        t.localEulerAngles = EditorGUILayout.Vector3Field("Rotation", t.localEulerAngles);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        t.localScale = EditorGUILayout.Vector3Field("Scale", t.localScale);
        EditorGUILayout.EndHorizontal();
    }*/
}