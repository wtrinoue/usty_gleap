using UnityEngine;
using UnityEditor;

public class StatusWatcher : EditorWindow
{
    private static readonly StatusMethod[] Methods =
    {
        StatusMethod.Base,
        StatusMethod.Add,
        StatusMethod.Multiply
    };

    [MenuItem("Usty/Status Watcher")]
    private static void Open()
    {
        GetWindow<StatusWatcher>("Status Watcher");
    }

    private void OnEnable()
    {
        EditorApplication.update += Repaint;
    }

    private void OnDisable()
    {
        EditorApplication.update -= Repaint;
    }

    private void OnGUI()
    {
        if (!EditorApplication.isPlaying)
        {
            EditorGUILayout.HelpBox("ゲーム再生中に対象オブジェクトを選択してください。", MessageType.Info);
            return;
        }

        GameObject selectedObject = Selection.activeGameObject;
        if (selectedObject == null)
        {
            EditorGUILayout.HelpBox("StatusContainerを持つオブジェクトを選択してください。", MessageType.Info);
            return;
        }

        StatusContainer container = selectedObject.GetComponent<StatusContainer>();
        if (container == null)
        {
            EditorGUILayout.HelpBox("選択中のオブジェクトにStatusContainerがありません。", MessageType.Info);
            return;
        }

        EditorGUILayout.LabelField(selectedObject.name, EditorStyles.boldLabel);
        EditorGUILayout.LabelField("BASE", new GUIStyle(EditorStyles.label) { normal = { textColor = Color.red } });
        DrawStatusMatrix(container.statusVector);
        EditorGUILayout.LabelField("MODIFIED", new GUIStyle(EditorStyles.label) { normal = { textColor = Color.blue } });
        DrawStatusMatrix(container.GetStatus());

    }

    private void DrawStatusMatrix(StatusVector status)
    {
        if (status == null)
        {
            EditorGUILayout.HelpBox("StatusContainerのステータスが初期化されていません。", MessageType.Info);
            return;
        }

        EditorGUILayout.Space(4);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Category", EditorStyles.boldLabel, GUILayout.MinWidth(80));
        foreach (StatusMethod method in Methods)
        {
            GUILayout.Label(method.ToString(), EditorStyles.boldLabel, GUILayout.MinWidth(70));
        }
        EditorGUILayout.EndHorizontal();
        EditorGUI.DrawRect(
            GUILayoutUtility.GetRect(1f, 1f, GUILayout.ExpandWidth(true)),
            EditorGUIUtility.isProSkin ? new Color(0.35f, 0.35f, 0.35f) : new Color(0.7f, 0.7f, 0.7f));

        for (int categoryIndex = 0; categoryIndex < (int)StatusCategory.Count; categoryIndex++)
        {
            StatusCategory category = (StatusCategory)categoryIndex;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(category.ToString(), GUILayout.MinWidth(80));
            foreach (StatusMethod method in Methods)
            {
                EditorGUILayout.LabelField(status.Get(category, method).ToString("0.###"), GUILayout.MinWidth(70));
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.DrawRect(
                GUILayoutUtility.GetRect(1f, 1f, GUILayout.ExpandWidth(true)),
                EditorGUIUtility.isProSkin ? new Color(0.35f, 0.35f, 0.35f) : new Color(0.7f, 0.7f, 0.7f));
        }
    }
}
