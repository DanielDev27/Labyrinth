using UnityEngine;
using UnityEditor;

[System.Serializable, CustomPropertyDrawer (typeof (Dungeon))]
public class DungeonGeneratorDrawer : PropertyDrawer {
    SerializedProperty roomHeight;
    SerializedProperty roomWidth;
    SerializedProperty waitTime;
    SerializedProperty maxInbetweenRooms;
    SerializedProperty maxExtensionRooms;
    SerializedProperty width;
    SerializedProperty height;
    SerializedProperty startRoom;
    SerializedProperty endRooms;
    SerializedProperty roomHolderPrefab;
    SerializedProperty roomParent;
    SerializedProperty baseRoom;

    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty (position, label, property);
        GUILayout.Label ("Dungeon Generator Settings", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal ();
        waitTime = property.FindPropertyRelative ("waitTime");
        EditorGUILayout.PropertyField (waitTime);
        EditorGUILayout.EndHorizontal ();

        EditorGUILayout.BeginHorizontal ();
        GUILayout.Label ("Room Size", EditorStyles.label);
        EditorGUILayout.EndHorizontal ();
        EditorGUILayout.BeginHorizontal ();
        GUILayout.Label ("Room X", EditorStyles.miniLabel);
        roomWidth = property.FindPropertyRelative ("roomWidth");
        EditorGUILayout.PropertyField (roomWidth, GUIContent.none);
        GUILayout.Label ("Room Y", EditorStyles.miniLabel);
        roomHeight = property.FindPropertyRelative ("roomHeight");
        EditorGUILayout.PropertyField (roomHeight, GUIContent.none);
        EditorGUILayout.EndHorizontal ();

        EditorGUILayout.BeginHorizontal ();
        GUILayout.Label ("Grid Size", EditorStyles.label);
        EditorGUILayout.EndHorizontal ();
        EditorGUILayout.BeginHorizontal ();
        GUILayout.Label ("Grid X", EditorStyles.miniLabel);
        width = property.FindPropertyRelative ("width");
        EditorGUILayout.PropertyField (width, GUIContent.none);
        GUILayout.Label ("Grid Y", EditorStyles.miniLabel);
        height = property.FindPropertyRelative ("height");
        EditorGUILayout.PropertyField (height, GUIContent.none);
        EditorGUILayout.EndHorizontal ();

        EditorGUILayout.BeginHorizontal ();
        GUILayout.Label ("Amount of Rooms", EditorStyles.label);
        EditorGUILayout.EndHorizontal ();
        EditorGUILayout.BeginHorizontal ();
        GUILayout.Label ("Inbetween Rooms", EditorStyles.miniLabel);
        maxInbetweenRooms = property.FindPropertyRelative ("maxInbetweenRooms");
        EditorGUILayout.PropertyField (maxInbetweenRooms, GUIContent.none);
        GUILayout.Label ("Extension Rooms", EditorStyles.miniLabel);
        maxExtensionRooms = property.FindPropertyRelative ("maxExtensionRooms");
        EditorGUILayout.PropertyField (maxExtensionRooms, GUIContent.none);
        EditorGUILayout.EndHorizontal ();

        EditorGUILayout.BeginHorizontal ();
        GUILayout.Label ("Rooms", EditorStyles.label);
        GUILayout.Label ("Settings, Parents, and Prefabs", EditorStyles.label);
        EditorGUILayout.EndHorizontal ();
        EditorGUILayout.BeginHorizontal ();
        startRoom = property.FindPropertyRelative ("startRoom");
        EditorGUILayout.PropertyField (startRoom);
        EditorGUILayout.EndHorizontal ();
        EditorGUILayout.BeginHorizontal ();
        endRooms = property.FindPropertyRelative ("endRooms");
        EditorGUILayout.PropertyField (endRooms);
        EditorGUILayout.EndHorizontal ();
        EditorGUILayout.BeginHorizontal ();
        roomHolderPrefab = property.FindPropertyRelative ("roomHolderPrefab");
        EditorGUILayout.PropertyField (roomHolderPrefab);
        EditorGUILayout.EndHorizontal ();
        EditorGUILayout.BeginHorizontal ();
        roomParent = property.FindPropertyRelative ("roomParent");
        EditorGUILayout.PropertyField (roomParent);
        EditorGUILayout.EndHorizontal ();
        EditorGUILayout.BeginHorizontal ();
        baseRoom = property.FindPropertyRelative ("baseRoom");
        EditorGUILayout.PropertyField (baseRoom);
        EditorGUILayout.EndHorizontal ();


        EditorGUI.EndProperty ();
    }
}