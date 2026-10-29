using UnityEditor;
using UnityEngine;

// Inspector personalizado para DayNightOverlay: dibuja el dropdown "Modo de capa" explícitamente.
[CustomEditor(typeof(DayNightOverlay))]
public class DayNightOverlayEditor : Editor
{
    SerializedProperty overlayType;
    SerializedProperty layerMode;
    SerializedProperty sunsetColor;
    SerializedProperty nightColor;
    SerializedProperty maxAlpha;
    SerializedProperty fadeSpeed;

    void OnEnable()
    {
        overlayType = serializedObject.FindProperty("overlayType");
        layerMode = serializedObject.FindProperty("layerMode");
        sunsetColor = serializedObject.FindProperty("sunsetColor");
        nightColor = serializedObject.FindProperty("nightColor");
        maxAlpha = serializedObject.FindProperty("maxAlpha");
        fadeSpeed = serializedObject.FindProperty("fadeSpeed");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Overlay de ambiente (tarde/noche)", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(overlayType, new GUIContent("Tipo de overlay"));
        EditorGUILayout.PropertyField(layerMode, new GUIContent("Modo de capa"));
        EditorGUILayout.Space(4);
        EditorGUILayout.PropertyField(sunsetColor, new GUIContent("Color tarde"));
        EditorGUILayout.PropertyField(nightColor, new GUIContent("Color noche"));
        EditorGUILayout.PropertyField(maxAlpha, new GUIContent("Opacidad máxima"));
        EditorGUILayout.PropertyField(fadeSpeed, new GUIContent("Velocidad de fade"));

        serializedObject.ApplyModifiedProperties();
    }
}