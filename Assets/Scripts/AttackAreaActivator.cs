using UnityEngine;

public class AttackAreaActivator : MonoBehaviour
{
    Collider[] attackAreaColliders; // 攻撃判定コライダの配列.

    void Start()
    {
        // 子供のGameObjectからAttackAreaスクリプトがついているGameObjectを探す。
        var attackAreas = this.GetComponentsInChildren<AttackArea>();
        this.attackAreaColliders = new Collider[attackAreas.Length];

        for (var attackAreaCnt = 0; attackAreaCnt < attackAreas.Length; attackAreaCnt++)
        {
            // AttackAreaスクリプトがついているGameObjectのコライダを配列に格納する.
            this.attackAreaColliders[attackAreaCnt] = attackAreas[attackAreaCnt].GetComponent<Collider>();
            this.attackAreaColliders[attackAreaCnt].enabled = false;  // 初期はfalseにしておく.
        }
    }

    // アニメーションイベントのStartAttackHitを受け取ってコライダを有効にする
    void StartAttackHit()
    {
        foreach (var attackAreaCollider in this.attackAreaColliders)
        {
            attackAreaCollider.enabled = true;
        }
    }

    // アニメーションイベントのEndAttackHitを受け取ってコライダを無効にする
    void EndAttackHit()
    {
        foreach (var attackAreaCollider in this.attackAreaColliders)
        {
            attackAreaCollider.enabled = false;
        }
    }
}
