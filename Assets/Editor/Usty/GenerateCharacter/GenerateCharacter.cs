using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class CharacterGenerator : EditorWindow
{
    private CharacterRegister characterRegister;
    private Vector2 scrollPosition;

    [MenuItem("Usty/CharacterGenerator")]
    private static void Open()
    {
        GetWindow<CharacterGenerator>("Usty");
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
        LoadCharacterRegister();
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    /// <summary>
    /// エディタースクリプトと同じフォルダにある CharacterRegister アセットをロードする
    /// </summary>
    private void LoadCharacterRegister()
    {
        // 自身のスクリプトのアセットパスを取得
        MonoScript script = MonoScript.FromScriptableObject(this);
        string scriptPath = AssetDatabase.GetAssetPath(script);
        string directoryPath = Path.GetDirectoryName(scriptPath);

        // 同一フォルダから CharacterRegister を検索
        string[] guids = AssetDatabase.FindAssets("t:CharacterRegister", new[] { directoryPath });

        if (guids.Length > 0)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            characterRegister = AssetDatabase.LoadAssetAtPath<CharacterRegister>(assetPath);
        }
        else
        {
            // 同一フォルダに見つからない場合はプロジェクト全体から検索
            guids = AssetDatabase.FindAssets("t:CharacterRegister");
            if (guids.Length > 0)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                characterRegister = AssetDatabase.LoadAssetAtPath<CharacterRegister>(assetPath);
            }
        }
    }

    private void OnGUI()
    {
        // 使い方の説明表示
        EditorGUILayout.HelpBox(
            "【使い方】\n" +
            "・ドラッグ＆ドロップ：Sceneビューへ配置\n" +
            "・ダブルクリック：該当Prefabを選択してInspectorで編集",
            MessageType.Info
        );
        EditorGUILayout.Space(5);
        // アセットがアサインされていない場合のフォールバック（手動アサイン枠）
        characterRegister = (CharacterRegister)EditorGUILayout.ObjectField(
            "Character Register",
            characterRegister,
            typeof(CharacterRegister),
            false
        );

        if (characterRegister == null)
        {
            EditorGUILayout.HelpBox("CharacterRegister アセットが見つかりません。同一フォルダに配置するか直接セットしてください。", MessageType.Warning);
            return;
        }

        EditorGUILayout.Space(10);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // 定義されている UstyCategory の列挙値を順に処理
        foreach (UstyCategory category in Enum.GetValues(typeof(UstyCategory)))
        {
            DrawCategoryGroup(category);
        }

        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// カテゴリごとにヘッダーと登録アイテムのグリッドを描画する
    /// </summary>
    private void DrawCategoryGroup(UstyCategory category)
    {
        if (characterRegister.objects == null) return;

        // 該当カテゴリのデータを抽出（Prefabが指定されているもののみ）
        var items = characterRegister.objects
            .Where(x => x != null && x.category == category && x.prefab != null)
            .ToList();

        // カテゴリラベル
        EditorGUILayout.LabelField(category.ToString(), EditorStyles.boldLabel);
        Rect lineRect = EditorGUILayout.GetControlRect(false, 1);
        EditorGUI.DrawRect(lineRect, new Color(0.5f, 0.5f, 0.5f, 0.5f)); // 区切り線
        EditorGUILayout.Space(5);

        if (items.Count == 0)
        {
            EditorGUILayout.LabelField("（登録なし）", EditorStyles.miniLabel);
            EditorGUILayout.Space(10);
            return;
        }

        // ウィンドウ幅に応じた折り返し表示の計算
        float tileSize = 100f;
        float spacing = 8f;
        float currentWidth = EditorGUIUtility.currentViewWidth - 30f; // 余白調整
        int columns = Mathf.Max(1, Mathf.FloorToInt((currentWidth + spacing) / (tileSize + spacing)));

        EditorGUILayout.BeginVertical();
        for (int i = 0; i < items.Count; i += columns)
        {
            EditorGUILayout.BeginHorizontal();
            for (int j = 0; j < columns; j++)
            {
                int index = i + j;
                if (index < items.Count)
                {
                    DrawObjectPanel(items[index].prefab, tileSize);
                    GUILayout.Space(spacing);
                }
                else
                {
                    // グリッドの位置合わせ用ダミー
                    GUILayout.FlexibleSpace();
                }
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(spacing);
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(15);
    }

    /// <summary>
    /// オブジェクトひとつの正方形パネルを描画する
    /// </summary>
    /// <summary>
    /// オブジェクトひとつの正方形パネルを描画する
    /// </summary>
    private void DrawObjectPanel(GameObject obj, float size)
    {
        Rect panelRect = GUILayoutUtility.GetRect(size, size, GUILayout.Width(size), GUILayout.Height(size));

        GUI.Box(panelRect, GUIContent.none);

        // Sprite と SpriteRenderer の Color を取得
        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
        Sprite sprite = spriteRenderer != null ? spriteRenderer.sprite : null;
        Color spriteColor = spriteRenderer != null ? spriteRenderer.color : Color.white;

        Rect spriteRect = new Rect(
            panelRect.x + 10,
            panelRect.y + 10,
            panelRect.width - 20,
            65
        );

        if (sprite != null)
        {
            Texture2D texture = sprite.texture;
            Rect textureRect = sprite.textureRect;

            Rect uv = new Rect(
                textureRect.x / texture.width,
                textureRect.y / texture.height,
                textureRect.width / texture.width,
                textureRect.height / texture.height
            );

            float aspect = textureRect.width / textureRect.height;
            float maxWidth = spriteRect.width;
            float maxHeight = spriteRect.height;

            float width = maxWidth;
            float height = width / aspect;

            if (height > maxHeight)
            {
                height = maxHeight;
                width = height * aspect;
            }

            Rect drawRect = new Rect(
                spriteRect.x + (spriteRect.width - width) / 2,
                spriteRect.y + (spriteRect.height - height) / 2,
                width,
                height
            );

            // GUI.color に SpriteRenderer の Color を設定して描画
            Color savedColor = GUI.color;
            GUI.color = spriteColor;

            GUI.DrawTextureWithTexCoords(drawRect, texture, uv);

            GUI.color = savedColor; // 元のGUIカラーに戻す
        }

        // オブジェクト名
        Rect nameRect = new Rect(
            panelRect.x + 2,
            panelRect.y + 78,
            panelRect.width - 4,
            18
        );

        GUI.Label(
            nameRect,
            obj.name,
            new GUIStyle(EditorStyles.centeredGreyMiniLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true
            }
        );

        // イベントの判定処理
        Event currentEvent = Event.current;
        if (panelRect.Contains(currentEvent.mousePosition))
        {
            // 1. ダブルクリック時の処理（アセットをInspectorで開く / Projectウィンドウでハイライト）
            if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0 && currentEvent.clickCount >= 2)
            {
                Selection.activeObject = obj;              // Inspectorで表示
                EditorGUIUtility.PingObject(obj);         // Projectウィンドウで強調（位置をアピール）
                currentEvent.Use();
            }
            // 2. ドラッグ開始の検出
            else if (currentEvent.type == EventType.MouseDrag)
            {
                DragAndDrop.PrepareStartDrag();
                DragAndDrop.objectReferences = new UnityEngine.Object[] { obj };
                DragAndDrop.StartDrag(obj.name);
                currentEvent.Use();
            }
        }
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;

        if (e.type != EventType.DragUpdated && e.type != EventType.DragPerform)
        {
            return;
        }

        if (DragAndDrop.objectReferences.Length == 0)
            return;

        GameObject prefab = DragAndDrop.objectReferences[0] as GameObject;

        if (prefab == null)
            return;

        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

        if (e.type != EventType.DragPerform)
            return;

        DragAndDrop.AcceptDrag();

        // Scene上のマウス位置（XY平面）
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        Plane plane = new Plane(Vector3.forward, Vector3.zero);

        if (!plane.Raycast(ray, out float distance))
            return;

        Vector3 position = ray.GetPoint(distance);

        // Prefab生成
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(
            prefab,
            EditorSceneManager.GetActiveScene()
        );

        instance.transform.position = position;

        Undo.RegisterCreatedObjectUndo(instance, "Place Prefab");
        Selection.activeGameObject = instance;

        e.Use();
    }
}