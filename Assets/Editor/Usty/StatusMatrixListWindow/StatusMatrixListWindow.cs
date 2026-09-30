using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class StatusMatrixListWindow : EditorWindow
{
    private Vector2 scrollPosition;
    private List<StatusMatrix> matrixAssets = new List<StatusMatrix>();

    // アセットごとの CustomEditor をキャッシュする辞書
    private Dictionary<StatusMatrix, Editor> editorCache = new Dictionary<StatusMatrix, Editor>();

    // アセットごとの開閉（折りたたみ）状態を管理する辞書
    private Dictionary<StatusMatrix, bool> foldoutStates = new Dictionary<StatusMatrix, bool>();

    [MenuItem("Usty/Status Matrix List Window")]
    public static void Open()
    {
        GetWindow<StatusMatrixListWindow>("Status Matrix 一覧");
    }

    private void OnEnable()
    {
        RefreshList();
    }

    private void OnDisable()
    {
        // メモリリーク防止のため、キャッシュした Editor を破棄
        foreach (var editor in editorCache.Values)
        {
            if (editor != null) DestroyImmediate(editor);
        }
        editorCache.Clear();
        foldoutStates.Clear();
    }

    private void RefreshList()
    {
        matrixAssets.Clear();

        // プロジェクト内のすべての StatusMatrix アセットを検索
        string[] guids = AssetDatabase.FindAssets("t:StatusMatrix");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var asset = AssetDatabase.LoadAssetAtPath<StatusMatrix>(path);
            if (asset != null)
            {
                matrixAssets.Add(asset);

                // 初期状態を設定（まだ登録がなければ「閉じている状態(false)」にする）
                if (!foldoutStates.ContainsKey(asset))
                {
                    foldoutStates[asset] = false; // 初期値を true にすれば最初から開くことも可能
                }
            }
        }
    }

    private void OnGUI()
    {
        // ツールバー (更新ボタン & 一括開閉ボタン)
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        if (GUILayout.Button("更新 (Refresh)", EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
            RefreshList();
        }

        // 便利機能：すべて開く / すべて閉じる ボタン
        if (GUILayout.Button("すべて開く", EditorStyles.toolbarButton, GUILayout.Width(80)))
        {
            SetAllFoldouts(true);
        }
        if (GUILayout.Button("すべて閉じる", EditorStyles.toolbarButton, GUILayout.Width(80)))
        {
            SetAllFoldouts(false);
        }
        EditorGUILayout.EndHorizontal();

        if (matrixAssets.Count == 0)
        {
            EditorGUILayout.HelpBox("StatusMatrix アセットが見つかりませんでした。", MessageType.Info);
            return;
        }

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        foreach (var matrix in matrixAssets)
        {
            if (matrix == null) continue;

            EditorGUILayout.BeginVertical(GUI.skin.box);

            // --- ヘッダー部分 (アコーディオン化) ---
            EditorGUILayout.BeginHorizontal();

            // アセット名付きの Foldout
            bool isOpen = foldoutStates.GetValueOrDefault(matrix, false);
            isOpen = EditorGUILayout.Foldout(isOpen, matrix.name, true, EditorStyles.foldoutHeader);
            foldoutStates[matrix] = isOpen;

            // Projectで表示するボタン
            if (GUILayout.Button("Projectで表示", GUILayout.Width(100)))
            {
                EditorGUIUtility.PingObject(matrix);
            }
            EditorGUILayout.EndHorizontal();

            // --- アコーディオンの中身（開いているときだけ描画） ---
            if (isOpen)
            {
                EditorGUILayout.Space(5);

                // キャッシュから Editor を取得（なければ作成）
                if (!editorCache.TryGetValue(matrix, out var cachedEditor) || cachedEditor == null)
                {
                    cachedEditor = Editor.CreateEditor(matrix);
                    editorCache[matrix] = cachedEditor;
                }

                // CustomEditor の OnInspectorGUI() を実行
                if (cachedEditor != null)
                {
                    cachedEditor.OnInspectorGUI();
                }
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }

        EditorGUILayout.EndScrollView();
    }

    // すべての要素の開閉を一括切り替えする補助メソッド
    private void SetAllFoldouts(bool isOpen)
    {
        var keys = new List<StatusMatrix>(foldoutStates.Keys);
        foreach (var key in keys)
        {
            foldoutStates[key] = isOpen;
        }
    }
}