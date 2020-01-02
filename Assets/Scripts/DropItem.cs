using UnityEngine;

public class DropItem : MonoBehaviour
{
    public enum ItemKind
    {
        Attack,
        Heal,
    };
    public ItemKind kind;

    void OnTriggerEnter(Collider other)
    {
        // Playerか判定
        if (other.CompareTag("Player"))
        {
            // アイテム取得
            var aStatus = other.GetComponent<CharacterStatus>();
            aStatus.GetItem(this.kind);
            // 取得したらアイテムを消す
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        var velocity = Random.insideUnitSphere * 2.0f + Vector3.up * 8.0f;
        var rigidbody = this.GetComponent<Rigidbody>();
        rigidbody.velocity = velocity;
    }
}
