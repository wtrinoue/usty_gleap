using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using SC = StatusCategory;
using SM = StatusMethod;

public class StatusVector
{
    private readonly float[,] values; // readonlyは差し替えは防げるが、中身の変更は防げない。

    // 空初期化
    public StatusVector()
    {
        values = new float[
            (int)StatusCategory.Count,
            (int)StatusMethod.Count
        ];

        InitializeDefaults();
    }

    // ScriptableObject初期化
    public StatusVector(StatusMatrix matrix)
    {
        values = new float[
            (int)StatusCategory.Count,
            (int)StatusMethod.Count
        ];

        for (int c = 0; c < (int)StatusCategory.Count; c++)
        {
            values[c, (int)StatusMethod.Base] =
                matrix.Get((StatusCategory)c, StatusMethod.Base);
        }

        // ShowStatus();
    }

    // コピーコンストラクター
    public StatusVector(StatusVector other)
    {
        values = new float[
            (int)StatusCategory.Count,
            (int)StatusMethod.Count
        ];
        for (int c = 0; c < (int)StatusCategory.Count; c++)
        {
            values[c, (int)StatusMethod.Base] =
                other.Get((StatusCategory)c, StatusMethod.Base);
        }
    }

    private void InitializeDefaults()
    {
        for (int c = 0; c < (int)StatusCategory.Count; c++)
        {
            values[c, (int)StatusMethod.Base] = 0f;
            values[c, (int)StatusMethod.Add] = 0f;
            values[c, (int)StatusMethod.Multiply] = 0f;
        }
    }

    public float Get(StatusCategory c, StatusMethod m)
    {
        return values[(int)c, (int)m];
    }

    public void Set(StatusCategory c, StatusMethod m, float v)
    {
        values[(int)c, (int)m] = v;
    }

    public float Calculate(StatusCategory category)
    {
        float baseValue = Get(category, StatusMethod.Base);
        float addValue = Get(category, StatusMethod.Add);
        float mulValue = Get(category, StatusMethod.Multiply);

        return (baseValue + addValue) * (1f + mulValue);
    }

    public void Add(StatusCategory c, StatusMethod m, float v)
    {
        values[(int)c, (int)m] += v;
    }

    // ✔ StatusVector同士の合成
    public void Merge(StatusVector other)
    {
        for (int c = 0; c < (int)StatusCategory.Count; c++)
        {
            for (int m = 0; m < (int)StatusMethod.Count; m++)
            {
                values[c, m] += other.values[c, m];
            }
        }
    }

    // 並列処理を用いたStatusVector同士の合成（10000~要素ぐらいから）
    public void ParallelMerge(StatusVector other)
    {
        // 外側のループ（Category）を並列化
        Parallel.For(0, (int)StatusCategory.Count, c =>
        {
            // 内側のループ（Method）は通常
            for (int m = 0; m < (int)StatusMethod.Count; m++)
            {
                values[c, m] += other.values[c, m];
            }
        });
    }


    // StatusVectorの一時的な合成のためのメソッド。バフシステムに用いる。
    public StatusVector Offset(StatusVector vector)
    {
        var result = new StatusVector(this);
        result.Merge(vector);
        return result;
    }

    public float this[StatusCategory c, StatusMethod m]
    {
        get => values[(int)c, (int)m];
        set => values[(int)c, (int)m] = value;
    }

    // ステータスについて、バリデーションを設定する。※これはStatusTokenで使用する際に用いる。
    public void Validate()
    {
        StatusValidation.Validate(this);
    }

    public void ShowStatus()
    {
        string logMessage = "=== ステータス行列 ===\n";

        for (int c = 0; c < (int)StatusCategory.Count; c++)
        {
            // 行の開始
            logMessage += $"Category {c}: [ ";

            for (int m = 0; m < (int)StatusMethod.Count; m++)
            {
                // 値を追加（見やすくするためにタブ区切り）
                logMessage += $"{values[c, m]}\t";
            }

            // 行の終わり
            logMessage += "]\n";
        }

        // 最後にまとめて出力（1つのログとして表示されます）
        UnityEngine.Debug.Log(logMessage);
    }
}

static class StatusValidation
{
    public static void Validate(in StatusVector s)
    {
        // 基本の体力に加算や乗算は省く
        s.Set(SC.HP, SM.Add, 0f);
        s.Set(SC.HP, SM.Multiply, 0f);


        float minLimit = 0.01f;
        float baseLimit = 1.0f;
        float baseAndAdd = 0f;
        foreach (SC sc in Enum.GetValues(typeof(SC)))
        {
            // StatusMethod.Multiplyはある一定値よりも大きくならないといけない。
            if (1.0f + s.Get(sc, SM.Multiply) < minLimit)
            {
                s.Set(sc, SM.Multiply, minLimit - 1f);
            }

            // StatusMethod.BaseとStatusMethod.Addの和はある一定以上は保証する。
            baseAndAdd = s.Get(sc, SM.Base) + s.Get(sc, SM.Add);
            if (baseAndAdd < baseLimit)
            {
                s.Set(sc, SM.Add, baseLimit - baseAndAdd);
            }
        }

    }
}