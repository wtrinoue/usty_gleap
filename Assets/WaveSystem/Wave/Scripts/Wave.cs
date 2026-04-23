using UnityEngine;
using System.Collections;

public class Wave : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(DestroyMeAfterDetach());
    }

    IEnumerator DestroyMeAfterDetach()
    {
        // 子オブジェクトをすべて親から切り離す
        Transform[] children = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            children[i] = transform.GetChild(i);
        }

        foreach (Transform child in children)
        {
            child.SetParent(null);
        }

        // 1フレーム待つ（切り離し処理を確実に反映させる）
        yield return null;

        // 自分自身を破壊
        Destroy(gameObject);
    }
}
