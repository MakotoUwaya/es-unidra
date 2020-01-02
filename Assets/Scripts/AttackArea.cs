using UnityEngine;

public class AttackArea : MonoBehaviour
{
    CharacterStatus status;

    void Start()
    {
        this.status = this.transform.root.GetComponent<CharacterStatus>();
    }

    public class AttackInfo
    {
        public int attackPower; // この攻撃の攻撃力.
        public Transform attacker; // 攻撃者.
    }

    // 攻撃情報を取得する.
    AttackInfo GetAttackInfo()
    {
        var attackInfo = new AttackInfo
        {
            // 攻撃力の計算
            attackPower = this.status.Power,
            attacker = this.transform.root
        };

        return attackInfo;
    }

    // 当たった.
    void OnTriggerEnter(Collider other)
    {
        // 攻撃が当たった相手のDamageメッセージをおくる.
        other.SendMessage("Damage", this.GetAttackInfo());
        // 攻撃した対象を保存.
        this.status.lastAttackTarget = other.transform.root.gameObject;
    }

    // 攻撃判定を有効にする.
    void OnAttack()
    {
        var collider = this.transform.root.GetComponent<Collider>();
        collider.enabled = true;
    }

    // 攻撃判定を無効にする.
    void OnAttackTermination()
    {
        var collider = this.transform.root.GetComponent<Collider>();
        collider.enabled = false;
    }
}
