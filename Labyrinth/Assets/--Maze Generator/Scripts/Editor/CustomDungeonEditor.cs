using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

[System.Serializable]
//[CustomEditor(typeof(RoomDataObject))]
public class CustomDungeonEditor : EditorWindow {
    List<RoomDataObject> roomDataList;
    Vector2 scrollPos;

    [MenuItem ("Window/Dungeon Room Editor")]
    public static void Init () {
        CustomDungeonEditor window = (CustomDungeonEditor) EditorWindow.GetWindow (typeof (CustomDungeonEditor));
        window.Show ();
    }

    void OnEnable () {
        roomDataList = Resources.FindObjectsOfTypeAll<RoomDataObject> ().ToList ();
    }

    void OnInspectorUpdate () {
        Repaint ();
    }

    void OnGUI () {
        EditorGUILayout.LabelField ("All Room Prefabs", EditorStyles.boldLabel);
        scrollPos = EditorGUILayout.BeginScrollView (scrollPos, GUILayout.Width (450), GUILayout.Height (700));
        foreach (var roomData in roomDataList) {
            EditorGUILayout.LabelField ($"{roomData.nameRoom}", EditorStyles.boldLabel);
            SerializedObject _serializedObject = new SerializedObject (roomData);
            SerializedProperty _property = _serializedObject.GetIterator ();
            while (_property.NextVisible (true)) {
                EditorGUILayout.PropertyField (_property, false);
                _serializedObject.ApplyModifiedProperties ();
            }
        }

        EditorGUILayout.EndScrollView ();
    }
}