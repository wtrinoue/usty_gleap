# Unity Audio設計まとめ（SoundData + AudioManager）

## ■ 概要
この設計は、音の再生処理と音のデータを分離し、拡張性・保守性を高めることを目的としています。

- 再生処理 → AudioManager
- 音のデータ → SoundData（ScriptableObject）
- 使用側（武器など）→ SoundDataを参照するだけ

---

## ■ 構成

### 1. SoundData（音のデータ）
音に関する情報をまとめたScriptableObject

```csharp
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/SoundData")]
public class SoundData : ScriptableObject
{
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.5f, 1.5f)] public float pitch = 1f;

    public bool randomPitch = false;
    public float pitchMin = 0.9f;
    public float pitchMax = 1.1f;
}
```

### 特徴
- 音量・ピッチをデータとして管理
- ランダムピッチ対応で自然な音にできる
- 使い回しが可能

---

### 2. AudioManager（音の再生管理）
音の再生を一括で管理するクラス

```csharp
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource seSource;
    [SerializeField] private AudioSource bgmSource;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void PlaySE(SoundData sound)
    {
        if (sound == null || sound.clip == null) return;

        float pitch = sound.pitch;

        if (sound.randomPitch)
        {
            pitch = Random.Range(sound.pitchMin, sound.pitchMax);
        }

        seSource.pitch = pitch;
        seSource.PlayOneShot(sound.clip, sound.volume);
    }

    public void PlayBGM(SoundData sound)
    {
        if (sound == null || sound.clip == null) return;

        bgmSource.clip = sound.clip;
        bgmSource.volume = sound.volume;
        bgmSource.pitch = sound.pitch;
        bgmSource.loop = true;
        bgmSource.Play();
    }
}
```

### 特徴
- 音の再生を一箇所に集約
- SEとBGMを分離
- どこからでも呼び出せる

---

### 3. 使用側（武器など）

```csharp
[SerializeField] private SoundData attackSound;

void Attack()
{
    AudioManager.Instance.PlaySE(attackSound);
}
```

### 特徴
- 音の詳細を知らなくていい
- データを渡すだけで再生できる

---

## ■ この設計のメリット

### 1. データドリブン
- 音の調整はコードではなくデータで行う
- SoundDataを編集するだけで反映

### 2. 再利用性が高い
- 同じ音を複数の場所で使い回せる

### 3. 保守性が高い
- 再生処理がAudioManagerに集約される
- 修正箇所が明確

### 4. 拡張しやすい
後から以下を追加可能：
- 音量設定（オプション）
- 3Dサウンド
- AudioMixer対応
- SEの同時再生制御

---

## ■ 注意点

### 1. AudioSourceが1つだと制限がある
- 同時再生時にpitchが上書きされる

#### 対策
- AudioSourceを複数持つ
- AudioSourceプールを導入

---

## ■ 発展案（上級）

- AudioSourceプール
- 3D音（位置付き再生）
- AudioMixerで音量管理
- SE種類をenumで管理

---

## ■ まとめ

この設計の本質は以下です：

- 再生処理とデータを分離
- AudioManagerに責務を集約
- SoundDataで音を管理

これにより、シンプルかつ拡張性の高い音システムを構築できます。