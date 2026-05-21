using System;
using System.Collections.Generic;

public class State
{
    private readonly Action[] EnterActions;
    private readonly Action[] UpdateActions;
    private readonly Action[] ExitActions;
    private readonly Interval[] Intervals; // インターバル処理を保持する配列を追加

    // StateBuilder から呼ばれる新しいコンストラクタ
    public State(Action[] enterActions, Action[] updateActions, Action[] exitActions, Interval[] intervals)
    {
        EnterActions = enterActions;
        UpdateActions = updateActions;
        ExitActions = exitActions;
        Intervals = intervals;
    }

    public void Enter()
    {
        // 状態進入時にインターバルのタイマーをリセット
        foreach (var interval in Intervals)
        {
            interval?.Reset();
        }

        foreach (var action in EnterActions)
        {
            action?.Invoke();
        }
    }

    // 毎フレーム呼び出す Update。Unity で使う場合は引数に Time.deltaTime を渡せるようにします
    public void Update(float deltaTime)
    {
        // 定期実行（インターバル）のタイマーを進める
        foreach (var interval in Intervals)
        {
            interval?.Tick(deltaTime);
        }

        // 通常の毎フレーム更新処理
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

public class Interval
{
    private readonly float interval;
    private float timer;

    private readonly Action[] actions;

    public Interval(float interval, params Action[] actions)
    {
        this.interval = interval;
        this.actions = actions;
        this.timer = 0f;
    }

    public void Tick(float dt)
    {
        timer += dt;

        if (timer >= interval)
        {
            timer -= interval;

            foreach (var a in actions)
                a?.Invoke();
        }
    }

    public void Reset()
    {
        timer = 0f;
    }
}

public class StateBuilder
{
    private readonly List<Action> enter = new();
    private readonly List<Action> update = new();
    private readonly List<Action> exit = new();
    private readonly List<Interval> intervals = new();

    public StateBuilder Enter(params Action[] actions)
    {
        enter.AddRange(actions);
        return this;
    }

    public StateBuilder Update(params Action[] actions)
    {
        update.AddRange(actions);
        return this;
    }

    public StateBuilder Exit(params Action[] actions)
    {
        exit.AddRange(actions);
        return this;
    }

    public StateBuilder Interval(Interval interval)
    {
        intervals.Add(interval);
        return this;
    }

    public State Build()
    {
        return new State(
            enter.ToArray(),
            update.ToArray(),
            exit.ToArray(),
            intervals.ToArray()
        );
    }
}