using UnityEngine;

[RequireComponent(typeof(FollowEnemyAnimation))]
public class EnemyFollow : MonoBehaviour
{
  private Transform player; //playerのTransform
  public float speed = 4f; //敵の移動速度
  public float stopDistance = 1f;
  private bool isMove = true;
  private bool isDead = false;
  private float pastHp = 11f;
  private StatusManager statusManager;
  private StatusActionHolder statusActionHolder;
  private FollowEnemyAnimation animation;
  private GenerateGrave generateGrave;
  private TargetStatusAction attackAction;
  void Start()
  {
    statusManager = GetComponent<StatusManager>();
    generateGrave = GetComponent<GenerateGrave>();
    statusActionHolder = GetComponent<StatusActionHolder>();
    attackAction = statusActionHolder.GetTargetStatusActionFromIndex(0);
    player = PlayerManager.Instance.CurrentPlayer;
    animation = GetComponent<FollowEnemyAnimation>();
  }

  void Update()
  {
    Move();
    CheckHpAndDeath();
    CheckHpAndHurt();
  }

  public void Move()
  {
    if(!isMove) return;
    if(isDead) return;
    speed = statusManager.GetSpeed();
    if (player == null){
      animation.Idle();
      if(PlayerManager.Instance == null || PlayerManager.Instance.CurrentPlayer == null) return;
      player = PlayerManager.Instance.CurrentPlayer;
      return;
    }
    animation.Run();

    //playerの方向を取得
    Vector3 direction = (player.position - transform.position).normalized;

    float distance = Vector3.Distance(player.position, transform.position);

    if(distance > stopDistance){
      //方向に向かって移動
      transform.position += direction * speed * Time.deltaTime;
    }
  }

  public void Attack(GameObject target)
  {
      animation.Attack();
      attackAction.Execute(gameObject, target);
      animation.Idle(); // 例えば待った後にIdleに戻す
  }

  public void CheckHpAndDeath()
  {
    if(isDead) return;
    float CurrentHP = statusManager.BaseStatus.CurrentHP;
    if(CurrentHP <= 0)
    {
      isMove = false;
      isDead = true;
      animation.Death();
      Destroy(this.gameObject, 2f);
    }
  }

  public void CheckHpAndHurt()
  {
    if(isDead) return;
    Debug.Log(statusManager.BaseStatus.CurrentHP);
    if(statusManager.BaseStatus.CurrentHP < pastHp)
    {
      Hurt();
    }
  }

  public void Hurt()
  {
    isMove = false;
    animation.Hurt();
    pastHp = statusManager.BaseStatus.CurrentHP;
    isMove = true;
  }

  void OnCollisionEnter2D(Collision2D collision)
  {
    if(!collision.gameObject.CompareTag("Player"))
        return;

    if(collision.gameObject == null)
        return;

    Debug.Log("ぶつかりました！");
    Attack(collision.gameObject);
  }
}
