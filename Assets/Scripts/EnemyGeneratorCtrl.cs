using UnityEngine;
using System.Collections;
using MonobitEngine;

public class EnemyGeneratorCtrl : MonobitEngine.MonoBehaviour
{
    public GameObject enemy;
    // 敵を格納
    GameObject[] existEnemys;
    // アクティブの最大数
    public int maxEnemy = 2;

    void Start()
    {
        // 配列確保
        this.existEnemys = new GameObject[this.maxEnemy];
        // 周期的に実行したい場合はコルーチンを使うと簡単に実装できます。
        this.StartCoroutine(this.Exec());
    }

    // 敵を作成します
    IEnumerator Exec()
    {
        while (true)
        {
            if (MonobitNetwork.isHost)
            {
                this.Generate();
            }
            yield return new WaitForSeconds(3.0f);
        }
    }

    void Generate()
    {
        for (var enemyCount = 0; enemyCount < this.existEnemys.Length; ++enemyCount)
        {
            if (this.existEnemys[enemyCount] == null)
            {
                // 敵作成
                this.existEnemys[enemyCount] = MonobitNetwork.Instantiate(
                    this.enemy.name,
                    this.transform.position,
                    this.transform.rotation,
                    0,
                    null,
                    false,
                    false,
                    false
                );
                return;
            }
        }
    }

}
