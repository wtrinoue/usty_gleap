using System;

public interface SelfRule
{
    public void Execute(in StatusVector own, Action action);
}