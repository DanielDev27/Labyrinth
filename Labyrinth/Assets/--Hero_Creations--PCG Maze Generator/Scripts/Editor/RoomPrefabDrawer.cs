using UnityEngine;
using UnityEditor;

[System.Serializable, CustomPropertyDrawer (typeof (RoomDataObject))]
public class RoomPrefabDrawer : PropertyDrawer {
    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty (position, label, property);

        position = EditorGUI.PrefixLabel (position, GUIUtility.GetControlID (FocusType.Passive), GUIContent.none);
        var _nameRect = new Rect (position.x, position.y, position.width / 3, position.height);
        var _roomRect = new Rect (position.x + position.width / 2, position.y, position.width / 2, position.height);

        SerializedObject roomDataSerializedObject = new SerializedObject (property.objectReferenceValue);
        EditorGUI.PropertyField (_roomRect, roomDataSerializedObject.FindProperty ("RoomModel"), GUIContent.none, false);

        roomDataSerializedObject.ApplyModifiedProperties ();

        EditorGUI.PropertyField (_nameRect, roomDataSerializedObject.FindProperty ("nameRoom"), GUIContent.none, false);
        
        

        EditorGUI.EndProperty ();
    }
}