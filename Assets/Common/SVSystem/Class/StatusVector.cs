using System;

public class StatusVector
{
    private readonly float[,] values;

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

        InitializeDefaults();

        for (int c = 0; c < (int)StatusCategory.Count; c++)
        {
            values[c, (int)StatusMethod.Base] =
                matrix.Get((StatusCategory)c, StatusMethod.Base);
        }
    }

    private void InitializeDefaults()
    {
        for (int c = 0; c < (int)StatusCategory.Count; c++)
        {
            values[c, (int)StatusMethod.Base] = 0f;
            values[c, (int)StatusMethod.Add] = 0f;
            values[c, (int)StatusMethod.Multiply] = 1f;
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

    public void Sub(StatusCategory c, StatusMethod m, float v)
    {
        values[(int)c, (int)m] -= v;
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

    // StatusVectorの一時的な合成のためのメソッド。バフシステムに用いる。
    public StatusVector Offset(StatusVector vector)
    {
        var result = new StatusVector();
        result.Merge(vector);
        return result;
    }

    public float this[StatusCategory c, StatusMethod m]
    {
        get => values[(int)c, (int)m];
        set => values[(int)c, (int)m] = value;
    }
}