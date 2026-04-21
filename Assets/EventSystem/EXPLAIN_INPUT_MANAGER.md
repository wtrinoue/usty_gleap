# Unity InputManager イベント駆動設計（Invoke方式）

## ■概要

Unityにおける入力管理を「イベント駆動（Push型）」で設計する方式。  
InputManagerが入力を検知し、イベント（Invoke）で各システムへ通知する。

---

## ■設計思想

### ❌ 従来（Pull型）

- Playerが毎フレームInputManagerを監視
- 依存が強い
- 拡張性が低い

---

### ✔ 新方式（Push型 / イベント駆動）

- InputManagerが入力を検知
- 発生した瞬間にイベントを通知（Invoke）
- 各システムは受け取るだけ

---

## ■構成

InputManager（シングルトン）
↓ イベント発火
Player / UI / Audio / Effect
↓ 受け取り
各処理を実行

---

## ■InputManager実装

```csharp
using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    public event Action OnAttack;
    public event Action<Vector2> OnMove;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        Vector2 move = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        );

        if (move != Vector2.zero)
            OnMove?.Invoke(move);

        if (Input.GetKeyDown(KeyCode.Space))
            OnAttack?.Invoke();
    }
}
```

## ■PlayerController(受け取り側)の実装

```csharp
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    void OnEnable()
    {
        InputManager.Instance.OnAttack += Attack;
        InputManager.Instance.OnMove += Move;
    }

    void OnDisable()
    {
        InputManager.Instance.OnAttack -= Attack;
        InputManager.Instance.OnMove -= Move;
    }

    void Attack()
    {
        Debug.Log("Attack!");
    }

    void Move(Vector2 dir)
    {
        transform.position += (Vector3)dir * Time.deltaTime;
    }
}
```
