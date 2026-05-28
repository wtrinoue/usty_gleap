using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StatusMatrix))]
public class StatusMatrixEditor : Editor
{
    public override void OnInspectorGUI()
    {
        StatusMatrix matrix = (StatusMatrix)target;

        int rows = (int)StatusCategory.Count;
        int cols = (int)StatusMethod.Count;

        EditorGUILayout.LabelField("Status Matrix", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // ヘッダー
        EditorGUILayout.BeginHorizontal();
        GUILayout.Width(80);

        EditorGUILayout.LabelField("", GUILayout.Width(80));
        for (int m = 0; m < cols; m++)
        {
            EditorGUILayout.LabelField(((StatusMethod)m).ToString(), GUILayout.Width(80));
        }
        EditorGUILayout.EndHorizontal();

        // 本体
        for (int c = 0; c < rows; c++)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(((StatusCategory)c).ToString(), GUILayout.Width(80));

            for (int m = 0; m < cols; m++)
            {
                float value = matrix.Get((StatusCategory)c, (StatusMethod)m);
                float newValue = EditorGUILayout.FloatField(value, GUILayout.Width(80));

                if (value != newValue)
                {
                    matrix.Set((StatusCategory)c, (StatusMethod)m, newValue);
                    EditorUtility.SetDirty(matrix);
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(matrix);
        }
    }
}