using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class UstyWindow : EditorWindow
{
    private List<GameObject> registeredObjects = new();

    [MenuItem("Usty/Usty Window")]
    private static void Open()
    {
        GetWindow<UstyWindow>("Usty");
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnGUI()
    {
        GUILayout.Label(
            "登録オブジェクト",
            EditorStyles.boldLabel
        );

        GUILayout.Space(5);

        // 登録されているオブジェクトを表示
        for (int i = 0; i < registeredObjects.Count; i++)
        {
            DrawObject(i);
        }

        GUILayout.Space(10);

        // オブジェクト追加
        if (GUILayout.Button("＋ オブジェクトを追加"))
        {
            registeredObjects.Add(null);
        }
    }

    private void DrawObject(int index)
    {
        GameObject obj = registeredObjects[index];

        // オブジェクト未登録の場合
        if (obj == null)
        {
            EditorGUILayout.BeginHorizontal();

            registeredObjects[index] =
                (GameObject)EditorGUILayout.ObjectField(
                    "Prefab",
                    null,
                    typeof(GameObject),
                    false
                );

            if (GUILayout.Button("×", GUILayout.Width(25)))
            {
                registeredObjects.RemoveAt(index);
            }

            EditorGUILayout.EndHorizontal();

            return;
        }

        // 正方形のパネル
        Rect panelRect = GUILayoutUtility.GetRect(
            100,
            100
        );

        GUI.Box(
            panelRect,
            GUIContent.none
        );

        // Sprite取得
        SpriteRenderer spriteRenderer =
            obj.GetComponent<SpriteRenderer>();

        Sprite sprite =
            spriteRenderer != null
                ? spriteRenderer.sprite
                : null;

        // Sprite表示領域
        Rect spriteRect = new Rect(
            panelRect.x + 10,
            panelRect.y + 10,
            panelRect.width - 20,
            70
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

            // Spriteの縦横比
            float aspect =
                textureRect.width / textureRect.height;

            // 表示領域
            float maxWidth = spriteRect.width;
            float maxHeight = spriteRect.height;

            float width = maxWidth;
            float height = width / aspect;

            if (height > maxHeight)
            {
                height = maxHeight;
                width = height * aspect;
            }

            // 中央寄せ
            Rect drawRect = new Rect(
                spriteRect.x + (spriteRect.width - width) / 2,
                spriteRect.y + (spriteRect.height - height) / 2,
                width,
                height
            );

            GUI.DrawTextureWithTexCoords(
                drawRect,
                texture,
                uv
            );
        }

        // オブジェクト名
        Rect nameRect = new Rect(
            panelRect.x + 5,
            panelRect.y + 80,
            panelRect.width - 10,
            15
        );

        GUI.Label(
            nameRect,
            obj.name,
            new GUIStyle(EditorStyles.centeredGreyMiniLabel)
            {
                alignment = TextAnchor.MiddleCenter
            }
        );

        // ドラッグ開始
        if (Event.current.type == EventType.MouseDown &&
            panelRect.Contains(Event.current.mousePosition))
        {
            DragAndDrop.PrepareStartDrag();

            DragAndDrop.objectReferences =
                new Object[]
                {
                obj
                };

            DragAndDrop.StartDrag(obj.name);

            Event.current.Use();
        }
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;

        if (e.type != EventType.DragUpdated &&
            e.type != EventType.DragPerform)
        {
            return;
        }

        if (DragAndDrop.objectReferences.Length == 0)
            return;

        GameObject prefab =
            DragAndDrop.objectReferences[0] as GameObject;

        if (prefab == null)
            return;

        DragAndDrop.visualMode =
            DragAndDropVisualMode.Copy;

        if (e.type != EventType.DragPerform)
            return;

        DragAndDrop.AcceptDrag();

        // Scene上のマウス位置
        Ray ray =
            HandleUtility.GUIPointToWorldRay(
                e.mousePosition
            );

        // XY平面
        Plane plane =
            new Plane(
                Vector3.forward,
                Vector3.zero
            );

        if (!plane.Raycast(ray, out float distance))
            return;

        Vector3 position =
            ray.GetPoint(distance);

        // Prefab生成
        GameObject instance =
            (GameObject)PrefabUtility.InstantiatePrefab(
                prefab,
                EditorSceneManager.GetActiveScene()
            );

        instance.transform.position = position;

        Undo.RegisterCreatedObjectUndo(
            instance,
            "Place Prefab"
        );

        Selection.activeGameObject = instance;

        e.Use();
    }
}