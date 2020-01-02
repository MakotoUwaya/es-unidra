using UnityEngine;

public class EnemyCtrl : MonoBehaviour
{
    GameRuleCtrl gameRuleCtrl;
    CharacterStatus status;
    CharaAnimation charaAnimation;
    CharacterMove characterMove;
    Transform attackTarget;

    public float waitBaseTime = 2.0f;
    float waitTime;
    public float walkRange = 5.0f;
    public Vector3 basePosition;
    public GameObject[] dropItemPrefab;

    enum State
    {
        Walking,	// 探索
        Chasing,	// 追跡
        Attacking,	// 攻撃
        Died,       // 死亡
    };

    State state = State.Walking;
    State nextState = State.Walking;

    void Start()
    {
        this.gameRuleCtrl = FindObjectOfType<GameRuleCtrl>();
        this.status = this.GetComponent<CharacterStatus>();
        this.charaAnimation = this.GetComponent<CharaAnimation>();
        this.characterMove = this.GetComponent<CharacterMove>();
        // 初期位置を保持
        this.basePosition = this.transform.position;
        // 待機時間
        this.waitTime = this.waitBaseTime;
    }

    void Update()
    {
        switch (this.state)
        {
            case State.Walking:
                this.Walking();
                break;
            case State.Chasing:
                this.Chasing();
                break;
            case State.Attacking:
                this.Attacking();
                break;
        }

        if (this.state != this.nextState)
        {
            this.state = this.nextState;
            switch (this.state)
            {
                case State.Walking:
                    this.WalkStart();
                    break;
                case State.Chasing:
                    this.ChaseStart();
                    break;
                case State.Attacking:
                    this.AttackStart();
                    break;
                case State.Died:
                    this.Died();
                    break;
            }
        }
    }


    // ステートを変更する.
    void ChangeState(State nextState)
    {
        this.nextState = nextState;
    }

    void WalkStart()
    {
        this.StateStartCommon();
    }

    void Walking()
    {
        // 待機時間がまだあったら
        if (this.waitTime > 0.0f)
        {
            // 待機時間を減らす
            this.waitTime -= Time.deltaTime;
            // 待機時間が無くなったら
            if (this.waitTime <= 0.0f)
            {
                // 範囲内の何処か
                var randomValue = Random.insideUnitCircle * this.walkRange;
                // 移動先の設定
                var destinationPosition = this.basePosition + new Vector3(randomValue.x, 0.0f, randomValue.y);
                //　目的地の指定.
                this.SendMessage("SetDestination", destinationPosition);
            }
        }
        else
        {
            // 目的地へ到着
            if (this.characterMove.Arrived())
            {
                // 待機状態へ
                this.waitTime = Random.Range(this.waitBaseTime, this.waitBaseTime * 2.0f);
            }
            // ターゲットを発見したら追跡
            if (this.attackTarget)
            {
                this.ChangeState(State.Chasing);
            }
        }
    }
    // 追跡開始
    void ChaseStart()
    {
        this.StateStartCommon();
    }
    // 追跡中
    void Chasing()
    {
        // 移動先をプレイヤーに設定
        this.SendMessage("SetDestination", this.attackTarget.position);
        // 2m以内に近づいたら攻撃
        if (Vector3.Distance(this.attackTarget.position, this.transform.position) <= 2.0f)
        {
            this.ChangeState(State.Attacking);
        }
    }

    // 攻撃ステートが始まる前に呼び出される.
    void AttackStart()
    {
        this.StateStartCommon();
        this.status.attacking = true;

        // 敵の方向に振り向かせる.
        var targetDirection = (this.attackTarget.position - this.transform.position).normalized;
        this.SendMessage("SetDirection", targetDirection);

        // 移動を止める.
        this.SendMessage("StopMove");
    }

    // 攻撃中の処理.
    void Attacking()
    {
        if (this.charaAnimation.IsAttacked())
        {
            this.ChangeState(State.Walking);
        }
        // 待機時間を再設定
        this.waitTime = Random.Range(this.waitBaseTime, this.waitBaseTime * 2.0f);
        // ターゲットをリセットする
        this.attackTarget = null;
    }

    void DropItem()
    {
        if (this.dropItemPrefab.Length == 0) 
        {
            return;
        }
        var dropItem = this.dropItemPrefab[Random.Range(0, this.dropItemPrefab.Length)];
        Instantiate(dropItem, this.transform.position, Quaternion.identity);
    }

    void Died()
    {
        this.status.died = true;
        this.DropItem();
        
        if (this.gameObject.CompareTag("Boss"))
        {
            this.gameRuleCtrl.GameClear();
        }
        Destroy(this.gameObject);
    }

    void Damage(AttackArea.AttackInfo attackInfo)
    {
        this.status.HP -= attackInfo.attackPower;
        if (this.status.HP <= 0)
        {
            this.status.HP = 0;
            // 体力０なので死亡
            this.ChangeState(State.Died);
        }
    }

    // ステートが始まる前にステータスを初期化する.
    void StateStartCommon()
    {
        this.status.attacking = false;
        this.status.died = false;
    }
    // 攻撃対象を設定する
    public void SetAttackTarget(Transform target)
    {
        this.attackTarget = target;
    }
}
