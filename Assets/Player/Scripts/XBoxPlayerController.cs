using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class XBoxPlayerController : MonoBehaviour, PlayerInputActions.IPlayerActions
{
    private Vector2 moveInput;
    private Vector2 lookInput;

    private PlayerInputActions inputActions;

    private WeaponCoreHolder weaponCoreHolder;
    private List<GameObject> weaponCoreList;

    private GraveHolder graveHolder;
    private List<GameObject> graves;

    private PlayerMove playerMove;


    private int currentWeaponCoreIndex = 0;

    private bool isDestroyed = false;
    private bool isInitialized = false;

    void Awake()
    {
        Debug.Log("Playerを初期化した");
        inputActions = new PlayerInputActions();
        inputActions.Player.AddCallbacks(this);

        playerMove = GetComponent<PlayerMove>();

        weaponCoreHolder = GetComponent<WeaponCoreHolder>();
        weaponCoreList = weaponCoreHolder.GetWeaponCoreList();

        graveHolder = GetComponent<GraveHolder>();
        graves = graveHolder.GetGraves();
        isInitialized = true;
    }
    void OnEnable()
    {
        if (inputActions != null)
            inputActions.Player.Enable();
    }
    void OnDisable()
    {
        if (inputActions != null)
        {
            inputActions.Player.RemoveCallbacks(this);
            inputActions.Player.Disable();
        }
    }

    // =========================
    // 🎮 Move（移動）
    // =========================
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (playerMove != null)
            playerMove.SetMoveInput(moveInput);
    }

    // =========================
    // 👀 Look（照準）
    // =========================
    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    // =========================
    // 🔥 Attack（武器発射）
    // =========================
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!isInitialized) return;
        if (isDestroyed) return;
        if (!context.performed) return;

        if (weaponCoreList == null || weaponCoreList.Count == 0)
            weaponCoreList = weaponCoreHolder.GetWeaponCoreList();

        if (weaponCoreList.Count == 0) return;

        // Xboxはマウスがないので lookInput or forward を使用
        Vector3 direction;

        if (lookInput != Vector2.zero)
        {
            direction = -new Vector3(lookInput.x, lookInput.y, 0f).normalized;
        }
        else
        {
            direction = transform.up;
        }

        // 武器を取得する。
        weaponCoreList ??= weaponCoreHolder.GetWeaponCoreList();
        if(weaponCoreList.Count == 0) return;

        GameObject clone = Instantiate(
            weaponCoreList[currentWeaponCoreIndex],
            transform.position,
            Quaternion.identity
        );

        clone.transform.SetParent(transform);
        clone.transform.up = direction;
    }

    // =========================
    // ☠ Self Destruction（自爆）
    // =========================
public void OnSelfDistruction(InputAction.CallbackContext context)
{
    if (!isInitialized) return;

    Debug.Log($"graveHolder: {graveHolder}, graves: {graves}");

    if (isDestroyed)
    {
        Debug.Log("破壊されています");
        return;
    }

    if (!context.performed)
    {
        Debug.Log("SelfDestruction input not performed");
        return;
    }

    // graves と graveHolder が初期化されているか確認
    if (graves == null)
    {
        graves = graveHolder.GetGraves();
    }

    Debug.Log("SelfDestructionが動作します");

    if (graves.Count == 0)
    {
        DestroyMe();
        return;
    }

    // 自分の位置に生成
    Instantiate(graves[0], transform.position, Quaternion.identity);
    graveHolder.RemoveFirstGrave();

    // 自分を破壊
    DestroyMe();
}

    // =========================
    // 🔁 武器切り替え
    // =========================
    public void OnChangeWeapon(InputAction.CallbackContext context)
    {
        if (isDestroyed) return;
        if (!isInitialized) return;
        if (!context.performed) return;

        if (weaponCoreList == null || weaponCoreList.Count == 0)
            weaponCoreList = weaponCoreHolder.GetWeaponCoreList();

        if (weaponCoreList.Count == 0) return;

        // 🔁 ループで次へ
        currentWeaponCoreIndex++;

        if (currentWeaponCoreIndex >= weaponCoreList.Count)
            currentWeaponCoreIndex = 0;
    }

    // =========================
    // 💀 Grave切り替え＆使用
    // =========================
    public void OnChangeGrave(InputAction.CallbackContext context)
    {
        if(true)return;
        if (isDestroyed) return;
        if (!context.performed) return;

        if (graves == null)
            graves = graveHolder.GetGraves();

        if (graves.Count == 0)
        {
            DestroyMe();
            return;
        }

        Instantiate(graves[0], transform.position, Quaternion.identity);
        graveHolder.RemoveFirstGrave();

        DestroyMe();
    }

    // =========================
    // ☠ Destroy
    // =========================
    private void DestroyMe()
    {
        if (isDestroyed) return;
        isDestroyed = true;
        Destroy(gameObject);
    }
}