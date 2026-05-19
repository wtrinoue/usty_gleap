using System;
using UnityEngine;

public class State
{
    private Action[] EnterActions;
    private Action[] UpdateActions;
    private Action[] ExitActions;

    public State(Action[] enterActions, Action[] updateActions, Action[] exitActions)
    {
        EnterActions = enterActions;
        UpdateActions = updateActions;
        ExitActions = exitActions;
    }

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