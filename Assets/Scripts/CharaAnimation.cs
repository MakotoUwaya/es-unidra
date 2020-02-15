using UnityEngine;

public class CharaAnimation : MonoBehaviour
{
    Animator animator;
    CharacterStatus status;
    Vector3 prePosition;
    bool isDown = false;
    bool attacked = false;

    public bool IsAttacked()
    {
        return this.attacked;
    }

    void StartAttackHit()
    {
        // Debug.Log("StartAttackHit");
    }

    void EndAttackHit()
    {
        // Debug.Log("EndAttackHit");
    }

    void EndAttack()
    {
        this.attacked = true;
    }

    void Start()
    {
        this.animator = this.GetComponent<Animator>();
        this.status = this.GetComponent<CharacterStatus>();

        this.prePosition = this.transform.position;
    }

    void Update()
    {
        var delta_position = this.transform.position - this.prePosition;
        this.animator.SetFloat("Speed", delta_position.magnitude / Time.deltaTime);

        if (this.attacked && !this.status.attacking)
        {
            this.attacked = false;
        }
        this.animator.SetBool("Attacking", (!this.attacked && this.status.attacking));

        if (!this.isDown && this.status.died)
        {
            this.isDown = true;
            this.animator.SetTrigger("Down");
        }

        this.prePosition = this.transform.position;
    }
}