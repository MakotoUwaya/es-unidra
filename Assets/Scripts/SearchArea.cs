using UnityEngine;

public class SearchArea : MonoBehaviour
{
    EnemyCtrl enemyCtrl;
    void Start()
    {
        // EnemyCtrlをキャッシュする
        this.enemyCtrl = this.transform.root.GetComponent<EnemyCtrl>();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            this.enemyCtrl.SetAttackTarget(other.transform);
        }
    }
}
