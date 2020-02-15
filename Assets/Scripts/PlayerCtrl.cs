using UnityEngine;
using MonobitEngine;

public class PlayerCtrl : MonobitEngine.MonoBehaviour
{
    const float RayCastMaxDistance = 100.0f;
    GameRuleCtrl gameRuleCtrl;
    CharacterStatus status;
    CharaAnimation charaAnimation;
    Transform attackTarget;
    InputManager inputManager;
    public float attackRange = 1.5f;
    public GameObject hitEffect;
    TargetCursor targetCursor;
    public AudioClip deathSeClip;
    AudioSource deathSeAudio;
    State state = State.Walking;
    State nextState = State.Walking;

    MonobitView playerMonobitView;

    enum State
    {
        Walking,
        Attacking,
        Died
    }

    private void Awake()
    {
        // すべての親オブジェクトに対して MonobitView コンポーネントを検索する
        if (this.GetComponentInParent<MonobitView>() != null)
        {
            this.playerMonobitView = this.GetComponentInParent<MonobitView>();
        }
        // 親オブジェクトに存在しない場合、すべての子オブジェクトに対して MonobitView コンポーネントを検索する
        else if (this.GetComponentInChildren<MonobitView>() != null)
        {
            this.playerMonobitView = this.GetComponentInChildren<MonobitView>();
        }
        // 親子オブジェクトに存在しない場合、自身のオブジェクトに対して MonobitView コンポーネントを検索して設定する
        else
        {
            this.playerMonobitView = this.GetComponent<MonobitView>();
        }
    }

    void Start()
    {
        this.gameRuleCtrl = FindObjectOfType<GameRuleCtrl>();
        this.status = this.GetComponent<CharacterStatus>();
        this.charaAnimation = this.GetComponent<CharaAnimation>();
        this.inputManager = FindObjectOfType<InputManager>();
        this.targetCursor = FindObjectOfType<TargetCursor>();
        this.targetCursor.SetPosition(this.transform.position);

        this.deathSeAudio = this.gameObject.AddComponent<AudioSource>();
        this.deathSeAudio.clip = this.deathSeClip;
        this.deathSeAudio.loop = false;

        // Assert
        if (this.gameRuleCtrl == null)
        {
            Debug.LogError("Not found 'GameRuleCtrl' object.");
        }
        if (this.status == null)
        {
            Debug.LogError("Not found 'CharacterStatus' component.");
        }
        if (this.charaAnimation == null)
        {
            Debug.LogError("Not found 'CharaAnimation' component.");
        }
        if (this.inputManager == null)
        {
            Debug.LogError("Not found 'InputManager' object.");
        }
    }

    void Update()
    {
        if (!this.playerMonobitView.isMine)
        {
            return;
        }

        switch (this.state)
        {
            case State.Walking:
                this.Walking();
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
                case State.Attacking:
                    this.AttackStart();
                    break;
                case State.Died:
                    this.Died();
                    break;
            }
        }

    }
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
        if (this.inputManager.Clicked())
        {
            var clickPos = this.inputManager.GetCursorPosition();
            // RayCastで対象物を調べる
            var ray = Camera.main.ScreenPointToRay(clickPos);
            if (Physics.Raycast(ray, out var hitInfo, RayCastMaxDistance, 
                1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("EnemyHit")))
            {
                var layer = hitInfo.collider.gameObject.layer;
                if (layer == LayerMask.NameToLayer("Ground"))
                {
                    this.SetDestinationPoint(hitInfo.point);
                }
                else if (layer == LayerMask.NameToLayer("EnemyHit"))
                {
                    var hitPoint = hitInfo.point;
                    hitPoint.y = this.transform.position.y;
                    var distance = Vector3.Distance(hitPoint, this.transform.position);
                    if (distance < this.attackRange)
                    {
                        this.attackTarget = hitInfo.collider.transform;
                        this.targetCursor.SetPosition(this.attackTarget.position);
                        this.ChangeState(State.Attacking);
                    }
                    else
                    {
                        this.SetDestinationPoint(hitInfo.point);
                    }
                }
            }
        }
    }

    void SetDestinationPoint(Vector3 point)
    {
        this.SendMessage("SetDestination", point);
        this.targetCursor.SetPosition(point);
    }

    void AttackStart()
    {
        this.StateStartCommon();
        this.status.attacking = true;

        var targetDirection = (this.attackTarget.position - this.transform.position).normalized;
        this.SendMessage("SetDirection", targetDirection);

        this.SendMessage("StopMove");
    }

    void Attacking()
    {
        if (this.charaAnimation.IsAttacked())
        {
            this.ChangeState(State.Walking);
        }
    }

    void Died()
    {
        this.status.died = true;
        this.gameRuleCtrl.GameOver();

        this.deathSeAudio.Play();
    }

    void Damage(AttackArea.AttackInfo attackInfo)
    {
        var effect = Instantiate(this.hitEffect, this.transform.position, Quaternion.identity);
        effect.transform.localPosition = this.transform.position + new Vector3(0.0f, 0.5f, 0.0f);
        Destroy(effect, 0.3f);

        Debug.Log($"Player damage: {nameof(this.DamageMine)}");
        this.playerMonobitView.RPC(nameof(this.DamageMine), MonobitTargets.All, attackInfo.attackPower);
    }

    [MunRPC]
    void DamageMine(int damage)
    {
        Debug.Log($"Player DamageMine: {damage}");
        this.status.HP -= damage;
        if (this.status.HP <= 0)
        {
            this.status.HP = 0;
            // 体力０なので死亡
            this.ChangeState(State.Died);
        }
    }



    void StateStartCommon()
    {
        this.status.attacking = false;
        this.status.died = false;
    }
}
