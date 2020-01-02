using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    const float RayCastMaxDistance = 100.0f;
    GameRuleCtrl gameRuleCtrl;
    CharacterStatus status;
    CharaAnimation charaAnimation;
    Transform attackTarget;
    InputManager inputManager;
    public float attackRange = 1.5f;
    State state = State.Walking;
    State nextState = State.Walking;

    enum State
    {
        Walking,
        Attacking,
        Died
    }

    void Start()
    {
        this.gameRuleCtrl = FindObjectOfType<GameRuleCtrl>();
        this.status = this.GetComponent<CharacterStatus>();
        this.charaAnimation = this.GetComponent<CharaAnimation>();
        this.inputManager = FindObjectOfType<InputManager>();

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
                    this.SendMessage("SetDestination", hitInfo.point);
                }
                else if (layer == LayerMask.NameToLayer("EnemyHit"))
                {
                    var hitPoint = hitInfo.point;
                    hitPoint.y = this.transform.position.y;
                    var distance = Vector3.Distance(hitPoint, this.transform.position);
                    if (distance < this.attackRange)
                    {
                        this.attackTarget = hitInfo.collider.transform;
                        this.ChangeState(State.Attacking);
                    }
                    else
                    {
                        this.SendMessage("SetDestination", hitInfo.point);
                    }
                }
            }
        }
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
    }

    void Damage(AttackArea.AttackInfo attackInfo)
    {
        this.status.HP -= attackInfo.attackPower;
        if (this.status.HP <= 0)
        {
            this.status.HP = 0;
            this.ChangeState(State.Died);
        }
    }

    void StateStartCommon()
    {
        this.status.attacking = false;
        this.status.died = false;
    }
}
