#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

// Inspector personalizado para AllyShootSound con boton de prueba.
[CustomEditor(typeof(AllyShootSound))]
public class AllyShootSoundEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AllyShootSound sound = (AllyShootSound)target;

        EditorGUILayout.Space();
        if (GUILayout.Button("Probar sonido de disparo"))
        {
            if (sound.shootClip != null)
            {
                AudioSource.PlayClipAtPoint(sound.shootClip, sound.transform.position, sound.volume);
            }
            else
            {
                EditorUtility.DisplayDialog("AllyShootSound", "No hay clip asignado.", "OK");
            }
        }
    }
}
#endif