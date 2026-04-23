using UnityEngine;
using UnityEngine.SceneManagement;

public class GameClearManager : MonoBehaviour
{
    private WaveManager waveManager;
    [SerializeField] private GameObject clearUI; // クリア時に表示するパネル
    [SerializeField] private string titleSceneName = "TitleScene"; // タイトルシーンの名前

    private bool isCleared = false;

    void Start()
    {
        // 1. "WaveManager" という名前のオブジェクトを探す
        GameObject managerObj = GameObject.Find("WaveManager");

        if (managerObj != null)
        {
            // 2. その中にある Emitter スクリプトを取得する
            waveManager = managerObj.GetComponent<WaveManager>();
        }

        // 初期状態ではクリアUIを隠しておく
        if (clearUI != null)
        {
            clearUI.SetActive(false);
        }
    }

    void Update()
    {
        // もし WaveManager（emitter）が見つかっていないなら、これ以降の処理をしない
        if (waveManager == null) return; 
    
        if (!isCleared)
        {
            if (waveManager.allWaveFinish)
            {
                if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
                {
                    ShowGameClear();
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                BackToTitle();
            }
        }
    }

    private void ShowGameClear()
    {
        isCleared = true;
        if (clearUI != null)
        {
            clearUI.SetActive(true); // パネルを表示して画面を暗くする
        }
    }

    // ボタンとスペースキー両方から呼ばれるタイトル遷移処理
    public void BackToTitle()
    {
        SceneManager.LoadScene(titleSceneName);
    }
}