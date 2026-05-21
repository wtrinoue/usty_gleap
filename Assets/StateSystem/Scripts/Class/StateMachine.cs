using UnityEngine; // Unity の Time.deltaTime などを使う場合は必要（あるいは引数で受け取る）

public class StateMachine
{
    private State current;
    private bool initialized = false;

    // 外部から deltaTime（Time.deltaTime など）を受け取れるように変更
    public void Update(IStateProvider provider, float deltaTime)
    {
        State next = provider.ProvideState();

        if (!initialized)
        {
            current = next;
            current?.Enter();
            initialized = true;
        }
        else if (next != current)
        {
            current?.Exit();
            current = next;
            current?.Enter();
        }

        // 修正した State.Update(float deltaTime) に経過時間を渡す
        current?.Update(deltaTime);
    }
}