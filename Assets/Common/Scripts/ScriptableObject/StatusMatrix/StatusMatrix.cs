using UnityEngine;

[CreateAssetMenu(menuName = "Status/Status Matrix")]
public class StatusMatrix : ScriptableObject
{
    [SerializeField]
    private float[] values;

    private int Width => (int)StatusMethod.Count;
    private void OnEnable()
    {
        EnsureInit();
    }

    private int Index(StatusCategory c, StatusMethod m)
    {
        return (int)c * Width + (int)m;
    }

    public float Get(StatusCategory c, StatusMethod m)
    {
        EnsureInit();
        return values[Index(c, m)];
    }

    public void Set(StatusCategory c, StatusMethod m, float v)
    {
        values[Index(c, m)] = v;
    }

    public void Add(StatusCategory c, StatusMethod m, float v)
    {
        values[Index(c, m)] += v;
    }

    public void Sub(StatusCategory c, StatusMethod m, float v)
    {
        values[Index(c, m)] -= v;
    }
    private void EnsureInit()
    {
        int size = (int)StatusCategory.Count * (int)StatusMethod.Count;

        if (values == null || values.Length != size)
        {
            System.Array.Resize(ref values, size);
        }
    }
#if UNITY_EDITOR
        private void OnValidate()
        {
            EnsureInit();
        }
#endif
}