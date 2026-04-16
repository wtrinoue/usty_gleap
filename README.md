# ★ゲーム開発目標★
- **0211:後期開発発表**
  - 根幹となるコンセプトの実装（完）
- **春休み期間**
  - 休んだ
- **2026年前期**  
  - 後期に開発が進められるように勉強したい
- **2026年後期**
  - 発表では前回に比べてさらに良くする

# ★ゲーム開発概要★
### 目次
1. コンセプト、ストーリー
1. 設計理念について
1. BuffとEffectの設計
1. WeaponCoreの設計（これから追加する）
1. 操作の設計
1. ディレクトリ構成
1. PlayerManagerの実装
1. 実行プロセスの集約化（もうちょい詳しく例となるコード入れよう）
1. 必須コンポーネント自動追加　※できればの範囲
1. Animationの実装（後でまとめる）
## コンセプト、ストーリー
今回のゲームは、ダダサバイバーのような見下ろし方2Dアクションゲームをベースとして、一般的な死の概念を覆したゲームを作成する。具体的には、Playerの体力がなくなったらGAMEOVERではなく、Graveというお墓を生成してステージを進むという感じになる。  
舞台は中世で、主人公は勇者である。
## 設計理念について
今回の開発では、すべてGameObjectにStatusHolder、StatusActionHolder、StatusManagerをアタッチすることによって、共通した汎用性の高い実装を試みた。目的としてはEnemyもPlayerもGraveも同じようにStatusのやり取りをできるようにすることで、多種多様なゲーム実装を可能にするためだ。
- **StatusHolderの役割**  
ScriptableObjectを利用して、オブジェクトごとにStatusを作成
。基本となるBaseStatusと、基本のバフとなるBuffStatusを保持。

- **StatusManagerの役割**  
Statusを有するすべてのGameObjectにアタッチするコンポーネント。Statusを用いた計算ルールを実装する。取得には(baseStatus + addStatus)*multipleStatusのように式構造によって計算・出力し、ダメージや回復の計算も行う。また、バフやエフェクトの適用も行う。

- **StatusActionHolderの役割**  
StatusActionHolderにはStatusActionを格納する。StatusActionには３パターンあり、TargetStatusActionとSelfStatusActionとGenerateActionである。また、StatusActionとはStatusを持つ者同士の数値のやり取りの約束事である。
    - **TargetStatusAction**  
    自身のStatusManagerから必要な値の取得、相手のStatusMangerから値変更メソッドを使い、Statusを変更。  
    例） TargetStatusAction.Execute(gameObject, other.gameObject);

    - **SelfStatusAction**  
    自身のStatusMangerを用いた処理。  
    例） SelfStatusAction.Execute(gameObject);

    - **GenerateAction**  
    オブジェクトを生成するための処理。自分自身の位置を用いて処理。  
    例)  GenerateAction.Execute(gameObject);

以下のコンポーネントは必ずアタッチする。  

<img width="304" height="68" alt="image" src="https://github.com/user-attachments/assets/9eba275e-ccae-41b4-a590-19cdd77b363e" />

また、StatusHolderには以下のBaseStatusとBuffStatusをCreate/Statusから作成し、インスペクターにドロップする。基本的にはオブジェクトごとに作るが、Statusを共有したい場合は同じStatusを入れてもいい。

<img width="431" height="61" alt="image" src="https://github.com/user-attachments/assets/0fcba4b4-8438-4db5-8950-5de9806ecb9b" />
<img width="593" height="164" alt="image" src="https://github.com/user-attachments/assets/e0100c16-ccf1-46d2-8f81-25c9806ef6de" />

StatusActionHolderにはCreate/TargetStatusAction、SelfStatusAction、GenerateActionのいずれかを作成してセットする。Actionの中にはインスペクターで値を指定しなくてもいいものもあるので、その場合は共通のアクションを用いる。

<img width="245" height="193" alt="image" src="https://github.com/user-attachments/assets/82beb32a-b528-4440-a418-6f2ff88809fe" />
<img width="432" height="280" alt="image" src="https://github.com/user-attachments/assets/042addd5-4970-4f5e-b78b-f6fb11285627" />





## BuffとEffectの設計
バフとエフェクトは以下のように定義する。
- **Buff**  
Buffとは、GameObjectのBuffStatusに一時的な値の変化を与えるための要素である。一時的にBuffStatusを合算することで実装する。また、抽象クラスのBuffを継承したScriptableObjectクラスのメソッドをoverrideすることで、合算後にBuffStatusを操作する支配的なBuffも適用できる。

- **Effect**  
Effectとは、GameObjectのStatusに一時的な変化を与える要素である。例えば、毒の実装では、等間隔でダメージを与えるという形で実装する。バフがBuffStatusなら、エフェクトはBaseStatusへの効果と考えてほしい。

また、以下二つのルールをバフとエフェクトに適用する。
- **同一効果には排他的である**  
効果を与えたオブジェクトが同じ、効果が同じ。これら二つを満たしている効果は重複して存在しない。メソッド内でバリデーションにより防いでいる。

- **持続時間**  
durationとして効果時間を定義し、この時間分効果は持続する。

- **発動間隔**  
staticIntervalで、durationの持続時間の中でどのくらいの間隔で効果が発動するか決める。0の場合は、効果時間の中でずっと発動していることになる。
## WeaponCoreの設計
<img width="240" height="350" alt="weaponcore" src="https://github.com/user-attachments/assets/410db7ce-3da9-4e1d-8bf2-1355392033f4" />  

### 説明
- WeaponCoreは、敵にダメージやバフなどの効果を与えるオブジェクトを生成・管理する役割を持つ。
PlayerはWeaponCoreを呼び出すだけに責務を限定し、具体的な攻撃処理はすべてWeaponCoreに委ねる。
これによりPlayerの実装はシンプルに保たれ、武器の追加もWeaponCoreを拡張するだけで対応できる。
## 操作の設計
PlayerActionは以下のPC操作に対応して動作する。
- ### 操作方法  
    - **WASD**  
    Playerの移動

    - **マウス(左クリック)**  
    武器の使用

    - **マウス(ホイール)**  
    武器の切り替え

    - **スペース**  
    自爆 and リスポーン
## ディレクトリ構成
ディレクトリは「機能 => ファイル形式 => 意味」の順で構成する。以下に例を示す。

<img width="332" height="637" alt="image" src="https://github.com/user-attachments/assets/0aa4b000-f3e1-4bd5-b30d-82b74749e548" />

上の場合、機能として最低限分解したディレクトリ構成は以下のようになる。  

<img width="255" height="235" alt="image" src="https://github.com/user-attachments/assets/654a08b7-6ce1-4aa9-b59f-044c9143b222" />

- ### 機能の分類例  
  - キャラクター  
   Player、Enemy、Grave、Item
  - システム  
   WaveSystem、Spawner

## PlayerManagerの実装
PlayerManagerをシーン上に配置し、Playerが自身の存在を登録・解除、他のオブジェクトがPlayerManagerを通してオブジェクトの存在を取得することで、Findtagなどの重い処理を回避する。  
1. **PlayerManagerをシーンのHierarchyに配置する**
<img width="283" height="43" alt="image" src="https://github.com/user-attachments/assets/f8e07f17-d72f-439c-918e-af996aba4cc7" />

1. **PlayerManagerのインスタンスを取得してPlayerをセット（例）**
```csharp
    private Transform player;

    ///他の処理

    private void SetPlayer(){
        if (player == null){
          if(PlayerManager.Instance == null || PlayerManager.Instance.CurrentPlayer == null) return;
          player = PlayerManager.Instance.CurrentPlayer;
          return;
        }
    }
```

## 実行プロセスの集約化
　おそらくフレームごとの実装にはUpdate、一定時間の間隔での実装にはCoroutineを使っているが、各クラスに定義すると並列で処理しなければならないのでオーバーヘッドが増えて、オブジェクトの数だけ負荷が増加する。そのために、UpdateとCoroutineを実行するのは一か所にして、繰り返し処理をしたいオブジェクトはそこに関数を登録する形で処理する。

## 必須コンポーネント自動追加　※できればの範囲
```csharp
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]// ←これで勝手につく
public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
}
```
## Animationの実装
### Animationの仕組み

AnimationControllerでAnimationの遷移図を作ることで実装できる。リアルタイムでのAnimationはその時の状態で確定する。  
<img width="959" height="521" alt="image" src="https://github.com/user-attachments/assets/994d2c30-1393-4dbe-8dd6-2f641ee633d9" />

ちなみに今回は**定常状態**に必ず**回帰**するようにする発想で作った。他にもいろいろな遷移のさせ方があるが、状況に応じて組んでほしい。  
<img width="2482" height="1898" alt="statetransition" src="https://github.com/user-attachments/assets/771c337e-6401-46e7-acce-2283bef13a3c" />



- **Entry**  
  シーンが再生されたときに初めに持つStateで、ここから次の状態へと遷移していく。
- **AnyState**  
  これはリアルタイムでの現在の状態を指す。例えば、現時点でIdleでHurtのTriggerで遷移した場合は*AnyState(Idle)->Hurt*となる。また、現時点でAttackでHurtのTriggerで遷移した場合は*AnyState(Attack)->Hurt*となる。
- **Transition**  
  ある状態から次の状態への遷移先で、条件を付与することによって遷移できるかどうか切り替えられる。
  - _Trigger_  
    Transitionの遷移を一度だけ許可する。boolを一度だけtrueにしてからすぐにfalseになるような動作となる。
  - _Bool_  
    boolは**Transitionの遷移を許す**という動作。conditionで真偽値がどのような値で許可するか決められる。

※Transitionの遷移の仕方によってはAnimationが切り替わらないという詰みの構成になる可能性があるので注意。

### ディレクトリ構成

Animationは単なる動きに過ぎないので共通部品として絵が同じものを種類としてまとめる。しかし、AnimationControllerはキャラごとに動きが異なるのでキャラごとにそれぞれ作る。なので、AnimationをUnityAssetStoreからダウンロードするときのフォルダ構成はそのままにしてもらってもいい。

- Animationは共通化する
- AnimationControllerは機能分割する（キャラごとに）

### Animationのつけ方

1. 既存のステータスや移動が書いてあるスクリプトを見て、どんな種類のモーションに分けられるかを考える。
1. モーションに分けられたら、それに対応したAnimationControllerを作成する。
1. AnimationControllerを作成してTransitionが確定したら、それを切り替えるためのAnimationScriptを作成する
1. AnimationControllerに適切なAnimationをセットする。
1. **「データ処理はそのままで、納得するAnimation動作に仕上げる」** これを最終目標にして修正していってください。

**FollowEnemyでの実装例**

- AnimationControllerの構成をもとにそれぞれのAnimationへの遷移に対応したスクリプトを作成する。

```csharp
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FollowEnemyAnimation : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Idle()
    {
        animator.SetBool("isRun", false);
    }
    public void Run()
    {
        animator.SetBool("isRun", true);
    }
    public void Attack()
    {
        animator.SetTrigger("Attack");
    }
    public void Death()
    {
        animator.SetTrigger("Death");
    }
    public void Hurt()
    {
        animator.SetTrigger("Hurt");
    }
}
```

- Animationのスクリプトが書き終えたところで、キャラの動作やシステムが書かれているスクリプトにAnimation用のメソッドを入れ込む。※Animationを始め入れるとかなり書き直しや新しい処理を加えないといけないので、上記のようなスクリプトができたら後ほど統合する。
