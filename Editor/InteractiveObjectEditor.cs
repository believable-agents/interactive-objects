using NodeCanvas.BehaviourTrees;
using Rotorz.ReorderableList;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InteractiveObjectBT))]
public class InteractiveObjectEditor : Editor
{

    SerializedProperty actions;
    SerializedProperty offsets;
    SerializedProperty lookAts;

    void OnEnable() {
        actions = serializedObject.FindProperty("actions");
        offsets = serializedObject.FindProperty("positionOffsets");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        ReorderableListGUI.Title("Actions");
        ReorderableListGUI.ListField(actions);
        ReorderableListGUI.Title("Offsets");
        ReorderableListGUI.ListField(offsets);
        serializedObject.ApplyModifiedProperties();
    }
}

[CustomPropertyDrawer(typeof(InteractiveObjectBT.PositionOffset))]
public class PositionOffsetEditor : PropertyDrawer
{
    const int LabelWidth = 15;
    public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label) {

        position.height = 16;
        var nr = new Rect(position);
        var width = position.width / 2;
        EditorGUI.LabelField(position, "P: ");

        nr.x = position.x + LabelWidth;
        nr.width = width - LabelWidth;
        prop.FindPropertyRelative("Position").vector3Value = EditorGUI.Vector3Field(nr, "", prop.FindPropertyRelative("Position").vector3Value);

        nr.x = position.x + nr.width + LabelWidth;
        EditorGUI.LabelField(nr, "L: ");
        nr.x = position.x + nr.width + LabelWidth + LabelWidth;
        prop.FindPropertyRelative("LookAt").vector3Value = EditorGUI.Vector3Field(nr, "", prop.FindPropertyRelative("LookAt").vector3Value);
    }
}

[CustomPropertyDrawer(typeof(InteractiveObjectBT.BTAction))]
public class InteractiveActionEditor : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty prop, GUIContent label) {
        //var innerEntriesProp = prop.FindPropertyRelative("modifiers");
        //return ReorderableListGUI.CalculateListFieldHeight(innerEntriesProp) + 90;
        return 70;
    }

    const int LabelWidth = 15;
    public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label) {
        var y = position.y;
        var height = position.height;
        var labelRect = new Rect(position);
        labelRect.height = 16;
        var valueRect = new Rect(position);
        valueRect.height = 16;
        valueRect.x += 75;
        valueRect.width -= 80;

        var action = prop.FindPropertyRelative("action");
        var owner = prop.FindPropertyRelative("owner");
        var bt = prop.FindPropertyRelative("BT");

        labelRect.y += 5;
        valueRect.y += 5;
        EditorGUI.LabelField(labelRect, "Name: ");
        action.stringValue = EditorGUI.TextField(valueRect, action.stringValue);


        labelRect.y += 20;
        valueRect.y += 20;
        EditorGUI.LabelField(labelRect, "Owner: ");
        owner.stringValue = EditorGUI.TextField(valueRect, owner.stringValue);

        labelRect.y += 20;
        valueRect.y += 20;
        EditorGUI.LabelField(labelRect, "Behaviour: ");
        bt.objectReferenceValue = EditorGUI.ObjectField(valueRect, bt.objectReferenceValue, typeof(BehaviourTree), true);

        //position.y = y + 65;
        //position.height = 25;
        //EditorGUI.LabelField(position, "Modifiers", LabelHelper.HeaderStyle);

        //position.y = y + 85;
        //position.height = height - 80;
        //var innerEntriesProp = prop.FindPropertyRelative("modifiers");
        //ReorderableListGUI.ListFieldAbsolute(position, innerEntriesProp);
    }
}

//[CustomPropertyDrawer(typeof(InteractiveObjectBT.Modifier))]
//public class InteractiveActionModifierEditor : PropertyDrawer
//{
//    public override float GetPropertyHeight(SerializedProperty prop, GUIContent label) {
//        var innerEntriesProp = prop.FindPropertyRelative("personalityModifiers");
//        return ReorderableListGUI.CalculateListFieldHeight(innerEntriesProp) + 65;
//    }

//    public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label) {

//        var y = position.y;
//        var height = position.height;

//        position.height = 16;
//        EditorGUI.PropertyField(position, prop.FindPropertyRelative("type"));
//        position.y += 20;
//        EditorGUI.PropertyField(position, prop.FindPropertyRelative("delta"));

//        position.y = y + 40;
//        position.height = 23;
//        EditorGUI.LabelField(position, "Personality", LabelHelper.HeaderStyle);

//        position.y = y + 60;
//        position.height = height - 60;
//        var innerEntriesProp = prop.FindPropertyRelative("personalityModifiers");
//        ReorderableListGUI.ListFieldAbsolute(position, innerEntriesProp);
//    }
//}

//[CustomPropertyDrawer(typeof(InteractiveObjectBT.PersonalityModifier))]
//public class InteractiveActionPersonalityModifierEditor : PropertyDrawer
//{
//    public override float GetPropertyHeight(SerializedProperty prop, GUIContent label) {
//        return 40;
//    }

//    public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label) {
//        position.height = 16;
//        EditorGUI.PropertyField(position, prop.FindPropertyRelative("type"));
//        position.y += 20;
//        EditorGUI.PropertyField(position, prop.FindPropertyRelative("delta"));
//    }
//}


static class LabelHelper
{
    public static GUIStyle HeaderStyle;

    static LabelHelper() {
        HeaderStyle = new GUIStyle();
        HeaderStyle.border = new RectOffset(2, 2, 2, 1);
        HeaderStyle.margin = new RectOffset(5, 5, 5, 0);
        HeaderStyle.padding = new RectOffset(5, 5, 0, 0);
        HeaderStyle.alignment = TextAnchor.MiddleLeft;
        HeaderStyle.fontStyle = FontStyle.Bold;
        HeaderStyle.normal.background = EditorGUIUtility.isProSkin
            ? LoadTexture(s_DarkSkin)
                : LoadTexture(s_LightSkin);
        HeaderStyle.normal.textColor = EditorGUIUtility.isProSkin
            ? new Color(0.8f, 0.8f, 0.8f)
                : new Color(0.2f, 0.2f, 0.2f);
    }

    static Texture2D LoadTexture(string textureData) {
        byte[] imageData = Convert.FromBase64String(textureData);

        // Gather image size from image data.
        int texWidth, texHeight;
        GetImageSize(imageData, out texWidth, out texHeight);

        // Generate texture asset.
        var tex = new Texture2D(texWidth, texHeight, TextureFormat.ARGB32, false);
        tex.hideFlags = HideFlags.HideAndDontSave;
        tex.name = "ReorderableList";
        tex.filterMode = FilterMode.Point;
        tex.LoadImage(imageData);

        return tex;
    }

    private static void GetImageSize(byte[] imageData, out int width, out int height) {
        width = ReadInt(imageData, 3 + 15);
        height = ReadInt(imageData, 3 + 15 + 2 + 2);
    }

    private static int ReadInt(byte[] imageData, int offset) {
        return (imageData[offset] << 8) | imageData[offset + 1];
    }

    /// <summary>
    /// Resource assets for light skin.
    /// </summary>
    /// <remarks>
    /// <para>Resource assets are PNG images which have been encoded using a base-64
    /// string so that actual asset files are not necessary.</para>
    /// </remarks>
    private static string s_LightSkin =
        "iVBORw0KGgoAAAANSUhEUgAAAAUAAAAECAYAAABGM/VAAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAEFJREFUeNpi/P//P0NxcfF/BgRgZP78+fN/VVVVhpCQEAZjY2OGs2fPNrCApBwdHRkePHgAVwoWnDVrFgMyAAgwAAt4E1dCq1obAAAAAElFTkSuQmCC";
    /// <summary>
    /// Resource assets for dark skin.
    /// </summary>
    /// <remarks>
    /// <para>Resource assets are PNG images which have been encoded using a base-64
    /// string so that actual asset files are not necessary.</para>
    /// </remarks>
    private static string s_DarkSkin =

        "iVBORw0KGgoAAAANSUhEUgAAAAUAAAAECAYAAABGM/VAAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAADtJREFUeNpi/P//P4OKisp/Bii4c+cOIwtIQE9Pj+HLly9gQRCfBcQACbx69QqmmAEseO/ePQZkABBgAD04FXsmmijSAAAAAElFTkSuQmCC";
}
