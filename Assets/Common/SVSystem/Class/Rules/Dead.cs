using System;
using UnityEngine;

public class Dead : SelfRule
{
    public void Execute(in StatusVector own, Action action)
    {
        float myHP = own.Get(StatusCategory.HP, StatusMethod.Base);
        if (myHP <= 0)
        {
            action.Invoke();
        }
    }
}
