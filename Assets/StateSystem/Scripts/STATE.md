## 第二案
State、StateMachine、IStateProviderの三つの連携。
### Stateの実装について
> State：関数登録型、純粋クラス、実行関数の保持のみ
``` csharp
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
```
> Stateの使用例
``` csharp
using UnityEngine;

public class StateSample : MonoBehaviour
{
    State idleState;

    void Start()
    {
        idleState = new State(

            // Enter
            [
                OnEnter,
                PlayIdleAnimation
            ],

            // Update
            [
                IdleUpdate,
                CheckInput
            ],

            // Exit
            [
                OnExit,
                StopAnimation
            ]
        );

        idleState.Enter();
    }

    void Update()
    {
        idleState.Update();
    }

    // =========================
    // Enter
    // =========================

    void OnEnter()
    {
        Debug.Log("Enter Idle");
    }

    void PlayIdleAnimation()
    {
        Debug.Log("Play Idle Animation");
    }

    // =========================
    // Update
    // =========================

    void IdleUpdate()
    {
        Debug.Log("Idle Updating");
    }

    void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space Pressed");
        }
    }

    // =========================
    // Exit
    // =========================

    void OnExit()
    {
        Debug.Log("Exit Idle");
    }

    void StopAnimation()
    {
        Debug.Log("Stop Animation");
    }

    void OnDestroy()
    {
        idleState.Exit();
    }
}
```
### StateMachineの実装について
> StateMachine：IStateProvider連携、Stateの実行のみ
``` csharp
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
```
### IStateProviderの実装について
> IStateProvider：StateMachineとの連携、Stateを渡すのを保証、判断用のMonoBehaviourに付けるといいかも
``` csharp
public interface IStateProvider
{
    State ProvideState();
}
```
> IStateProviderの使用例
``` csharp
using UnityEngine;

public class EnemyAI : MonoBehaviour, IStateProvider
{
    [Header("References")]
    public MoveAction move;
    public StatusAction status;
    public AnimationAction anim;

    [Header("States")]
    public State idleState;
    public State moveState;
    public State lowHpState;

    void Start()
    {
        idleState = new State(
            new[] { anim.PlayIdle },
            new[] { () => { } },
            new[] { () => { } }
        );

        moveState = new State(
            new[] { anim.PlayMove },
            new[] { () => move.MoveToTarget(transform) },
            new[] { () => { } }
        );

        lowHpState = new State(
            new[] { anim.PlayIdle },
            new[] { () => Debug.Log("Low HP Escape") },
            new[] { () => { } }
        );
    }

    public State ProvideState()
    {
        // ① HPが低いなら最優先
        if (status.IsLowHP())
        {
            return lowHpState;
        }

        // ② 移動条件（仮：ターゲットあり）
        if (move.target != null)
        {
            return moveState;
        }

        // ③ それ以外はIdle
        return idleState;
    }
}

public class MoveAction : MonoBehaviour
{
    public Transform target;

    public void MoveToTarget(Transform self)
    {
        if (target == null) return;

        self.position = Vector3.MoveTowards(
            self.position,
            target.position,
            Time.deltaTime * 3f
        );
    }
}

public class StatusAction : MonoBehaviour
{
    public float hp = 100;

    public bool IsLowHP()
    {
        return hp < 30f;
    }
}

public class AnimationAction : MonoBehaviour
{
    public void PlayIdle()
    {
        Debug.Log("Play Idle Animation");
    }

    public void PlayMove()
    {
        Debug.Log("Play Move Animation");
    }
}
```