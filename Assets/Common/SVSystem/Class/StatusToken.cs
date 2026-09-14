public abstract class StatusToken
{
    private StatusVector source;

    public void SetSource(StatusVector s)
    {
        source = new StatusVector(s);
    }

    abstract public void Execute(in StatusVector target);
}