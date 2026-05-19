using System;
using UnityEngine;

public record State(
    Action[] EnterActions,
    Action[] UpdateActions,
    Action[] ExitActions)
{
    public void Enter()
    {
        foreach (var action in EnterActions)
        {
            action?.Invoke();
        }
    }

    public void Update()
    {
        foreach (var action in UpdateActions)
        {
            action?.Invoke();
        }
    }

    public void Exit()
    {
        foreach (var action in ExitActions)
        {
            action?.Invoke();
        }
    }
}