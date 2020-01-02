using UnityEngine;

public class CharacterStatus : MonoBehaviour
{
    // 体力
    public int HP = 100;
    public int MaxHP = 100;

    // 攻撃力
    public int Power = 10;

    // 最後に攻撃した対象
    public GameObject lastAttackTarget = null;

    // プレイヤー名.
    public string characterName = "Player";

    //状態
    public bool attacking = false;
    public bool died = false;

    public bool powerBoost = false;
    float powerBoostTime = 0.0f;

    public void GetItem(DropItem.ItemKind itemKind)
    {
        switch (itemKind)
        {
            case DropItem.ItemKind.Attack:
                this.powerBoostTime = 5.0f;
                break;
            case DropItem.ItemKind.Heal:
                // MaxHPの半分回復
                this.HP = Mathf.Min(this.HP + this.MaxHP / 2, this.MaxHP);
                break;
        }
    }

    void Update()
    {
        this.powerBoost = false;
        if (this.powerBoostTime > 0.0f)
        {
            this.powerBoost = true;
            this.powerBoostTime = Mathf.Max(this.powerBoostTime - Time.deltaTime, 0.0f);
        }
    }
}
