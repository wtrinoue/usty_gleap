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
            values[c, (int)StatusMethod.Multiply] = 0f;
        }
    }

    public float Get(StatusCategory c)
    {
        float baseValue = values[(int)c, (int)StatusMethod.Base];
        float addValue = values[(int)c, (int)StatusMethod.Add];
        float multiplyValue = values[(int)c, (int)StatusMethod.Multiply];

        return (baseValue + addValue) * (1f + multiplyValue);
    }

    public void Set(StatusCategory c, StatusMethod m, float v)
    {
        values[(int)c, (int)m] = v;
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

    public float this[StatusCategory c, StatusMethod m]
    {
        get => values[(int)c, (int)m];
        set => values[(int)c, (int)m] = value;
    }
}

public enum StatusCategory
{
    HP = 0,
    Attack = 1,
    Magic = 2,
    Defense = 3,
    Speed = 4,
    Count = 5
}

public enum StatusMethod
{
    Base = 0,
    Add = 1,
    Multiply = 2,
    Count = 3
}