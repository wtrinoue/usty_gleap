using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(StatusHolder))]
[RequireComponent(typeof(StatusActionHolder))]
public class StatusManager : MonoBehaviour,IHasStatusManager,ICoroutineUpdatable
{
    // ===== StatusHolderの参照 =====
    private StatusHolder statusHolder;
    // ===== 実行時ステータス =====
    private BaseStatus baseStatus;

    private BuffStatus buffStatus;

    private BuffStatus temporaryBuffStatus;
    private List<Buff> buffs;
    private List<Effect> effects;

    //インターフェースによるStatusManagerの取得
    public StatusManager Status => this;
    public BaseStatus BaseStatus => baseStatus;
    public BuffStatus BuffStatus => buffStatus;
    public BuffStatus TemporaryBuffStatus => temporaryBuffStatus;
    //破壊後の判定
    private bool isDestroyed = false;
    void Start()
    {
        statusHolder = GetComponent<StatusHolder>();
        // 基本用
        baseStatus = statusHolder.GetBaseStatus;
        buffStatus = statusHolder.GetBuffStatus;
        // バッファー用
        temporaryBuffStatus = new BuffStatus(statusHolder.GetTemporaryBuffStatus);
        buffs = statusHolder.GetBuffs;
        effects = statusHolder.GetEffects;
        // CoroutineManagerに登録
        CoroutineManager.Instance.Register(this);
    }

    void OnDestroy()
    {
        isDestroyed = true;
        CoroutineManager.Instance.Unregister(this);
    }

    // Update is called once per frame
    public void ManagedCoroutine(float interval)
    {
        if(isDestroyed)return;
        UpdateBuffs(interval);
        RecalculateBuffStatus();
        UpdateEffects(interval);
        TriggerEffects();
    }

    //バフの処理
    public void UpdateBuffs(float deltaTime)
    {
        if(buffs == null)return;
        for (int i = buffs.Count - 1; i >= 0; i--)
        {
            Buff buff = buffs[i];

            buff.ChangeDuration(-deltaTime);
            buff.ChangeInterval(-deltaTime);

            if (buff.GetDuration() <= 0)
            {
                buffs.RemoveAt(i);
            }
        }
    }

    public void RecalculateBuffStatus()
    {
        if(temporaryBuffStatus == null)return;
        // ベースステータスをコピー
        temporaryBuffStatus.CopyFrom(BuffStatus);

        // ステータス合算
        foreach (Buff buff in buffs)
        {
            if (buff.GetInterval() > 0) continue;
            temporaryBuffStatus.Merge(buff.GetBuffStatus());
        }

        // 各Buffにtemporaryを埋め込む & Intervalリセット
        foreach (Buff buff in buffs)
        {
            if (buff.GetInterval() > 0) continue;

            buff.EmbedBuff(temporaryBuffStatus);

            // 発動したBuffのIntervalをリセット
            buff.SetInterval(buff.GetStaticInterval());
        }
    }

    public void AddBuff(Buff buff)
    {
        // buffsがnullの場合は初期化されていないため、スキップ
        if(buffs == null){return;}

        // buffを渡したオブジェクトとassetの種類が同じであれば追加しない。
        bool alreadyExists = buffs.Any(b =>
            b.ObjectId == buff.ObjectId &&
            b.name == buff.name
        );

        if (alreadyExists) return;

        buffs.Add(buff);
    }

    //エフェクトの処理
    public void UpdateEffects(float deltaTime)
    {
        if(effects == null)return;
        for (int i = effects.Count - 1; i >= 0; i--)
        {
            Effect effect = effects[i];

            effect.ChangeDuration(-deltaTime);
            effect.ChangeInterval(-deltaTime);

            if (effect.GetDuration() <= 0)
            {
                effects.RemoveAt(i);
            }
        }
    }

    public void TriggerEffects()
    {
        if(effects == null)return;
        if (effects.Count == 0) return;

        foreach (Effect effect in effects)
        {
            if (effect.GetInterval() > 0) continue;

            // 効果実行
            effect.CustomEffect(this);

            // クールダウンリセット
            effect.SetInterval(effect.GetStaticInterval());
        }
    }
    public void AddEffect(Effect effect)
    {
        if(effects == null){return;}
        // effectを渡したオブジェクトとassetの種類が同じであれば追加しない。
        bool alreadyExists = effects.Any(e =>
            e.ObjectId == effect.ObjectId &&
            e.name == effect.name
        );

        if(alreadyExists) return;

        effects.Add(effect);
        Debug.Log("追加しました");
    }
    public void ApplyDamage(float damage)
    {
        float currentHP = baseStatus.CurrentHP;
        baseStatus.SetCurrentHP(currentHP - damage);
    }

    public void ApplyHeal(float amount)
    {
        float currentHP = baseStatus.CurrentHP;
        baseStatus.SetCurrentHP(currentHP + amount);
    }

    public float GetAttackPower()
    {
        float baseAttack = baseStatus.BaseAttack;
        float addAttack = temporaryBuffStatus.AddAttack;
        float multipleAttack = temporaryBuffStatus.MultipleAttack;

        Debug.Log("あなたの攻撃力は: "+(baseAttack + addAttack)*multipleAttack);
        return (baseAttack + addAttack)*multipleAttack;
    }

    public float GetSpeed()
    {
        if(this == null)return 0f;
        float baseSpeed = baseStatus.BaseSpeed;
        float addSpeed = temporaryBuffStatus.AddSpeed;
        float multipleSpeed = temporaryBuffStatus.MultipleSpeed;
        return (baseSpeed + addSpeed)*multipleSpeed;
        //※クリックが早すぎるとnull参照になる恐れがある。
    }
}

//★StatusManagerはStatusインスタンスの保持と、変更ルールに応じたメソッドを設定

//外部関数はStatusManagerを用いてメソッドを呼び出すだけで、簡単に値変更できるようにしてある。

//・Intervalの実装、付与したインスタンスのIDとBuffまたはEffect名が一致したら外すようにする。