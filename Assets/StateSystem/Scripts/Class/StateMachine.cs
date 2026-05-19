public class StateMachine
{
    private State current;
    private bool initialized = false;

    public void Update(IStateProvider provider)
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

        current?.Update();
    }
}